using LogsManagment.Core;
using LogsManagment.Core.Enums;
using LogsManagment.Core.Features.Users.Commands.Models;
using LogsManagment.Core.Features.Users.Queries.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogsManagment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator mediator;

        public UserController(IMediator _mediator)
        {
            mediator = _mediator;
        }
        [HttpPost("AddUser")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> AddUser([FromBody] AddUserCommandModel addUserCommandModel)
        {
            var res = await mediator.Send(addUserCommandModel);
            return Ok(res);
        }

        [HttpPost("Login")]
        public IActionResult Login(LoginUserCommandModel loginUserCommandModel)
        {
            var res = mediator.Send(loginUserCommandModel).Result;
            Helpers.SetRefreshTokenInCookie(Response, res.RefreshToken, res.RefreshTokenExpiration);
            return Ok(res);
        }

        [HttpPost("AssignUserToLog")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> AssignUserToLog([FromBody] AssignUserToLogCommandModel assignUserToLogCommandModel)
        {
            var res = await mediator.Send(assignUserToLogCommandModel);
            return Ok(res);
        }

        [HttpGet("refreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var res = await mediator.Send(new RefreshTokenQueryModel() { Response = Response, RefreshToken = refreshToken });
            return Ok(res);
        }

        [HttpGet("Test")]
        [Authorize]
        public IActionResult Test()
        {
            return Ok("Hello World");
        }
    }
}
