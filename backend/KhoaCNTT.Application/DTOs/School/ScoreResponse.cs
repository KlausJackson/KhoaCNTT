

namespace KhoaCNTT.Application.DTOs.School
{
    public class ScoreResponse
    {
        public string SubjectName { get; set; } = string.Empty;
        public int Credits { get; set; }
        public double ProcessMark { get; set; } // markQT
        public double ExamMark { get; set; }    // markTHI
        public double FinalMark { get; set; }   // mark
        public string CharMark { get; set; } = string.Empty;
        public bool IsCalculated { get; set; }
    }
}