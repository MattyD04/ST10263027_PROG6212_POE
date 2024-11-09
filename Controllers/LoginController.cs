using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ST10263027_PROG6212_POE.Data;
using ST10263027_PROG6212_POE.Models;
using System.Text.RegularExpressions;

namespace ST10263027_PROG6212_POE.Controllers
{
    public class LoginController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public LoginController(
            AppDbContext context,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
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

        [HttpGet]
        public IActionResult HandleHRLogin()
        {
            return View("~/Views/Home/HumanResourcesLogin.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, bool isManager = false, bool isCoordinator = false, bool isHR = false)
        {
            var (isValidUsername, usernameErrors) = ValidateUsername(username);
            var (isValidPassword, passwordErrors) = ValidatePassword(password);

            if (!isValidUsername || !isValidPassword)
            {
                var errors = new List<string>();
                errors.AddRange(usernameErrors);
                errors.AddRange(passwordErrors);

                TempData["ErrorMessage"] = string.Join("\n", errors);
                return ReturnToAppropriateLoginView(isManager, isCoordinator, isHR);
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
            else if (isHR)
            {
                HttpContext.Session.SetString("UserType", "HR");
                return await HandleHRLoginPost(username, password);
            }
            else
            {
                HttpContext.Session.SetString("UserType", "Lecturer");
                return await HandleLecturerLoginPost(username, password);
            }
        }

        private IActionResult ReturnToAppropriateLoginView(bool isManager, bool isCoordinator, bool isHR)
        {
            if (isManager)
                return View("~/Views/Home/AcademicManagerLogin.cshtml");
            else if (isCoordinator)
                return View("~/Views/Home/ProgrammeCoordinatorLogin.cshtml");
            else if (isHR)
                return View("~/Views/Home/HumanResourcesLogin.cshtml");
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

        private async Task<IActionResult> HandleHRLoginPost(string username, string password)
        {
            var hrUser = await _context.HumanResource
                .FirstOrDefaultAsync(hr => hr.HumanResourcesNum == username);

            if (hrUser == null)
            {
                hrUser = new HRDashboard
                {
                    HumanResourcesNum = username,
                    Password = password
                };
                _context.HumanResource.Add(hrUser);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Human Resources account created successfully.";
            }
            else if (hrUser.Password != password)
            {
                TempData["ErrorMessage"] = "Invalid password.";
                return View("~/Views/Home/HumanResourcesLogin.cshtml");
            }

            var identityUser = await _userManager.FindByNameAsync(username);
            if (identityUser == null)
            {
                identityUser = new IdentityUser { UserName = username };
                await _userManager.CreateAsync(identityUser, password);
            }

            await _signInManager.SignInAsync(identityUser, isPersistent: false);

            if (!await _userManager.IsInRoleAsync(identityUser, "HumanResources"))
            {
                await _userManager.AddToRoleAsync(identityUser, "HumanResources");
            }

            return RedirectToAction("HRDashboard", "Home");
        }
    }
}
