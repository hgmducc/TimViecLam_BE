using Microsoft.AspNetCore.Mvc;
using TimViecLam.Models.Dto.Request;
using TimViecLam.Repository.IRepository;
using TimViecLam.Models.Dto.Response;

namespace TimViecLam.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository authRepository;

        public AuthController(IAuthRepository authRepository)
        {
            this.authRepository = authRepository;
        }

        [HttpPost("register/candidate")]
        public async Task<IActionResult> RegisterCandidate([FromBody] RegisterCandidateRequest requestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Dữ liệu không hợp lệ." });
            }

            AuthResult result = await authRepository.RegisterCandidateAsync(requestDto);

            return StatusCode(result.Status, result);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest requestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Dữ liệu không hợp lệ." });
            }

            AuthResult result = await authRepository.LoginAsync(requestDto);

            return StatusCode(result.Status, result);
        }
    }
}
