using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ST10263027_PROG6212_POE.Models
{//table for the claims
    public class Claim
    {
        [Key]
        public int ClaimID { get; set; } //Primary key for the claims table
        [Required]
        public string ClaimNum { get; set; } // attribute to store the number of a claim
        [Required]
        public int LecturerID { get; set; } // attribute to create a reference to the lecturer table
        public int? CoordinatorID { get; set; } // attribute to create a reference to the Programme Coordinator table
        public int? ManagerID { get; set; } // attribute to create a reference to the Academic Manager table
        [Required]
        public DateTime SubmissionDate { get; set; } // attribute to store the submission date
        [Required]
        public string ClaimStatus { get; set; } // attribute to store the status of a claim (which will be accepted, rejected or pending)
        public string Filename { get; set; } // attribute to store the name of a file
        public string ContentType { get; set; } // attribute to store the content of a file
        public byte[] FileData { get; set; } // attribute to store the data of a file
        public string Comments { get; set; } // attribute to store the comments submitted by a lecturer

        [ForeignKey("LecturerID")]
        public virtual Lecturer Lecturer { get; set; } // creates a foreign key reference to the lecturer table
        [ForeignKey("CoordinatorID")]
        public virtual ProgrammeCoordinator Coordinator { get; set; } // creates a foreign key reference to the Programme Cordinator table
        [ForeignKey("ManagerID")]
        public virtual AcademicManager Manager { get; set; } // creates a foreign key reference to the Academic Manager table
    }
}
//-----------------------------------End of file--------------------------------------------//
