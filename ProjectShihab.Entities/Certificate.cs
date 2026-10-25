using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectShihab.Entities
{
    [Table("Certificate")]
    public class Certificate
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int TrainingEnrollmentId { get; set; }
        public int TutorId { get; set; }
        public int TrainerId { get; set; }
        public int CourseId { get; set; }
        [Required]
        [StringLength(50)]
        public string CertificateNo { get; set; } = null!;
        public DateTime IssuedAt { get; set; }

        [ForeignKey("TrainingEnrollmentId")]
        public virtual TrainingEnrollment? TrainingEnrollment { get; set; }

        [ForeignKey("TutorId")]
        public virtual User? Tutor { get; set; }

        [ForeignKey("TrainerId")]
        public virtual User? Trainer { get; set; }

        [ForeignKey("CourseId")]
        public virtual Course? Course { get; set; }
    }
}
