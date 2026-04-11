using QLSanBong.Domain.Entities.Base;
using System.Linq.Expressions;

namespace QLSanBong.Domain.Interfaces;

public interface IGenericRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();

    // Trả về IQueryable để hỗ trợ Filter, Paging ở tầng Service (chưa execute SQL)
    IQueryable<T> GetAllQueryable();

    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> expression);

    Task AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entities);

    void Update(T entity);
    void Delete(T entity);
    void DeleteRange(IEnumerable<T> entities);
}