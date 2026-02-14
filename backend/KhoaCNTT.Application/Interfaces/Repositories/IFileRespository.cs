
using KhoaCNTT.Domain.Entities;

namespace KhoaCNTT.Application.Interfaces.Repositories
{
    public interface IFileRepository
    {
        Task AddAsync(FileResource file);
        Task<FileResource?> GetByIdAsync(int id);
        Task UpdateAsync(FileResource file);
        Task DeleteAsync(FileResource file);
        Task<List<FileResource>> GetPendingFilesAsync();
        Task<List<FileResource>> SearchAsync(string keyword, int pageIndex, int pageSize, List<string>? subjectCodes);
    }
}