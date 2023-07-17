
using Core.Entities;
using Core.Entities.Views;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface INavItemsQueryRepository : IQueryRepository<View_NavItems>
    {
        Task<IReadOnlyList<View_NavItems>> GetAllAsync();
        Task<IReadOnlyList<View_NavItems>> GetAsyncList();
        Task<View_NavItems> GetByItemSerialNoAsync(string ItemSerialNo);
        Task<long> Update(View_NavItems todolist);
        Task<View_NavItems> GetByItemSerialNoExitsAsync(View_NavItems ItemSerialNo);
    }
}
