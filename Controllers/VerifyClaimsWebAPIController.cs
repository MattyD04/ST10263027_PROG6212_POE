using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using ST10263027_PROG6212_POE.Data;
using ST10263027_PROG6212_POE.Models;

namespace ST10263027_PROG6212_POE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VerifyClaimsWebAPIController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IValidator<ClaimViewModel> _claimValidator;

        public VerifyClaimsWebAPIController(AppDbContext context, IValidator<ClaimViewModel> claimValidator)
        {
            _context = context;
            _claimValidator = claimValidator;
        }

        [HttpGet("GetPendingClaims")]
        public async Task<ActionResult<IEnumerable<ClaimViewModel>>> GetPendingClaims()
        {
            try
            {
                var claims = await _context.Claims
                    .Include(c => c.Lecturer)
                    .Where(c => c.ClaimStatus == "Pending")
                    .Select(c => new ClaimViewModel
                    {
                        ClaimID = c.ClaimID,
                        ClaimNum = c.ClaimNum ?? "",
                        LecturerNum = c.Lecturer.LecturerNum ?? "",
                        SubmissionDate = c.SubmissionDate,
                        HoursWorked = c.Lecturer.HoursWorked,
                        HourlyRate = c.Lecturer.HourlyRate,
                        TotalAmount = c.Lecturer.HoursWorked * c.Lecturer.HourlyRate,
                        Comments = c.Comments ?? "",
                        Filename = c.Filename ?? ""
                    })
                    .ToListAsync();

                return Ok(claims);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving claims", error = ex.Message });
            }
        }

        [HttpPut("ApproveClaim/{id}")]
        public async Task<IActionResult> ApproveClaim(int id, [FromBody] ClaimViewModel claimViewModel)
        {
            try
            {
                // Validate the incoming ClaimViewModel using FluentValidation
                var validationResult = await _claimValidator.ValidateAsync(claimViewModel);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }

                var claim = await _context.Claims.FindAsync(id);
                if (claim == null)
                {
                    return NotFound(new { message = "Claim not found" });
                }

                // Calculate the total amount
                var totalAmount = claim.Lecturer.HoursWorked * claim.Lecturer.HourlyRate;

                // Check if the total amount is less than or equal to R15,000
                if (totalAmount <= 15000)
                {
                    // Automatically approve the claim
                    claim.ClaimStatus = "Approved";
                    await _context.SaveChangesAsync();
                    return Ok(new { message = "Claim approved automatically due to policy" });
                }
                else
                {
                    // Proceed with the manual approval process
                    claim.ClaimStatus = "Pending";
                    await _context.SaveChangesAsync();
                    return Ok(new { message = "Claim is pending manual approval" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error approving claim", error = ex.Message });
            }
        }

        [HttpPut("RejectClaim/{id}")]
        public async Task<IActionResult> RejectClaim(int id, [FromBody] ClaimViewModel claimViewModel)
        {
            try
            {
                // Validate the incoming ClaimViewModel using FluentValidation
                var validationResult = await _claimValidator.ValidateAsync(claimViewModel);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }

                var claim = await _context.Claims.FindAsync(id);
                if (claim == null)
                {
                    return NotFound(new { message = "Claim not found" });
                }

                claim.ClaimStatus = "Rejected";
                await _context.SaveChangesAsync();

                return Ok(new { message = "Claim rejected successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error rejecting claim", error = ex.Message });
            }
        }
    }
}