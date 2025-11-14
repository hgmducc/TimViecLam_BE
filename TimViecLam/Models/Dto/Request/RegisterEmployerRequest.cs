using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimViecLam.Models.Dto.Request
{
    public class RegisterEmployerRequest
    {
        // === USERS FIELDS (Bắt buộc) ===
        [Required(ErrorMessage = "Họ và tên là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Tên đầy đủ không được vượt quá 100 ký tự.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "Mật khẩu phải chứa ít nhất 8 và tối đa 50 ký tự.")] // Đã sửa giới hạn tối đa
        public string Password { get; set; } = string.Empty;

        // === USERS FIELDS (Tùy chọn/Business Rule) ===
        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")] // Business Rule: Bắt buộc ở tầng API
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        [StringLength(20)] // Sửa thành 20 theo SQL
        public string PhoneNumber { get; set; } = string.Empty;

        public DateOnly? DateOfBirth { get; set; }
        public string? Gender { get; set; }

        // Địa chỉ cá nhân của người đại diện (tùy chọn)
        [StringLength(255)]
        public string? UserAddress { get; set; }

        // === EMPLOYERS FIELDS (CompanyName Bắt buộc, còn lại Tùy chọn) ===
        [Required(ErrorMessage = "Tên công ty là bắt buộc.")] // NOT NULL trong SQL
        [StringLength(150, ErrorMessage = "Tên công ty không được vượt quá 150 ký tự.")]
        public string CompanyName { get; set; } = string.Empty;

        // CompanyAddress là NULL trong SQL, nhưng bạn đang bắt buộc ở API
        [Required(ErrorMessage = "Địa chỉ công ty là bắt buộc.")]
        [StringLength(255, ErrorMessage = "Địa chỉ công ty không được vượt quá 255 ký tự.")]
        public string CompanyAddress { get; set; } = string.Empty; // Đổi tên để rõ ràng hơn

        [Url(ErrorMessage = "Định dạng URL trang web không hợp lệ.")]
        [StringLength(255)]
        public string? CompanyWebsite { get; set; }

        public string? CompanyDescription { get; set; } // NVARCHAR(MAX)

        [StringLength(50)]
        public string? TaxCode { get; set; }

        [StringLength(100)]
        public string? BusinessLicenseNumber { get; set; }

        [StringLength(255)]
        public string? BusinessLicenseFile { get; set; }

        // Thông tin liên hệ phụ
        [StringLength(100)]
        public string? ContactPerson { get; set; }

        [EmailAddress]
        [StringLength(150)]
        public string? ContactEmail { get; set; }

        [Phone]
        [StringLength(20)]
        public string? ContactPhone { get; set; }
    }
}