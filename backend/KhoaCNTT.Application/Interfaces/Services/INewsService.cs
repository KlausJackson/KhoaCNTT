using KhoaCNTT.API.Models.Comment;
using KhoaCNTT.Application.DTOs;
using KhoaCNTT.Application.DTOs.News;

namespace KhoaCNTT.Application.Interfaces.Services;

public interface INewsService
{
    // ── News ──────────────────────────────────────────────────────
    Task<IEnumerable<NewsResponse>> GetAllNewsAsync();
    Task<NewsResponse> GetNewsByIdAsync(int id);

    // ── Requests ──────────────────────────────────────────────────
    Task<IEnumerable<NewsRequestResponse>> GetPendingRequestsAsync();
    Task<NewsRequestResponse> SubmitCreateRequestAsync(CreateNewsRequest dto, int submitterId, bool isSenior);
    Task<NewsRequestResponse> SubmitReplaceRequestAsync(UpdateNewsRequest dto, int submitterId, bool isSenior);
    Task DeleteNewsAsync(int newsId);
    Task<NewsApprovalResponse> ProcessApprovalAsync(ApproveNewsRequest dto, int approverId);

    // ── Comments ──────────────────────────────────────────────────
    Task<IEnumerable<CommentResponse>> GetCommentsByNewsIdAsync(int newsId);
    Task<CommentResponse> AddCommentAsync(int newsId, CreateCommentRequest dto, string msv, string studentName);
    Task DeleteCommentAsync(int commentId);
}