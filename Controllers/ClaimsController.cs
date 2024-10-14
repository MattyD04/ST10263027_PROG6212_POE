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

        public ClaimsController(AppDbContext context)
        {
            _context = context;
        }

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
            if (ModelState.IsValid)
            {
                // Creates a new lecturer
                var lecturer = new Lecturer
                {
                    LecturerNum = lecturer_number,
                    HourlyRate = hourlyRate,
                    HoursWorked = hoursWorked
                };

                _context.Lecturers.Add(lecturer);
                await _context.SaveChangesAsync();

                // Creates a new claim
                var claim = new Claim
                {
                    ClaimNum = claim_number,
                    LecturerID = lecturer.LecturerId,
                    SubmissionDate = submissionDate,
                    ClaimStatus = "Pending",
                    Comments = additionalNotes,
                    
                };

                // This sectiion handles the uploading of a file
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
//-----------------------------------End of file--------------------------------------------//
