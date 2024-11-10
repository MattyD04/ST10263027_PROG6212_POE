using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using ST10263027_PROG6212_POE.Models;
using ST10263027_PROG6212_POE.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ST10263027_PROG6212_POE.Roles;
using System.ComponentModel.DataAnnotations;
using FluentValidation;
using ST10263027_PROG6212_POE.Views.Home;

namespace ST10263027_PROG6212_POE.Controllers
{
    public class HomeController : Controller
    {
            private readonly ILogger<HomeController> _logger;
            private readonly AppDbContext _context;
            private readonly IValidator<ClaimViewModel> _validator;

            public HomeController(ILogger<HomeController> logger, AppDbContext context, IValidator<ClaimViewModel> validator)
            {
                _logger = logger;
                _context = context;
                _validator = validator;
            }


            public IActionResult Index()
        {
            return View();
        }

        [AuthorisingRoles("Lecturer")] //restricts access to the claim submission form for only lecturers
        public IActionResult Privacy()
        {
            return View();
        }

        [AuthorisingRoles("Lecturer")] //restricts access to the tracking of claims page for only lecturers
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
                TempData["ErrorMessage"] = "Claim Number is required.";
                return View("TrackClaims");
            }

            var claim = await _context.Claims
                .FirstOrDefaultAsync(c => c.ClaimNum == claim_number);

            if (claim == null)
            {
                TempData["ErrorMessage"] = "Claim not found for the provided claim number.";
                return View("TrackClaims");
            }

            ViewBag.ClaimStatus = claim.ClaimStatus;
            return View("TrackClaims");
        }
        //***************************************************************************************//
        [AuthorisingRoles("Manager", "Coordinator")] //restricts access to the verifying of claims page to only be accessible by managers and coordinators
        public IActionResult VerifyClaims()
        {
            return View();
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
        public IActionResult HumanResourcesLogin()
        {
            return View();
        }
        [AuthorisingRoles("HR")]
        public async Task<IActionResult> HRDashboard()
        {
            var approvedClaims = await _context.Claims
                .Include(c => c.Lecturer)
                .Where(c => c.ClaimStatus == "Approved")
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
                    Filename = claim.Filename,
                    ClaimStatus = claim.ClaimStatus
                })
                .ToListAsync();

            var viewModel = new HRDashboardViewModel
            {
                ApprovedClaims = approvedClaims
            };

            return View(viewModel);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Logout()
        {
            //Clears the session to log the user out
            HttpContext.Session.Clear();

            // Redirects to the home page
            return RedirectToAction("Index", "Home");
        }
    }
}
//*************************************End of file**************************************************//
