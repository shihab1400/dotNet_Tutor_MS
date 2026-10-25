using System.ComponentModel.DataAnnotations;

namespace ProjectShihab.Models
{
    public class LoginModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
    }

    public class RegisterModel
    {
        [Required]
        [StringLength(250)]
        [Display(Name = "Name")]
        public string UserName { get; set; } = null!;
        [Required]
        [EmailAddress]
        [StringLength(250)]
        public string Email { get; set; } = null!;
        [Required]
        public string Gender { get; set; } = "Male";
        [Required]
        [StringLength(50)]
        [Display(Name = "Contact Number")]
        public string ContactNumber { get; set; } = null!;
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime Dob { get; set; } = DateTime.Today.AddYears(-18);
        [Required]
        [StringLength(250)]
        public string Address { get; set; } = null!;
        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
        [Required]
        [Compare("Password")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = null!;
        // Student or Tutor for public registration; ignored when an admin creates a trainer.
        public string Role { get; set; } = "Student";
    }
}
