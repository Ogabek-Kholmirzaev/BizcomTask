#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;

namespace BizcomTask.DTOs;

public class RegisterUserDto
{
    [Required]
    [RegularExpression("^[A-Za-z]+$")]
    public string FirstName { get; set; }

    [Required]
    [RegularExpression("^[A-Za-z]+$")]
    public string LastName { get; set; }

    [Required]
    [RegularExpression(@"\+9989\d{8}")]
    public string PhoneNumber { get; set; }

    [Required]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")]
    public string Email { get; set; }

    [Required]
    [MinLength(6)]
    public string Password { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime BirthDate { get; set; }
}