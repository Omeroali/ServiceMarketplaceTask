using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ServiceMarketplace.Application.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ServiceMarketplace.Infrastructure.Identity;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _context; 

    public AuthService(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _configuration = configuration;
        _context = context;
    }

    // Register
    public async Task<string> RegisterAsync(string email, string password, string fullName)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = fullName
        };

        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
            throw new Exception(string.Join(",", result.Errors.Select(e => e.Description)));

        await _userManager.AddToRoleAsync(user, "Customer");
        //await _userManager.AddToRoleAsync(user, "Provider");
        //await _userManager.AddToRoleAsync(user, "Admin");

        return await GenerateToken(user);
    }

    // Login
    public async Task<string> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null || !await _userManager.CheckPasswordAsync(user, password))
            throw new Exception("Invalid credentials");

        return await GenerateToken(user);
    }

    private async Task<string> GenerateToken(ApplicationUser user)
    {
        var jwtSettings = _configuration.GetSection("Jwt");

        var roles = await _userManager.GetRolesAsync(user);

        var roleIds = _context.Roles
            .Where(r => roles.Contains(r.Name!))
            .Select(r => r.Id)
            .ToList();

        var permissions = _context.RolePermissions
            .Where(rp => roleIds.Contains(rp.RoleId))
            .Select(rp => rp.Permission.Name)
            .Distinct()
            .ToList();

        //  Claims
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!)
        };

        //  Roles
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        //  Permissions 
        foreach (var permission in permissions)
        {
            claims.Add(new Claim("permission", permission!));
        }

        // Token
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings["Key"]!)
        );

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                double.Parse(jwtSettings["DurationInMinutes"]!)
            ),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task CheckUserLimit(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            throw new Exception("User not found");

        var roles = await _userManager.GetRolesAsync(user);

        if (roles.Contains("Admin"))
            return;

        if (!user.IsSubscribed)
        {
            var count = _context.ServiceRequests
                .Count(x => x.CustomerId == userId);

            if (count >= 3)
                throw new Exception("Free users can only create up to 3 requests");
        }
    }
}