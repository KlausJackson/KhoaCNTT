using KhoaCNTT.Application.DTOs.School;
using KhoaCNTT.Application.Interfaces.Services;
using System.Net.Http.Headers;

using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KhoaCNTT.Infrastructure.ExternalServices
{
    public class SchoolApiClient : ISchoolApiService
    {
        private readonly HttpClient _httpClient;

        public SchoolApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // Cấu hình options: Chấp nhận mọi kiểu hoa/thường, số/chuỗi
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };

        // --- 1. LOGIN ---
        public async Task<string?> LoginStudentAsync(string username, string password)
        {
            try
            {
                var loginData = new Dictionary<string, string>
                {
                    { "client_id", "education_client" },
                    { "grant_type", "password" },
                    { "username", username },
                    { "password", password },
                    { "client_secret", "password" }
                };

                var content = new FormUrlEncodedContent(loginData);

                // Gọi API: /education/oauth/token
                var response = await _httpClient.PostAsync("/education/oauth/token", content);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Login failed with status code: {response.StatusCode}");
                    return null;
                }

                var result = await response.Content.ReadFromJsonAsync<SchoolLoginResponse>();
                return result?.AccessToken;
            }
            catch
            {
                return null;
            }
        }


        // --- 2. GET SCORES (ĐIỂM) ---
        public async Task<List<ScoreResponse>> GetScoresAsync(string accessToken)
        {
            SetupToken(accessToken);

            try
            {
                var response = await _httpClient.GetAsync("/education/api/studentsubjectmark/getListStudentMarkBySemesterByLoginUser/0");

                if (!response.IsSuccessStatusCode) return new List<ScoreResponse>();

                var jsonString = await response.Content.ReadAsStringAsync();

                var rawList = JsonSerializer.Deserialize<List<RawScoreElement>>(jsonString, _jsonOptions);

                if (rawList == null) return new List<ScoreResponse>();

                return rawList.Select(x => new ScoreResponse
                {
                    SubjectName = x.Subject?.SubjectName ?? "Unknown",
                    Credits = x.Subject?.NumberOfCredit ?? 0,
                    IsCalculated = x.Subject?.IsCalculateMark ?? false,
                    ProcessMark = x.MarkQT ?? 0,
                    ExamMark = x.MarkTHI ?? 0,
                    FinalMark = x.Mark ?? 0,
                    CharMark = x.CharMark ?? ""
                }).ToList();
            }
            catch
            {
                return new List<ScoreResponse>();
            }
        }

        // --- 3. GET SCHEDULE (THỜI KHÓA BIỂU) ---
        public async Task<List<ScheduleResponse>> GetScheduleAsync(string accessToken, string semesterId)
        {
            SetupToken(accessToken);

            try
            {
                // API: /education/api/StudentCourseSubject/studentLoginUser/{semesterId}
                var response = await _httpClient.GetAsync($"/education/api/StudentCourseSubject/studentLoginUser/{semesterId}");
                if (!response.IsSuccessStatusCode) return new List<ScheduleResponse>();

                var jsonString = await response.Content.ReadAsStringAsync();


                //// return full JSON for debugging
                //return new List<ScheduleResponse>
                //{
                //    new ScheduleResponse
                //    {
                //        SubjectCode = "DEBUG",
                //        Room = jsonString,
                //    }
                //};

                var rawList = JsonSerializer.Deserialize<List<RawScheduleElement>>(jsonString, _jsonOptions);
                var result = new List<ScheduleResponse>();

                if (rawList == null) return result;

                foreach (var subject in rawList)
                {
                    var subjectCode = subject.SubjectCode ?? "Unknown";
                    var timetables = subject.CourseSubject?.Timetables;

                    if (timetables != null)
                    {
                        foreach (var tt in timetables)
                        {
                            result.Add(new ScheduleResponse
                            {
                                SubjectCode = subjectCode,
                                Room = tt.RoomName ?? "Unknown",
                                StartHour = tt.StartHour?.StartString ?? "",
                                EndHour = tt.EndHour?.EndString ?? "",
                                StartDateTimestamp = tt.StartDate ?? 0,
                                EndDateTimestamp = tt.EndDate ?? 0,
                                WeekIndex = tt.WeekIndex ?? 0
                            });
                        }
                    }
                }
                return result;
            }
            catch
            {
                return new List<ScheduleResponse>();
            }
        }

        // Helper set header
        private void SetupToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        // --- Class JSON Điểm ---
        private class RawScoreElement
        {
            [JsonPropertyName("markQT")] public double? MarkQT { get; set; }
            [JsonPropertyName("markTHI")] public double? MarkTHI { get; set; }
            [JsonPropertyName("mark")] public double? Mark { get; set; }
            [JsonPropertyName("charMark")] public string? CharMark { get; set; }
            [JsonPropertyName("subject")] public RawSubject? Subject { get; set; }
        }
        private class RawSubject
        {
            [JsonPropertyName("subjectName")] public string? SubjectName { get; set; }
            [JsonPropertyName("numberOfCredit")] public int? NumberOfCredit { get; set; }
            [JsonPropertyName("isCalculateMark")] public bool? IsCalculateMark { get; set; }
        }

        // --- Class JSON Thời khóa biểu ---
        private class RawScheduleElement
        {
            [JsonPropertyName("subjectCode")] public string? SubjectCode { get; set; }
            [JsonPropertyName("courseSubject")] public RawCourseSubject? CourseSubject { get; set; }
        }
        private class RawCourseSubject
        {
            [JsonPropertyName("timetables")] public List<RawTimetable>? Timetables { get; set; }
        }
        private class RawTimetable
        {
            [JsonPropertyName("roomName")] public string? RoomName { get; set; }
            [JsonPropertyName("startDate")] public long? StartDate { get; set; }
            [JsonPropertyName("endDate")] public long? EndDate { get; set; }
            [JsonPropertyName("weekIndex")] public int? WeekIndex { get; set; }
            [JsonPropertyName("startHour")] public RawHour? StartHour { get; set; }
            [JsonPropertyName("endHour")] public RawHour? EndHour { get; set; }
        }
        private class RawHour
        {
            [JsonPropertyName("startString")] public string? StartString { get; set; }
            [JsonPropertyName("endString")] public string? EndString { get; set; }
        }

    }
}



