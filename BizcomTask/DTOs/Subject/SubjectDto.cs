using BizcomTask.DTOs.StudentSubject;
using BizcomTask.DTOs.TeacherSubject;

#pragma warning disable CS8618
namespace BizcomTask.DTOs.Subject;

public class SubjectDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Key { get; set; }

    public List<StudentSubjectDto> Students { get; set; }
    public List<TeacherSubjectDto> Teachers { get; set; }
}