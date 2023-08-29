using Core.Entities;
using Core.Entities.Views;
using Core.EntityModels;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface ICCDFImplementationQueryRepository : IQueryRepository <View_GetCCFImplementation>
    {
        long Insert(CCFInformationModels _CCFInformationModels);
        Task<long> InsertDetail(CCFDImplementationDetails cCFDImplementation);
        Task<long> UpdateDetail(CCFDImplementationDetails cCFDImplementation);
        Task<IReadOnlyList<View_GetCCFImplementation>> GetAllAsync();
        Task<IReadOnlyList<View_GetCCFImplementation>> GetAllSaveAsync(long id);
    }
}
