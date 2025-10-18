namespace BackendApi.Core.Models.Dto
{
    public class AddClassStandingInputDto
    {
        public string Label { get; set; } = string.Empty; // e.g., "SW/ASS/GRP WRK 1"
        public int SubjectId { get; set; }
        public int Total { get; set; }
        public List<ClassStandingScoreDto> StudentScores { get; set; } = new();
        public int AcademicPeriodId { get; set; }

    }

    public class ClassStandingScoreDto
    {
        public int StudentId { get; set; }
        public int MidtermGradeId { get; set; }
        public int Score { get; set; }
    }

}
