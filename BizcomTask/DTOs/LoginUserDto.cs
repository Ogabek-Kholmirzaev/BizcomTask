using System.ComponentModel.DataAnnotations;

namespace BizcomTask.DTOs;

public class LoginUserDto
{
    [Required]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")]
    public string Email { get; set; }

    [Required]
    [MinLength(6)]
    public string Password { get; set; }
}