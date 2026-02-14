
using KhoaCNTT.Domain.Entities;


namespace KhoaCNTT.Application.Interfaces.Repositories
{
    public interface ISubjectRepository
    {
        Task<List<Subject>> GetAllAsync();
        Task<Subject?> GetByCodeAsync(string code);
    }
}