using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectShihab.Entities
{
    [Table("Coupon")]
    public class Coupon
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Code { get; set; } = null!;
        [Required]
        [StringLength(50)]
        public string Type { get; set; } = CouponTypes.Percentage;
        [Range(0.01, 10000000)]
        public decimal Value { get; set; }
        [Display(Name = "Expiry Date")]
        public DateTime ExpiryDate { get; set; }
        [Range(1, int.MaxValue)]
        [Display(Name = "Max Uses")]
        public int MaxUses { get; set; } = 1;
        public int UsedCount { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
