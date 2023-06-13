using BizcomTask.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BizcomTask.DTOs;
using BizcomTask.Entities;
using BizcomTask.Statics;
using Mapster;
using Microsoft.AspNetCore.Authorization;

namespace BizcomTask.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;

    public AccountController(IUnitOfWork unitOfWork, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
    }

    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Profile()
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        var student = _unitOfWork.StudentRepository.Get(s => s.Email == email);
        var teacher = _unitOfWork.TeacherRepository.Get(t => t.Email == email);

        if (student != null)
            return Ok(student.Adapt<UserDto>());

        return Ok(teacher.Adapt<UserDto>());
    }


    [HttpPost("signup/student")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterStudent(RegisterUserDto registerUserDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (IsUserOrTeacherExist(registerUserDto.Email, registerUserDto.PhoneNumber))
            return BadRequest("Email or Phone Number already exists");

        var student = registerUserDto.Adapt<Student>();

        _unitOfWork.StudentRepository.Add(student);
        await _unitOfWork.SaveChangesAsync();

        student.Role = UserRoles.Student;
        student.StudentRegNumber = student.Id;
        student.BirthDate = registerUserDto.BirthDate.Date;
        await _unitOfWork.SaveChangesAsync();

        return Ok(GenerateToken(student.Adapt<UserDto>()));
    }

    [HttpPost("signup/teacher")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterTeacher(RegisterUserDto registerUserDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (IsUserOrTeacherExist(registerUserDto.Email, registerUserDto.PhoneNumber))
            return BadRequest("Email or Phone Number already exists");

        var teacher = registerUserDto.Adapt<Teacher>();

        teacher.Role = UserRoles.Teacher;
        teacher.BirthDate = registerUserDto.BirthDate.Date;
        _unitOfWork.TeacherRepository.Add(teacher);
        await _unitOfWork.SaveChangesAsync();

        return Ok(GenerateToken(teacher.Adapt<UserDto>()));
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Login(LoginUserDto loginUserDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!IsUserOrTeacherExist(loginUserDto.Email))
            return NotFound();

        var student = _unitOfWork.StudentRepository.Get(s => s.Email == loginUserDto.Email);

        if (student != null)
        {
            if (student.Password != loginUserDto.Password)
                return NotFound();

            return Ok(GenerateToken(student.Adapt<UserDto>()));
        }

        var teacher = _unitOfWork.TeacherRepository.Get(t => t.Email == loginUserDto.Email);

        if (teacher?.Password != loginUserDto.Password)
            return NotFound();

        return Ok(GenerateToken(loginUserDto.Adapt<UserDto>()));
    }

    private bool IsUserOrTeacherExist(string email, string? phoneNumber = null)
    {
        var student = _unitOfWork.StudentRepository
            .Get(s => s.Email == email || s.PhoneNumber == phoneNumber);

        var teacher = _unitOfWork.TeacherRepository
            .Get(t => t.Email == email || t.PhoneNumber == phoneNumber);

        if (student != null || teacher != null)
            return true;

        return false;
    }

    private string GenerateToken(UserDto userDto)
    {
        var keyByte = System.Text.Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);
        var securityKey = new SigningCredentials(new SymmetricSecurityKey(keyByte), SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, userDto.FirstName),
            new Claim(ClaimTypes.NameIdentifier, userDto.Id.ToString()),
            new Claim(ClaimTypes.Email, userDto.Email),
            new Claim(ClaimTypes.MobilePhone, userDto.PhoneNumber),
            new Claim(ClaimTypes.Role, userDto.Role)
        };

        var security = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            signingCredentials: securityKey,
            expires: DateTime.Now.AddMinutes(20)
        );

        var token = new JwtSecurityTokenHandler().WriteToken(security);

        return token;
    }
}