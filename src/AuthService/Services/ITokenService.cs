using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthService.Helpers;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

public interface ITokenService
{
    string GenerateToken(int idUsuario, string nombreUsuario);
}

public class TokenService : ITokenService
{
    private readonly JwtOptions _opts;
    public TokenService(IOptions<JwtOptions> opts) => _opts = opts.Value;

    public string GenerateToken(int idUsuario, string nombreUsuario)
    {
        var claims = new[]
        {
           new Claim(JwtRegisteredClaimNames.Sub, idUsuario.ToString()),
           new Claim(JwtRegisteredClaimNames.UniqueName, nombreUsuario)
       };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opts.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_opts.AccessTokenExpirationMinutes),
            SigningCredentials = creds,
            Issuer = _opts.Issuer,
            Audience = _opts.Audience
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
