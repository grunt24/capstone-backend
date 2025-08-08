namespace BackendApi.Core.Models.Dto
{
    public class UserRoleUpdateDto
    {
        public UserRole Role { get; set; }
    }
    public class UserUpdateDto
    {
        public string? Fullname { get; set; }
        public string? Department { get; set; }
        public string? YearLevel { get; set; }
    }
}
