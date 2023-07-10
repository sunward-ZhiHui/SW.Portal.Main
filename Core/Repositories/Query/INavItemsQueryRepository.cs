
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
    public interface INavItemsQueryRepository : IQueryRepository<Navitems>
    {
        Task<IReadOnlyList<Navitems>> GetAllAsync();
        Task<Navitems> GetByItemSerialNoAsync(string ItemSerialNo);
        Task<long> Update(Navitems todolist);
        Task<Navitems> GetByItemSerialNoExitsAsync(Navitems ItemSerialNo);
    }
}
