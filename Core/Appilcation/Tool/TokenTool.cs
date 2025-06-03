using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Domain.Tool;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Appilcation.Tool;

public class TokenTool
{
    public static string Generate(IConfiguration configuration, GenTokenDto dto)
    {
        var key = Encoding.ASCII.GetBytes(configuration["JWTOption:Key"] ?? "");

        var handler = new JwtSecurityTokenHandler();

        var claims = new List<Claim>
        {
            new("Name", dto.Name ?? ""),
            new("Email", dto.Name ?? ""),
            new("Id", dto.Id.ToString() ?? "")
        };

        if (dto.Roles is not null)
            foreach (var role in dto.Roles)
                claims.Add(new Claim("Role", role));

        var token = handler.CreateToken(new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(10),
            Issuer = configuration["JWTOption:Issuer"],
            Audience = configuration["JWTOption:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        });
        return handler.WriteToken(token);
    }

    public static string CreateRefreshToken()
    {
        var number = new byte[32];
        using var random = RandomNumberGenerator.Create();
        random.GetBytes(number);
        return Convert.ToBase64String(number);
    }
}