using LogsManagment.Core.Common;
using MediatR;

namespace LogsManagment.Core.Features.Users.Commands.Models
{
    public class AssignUserToLogCommandModel : IRequest<Response<string>>
    {
        public int UserId { get; set; }
        public int LogId { get; set; }
    }
}
