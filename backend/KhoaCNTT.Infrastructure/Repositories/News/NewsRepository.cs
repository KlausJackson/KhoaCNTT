using KhoaCNTT.Application.Interfaces.Repositories;
using KhoaCNTT.Domain.Entities.NewsEntities;
using KhoaCNTT.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

using KhoaCNTT.Application.Interfaces.Repositories.INewsRepositories;

    // ── NewsRepository ────────────────────────────────────────────

    public class NewsRepository : INewsRepository
    {
        private readonly AppDbContext _context;
        public NewsRepository(AppDbContext context) => _context = context;

        public async Task<News?> GetByIdAsync(int id) =>
            await _context.Set<News>()
                .Include(n => n.CurrentResource)
                .FirstOrDefaultAsync(n => n.Id == id);

        public async Task<News?> GetByIdWithResourceAsync(int newsId, CancellationToken ct = default) =>
            await _context.Set<News>()
                .Include(n => n.CurrentResource)
                .FirstOrDefaultAsync(n => n.Id == newsId, ct);

        public async Task<IEnumerable<News>> GetAllWithResourceAsync(CancellationToken ct = default) =>
            await _context.Set<News>()
                .Include(n => n.CurrentResource)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync(ct);

        public async Task IncrementViewCountAsync(int newsId, CancellationToken ct = default) =>
            await _context.Set<News>()
                .Where(n => n.Id == newsId)
                .ExecuteUpdateAsync(s => s.SetProperty(n => n.ViewCount, n => n.ViewCount + 1), ct);

        public async Task AddAsync(News entity, CancellationToken ct)
        {
            _context.Set<News>().Add(entity);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(News entity, CancellationToken ct)
        {
            _context.Set<News>().Update(entity);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(News entity, CancellationToken ct)
        {
            _context.Set<News>().Remove(entity);
            await _context.SaveChangesAsync(ct);
        }

        public Task<List<News>> GetAllAsync() => throw new NotImplementedException();
        public Task<List<News>> GetAllAsync(Expression<Func<News, bool>> predicate) => throw new NotImplementedException();
    }

    // ── NewsResourceRepository ────────────────────────────────────

    public class NewsResourceRepository : INewsResourceRepository
    {
        private readonly AppDbContext _context;
        public NewsResourceRepository(AppDbContext context) => _context = context;

        public async Task<NewsResource?> GetByIdAsync(int id) =>
            await _context.Set<NewsResource>().FindAsync(id);

        public async Task<NewsResource?> GetByIdAsync(int id, CancellationToken ct = default) =>
            await _context.Set<NewsResource>().FindAsync(new object[] { id }, ct);

        public async Task AddAsync(NewsResource entity, CancellationToken ct)
        {
            _context.Set<NewsResource>().Add(entity);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(NewsResource entity, CancellationToken ct)
        {
            _context.Set<NewsResource>().Update(entity);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(NewsResource entity, CancellationToken ct)
        {
            _context.Set<NewsResource>().Remove(entity);
            await _context.SaveChangesAsync(ct);
        }

        public Task<List<NewsResource>> GetAllAsync() => throw new NotImplementedException();
        public Task<List<NewsResource>> GetAllAsync(Expression<Func<NewsResource, bool>> predicate) => throw new NotImplementedException();
    }

    // ── NewsRequestRepository ─────────────────────────────────────

    public class NewsRequestRepository : INewsRequestRepository
    {
        private readonly AppDbContext _context;
        public NewsRequestRepository(AppDbContext context) => _context = context;

        public async Task<NewsRequest?> GetByIdAsync(int id) =>
            await _context.Set<NewsRequest>()
                .Include(r => r.NewResource)
                .Include(r => r.OldResource)
                .Include(r => r.TargetNews)
                .FirstOrDefaultAsync(r => r.Id == id);

        public async Task<NewsRequest?> GetByIdWithDetailsAsync(int id, CancellationToken ct = default) =>
            await _context.Set<NewsRequest>()
                .Include(r => r.NewResource)
                .Include(r => r.OldResource)
                .Include(r => r.TargetNews)
                .FirstOrDefaultAsync(r => r.Id == id, ct);

        public async Task<IEnumerable<NewsRequest>> GetPendingAsync(CancellationToken ct = default) =>
            await _context.Set<NewsRequest>()
                .AsNoTracking()
                .Include(r => r.NewResource)
                    .ThenInclude(nr => nr.Admin)
                .Include(r => r.OldResource)
                .Include(r => r.TargetNews)
                .Where(r => !r.IsProcessed)
                .OrderBy(r => r.CreatedAt)
                .ToListAsync(ct);

        public async Task AddAsync(NewsRequest entity, CancellationToken ct)
        {
            _context.Set<NewsRequest>().Add(entity);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(NewsRequest entity, CancellationToken ct)
        {
            _context.Set<NewsRequest>().Update(entity);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(NewsRequest entity, CancellationToken ct)
        {
            _context.Set<NewsRequest>().Remove(entity);
            await _context.SaveChangesAsync(ct);
        }

        public Task<List<NewsRequest>> GetAllAsync() => throw new NotImplementedException();
        public Task<List<NewsRequest>> GetAllAsync(Expression<Func<NewsRequest, bool>> predicate) => throw new NotImplementedException();
    }

    // ── NewsApprovalRepository ────────────────────────────────────

    public class NewsApprovalRepository : INewsApprovalRepository
    {
        private readonly AppDbContext _context;
        public NewsApprovalRepository(AppDbContext context) => _context = context;

        public async Task<NewsApproval?> GetByIdAsync(int id) =>
            await _context.Set<NewsApproval>().FindAsync(id);

        public async Task<NewsApproval?> GetByRequestIdAsync(int requestId, CancellationToken ct = default) =>
            await _context.Set<NewsApproval>()
                .FirstOrDefaultAsync(a => a.NewsRequestId == requestId, ct);

        public async Task AddAsync(NewsApproval entity, CancellationToken ct)
        {
            _context.Set<NewsApproval>().Add(entity);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(NewsApproval entity, CancellationToken ct)
        {
            _context.Set<NewsApproval>().Update(entity);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(NewsApproval entity, CancellationToken ct)
        {
            _context.Set<NewsApproval>().Remove(entity);
            await _context.SaveChangesAsync(ct);
        }

        public Task<List<NewsApproval>> GetAllAsync() => throw new NotImplementedException();
        public Task<List<NewsApproval>> GetAllAsync(Expression<Func<NewsApproval, bool>> predicate) => throw new NotImplementedException();
    }
