﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ST10263027_PROG6212_POE.Data;
using ST10263027_PROG6212_POE.Models;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IActionResult> SubmitClaim( //method to handle the submission of claims (corrections by Claude AI)
           string lecturer_number,
           string claim_number,
           DateTime submissionDate,
           double hoursWorked,
           double hourlyRate,
           string additionalNotes,
           IFormFile file)
        {
            // Checks to see if the model state is valid
            if (ModelState.IsValid)
            {
                // Validates file size if file exists
                if (file != null)
                {
                    if (file.Length > MaxFileSizeBytes)
                    {
                        ModelState.AddModelError("file", "The file size exceeds the limit of 5 MB.");
                        TempData["ErrorMessage"] = "The file size exceeds the limit of 5 MB.";
                        return View();
                    }

                    // Validates file extension
                    string[] allowedExtensions = { ".pdf", ".docx", ".xlsx" };
                    var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError("file", "Only .pdf, .docx, and .xlsx files are allowed.");
                        TempData["ErrorMessage"] = "Only .pdf, .docx, and .xlsx files are allowed.";
                        return View();
                    }
                }

                // Creates Lecturer object if it does not exist
                var lecturer = new Lecturer
                {
                    LecturerNum = lecturer_number,
                    HourlyRate = hourlyRate,
                    HoursWorked = hoursWorked,
                    Password = "default_password" // Assigns a default password (for error handling)
                };

                // Add lecturer to the database
                _context.Lecturers.Add(lecturer);
                await _context.SaveChangesAsync();

                // Creates a Claim object if it does not exist
                var claim = new Claim
                {
                    ClaimNum = claim_number,
                    LecturerID = lecturer.LecturerId,
                    SubmissionDate = submissionDate,
                    ClaimStatus = "Pending",
                    Comments = additionalNotes
                };

                // Process file if it exists
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

                // Add claim to the database
                _context.Claims.Add(claim);
                await _context.SaveChangesAsync();

                
                TempData["SuccessMessage"] = "Your claim has been submitted successfully."; //message displays if the claim has been submitted successfully
                TempData["UploadedFileName"] = claim.Filename; //displays the name of the file submitted

                return RedirectToAction("Privacy", "Home"); //redirects back to the Claim submission form so more claims can be submitted
            }

            // If model state is not valid, return to the form
            return View();
        }

        
    }
}
