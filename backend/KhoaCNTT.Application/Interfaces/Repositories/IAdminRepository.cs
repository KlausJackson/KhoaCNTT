
using KhoaCNTT.Domain.Entities;


namespace KhoaCNTT.Application.Interfaces.Repositories
{
    public interface IAdminRepository
    {
        Task<AdminUser?> GetByUsernameAsync(string username);
        Task<AdminUser?> GetByIdAsync(int id);
        Task AddAsync(AdminUser admin);
        Task UpdateAsync(AdminUser admin);
        Task DeleteAsync(AdminUser admin);
        Task<List<AdminUser>> GetAllAsync();
    }
}