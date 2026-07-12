using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DealHawk.Application.DTOs;
using DealHawk.Application.Features.Auth;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DealHawk.API.Controllers
{
    public class AuthController : ApiControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterCommand command)
        {
            var result = await Mediator.Send(command);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginCommand command)
        {
            var result = await Mediator.Send(command);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [Authorize]
        [HttpGet("me")]
        public ActionResult GetCurrentUserDetails()
        {
            var username = User.Identity?.Name;
            var email = User.FindFirstValue(ClaimTypes.Email);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var roles = new System.Collections.Generic.List<string>();
            foreach (var claim in User.FindAll(ClaimTypes.Role))
            {
                roles.Add(claim.Value);
            }
            return Ok(new
            {
                UserId = userId,
                Username = username,
                Email = email,
                Roles = roles
            });
        }
    }
}
