
using KhoaCNTT.Application.DTOs.News;
using KhoaCNTT.Application.DTOs;

namespace KhoaCNTT.Application.Interfaces.Services.INewsServices;
public interface INewsService
{
    Task CreateNewsAsync(CreateNewsRequest req, string username);
    Task UpdateNewsAsync(int id, UpdateNewsRequest req, string username);
    Task DeleteNewsAsync(int id);
    Task ApproveNewsAsync(int requestId, bool isApproved, string? reason, string username);
    Task<NewsResponse> GetNewsByIdAsync(int id);
    Task<PagedResult<NewsResponse>> SearchNewsAsync(string? keyword, string? newsType, int page, int pageSize, string? userId, bool isAdmin);
    Task<PagedResult<NewsRequestDto>> GetPendingRequestsAsync();

    Task<Dictionary<string, int>> GetStatsByTypeAsync();
    Task<Dictionary<string, int>> GetStatsByMonthAsync(int year);

    Task AddCommentAsync(int newsId, string msv, string studentName, string content);
    Task DeleteCommentAsync(int id);
}