using KhoaCNTT.Domain.Entities.NewsEntities;

namespace KhoaCNTT.Application.Interfaces.Repositories.INewsRepositories;

public interface INewsRequestRepository : IRepository<NewsRequest>
{
     Task<IEnumerable<NewsRequest>> GetPendingRequestsAsync();
}