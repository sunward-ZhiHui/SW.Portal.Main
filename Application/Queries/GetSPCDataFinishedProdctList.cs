using Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetSPCDataFinishedProdctList : IRequest<List<SPCDataFinishedProdct>>
    {
        
    }
    public class CreateSPCDataFinishedProdctQuery : IRequest<SPCDataFinishedProdct>
    {
        public SPCDataFinishedProdct? SPCDataFinishedProdct { get; set; }
    }
    public class UpdateSPCDataFinishedProdctQuery : IRequest<SPCDataFinishedProdct>
    {
        public SPCDataFinishedProdct? DataItem { get; set; }
        public SPCDataFinishedProdct? ChangedItem { get; set; }
    }

    public class DeleteSPCDataFinishedProdctQuery : IRequest<bool>
    {
        public long ID { get; set; }
    }
}
