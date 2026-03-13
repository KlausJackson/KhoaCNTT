using KhoaCNTT.Application.DTOs.News;

namespace KhoaCNTT.Application.Interfaces.Services;

public interface INewsService
{
    // ── Read ─────────────────────────────────────────────────────
    Task<IEnumerable<NewsResponse>> GetAllNewsAsync();
    Task<NewsResponse> GetNewsByIdAsync(int id);
    Task<IEnumerable<NewsRequestResponse>> GetPendingRequestsAsync();

    // ── Gửi yêu cầu (mọi cấp) ────────────────────────────────────
    /// <param name="isSenior">true = Cấp 1/2 → tự duyệt ngay; false = Cấp 3 → chờ duyệt</param>
    Task<NewsRequestResponse> SubmitCreateRequestAsync(CreateNewsRequest dto, int submitterId, bool isSenior);
    Task<NewsRequestResponse> SubmitReplaceRequestAsync(UpdateNewsRequest dto, int submitterId, bool isSenior);

    // ── Duyệt / Từ chối (Cấp 1/2) ────────────────────────────────
    Task<NewsApprovalResponse> ProcessApprovalAsync(ApproveNewsRequest dto, int approverId);

    // ── Xóa (Cấp 1/2) ────────────────────────────────────────────
    Task DeleteNewsAsync(int newsId);
}