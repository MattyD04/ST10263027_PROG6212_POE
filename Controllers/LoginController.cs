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
        public IActionResult HandleCoordinatorLoginn()
        {
            return View();
        }
        
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, bool isManager = false, bool isCoordinator = false)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                TempData["ErrorMessage"] = "Please enter both Username and Password.";
                return View(); // Stay on the same page
            }
            if (isManager)
            {
                HttpContext.Session.SetString("UserType", "Manager"); //assigns the role of academic manager upon signing in
                return await HandleManagerLogin(username, password);
            }
            else if (isCoordinator)
            {
                HttpContext.Session.SetString("UserType", "Coordinator"); //assigns the role of programme coordinator upon signing in
                return await HandleCoordinatorLogin(username, password);
            }
            else
            {
                HttpContext.Session.SetString("UserType", "Lecturer"); //assigns the role of programme coordinator upon signing in
                return await HandleLecturerLogin(username, password);
            }
        }
        //***************************************************************************************//
        private async Task<IActionResult> HandleManagerLogin(string username, string password)
        {
            // Handle login for Academic Manager
            var academicManager = await _context.AcademicManagers
                .FirstOrDefaultAsync(am => am.ManagerNum == username);
            if (academicManager == null)
            {
                // If the manager doesn't exist, create a new one
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
                // Invalid password
                TempData["ErrorMessage"] = "Invalid password.";
                return View("AcademicManagerLogin"); // Return to the Academic Manager login page
            }
            // Redirect to VerifyClaims for Academic Manager
            return RedirectToAction("VerifyClaims", "Home");
        }
        //***************************************************************************************//
        [HttpPost]
        public async Task<IActionResult> HandleCoordinatorLogin(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                TempData["ErrorMessage"] = "Please enter both Username and Password.";
                return View(); // Return to the same view
            }
            // Handle login for Programme Coordinator
            var coordinator = await _context.ProgrammeCoordinators
                .FirstOrDefaultAsync(pc => pc.CoordinatorNum == username);
            if (coordinator == null)
            {
                // If the coordinator doesn't exist, create a new one
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
                // Invalid password
                TempData["ErrorMessage"] = "Invalid password.";
                return View(); // Return to the same view
            }
            // Redirect to VerifyClaims for Programme Coordinator
            return RedirectToAction("VerifyClaims", "Home");
        }
        //***************************************************************************************//
        private async Task<IActionResult> HandleLecturerLogin(string username, string password)
        {
            // Handle login for Lecturer
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
                return View(); // Stay on the same page
            }
            // Redirect to Privacy for Lecturer
            return RedirectToAction("Privacy", "Home");
        }
        //***************************************************************************************//
        public IActionResult Index()
        {
            return View();
        }
    }
    //***********************************End of file*************************************************//
}