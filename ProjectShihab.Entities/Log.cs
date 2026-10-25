using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectShihab.Entities
{
    [Table("Log")]
    public class Log
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? UserId { get; set; }
        [StringLength(250)]
        public string UserName { get; set; } = "-";
        [StringLength(50)]
        public string Role { get; set; } = "Guest";
        [Required]
        [StringLength(100)]
        public string Action { get; set; } = null!;
        [StringLength(100)]
        public string? Entity { get; set; }
        public int? EntityId { get; set; }
        [StringLength(1000)]
        public string? Details { get; set; }
        [StringLength(50)]
        public string? IpAddress { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
