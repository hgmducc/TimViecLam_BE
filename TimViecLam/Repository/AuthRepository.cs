using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TimViecLam.Data;
using TimViecLam.Models.Domain;
using TimViecLam.Models.Dto.Request;
using TimViecLam.Models.Dto.Response;
using TimViecLam.Repository.IRepository;


namespace TimViecLam.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext dbContext;

        public AuthRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public Task<AuthResult> LoginAsync(LoginRequest requestDto)
        {
            throw new NotImplementedException();
        }

        public async Task<AuthResult> RegisterCandidateAsync(RegisterCandidateRequest requestDto)
        {
            //kiểm tra email/sdt đã tồn tại chưa
            bool emailExists = dbContext.Users.Any(u => u.Email == requestDto.Email);
            if (emailExists == true)
            {
                // Trả về lỗi 409 
                return new AuthResult
                {
                    IsSuccess = false,
                    Status = 409, // Giả sử bạn thêm trường Status vào AuthResult
                    ErrorCode = "EMAIL_EXISTS",
                    Message = "Địa chỉ email này đã được đăng ký."
                };
            }
            bool phoneExists = dbContext.Users.Any(u => u.Phone == requestDto.PhoneNumber);
            if (phoneExists == true)
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    Status = 409,
                    ErrorCode = "PHONE_EXISTS",
                    Message = "Số điện thoại này đã được đăng ký."
                };
            }

            // hash mật khẩu
            string hashed = BCrypt.Net.BCrypt.HashPassword(requestDto.Password, workFactor: 12);

            // Tạo user mới
            var user = new User
            {
                FullName = requestDto.FullName,
                Email = requestDto.Email,
                Phone = requestDto.PhoneNumber,
                PasswordHash = hashed,
                DateOfBirth = requestDto.DateOfBirth
            };

            return new AuthResult { IsSuccess = true, Message = "Đăng ký thành công." };
        }
    }
}
