using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimViecLam.Models.Domain
{
    [Table("Candidates")]
    public class Candidate
    {
        // Khóa chính riêng
        [Key]
        public int CandidateID { get; set; }

        // Khóa ngoại NOT NULL UNIQUE
        [Required]
        public int UserID { get; set; }

        [StringLength(100)]
        public string? HighestEducation { get; set; } // NVARCHAR(100) NULL

        [StringLength(150)]
        public string? Major { get; set; } // NVARCHAR(150) NULL

        public int ExperienceYears { get; set; } = 0; // INT DEFAULT 0

        public string? Skills { get; set; } // NVARCHAR(MAX)

        // DECIMAL(12,2) NULL - [Column(TypeName=...)] đảm bảo kiểu dữ liệu SQL chính xác
        [Column(TypeName = "decimal(12, 2)")]
        public decimal? ExpectedSalary { get; set; }

        [StringLength(30)]
        public string? JobType { get; set; }

        [StringLength(255)]
        public string? DesiredLocation { get; set; }

        [StringLength(255)]
        public string? CVUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Thuộc tính điều hướng
        [ForeignKey("UserID")]
        public User User { get; set; } = null!;
    }
}
