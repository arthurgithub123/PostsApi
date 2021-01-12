using Microsoft.EntityFrameworkCore;
using PostsApi.Context;
using PostsApi.Models.Entities;
using System;
using System.Linq;

namespace PostsApi.Repositories.Generic
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseModel
    {
        public GenericRepository(PostsContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        private readonly PostsContext _context;
        private readonly DbSet<T> _dbSet;

        public T Create(T genericType)
        {
            _dbSet.Add(genericType);
            _context.SaveChanges();

            return genericType;
        }

        public IQueryable<T> GetAll()
        {
            return _dbSet.AsQueryable();
        }

        public T GetById(Guid id)
        {
            return _dbSet.SingleOrDefault(genericType => genericType.Id.Equals(id));
        }

        public T Update(T genericType)
        {
            var result = _dbSet.SingleOrDefault(generic => generic.Id.Equals(genericType.Id));

            if (result != null)
            {
                _context.Entry(result).CurrentValues.SetValues(genericType);
                _context.SaveChanges();

                return result;
            }
            else
            {
                return null;
            }
        }

        public bool Delete(Guid id)
        {
            var result = _dbSet.SingleOrDefault(genericType => genericType.Id.Equals(id));

            if (result != null)
            {
                _dbSet.Remove(result);
                _context.SaveChanges();

                return true;
            }

            return false;
        }
    }
}
