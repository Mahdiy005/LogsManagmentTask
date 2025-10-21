using FluentValidation;
using LogsManagment.Core.Features.Logs.Commands.Models;
using LogsManagment.Core.Resource;
using Microsoft.Extensions.Localization;

namespace LogsManagment.Core.Features.Logs.Commands.Validators
{
    public class AddLogValidator : AbstractValidator<AddLogCommandModel>
    {
        private readonly IStringLocalizer<SharedResources> stringLocalizer;

        public AddLogValidator(IStringLocalizer<SharedResources> stringLocalizer)
        {
            this.stringLocalizer = stringLocalizer;
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage(stringLocalizer[LogsManagment.Core.Resource.SharedResourcesKeys.RequiredField])
                .MaximumLength(100).WithMessage(stringLocalizer[LogsManagment.Core.Resource.SharedResourcesKeys.DontExceed100]);
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");
        }
    }
}
