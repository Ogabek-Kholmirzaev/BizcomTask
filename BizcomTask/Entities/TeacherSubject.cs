#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations.Schema;

namespace BizcomTask.Entities;

public class TeacherSubject
{
    public int Id { get; set; }
    public bool IsAdmin { get; set; }
    
    public int TeacherId { get; set; }
    [ForeignKey(nameof(TeacherId))]
    public virtual Teacher Teacher { get; set; }

    public int SubjectId { get; set; }
    [ForeignKey(nameof(SubjectId))]
    public virtual Subject Subject { get; set; }
}