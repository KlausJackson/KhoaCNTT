
using System.Security.Claims;
using KhoaCNTT.Application.DTOs.Admin;
using KhoaCNTT.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KhoaCNTT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Phải là Admin
    public class AdminsController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminsController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        // Middleware kiểm tra quyền Cấp 1
        private bool IsRootAdmin()
        {
            var levelStr = User.FindFirst("Level")?.Value;
            return int.TryParse(levelStr, out int level) && level == 1;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            if (!IsRootAdmin()) return Forbid(); // Chỉ Cấp 1 xem được danh sách

            var result = await _adminService.GetAllAdminsAsync();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAdminRequest request)
        {
            if (!IsRootAdmin()) return Forbid();

            await _adminService.CreateAdminAsync(request);
            return Ok(new { Message = "Tạo tài khoản thành công" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAdminRequest request)
        {
            if (!IsRootAdmin()) return Forbid();

            // API này dùng chung cho: Sửa thông tin, Phân quyền (đổi Level), Vô hiệu hóa (đổi IsActive)
            await _adminService.UpdateAdminAsync(id, request);
            return Ok(new { Message = "Cập nhật thành công" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsRootAdmin()) return Forbid();
            var currentUsername = User.Identity?.Name ?? "";
            await _adminService.DeleteAdminAsync(currentUsername, id);
            return Ok(new { Message = "Đã xóa tài khoản" });
        }

        //[HttpPut("{id}/password")]
        //public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordRequest req)
        //{
        //    if (!IsRootAdmin()) return Forbid();

        //    await _adminService.ChangePasswordAsync(id, req.NewPassword);
        //    return Ok(new { Message = "Đổi mật khẩu thành công" });
        //}
    }

    //public class ChangePasswordRequest { public string NewPassword { get; set; } }
}