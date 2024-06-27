using AuthManager.API.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthManager.API.Services;

public class TokenGenerator : ITokenGenerator
{
    private readonly string _jwtKey;
    private readonly string _jwtIssuer;
    private readonly string _jwtAudience;
    private readonly int _tokenLifetimeInMinutes;
    private readonly int _refreshTokenLifeTimeInHours;

    public TokenGenerator(IConfiguration configuration)
    {
        _jwtKey = configuration["Jwt:Key"] ?? throw new ArgumentException("Jwt.Key not configured", nameof(configuration));
        _jwtIssuer = configuration["Jwt:Issuer"] ?? throw new ArgumentException("Jwt.Issuer not configured", nameof(configuration));
        _jwtAudience = configuration["Jwt:Audience"] ?? throw new ArgumentException("Jwt.Audience not configured", nameof(configuration));

        if (false == int.TryParse(configuration["Jwt:TokenLifeTimeInMinutes"], out _tokenLifetimeInMinutes))
            throw new ArgumentException("Jwt.TokenLifeTimeInMinutes not configured", nameof(configuration));
        
        if (false == int.TryParse(configuration["Jwt:RefreshTokenLifeTimeInHours"], out _refreshTokenLifeTimeInHours))
            throw new ArgumentException("Jwt.RefreshTokenLifeTimeInHours not configured", nameof(configuration));
    }

    public string GenerateToken(IdentityUser user)
    {
        var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName ?? throw new ArgumentNullException(nameof(user.UserName))),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtIssuer,
            audience: _jwtAudience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(_tokenLifetimeInMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public RefreshToken GenerateRefreshToken(string userId)
    {
        return new RefreshToken
        {
            Token = Guid.NewGuid().ToString(),
            UserId = userId,
            ExpiryDate = DateTime.Now.AddHours(_refreshTokenLifeTimeInHours),
        };
    }
}
