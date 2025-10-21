using LogsManagment.Core.Common;
using LogsManagment.Core.Features.Logs.Queries.Dtos;
using LogsManagment.Core.Features.Logs.Queries.Models;
using LogsManagment.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LogsManagment.Core.Features.Logs.Queries.Handlers
{
    /*
     * i means if it Admin will retrieve all logs , but if it User it will retrieve only assigned to it
     */
    public class GetAllLogsHandler : ResponseHandler, IRequestHandler<GetAllLogsQueryModel, Response<List<GetAllLogsDto>>>
    {
        private readonly Data.AppContext appContext;
        private readonly IHttpContextAccessor httpContextAccessor;

        public GetAllLogsHandler(Data.AppContext _appContext, IHttpContextAccessor _httpContextAccessor)
        {
            appContext = _appContext;
            httpContextAccessor = _httpContextAccessor;
        }
        public async Task<Response<List<GetAllLogsDto>>> Handle(GetAllLogsQueryModel request, CancellationToken cancellationToken)
        {
            var user = httpContextAccessor.HttpContext?.User;
            var userIdClaim = user?.FindFirst(ClaimTypes.NameIdentifier);
            var roleClaims = user?.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList() ?? new List<string>();

            IQueryable<Log> logsQuery = appContext.Logs.Include(l => l.AdminWhoCreated);

            if (roleClaims.Contains("User") && userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                // Only logs assigned to the current user
                logsQuery = appContext.UserLogs
                    .Where(ul => ul.UserId == userId)
                    .Select(ul => ul.Log);
            }
            // else if Admin, retrieve all logs (already set)

            var logs = await logsQuery.ToListAsync();

            var logsDto = logs.Select(l => new GetAllLogsDto
            {
                Id = l.Id,
                Title = l.Title,
                Description = l.Description,
                CreatedBy = l.AdminWhoCreated?.UserName
            }).ToList();

            return Success(logsDto, logsDto.Count == 0 ? "No logs assigned to this user." : "Logs retrieved successfully", "Retrived Successfully");
        }
    }
}
