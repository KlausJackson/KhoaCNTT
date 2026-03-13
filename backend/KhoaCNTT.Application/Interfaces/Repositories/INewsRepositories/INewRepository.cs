using KhoaCNTT.Domain.Entities;
using KhoaCNTT.Domain.Entities.NewsEntities;

namespace KhoaCNTT.Application.Interfaces.Repositories.INewsRepositories;

public interface INewsRepository : IRepository<News>
{
    Task<News?> GetByIdWithResourceAsync(int newsId, CancellationToken ct = default);
    Task<IEnumerable<News>> GetAllWithResourceAsync(CancellationToken ct = default);
    Task IncrementViewCountAsync(int newsId, CancellationToken ct = default);
}
