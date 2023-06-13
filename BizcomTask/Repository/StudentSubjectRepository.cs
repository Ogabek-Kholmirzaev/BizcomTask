using BizcomTask.Data;
using BizcomTask.Entities;
using BizcomTask.Repository.IRepository;

namespace BizcomTask.Repository;

public class StudentSubjectRepository : Repository<StudentSubject>, IStudentSubjectRepository
{
    public StudentSubjectRepository(AppDbContext context) : base(context)
    {
    }
}