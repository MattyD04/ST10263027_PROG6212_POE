namespace ST10263027_PROG6212_POE.Models
{
    //claimviewmodel to combine the details of lecturers and claims together
    public class ClaimViewModel
    {
        public int ClaimID { get; set; }
        public string ClaimNum { get; set; }
        public string LecturerNum { get; set; }
        public DateTime SubmissionDate { get; set; }
        public double HoursWorked { get; set; }
        public double HourlyRate { get; set; }
        public double TotalAmount { get; set; }
        public string Comments { get; set; }
        public string Filename { get; set; }
    }
}
