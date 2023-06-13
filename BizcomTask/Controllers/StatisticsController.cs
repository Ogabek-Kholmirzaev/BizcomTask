using BizcomTask.DTOs;
using BizcomTask.DTOs.Student;
using BizcomTask.DTOs.StudentSubject;
using BizcomTask.DTOs.Subject;
using BizcomTask.DTOs.Teacher;
using BizcomTask.DTOs.TeacherSubject;
using BizcomTask.Mappers;
using BizcomTask.Repository.IRepository;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace BizcomTask.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StatisticsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public StatisticsController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet("students-under-20yo")]
    [ProducesResponseType(typeof(List<StudentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStudentsUnder20()
    {
        var currentYear = DateTime.UtcNow.Year;
        var studentsUnder20 = _unitOfWork.StudentRepository.GetAll()
            .Where(s => currentYear - s.BirthDate.Year < 20).ToList();

        return Ok(studentsUnder20.Adapt<List<StudentDto>>());
    }

    [HttpGet("students-birthday-between-12Aug-18Sep")]
    [ProducesResponseType(typeof(List<StudentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStudentsBirthdayBetween12Aug18Sep()
    {
        var august = 8;
        var augustDay = 12;
        var september = 9;
        var septemberDay = 18;

        var studentsInRange = _unitOfWork.StudentRepository.GetAll()
            .Where(s => (s.BirthDate.Month >= august && s.BirthDate.Month <= september)
                        && (s.BirthDate.Day >= augustDay && s.BirthDate.Day <= septemberDay)).ToList();

        return Ok(studentsInRange.Adapt<List<StudentDto>>());
    }

    [HttpGet("users-beeline-number")]
    [ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBeelineUsers()
    {
        var users = new List<UserDto>();
        var students = _unitOfWork.StudentRepository.GetAll()
            .Where(s => s.PhoneNumber[5] == '0' || s.PhoneNumber[5] == '1').ToList();

        var teachers = _unitOfWork.TeacherRepository.GetAll()
            .Where(t => t.PhoneNumber[5] == '0' || t.PhoneNumber[5] == '1');

        users.AddRange(students.Adapt<List<UserDto>>());
        users.AddRange(teachers.Adapt<List<UserDto>>());

        return Ok(users);
    }

    [HttpGet("students-filter-phrase")]
    [ProducesResponseType(typeof(StudentDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStudentsFilterByPhrase(string phrase)
    {
        var studentsFilter = _unitOfWork.StudentRepository.GetAll()
            .Where(s => s.FirstName.Contains(phrase) || s.LastName.Contains(phrase));

        return Ok(studentsFilter.Adapt<StudentDto>());
    }

    [HttpGet("student-high-score-subject/{userId}")]
    [ProducesResponseType(typeof(StudentSubjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserSubjectMaxScore(int userId)
    {
        var student = _unitOfWork.StudentRepository.Get(s => s.Id == userId);

        if (student == null)
            return NotFound();

        if (student.Subjects.Count == 0)
            return NotFound();

        var subjects = student.Subjects.OrderByDescending(s => s.Grade).ToList();
        var subjectDto = new StudentSubjectDto
        {
            Id = subjects[0].Id,
            Grade = subjects[0].Grade,
            StudentId = subjects[0].StudentId,
            SubjectId = subjects[0].Id,
            SubjectName = subjects[0].Subject.Name
        };

        return Ok(subjectDto);
    }

    [HttpGet("teacher/{teacherId}/subject-students-score>80-and-count<10")]
    [ProducesResponseType(typeof(TeacherSubjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTeacherSubjectStudentScoreOver80AndCountOver10(int teacherId)
    {
        var teacher = _unitOfWork.TeacherRepository.Get(t => t.Id == teacherId);

        if (teacher == null)
            return NotFound();

        foreach (var subject in teacher.Subjects)
        {
            var students = subject.Subject.Students.Where(s=>s.Grade > 80).ToList();

            if(students.Count < 10)
                continue;

            var subjectDto = new TeacherSubjectDto
            {
                Id = subject.Id,
                SubjectId = subject.SubjectId,
                IsAdmin = subject.IsAdmin,
                TeacherId = subject.TeacherId,
                SubjectName = subject.Subject.Name
            };

            return Ok(subjectDto);
        }

        return NotFound();
    }

    [HttpGet("teachers-student-score>97")]
    [ProducesResponseType(typeof(List<TeacherDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTeachersByStudentsHighScoreOver97()
    {
        var teachersDto = new List<TeacherDto>();
        var teachers = _unitOfWork.TeacherRepository.GetAll();

        foreach (var teacher in teachers)
        {
            foreach (var subject in teacher.Subjects)
            {
                var isTrue = subject.Subject.Students.Any(s => s.Grade > 97);

                if (isTrue)
                {
                    teachersDto.Add(teacher.Adapt<TeacherDto>());
                    break;
                }
            }
        }

        return Ok(teachersDto);
    }

    [HttpGet("subject-highest-average-score")]
    [ProducesResponseType(typeof(SubjectDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSubjectHighestAverageScore()
    {
        var subjectDto = new SubjectDto();
        double averageScore = -1;

        var subjects = _unitOfWork.SubjectRepository.GetAll().ToList();

        foreach (var subject in subjects)
        {
            var overAllScore = subject.Students.Sum(s => s.Grade);
            var studentsCount = subject.Students.Count();
            var average = (double)overAllScore / studentsCount;

            if (average > averageScore)
            {
                subjectDto = subject.ToDto();
                averageScore = average;
            }
        }

        return Ok(subjectDto);
    }
}