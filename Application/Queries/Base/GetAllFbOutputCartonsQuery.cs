using Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.Base
{
    public  class GetAllFbOutputCartonsQuery : PagedRequest, IRequest<List<FbOutputCartons>>
    {
    }
    public class CreateFbOutputCartonsQuery : FbOutputCartons, IRequest<long>
    {
    }
    public class GetAllDispensedMeterialQuery : PagedRequest, IRequest<List<DispensedMeterial>>
    {
    }
    public class CreateDispensedMeterialQuery : DispensedMeterial, IRequest<long>
    {
    }
    public class EditFbOutputCartonsQuery : FbOutputCartons, IRequest<long>
    {

    }
    public class DeleteFbOutputCartonsQuery : FbOutputCartons, IRequest<long>
    {
        public long ID { get; set; }

        public DeleteFbOutputCartonsQuery(long Id)
        {
            this.ID = Id;
        }
    }
}
