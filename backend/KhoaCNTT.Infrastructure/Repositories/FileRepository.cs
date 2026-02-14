
using BCrypt.Net;
using KhoaCNTT.Application.Interfaces.Repositories;
using KhoaCNTT.Domain.Entities;
using KhoaCNTT.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            return await _context.Files.FindAsync(id);
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
                .Where(f => f.Status == Domain.Enums.FileStatus.Pending)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<FileResource>> SearchAsync(string keyword, int pageIndex, int pageSize, List<string>? subjectCodes)
        {
            var query = _context.Files.AsQueryable();

            // 1. Chỉ lấy file đã duyệt
            query = query.Where(f => f.Status == Domain.Enums.FileStatus.Approved);

            // 2. Lọc theo từ khóa (Tiêu đề hoặc Tên file)
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(f => f.Title.Contains(keyword) || f.FileName.Contains(keyword));
            }

            // 3. Lọc theo DANH SÁCH mã môn
            if (subjectCodes != null && subjectCodes.Any())
            {
                query = query.Where(f => subjectCodes.Contains(f.SubjectCode));
            }

            return await query
                .OrderByDescending(f => f.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}