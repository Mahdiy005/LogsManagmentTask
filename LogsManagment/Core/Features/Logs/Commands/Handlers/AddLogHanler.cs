using LogsManagment.Core.Common;
using LogsManagment.Core.Features.Logs.Commands.Models;
using MediatR;

namespace LogsManagment.Core.Features.Logs.Commands.Handlers
{
    public class AddLogHanler : ResponseHandler, IRequestHandler<AddLogCommandModel, Response<string>>
    {
        private readonly Data.AppContext appContext;
        private readonly IHttpContextAccessor httpContextAccessor;

        public AddLogHanler(Data.AppContext _appContext, IHttpContextAccessor _httpContextAccessor)
        {
            appContext = _appContext;
            httpContextAccessor = _httpContextAccessor;
        }
        public async Task<Response<string>> Handle(AddLogCommandModel request, CancellationToken cancellationToken)
        {
            // Get current user ID from claims
            var userIdClaim = httpContextAccessor.HttpContext?.User?.FindFirst("uid");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int adminId))
            {
                return Unauthorized<string>();
            }

            var log = new Data.Entities.Log
            {
                Title = request.Title,
                Description = request.Description,
                AdminId = adminId,
                CreatedAt = DateTime.UtcNow
            };
            var res = await appContext.Logs.AddAsync(log);
            appContext.SaveChanges();
            return Created("Log Created Succefully");
        }
    }
}
