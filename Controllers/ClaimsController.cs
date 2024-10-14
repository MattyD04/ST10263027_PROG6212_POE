using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using ST10263027_PROG6212_POE.Data;
using ST10263027_PROG6212_POE.Models;

namespace ST10263027_PROG6212_POE.Controllers
{
    public class ClaimsController : Controller
    {
        private readonly AppDbContext _context;
        private const int MaxFileSizeBytes = 5 * 1024 * 1024; // limits the size of a file uploaded

        public ClaimsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken] 
        //code corrections and changes by Claude AI
        public async Task<IActionResult> SubmitClaim(
            string lecturer_number,
            string claim_number,
            DateTime submissionDate,
            double hoursWorked,
            double hourlyRate,
            string additionalNotes,
            IFormFile file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    if (file.Length > MaxFileSizeBytes)
                    {
                        ModelState.AddModelError("file", "The file size exceeds the limit of 5 MB.");
                        TempData["ErrorMessage"] = "The file size exceeds the limit of 5 MB.";
                        return View("~/Views/Home/Privacy.cshtml");
                    }

                    string[] allowedExtensions = { ".pdf", ".docx", ".xlsx" };
                    var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError("file", "Only .pdf, .docx, and .xlsx files are allowed.");
                        TempData["ErrorMessage"] = "Only .pdf, .docx, and .xlsx files are allowed.";
                        return View("~/Views/Home/Privacy.cshtml");
                    }
                }

                var lecturer = new Lecturer
                {
                    LecturerNum = lecturer_number,
                    HourlyRate = hourlyRate,
                    HoursWorked = hoursWorked
                };
                _context.Lecturers.Add(lecturer);
                await _context.SaveChangesAsync();

                var claim = new Claim
                {
                    ClaimNum = claim_number,
                    LecturerID = lecturer.LecturerId,
                    SubmissionDate = submissionDate,
                    ClaimStatus = "Pending",
                    Comments = additionalNotes
                };

                if (file != null && file.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await file.CopyToAsync(memoryStream);
                        claim.FileData = memoryStream.ToArray();
                        claim.Filename = file.FileName;
                        claim.ContentType = file.ContentType;
                    }
                }

                _context.Claims.Add(claim);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Your claim has been submitted successfully.";
                TempData["UploadedFileName"] = claim.Filename;
                return View("~/Views/Home/Privacy.cshtml");
            }

            return View("~/Views/Home/Privacy.cshtml");
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}