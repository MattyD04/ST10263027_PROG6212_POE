using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ST10263027_PROG6212_POE.Models
{
    //table for the Human Resources
    public class HRDashboard
    {
        [Key]
        public int HumanResourcesID { get; set; } // Primary key for the HumanResources table

        [Required]
        public string HumanResourcesNum { get; set; } // Attribute to store the number for Human Resources

        [Required]
        [StringLength(100)]
        public string Password { get; set; } // Password for Human Resources
    }
}
//**************************************************end of file***********************************************//