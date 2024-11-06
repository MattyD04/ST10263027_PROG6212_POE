using Microsoft.AspNetCore.Mvc;
using ST10263027_PROG6212_POE.Models;
using ST10263027_PROG6212_POE.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
//code corrections done by Claude AI
//this file handles the Web API responsible for handling communications between the front end and back end systems
namespace ST10263027_PROG6212_POE.Controllers
{
    [Authorize(Roles = "ProgrammeCoordinator,AcademicManager")] //ensuring that the relevant parties can access the API end points
    [Route("api/[controller]")]
    [ApiController]
    public class WebApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public WebApiController(AppDbContext context)
        {
            _context = context;
        }
        //****************************************************************************************************************************//
        // Endpoint to approve a claim
        [HttpPost("approve/{id}")]
        public async Task<IActionResult> ApproveClaim(int id)
        {
            var claim = await _context.Claims.FindAsync(id);
            if (claim == null)
            {
                return NotFound("Claim not found");
            }

            if (claim.Lecturer.HoursWorked > 0 && claim.Lecturer.HourlyRate > 0)
            {
                claim.ClaimStatus = "Approved";
                await _context.SaveChangesAsync();
                return Ok(new { message = "Claim approved successfully" });
            }
            return BadRequest("Claim does not meet the criteria for approval");
        }
        //****************************************************************************************************************************//
        // Endpoint to reject a claim
        [HttpPost("reject/{id}")]
        public async Task<IActionResult> RejectClaim(int id)
        {
            var claim = await _context.Claims.FindAsync(id);
            if (claim == null)
            {
                return NotFound("Claim not found");
            }

            claim.ClaimStatus = "Rejected";
            await _context.SaveChangesAsync();
            return Ok(new { message = "Claim rejected successfully" });
        }

    }
}
//***************************************************End of file**************************************************//