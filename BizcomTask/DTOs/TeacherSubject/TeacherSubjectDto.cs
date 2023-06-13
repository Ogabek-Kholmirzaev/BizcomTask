#pragma warning disable CS8618
namespace BizcomTask.DTOs.TeacherSubject;

public class TeacherSubjectDto
{
    public int Id { get; set; }
    public int SubjectId { get; set; }
    public bool IsAdmin { get; set; }
    public int TeacherId { get; set; }
    public string SubjectName { get; set; }
}