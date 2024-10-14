using System.ComponentModel.DataAnnotations;

namespace ST10263027_PROG6212_POE.Models
{
    //table for the lecturers
    public class Lecturer
    {
        [Key]
        public int LecturerId { get; set; } //Primary key for the lecturer table
        [Required]
        public string LecturerNum { get; set; } //attribute to store the lecturer's number
        [Required]
        [Range(0, double.MaxValue)]
        public double HourlyRate { get; set; } //attribute to store a lecturer's hourly rate
        [Range(0, double.MaxValue)]
        public double HoursWorked { get; set; } //attribute to store how many hours worked for a lecturer
        [Required]
        [StringLength(100)] 
        public string Password { get; set; } //attribute to store a lecturer's password
    }
}
//-----------------------------------End of file--------------------------------------------//

