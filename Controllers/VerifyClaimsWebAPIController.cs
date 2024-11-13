using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using ST10263027_PROG6212_POE.Data;
using ST10263027_PROG6212_POE.Models;
using System.Threading.Tasks;
//this controller handles the communication between the backend and frontend of the verify claims page by using the ASP.NET Web API
namespace ST10263027_PROG6212_POE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VerifyClaimsWebAPIController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IValidator<ClaimViewModel> _validator;

        public VerifyClaimsWebAPIController(AppDbContext context, IValidator<ClaimViewModel> validator)
        {
            _context = context;
            _validator = validator;
        }

        [HttpGet]
        public async Task<IActionResult> GetPendingClaims() //this method fetches all the pending claims that are yet to be either approved or rejected and display them in a table
        {
            var claimViewModels = await _context.Claims
                .Where(claim => claim.ClaimStatus == "Pending")
                .Select(claim => new ClaimViewModel
                {
                    ClaimID = claim.ClaimID,//this fetches the ClaimID from the Claim table
                    ClaimNum = claim.ClaimNum, //this fetches the Claim Number from the Claim table
                    LecturerNum = claim.Lecturer.LecturerNum, //this fetches the lecturer number from the lecturer table
                    SubmissionDate = claim.SubmissionDate, //this fetches the submission date for a claim from the claim table
                    HoursWorked = claim.Lecturer.HoursWorked, //this fetches the lecturer's hours worked from the lecturer table
                    HourlyRate = claim.Lecturer.HourlyRate,  //this fetches the lecturer's hourly rate from the lecturer table
                    TotalAmount = claim.Lecturer.HoursWorked * claim.Lecturer.HourlyRate, //calculates the total amount of a lecturer's hours worked and hourly rate 
                    Comments = claim.Comments,  //this fetches the a claim's comments from the Claim table
                    Filename = claim.Filename //this fetches the name of a file from the Claim table
                })
                .ToListAsync();

            return Ok(claimViewModels);
        }

        [HttpPost]
        [Route("approve")]
        public async Task<IActionResult> ApproveClaim(int claimId, bool isManual = false) //this method handles the approval of claims (errors fixed by Claude AI because the claims were not being approved properly)
        {
            var claim = await _context.Claims.Include(c => c.Lecturer).FirstOrDefaultAsync(c => c.ClaimID == claimId);
            if (claim == null)
            {
                return NotFound();
            }

            // Create a view model for validation
            var claimViewModel = new ClaimViewModel
            {
                ClaimID = claim.ClaimID,//this fetches the ClaimID from the Claim table
                ClaimNum = claim.ClaimNum, //this fetches the Claim Number from the Claim table
                LecturerNum = claim.Lecturer.LecturerNum, //this fetches the lecturer number from the lecturer table
                SubmissionDate = claim.SubmissionDate, //this fetches the submission date for a claim from the claim table
                HoursWorked = claim.Lecturer.HoursWorked, //this fetches the lecturer's hours worked from the lecturer table
                HourlyRate = claim.Lecturer.HourlyRate,  //this fetches the lecturer's hourly rate from the lecturer table
                TotalAmount = claim.Lecturer.HoursWorked * claim.Lecturer.HourlyRate, //calculates the total amount of a lecturer's hours worked and hourly rate 
                Comments = claim.Comments,  //this fetches the a claim's comments from the Claim table
                Filename = claim.Filename //this fetches the name of a file from the Claim table
            };

            // Automatically approve claims under R15,000
            if (claimViewModel.TotalAmount <= 15000)
            {
                return await ApproveClaimAutomatically(claim);
            }
            else
            {
                // For claims above R15,000, manually approve after validation
                var validationResult = await _validator.ValidateAsync(claimViewModel);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
                }
                claim.ClaimStatus = "Approved"; //sets the claim's status to appoved if the manager/coordinator clicks on the "Approve" button
                await _context.SaveChangesAsync();
                return Ok();
            }
        }
        //***************************************************************************************//
        private async Task<IActionResult> ApproveClaimAutomatically(Claim claim)//this method automatically approves claims if the required standards are met
        {
            // Logic for automatically approving claims with TotalAmount <= 15000
            claim.ClaimStatus = "Approved";//sets the claim's status to "Approved"
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Claim has been automatically approved due to policy." }); //message that displays if a claim has been approved automatically
        }

        [HttpPost]
        [Route("reject")] //this method handles the rejection of claims
        public async Task<IActionResult> RejectClaim(int claimId)
        {
            var claim = await _context.Claims.FindAsync(claimId);
            if (claim == null)
            {
                return NotFound();
            }

            claim.ClaimStatus = "Rejected"; //sets the claim's status to "Rejected"
            await _context.SaveChangesAsync();
            return Ok();
        }
    }

}
//*************************************End of file**************************************************//
