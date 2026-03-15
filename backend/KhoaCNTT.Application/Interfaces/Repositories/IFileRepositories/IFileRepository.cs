using KhoaCNTT.Domain.Entities;
using KhoaCNTT.Domain.Entities.FileEntities;

namespace KhoaCNTT.Application.Interfaces.Repositories
{
    public interface IFileRepository : IRepository<FileEntity>
    {
        Task<PagedResult<FileEntity>> SearchAsync(string? keyword, List<string>? subjectCodes, string? fileType, int page, int pageSize);
        Task<Dictionary<string, int>> GetStatsByFileTypeAsync();
        Task<Dictionary<string, int>> GetStatsBySubjectAsync();
        Task<Dictionary<string, int>> GetStatsByTrafficAsync();
    }
}