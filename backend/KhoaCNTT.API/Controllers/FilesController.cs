
using System.Security.Claims;
using KhoaCNTT.Application.DTOs.File;
using KhoaCNTT.Application.DTOs;
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
            var (_, level) = GetAdminInfoFromToken();
            return level == 1 || level == 2;
        }

        // Upload file
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Upload([FromForm] UploadFileRequest request)
        {
            // Lấy username và level từ Token
            var (username, level) = GetAdminInfoFromToken();
            await _fileService.UploadFileAsync(request, username);

            string? message;
            if (IsAdminLevel12()) {
                message = "Tạo tài liệu mới thành công.";
            } else {
                message = "Tạo yêu cầu duyệt tài liệu mới thành công.";
            }
            return Ok(new { Message = message });
        }

        // Sửa thông tin metadata
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateInfo(int id, [FromBody] UpdateFileRequest request)
        {
            await _fileService.UpdateFileInfoAsync(id, request);
            return Ok(new { Message = "Cập nhật thông tin tài liệu thành công." });
        }

        // Thay thế file
        [HttpPost("{id}/replace")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Replace(int id, [FromForm] UploadFileRequest request)
        {
            var (username, level) = GetAdminInfoFromToken();
            await _fileService.ReplaceFileAsync(id, request, username);
            var message = "";
            if (IsAdminLevel12()) {
                message = "Thay đổi tài liệu thành công.";
            } else {
                message = "Tạo yêu cầu duyệt tài liệu mới thành công.";
            }
            return Ok(new { Message = message });
        }


        // Lấy danh sách file đang chờ duyệt
        [HttpGet("requests/pending")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPendingList()
        {
            if (!IsAdminLevel12()) return Forbid(); // Chỉ Cấp 1, 2 xem được danh sách
            var result = await _fileService.GetPendingRequestsAsync();
            return Ok(result);
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
        [HttpPut("requests/{id}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(int id, [FromBody] ApproveRequest req)
        {
            if (!IsAdminLevel12()) return Forbid(); // Chỉ Cấp 1,2 xem được duyệt
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _fileService.ApproveFileAsync(id, req.IsApproved, req.Reason, username!);
            return Ok(new { Message = "Xử lý yêu cầu duyệt thành công." });
        }

        // Tìm kiếm file / xem toàn bộ file theo trang
        [HttpGet("search")]
        public async Task<IActionResult> Search(
        [FromQuery] string? keyword,
        [FromQuery] List<string>? subjectCodes,
        [FromQuery] string? fileType,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
        {
            // Nếu subjectCodes rỗng thì truyền null xuống dưới
            var codes = (subjectCodes != null && subjectCodes.Count > 0) ? subjectCodes : null;

            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            bool isAdmin = User.IsInRole("Admin");

            var result = await _fileService.SearchFilesAsync(keyword ?? "", codes, fileType, page, pageSize, userIdStr, isAdmin);
            return Ok(result);
        }

        // Xem 1 file
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            bool isAdmin = User.IsInRole("Admin");

            var (stream, contentType) = await _fileService.GetFileByIdAsync(id, userIdStr, isAdmin);
            return File(stream, contentType);
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteFile(int id)
        {
            // Xóa chỉ dành cho admin cấp 1, 2
            if (!IsAdminLevel12()) return Forbid();
            await _fileService.DeleteFileAsync(id);
            return Ok(new { Message = "Đã xóa file thành công." });
        }

        // --- CÁC API THỐNG KÊ (STATS) ---
        [HttpGet("stats/type")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetStatsByFileType() => Ok(await _fileService.GetStatsByFileTypeAsync());

        [HttpGet("stats/subject")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetStatsBySubject() => Ok(await _fileService.GetStatsBySubjectAsync());

        [HttpGet("stats/traffic")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetStatsByTraffic() => Ok(await _fileService.GetStatsByTrafficAsync());
    

        private (string username, int level) GetAdminInfoFromToken()
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            var levelStr = User.FindFirst("Level")?.Value;
            int level = int.TryParse(levelStr, out int l) ? l : 3; // Mặc định cấp 3
            return (username, level);
        }
    }
}