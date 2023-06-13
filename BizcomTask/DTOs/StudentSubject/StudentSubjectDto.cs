#pragma warning disable CS8618
namespace BizcomTask.DTOs.StudentSubject;

public class StudentSubjectDto
{
    public int Id { get; set; }
    public int Grade { get; set; }
    public int StudentId { get; set; }
    public int SubjectId { get; set; }
    public string SubjectName { get; set; }
}