using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectShihab.Entities
{
    [Table("User")]
    public class User
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [StringLength(250)]
        [Display(Name = "Name")]
        public string UserName { get; set; } = null!;
        [Required]
        [EmailAddress]
        [StringLength(250)]
        public string Email { get; set; } = null!;
        [Required]
        [StringLength(500)]
        public string PasswordHash { get; set; } = null!;
        [Required]
        [StringLength(50)]
        public string Gender { get; set; } = null!;
        [Required]
        [StringLength(50)]
        [Display(Name = "Contact Number")]
        public string ContactNumber { get; set; } = null!;
        [Required]
        [Display(Name = "Date of Birth")]
        public DateTime Dob { get; set; }
        [Required]
        [StringLength(250)]
        public string Address { get; set; } = null!;
        [Required]
        [StringLength(50)]
        public string Role { get; set; } = null!;
        public bool Status { get; set; } = true;
        public DateTime CreatedAt { get; set; }
    }
}
