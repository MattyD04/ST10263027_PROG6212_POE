using System.ComponentModel.DataAnnotations;

namespace ST10263027_PROG6212_POE.Models
{
    public class ProgrammeCoordinator
    {
        [Key]
        public int CoordinatorID { get; set; }
        [Required]
        public string CoordinatorNum { get; set; }
        
    }
}
//-----------------------------------End of file--------------------------------------------//

