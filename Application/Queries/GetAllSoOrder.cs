using Application.Queries.Base;
using Core.Entities;
using Core.Entities.Views;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetAllSoOrder : PagedRequest, IRequest<List<View_SoSalesOrder>>
    {
        public string SearchString { get; set; }
    }
    public class GetAllSoOrderBySession : PagedRequest, IRequest<View_SoSalesOrder>
    {
        public string SearchString { get; set; }
        public Guid? SesionId { get; set; }
        public GetAllSoOrderBySession(Guid? SessionId)
        {
            this.SesionId = SessionId;
        }
    }


}
