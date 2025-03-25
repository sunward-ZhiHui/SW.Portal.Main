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
    public interface IAuditLogQueryRepository
    {
        Task<IReadOnlyList<AuditLog>> GetAuditLog(AuditLog auditLog);
        Task<AuditLog> InsertAuditLog(AuditLog auditLog);
        Task<object?> GetDataSourceOldData(string? tableName, string? PrimaryKeyName, long? Id);
    }
}
