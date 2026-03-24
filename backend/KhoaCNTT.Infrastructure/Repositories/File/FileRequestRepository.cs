using KhoaCNTT.Application.Interfaces.Repositories;
using KhoaCNTT.Domain.Entities.FileEntities;
using KhoaCNTT.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace KhoaCNTT.Infrastructure.Repositories.File
{

    public class FileRequestRepository(AppDbContext context) : IFileRequestRepository
    {
        private readonly AppDbContext _context = context;

        public async Task AddAsync(FileRequest entity) { _context.Set<FileRequest>().Add(entity); await _context.SaveChangesAsync(); }
        public async Task UpdateAsync(FileRequest entity) { _context.Set<FileRequest>().Update(entity); await _context.SaveChangesAsync(); }
        public async Task<FileRequest?> GetByIdAsync(int id) => await _context.Set<FileRequest>()
            .Include(r => r.NewResource).Include(r => r.TargetFile).FirstOrDefaultAsync(r => r.Id == id);

        public async Task<List<FileRequest>> GetPendingRequestsWithDetailsAsync()
        {
            return await _context.Set<FileRequest>()
                .AsNoTracking()
                .Include(r => r.TargetFile)
                .Include(r => r.NewResource)
                    .ThenInclude(nr => nr.Admin)
                .Include(r => r.NewResource)
                .Include(r => r.OldResource)
                .Where(r => !r.IsProcessed)
                .OrderBy(r => r.CreatedAt)
                .ToListAsync();
        }

        // Các hàm IRepository khác...
        public Task DeleteAsync(FileRequest entity) => throw new NotImplementedException();
        public Task<List<FileRequest>> GetAllAsync() => throw new NotImplementedException();
        public Task<List<FileRequest>> GetAllAsync(System.Linq.Expressions.Expression<Func<FileRequest, bool>> predicate) => throw new NotImplementedException();
    }
}
