using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AuthService.Services;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;

        public AuthController(IUserService userService, ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        // DTO interno para la petición
        public record LoginRequest(
            [Required(ErrorMessage = "El username es obligatorio")]
            string Username,

            [Required(ErrorMessage = "La password es obligatoria")]
            string Password
        );

        // DTO de respuesta
        public record LoginResponse(string AccessToken);

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            // Llamada a tu servicio que invoca el SP vía EF Core
            var userId = await _userService.ValidateCredentialsAsync(req.Username, req.Password);
            if (userId == null)
                return Unauthorized(new { Message = "Credenciales inválidas" });

            // Genera el JWT
            var token = _tokenService.GenerateToken(userId.Value, req.Username);
            return Ok(new LoginResponse(token));
        }
    }
}
