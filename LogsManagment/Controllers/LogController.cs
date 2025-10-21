using LogsManagment.Core.Enums;
using LogsManagment.Core.Features.Logs.Commands.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogsManagment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly IMediator mediator;

        public LogController(IMediator _mediator)
        {
            mediator = _mediator;
        }

        [HttpPost("Add")]
        [Authorize(Roles = Roles.Admin)]
        public IActionResult AddLog(AddLogCommandModel model)
        {
            var res = mediator.Send(model).Result;
            return Ok(res);
        }

        [HttpGet("GetAll")]
        [Authorize(Roles = Roles.Admin + "," + Roles.User)]
        public async Task<IActionResult> GetAllLogs()
        {
            var query = new Core.Features.Logs.Queries.Models.GetAllLogsQueryModel();
            var res = await mediator.Send(query);
            return Ok(res);
        }

        [HttpGet("GetById/{logId}")]
        [Authorize(Roles = Roles.Admin + "," + Roles.User)]
        public async Task<IActionResult> GetLogById(int logId)
        {
            var query = new Core.Features.Logs.Queries.Models.GetSingleQueryModel { LogId = logId };
            var res = await mediator.Send(query);
            return Ok(res);
        }
    }
}
