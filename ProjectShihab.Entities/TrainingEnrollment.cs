using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectShihab.Entities
{
    [Table("TrainingEnrollment")]
    public class TrainingEnrollment
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int TutorId { get; set; }
        public int TrainingOfferingId { get; set; }
        public int PaymentId { get; set; }
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = EnrollmentStatus.Enrolled;
        public DateTime EnrolledAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        [ForeignKey("TutorId")]
        public virtual User? Tutor { get; set; }

        [ForeignKey("TrainingOfferingId")]
        public virtual TrainingOffering? TrainingOffering { get; set; }

        [ForeignKey("PaymentId")]
        public virtual Payment? Payment { get; set; }
    }
}
