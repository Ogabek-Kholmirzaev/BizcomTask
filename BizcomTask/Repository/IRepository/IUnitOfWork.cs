namespace BizcomTask.Repository.IRepository;

public interface IUnitOfWork
{
    IStudentRepository StudentRepository { get; }
    ITeacherRepository TeacherRepository { get; }
    ISubjectRepository SubjectRepository { get; }
    IStudentSubjectRepository StudentSubjectRepository { get; }
    ITeacherSubjectRepository TeacherSubjectRepository { get; }

    Task SaveChangesAsync();
}