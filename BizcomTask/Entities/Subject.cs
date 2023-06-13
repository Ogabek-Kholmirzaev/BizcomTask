#pragma warning disable CS8618
namespace BizcomTask.Entities;

public class Subject
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Key { get; set; }

    public virtual List<StudentSubject> Students { get; set; }
    public virtual List<TeacherSubject> Teachers { get; set; }
}