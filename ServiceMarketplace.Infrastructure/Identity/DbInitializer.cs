using Microsoft.AspNetCore.Identity;
using ServiceMarketplace.Domain.Entities;
using ServiceMarketplace.Infrastructure.Identity;

namespace ServiceMarketplace.Infrastructure.Identity;

public static class DbInitializer
{
    public static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
    {
        string[] roles = { "Admin", "Provider", "Customer" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    public static async Task SeedAdminUser(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager)
    {
        var email = "admin@site.com";
        var password = "P@ssw0rd";

        var user = await userManager.FindByEmailAsync(email);

        if (user == null)
        {
            var admin = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = "System Admin"
            };

            var result = await userManager.CreateAsync(admin, password);

            if (!result.Succeeded)
                throw new Exception("Failed to create admin user");

          
            await userManager.AddToRoleAsync(admin, "Admin");
        }
    }

    public static async Task SeedPermissions(
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext context)
    {
        if (!context.Permissions.Any())
        {
            var permissions = new List<Permission>
            {
                new Permission { Name = "request.create" },
                new Permission { Name = "request.accept" },
                new Permission { Name = "request.complete" }
            };

            context.Permissions.AddRange(permissions);
            await context.SaveChangesAsync();
        }

        var adminRole = await roleManager.FindByNameAsync("Admin");
        var customerRole = await roleManager.FindByNameAsync("Customer");
        var providerRole = await roleManager.FindByNameAsync("Provider");

        var allPermissions = context.Permissions.ToList();

        if (!context.RolePermissions.Any())
        {
            foreach (var perm in allPermissions)
            {
                context.RolePermissions.Add(new RolePermission
                {
                    RoleId = adminRole!.Id,
                    PermissionId = perm.Id
                });
            }

            // Customer  
            var create = allPermissions.First(p => p.Name == "request.create");

            context.RolePermissions.Add(new RolePermission
            {
                RoleId = customerRole!.Id,
                PermissionId = create.Id
            });

            // Provider 

            var accept = allPermissions.First(p => p.Name == "request.accept");
            var complete = allPermissions.First(p => p.Name == "request.complete");

            // accept
            context.RolePermissions.Add(new RolePermission
            {
                RoleId = providerRole!.Id,
                PermissionId = accept.Id
            });

            // complete
            context.RolePermissions.Add(new RolePermission
            {
                RoleId = providerRole!.Id,
                PermissionId = complete.Id
            });

            await context.SaveChangesAsync();
        }
    }
}