using AuthManager.API.Models;
using AuthManager.API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public AuthController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ITokenGenerator tokenGenerator,
            IRefreshTokenRepository refreshTokenRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenGenerator = tokenGenerator;
            _refreshTokenRepository = refreshTokenRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var user = new IdentityUser { UserName = model.Username, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Ok(new { Result = "User created successfully" });
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);

            if (false == result.Succeeded)
                return Unauthorized();

            var user = await _userManager.FindByNameAsync(model.Username);

            if (user is null)
                return Unauthorized();

            var token = _tokenGenerator.GenerateToken(user);
            var refreshToken = _tokenGenerator.GenerateRefreshToken(user.Id);

            await _refreshTokenRepository.AddAsync(refreshToken);

            return Ok(new { Token = token, RefreshToken = refreshToken.Token });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshModel model)
        {
            var refreshToken = await _refreshTokenRepository.FindByTokenAsync(model.RefreshToken);

            if (refreshToken == null || refreshToken.ExpiryDate <= DateTime.UtcNow)
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByIdAsync(refreshToken.UserId);

            if (user is null)
                return Unauthorized();

            var token = _tokenGenerator.GenerateToken(user);

            var newRefreshToken = _tokenGenerator.GenerateRefreshToken(user.Id);

            await _refreshTokenRepository.RemoveAsync(refreshToken);
            await _refreshTokenRepository.AddAsync(newRefreshToken);

            return Ok(new { Token = token, RefreshToken = newRefreshToken.Token });
        }
    }
}
