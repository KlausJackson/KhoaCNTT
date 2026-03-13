using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KhoaCNTT.Domain.Entities.NewsEntities;

namespace KhoaCNTT.Application.Interfaces.Repositories.INewsRepositories;

public interface INewsApprovalRepository : IRepository<NewsApproval>
{
    Task<NewsApproval?> GetByRequestIdAsync(int requestId, CancellationToken ct = default);
}
