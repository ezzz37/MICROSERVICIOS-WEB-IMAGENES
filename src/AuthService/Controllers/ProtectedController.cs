using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("protected")]
    public class ProtectedController : ControllerBase
    {
        [HttpGet("hello")]
        [Authorize]
        public IActionResult Hello()
        {
            return Ok(new { message = "¡Estas dentro!" });
        }
    }
}
