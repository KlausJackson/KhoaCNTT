using KhoaCNTT.Application.DTOs.School;

namespace KhoaCNTT.Application.Interfaces.Services
{
    public interface ISchoolApiService
    {
        // Trả về access_token nếu thành công, null nếu thất bại
        Task<string?> LoginStudentAsync(string username, string password);

        // Trả về List object đã được làm sạch
        Task<List<ScoreResponse>> GetScoresAsync(string accessToken);
        Task<List<ScheduleResponse>> GetScheduleAsync(string accessToken, string semesterId);
    }
}
