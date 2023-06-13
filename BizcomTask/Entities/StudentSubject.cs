#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations.Schema;

namespace BizcomTask.Entities;

public class StudentSubject
{
    public int Id { get; set; }
    public int Grade { get; set; }

    public int StudentId { get; set; }
    [ForeignKey(nameof(StudentId))]
    public virtual Student Student { get; set; }

    public int SubjectId { get; set; }
    [ForeignKey(nameof(SubjectId))]
    public virtual Subject Subject { get; set; }
}