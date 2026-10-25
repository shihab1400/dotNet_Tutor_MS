using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectShihab.Entities
{
    [Table("Payment")]
    public class Payment
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int UserId { get; set; }
        [Required]
        [StringLength(50)]
        public string Kind { get; set; } = null!;
        public int TargetId { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public int? CouponId { get; set; }
        [Required]
        [StringLength(50)]
        public string TransactionRef { get; set; } = null!;
        public DateTime PaidAt { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [ForeignKey("CouponId")]
        public virtual Coupon? Coupon { get; set; }
    }
}
