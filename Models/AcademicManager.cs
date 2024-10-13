using System.ComponentModel.DataAnnotations;

namespace ST10263027_PROG6212_POE.Models
{
    public class AcademicManager
    {
        [Key]
        public int ManagerID { get; set; }
        [Required]
        public string ManagerNum { get; set; }
        
    }
}
