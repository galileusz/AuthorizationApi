using AuthManager.API.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthManager.API.DAL;
public class AuthManagerDbContext : IdentityDbContext<IdentityUser>
{
    public AuthManagerDbContext(DbContextOptions<AuthManagerDbContext> options)
        : base(options)
    {
    }

    public DbSet<RefreshToken> RefreshTokens { get; set; }
}