using KhoaCNTT.Application.Interfaces.Services;
using KhoaCNTT.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace KhoaCNTT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST: api/Auth/login/admin
        [HttpPost("login/admin")]
        public async Task<IActionResult> LoginAdmin([FromBody] LoginRequest request)
        {
            try
            {
                var token = await _authService.LoginAdminAsync(request.Username, request.Password);
                return Ok(new { Token = token, Role = "Admin", Username = request.Username });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
        }

        // POST: api/Auth/login/student
        [HttpPost("login/student")]
        public async Task<IActionResult> LoginStudent([FromBody] LoginRequest request)
        {
            try
            {
                var token = await _authService.LoginStudentAsync(request.Username, request.Password);
                return Ok(new { Token = token, Role = "Student", msv = request.Username });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
        }
    }

    // Class DTO nhận dữ liệu
    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}