
using KhoaCNTT.Application.DTOs.Admin;

public interface IAdminService
{
    Task<List<AdminResponse>> GetAllAdminsAsync();
    Task CreateAdminAsync(CreateAdminRequest request);
    Task UpdateAdminAsync(int id, UpdateAdminRequest request);
    Task DeleteAdminAsync(string currentUsername, int id);
    Task ChangePasswordAsync(int id, string newPassword);
}