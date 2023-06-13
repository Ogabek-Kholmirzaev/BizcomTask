using System.Security.Claims;
using BizcomTask.DTOs.StudentSubject;
using BizcomTask.DTOs.Subject;
using BizcomTask.Entities;
using BizcomTask.Mappers;
using BizcomTask.Repository.IRepository;
using BizcomTask.Statics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BizcomTask.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class SubjectsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public SubjectsController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<SubjectDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllSubjects()
    {
        var subjects = _unitOfWork.SubjectRepository.GetAll().ToList();
        var subjectsDto = subjects.Select(s => s.ToDto()).ToList();

        return Ok(subjectsDto);
    }

    [HttpGet("student")]
    [Authorize(Roles = UserRoles.Student)]
    [ProducesResponseType(typeof(List<StudentSubjectDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStudentSubject()
    {
        var studentSubjects = _unitOfWork.StudentSubjectRepository.GetAll().ToList();
        var studentSubjectsDto = studentSubjects.Select(studentSubject => new StudentSubjectDto
            {
                Id = studentSubject.Id,
                Grade = studentSubject.Grade,
                StudentId = studentSubject.StudentId,
                SubjectId = studentSubject.SubjectId,
                SubjectName = studentSubject.Subject.Name
            })
            .ToList();

        return Ok(studentSubjectsDto);
    }

    [Authorize(Roles = UserRoles.Teacher)]
    [HttpPost]
    [ProducesResponseType(typeof(List<SubjectDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateSubject([FromBody] CreateSubjectDto createSubjectDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var subject = new Subject
        {
            Name = createSubjectDto.Name,
            Key = Guid.NewGuid().ToString(),
            Students = new List<StudentSubject>(),
            Teachers = new List<TeacherSubject>()
        };

        _unitOfWork.SubjectRepository.Add(subject);
        await _unitOfWork.SaveChangesAsync();

        _unitOfWork.TeacherSubjectRepository.Add(new TeacherSubject
        {
            IsAdmin = true,
            TeacherId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)),
            SubjectId = subject.Id,
        });

        await _unitOfWork.SaveChangesAsync();

        return Ok(subject.ToDto());
    }

    [HttpGet("{subjectId}")]
    [ProducesResponseType(typeof(SubjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSubjectById(int subjectId)
    {
        var subject = _unitOfWork.SubjectRepository.Get(s => s.Id == subjectId);

        if (subject == null)
            return NotFound();

        return Ok(subject.ToDto());
    }

    [Authorize(Roles = UserRoles.Teacher)]
    [HttpPut("{subjectId}")]
    [ProducesResponseType(typeof(SubjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateSubject(int subjectId, [FromBody] UpdateSubjectDto updateSubjectDto)
    {
        var subject = _unitOfWork.SubjectRepository.Get(s => s.Id == subjectId);

        if (subject == null)
            return NotFound();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!subject.Teachers.Any(t => t.TeacherId.ToString() == teacherId && t.IsAdmin))
            return Forbid();

        subject.Name = updateSubjectDto.Name;
        await _unitOfWork.SaveChangesAsync();

        return Ok(subject.ToDto());
    }

    [HttpDelete("{subjectId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSubject(int subjectId)
    {
        var subject = _unitOfWork.SubjectRepository.Get(s => s.Id == subjectId);

        if (subject == null)
            return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!subject.Teachers.Any(t => t.TeacherId.ToString() == userId && t.IsAdmin))
            return Forbid();

        _unitOfWork.SubjectRepository.Remove(subject);
        await _unitOfWork.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("{subjectId}/join")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> JoinSubject(int subjectId, [FromBody] JoinSubjectDto joinSubjectDto)
    {
        var subject = _unitOfWork.SubjectRepository.Get(s => s.Id == subjectId);

        if (subject == null)
            return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userRole = User.FindFirstValue(ClaimTypes.Role);

        if (userRole == UserRoles.Teacher && subject.Teachers.Any(t => t.TeacherId.ToString() == userId))
        {
            _unitOfWork.TeacherSubjectRepository.Add(new TeacherSubject
            {
                IsAdmin = false,
                TeacherId = int.Parse(userId),
                SubjectId = subject.Id
            });

            await _unitOfWork.SaveChangesAsync();
        }

        if (userRole == UserRoles.Teacher && subject.Students.Any(s => s.StudentId.ToString() == userId))
        {
            _unitOfWork.StudentSubjectRepository.Add(new StudentSubject
            {
                Grade = 0,
                StudentId = int.Parse(userId),
                SubjectId = subject.Id
            });

            await _unitOfWork.SaveChangesAsync();
        }

        return BadRequest();
    }
}