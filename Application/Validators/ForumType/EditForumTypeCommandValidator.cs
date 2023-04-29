
using Application.Commands;
using FluentValidation;
using Microsoft.Extensions.Localization;


namespace CMS.Application.Commands
{
    public class EditForumTypeCommandValidator : AbstractValidator<EditForumTypeCommand>
    {
        public EditForumTypeCommandValidator()
        {
            RuleFor(request => request.Name)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => "Name is required!");

            RuleFor(request => request.Description)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => "Description is required!");

            RuleFor(request => request.AddedByUserID)
                //.Null().WithMessage(x => "Added userId is required!.")
                .GreaterThan(0).WithMessage(x => "Added UserId is required!");

            RuleFor(request => request.StatusCodeID)
                //.Null().WithMessage(x => "Added userId is required!.")
                .GreaterThan(0).WithMessage(x => "Status is required!");

        }
    }
}
