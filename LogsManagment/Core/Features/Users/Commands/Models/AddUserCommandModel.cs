using LogsManagment.Core.Common;
using MediatR;

namespace LogsManagment.Core.Features.Users.Commands.Models
{
    public class AddUserCommandModel : IRequest<Response<string>>
    {
        public AddUserCommandModel(string name, string email, string password)
        {
            Name = name;
            Email = email;
            Password = password;
        }

        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
