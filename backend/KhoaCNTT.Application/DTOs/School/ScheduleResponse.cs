

namespace KhoaCNTT.Application.DTOs.School
{
    public class ScheduleResponse
    {
        public string SubjectCode { get; set; } = string.Empty;
        public string Room { get; set; } = string.Empty;
        public string StartHour { get; set; } = string.Empty;
        public string EndHour { get; set; } = string.Empty;
        public long StartDateTimestamp { get; set; }
        public long EndDateTimestamp { get; set; }
        public int WeekIndex { get; set; }
    }
}