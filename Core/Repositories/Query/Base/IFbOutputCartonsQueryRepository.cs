using Core.Entities;
using Core.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query.Base
{
    public interface IFbOutputCartonsQueryRepository
    {
        Task<IReadOnlyList<FbOutputCartons>> GetAllAsync();
        Task<long> Insert(FbOutputCartons fbOutputCartons);
       Task<long> Update(FbOutputCartons fbOutputCartons);
       Task<long> Delete(long id);
    }
}
