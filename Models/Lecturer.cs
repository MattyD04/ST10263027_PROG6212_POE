using System.ComponentModel.DataAnnotations;

namespace ST10263027_PROG6212_POE.Models
{
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
    }
}
