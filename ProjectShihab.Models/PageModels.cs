using System.ComponentModel.DataAnnotations;

namespace ProjectShihab.Models
{
    public class CheckoutModel
    {
        public string Kind { get; set; } = null!;
        public int TargetId { get; set; }
        public string Title { get; set; } = null!;
        public string Subtitle { get; set; } = string.Empty;
        public decimal Original { get; set; }
        public decimal Discount { get; set; }
        public decimal Final { get; set; }
        [Display(Name = "Coupon Code")]
        public string? CouponCode { get; set; }
        public string? CouponMessage { get; set; }
    }

    public class RatingModel
    {
        public int CourseEnrollmentId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        [Range(1, 5, ErrorMessage = "Choose 1 to 5 stars.")]
        public int Rating { get; set; }
        [StringLength(250)]
        public string? Feedback { get; set; }
    }

    public class MaterialUploadModel
    {
        // "Course" (tutor, OfferedCourse) or "Training" (trainer, TrainingOffering)
        public string Kind { get; set; } = null!;
        public int TargetId { get; set; }
        [Required]
        [StringLength(250)]
        public string Title { get; set; } = null!;
    }

    public class LogFilterModel
    {
        public string? Search { get; set; }
        public string? Role { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public int Page { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
    }
}
