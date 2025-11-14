using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimViecLam.Data;
using TimViecLam.Models.Dto.Request;

namespace TimViecLam.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;

        // inject chuỗi kết nối database
        public AuthController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IActionResult RegisterCandidate(RegisterCandidateRequest rgCandidateDto)
        {


            // mã hóa mật khẩu người dùng nhập vào


            return Ok();
        }
    }
}
