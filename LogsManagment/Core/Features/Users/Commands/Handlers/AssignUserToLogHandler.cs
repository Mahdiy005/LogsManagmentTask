using LogsManagment.Core.Common;
using LogsManagment.Core.Features.Users.Commands.Models;
using LogsManagment.Core.Hubs;
using LogsManagment.Data.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace LogsManagment.Core.Features.Users.Commands.Handlers
{
    public class AssignUserToLogHandler : ResponseHandler, IRequestHandler<AssignUserToLogCommandModel, Response<string>>
    {
        private readonly Data.AppContext _appContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHubContext<NotificationHub> hubContext;

        public AssignUserToLogHandler(Data.AppContext appContext, UserManager<AppUser> userManager, IHubContext<NotificationHub> hubContext)
        {
            _appContext = appContext;
            _userManager = userManager;
            this.hubContext = hubContext;
        }

        public async Task<Response<string>> Handle(AssignUserToLogCommandModel request, CancellationToken cancellationToken)
        {
            // ✅ تأكد إن المستخدم موجود
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
                return BadRequest<string>("User does not exist.");

            // ✅ تحقق إن المستخدم مش Admin
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Admin"))
                return BadRequest<string>("Cannot assign an Admin to a log.");

            // ✅ تحقق إن الـ Log موجود
            var logExists = await _appContext.Logs.AnyAsync(l => l.Id == request.LogId, cancellationToken);
            if (!logExists)
                return BadRequest<string>("Log does not exist.");

            // ✅ تحقق إن المستخدم مش متسجل بالفعل
            var alreadyAssigned = await _appContext.UserLogs
                .AnyAsync(ul => ul.UserId == request.UserId && ul.LogId == request.LogId, cancellationToken);

            if (alreadyAssigned)
                return BadRequest<string>("User already assigned to this log.");

            // ✅ أضف العلاقة
            await _appContext.UserLogs.AddAsync(new UserLog
            {
                LogId = request.LogId,
                UserId = request.UserId
            }, cancellationToken);

            await _appContext.SaveChangesAsync(cancellationToken);

            // ✅ Send real-time notification to the user
            await hubContext.Clients.User(user.Id.ToString())
                .SendAsync("ReceiveNotification", $"You have been assigned to Log ID {request.LogId}");


            return Created($"User '{user.UserName}' assigned to log successfully.");
        }
    }
}
