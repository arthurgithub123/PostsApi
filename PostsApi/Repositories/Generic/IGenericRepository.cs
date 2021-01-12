using PostsApi.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostsApi.Repositories.Generic
{
    public interface IGenericRepository<T> where T : BaseModel
    {
        T Create(T genericType);
        T GetById(Guid id);
        IQueryable<T> GetAll();
        T Update(T genericType);
        bool Delete(Guid id);
    }
}
