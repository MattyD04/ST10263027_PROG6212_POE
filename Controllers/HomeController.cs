using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using ST10263027_PROG6212_POE.Models;
using ST10263027_PROG6212_POE.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ST10263027_PROG6212_POE.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult TrackClaims()
        {
            return View();
        }

        //***************************************************************************************//
        [HttpPost]
        public async Task<IActionResult> TrackClaim(string claim_number)
        {
            if (string.IsNullOrEmpty(claim_number))
            {
                TempData["ErrorMessage"] = "Claim Number is required."; // If a user tries to track a claim without inputting the number
                return View("TrackClaims");
            }
            var claim = await _context.Claims
                .FirstOrDefaultAsync(c => c.ClaimNum == claim_number);
            if (claim == null)
            {
                TempData["ErrorMessage"] = "Claim not found for the provided claim number."; // If a user enters an invalid claim number
                return View("TrackClaims");
            }
            ViewBag.ClaimStatus = claim.ClaimStatus;
            return View("TrackClaims");
        }

        //***************************************************************************************//
        public IActionResult VerifyClaims()
        {
            var claimViewModels = _context.Claims
                .Where(claim => claim.ClaimStatus == "Pending")
                .Select(claim => new ClaimViewModel
                {
                    ClaimID = claim.ClaimID, // Fetches the ClaimID
                    ClaimNum = claim.ClaimNum, // Fetches the Claim Number
                    LecturerNum = claim.Lecturer.LecturerNum, // Fetches the Lecturer Number
                    SubmissionDate = claim.SubmissionDate, // Fetches the submission date of the claim
                    HoursWorked = claim.Lecturer.HoursWorked, // Fetches the lecturer's hours worked
                    HourlyRate = claim.Lecturer.HourlyRate, // Fetches the lecturer's hourly rate
                    TotalAmount = claim.Lecturer.HoursWorked * claim.Lecturer.HourlyRate, // Calculates the total amount of the contract
                    Comments = claim.Comments, // Fetches the comments submitted by the lecturer
                    Filename = claim.Filename // Fetches the name of the file submitted by the lecturer
                })
                .ToList();

            

            return View(claimViewModels); // Return the view after all validations
        }

        //***************************************************************************************//
        [HttpPost]
        public async Task<IActionResult> ProcessClaim(int id) // Automated processing of claim
        {
            var claim = await _context.Claims.FindAsync(id);
            if (claim != null)
            {
                // Recalculate the total amount
                double totalAmount = claim.Lecturer.HoursWorked * claim.Lecturer.HourlyRate;

                // Automated approval/rejection criteria based on total amount
                if (totalAmount > 1000) // Example threshold: if TotalAmount > R1000, approve
                {
                    claim.ClaimStatus = "Approved"; // Automatically approve
                    TempData["SuccessMessage"] = $"Claim has been approved successfully. Total amount: R{totalAmount}.";
                }
                else
                {
                    claim.ClaimStatus = "Rejected"; // Automatically reject
                    TempData["ErrorMessage"] = $"Claim has been rejected due to low amount. Total amount: R{totalAmount}.";
                }

                // Save the changes to the database
                await _context.SaveChangesAsync();
            }
            else
            {
                TempData["ErrorMessage"] = "Claim not found."; // Claim not found
            }

            return RedirectToAction(nameof(VerifyClaims)); // Redirect back to the Verify Claims page
        }

        //***************************************************************************************//
        public IActionResult LecturerLogin()
        {
            return View();
        }

        public IActionResult AcademicManagerLogin()
        {
            return View();
        }

        public IActionResult ProgrammeCoordinatorLogin()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
