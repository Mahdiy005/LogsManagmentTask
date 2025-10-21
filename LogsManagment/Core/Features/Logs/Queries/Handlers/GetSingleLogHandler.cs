using LogsManagment.Core.Common;
using LogsManagment.Core.Features.Logs.Queries.Dtos;
using LogsManagment.Core.Features.Logs.Queries.Models;
using LogsManagment.Core.Resource;
using LogsManagment.Data.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Security.Claims;

namespace LogsManagment.Core.Features.Logs.Queries.Handlers
{
    public class GetSingleLogHandler : ResponseHandler, IRequestHandler<GetSingleQueryModel, Response<GetLogByIdDto>>
    {
        private readonly Data.AppContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;
        private readonly IStringLocalizer<SharedResources> stringLocalizer;

        public GetSingleLogHandler(Data.AppContext context, IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager, IStringLocalizer<SharedResources> stringLocalizer)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            this.stringLocalizer = stringLocalizer;
        }

        public async Task<Response<GetLogByIdDto>> Handle(GetSingleQueryModel request, CancellationToken cancellationToken)
        {
            // 1️⃣ Get current user
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue("uid");
            if (userId == null)
                return Unauthorized<GetLogByIdDto>();

            var user = await _userManager.FindByIdAsync(userId);
            var roles = await _userManager.GetRolesAsync(user);

            // 2️⃣ Fetch log by Id
            var log = await _context.Logs
                .Include(l => l.UsersAssigned)
                .ThenInclude(u => u.User.AssignedLogs)
                .FirstOrDefaultAsync(l => l.Id == request.LogId, cancellationToken);

            if (log == null)
                return NotFound<GetLogByIdDto>(stringLocalizer[SharedResourcesKeys.NotFound]);

            // 3️⃣ Check access permission
            if (roles.Contains("Admin") == false)
            {
                bool assignedToUser = log.UsersAssigned.Any(u => u.UserId == int.Parse(userId));
                if (!assignedToUser)
                    return Forbidden<GetLogByIdDto>();
            }

            // 4️⃣ Map to DTO
            var result = new GetLogByIdDto
            {
                Id = log.Id,
                Title = log.Title,
                Description = log.Description,
                CreatedAt = log.CreatedAt,
                Users = log.UsersAssigned.Select(u => new UserForLogDto
                {
                    Id = u.User.Id,
                    UserName = u.User.UserName,
                    Email = u.User.Email
                }).ToList()
            };
            // change seen status if user with role User 
            if (roles.Contains("User"))
            {
                var userLog = await _context.UserLogs
                    .FirstOrDefaultAsync(ul => ul.LogId == log.Id && ul.UserId == int.Parse(userId), cancellationToken);
                if (userLog != null && !userLog.Seen)
                {
                    userLog.Seen = true;
                    await _context.SaveChangesAsync(cancellationToken);
                }
            }

            return Success(result, message: stringLocalizer[SharedResourcesKeys.RetrivedSuccess]);
        }
    }
}
