using TimViecLam.Models.Dto.Request;
using TimViecLam.Models.Dto.Response;

namespace TimViecLam.Repository.IRepository
{
    public interface IAuthRepository
    {
        Task<AuthResult> RegisterCandidateAsync(RegisterCandidateRequest requestDto);
        Task<AuthResult> LoginAsync(LoginRequest requestDto);
    }
}
