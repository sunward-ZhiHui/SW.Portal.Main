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
        Task<IReadOnlyList<IpirApp>> GetAllListByAsync();
        Task<IReadOnlyList<IPIRReportingInformation>> GetAllIPIRmobileByAsync(long IpirappId);
        Task<IReadOnlyList<DocumentProfileNoSeries>> GetProfileType();
        Task<IpirApp> InsertOrUpdateIpirApp(IpirApp value);
        Task<IpirApp> UpdateIpirPIC(IpirApp value);
        Task<IpirApp> DeleteIpirApp(IpirApp value);
        Task<IpirApp> UpdateIpirSupervisor(IpirApp value);
        Task<IPIRReportingInformation> DeleteIpirReportingInformation(IPIRReportingInformation value);
        Task<IpirApp> GetAllByOneAsync(Guid? SessionId);
        Task<IPIRReportingInformation> InsertOrUpdateIpirReportingInformation(IPIRReportingInformation value);
        Task<IReadOnlyList<IpirAppCheckedDetailsModel>> GetIpirAppDetails(long? value);
        Task<IpirAppCheckedDetailsModel> InsertIpirAppCheckedDetails(IpirAppCheckedDetailsModel value);

        Task<IpirAppCheckedDetailsModel> DeleteIpirAppCheckedDetails(IpirAppCheckedDetailsModel value);
        Task<IpirAppIssueDep> UpdateDynamicFormDataIssueDetails(Guid? SessionId, long? ActivityInfoIssueId, long? dynamicFormDataId);
        Task<IpirAppIssueDep> GetIpirAppIssueDepByDynamicForm(long? IpirAppIssueDepId);
    }
}
