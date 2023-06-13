using BizcomTask.Data;
using BizcomTask.Repository.IRepository;

namespace BizcomTask.Repository;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    public IStudentRepository StudentRepository { get; }
    public ITeacherRepository TeacherRepository { get; }
    public ISubjectRepository SubjectRepository { get; }
    public IStudentSubjectRepository StudentSubjectRepository { get; }
    public ITeacherSubjectRepository TeacherSubjectRepository { get; }

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        StudentRepository ??= new StudentRepository(_context);
        TeacherRepository ??= new TeacherRepository(_context);
        SubjectRepository ??= new SubjectRepository(_context);
        StudentSubjectRepository ??= new StudentSubjectRepository(_context);
        TeacherSubjectRepository ??= new TeacherSubjectRepository(_context);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}