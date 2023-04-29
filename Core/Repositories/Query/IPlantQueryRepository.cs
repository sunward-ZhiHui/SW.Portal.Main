
using Core.Entities.Views;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface IPlantQueryRepository : IQueryRepository<ViewPlants>
    {
        Task<IReadOnlyList<ViewPlants>> GetAllAsync();
        Task<ViewPlants> GetByIdAsync(Int64 id);
    }
}
