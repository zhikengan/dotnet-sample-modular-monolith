using App.Modules.User.Data;
using App.Shared.Exceptions;
using App.Shared.Infrastructure;
using App.Shared.Models;
using App.Shared.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace App.Modules.User.Features.LoginUser;

public class LoginUserHandler : ICommandHandler<LoginUserCommand, StdResponse<string>>
{
    private readonly UserDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LoginUserHandler(UserDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<StdResponse<string>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == request.Username && u.PasswordHash == request.Password, cancellationToken);
        
        Assert.NotNull(user, ErrorCodes.USER_NOT_FOUND);

        var token = GenerateJwe(user!.Id, user.Username);

        // Set Cookie
        if (_httpContextAccessor.HttpContext != null)
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Append("AuthCookie", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true, 
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(1)
            });
        }

        return StdResponse<string>.Ok(token);
    }

    private string GenerateJwe(Guid userId, string username)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Name, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SuperSecretKey12345678901234567890"));
        var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var encryptionKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("EncryptionKey12345678901234567890"));
        var encryptingCredentials = new EncryptingCredentials(encryptionKey, JwtConstants.DirectKeyUseAlg, SecurityAlgorithms.Aes128CbcHmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = signingCredentials,
            EncryptingCredentials = encryptingCredentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
