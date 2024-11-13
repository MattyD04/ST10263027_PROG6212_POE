using ST10263027_PROG6212_POE.Models;

namespace ST10263027_PROG6212_POE.Models
{
    public class HRDashboardViewModel
    {
        public List<ClaimViewModel> ApprovedClaims { get; set; } //list of approved claims to be displayed in the HR dashboard
        public List<Lecturer> Lecturers { get; set; } //list of lecturers to be displayed in the HR dashboard
    }
}