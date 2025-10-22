namespace BackendApi.Core.Models.Dto
{
    public class StudentSubjectsDto
    {
        public int StudentId { get; set; }
        public List<int>? SubjectIds { get; set; }
    }
    public class StudentSubjectGroupedDto
    {
        public int UserId { get; set; }
        public string Fullname { get; set; } = string.Empty;
        public List<SubjectItemDto> Subjects { get; set; } = new();
    }

    public class SubjectItemDto
    {
        public int SubjectId { get; set; }
        public string? SubjectName { get; set; }
        public string? SubjectCode { get; set; }
        public string? TeacherName { get; set; }
        public string? Department { get; set; }

    }




}
