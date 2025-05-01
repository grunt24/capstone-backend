using System.ComponentModel.DataAnnotations;

namespace Capstone.Core.Entities.Dto;

public class UserDto
{
    [Required]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }
}