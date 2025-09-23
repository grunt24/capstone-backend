namespace BackendApi.Core.Models.Dto
{
    public class TeacherDto
    {
        public string? Fullname { get; set; }
        public int UserId { get; set; }
        public List<int> SubjectIds { get; set; } = new List<int>();
    }

    public class TeacherWithSubjectsDto
    {
        public int Id { get; set; }
        public string? Fullname { get; set; }
        public int UserId { get; set; }
        public List<SubjectDto>? Subjects { get; set; }
    }

    public class StudentInfoDto
    {
        public int UserId { get; set; }
        public string? Fullname { get; set; }
        public List<SubjectItemDto> Subjects { get; set; } = new List<SubjectItemDto>();
    }
}
