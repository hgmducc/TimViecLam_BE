using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimViecLam.Models.Domain
{
    [Table("Users")] // Ánh xạ đến tên bảng số nhiều trong SQL
    public class User
    {
        // Khóa chính (IDENTITY(1,1) PRIMARY KEY)
        [Key]
        public int UserID { get; set; }

        // NVARCHAR(100) NOT NULL
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        // NVARCHAR(150) NOT NULL UNIQUE
        [Required]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        // NVARCHAR(20) UNIQUE NULL
        [StringLength(20)]
        public string? Phone { get; set; }

        // NVARCHAR(255) NOT NULL
        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        // DATE NULL (Sử dụng DateOnly cho kiểu DATE)
        public DateOnly? DateOfBirth { get; set; }

        // NVARCHAR(10) NULL
        [StringLength(10)]
        public string? Gender { get; set; }

        // NVARCHAR(255) NULL
        [StringLength(255)]
        public string? Address { get; set; }

        // NVARCHAR(20) NOT NULL (ràng buộc CHECK(Role IN (...)) được xử lý ở lớp logic/database)
        [Required]
        [StringLength(20)]
        public string Role { get; set; } = string.Empty;

        // DATETIME2 DEFAULT SYSDATETIME()
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // DATETIME2 DEFAULT SYSDATETIME()
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // NVARCHAR(20) DEFAULT N'Active'
        [StringLength(20)]
        public string Status { get; set; } = "Active";

        // --- Thuộc tính điều hướng (Navigation Properties) cho mối quan hệ 1-1 ---
        public Administrator? Administrator { get; set; }
        public Employer? Employer { get; set; }
        public Candidate? Candidate { get; set; }
    }
}
