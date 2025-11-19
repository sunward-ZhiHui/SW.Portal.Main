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
    public interface ILoginSessionHistoryQueryRepository
    {
        //Custom operation which is not generic
        Task<IReadOnlyList<LoginSessionHistory>> GetAllByAsync();
        Task<LoginSessionHistory> InsertLoginSessionHistory(LoginSessionHistory loginSessionHistory);
        Task<LoginSessionHistory> UpdateLastActivity(LoginSessionHistory loginSessionHistory);
        Task<LoginSessionHistory> UpdateLogOutActivity(LoginSessionHistory loginSessionHistory);
        Task<LoginSessionHistory> GetLoginSessionHistoryOne(Guid? SessionId, long? UserId);
    }
}
