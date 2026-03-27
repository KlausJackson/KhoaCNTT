
using KhoaCNTT.Domain.Entities.NewsEntities;
namespace KhoaCNTT.Application.Interfaces.Repositories.INewsRepositories;

public interface INewsRepository : IRepository<News>
{
        Task<PagedResult<News>> SearchAsync(string? keyword, string? newsType, int page, int pageSize);
        Task<News?> GetByIdWithDetailsAsync(int id);
        Task IncrementViewCountAsync(int newsId);
        Task<Dictionary<string, int>> GetStatsByTypeAsync();
        Task<Dictionary<string, int>> GetStatsByMonthAsync(int year);
}