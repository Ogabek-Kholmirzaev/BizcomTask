using System.ComponentModel.DataAnnotations;
#pragma warning disable CS8618

namespace BizcomTask.DTOs.Subject;

public class UpdateSubjectDto
{
    [Required]
    public string Name { get; set; }
}