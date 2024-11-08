using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using ST10263027_PROG6212_POE.Data;
using ST10263027_PROG6212_POE.Models;
using System.Threading.Tasks;

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
        public async Task<IActionResult> GetPendingClaims()
        {
            var claimViewModels = await _context.Claims
                .Where(claim => claim.ClaimStatus == "Pending")
                .Select(claim => new ClaimViewModel
                {
                    ClaimID = claim.ClaimID,
                    ClaimNum = claim.ClaimNum,
                    LecturerNum = claim.Lecturer.LecturerNum,
                    SubmissionDate = claim.SubmissionDate,
                    HoursWorked = claim.Lecturer.HoursWorked,
                    HourlyRate = claim.Lecturer.HourlyRate,
                    TotalAmount = claim.Lecturer.HoursWorked * claim.Lecturer.HourlyRate,
                    Comments = claim.Comments,
                    Filename = claim.Filename
                })
                .ToListAsync();

            return Ok(claimViewModels);
        }

        [HttpPost]
        [Route("approve")]
        public async Task<IActionResult> ApproveClaim(int claimId, bool isManual = false)
        {
            var claim = await _context.Claims.Include(c => c.Lecturer).FirstOrDefaultAsync(c => c.ClaimID == claimId);
            if (claim == null)
            {
                return NotFound();
            }

            // Create a view model for validation
            var claimViewModel = new ClaimViewModel
            {
                ClaimID = claim.ClaimID,
                ClaimNum = claim.ClaimNum,
                LecturerNum = claim.Lecturer.LecturerNum,
                SubmissionDate = claim.SubmissionDate,
                HoursWorked = claim.Lecturer.HoursWorked,
                HourlyRate = claim.Lecturer.HourlyRate,
                TotalAmount = claim.Lecturer.HoursWorked * claim.Lecturer.HourlyRate,
                Comments = claim.Comments,
                Filename = claim.Filename
            };

            // Automatically approve claims with a TotalAmount of <= 15000 (as per policy)
            if (claimViewModel.TotalAmount <= 15000)
            {
                claim.ClaimStatus = "Approved";
            }
            else
            {
                // For claims above R15,000, manually approve after validation
                var validationResult = await _validator.ValidateAsync(claimViewModel);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
                }
                claim.ClaimStatus = "Approved";
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        [Route("reject")]
        public async Task<IActionResult> RejectClaim(int claimId)
        {
            var claim = await _context.Claims.FindAsync(claimId);
            if (claim == null)
            {
                return NotFound();
            }

            claim.ClaimStatus = "Rejected";
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}