using AuthManager.API.DAL;
using AuthManager.API.DAL.Entities;
using AuthManager.API.Services;
using Microsoft.EntityFrameworkCore;

namespace AuthManager.Tests;

public class RefreshTokenRepositoryTests
{
    private AuthManagerDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AuthManagerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AuthManagerDbContext(options);
    }

    [Fact]
    public async Task AddAsync_ShouldAddRefreshToken()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var repository = new RefreshTokenRepository(context);
        var refreshToken = new RefreshToken
        {
            Token = "test_token",
            UserId = "test_user",
            ExpiryDate = DateTime.Now.AddHours(1)
        };

        // Act
        await repository.AddAsync(refreshToken);

        // Assert
        var retrievedToken = await context.RefreshTokens.FirstOrDefaultAsync(e => e.Token == "test_token");
        Assert.NotNull(retrievedToken);
        Assert.Equal("test_token", retrievedToken.Token);
        Assert.Equal("test_user", retrievedToken.UserId);
        Assert.Equal(refreshToken.ExpiryDate, retrievedToken.ExpiryDate);
    }

    [Fact]
    public async Task FindByTokenAsync_ShouldReturnRefreshToken()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var repository = new RefreshTokenRepository(context);
        var refreshToken = new RefreshToken
        {
            Token = "test_token",
            UserId = "test_user",
            ExpiryDate = DateTime.Now.AddHours(1)
        };

        await context.RefreshTokens.AddAsync(refreshToken);
        await context.SaveChangesAsync();

        // Act
        var retrievedToken = await repository.FindByTokenAsync("test_token");

        // Assert
        Assert.NotNull(retrievedToken);
        Assert.Equal("test_token", retrievedToken.Token);
        Assert.Equal("test_user", retrievedToken.UserId);
        Assert.Equal(refreshToken.ExpiryDate, retrievedToken.ExpiryDate);
    }

    [Fact]
    public async Task FindByTokenAsync_ShouldReturnNull_WhenTokenNotFound()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var repository = new RefreshTokenRepository(context);

        // Act
        var retrievedToken = await repository.FindByTokenAsync("non_existent_token");

        // Assert
        Assert.Null(retrievedToken);
    }

    [Fact]
    public async Task RemoveAsync_ShouldRemoveRefreshToken()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var repository = new RefreshTokenRepository(context);
        var refreshToken = new RefreshToken
        {
            Token = "test_token",
            UserId = "test_user",
            ExpiryDate = DateTime.Now.AddHours(1)
        };

        await context.RefreshTokens.AddAsync(refreshToken);
        await context.SaveChangesAsync();

        // Act
        await repository.RemoveAsync(refreshToken);

        // Assert
        var retrievedToken = await context.RefreshTokens.FirstOrDefaultAsync(e => e.Token == "test_token");
        Assert.Null(retrievedToken);
    }
}