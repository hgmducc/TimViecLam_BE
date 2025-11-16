using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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
        private readonly IConfiguration configuration;
        private readonly string secretKey;

        public AuthRepository(ApplicationDbContext dbContext, IConfiguration configuration)
        {
            this.dbContext = dbContext;
            this.configuration = configuration;
            secretKey = configuration.GetValue<string>("ApiSetting:Secret");
        }

        // (Giả sử bạn đã cập nhật DTO thành LoginRequest { string Username, string Password })

        public async Task<AuthResult> LoginAsync(LoginRequest requestDto)
        {
            try
            {
                // 1️⃣ Tìm user theo username (có thể là email hoặc SĐT)
                // COMMENT: Sửa logic tìm kiếm.
                // Gộp Email và PhoneNumber thành một câu truy vấn duy nhất
                // dựa trên thuộc tính 'Username' mới của requestDto.
                var user = await dbContext.Users
                    .FirstOrDefaultAsync(u => u.Email == requestDto.Username || u.Phone == requestDto.Username);

                // COMMENT: Thay đổi cách xử lý lỗi "Không tìm thấy User".
                // Gộp chung lỗi "Không tìm thấy" và "Sai mật khẩu" thành một
                // thông báo duy nhất. Đây là một best-practice về bảo mật
                // để chống lại "User Enumeration Attack" (tấn công dò tìm user).
                if (user == null)
                    return new AuthResult
                    {
                        IsSuccess = false,
                        Status = 401, // COMMENT: Trả về 401 (Unauthorized) thay vì 404
                        ErrorCode = "INVALID_CREDENTIALS",
                        Token = "", // COMMENT: Thêm Token = "" cho thống nhất
                        Message = "Tài khoản hoặc mật khẩu không chính xác." // COMMENT: Thông báo chung
                    };

                // 2️⃣ Nếu user đăng ký bằng Google thì không có password (Logic này vẫn giữ nguyên)
                if (user.PasswordHash == null)
                    return new AuthResult
                    {
                        IsSuccess = false,
                        Status = 400,
                        ErrorCode = "GOOGLE_LOGIN_REQUIRED",
                        Message = "Tài khoản này được đăng ký bằng Google. Vui lòng đăng nhập bằng Google."
                    };

                // 3️⃣ Kiểm tra mật khẩu
                // COMMENT: requestDto.Password vẫn được sử dụng như cũ
                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(requestDto.Password, user.PasswordHash);

                if (!isPasswordValid)
                    return new AuthResult
                    {
                        IsSuccess = false,
                        Status = 401,
                        ErrorCode = "INVALID_CREDENTIALS",
                        Token = "", // Giữ nguyên
                        Message = "Tài khoản hoặc mật khẩu không chính xác." // COMMENT: Dùng thông báo chung
                    };

                // 4️⃣ Tạo JWT token (Logic này vẫn giữ nguyên)
                var key = Encoding.UTF8.GetBytes(secretKey); // (secretKey lấy từ configuration)
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserID.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("role", user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }),
                    Expires = DateTime.UtcNow.AddMinutes(double.Parse(configuration["ApiSetting:ExpiresInMinutes"])),
                    Issuer = configuration["ApiSetting:Issuer"],
                    Audience = configuration["ApiSetting:Audience"],
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature
                    )
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var jwtToken = tokenHandler.WriteToken(token);

                return new AuthResult
                {
                    IsSuccess = true,
                    Status = 200,
                    Message = "Đăng nhập thành công.",
                    Token = jwtToken
                };
            }
            catch (Exception ex)
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    Status = 500,
                    ErrorCode = "SERVER_ERROR",
                    Message = "Đã xảy ra lỗi máy chủ: " + ex.Message
                };
            }
        }
        public async Task<AuthResult> RegisterCandidateAsync(RegisterCandidateRequest requestDto)
        {
            try
            {
                // Kiểm tra email/sđt đã tồn tại
                bool emailExists = await dbContext.Users.AnyAsync(u => u.Email == requestDto.Email);
                if (emailExists)
                    return new AuthResult
                    {
                        IsSuccess = false,
                        Status = 409,
                        ErrorCode = "EMAIL_EXISTS",
                        Message = "Địa chỉ email này đã được đăng ký."
                    };

                bool phoneExists = await dbContext.Users.AnyAsync(u => u.Phone == requestDto.PhoneNumber);
                if (phoneExists)
                    return new AuthResult
                    {
                        IsSuccess = false,
                        Status = 409,
                        ErrorCode = "PHONE_EXISTS",
                        Message = "Số điện thoại này đã được đăng ký."
                    };

                // Hash mật khẩu
                string hashed = BCrypt.Net.BCrypt.HashPassword(requestDto.Password, workFactor: 12);

                // Tạo user mới
                var newUser = new User
                {
                    FullName = requestDto.FullName,
                    Email = requestDto.Email,
                    Phone = requestDto.PhoneNumber,
                    PasswordHash = hashed,
                    DateOfBirth = requestDto.DateOfBirth,
                    Gender = requestDto.Gender,
                    Address = requestDto.address,
                    Role = "Candidate",
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow,
                };

                // Tạo Candidate và gán
                var newCandidate = new Candidate();
                newUser.Candidate = newCandidate;

                await dbContext.Users.AddAsync(newUser);
                await dbContext.SaveChangesAsync();

                if (newUser.UserID <= 0)
                    return new AuthResult
                    {
                        IsSuccess = false,
                        Status = 500,
                        ErrorCode = "REGISTER_FAILED",
                        Message = "Đăng ký không thành công, vui lòng thử lại."
                    };

                return new AuthResult
                {
                    IsSuccess = true,
                    Status = 201,
                    Message = "Đăng ký thành công."
                };
            }
            catch (Exception ex)
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    Status = 500,
                    ErrorCode = "SERVER_ERROR",
                    Message = "Đã xảy ra lỗi máy chủ: " + ex.Message
                };
            }
        }
    }
}
