#pragma warning disable CS8618
namespace BizcomTask.Entities;

public class Student
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public DateTime BirthDate { get; set; }
    public int StudentRegNumber { get; set; }
    public string Role { get; set; }

    public virtual List<StudentSubject> Subjects { get; set; }
}