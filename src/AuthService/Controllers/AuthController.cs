using AuthService.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _users;
        private readonly ITokenService _tokens;

        public AuthController(IUserService users, ITokenService tokens)
        {
            _users = users;
            _tokens = tokens;
        }

        public record LoginRequest(string Username, string Password);
        public record LoginResponse(string AccessToken);

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest req)
        {
            var user = _users.ValidateCredentials(req.Username, req.Password);
            if (user == null)
                return Unauthorized(new { message = "Usuario o contraseña inválidos" });

            var jwt = _tokens.GenerateToken(user);
            return Ok(new LoginResponse(jwt));
        }
    }
}
