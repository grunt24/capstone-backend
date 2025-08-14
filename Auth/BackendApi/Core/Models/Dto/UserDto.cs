namespace BackendApi.Core.Models.Dto
{
    public class UserDto
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public UserRole Role { get; set; }
        public string? Fullname { get; set; }
    }
}
