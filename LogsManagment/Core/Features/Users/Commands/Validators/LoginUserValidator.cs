using FluentValidation;
using LogsManagment.Core.Features.Users.Commands.Models;

namespace LogsManagment.Core.Features.Users.Commands.Validators
{
    public class LoginUserValidator : AbstractValidator<LoginUserCommandModel>
    {
        public LoginUserValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Name is required.");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
        }
    }
}
