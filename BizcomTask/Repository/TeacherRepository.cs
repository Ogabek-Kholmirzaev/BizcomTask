using BizcomTask.Data;
using BizcomTask.Entities;
using BizcomTask.Repository.IRepository;

namespace BizcomTask.Repository;

public class TeacherRepository : Repository<Teacher>, ITeacherRepository
{
    public TeacherRepository(AppDbContext context) : base(context)
    {
    }
}