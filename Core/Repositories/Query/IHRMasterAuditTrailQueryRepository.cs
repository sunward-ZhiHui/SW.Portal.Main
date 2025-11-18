using Core.Entities;
using Core.Entities.Views;
using Core.EntityModels;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Google.Rpc.Context.AttributeContext.Types;

namespace Core.Repositories.Query
{
    public interface IHRMasterAuditTrailQueryRepository
    {
        //Custom operation which is not generic
        Task InsertHRMasterAuditTrail(string? Type, string? FormType, string? PreValue, string? CurrentValue, long? HRMasterSetID, Guid? SessionId, long? AuditUserId, DateTime? AuditDate, bool? IsDeleted, string? columnName, Guid? UniqueSessionId = null);
        Task<IReadOnlyList<HRMasterAuditTrail>> GetHRMasterAuditList(string? MasterType, long? MasterId, bool? IsDeleted, Guid? SessionId, string? AddTypeId = "");
        Task<IReadOnlyList<FileProfileTypeModel>> GetHRMasterSWAuditList(string? MasterType, bool? IsDeleted);
    }
}
