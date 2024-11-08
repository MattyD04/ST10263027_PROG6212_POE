using FluentValidation;
using ST10263027_PROG6212_POE.Models;

public class ClaimViewModelValidator : AbstractValidator<ClaimViewModel>
{
    public ClaimViewModelValidator()
    {
        RuleFor(c => c.ClaimNum)
            .NotEmpty().WithMessage("Claim Number is required.");
        RuleFor(c => c.LecturerNum)
            .NotEmpty().WithMessage("Lecturer Number is required.");
        RuleFor(c => c.SubmissionDate)
            .NotEmpty().WithMessage("Submission Date is required.");
        RuleFor(c => c.HoursWorked)
            .GreaterThan(0).WithMessage("Hours Worked must be greater than 0.");
        RuleFor(c => c.HourlyRate)
            .GreaterThan(0).WithMessage("Hourly Rate must be greater than 0.");
        RuleFor(c => c.TotalAmount)
            .GreaterThan(0).WithMessage("Total Amount must be greater than 0.");

        // Add a new rule to check if the TotalAmount is less than or equal to R15,000
        When(c => c.TotalAmount <= 15000, () =>
        {
            RuleFor(c => c.TotalAmount).Must(a => true).WithMessage("Claim has been automatically approved due to policy."); // This will automatically approve the claim
        });

        // Keep the existing rule for claims greater than R15,000
        When(c => c.TotalAmount > 15000, () =>
        {
            RuleFor(c => c.TotalAmount).Must(a => false).WithMessage("Total Amount is greater than R15,000 and cannot be automatically approved.");
        });
    }
}