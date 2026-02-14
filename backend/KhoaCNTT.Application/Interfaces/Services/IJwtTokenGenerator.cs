
namespace KhoaCNTT.Application.Interfaces.Services
{
    public interface IJwtTokenGenerator
    {
        string GenerateAdminToken(string username, int level);
    }
}
