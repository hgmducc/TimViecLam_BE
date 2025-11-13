using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimViecLam.Models.Domain
{
    [Table("Employers")]
    public class Employer
    {
        // Khóa chính riêng
        [Key]
        public int EmployerID { get; set; }

        // Khóa ngoại NOT NULL UNIQUE
        [Required]
        public int UserID { get; set; }

        // NVARCHAR(150) NOT NULL
        [Required]
        [StringLength(150)]
        public string CompanyName { get; set; } = string.Empty;

        [StringLength(255)]
        public string? CompanyAddress { get; set; }

        [StringLength(255)]
        public string? CompanyWebsite { get; set; }

        public string? CompanyDescription { get; set; } // NVARCHAR(MAX) NULL

        [StringLength(50)]
        public string? TaxCode { get; set; } // UNIQUE NULL

        [StringLength(100)]
        public string? BusinessLicenseNumber { get; set; }

        [StringLength(255)]
        public string? BusinessLicenseFile { get; set; }

        // NVARCHAR(30) DEFAULT N'Chưa xác minh'
        [StringLength(30)]
        public string VerificationStatus { get; set; } = "Chưa xác minh";

        [StringLength(100)]
        public string? ContactPerson { get; set; }

        [StringLength(150)]
        public string? ContactEmail { get; set; }

        [StringLength(20)]
        public string? ContactPhone { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Thuộc tính điều hướng
        [ForeignKey("UserID")]
        public User User { get; set; } = null!;
    }
}
