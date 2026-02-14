
using KhoaCNTT.Application.Interfaces.Repositories;
using KhoaCNTT.Domain.Entities;
using KhoaCNTT.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace KhoaCNTT.Infrastructure.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly AppDbContext _context;

        public AdminRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AdminUser?> GetByUsernameAsync(string username)
        {
            return await _context.AdminUsers
                .FirstOrDefaultAsync(u => u.Username == username);
        }


        public async Task<AdminUser?> GetByIdAsync(int id)
        {
            return await _context.AdminUsers.FindAsync(id);
        }

        public async Task AddAsync(AdminUser admin)
        {
            await _context.AdminUsers.AddAsync(admin);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(AdminUser admin)
        {
            _context.AdminUsers.Update(admin);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(AdminUser admin)
        {
            _context.AdminUsers.Remove(admin);
            await _context.SaveChangesAsync();
        }

        public async Task<List<AdminUser>> GetAllAsync()
        {
            return await _context.AdminUsers.ToListAsync();
        }
    }
}