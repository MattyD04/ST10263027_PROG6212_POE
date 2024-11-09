using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ST10263027_PROG6212_POE.Models;  // Add this to reference your models

namespace ST10263027_PROG6212_POE.Views.Home
{
    public class HRDashboardModel : PageModel
    {
        public HRDashboard HRUser { get; set; }  // Property to hold the HR user data

        public void OnGet()
        {
            HRUser = new HRDashboard();  // Initialize the property
        }
    }
}