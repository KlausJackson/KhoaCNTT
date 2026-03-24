
using KhoaCNTT.Application.Interfaces.Repositories;
using KhoaCNTT.Domain.Entities;
using KhoaCNTT.Domain.Entities.FileEntities;
using KhoaCNTT.Domain.Enums;
using KhoaCNTT.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace KhoaCNTT.Infrastructure.Repositories
{
    public class FileRepository(AppDbContext context) : IFileRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<FileEntity?> GetByIdAsync(int id) =>
            await _context.Set<FileEntity>()
            .Include(f => f.CurrentResource)
            .Include(f => f.Subject)
            .FirstOrDefaultAsync(f => f.Id == id);

        public async Task AddAsync(FileEntity entity) { _context.Set<FileEntity>().Add(entity); await _context.SaveChangesAsync(); }
        public async Task UpdateAsync(FileEntity entity) { _context.Set<FileEntity>().Update(entity); await _context.SaveChangesAsync(); }
        public async Task DeleteAsync(FileEntity entity) { _context.Set<FileEntity>().Remove(entity); await _context.SaveChangesAsync(); }

        public Task<List<FileEntity>> GetAllAsync() => throw new NotImplementedException();
        public Task<List<FileEntity>> GetAllAsync(System.Linq.Expressions.Expression<Func<FileEntity, bool>> predicate) => throw new NotImplementedException();

        public async Task<PagedResult<FileEntity>> SearchAsync(string? keyword, List<string>? subjectCodes, string? fileType, int page, int pageSize)
        {
            var query = _context.Set<FileEntity>().Include(f => f.CurrentResource)
                .Include(f => f.Subject)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(f => f.Title.Contains(keyword) || (f.CurrentResource.FileName.Contains(keyword)));

            if (subjectCodes != null && subjectCodes.Any())
            {
                query = query.Where(f => f.SubjectCode != null && subjectCodes.Contains(f.SubjectCode));
            }

            if (!string.IsNullOrEmpty(fileType) && Enum.TryParse<FileType>(fileType, true, out var parsedFileType))
            {
                query = query.Where(f => f.FileType == parsedFileType);
            }

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(f => f.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<FileEntity>
            {
                Items = items,
                Total = total
            };
        }

        public async Task<Dictionary<string, int>> GetStatsByFileTypeAsync()
        {
            return await _context.Set<FileEntity>()
                .AsNoTracking()
                .GroupBy(f => f.FileType)
                .Select(g => new { Key = g.Key.ToString(), Count = g.Count() })
                .ToDictionaryAsync(x => x.Key, x => x.Count);
        }

        public async Task<Dictionary<string, int>> GetStatsBySubjectAsync()
        {
            return await _context.Set<FileEntity>()
                .AsNoTracking()
                .Where(f => f.SubjectCode != null)
                .GroupBy(f => f.SubjectCode)
                .Select(g => new { Key = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Key!, x => x.Count);
        }

        public async Task<Dictionary<string, int>> GetStatsByTrafficAsync()
        {
            var totalViews = await _context.Set<FileEntity>()
                .AsNoTracking().SumAsync(f => f.ViewCount);
            var totalDownloads = await _context.Set<FileEntity>().SumAsync(f => f.DownloadCount);

            return new Dictionary<string, int>
            {
                { "Views", totalViews },
                { "Downloads", totalDownloads }
            };
        }
    }
}