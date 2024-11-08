using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ST10263027_PROG6212_POE.Data;
using ST10263027_PROG6212_POE.Models;
using System.Text.RegularExpressions;

namespace ST10263027_PROG6212_POE.Controllers
{
    public class LoginController : Controller
    {
        private readonly AppDbContext _context;
        public LoginController(AppDbContext context)
        {
            _context = context;
        }

        private (bool isValid, string errorMessage) ValidatePassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                return (false, "Password cannot be empty.");

            if (password.Length < 8)
                return (false, "Password must be at least 8 characters long.");

            if (!Regex.IsMatch(password, @"[A-Z]"))
                return (false, "Password must contain at least one uppercase letter.");

            if (!Regex.IsMatch(password, @"[a-z]"))
                return (false, "Password must contain at least one lowercase letter.");

            if (!Regex.IsMatch(password, @"[0-9]"))
                return (false, "Password must contain at least one number.");

            if (!Regex.IsMatch(password, @"[!@#$%^&*(),.?"":{}|<>]"))
                return (false, "Password must contain at least one special character.");

            return (true, string.Empty);
        }

        [HttpGet]
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult HandleCoordinatorLogin()
        {
            return View("~/Views/Home/ProgrammeCoordinatorLogin.cshtml");
        }

        [HttpGet]
        public IActionResult HandleManagerLogin()
        {
            return View("~/Views/Home/AcademicManagerLogin.cshtml");
        }

        [HttpGet]
        public IActionResult HandleLecturerLogin()
        {
            return View("~/Views/Home/LecturerLogin.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, bool isManager = false, bool isCoordinator = false)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                TempData["ErrorMessage"] = "Please enter both Username and Password.";
                return ReturnToAppropriateLoginView(isManager, isCoordinator);
            }

            var (isValid, errorMessage) = ValidatePassword(password);
            if (!isValid)
            {
                TempData["ErrorMessage"] = errorMessage;
                return ReturnToAppropriateLoginView(isManager, isCoordinator);
            }

            if (isManager)
            {
                HttpContext.Session.SetString("UserType", "Manager");
                return await HandleManagerLoginPost(username, password);
            }
            else if (isCoordinator)
            {
                HttpContext.Session.SetString("UserType", "Coordinator");
                return await HandleCoordinatorLoginPost(username, password);
            }
            else
            {
                HttpContext.Session.SetString("UserType", "Lecturer");
                return await HandleLecturerLoginPost(username, password);
            }
        }

        private IActionResult ReturnToAppropriateLoginView(bool isManager, bool isCoordinator)
        {
            if (isManager)
                return View("~/Views/Home/AcademicManagerLogin.cshtml");
            else if (isCoordinator)
                return View("~/Views/Home/ProgrammeCoordinatorLogin.cshtml");
            else
                return View("~/Views/Home/LecturerLogin.cshtml");
        }

        private async Task<IActionResult> HandleManagerLoginPost(string username, string password)
        {
            var academicManager = await _context.AcademicManagers
                .FirstOrDefaultAsync(am => am.ManagerNum == username);

            if (academicManager == null)
            {
                academicManager = new AcademicManager
                {
                    ManagerNum = username,
                    Password = password
                };
                _context.AcademicManagers.Add(academicManager);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Academic Manager account created successfully.";
            }
            else if (academicManager.Password != password)
            {
                TempData["ErrorMessage"] = "Invalid password.";
                return View("~/Views/Home/AcademicManagerLogin.cshtml");
            }

            return RedirectToAction("VerifyClaims", "Home");
        }

        private async Task<IActionResult> HandleCoordinatorLoginPost(string username, string password)
        {
            var coordinator = await _context.ProgrammeCoordinators
                .FirstOrDefaultAsync(pc => pc.CoordinatorNum == username);

            if (coordinator == null)
            {
                coordinator = new ProgrammeCoordinator
                {
                    CoordinatorNum = username,
                    Password = password
                };
                _context.ProgrammeCoordinators.Add(coordinator);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Programme Coordinator account created successfully.";
            }
            else if (coordinator.Password != password)
            {
                TempData["ErrorMessage"] = "Invalid password.";
                return View("~/Views/Home/ProgrammeCoordinatorLogin.cshtml");
            }

            return RedirectToAction("VerifyClaims", "Home");
        }

        private async Task<IActionResult> HandleLecturerLoginPost(string username, string password)
        {
            var lecturer = await _context.Lecturers
                .FirstOrDefaultAsync(l => l.LecturerNum == username);

            if (lecturer == null)
            {
                lecturer = new Lecturer
                {
                    LecturerNum = username,
                    Password = password
                };
                _context.Lecturers.Add(lecturer);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Lecturer account created successfully.";
            }
            else if (lecturer.Password != password)
            {
                TempData["ErrorMessage"] = "Invalid password.";
                return View("~/Views/Home/LecturerLogin.cshtml");
            }

            return RedirectToAction("Privacy", "Home");
        }
    }
}