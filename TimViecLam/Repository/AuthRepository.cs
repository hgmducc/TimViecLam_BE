using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
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
        private string secretKey;

        public AuthRepository(ApplicationDbContext dbContext, IConfiguration configuration)
        {
            this.dbContext = dbContext;
            secretKey = configuration.GetValue<string>("ApiSetting:Secret");
        }
        public async Task<AuthResult> LoginAsync(LoginRequest requestDto)
        {
            try
            {
                // tìm thong tin user theo email/sdt
                var user = await dbContext.Users
                    .FirstOrDefaultAsync(u => u.Email == requestDto.Email || u.Phone == requestDto.PhoneNumber);
                if (user == null)
                {
                    return new AuthResult
                    {
                        IsSuccess = false,
                        Status = 404,
                        ErrorCode = "USER_NOT_FOUND",
                        Message = "Không tìm thấy người dùng với email/số điện thoại đã cung cấp."
                    };
                }

                // Nếu user đăng ký bằng Google thì không có password
                if (user.PasswordHash == null)
                {
                    return new AuthResult
                    {
                        IsSuccess = false,
                        Status = 400,
                        ErrorCode = "GOOGLE_LOGIN_REQUIRED",
                        Message = "Tài khoản này được đăng ký bằng Google. Vui lòng đăng nhập bằng Google."
                    };
                }

                // kiểm tra mật khẩu
                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(requestDto.Password, user.PasswordHash);
                if(!isPasswordValid)
                {
                    return new AuthResult
                    {
                        IsSuccess = false,
                        Status = 401,
                        ErrorCode = "INVALID_CREDENTIALS",
                        Token = "",
                        Message = "Mật khẩu không đúng. Vui lòng thử lại."
                    };
                }

                // tồn tại sinh token 
                var token = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(secretKey);
                var tokenDescriptor = new SecurityTokenDescriptor
                {

                }

                // Đăng nhập thành công
                return new AuthResult
                {
                    IsSuccess = true,
                    Status = 200,
                    Message = "Đăng nhập thành công."
                };
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần thiết
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

                // thêm vào database
                var newCandidate = new Candidate();
                newUser.Candidate = newCandidate;

                await dbContext.Users.AddAsync(newUser);
                await dbContext.SaveChangesAsync();
                if (newUser.UserID <= 0)
                {
                    return new AuthResult
                    {
                        IsSuccess = false,
                        Status = 500,
                        ErrorCode = "REGISTER_FAILED",
                        Message = "Đăng ký không thành công, vui lòng thử lại."
                    };
                }

                return new AuthResult
                {
                    IsSuccess = true,
                    Status = 201,
                    Message = "Đăng ký thành công."
                };
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần thiết
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
