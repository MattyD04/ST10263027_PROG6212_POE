using System.ComponentModel.DataAnnotations;

namespace ST10263027_PROG6212_POE.Models
{
    //table for the lecturers
    public class Lecturer
    {
        [Key]
        public int LecturerId { get; set; }
        [Required]
        public string LecturerNum { get; set; }
        [Required]
        [Range(0, double.MaxValue)]
        public double HourlyRate { get; set; }
        [Range(0, double.MaxValue)]
        public double HoursWorked { get; set; }
        [Required]
        [StringLength(100)] 
        public string Password { get; set; }
    }
}
//-----------------------------------End of file--------------------------------------------//

