using LogsManagment.Core.Common;
using MediatR;

namespace LogsManagment.Core.Features.Logs.Commands.Models
{
    public class AddLogCommandModel : IRequest<Response<string>>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        //public int AdminId { get; set; }
    }
}
