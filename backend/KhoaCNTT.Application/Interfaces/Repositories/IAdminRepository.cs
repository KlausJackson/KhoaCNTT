
using KhoaCNTT.Domain.Entities;
using KhoaCNTT.Domain.Entities.FileEntities;


namespace KhoaCNTT.Application.Interfaces.Repositories
{
    public interface IAdminRepository : IRepository<Admin>
    {
        Task<Admin?> GetByUsernameAsync(string username);
        Task<Admin?> GetByEmailAsync(string email);
    }
}