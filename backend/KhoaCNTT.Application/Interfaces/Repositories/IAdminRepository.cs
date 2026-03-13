
using KhoaCNTT.Domain.Entities;


namespace KhoaCNTT.Application.Interfaces.Repositories
{
    public interface IAdminRepository
    {
        Task<Admin?> GetByUsernameAsync(string username);
        Task<Admin?> GetByIdAsync(int id);
        Task AddAsync(Admin admin);
        Task UpdateAsync(Admin admin);
        Task DeleteAsync(Admin admin);
        Task<List<Admin>> GetAllAsync();
        Task<bool> ExistsAsync(int requesterId, CancellationToken ct);
    }
}