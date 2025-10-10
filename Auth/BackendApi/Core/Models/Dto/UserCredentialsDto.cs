namespace BackendApi.Core.Models.Dto
{
    public class UserCredentialsDto
    {
        public required string StudentNumber { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public string? Fullname { get; set; }
        public string? Department { get; set; }
        public string? YearLevel { get; set; }
    }
    public class LoginDto
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
    public class CreateTeacherWithAccountDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Fullname { get; set; } = string.Empty;
        public List<int> SubjectIds { get; set; } = new();
    }
}
