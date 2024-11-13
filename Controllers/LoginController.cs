using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ST10263027_PROG6212_POE.Data;
using ST10263027_PROG6212_POE.Models;
using System.Text.RegularExpressions;

//this controller handles all actions related to the login functionality of the web application
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
        //***************************************************************************************//
        private (bool isValid, List<string> errorMessages) ValidatePassword(string password) //this handles all the requirements for passwords (corrections by Claude AI)
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(password))
            {
                errors.Add("Password cannot be empty."); //message that displays if a user attempts to sign/login without a password
                return (false, errors);
            }

            if (password.Length < 8)
                errors.Add("Password must be at least 8 characters long."); //specifies that the length of a password must be at least 8 characters long

            if (!Regex.IsMatch(password, @"[A-Z]"))
                errors.Add("Password must contain at least one uppercase letter."); //specifies that the a password must have at least one uppercase letter

            if (!Regex.IsMatch(password, @"[a-z]"))
                errors.Add("Password must contain at least one lowercase letter."); //specifies that the a password must have at least one lowercase letter

            if (!Regex.IsMatch(password, @"[0-9]"))
                errors.Add("Password must contain at least one number."); //specifies that the a password must have at least one number 

            if (!Regex.IsMatch(password, @"[!@#$%^&*(),.?"":{}|<>]"))
                errors.Add("Password must contain at least one special character (e.g. !@#$%^&*(),.?\":{}|<>)."); //specifies that the a password must have at least one special character in it

            return (errors.Count == 0, errors); //returns the appropriate error message 
        }
        //***************************************************************************************//
        private (bool isValid, List<string> errorMessages) ValidateUsername(string username) //this handles the validation of usernames to ensure they meet required standards
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(username))
            {
                errors.Add("Username cannot be empty."); //this message displays if a user attempts to sign in/login without entering in a username
                return (false, errors);
            }

            if (username.Length < 6 || !Regex.IsMatch(username, @"^\d+$"))
            {
                errors.Add("Username must be at least 6 numbers long."); //specifies that a username must be at least 6 numbers long (lecturer number, manager number, coordinator number, HR number)
                return (false, errors);
            }

            return (true, errors);
        }
        //***************************************************************************************//
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
        //***************************************************************************************//
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, bool isManager = false, bool isCoordinator = false, bool isHR = false)
        //using asp.net identity, this ensures that whatever password or username a user enters when they sign in/login in a specific login form, they are assigned appropriate roles. This restricts access and enhances security (corrections by Claude AI)
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
                HttpContext.Session.SetString("UserType", "Manager"); //creating the manager role and saving the details
                return await HandleManagerLoginPost(username, password);
            }
            else if (isCoordinator)
            {
                HttpContext.Session.SetString("UserType", "Coordinator"); //creating the coordinator role and saving the details
                return await HandleCoordinatorLoginPost(username, password);
            }
            else if (isHR)
            {
                HttpContext.Session.SetString("UserType", "HR"); //creating the HR role and saving the details
                return await HandleHRLoginPost(username, password);
            }
            else
            {
                HttpContext.Session.SetString("UserType", "Lecturer"); //creating the lecturer role and saving the details
                return await HandleLecturerLoginPost(username, password);
            }
        }
        //***************************************************************************************//
        private IActionResult ReturnToAppropriateLoginView(bool isManager, bool isCoordinator, bool isHR) //returns the appropriate role to the relevant login view (corrections by Claude AI to fix errors)
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

        private async Task<IActionResult> HandleManagerLoginPost(string username, string password) //this method handles the login for the manager (corrections by Claude AI to fix errors that kept saying the details were incorrect)
        {
            var academicManager = await _context.AcademicManagers
                .FirstOrDefaultAsync(am => am.ManagerNum == username);

            if (academicManager == null)
            {
                //if an academic manager does not exist then a new one is created
                academicManager = new AcademicManager
                {
                    ManagerNum = username, //the manager's num is their username
                    Password = password //the manager's password
                };
                _context.AcademicManagers.Add(academicManager);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Academic Manager account created successfully."; //message displays if a manager's login details meets all requirements 
            }
            else if (academicManager.Password != password)
            {
                TempData["ErrorMessage"] = "Invalid password."; //message displays if the password was incorrect
                return View("~/Views/Home/AcademicManagerLogin.cshtml"); //returns the user back to the login page for the manager
            }

            return RedirectToAction("VerifyClaims", "Home"); //once login is successful, the manager is directed to the verify claims page
        }
        //***************************************************************************************//
        private async Task<IActionResult> HandleCoordinatorLoginPost(string username, string password) //this method handles the login for the coordinator (corrections by Claude AI to fix errors that kept saying the details were incorrect)
        {
            var coordinator = await _context.ProgrammeCoordinators
                .FirstOrDefaultAsync(pc => pc.CoordinatorNum == username);

            if (coordinator == null)
            {
                //if a coordinator does not exist then a new one is created
                coordinator = new ProgrammeCoordinator
                {
                    CoordinatorNum = username, //the coordinator's num is their username
                    Password = password //the coordinator's password
                };
                _context.ProgrammeCoordinators.Add(coordinator);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Programme Coordinator account created successfully."; //message displays if a coordinator's login details meets all requirements 
            }
            else if (coordinator.Password != password)
            {
                TempData["ErrorMessage"] = "Invalid password."; //message displays if the password was incorrect
                return View("~/Views/Home/ProgrammeCoordinatorLogin.cshtml"); //returns the user back to the login page for the coordinator
            }

            return RedirectToAction("VerifyClaims", "Home"); //once login is successful, the coordinator is directed to the verify claims page
        
        }
        //***************************************************************************************//
        private async Task<IActionResult> HandleLecturerLoginPost(string username, string password) //this method handles the login for the lecturer (sytax errors fixed by ChatGpt)
        
            {
            var lecturer = await _context.Lecturers
                .FirstOrDefaultAsync(l => l.LecturerNum == username);

            if (lecturer == null)
            {
                //if a lecturer does not exist then a new one gets created
                lecturer = new Lecturer
                {
                    LecturerNum = username, //a lecturer's number is their username
                    Password = password //this is a lecturer's password
                };
                _context.Lecturers.Add(lecturer);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Lecturer account created successfully."; //message displays if a lecturer's login details meets all requirements 
            }
            else if (lecturer.Password != password)
            {
                TempData["ErrorMessage"] = "Invalid password."; //message displays if the password was incorrect
                return View("~/Views/Home/LecturerLogin.cshtml"); //redirects the user back to the login page for the lecturer login
            }

            return RedirectToAction("Privacy", "Home"); //once login is successful, the lecturer is directed to the page where they can submit a claim
        }
        //***************************************************************************************//
        private async Task<IActionResult> HandleHRLoginPost(string username, string password) //this method handles the login of HR (corrections done by Claude because the login page kept throwing exceptions)
        {
            var hrUser = await _context.HumanResource
                .FirstOrDefaultAsync(hr => hr.HumanResourcesNum == username);

            if (hrUser == null)
            {
                //if an HR does not exist then a new one is created
                hrUser = new HRDashboard
                {
                    HumanResourcesNum = username, //the Human Resource num is the username for the HR   
                    Password = password //this is the HR's password
                };
                _context.HumanResource.Add(hrUser);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Human Resources account created successfully."; //message displays if the HR login details meets all requirements 
            }
            else if (hrUser.Password != password)
            {
                TempData["ErrorMessage"] = "Invalid password."; //message displays if the password was incorrect
                return View("~/Views/Home/HumanResourcesLogin.cshtml"); //redirects the user back to the human resources login page to try again
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

            return RedirectToAction("HRDashboard", "Home"); //once login is successful, then the HR is redirected to the HRDashboard page
        }
    }
}
//*************************************End of file**************************************************//