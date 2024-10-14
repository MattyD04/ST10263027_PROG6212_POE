using System.ComponentModel.DataAnnotations;

namespace ST10263027_PROG6212_POE.Models
{
    //table for the Programme Coordinators
    public class ProgrammeCoordinator
    {
        [Key]
        public int CoordinatorID { get; set; } //Primary key for the coordinators
        [Required]
        public string CoordinatorNum { get; set; } //attribute to store the number for a coordinator
        
    }
}
//-----------------------------------End of file--------------------------------------------//

