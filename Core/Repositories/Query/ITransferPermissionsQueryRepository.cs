
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
    public interface ITransferPermissionsQueryRepository : IQueryRepository<DocumentUserRoleModel>
    {
        Task<IReadOnlyList<UserGroupUser>> GetUserGroupUsers(long? userIds);
        Task<List<long?>> DeleteTransferPermissionsUserGroupUser(List<long?> ids);
        Task<List<long?>> UpdateTransferPermissionsUserGroupUser(List<long?> ids, long? Id);
        Task<IReadOnlyList<DocumentUserRoleModel>> GetTransferPermissionDocumentUserRoleList(long? Id);
        Task<List<long?>> DeleteTransferPermissionsDocumentUserRoleUser(List<long?> ids);
        Task<List<long?>> UpdateTransferPermissionsDocumentUserRoleUser(List<long?> ids, long? Id);

    }
}
