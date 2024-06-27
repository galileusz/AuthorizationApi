using AuthManager.API.Services;
using AuthManager.API.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Xunit;

namespace AuthManager.Tests;

public class TokenGeneratorTests
{
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly TokenGenerator _tokenGenerator;

    public TokenGeneratorTests()
    {
        _configurationMock = new Mock<IConfiguration>();

        _configurationMock.SetupGet(c => c["Jwt:Key"]).Returns("your_secret_key_which_should_be_long_enough");
        _configurationMock.SetupGet(c => c["Jwt:Issuer"]).Returns("your_issuer");
        _configurationMock.SetupGet(c => c["Jwt:Audience"]).Returns("your_audience");
        _configurationMock.SetupGet(c => c["Jwt:TokenLifeTimeInMinutes"]).Returns("30");
        _configurationMock.SetupGet(c => c["Jwt:RefreshTokenLifeTimeInHours"]).Returns("24");

        _tokenGenerator = new TokenGenerator(_configurationMock.Object);
    }

    [Fact]
    public void GenerateToken_ShouldReturnValidJwtToken()
    {
        // Arrange
        var user = new IdentityUser("testuser");

        // Act
        var token = _tokenGenerator.GenerateToken(user);

        // Assert
        Assert.NotNull(token);
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        Assert.Equal("testuser", jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);
        Assert.Equal("your_issuer", jwtToken.Issuer);
        Assert.Equal("your_audience", jwtToken.Audiences.First());
    }

    [Fact]
    public void GenerateToken_ShouldThrowException_WhenUserNameIsNull()
    {
        // Arrange
        var user = new IdentityUser();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _tokenGenerator.GenerateToken(user));
    }

    [Fact]
    public void GenerateRefreshToken_ShouldReturnValidRefreshToken()
    {
        // Arrange
        var userId = "testuserId";

        // Act
        var refreshToken = _tokenGenerator.GenerateRefreshToken(userId);

        // Assert
        Assert.NotNull(refreshToken);
        Assert.Equal(userId, refreshToken.UserId);
        Assert.True(refreshToken.ExpiryDate > DateTime.Now);
        Assert.NotEmpty(refreshToken.Token);
    }
}
