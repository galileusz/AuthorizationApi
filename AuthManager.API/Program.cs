using AuthManager.API.DAL;
using AuthManager.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<ITokenGenerator, TokenGenerator>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

builder.Services.AddDbContext<AuthManagerDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AuthManagerDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = GetTokenValidationParameters(builder.Configuration);
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

TokenValidationParameters GetTokenValidationParameters(IConfiguration configuration)
{
    const int VALID_KEY_LENGTH = 32;

    var jwtKey = configuration["Jwt:Key"] ?? throw new ArgumentException("Jwt.Key not configured", nameof(configuration));
    var jwtIssuer = configuration["Jwt:Issuer"] ?? throw new ArgumentException("Jwt.Issuer not configured", nameof(configuration));
    var jwtAudience = configuration["Jwt:Audience"] ?? throw new ArgumentException("Jwt.Audience not configured", nameof(configuration));

    if (jwtKey.Length != VALID_KEY_LENGTH)
        throw new ArgumentException($"Jwt.Key is not valid (length = {VALID_KEY_LENGTH})", nameof(configuration));

    var result = new TokenValidationParameters();
    result.ValidateIssuer = true;
    result.ValidateAudience = true;
    result.ValidateLifetime = true;
    result.ValidateIssuerSigningKey = true;
    result.ValidIssuer = jwtIssuer;
    result.ValidAudience = jwtAudience;
    result.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

    return result;
}
