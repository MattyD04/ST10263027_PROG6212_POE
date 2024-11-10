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
        [HttpPost]
        [AuthorisingRoles("HR")]
        public async Task<IActionResult> GenerateInvoice(int claimId)
        {
            try
            {
                // Fetch the claim details with related lecturer information
                var claim = await _context.Claims
                    .Include(c => c.Lecturer)
                    .FirstOrDefaultAsync(c => c.ClaimID == claimId);

                if (claim == null)
                {
                    TempData["Error"] = "Claim not found.";
                    return RedirectToAction("HRDashboard");
                }

                // Create the report viewer and set its properties
                var reportViewer = new ReportViewer
                {
                    ProcessingMode = ProcessingMode.Remote,
                    SizeToReportContent = true,
                    ServerReport = {
                    ReportServerUrl = new Uri(_reportServerUrl),
                    ReportPath = "/LecturerInvoices/LecturerInvoice"
                }
                };

                // Create parameters for the report
                var parameters = new List<ReportParameter>
            {
                new ReportParameter("ClaimID", claim.ClaimID.ToString()),
                new ReportParameter("ClaimNumber", claim.ClaimNum),
                new ReportParameter("LecturerNumber", claim.Lecturer.LecturerNum),
                new ReportParameter("SubmissionDate", claim.SubmissionDate.ToString("yyyy-MM-dd")),
                new ReportParameter("HoursWorked", claim.Lecturer.HoursWorked.ToString()),
                new ReportParameter("HourlyRate", claim.Lecturer.HourlyRate.ToString("C")),
                new ReportParameter("TotalAmount", (claim.Lecturer.HoursWorked * claim.Lecturer.HourlyRate).ToString("C")),
                new ReportParameter("InvoiceDate", DateTime.Now.ToString("yyyy-MM-dd"))
            };

                reportViewer.ServerReport.SetParameters(parameters);

                // Render the report to PDF
                byte[] bytes = reportViewer.ServerReport.Render("PDF");

                // Generate a unique filename
                string fileName = $"Invoice_{claim.ClaimNum}_{DateTime.Now:yyyyMMddHHmmss}.pdf";

                // Update the claim to mark invoice as generated
                claim.InvoiceGenerated = true;
                claim.InvoiceGeneratedDate = DateTime.Now;
                await _context.SaveChangesAsync();

                // Return the PDF file
                return File(bytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error generating invoice: {ex.Message}");
                TempData["Error"] = "Error generating invoice. Please try again later.";
                return RedirectToAction("HRDashboard");
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
