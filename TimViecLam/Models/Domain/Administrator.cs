using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimViecLam.Models.Domain
{
    [Table("Administrators")]
    public class Administrator
    {
        // Khóa chính riêng (AdminID INT IDENTITY(1,1) PRIMARY KEY)
        [Key]
        public int AdminID { get; set; }

        // Khóa ngoại (UserID INT NOT NULL UNIQUE)
        [Required]
        public int UserID { get; set; }

        // NVARCHAR(20) DEFAULT N'Moderator'
        [StringLength(20)]
        public string RoleLevel { get; set; } = "Moderator";

        // NVARCHAR(MAX) NULL
        public string? Note { get; set; }

        // DATETIME2 NULL
        public DateTime? LastLogin { get; set; }

        // Thuộc tính điều hướng (FK_Administrators_Users)
        [ForeignKey("UserID")]
        public User User { get; set; } = null!; // UserID NOT NULL nên User phải tồn tại
    }
}
