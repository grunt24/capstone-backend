namespace BackendApi.Core.Models.Dto
{
    public class SubjectDto
    {
        public string? SubjectName { get; set; }
        public string? SubjectCode { get; set; }
        public string? Description { get; set; }
        public int Credits { get; set; }
        public int? TeacherId { get; set; }
        public string? Department { get; set; }
    }

    public class SubjectWithTeacherDto
    {
        public int Id { get; set; }
        public string? SubjectName { get; set; }
        public string? SubjectCode { get; set; }
        public string? Description { get; set; }
        public int Credits { get; set; }
        public string? TeacherName { get; set; }
        public string? SubjectDepartment { get; set; }


    }
}
