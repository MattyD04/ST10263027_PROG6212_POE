using System.ComponentModel.DataAnnotations;

namespace ST10263027_PROG6212_POE.Models
{
    //table for the Programme Coordinators
    public class ProgrammeCoordinator
    {
        [Key]
        public int CoordinatorID { get; set; } // Primary key for the coordinators

        [Required]
        public string CoordinatorNum { get; set; } // Attribute to store the number for a coordinator

        [Required]
        [StringLength(100)] // Set max length for password field, adjust as needed
        public string Password { get; set; } // Password for the coordinator
    }

}
//**************************************************end of file***********************************************//

