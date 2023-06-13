using System.Linq.Expressions;
using BizcomTask.Data;
using BizcomTask.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace BizcomTask.Repository;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _context;

    public Repository(AppDbContext context)
    {
        _context = context;
    }

    public IEnumerable<T> GetAll()
    {
        return _context.Set<T>().ToList();
    }

    public T? Get(Expression<Func<T, bool>> filter)
    {
        return _context.Set<T>().FirstOrDefault(filter);
    }

    public void Add(T entity)
    {
        _context.Set<T>().Add(entity);
    }

    public void Update(T entity)
    {
        _context.Set<T>().Update(entity);
    }

    public void Remove(T entity)
    {
        _context.Set<T>().Remove(entity);
    }

    public void RemoveRange(IEnumerable<T> entity)
    {
        _context.Set<T>().RemoveRange(entity);
    }
}