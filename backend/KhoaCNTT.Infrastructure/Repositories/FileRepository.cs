
using BCrypt.Net;
using KhoaCNTT.Application.Interfaces.Repositories;
using KhoaCNTT.Domain.Entities;
using KhoaCNTT.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace KhoaCNTT.Infrastructure.Repositories
{
    public class FileRepository : IFileRepository
    {
        private readonly AppDbContext _context;

        public FileRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(FileResource file)
        {
            await _context.Files.AddAsync(file);
            await _context.SaveChangesAsync();
        }

        public async Task<FileResource?> GetByIdAsync(int id)
        {
            // return await _context.Files.FindAsync(id);
            return await _context.Files
             .Include(f => f.Subject)
             .Include(f => f.CreatedBy) // Lấy tên người tạo
             .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task UpdateAsync(FileResource file)
        {
            _context.Files.Update(file);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(FileResource file)
        {
            _context.Files.Remove(file);
            await _context.SaveChangesAsync();
        }
        public async Task<List<FileResource>> GetPendingFilesAsync()
        {
            return await _context.Files
                .Include(f => f.Subject)
                .Include(f => f.CreatedBy)
                .Where(f => f.Status == Domain.Enums.FileStatus.Pending)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<FileResource>> SearchAsync(string keyword, int pageIndex, int pageSize, List<string>? subjectCodes)
        {
            var query = _context.Files
                .Include(f => f.Subject)
                .AsQueryable();

            query = query.Where(f => f.Status == Domain.Enums.FileStatus.Approved);

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(f => f.Title.Contains(keyword) || f.FileName.Contains(keyword));
            }

            if (subjectCodes != null && subjectCodes.Any())
            {
                query = query.Where(f => subjectCodes.Contains(f.Subject.SubjectCode));
            }

            return await query
                .OrderByDescending(f => f.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}