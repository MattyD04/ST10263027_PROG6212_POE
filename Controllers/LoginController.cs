using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ST10263027_PROG6212_POE.Data;
using ST10263027_PROG6212_POE.Models;

namespace ST10263027_PROG6212_POE.Controllers
{
    public class LoginController : Controller
    {
        private readonly AppDbContext _context;

        public LoginController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult HandleCoordinatorLogin() 
        {
            return View("~/Views/Home/ProgrammeCoordinatorLogin.cshtml");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View("~/Views/Home/LecturerLogin.cshtml");
        }
        
        [HttpGet]
        public IActionResult HandleManagerLogin()
        {
            return View("~/Views/Home/AcademicManagerLogin.cshtml");
        }


        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, bool isManager = false, bool isCoordinator = false)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                TempData["ErrorMessage"] = "Please enter both Username and Password.";
                return View("~/Views/Home/LecturerLogin.cshtml");
            }

            if (isManager)
            {
                return await HandleManagerLogin(username, password);
            }
            else if (isCoordinator)
            {
                return await HandleCoordinatorLogin(username, password);
            }
            else
            {
                return await HandleLecturerLogin(username, password);
            }
        }

        private async Task<IActionResult> HandleManagerLogin(string username, string password) //method to handle the login of an Academic Manager (code corrections done by Claude AI)
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

                TempData["SuccessMessage"] = "Academic Manager account created successfully. Please log in."; //displays if manager logged in correctly
                return View("~/Views/Home/AcademicManagerLogin.cshtml"); //directs to the Academic Manager view
            }
            else if (academicManager.Password != password)
            {
                TempData["ErrorMessage"] = "Invalid password.";
                return View("~/Views/Home/AcademicManagerLogin.cshtml"); //displays if the password already stored in the DB does not match the manager's number
            }

            // Successful login
            return RedirectToAction("VerifyClaims", "Home"); //after a successful login, the manager is redirected to the relevant page
        }

        [HttpPost]
        public async Task<IActionResult> HandleCoordinatorLogin(string username, string password)//method to handle the login of a programme coordinator
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                TempData["ErrorMessage"] = "Please enter both Username and Password.";
                return View("~/Views/Home/ProgrammeCoordinatorLogin.cshtml");
            }

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

        private async Task<IActionResult> HandleLecturerLogin(string username, string password)
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

        public IActionResult Index()
        {
            return View("~/Views/Home/Index.cshtml");
        }
    }
}