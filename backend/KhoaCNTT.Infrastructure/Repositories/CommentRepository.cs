using KhoaCNTT.Application.Interfaces.Repositories.INewsRepositories;
using KhoaCNTT.Domain.Entities.NewsEntities;
using KhoaCNTT.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace KhoaCNTT.Infrastructure.Repositories.News;

public class CommentRepository(AppDbContext context) : ICommentRepository
{
    public async Task<Comment?> GetByIdAsync(int id) =>
        await context.Set<Comment>().FindAsync(id);

    public async Task<IEnumerable<Comment>> GetByNewsIdAsync(int newsId) =>
        await context.Set<Comment>()
            .Where(c => c.NewsId == newsId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

    public async Task AddAsync(Comment entity)
    {
        context.Set<Comment>().Add(entity);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Comment entity)
    {
        context.Set<Comment>().Update(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Comment entity)
    {
        context.Set<Comment>().Remove(entity);
        await context.SaveChangesAsync();
    }

    public async Task<List<Comment>> GetAllAsync() =>
        await context.Set<Comment>().ToListAsync();

    public async Task<List<Comment>> GetAllAsync(
        Expression<Func<Comment, bool>> predicate) =>
        await context.Set<Comment>().Where(predicate).ToListAsync();
}