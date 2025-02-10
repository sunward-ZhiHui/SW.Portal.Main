using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface IIncidentAppSettingsQueryRepository
    {
        Task<IReadOnlyList<IncidentAppSettings>> GetAllAsync();
        Task<long> Insert(IncidentAppSettings IncidentAppSettings);
        Task<long> Update(IncidentAppSettings IncidentAppSettings);
    }
}
