using KhoaCNTT.Application.DTOs;
using KhoaCNTT.Application.Interfaces.Repositories.INewsRepositories;
using KhoaCNTT.Domain.Enums;
using KhoaCNTT.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
// BÍ DANH GIẢI QUYẾT XUNG ĐỘT NAMESPACE (LỖI CS0118)
using NewsEntity = KhoaCNTT.Domain.Entities.NewsEntities.News;

namespace KhoaCNTT.Infrastructure.Repositories.News
{
    public class NewsRepository(AppDbContext context) : INewsRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<NewsEntity?> GetByIdWithDetailsAsync(int id) =>
            await _context.Set<NewsEntity>()
                .Include(n => n.CurrentResource)
                .Include(n => n.Admin) // Đảm bảo Entity News có thuộc tính AdminUser Admin {get;set;}
                .Include(n => n.Comments)
                .FirstOrDefaultAsync(n => n.Id == id);

        public async Task<PagedResult<NewsEntity>> SearchAsync(string? keyword, string? newsTypeStr, int page, int pageSize)
        {
            var query = _context.Set<NewsEntity>()
                .Include(n => n.CurrentResource)
                .Include(n => n.Admin)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(n => n.Title.Contains(keyword));

            if (!string.IsNullOrEmpty(newsTypeStr) && Enum.TryParse<NewsType>(newsTypeStr, out var parsedNewsType))
                query = query.Where(n => n.NewsType == parsedNewsType);

            var total = await query.CountAsync();
            var items = await query.OrderByDescending(n => n.CreatedAt)
                                   .Skip((page - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

            return new PagedResult<NewsEntity> { Total = total, Items = items };
        }

        public async Task IncrementViewCountAsync(int newsId) =>
            await _context.Set<NewsEntity>().Where(n => n.Id == newsId)
                 .ExecuteUpdateAsync(s => s.SetProperty(n => n.ViewCount, n => n.ViewCount + 1));

        public async Task<Dictionary<string, int>> GetStatsByTypeAsync() =>
            await _context.Set<NewsEntity>()
                .AsNoTracking()
                .GroupBy(n => n.NewsType)
                .Select(g => new { Type = g.Key.ToString(), Count = g.Count() })
                .ToDictionaryAsync(x => x.Type, x => x.Count);

        public async Task<Dictionary<string, int>> GetStatsByMonthAsync(int year) =>
            await _context.Set<NewsEntity>()
                .AsNoTracking()
                .Where(n => n.CreatedAt.Year == year)
                .GroupBy(n => n.CreatedAt.Month)
                .Select(g => new { Month = "Tháng " + g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Month, x => x.Count);

        // CRUD CƠ BẢN
        public async Task<NewsEntity?> GetByIdAsync(int id) => await _context.Set<NewsEntity>().FindAsync(id);
        public async Task AddAsync(NewsEntity entity) { _context.Set<NewsEntity>().Add(entity); await _context.SaveChangesAsync(); }
        public async Task UpdateAsync(NewsEntity entity) { _context.Set<NewsEntity>().Update(entity); await _context.SaveChangesAsync(); }
        public async Task DeleteAsync(NewsEntity entity) { _context.Set<NewsEntity>().Remove(entity); await _context.SaveChangesAsync(); }

        public Task<List<NewsEntity>> GetAllAsync() => throw new NotImplementedException();
        public Task<List<NewsEntity>> GetAllAsync(Expression<Func<NewsEntity, bool>> predicate) => throw new NotImplementedException();
    }
}