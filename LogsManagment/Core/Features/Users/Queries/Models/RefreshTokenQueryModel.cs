using LogsManagment.Core.Common;
using MediatR;

namespace LogsManagment.Core.Features.Users.Queries.Models
{
    public class RefreshTokenQueryModel : IRequest<ResponseToken<string>>
    {
        public string RefreshToken { get; set; }
        public HttpResponse Response { get; set; }
    }
}
