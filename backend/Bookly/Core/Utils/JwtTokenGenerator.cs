using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Core.Utils;

public static class JwtTokenGenerator
{
    public static Result<string> GenerateToken(Dictionary<string, string> claimsData, IConfiguration configuration)
    {
        try
        {
            var secretKeyInfo = configuration[Const.JwtSecretKey]!;
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKeyInfo));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = claimsData.Select(c => new Claim(c.Key, c.Value));
            var token = new JwtSecurityToken
            (
                issuer: "Bookly",
                audience: "Bookly",
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: signingCredentials
            );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return Result<string>.Success(jwt);
        }
        catch (Exception e)
        {
            return Result<string>.Failure($"Не удалось сгенерировать JWT-токен: {e.Message}");
        }
    }
}