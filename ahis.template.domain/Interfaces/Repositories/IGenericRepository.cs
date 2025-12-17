using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ahis.template.domain.Interfaces.Repositories
{
    public interface IGenericRepository<E> where E : class
    {
        Task<List<E>> GetAllAsync();
        Task<E> AddAsync(E entity);

        //IUnitOfWork UnitOfWork { get; }

        //E Add(E entity);

        //E Update(E entity);

        //void Delete(E entity);

        //Task<List<E>> GetAllAsync(List<Expression<Func<E, object>>> includes = null!, CancellationToken cancellationToken = default);

        //Task<E> GetByIdAsync(Guid id, List<Expression<Func<E, object>>> includes = null!, CancellationToken cancellationToken = default);

        //Task<List<E>> FindMatchesAsync(Specification<E> specification, List<Expression<Func<E, object>>> includes = null!, CancellationToken cancellationToken = default);

        //Task<List<E>> FindMatchesAsync(Specification<E> specification, int pageSize, int pageIndex, List<Expression<Func<E, object>>> includes = null!, CancellationToken cancellationToken = default);

        //Task<E?> FindFirstAsync(Specification<E> specification, List<Expression<Func<E, object>>> includes = null!, CancellationToken cancellationToken = default);

        //Task<bool> AnyAsync(Specification<E> specification, CancellationToken cancellationToken = default);

        //Task<int> CountAsync(Specification<E> specification, CancellationToken cancellationToken = default);
    }
}
