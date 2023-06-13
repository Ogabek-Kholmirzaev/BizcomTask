using BizcomTask.Data;
using BizcomTask.Entities;
using BizcomTask.Repository.IRepository;

namespace BizcomTask.Repository;

public class TeacherSubjectRepository : Repository<TeacherSubject>, ITeacherSubjectRepository
{
    public TeacherSubjectRepository(AppDbContext context) : base(context)
    {
    }
}