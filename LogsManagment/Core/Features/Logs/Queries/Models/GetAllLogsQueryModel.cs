using LogsManagment.Core.Common;
using LogsManagment.Core.Features.Logs.Queries.Dtos;
using MediatR;

namespace LogsManagment.Core.Features.Logs.Queries.Models
{
    public class GetAllLogsQueryModel : IRequest<Response<List<GetAllLogsDto>>>
    {
    }
}
