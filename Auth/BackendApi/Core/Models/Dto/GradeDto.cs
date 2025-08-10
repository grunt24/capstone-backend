using System.Collections.Generic;

namespace BackendApi.Core.Models.Dto
{
    public class GradeDto
    {
        public int StudentSubjectId { get; set; }
        public int StudentId { get; set; }
        public string? StudentFullname { get; set; }
        public int SubjectId { get; set; }
        public string? SubjectName { get; set; }
        public List<GradeItemDto> Scores { get; set; } = new List<GradeItemDto>();
        public double? MainGrade { get; set; }
        public double? CalculatedGrade { get; set; }
    }

    // DTO for a single score item
    public class GradeItemDto
    {
        public string Type { get; set; }
        public int Score { get; set; }
        public int Total { get; set; }
    }

    // New DTO for saving grades
    public class SaveGradesDto
    {
        public int StudentId { get; set; }
        public int SubjectId { get; set; }
        public List<GradeItemDto> Scores { get; set; } = new List<GradeItemDto>();
        public double? CalculatedGrade { get; set; }
        public double? MainGrade { get; set; }
    }
}