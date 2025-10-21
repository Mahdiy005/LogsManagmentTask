using FluentValidation;
using LogsManagment.Core.Features.Users.Commands.Models;

namespace LogsManagment.Core.Features.Users.Commands.Validators
{
    public class AddUserValidator : AbstractValidator<AddUserCommandModel>
    {
        public AddUserValidator()
        {
            RuleFor(u => u.Name)
                .NotEmpty().WithMessage("{PropertyName} is required")
                .NotNull().WithMessage("{PropertyName} is required")
                .MaximumLength(60).WithMessage("{PropertyName} Must be lass than 60 character")
                .MinimumLength(3).WithMessage("{PropertyName} Must be More than 2 character");
            RuleFor(u => u.Email)
                .NotEmpty().WithMessage("{PropertyName} is required")
                .NotNull().WithMessage("{PropertyName} is required")
                .EmailAddress().WithMessage("{PropertyName} is not valid");
            RuleFor(u => u.Password)
                .NotEmpty().WithMessage("{PropertyName} is required")
                .NotNull().WithMessage("{PropertyName} is required")
                .MinimumLength(6).WithMessage("{PropertyName} Must be More than 5 character");
        }
    }
}
