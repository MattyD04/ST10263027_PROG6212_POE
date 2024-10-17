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
                academicManager = new AcademicManager //if manger is signing in for the first time, this creates a new manager
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
                TempData["ErrorMessage"] = "Please enter both Username and Password."; //displays if a Coordinator does not enter their username and password
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

                TempData["SuccessMessage"] = "Programme Coordinator account created successfully."; //message displays if a Programme Coordinator signs in successfully
            }
            else if (coordinator.Password != password) 
            {
                TempData["ErrorMessage"] = "Invalid password."; //if the Programme Coordinator is logging in and the password does not match their number, this message displays
                return View("~/Views/Home/ProgrammeCoordinatorLogin.cshtml"); //redirects back to the login page so that the coordinator can try logging in again
            }

            return RedirectToAction("VerifyClaims", "Home");//directs to the Verify Claims form once the Coordinator is logged in successfully
        }

        private async Task<IActionResult> HandleLecturerLogin(string username, string password) //this method handles the login for the lecturer
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

                TempData["SuccessMessage"] = "Lecturer account created successfully."; //displays if the signing in/logging in of lecturers is successful
            }
            else if (lecturer.Password != password)
            {
                TempData["ErrorMessage"] = "Invalid password."; //if the password does not match a lecturer's number, this message displays
                return View("~/Views/Home/LecturerLogin.cshtml"); //returns back to the login view so the lecturer can try again
            }

            return RedirectToAction("Privacy", "Home"); //directs to the claims submission form if signing in/logging is successful
        }

        public IActionResult Index()
        {
            return View("~/Views/Home/Index.cshtml");
        }
    }
}