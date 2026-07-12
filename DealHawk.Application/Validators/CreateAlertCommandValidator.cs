using FluentValidation;
using DealHawk.Application.Features.PriceAlerts;

namespace DealHawk.Application.Validators
{
    public class CreateAlertCommandValidator : AbstractValidator<CreateAlertCommand>
    {
        public CreateAlertCommandValidator()
        {
            RuleFor(x => x.GameId)
                .GreaterThan(0).WithMessage("Valid Game ID is required.");

            RuleFor(x => x.TargetPrice)
                .GreaterThan(0).WithMessage("Target price must be greater than $0.00.");
        }
    }
}
