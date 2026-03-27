using KhoaCNTT.Application.Interfaces.Repositories.INewsRepositories;
using KhoaCNTT.Domain.Entities.NewsEntities;
using KhoaCNTT.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace KhoaCNTT.Infrastructure.Repositories.News
{
    public class NewsRequestRepository(AppDbContext context) : INewsRequestRepository
    {
        public async Task<NewsRequest?> GetByIdAsync(int id) =>
            await context.Set<NewsRequest>()
                .Include(r => r.NewResource)
                .Include(r => r.OldResource)
                .FirstOrDefaultAsync(r => r.Id == id);

        // Đã đổi tên hàm khớp với Interface (Fix CS0535)
        public async Task<IEnumerable<NewsRequest>> GetPendingRequestsAsync() =>
            await context.Set<NewsRequest>()
                .AsNoTracking()
                .Include(r => r.NewResource)
                    .ThenInclude(nr => nr.Admin) // <-- Thêm dòng này để DTO lấy được tên người yêu cầu
                .Include(r => r.OldResource)
                .Where(r => !r.IsProcessed)
                .OrderBy(r => r.CreatedAt)
                .ToListAsync();

        // Fix lỗi thiếu SaveChangesAsync của teammate
        public async Task AddAsync(NewsRequest entity)
        {
            await context.Set<NewsRequest>().AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(NewsRequest entity)
        {
            context.Set<NewsRequest>().Update(entity);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(NewsRequest entity)
        {
            context.Set<NewsRequest>().Remove(entity);
            await context.SaveChangesAsync();
        }

        public async Task<List<NewsRequest>> GetAllAsync() =>
            await context.Set<NewsRequest>().AsNoTracking().ToListAsync();

        public async Task<List<NewsRequest>> GetAllAsync(Expression<Func<NewsRequest, bool>> predicate) =>
            await context.Set<NewsRequest>().AsNoTracking().Where(predicate).ToListAsync();
    }
}