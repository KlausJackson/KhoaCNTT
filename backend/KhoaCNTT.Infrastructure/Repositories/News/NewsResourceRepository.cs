using KhoaCNTT.Application.Interfaces.Repositories.INewsRepositories;
using KhoaCNTT.Domain.Entities.NewsEntities;
using KhoaCNTT.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace KhoaCNTT.Infrastructure.Repositories.News
{
    public class NewsResourceRepository(AppDbContext context) : INewsResourceRepository
    {
        public async Task<NewsResource?> GetByIdAsync(int id) =>
            await context.Set<NewsResource>().FindAsync(id);

        public async Task AddAsync(NewsResource entity)
        {
            await context.Set<NewsResource>().AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(NewsResource entity)
        {
            context.Set<NewsResource>().Update(entity);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(NewsResource entity)
        {
            context.Set<NewsResource>().Remove(entity);
            await context.SaveChangesAsync();
        }

        public async Task<List<NewsResource>> GetAllAsync() =>
            await context.Set<NewsResource>().AsNoTracking().ToListAsync();

        public async Task<List<NewsResource>> GetAllAsync(Expression<Func<NewsResource, bool>> predicate) =>
            await context.Set<NewsResource>().AsNoTracking().Where(predicate).ToListAsync();
    }
}