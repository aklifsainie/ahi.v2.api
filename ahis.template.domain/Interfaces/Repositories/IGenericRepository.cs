using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ahis.template.domain.Interfaces.Repositories
{
    public interface IGenericRepository<E> where E : class
    {
        Task<List<E>> GetAllAsync();
        Task<E> AddAsync(E entity);
    }
}
