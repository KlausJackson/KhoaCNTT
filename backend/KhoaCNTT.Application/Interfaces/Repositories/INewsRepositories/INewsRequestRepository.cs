using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KhoaCNTT.Domain.Entities.NewsEntities;

namespace KhoaCNTT.Application.Interfaces.Repositories.INewsRepositories;


public interface INewsRequestRepository : IRepository<NewsRequest>
{
    Task<IEnumerable<NewsRequest>> GetPendingAsync(CancellationToken ct = default);
    Task<NewsRequest?> GetByIdWithDetailsAsync(int id, CancellationToken ct = default);
}
