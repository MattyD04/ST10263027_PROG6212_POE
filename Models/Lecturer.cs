using System.ComponentModel.DataAnnotations;

namespace ST10263027_PROG6212_POE.Models
{
    // Table for the lecturers
    public class Lecturer
    {
        [Key]
        public int LecturerId { get; set; } // Primary key for the lecturer table

        [Required]
        public string LecturerNum { get; set; } // Attribute to store the lecturer's number

        [Required]
        [Range(0, double.MaxValue)]
        public double HourlyRate { get; set; } // Attribute to store a lecturer's hourly rate

        [Range(0, double.MaxValue)]
        public double HoursWorked { get; set; } // Attribute to store how many hours worked for a lecturer

        [StringLength(100)]
        public string Password { get; set; } // Attribute to store a lecturer's password, allowing null values
    }
}
//**************************************************end of file***********************************************//