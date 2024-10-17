namespace ST10263027_PROG6212_POE.Models
{
    //claimviewmodel to combine the details of lecturers and claims together
    public class ClaimViewModel
    {
        public int ClaimID { get; set; } //ClaimID for the claim
        public string ClaimNum { get; set; } //Claim number of the claim  from the claim table
        public string LecturerNum { get; set; } //lecturer's number from the lecturer table
        public DateTime SubmissionDate { get; set; } //submission date of the claim from the claim table
        public double HoursWorked { get; set; } //hours worked from the lecturer table
        public double HourlyRate { get; set; } //hourly rate from the lecturer table
        public double TotalAmount { get; set; } //total amount of the multiplication of a lecturer's hours worked and their hourly rate
        public string Comments { get; set; } //comments submitted by the lecturer from the claim table
        public string Filename { get; set; } //name of the file in the claim table
    }
}
