using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using System.Text;
using auth10.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace auth10.Services;

public class JtwServices
{
    private readonly IConfiguration _configuration;
    private readonly SymmetricSecurityKey _key;
    public JtwServices(IConfiguration configuration)
    {
        _configuration = configuration;
        var secretKey = _configuration["JwtSettings:SecretKey"];
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
    }
      public string GenerateToken(User user)
        {
            var claim = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name,(user.Name ?? "Unknown")),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject =new ClaimsIdentity(claim),
                Issuer = _configuration["jwtSettings:Issuer"],
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JwtSettings:ExpirationInMinutes"])),
                Audience = _configuration["JwtSettings:Audience"],
                SigningCredentials = new SigningCredentials(_key,SecurityAlgorithms.HmacSha256)

            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescription);
            return tokenHandler.WriteToken(token);
        }
        
    

    
}