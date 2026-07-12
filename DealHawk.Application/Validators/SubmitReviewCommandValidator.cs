using FluentValidation;
using DealHawk.Application.Features.Reviews;

namespace DealHawk.Application.Validators
{
    public class SubmitReviewCommandValidator : AbstractValidator<SubmitReviewCommand>
    {
        public SubmitReviewCommandValidator()
        {
            RuleFor(x => x.GameId)
                .GreaterThan(0).WithMessage("Valid Game ID is required.");

            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5 stars.");

            RuleFor(x => x.Comment)
                .NotEmpty().WithMessage("Review comment cannot be empty.")
                .MaximumLength(500).WithMessage("Review comment cannot exceed 500 characters.");
        }
    }
}
