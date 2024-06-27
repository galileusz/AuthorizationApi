using AuthManager.API.DAL;
using AuthManager.API.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthManager.API.Services
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AuthManagerDbContext _context;

        public RefreshTokenRepository(AuthManagerDbContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken?> FindByTokenAsync(string token)
        {
            return await _context.RefreshTokens.FirstOrDefaultAsync(e => e.Token == token);
        }

        public async Task AddAsync(RefreshToken refreshToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Remove(refreshToken);
            await _context.SaveChangesAsync();
        }
    }
}
