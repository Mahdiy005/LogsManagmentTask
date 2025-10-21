using LogsManagment.Core.Common;
using MediatR;

namespace LogsManagment.Core.Features.Users.Commands.Models
{
    public class LoginUserCommandModel : IRequest<ResponseToken<string>>
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
