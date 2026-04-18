using Microsoft.AspNetCore.Authorization;

namespace ServiceMarketplace.API.Attributes;

public class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string permission)
    {
        Policy = permission;
    }
}