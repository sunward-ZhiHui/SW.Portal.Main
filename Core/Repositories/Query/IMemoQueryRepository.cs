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
    public interface IMemoQueryRepository
    {
        //Custom operation which is not generic
        Task<IReadOnlyList<Memo>> GetAllByAsync();
        Task<Memo> DeleteMemo(Memo memo);
        Task<Memo> InsertOrUpdateMemo(Memo memo);
        Task<MemoUser> UpdateMemoUserAcknowledgement(long? MemoUserId, bool? IsAcknowledgement);
        Task<IReadOnlyList<Memo>> GetAllByUserAsync(long? userId);
        Task<IReadOnlyList<MemoUser>> GetMemoUserByMemoIdync(long? MemoId);
        Task<Memo> InsertCloneMemo(Memo memo);
    }
}
