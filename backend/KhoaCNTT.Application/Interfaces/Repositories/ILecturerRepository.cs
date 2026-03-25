using KhoaCNTT.Domain.Entities;
using KhoaCNTT.Domain.Enums;

namespace KhoaCNTT.Application.Interfaces.Repositories
{
    public interface ILecturerRepository
    {
        Task<Lecturer?> GetByIdAsync(int id);
        Task<(List<Lecturer> Items, int TotalCount)> GetAllAsync(string? name, DegreeType? degree, string? position, string? subjectCode, int page, int pageSize);
        Task AddAsync(Lecturer lecturer);
        Task UpdateAsync(Lecturer lecturer);
        Task DeleteAsync(Lecturer lecturer);
    }
}
