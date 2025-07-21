using Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetSPCDataTrendingList : IRequest<List<SPCDataTrending>>
    {
        
    }
    public class CreateSPCDataTrendingQuery : IRequest<SPCDataTrending>
    {
        public SPCDataTrending? SPCDataTrending { get; set; }
    }
    public class UpdateSPCDataTrendingQuery : IRequest<SPCDataTrending>
    {
        public SPCDataTrending? DataItem { get; set; }
        public SPCDataTrending? ChangedItem { get; set; }
    }

    public class DeleteSPCDataTrendingQuery : IRequest<bool>
    {
        public long ID { get; set; }
    }
}
