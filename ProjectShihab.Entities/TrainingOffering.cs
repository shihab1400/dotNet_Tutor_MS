using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectShihab.Entities
{
    [Table("TrainingOffering")]
    public class TrainingOffering
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Display(Name = "Course")]
        public int CourseId { get; set; }
        [Display(Name = "Trainer")]
        public int TrainerId { get; set; }
        [Range(0, 10000000)]
        public decimal Price { get; set; }
        [Required]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }
        [Required]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }
        public bool Status { get; set; } = true;

        [ForeignKey("CourseId")]
        public virtual Course? Course { get; set; }

        [ForeignKey("TrainerId")]
        public virtual User? Trainer { get; set; }
    }
}
