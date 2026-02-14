
using System.Security.Claims;
using KhoaCNTT.Application.DTOs.File;
using KhoaCNTT.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KhoaCNTT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FilesController(IFileService fileService)
        {
            _fileService = fileService;
        }

        // Middleware kiểm tra quyền cấp 1, 2
        private bool IsAdminLevel12()
        {
            var levelStr = User.FindFirst("Level")?.Value;
            return int.TryParse(levelStr, out int level) && (level == 1 || level == 2);
        }

        // Upload file
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Upload([FromForm] UploadFileRequest request)
        {
            // Lấy username và level từ Token
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var levelStr = User.FindFirst("AdminLevel")?.Value;
            int level = int.TryParse(levelStr, out int l) ? l : 3; // Mặc định cấp 3 nếu ko có

            await _fileService.UploadFileAsync(request, level, username!);
            return Ok("Upload thành công");
        }

        // Lấy danh sách file đang chờ duyệt
        [HttpGet("pending")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPendingList()
        {
            if (!IsAdminLevel12()) return Forbid(); // Chỉ Cấp 1, 2 xem được danh sách
            var result = await _fileService.GetPendingFilesAsync();
            return Ok(result);
        }

        // Sửa thông tin metadata
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateInfo(int id, [FromBody] UpdateFileRequest request)
        {
            await _fileService.UpdateFileInfoAsync(id, request);
            return Ok(new { Message = "Cập nhật thông tin thành công" });
        }

        [HttpGet("{id}/download")]
        public async Task<IActionResult> Download(int id)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            bool isAdmin = User.IsInRole("Admin");

            var (stream, contentType, fileName) = await _fileService.DownloadFileAsync(id, userIdStr, isAdmin);

            return File(stream, contentType, fileName);
        }

        // Duyệt file
        [HttpPut("{id}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(int id, [FromBody] ApproveRequest req)
        {
            if (!IsAdminLevel12()) return Forbid(); // Chỉ Cấp 1,2 xem được duyệt
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _fileService.ApproveFileAsync(id, req.IsApproved, req.RejectReason, username!);
            return Ok();
        }

        // Tìm kiếm file / xem toàn bộ file theo trang
        [HttpGet("search")]
        public async Task<IActionResult> Search(
        [FromQuery] string? keyword,
        [FromQuery] List<string>? subjectCodes,
        [FromQuery] int page = 1)
        {
            // Nếu subjectCodes rỗng thì truyền null xuống dưới
            var codes = (subjectCodes != null && subjectCodes.Count > 0) ? subjectCodes : null;

            var result = await _fileService.SearchFilesAsync(keyword ?? "", codes, page, 10); // PageSize mặc định 10
            return Ok(result);
        }

        // Xem 1 file

    }

    public class ApproveRequest { public bool IsApproved { get; set; } public string? RejectReason { get; set; } }
}