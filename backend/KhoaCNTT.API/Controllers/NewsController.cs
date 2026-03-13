using System.Security.Claims;
using KhoaCNTT.Application.DTOs.News;
using KhoaCNTT.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KhoaCNTT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly INewsService _newsService;

        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }

        // Kiểm tra Admin cấp 1 hoặc 2 (có quyền duyệt)
        private bool IsAdminLevel12()
        {
            var levelStr = User.FindFirst("Level")?.Value;
            return int.TryParse(levelStr, out int level) && (level == 1 || level == 2);
        }

        private int GetCurrentAdminId()
        {
            var idStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(idStr, out int id) ? id : 0;
        }

        // ── Public ────────────────────────────────────────────────

        // GET: api/News
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _newsService.GetAllNewsAsync();
            return Ok(result);
        }

        // GET: api/News/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _newsService.GetNewsByIdAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        // ── Tạo tin tức (Sequence 2.3.6.1) ───────────────────────
        // POST: api/News/requests/create
        [HttpPost("requests/create")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SubmitCreate([FromBody] CreateNewsRequest dto)
        {
            try
            {
                var adminId = GetCurrentAdminId();
                var result = await _newsService.SubmitCreateRequestAsync(dto, adminId, IsAdminLevel12());
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // ── Sửa tin tức (Sequence 2.3.6.2) ───────────────────────
        // POST: api/News/requests/update
        [HttpPost("requests/update")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SubmitUpdate([FromBody] UpdateNewsRequest dto)
        {
            try
            {
                var adminId = GetCurrentAdminId();
                var result = await _newsService.SubmitReplaceRequestAsync(dto, adminId, IsAdminLevel12());
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // ── Xóa tin tức (Sequence 2.3.6.3) ───────────────────────
        // DELETE: api/News/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsAdminLevel12()) return Forbid(); // Chỉ cấp 1, 2 xóa được

            try
            {
                await _newsService.DeleteNewsAsync(id);
                return Ok(new { Message = "Đã xóa tin tức" });
            }
            catch (Exception ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        // ── Phê duyệt ────────────────────────────────────────────
        // GET: api/News/requests/pending
        [HttpGet("requests/pending")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPendingRequests()
        {
            if (!IsAdminLevel12()) return Forbid(); // Chỉ cấp 1, 2 xem được

            var result = await _newsService.GetPendingRequestsAsync();
            return Ok(result);
        }

        // PUT: api/News/requests/{id}/approve
        [HttpPut("requests/{id}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(int id, [FromBody] ApproveNewsRequest dto)
        {
            if (!IsAdminLevel12()) return Forbid(); // Chỉ cấp 1, 2 duyệt được

            try
            {
                dto.NewsRequestID = id;
                var approverId = GetCurrentAdminId();
                var result = await _newsService.ProcessApprovalAsync(dto, approverId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}