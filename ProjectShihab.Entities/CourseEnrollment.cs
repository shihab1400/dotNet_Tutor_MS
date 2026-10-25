using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectShihab.Entities
{
    [Table("CourseEnrollment")]
    public class CourseEnrollment
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int OfferedCourseId { get; set; }
        public int PaymentId { get; set; }
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = EnrollmentStatus.Enrolled;
        public DateTime EnrolledAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        [ForeignKey("StudentId")]
        public virtual User? Student { get; set; }

        [ForeignKey("OfferedCourseId")]
        public virtual OfferedCourse? OfferedCourse { get; set; }

        [ForeignKey("PaymentId")]
        public virtual Payment? Payment { get; set; }
    }
}
