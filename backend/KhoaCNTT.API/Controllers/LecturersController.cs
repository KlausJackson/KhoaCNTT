using KhoaCNTT.Application.DTOs.Lecturer;
using KhoaCNTT.Application.Interfaces.Services;
using KhoaCNTT.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KhoaCNTT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LecturersController : ControllerBase
    {
        private readonly ILecturerService _lecturerService;

        public LecturersController(ILecturerService lecturerService)
        {
            _lecturerService = lecturerService;
        }

        /// <summary>
        /// Xem danh sách giảng viên và tìm kiếm (dành cho Sinh viên / Khách – không bắt buộc đăng nhập).
        /// Tìm theo: tên, học vị, chức vụ, môn học (mã hoặc tên).
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetList([FromQuery] string? name, [FromQuery] int? degree, [FromQuery] string? position, [FromQuery] string? subjectCodeOrName, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var searchParams = new LecturerSearchParams
            {
                Name = name,
                Degree = degree.HasValue ? (DegreeType)degree.Value : null,
                Position = position,
                SubjectCodeOrName = subjectCodeOrName,
                Page = page,
                PageSize = pageSize
            };
            var result = await _lecturerService.GetListAsync(searchParams);
            return Ok(result);
        }

        /// <summary>
        /// Xem chi tiết một giảng viên (dành cho Sinh viên / Khách – không bắt buộc đăng nhập).
        /// </summary>
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var lecturer = await _lecturerService.GetByIdAsync(id);
            if (lecturer == null) return NotFound();
            return Ok(lecturer);
        }

        /// <summary>
        /// Tạo giảng viên mới. Chỉ Admin đã đăng nhập.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateLecturerRequest request)
        {
            await _lecturerService.CreateLecturerAsync(request);
            return Ok(new { Message = "Tạo giảng viên thành công." });
        }

        /// <summary>
        /// Cập nhật giảng viên. Chỉ Admin đã đăng nhập.
        /// </summary>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateLecturerRequest request)
        {
            await _lecturerService.UpdateLecturerAsync(id, request);
            return Ok(new { Message = "Cập nhật giảng viên thành công." });
        }

        /// <summary>
        /// Xóa giảng viên. Chỉ Admin đã đăng nhập.
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _lecturerService.DeleteLecturerAsync(id);
            return Ok(new { Message = "Đã xóa giảng viên." });
        }
    }
}
