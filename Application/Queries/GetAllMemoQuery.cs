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
}
