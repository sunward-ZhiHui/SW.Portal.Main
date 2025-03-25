using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query.Base
{
    public interface IQueryRepository<T> where T : class
    {

        Task<T> GetAsync(int id);
        Task<IEnumerable<T>> GetListAsync();
      
        Task<IEnumerable<T>> GetListAsync(string query);
        Task<IEnumerable<T>> GetListPaggedAsync(int pageNo, int pageSize, string condition, string orderby);
        void Add(T entity);
        Task<long> InsertQuery(T entity);
        Task<long> UpdateGenericAsync(T entity, List<string> columnsToUpdate);
    }
}
