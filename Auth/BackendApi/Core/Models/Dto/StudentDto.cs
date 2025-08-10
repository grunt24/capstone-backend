namespace BackendApi.Core.Models.Dto
{
    public class StudentDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; }
        public string? Fullname { get; set; }
        public string? Department { get; set; }
        public string? YearLevel { get; set; }


    }
    public class StudentWithGradesDto
    {
        public int StudentId { get; set; }
        public string Fullname { get; set; }
        // We now get grades from the new Grade model
        public double? MainGrade { get; set; }
        public double? CalculatedGrade { get; set; }
    }

    // New DTO to return a subject with its teacher and a list of enrolled students and their grades
    public class SubjectWithStudentsDto
    {
        public int Id { get; set; }
        public string SubjectName { get; set; }
        public string SubjectCode { get; set; }
        public string Description { get; set; }
        public int Credits { get; set; }
        public string TeacherName { get; set; }
        public ICollection<StudentWithGradesDto> Students { get; set; }
    }
}
