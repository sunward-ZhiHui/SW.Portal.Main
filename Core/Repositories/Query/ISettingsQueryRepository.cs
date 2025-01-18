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
    public interface ISettingsQueryRepository : IQueryRepository<OpenAccessUser>
    {
        //Custom operation which is not generic
        Task<IReadOnlyList<OpenAccessUserLink>> GetAllAsync(long? OpenAccessUserId);
        Task<OpenAccessUserLink> DeleteOpenAccessUserLink(OpenAccessUserLink value);
        Task<OpenAccessUserLink> InsertOpenAccessUserLink(OpenAccessUserLink value);
        Task<OpenAccessUser> GetAllByAsync(string? AccessType);
        Task<OpenAccessUserLink> GetDMSAccessByUser(long? UserID);
        Task<OpenAccessUserLink> GetEmailAccessByUser(long? UserID);
        Task<OpenAccessUserLink> GetNotifyPAAccessByUser(long? UserID);
        Task<OpenAccessUserLink> GetEmailOtherTagAccessByUser(long? UserID);
        Task<IReadOnlyList<OpenAccessUserLink>> GetDocumentAccessTypeEmptyAsync(string? accessType);
    }
}
