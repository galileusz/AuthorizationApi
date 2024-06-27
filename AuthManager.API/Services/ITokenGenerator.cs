using Microsoft.AspNetCore.Identity;

namespace AuthManager.API.Services
{
    public interface ITokenGenerator
    {
        string GenerateToken(IdentityUser user);
    }
}