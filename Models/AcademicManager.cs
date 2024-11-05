using System.ComponentModel.DataAnnotations;

namespace ST10263027_PROG6212_POE.Models
{
    //table for the Academic Managers
    public class AcademicManager
    {
        [Key]
        public int ManagerID { get; set; } // Primary key for the AcademicManager table
        [Required]
        public string ManagerNum { get; set; } // Attribute to store the number for an Academic Manager 
        [Required]
        [StringLength(100)] 
        public string Password { get; set; } // Password for the Academic Manager
    }

}
//**************************************************end of file***********************************************//

