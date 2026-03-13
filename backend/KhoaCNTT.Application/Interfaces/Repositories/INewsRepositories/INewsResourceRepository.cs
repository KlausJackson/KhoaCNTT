using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KhoaCNTT.Domain.Entities.NewsEntities;

namespace KhoaCNTT.Application.Interfaces.Repositories.INewsRepositories;

public interface INewsResourceRepository : IRepository<NewsResource>
{
    Task<NewsResource?> GetByIdAsync(int id, CancellationToken ct = default);
}
