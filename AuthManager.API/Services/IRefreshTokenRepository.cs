using AuthManager.API.DAL.Entities;

namespace AuthManager.API.Services;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> FindByTokenAsync(string token);
    Task AddAsync(RefreshToken refreshToken);
    Task RemoveAsync(RefreshToken refreshToken);
}