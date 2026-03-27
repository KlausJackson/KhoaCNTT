using KhoaCNTT.Application.Interfaces.Repositories.INewsRepositories;
using KhoaCNTT.Domain.Entities.NewsEntities;
using KhoaCNTT.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace KhoaCNTT.Infrastructure.Repositories.News;

public class NewsApprovalRepository(AppDbContext context) : INewsApprovalRepository
{
    public async Task<NewsApproval?> GetByIdAsync(int id) =>
        await context.Set<NewsApproval>().FindAsync(id);

    public async Task<NewsApproval?> GetByRequestIdAsync(int requestId) =>
        await context.Set<NewsApproval>()
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.NewsRequestId == requestId);

    public async Task AddAsync(NewsApproval entity)
    {
        await context.Set<NewsApproval>().AddAsync(entity);
    }

    public Task UpdateAsync(NewsApproval entity)
    {
        context.Set<NewsApproval>().Attach(entity);
        context.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(NewsApproval entity)
    {
        context.Set<NewsApproval>().Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<List<NewsApproval>> GetAllAsync() =>
        await context.Set<NewsApproval>()
            .AsNoTracking()
            .ToListAsync();

    public async Task<List<NewsApproval>> GetAllAsync(Expression<Func<NewsApproval, bool>> predicate) =>
        await context.Set<NewsApproval>()
            .AsNoTracking()
            .Where(predicate)
            .ToListAsync();
}