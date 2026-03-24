using KhoaCNTT.Domain.Entities.NewsEntities;

namespace KhoaCNTT.Application.Interfaces.Repositories.INewsRepositories;

public interface ICommentRepository : IRepository<Comment>
{
    Task<Comment?> GetByIdAsync(int id);
    Task<IEnumerable<Comment>> GetByNewsIdAsync(int newsId);
}