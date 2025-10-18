namespace BackendApi.Core.Models.Dto
{
        public class AddQuizInputDto
    {
        public int SubjectId { get; set; } 
        public string QuizLabel { get; set; } = string.Empty;
            public int QuizTotal { get; set; }
            public List<StudentQuizScoreDto> StudentScores { get; set; } = new();
        public int AcademicPeriodId { get; set; }
    }

    public class StudentQuizScoreDto
        {
            public int MidtermGradeId { get; set; }
            public int StudentId { get; set; }
            public int QuizScore { get; set; }
        }
}
