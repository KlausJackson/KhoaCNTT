
using System.Text.Json.Serialization;

namespace KhoaCNTT.Application.DTOs.School
{
    public class SchoolLoginResponse
    {
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }

        [JsonPropertyName("token_type")]
        public string? TokenType { get; set; }
    }
}
