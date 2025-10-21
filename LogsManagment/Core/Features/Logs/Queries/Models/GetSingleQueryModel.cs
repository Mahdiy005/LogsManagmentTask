using LogsManagment.Core.Common;
using LogsManagment.Core.Features.Logs.Queries.Dtos;
using MediatR;

namespace LogsManagment.Core.Features.Logs.Queries.Models
{
    public class GetSingleQueryModel : IRequest<Response<GetLogByIdDto>>
    {
        public int LogId { get; set; }
    }
}
