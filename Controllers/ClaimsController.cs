using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public ClaimsController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Privacy()
        {
            return View("Home/Privacy"); // Ensure it points to the correct view
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitClaim(string lecturer_number, string claim_number, DateTime submissionDate, double hoursWorked, double hourlyRate, string additionalNotes, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                var lecturer = await _context.Lecturers.FirstOrDefaultAsync(l => l.LecturerNum == lecturer_number);
                if (lecturer == null)
                {
                    ModelState.AddModelError("", "Lecturer not found.");
                    return View("Home/Privacy"); // Return Privacy view on error
                }

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
                lecturer.HoursWorked += hoursWorked;
                lecturer.HourlyRate = hourlyRate;

                await _context.SaveChangesAsync();

                // Set a success message to be displayed on the view
                TempData["SuccessMessage"] = "Your claim has been submitted successfully.";

                return View("Home/Privacy"); // Return the Privacy view after successful submission
            }

            return View("Home/Privacy"); // Return the Privacy view on error
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
