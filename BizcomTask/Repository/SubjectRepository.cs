using BizcomTask.Data;
using BizcomTask.Entities;
using BizcomTask.Repository.IRepository;

namespace BizcomTask.Repository;

public class SubjectRepository : Repository<Subject>, ISubjectRepository
{
    public SubjectRepository(AppDbContext context) : base(context)
    {
    }
}