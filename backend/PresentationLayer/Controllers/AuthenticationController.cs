using BusinessLogicLayer.DTOs.Authentication;
using BusinessLogicLayer.Interfaces.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        [HttpPost("login-by-google")]
        [AllowAnonymous]
        public async Task<IActionResult> Google(
            [FromBody] LoginByGoogleDtoRequest req,
            [FromServices] IAuthenticationService auth,
            CancellationToken ct)
        {
            var tokens = await auth.LoginByGoogleCodeAsync(req.Code, ct);
            return Ok(tokens);
        }

        [HttpPost("login-by-email")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginByEmail(
            [FromBody] LoginByEmailDtoRequest req,
            [FromServices] IAuthenticationService auth,
            CancellationToken ct)
        {
            var tokens = await auth.LoginByEmailAsync(req, ct);
            return Ok(tokens);
        }
    }
}
