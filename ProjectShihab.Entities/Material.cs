using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectShihab.Entities
{
    [Table("Material")]
    public class Material
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [StringLength(250)]
        public string Title { get; set; } = null!;
        [Required]
        [StringLength(50)]
        public string FileType { get; set; } = null!;
        [Required]
        [StringLength(500)]
        public string FilePath { get; set; } = null!;
        [StringLength(250)]
        public string? OriginalName { get; set; }
        public int? OfferedCourseId { get; set; }
        public int? TrainingOfferingId { get; set; }
        public int UploadedBy { get; set; }
        public DateTime UploadedAt { get; set; }

        [ForeignKey("OfferedCourseId")]
        public virtual OfferedCourse? OfferedCourse { get; set; }

        [ForeignKey("TrainingOfferingId")]
        public virtual TrainingOffering? TrainingOffering { get; set; }

        [ForeignKey("UploadedBy")]
        public virtual User? Uploader { get; set; }
    }
}
