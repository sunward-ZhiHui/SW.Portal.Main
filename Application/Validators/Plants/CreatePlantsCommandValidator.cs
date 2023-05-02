
using Application.Commands;
using FluentValidation;
using Microsoft.Extensions.Localization;


namespace CMS.Application.Commands
{
    public class CreatePlantsCommandValidator : AbstractValidator<CreatePlantCommand>
    {
        public CreatePlantsCommandValidator()
        {
            RuleFor(request => request.PlantCode)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => "Plant Code is required!");

            RuleFor(request => request.Description)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => "Description is required!");

            RuleFor(request => request.AddedByUserID)
                .GreaterThan(0).WithMessage(x => "Added UserId is required!");

            RuleFor(request => request.StatusCodeID)
                .GreaterThan(0).WithMessage(x => "Status is required!");

        }
    }
}
