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
    public interface IIpirAppQueryRepostitory
    {
        Task<IReadOnlyList<IpirApp>> GetAllByAsync();
        Task<IpirApp> InsertOrUpdateIpirApp(IpirApp value);
        Task<IpirApp> DeleteIpirApp(IpirApp value);
        Task<IpirApp> GetAllByOneAsync(Guid? SessionId);

        Task<IReadOnlyList<IpirAppCheckedDetailsModel>> GetIpirAppDetails(long? value);
        Task<IpirAppCheckedDetailsModel> InsertIpirAppCheckedDetails(IpirAppCheckedDetailsModel value);

        Task<IpirAppCheckedDetailsModel> DeleteIpirAppCheckedDetails(IpirAppCheckedDetailsModel value);
    }
}
