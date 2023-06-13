using BizcomTask.DTOs.StudentSubject;
using BizcomTask.DTOs.Subject;
using BizcomTask.DTOs.TeacherSubject;
using BizcomTask.Entities;
using Mapster;

namespace BizcomTask.Mappers;

public static class SubjectMapper
{
    public static SubjectDto ToDto(this Subject subject)
    {
        return new SubjectDto
        {
            Id = subject.Id,
            Name = subject.Name,
            Key = subject.Key,
            Students = subject.Students.Adapt<List<StudentSubjectDto>>(),
            Teachers = subject.Teachers.Adapt<List<TeacherSubjectDto>>()
        };
    }
}