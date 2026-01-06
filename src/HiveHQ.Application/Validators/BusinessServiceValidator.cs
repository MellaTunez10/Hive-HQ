using FluentValidation;
using HiveHQ.Domain.Entities;

namespace HiveHQ.Application.Validators;

public class BusinessServiceValidator : AbstractValidator<BusinessService>
{
    public BusinessServiceValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Service name is required.")
            .MinimumLength(3).WithMessage("Service name must be at least 3 characters.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
    }
}
