using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectShihab.Entities
{
    [Table("Course")]
    public class Course
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Display(Name = "Group")]
        public int GroupId { get; set; }
        [Required]
        [StringLength(250)]
        public string Subject { get; set; } = null!;
        [Range(0, 10000000)]
        [Display(Name = "Base Price")]
        public decimal BasePrice { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        [ForeignKey("GroupId")]
        public virtual Group? Group { get; set; }

        [ForeignKey("CreatedBy")]
        public virtual User? User { get; set; }
    }
}
