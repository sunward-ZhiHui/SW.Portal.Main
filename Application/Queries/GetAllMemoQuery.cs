using Application.Queries.Base;
using Core.Entities;
using Core.Entities.Views;
using Core.EntityModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetAllMemoQuery : PagedRequest, IRequest<List<Memo>>
    {
        public string? SearchString { get; set; }
    }
    public class DeleteMemo : Memo, IRequest<Memo>
    {
        public Memo Memo { get; set; }
        public DeleteMemo(Memo memo)
        {
            this.Memo = memo;
        }
    }
    public class InsertOrUpdateMemo : Memo, IRequest<Memo>
    {

    }
    public class UpdateMemoUserAcknowledgement : PagedRequest, IRequest<MemoUser>
    {
        public long? MemoUserId { get; set; }
        public bool? IsAcknowledgement { get; set; }
        public UpdateMemoUserAcknowledgement(long? memoUserId, bool? isAcknowledgement)
        {
            this.MemoUserId = memoUserId;
            this.IsAcknowledgement = isAcknowledgement;
        }
    }
    public class GetAllMemoByUserQuery : PagedRequest, IRequest<List<Memo>>
    {
        public long? UserId { get; set; }
        public GetAllMemoByUserQuery(long? userId)
        {
            this.UserId = userId;
        }
    }

    public class GetAllMemoByMemoIdQuery : PagedRequest, IRequest<List<MemoUser>>
    {
        public long? MemoId { get; set; }
        public GetAllMemoByMemoIdQuery(long? memoId)
        {
            this.MemoId = memoId;
        }
    }
    public class InsertCloneMemo : PagedRequest, IRequest<Memo>
    {
        public Memo Memo { get; set; }
        public InsertCloneMemo(Memo memo)
        {
            this.Memo = memo;
        }
    }
}
