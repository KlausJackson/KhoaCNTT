using KhoaCNTT.Application.Interfaces.Repositories;
using KhoaCNTT.Domain.Entities.FileEntities;
using KhoaCNTT.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KhoaCNTT.Infrastructure.Repositories.File
{

    public class FileResourceRepository(AppDbContext context) : IFileResourceRepository
    {
        private readonly AppDbContext _context = context;

        public async Task AddAsync(FileResource entity) { _context.Set<FileResource>().Add(entity); await _context.SaveChangesAsync(); }
        public async Task<FileResource?> GetByIdAsync(int id) => await _context.Set<FileResource>().FindAsync(id);

        // Các hàm IRepository khác...
        public Task DeleteAsync(FileResource entity) => throw new NotImplementedException();
        public Task UpdateAsync(FileResource entity) => throw new NotImplementedException();
        public Task<List<FileResource>> GetAllAsync() => throw new NotImplementedException();
        public Task<List<FileResource>> GetAllAsync(System.Linq.Expressions.Expression<Func<FileResource, bool>> predicate) => throw new NotImplementedException();
    }
}
