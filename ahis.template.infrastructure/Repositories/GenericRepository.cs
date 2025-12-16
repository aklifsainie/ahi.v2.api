using ahis.template.domain.Interfaces.Repositories;
using ahis.template.infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ahis.template.infrastructure.Repositories
{
    public class GenericRepository<E> : IGenericRepository<E> where E : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<E> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<E>();
        }

        public async Task<List<E>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<E> AddAsync(E entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
