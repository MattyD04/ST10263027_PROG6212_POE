using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ST10263027_PROG6212_POE.Models;
using ST10263027_PROG6212_POE.Data;
using System.Diagnostics;
using System.Linq;

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

        public IActionResult VerifyClaims() //method that handles the verification of claims by passing it to the table in the VerifyClaims View
        {
            var claimViewModels = _context.Claims
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
                .ToList();

            return View(claimViewModels);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveClaim(int id) //method that handles the approval of claims
        {
            var claim = await _context.Claims.FindAsync(id);
            if (claim != null)
            {
                claim.ClaimStatus = "Approved";  //changes the claim status from pending to accepted
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Claim has been approved successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Claim not found.";
            }

            return RedirectToAction(nameof(VerifyClaims));
        }

        [HttpPost]
        public async Task<IActionResult> RejectClaim(int id) //method that handles the rejection of claims
        {
            var claim = await _context.Claims.FindAsync(id);
            if (claim != null)
            {
                claim.ClaimStatus = "Rejected"; //changes the claim status from pending to rejected
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Claim has been rejected successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Claim not found.";
            }

            return RedirectToAction(nameof(VerifyClaims));
        }

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