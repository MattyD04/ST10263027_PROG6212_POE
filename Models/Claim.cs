using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ST10263027_PROG6212_POE.Models
{
    public class Claim
    {
        [Key]
        public int ClaimID { get; set; }
        [Required]
        public string ClaimNum { get; set; }
        [Required]
        public int LecturerID { get; set; }
        public int? CoordinatorID { get; set; }
        public int? ManagerID { get; set; }
        [Required]
        public DateTime SubmissionDate { get; set; }
        [Required]
        public string ClaimStatus { get; set; }
        public string Filename { get; set; }
        public string ContentType { get; set; }
        public byte[] FileData { get; set; }
        public string Comments { get; set; }

        [ForeignKey("LecturerID")]
        public virtual Lecturer Lecturer { get; set; }
        [ForeignKey("CoordinatorID")]
        public virtual ProgrammeCoordinator Coordinator { get; set; }
        [ForeignKey("ManagerID")]
        public virtual AcademicManager Manager { get; set; }
    }
}
//-----------------------------------End of file--------------------------------------------//
