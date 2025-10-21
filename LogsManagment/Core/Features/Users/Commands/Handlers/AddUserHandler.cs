using LogsManagment.Core.Common;
using LogsManagment.Core.Enums;
using LogsManagment.Core.Features.Users.Commands.Models;
using LogsManagment.Data.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace LogsManagment.Core.Features.Users.Commands.Handlers
{
    public class AddUserHandler : ResponseHandler, IRequestHandler<AddUserCommandModel, Response<string>>
    {
        private readonly UserManager<AppUser> _userManager;

        public AddUserHandler(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Response<string>> Handle(AddUserCommandModel request, CancellationToken cancellationToken)
        {
            var user = new AppUser
            {
                UserName = request.Name, // ✅ use username instead of Name
                Email = request.Email
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest<string>(errors);
            }

            // ✅ Assign "Regular" role (must exist in DB)
            await _userManager.AddToRoleAsync(user, Roles.User);

            return Success<string>("User added successfully");
        }
    }
}
