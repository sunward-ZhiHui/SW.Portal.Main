using Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Command.Base
{
    public interface ICommandRepository<T> where T : class
    {
        Task<T> GetByUsers(string Name);
         Task<T> AddwithValidateAsync(T entity);
        Task<int?> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);        
    }
}
