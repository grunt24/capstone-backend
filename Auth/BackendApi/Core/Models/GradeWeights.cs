namespace BackendApi.Core.Models
{
    public class GradeWeights
    {
        public int Id { get; set; }
        // Quizzes are 30% of the midterm grade
        public decimal QuizWeighted { get; set; } = 0.30m;

        // Class Standing is 25% of the midterm grade
        public decimal ClassStandingWeighted { get; set; } = 0.25m;

        // Speak English Policy (SEP) is 5% of the midterm grade
        public decimal SEPWeighted { get; set; } = 0.05m;

        // Project is 10% of the midterm grade
        public decimal ProjectWeighted { get; set; } = 0.10m;

        // The Midterm Exam is 30% of the midterm grade
        public decimal MidtermWeighted { get; set; } = 0.30m;
    }
}