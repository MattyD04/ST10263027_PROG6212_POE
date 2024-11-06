using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ST10263027_PROG6212_POE.Data;
using ST10263027_PROG6212_POE.Models;
using System.Threading.Tasks;

namespace ST10263027_PROG6212_POE.Controllers
{
    public class LoginController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public LoginController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
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
                return RedirectToAction(isManager ? "HandleManagerLogin" : isCoordinator ? "HandleCoordinatorLogin" : "Login");
            }

            var result = await _signInManager.PasswordSignInAsync(username, password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                // Redirect based on role if needed
                if (isManager)
                {
                    return RedirectToAction("VerifyClaims", "Home");
                }
                else if (isCoordinator)
                {
                    return RedirectToAction("VerifyClaims", "Home");
                }
                else
                {
                    return RedirectToAction("Privacy", "Home");
                }
            }

            TempData["ErrorMessage"] = "Invalid login attempt.";
            return RedirectToAction(isManager ? "HandleManagerLogin" : isCoordinator ? "HandleCoordinatorLogin" : "Login");
        }

        //****************************************************************************************************************************//
        private async Task<IActionResult> HandleManagerLogin(string username, string password) //method to handle the login of an Academic Manager
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                user = new IdentityUser { UserName = username };
                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "AcademicManager");
                    TempData["SuccessMessage"] = "Academic Manager account created successfully. Please log in.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to create account.";
                    return View("~/Views/Home/AcademicManagerLogin.cshtml");
                }
            }
            else if (!await _userManager.CheckPasswordAsync(user, password))
            {
                TempData["ErrorMessage"] = "Invalid password."; //displays if password does not match
                return View("~/Views/Home/AcademicManagerLogin.cshtml");
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("VerifyClaims", "Home"); //redirects after a successful login
        }

        //****************************************************************************************************************************//
        private async Task<IActionResult> HandleCoordinatorLogin(string username, string password) //method to handle the login of a Programme Coordinator
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                user = new IdentityUser { UserName = username };
                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "ProgrammeCoordinator");
                    TempData["SuccessMessage"] = "Programme Coordinator account created successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to create account.";
                    return View("~/Views/Home/ProgrammeCoordinatorLogin.cshtml");
                }
            }
            else if (!await _userManager.CheckPasswordAsync(user, password))
            {
                TempData["ErrorMessage"] = "Invalid password."; //displays if password does not match
                return View("~/Views/Home/ProgrammeCoordinatorLogin.cshtml");
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("VerifyClaims", "Home"); //redirects to Verify Claims after successful login
        }

        //****************************************************************************************************************************//
        private async Task<IActionResult> HandleLecturerLogin(string username, string password) //method to handle the login for the lecturer
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                user = new IdentityUser { UserName = username };
                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Lecturer");
                    TempData["SuccessMessage"] = "Lecturer account created successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to create account.";
                    return View("~/Views/Home/LecturerLogin.cshtml");
                }
            }
            else if (!await _userManager.CheckPasswordAsync(user, password))
            {
                TempData["ErrorMessage"] = "Invalid password."; //displays if password does not match
                return View("~/Views/Home/LecturerLogin.cshtml");
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Privacy", "Home"); //redirects to claims submission upon successful login
        }

        //****************************************************************************************************************************//
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Index()
        {
            return View("~/Views/Home/Index.cshtml");
        }
    }
}
//****************************************end of file***********************************************//
