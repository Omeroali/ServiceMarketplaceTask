using Microsoft.AspNetCore.Identity;

namespace ServiceMarketplace.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public bool IsSubscribed { get; set; } = false;
}