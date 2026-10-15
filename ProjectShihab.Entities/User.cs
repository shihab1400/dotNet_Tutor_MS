using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectShihab.Entities
{
    [Table("User")]
    public class User
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [StringLength(250)]
        public string UserName { get; set; } = null!;
        [Required]
        [StringLength(250)]
        public string Email { get; set; } = null!;
        [Required]
        [StringLength(50)]
        public string Gender { get; set; } = null!;
        [Required]
        [StringLength(50)] 
        public string ContactNumber { get; set; } = null!;
        [Required]
        [Display(Name = "Date of Birth")]
        public DateTime Dob { get; set; }
        [Required]
        [StringLength(50)] 
        public string Role { get; set; } = null!;
        [Required]
        [StringLength(250)] 
        public string Address { get; set; } = null!;
        [Required]
        [StringLength(50)] 
        public string Password { get; set; } = null!;
        [Required]
        public bool Status { get; set; }
    }
}
