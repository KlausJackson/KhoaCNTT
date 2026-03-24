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

    public class FileApprovalRepository(AppDbContext context) : IFileApprovalRepository
    {
        private readonly AppDbContext _context = context;

        public async Task AddAsync(FileApproval entity) { _context.Set<FileApproval>().Add(entity); await _context.SaveChangesAsync(); }

        // Các hàm IRepository khác...
        public Task DeleteAsync(FileApproval entity) => throw new NotImplementedException();
        public Task<FileApproval?> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task UpdateAsync(FileApproval entity) => throw new NotImplementedException();
        public Task<List<FileApproval>> GetAllAsync() => throw new NotImplementedException();
        public Task<List<FileApproval>> GetAllAsync(System.Linq.Expressions.Expression<Func<FileApproval, bool>> predicate) => throw new NotImplementedException();
    }
}
