using Core.Entities;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    //public interface ILocalStorageService : IQueryRepository<ApplicationUser>
    public interface ILocalStorageService<T> where T : class
    {
        Task<U?> GetItem<U>(string key);
        Task SetItem<U>(string key, U value);
        Task RemoveItem(string key);

        //Task<string> GetItemOne<T>(string key);
        Task<string> GetItemOne(string key);
        //Task SetItem(object userKey, ApplicationUser newEntity);
    }
}
