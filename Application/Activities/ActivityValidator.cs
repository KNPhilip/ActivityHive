using Domain;
using FluentValidation;

namespace Application.Activities;

public sealed class ActivityValidator : AbstractValidator<Activity>
{
    public ActivityValidator()
    {
        RuleFor(a => a.Title).NotEmpty().WithMessage("The title field is required.");
        RuleFor(a => a.Description).NotEmpty().WithMessage("The description field is required.");
        RuleFor(a => a.Date).NotEmpty().WithMessage("The date field is required.");
        RuleFor(a => a.Category).NotEmpty().WithMessage("The category field is required.");
        RuleFor(a => a.City).NotEmpty().WithMessage("The city field is required.");
        RuleFor(a => a.Venue).NotEmpty().WithMessage("The venue field is required.");
    }
}
