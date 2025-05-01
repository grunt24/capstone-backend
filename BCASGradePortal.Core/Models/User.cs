using System.ComponentModel.DataAnnotations;

namespace BCASGradePortal.Core.Models;

public enum UserRole
{
    Superadmin = 1,
    Admin,
    User,
}

public class User
{
    [Key]
    public int Id { get; set; }
    [MaxLength(30)]
    public required string Username { get; set; }
    [MaxLength(100)]
    public string Password { get; set; }
    public UserRole Role { get; set; } = UserRole.Superadmin;
}