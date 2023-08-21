using Core.Entities;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface INewEmailUploadQueryRepository : IQueryRepository<Documents>
    {
        Task<IReadOnlyList<Documents>> GetAllAsync();
        Task<long> Insert(Documents Documents);
    
    }
}
