using BizcomTask.Data;
using BizcomTask.Entities;
using BizcomTask.Repository.IRepository;

namespace BizcomTask.Repository;

public class StudentRepository : Repository<Student>, IStudentRepository
{
    public StudentRepository(AppDbContext context) : base(context)
    {
    }
}