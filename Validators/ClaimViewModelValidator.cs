using FluentValidation;
using ST10263027_PROG6212_POE.Models;
//this files defines the fluent validation rules (corrections by Chatgpt as rules were not working)
public class ClaimViewModelValidator : AbstractValidator<ClaimViewModel>
{
    public ClaimViewModelValidator()
    {
        RuleFor(c => c.ClaimNum)
            .NotEmpty().WithMessage("Claim Number is required."); //rule stating a claim number is required
        RuleFor(c => c.LecturerNum)
            .NotEmpty().WithMessage("Lecturer Number is required."); // rule stating a lecturer number is required
        RuleFor(c => c.SubmissionDate)
            .NotEmpty().WithMessage("Submission Date is required."); //rule stating a submission date is required
        RuleFor(c => c.HoursWorked)
            .GreaterThan(0).WithMessage("Hours Worked must be greater than 0."); //rule stating hours worked must be more than 0
        RuleFor(c => c.HourlyRate)
            .GreaterThan(0).WithMessage("Hourly Rate must be greater than 0."); //rule stating the hourly rate must be more than 0
        RuleFor(c => c.TotalAmount)
            .GreaterThan(0).WithMessage("Total Amount must be greater than 0."); //rule stating that total amount must be more than 0

       
        When(c => c.TotalAmount <= 15000, () =>
        {
            RuleFor(c => c.TotalAmount).Must(a => true).WithMessage("Claim has been automatically approved due to policy."); // This will automatically approve the claim
        }); //rule stating that a claim can be auto approved if the total amount is less than R15 000

    }
}
//**************************************************end of file***********************************************//