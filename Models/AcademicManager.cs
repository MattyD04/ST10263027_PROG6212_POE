using System.ComponentModel.DataAnnotations;

namespace ST10263027_PROG6212_POE.Models
{
    //table for the Academic Managers
    public class AcademicManager
    {
        [Key]
        public int ManagerID { get; set; } //Primary key for the AcademicManger table
        [Required]
        public string ManagerNum { get; set; } //attribute to store the number for an Academic Manager 
        
    }
}
//-----------------------------------End of file--------------------------------------------//

