using Application.Queries.Base;
using Application.Response;
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
    public class GetAllImplementationQuery : PagedRequest, IRequest<List<View_GetCCFImplementation>>
    {
    }
    public class EditImplementationQuery : CCFDImplementationDetails, IRequest<long>
    {
    }
    public class SaveImplementationQuery : CCFDImplementationDetails, IRequest<long>
    {

    }

    public class CreateCCFInformationModels : CCFInformationModels, IRequest<long>
    {
    }
    public class GetAllImplementationSaveQuery : PagedRequest, IRequest<List<View_GetCCFImplementation>>
    {
        public long ID { get; set; }

        public GetAllImplementationSaveQuery(long Id)
        {
            this.ID = Id;
        }
    }
    
}
