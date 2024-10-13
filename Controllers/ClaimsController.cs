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

        // Handles the submission of the claim form
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitClaim(
            string lecturer_number,
            string claim_number,
            DateTime submissionDate,
            double hoursWorked,
            double hourlyRate,
            string additionalNotes,
            IFormFile file)
        {
            // Check if the model is valid before processing the data
            if (ModelState.IsValid)
            {
                // Try to find the lecturer using the lecturer number provided
                var lecturer = await _context.Lecturers.FirstOrDefaultAsync(l => l.LecturerNum == lecturer_number);

                // If lecturer is not found, return an error
                if (lecturer == null)
                {
                    ModelState.AddModelError("", "Lecturer not found.");
                    return View("~/Views/Home/Privacy.cshtml"); // Return to the same form view with an error
                }

                // Create a new claim object with the provided form data
                var claim = new Claim
                {
                    ClaimNum = claim_number,
                    LecturerID = lecturer.LecturerId,
                    SubmissionDate = submissionDate,
                    ClaimStatus = "Pending",  // Default status is "Pending"
                    Comments = additionalNotes // Optional additional notes
                };

                // If a file is uploaded, store the file data
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

                // Add the claim to the database context
                _context.Claims.Add(claim);

                // Update the lecturer's hours worked and hourly rate
                lecturer.HoursWorked += hoursWorked;
                lecturer.HourlyRate = hourlyRate;

                // Save all the changes made to the context (Claim and Lecturer update)
                await _context.SaveChangesAsync();

                // Store a success message in TempData to show on the form
                TempData["SuccessMessage"] = "Your claim has been submitted successfully.";

                // Return to the form view after successful submission
                return View("~/Views/Home/Privacy.cshtml");
            }

            // If the model state is invalid, return the form view with validation errors
            return View("~/Views/Home/Privacy.cshtml");
        }

        // Default action for the Claims controller
        public IActionResult Index()
        {
            return View();
        }
    }
}
