using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectShihab.Entities
{
    [Table("Rating")]
    public class Rating
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int UserId { get; set; }
        [Required]
        [StringLength(250)]
        public string CourseType { get; set; } = null!;
        public int CourseId { get; set; }
        [Column("Rating")]
        [Required]
        public int RatingValue { get; set; }
        [Required]
        [StringLength(250)]
        public string Feedback { get; set; } = null!;


        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
