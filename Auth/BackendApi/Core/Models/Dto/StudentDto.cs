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
}
