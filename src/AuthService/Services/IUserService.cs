using AuthService.Models;

namespace AuthService.Services
{
    public interface IUserService
    {
        /// <summary>
        /// Valida usuario+contraseña. 
        /// Devuelve null si no existe o la contraseña no coincide.
        /// </summary>
        Usuario? ValidateCredentials(string username, string password);
    }
}
