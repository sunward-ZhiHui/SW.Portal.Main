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
    public interface INAVItemLinksQueryRepository
    {
        //Custom operation which is not generic
        Task<IReadOnlyList<NAVItemLinksModel>> GetAllByAsync();
        Task<NAVItemLinksModel> DeleteNavitemLinks(NAVItemLinksModel value);
        Task<NAVItemLinksModel> InsertOrUpdateNavitemLinks(NAVItemLinksModel value);

    }
}
