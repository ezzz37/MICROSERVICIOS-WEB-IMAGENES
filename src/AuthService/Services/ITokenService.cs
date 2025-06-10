using AuthService.Models;

namespace AuthService.Services
{
    public interface ITokenService
    {
        /// <summary>
        /// Genera un JWT a partir del usuario validado.
        /// </summary>
        string GenerateToken(Usuario user);
    }
}
