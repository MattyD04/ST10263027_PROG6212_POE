using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ST10263027_PROG6212_POE.Data;
using ST10263027_PROG6212_POE.Models;
using Microsoft.EntityFrameworkCore;

namespace ST10263027_PROG6212_POE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VerifyClaimsWebAPIController : ControllerBase
    {
            private readonly AppDbContext _context;

            public VerifyClaimsWebAPIController(AppDbContext context)
            {
                _context = context;
            }

            // GET: api/ClaimsWebApi/GetPendingClaims
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
                            ClaimNum = c.ClaimNum,
                            LecturerNum = c.Lecturer.LecturerNum,
                            SubmissionDate = c.SubmissionDate,
                            HoursWorked = c.Lecturer.HoursWorked,
                            HourlyRate = c.Lecturer.HourlyRate,
                            TotalAmount = c.Lecturer.HoursWorked * c.Lecturer.HourlyRate,
                            Comments = c.Comments,
                            Filename = c.Filename
                        })
                        .ToListAsync();

                    return Ok(claims);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { message = "Error retrieving claims", error = ex.Message });
                }
            }

            // PUT: api/ClaimsWebApi/ApproveClaim/5
            [HttpPut("ApproveClaim/{id}")]
            public async Task<IActionResult> ApproveClaim(int id)
            {
                try
                {
                    var claim = await _context.Claims.FindAsync(id);
                    if (claim == null)
                    {
                        return NotFound(new { message = "Claim not found" });
                    }

                    claim.ClaimStatus = "Approved";
                    await _context.SaveChangesAsync();

                    return Ok(new { message = "Claim approved successfully" });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { message = "Error approving claim", error = ex.Message });
                }
            }

            // PUT: api/ClaimsWebApi/RejectClaim/5
            [HttpPut("RejectClaim/{id}")]
            public async Task<IActionResult> RejectClaim(int id)
            {
                try
                {
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

