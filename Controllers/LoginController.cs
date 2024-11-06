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
                TempData["ErrorMessage"] = "Please enter both Username and Password."; //displays if fields are empty
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

        //****************************************************************************************************************************//
        private async Task<IActionResult> HandleManagerLogin(string username, string password) //method to handle the login of an Academic Manager
        {
            var academicManager = await _context.AcademicManagers
                .FirstOrDefaultAsync(am => am.ManagerNum == username);

            if (academicManager == null)
            {
                academicManager = new AcademicManager //if manager is signing in for the first time, this creates a new manager
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
                TempData["ErrorMessage"] = "Invalid password."; //displays if the stored password does not match
                return View("~/Views/Home/AcademicManagerLogin.cshtml");
            }

            // Successful login
            return RedirectToAction("VerifyClaims", "Home"); //redirects to the relevant page after a successful login
        }

        //****************************************************************************************************************************//
        [HttpPost]
        public async Task<IActionResult> HandleCoordinatorLogin(string username, string password) //method to handle the login of a programme coordinator
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                TempData["ErrorMessage"] = "Please enter both Username and Password."; //displays if fields are empty
                return View("~/Views/Home/ProgrammeCoordinatorLogin.cshtml"); //redirects back to the login page
            }

            var coordinator = await _context.ProgrammeCoordinators
                .FirstOrDefaultAsync(pc => pc.CoordinatorNum == username);

            if (coordinator == null)
            {
                coordinator = new ProgrammeCoordinator //if a Coordinator is signing in for the first time, this creates a new Coordinator
                {
                    CoordinatorNum = username,
                    Password = password
                };

                _context.ProgrammeCoordinators.Add(coordinator);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Programme Coordinator account created successfully."; //message displays if successfully logged in
            }
            else if (coordinator.Password != password)
            {
                TempData["ErrorMessage"] = "Invalid password."; //displays if the password does not match
                return View("~/Views/Home/ProgrammeCoordinatorLogin.cshtml"); //redirects back to the login page for retry
            }

            return RedirectToAction("VerifyClaims", "Home"); //redirects to the Verify Claims form after successful login
        }

        //****************************************************************************************************************************//
        private async Task<IActionResult> HandleLecturerLogin(string username, string password) //method to handle the login for the lecturer
        {
            var lecturer = await _context.Lecturers
                .FirstOrDefaultAsync(l => l.LecturerNum == username);

            if (lecturer == null)
            {
                lecturer = new Lecturer //if a lecturer is signing in for the first time, this creates a new lecturer
                {
                    LecturerNum = username,
                    Password = password
                };

                _context.Lecturers.Add(lecturer);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Lecturer account created successfully."; //displays if successfully logged in
            }
            else if (lecturer.Password != password)
            {
                TempData["ErrorMessage"] = "Invalid password."; //displays if the password does not match
                return View("~/Views/Home/LecturerLogin.cshtml"); //returns back to the login view for retry
            }

            return RedirectToAction("Privacy", "Home"); //redirects to the claims submission form upon successful login
        }

        //****************************************************************************************************************************//
        public IActionResult Index()
        {
            return View("~/Views/Home/Index.cshtml");
        }
    }
}
//****************************************end of file***********************************************//
