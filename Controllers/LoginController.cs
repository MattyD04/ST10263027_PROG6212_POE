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

        private (bool isValid, List<string> errorMessages) ValidatePassword(string password)
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(password))
            {
                errors.Add("Password cannot be empty.");
                return (false, errors);
            }

            if (password.Length < 8)
                errors.Add("Password must be at least 8 characters long.");

            if (!Regex.IsMatch(password, @"[A-Z]"))
                errors.Add("Password must contain at least one uppercase letter.");

            if (!Regex.IsMatch(password, @"[a-z]"))
                errors.Add("Password must contain at least one lowercase letter.");

            if (!Regex.IsMatch(password, @"[0-9]"))
                errors.Add("Password must contain at least one number.");

            if (!Regex.IsMatch(password, @"[!@#$%^&*(),.?"":{}|<>]"))
                errors.Add("Password must contain at least one special character (e.g. !@#$%^&*(),.?\":{}|<>).");

            return (errors.Count == 0, errors);
        }

        private (bool isValid, List<string> errorMessages) ValidateUsername(string username)
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(username))
            {
                errors.Add("Username cannot be empty.");
                return (false, errors);
            }

            if (username.Length < 6 || !Regex.IsMatch(username, @"^\d+$"))
            {
                errors.Add("Username must be at least 6 numbers long.");
                return (false, errors);
            }

            return (true, errors);
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
            var (isValidUsername, usernameErrors) = ValidateUsername(username);
            var (isValidPassword, passwordErrors) = ValidatePassword(password);

            if (!isValidUsername || !isValidPassword)
            {
                var errors = new List<string>();
                errors.AddRange(usernameErrors);
                errors.AddRange(passwordErrors);

                // Use Environment.NewLine instead of HTML <br/> tags
                TempData["ErrorMessage"] = string.Join("\n", errors);
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