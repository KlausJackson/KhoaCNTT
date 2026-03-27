using KhoaCNTT.API.Models.Comment;
using KhoaCNTT.Application.DTOs;
using KhoaCNTT.Application.DTOs.News;
using KhoaCNTT.Application.Interfaces.Repositories;
using KhoaCNTT.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KhoaCNTT.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NewsController(INewsService newsService, IAdminRepository adminRepo) : ControllerBase
{
    // ── Helpers phân quyền ────────────────────────────────────────

    private int GetAdminLevel() =>
        int.TryParse(User.FindFirst("Level")?.Value, out var level) ? level : 0;

    private bool IsLevel1Or2() => GetAdminLevel() is 1 or 2;
    private bool IsAnyAdminLevel() => GetAdminLevel() is 1 or 2 or 3;

    private async Task<int> GetCurrentAdminIdAsync()
    {
        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                       ?? User.FindFirst(ClaimTypes.Name)?.Value
                       ?? User.FindFirst("username")?.Value
                       ?? "";

        if (string.IsNullOrEmpty(username)) return 0;
        var admin = await adminRepo.GetByUsernameAsync(username);
        return admin?.Id ?? 0;
    }

    // Lấy MSV từ token trường (sub / mssv / MSV / NameIdentifier)
    // Token trường không có role="Student" nên không dùng Roles="Student"
    private (string msv, string studentName) GetStudentInfo() => (
        User.FindFirst("MSV")?.Value
            ?? User.FindFirst("mssv")?.Value
            ?? User.FindFirst("sub")?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? "",
        User.FindFirst("FullName")?.Value
            ?? User.FindFirst("fullname")?.Value
            ?? User.FindFirst(ClaimTypes.Name)?.Value
            ?? "Sinh viên"
    );

    // ── Public ────────────────────────────────────────────────────

    // GET: api/News
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await newsService.GetAllNewsAsync();
        return Ok(result);
    }

    // GET: api/News/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var result = await newsService.GetNewsByIdAsync(id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return NotFound(new { ex.Message, Detail = ex.InnerException?.Message });
        }
    }

    // ── Bình luận ─────────────────────────────────────────────────

    // GET: api/News/{newsId}/comments  — public
    [HttpGet("{newsId}/comments")]
    public async Task<IActionResult> GetComments(int newsId)
    {
        var result = await newsService.GetCommentsByNewsIdAsync(newsId);
        return Ok(result);
    }

    // POST: api/News/{newsId}/comments  — sinh viên hoặc admin đã đăng nhập
    // Không dùng [Authorize] vì token sinh viên là GUID, không phải JWT
    // MSV và StudentName được gửi trực tiếp từ frontend (lấy từ localStorage sau login)
    [HttpPost("{newsId}/comments")]
    public async Task<IActionResult> AddComment(int newsId, [FromBody] CreateCommentRequest dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { Message = "Nội dung bình luận không được để trống." });

        try
        {
            string msv, displayName;

            if (User.Identity?.IsAuthenticated == true && IsAnyAdminLevel())
            {
                // Admin đã đăng nhập bằng JWT chuẩn
                msv = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
                displayName = msv;
            }
            else
            {
                // Sinh viên: lấy từ body (frontend gửi kèm sau khi login hệ thống trường)
                msv = dto.Msv ?? "";
                displayName = dto.StudentName ?? "Sinh viên";
            }

            if (string.IsNullOrEmpty(msv))
                return BadRequest(new { Message = "Vui lòng đăng nhập để bình luận." });

            var result = await newsService.AddCommentAsync(newsId, dto, msv, displayName);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { ex.Message, Detail = ex.InnerException?.Message });
        }
    }

    // DELETE: api/News/comments/{commentId}  — admin cấp 1, 2, 3
    [HttpDelete("comments/{commentId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteComment(int commentId)
    {
        if (!IsAnyAdminLevel()) return Forbid();

        try
        {
            await newsService.DeleteCommentAsync(commentId);
            return Ok(new { Message = "Đã xóa bình luận thành công." });
        }
        catch (Exception ex)
        {
            return NotFound(new { ex.Message, Detail = ex.InnerException?.Message });
        }
    }

    // ── Tạo / Sửa tin tức ────────────────────────────────────────

    // POST: api/News/requests/create
    [HttpPost("requests/create")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SubmitCreate([FromBody] CreateNewsRequest dto)
    {
        if (!IsAnyAdminLevel()) return Forbid();

        try
        {
            var adminId = await GetCurrentAdminIdAsync();
            if (adminId == 0)
                return Unauthorized(new { Message = "Token không hợp lệ hoặc tài khoản Admin không còn tồn tại trong DB." });

            var result = await newsService.SubmitCreateRequestAsync(dto, adminId, IsLevel1Or2());
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { ex.Message, Detail = ex.InnerException?.Message });
        }
    }

    // POST: api/News/requests/update
    [HttpPost("requests/update")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SubmitUpdate([FromBody] UpdateNewsRequest dto)
    {
        if (!IsAnyAdminLevel()) return Forbid();

        try
        {
            var adminId = await GetCurrentAdminIdAsync();
            if (adminId == 0)
                return Unauthorized(new { Message = "Token không hợp lệ hoặc tài khoản Admin không còn tồn tại trong DB." });

            var result = await newsService.SubmitReplaceRequestAsync(dto, adminId, IsLevel1Or2());
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { ex.Message, Detail = ex.InnerException?.Message });
        }
    }

    // ── Xóa tin tức ──────────────────────────────────────────────

    // DELETE: api/News/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        if (!IsLevel1Or2()) return Forbid();

        try
        {
            await newsService.DeleteNewsAsync(id);
            return Ok(new { Message = "Đã xóa tin tức" });
        }
        catch (Exception ex)
        {
            return NotFound(new { ex.Message, Detail = ex.InnerException?.Message });
        }
    }

    // ── Phê duyệt ────────────────────────────────────────────────

    // GET: api/News/requests/pending
    [HttpGet("requests/pending")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetPendingRequests()
    {
        if (!IsAnyAdminLevel()) return Forbid();

        var result = await newsService.GetPendingRequestsAsync();
        return Ok(result);
    }

    // PUT: api/News/requests/{id}/approve
    [HttpPut("requests/{id}/approve")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Approve(int id, [FromBody] ApproveNewsRequest dto)
    {
        if (!IsLevel1Or2()) return Forbid();

        try
        {
            dto.NewsRequestID = id;
            var approverId = await GetCurrentAdminIdAsync();
            var result = await newsService.ProcessApprovalAsync(dto, approverId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { ex.Message, Detail = ex.InnerException?.Message });
        }
    }
}