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
    }
}