using System.Security.Claims;
using BizcomTask.DTOs.StudentSubject;
using BizcomTask.Repository.IRepository;
using BizcomTask.Statics;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BizcomTask.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GradesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public GradesController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpPost]
    [Authorize(Roles = UserRoles.Teacher)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> MarkStudentSubject(int subjectId, int studentId, int grade)
    {
        var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var subject = _unitOfWork.SubjectRepository.Get(s => s.Id == subjectId);

        if (subject == null)
            return NotFound();

        if (!subject.Teachers.Any(t => t.TeacherId.ToString() == teacherId))
            return Forbid();

        if (!subject.Students.Any(s => s.StudentId == studentId))
            return NotFound();

        var studentSubject = _unitOfWork.StudentSubjectRepository
            .Get(s => s.StudentId == studentId);

        if (studentSubject == null)
            return NotFound();

        studentSubject.Grade = grade;
        await _unitOfWork.SaveChangesAsync();

        return Ok();
    }
}