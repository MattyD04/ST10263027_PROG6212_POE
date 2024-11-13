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
        public async Task<IActionResult> TrackClaim(string claim_number) //method for tracking a claim submitted by a lecturer
        {
            if (string.IsNullOrEmpty(claim_number))
            {
                TempData["ErrorMessage"] = "Claim Number is required."; //error message that displays if a user attempts to track a claim without entering the claim number first
                return View("TrackClaims"); //returns back to the Track Claims page after error message displays
            }

            var claim = await _context.Claims
                .FirstOrDefaultAsync(c => c.ClaimNum == claim_number);

            if (claim == null)
            {
                TempData["ErrorMessage"] = "Claim not found for the provided claim number."; //error message that displays if an incorrect claim number is entered
                return View("TrackClaims");//returns back to the Track Claims page after error message displays
            }

            ViewBag.ClaimStatus = claim.ClaimStatus;
            return View("TrackClaims"); //once the claim status is retrieved (pending, approved or rejected) then the user is returned to the track claims page with the claim status displaying
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
        public async Task<IActionResult> HRDashboard() //method for putting all the approved claims into the table in the HR Dashboard (corrections done by Claude AI)
        {
            var approvedClaims = await _context.Claims
                .Include(c => c.Lecturer)
                .Where(c => c.ClaimStatus == "Approved")
                .Select(claim => new ClaimViewModel
                {
                    ClaimID = claim.ClaimID, //this fetches the ClaimID from the Claim table
                    ClaimNum = claim.ClaimNum, //this fetches the Claim Number from the Claim table
                    LecturerNum = claim.Lecturer.LecturerNum, //this fetches the lecturer number from the lecturer table
                    SubmissionDate = claim.SubmissionDate, //this fetches the submission date for a claim from the claim table
                    HoursWorked = claim.Lecturer.HoursWorked, //this fetches the lecturer's hours worked from the lecturer table
                    HourlyRate = claim.Lecturer.HourlyRate,  //this fetches the lecturer's hourly rate from the lecturer table
                    TotalAmount = claim.Lecturer.HoursWorked * claim.Lecturer.HourlyRate, //calculates the total amount of a lecturer's hours worked and hourly rate 
                    Comments = claim.Comments, //this fetches the a claim's comments from the Claim table
                    Filename = claim.Filename, //this fetches the name of a file from the Claim table
                    ClaimStatus = claim.ClaimStatus //this fetches a claim's status from the claim table to ensure only approved claims are fetched and not all claims
                })
                .ToListAsync();
            var lecturers = await _context.Lecturers.ToListAsync(); 
            var viewModel = new HRDashboardViewModel
            {
                ApprovedClaims = approvedClaims, //fetching all the approved claims that will be displayed in the HR dashboard
                Lecturers = lecturers //fetching all the lecturers so that the data will be displayed in the HR dashboard
            };

            return View(viewModel);
        }
        //***************************************************************************************//
        private string GetReportUrl(int claimId) //this method fetches the URL for the SSRS report which then gets passed to the GenerateInvoice method
        {
            var reportUrl = $"{_reportServerUrl}/Reports/render/ClaimInvoice?rs:Format=PDF&ClaimID={claimId}";
            _logger.LogInformation("Generated report URL: " + reportUrl);
            return reportUrl;
        }
        //***************************************************************************************//
        [HttpPost]
        [AuthorisingRoles("HR")]
        public async Task<IActionResult> GenerateInvoice(int claimId) //this method handles the generating of invoices for each approved claim in a PDF form (corrections done by Claude AI to fix exceptions that keep getting thrown)
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
                TempData["Message"] = "Error generating invoice: " + ex.Message; //error message that displays if there is an issue generating an invoice
                return RedirectToAction("HRDashboard"); //returns the user back to the HR Dashboard
            }
        }

        //***************************************************************************************//
        [HttpPost]
        [AuthorisingRoles("HR")]
        public async Task<IActionResult> UpdateLecturerPassword(int lecturerId, string newPassword) //this method handles the updating of a lecturer's password
        {
            try
            {
                var lecturer = await _context.Lecturers.FindAsync(lecturerId);
                if (lecturer == null)
                {
                    return Json(new { success = false, message = "Lecturer not found" }); //error message that displays if a lecturer is not found
                }

                lecturer.Password = newPassword;
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Password updated successfully" }); //success message that displays if a password is correctly updated
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error updating password" }); //error message that displays if there is a problem updating a password
            }
        }
        //***************************************************************************************//
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Logout() //method for logging a user out
        {
            //Clears the session to log the user out
            HttpContext.Session.Clear();

            // Redirects to the home page
            return RedirectToAction("Index", "Home");
        }
    }
}
//*************************************End of file**************************************************//
