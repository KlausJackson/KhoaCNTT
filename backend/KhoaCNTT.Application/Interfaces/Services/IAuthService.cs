
namespace KhoaCNTT.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<string> LoginAdminAsync(string username, string password);
        Task<string> LoginStudentAsync(string username, string password);
    }
}