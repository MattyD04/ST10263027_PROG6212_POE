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
using Microsoft.Extensions.Configuration;
using Microsoft.Reporting.WebForms;
using System.IO;
using System.Data;
using System.Net;

namespace ST10263027_PROG6212_POE.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        private readonly IValidator<ClaimViewModel> _validator;
        private readonly string _reportServerUrl;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger,
                     AppDbContext context,
                     IValidator<ClaimViewModel> validator,
                     IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _validator = validator;
            _configuration = configuration;
            _reportServerUrl = _configuration["ReportServer:Url"];
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
        public async Task<IActionResult> HRDashboard() //method for putting all the approved claims into the table in the HR Dashboard
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
            var lecturers = await _context.Lecturers.ToListAsync();
            var viewModel = new HRDashboardViewModel
            {
                ApprovedClaims = approvedClaims,
                Lecturers = lecturers
            };

            return View(viewModel);
        }
        private string GetReportUrl(int claimId)
        {
            var reportUrl = $"{_reportServerUrl}/Reports/render/ClaimInvoice?rs:Format=PDF&ClaimID={claimId}";
            _logger.LogInformation("Generated report URL: " + reportUrl);
            return reportUrl;
        }

        [HttpPost]
        [AuthorisingRoles("HR")]
        public async Task<IActionResult> GenerateInvoice(int claimId)
        {
            try
            {
                var reportUrl = GetReportUrl(claimId);

                using var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(reportUrl);
                response.EnsureSuccessStatusCode();

                var pdfBytes = await response.Content.ReadAsByteArrayAsync();
                return File(pdfBytes, "application/pdf", $"Invoice_{claimId}_{DateTime.Now:yyyyMMdd}.pdf");
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Error generating invoice: " + ex.Message;
                return RedirectToAction("HRDashboard");
            }
        }


        [HttpPost]
        [AuthorisingRoles("HR")]
        public async Task<IActionResult> UpdateLecturerPassword(int lecturerId, string newPassword)
        {
            try
            {
                var lecturer = await _context.Lecturers.FindAsync(lecturerId);
                if (lecturer == null)
                {
                    return Json(new { success = false, message = "Lecturer not found" });
                }

                lecturer.Password = newPassword;
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Password updated successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error updating password" });
            }
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
