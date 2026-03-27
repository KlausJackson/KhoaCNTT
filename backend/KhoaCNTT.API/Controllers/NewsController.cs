using KhoaCNTT.Application.DTOs.News;
using KhoaCNTT.Application.Interfaces.Services.INewsServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KhoaCNTT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController(INewsService _newsService) : ControllerBase
    {
        private string GetUsername() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateNews([FromBody] CreateNewsRequest req)
        {
            await _newsService.CreateNewsAsync(req, GetUsername());
            return Ok(new { Message = "Yêu cầu tạo tin tức thành công" });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateNews(int id, [FromBody] UpdateNewsRequest req)
        {
            await _newsService.UpdateNewsAsync(id, req, GetUsername());
            return Ok(new { Message = "Yêu cầu sửa tin tức thành công" });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteNews(int id)
        {
            // Có thể thêm logic kiểm tra Level 1, 2 ở đây
            await _newsService.DeleteNewsAsync(id);
            return Ok(new { Message = "Xóa tin tức thành công" });
        }

        [HttpPut("requests/{id}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveNews(int id, [FromBody] ApproveRequest req)
        {
            await _newsService.ApproveNewsAsync(id, req.IsApproved, req.Reason, GetUsername());
            return Ok(new { Message = "Đã duyệt yêu cầu" });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetNewsById(int id) => Ok(await _newsService.GetNewsByIdAsync(id));

        [HttpGet("search")]
        public async Task<IActionResult> SearchNews([FromQuery] string? keyword, [FromQuery] string? newsType, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = User.IsInRole("Admin");
            return Ok(await _newsService.SearchNewsAsync(keyword, newsType, page, pageSize, userId, isAdmin));
        }

        [HttpGet("pending")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPendingList() => Ok(await _newsService.GetPendingRequestsAsync());

        [HttpGet("stats/month")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetStatsByMonth([FromQuery] int year) => Ok(await _newsService.GetStatsByMonthAsync(year));

        [HttpGet("stats/type")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetStatsByType() => Ok(await _newsService.GetStatsByTypeAsync());

        [HttpPost("{id}/comments")]
        public async Task<IActionResult> AddComment(int id, [FromBody] AddCommentRequest req) // Bạn tạo thêm class nhỏ này nhé
        {
            await _newsService.AddCommentAsync(id, req.MSV, req.StudentName, req.Content);
            return Ok(new { Message = "Bình luận thành công" });
        }

        [HttpDelete("comments/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            await _newsService.DeleteCommentAsync(id);
            return Ok(new { Message = "Xóa bình luận thành công" });
        }
    }

    public class AddCommentRequest
    {
        public string MSV { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}