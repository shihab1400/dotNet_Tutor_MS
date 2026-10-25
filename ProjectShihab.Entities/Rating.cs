using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectShihab.Entities
{
    [Table("Rating")]
    public class Rating
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int CourseEnrollmentId { get; set; }
        public int StudentId { get; set; }
        public int TutorId { get; set; }
        public int OfferedCourseId { get; set; }
        [Column("Rating")]
        [Range(1, 5)]
        [Display(Name = "Rating")]
        public int RatingValue { get; set; }
        [StringLength(250)]
        public string? Feedback { get; set; }
        public DateTime CreatedAt { get; set; }

        [ForeignKey("CourseEnrollmentId")]
        public virtual CourseEnrollment? CourseEnrollment { get; set; }

        [ForeignKey("StudentId")]
        public virtual User? Student { get; set; }

        [ForeignKey("TutorId")]
        public virtual User? Tutor { get; set; }

        [ForeignKey("OfferedCourseId")]
        public virtual OfferedCourse? OfferedCourse { get; set; }
    }
}
