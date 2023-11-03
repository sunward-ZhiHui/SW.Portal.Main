using Application.Queries.Base;
using Core.Entities;
using Core.Entities.Views;
using Core.EntityModel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetAllApplicationMasterDetailQuery : IRequest<List<View_ApplicationMasterDetail>>
    {
        public long? Id { get; private set; }

        public GetAllApplicationMasterDetailQuery(long? Id)
        {
            this.Id = Id;
        }
    }
    public class GetAllApplicationMasterQuery : IRequest<List<ApplicationMaster>>
    {
        public string? SearchString { get; private set; }

    }
    public class GetAllApplicationMasterresultQuery : IRequest<List<ApplicationMasterDetail>>
    {
        public string? SearchString { get; private set; }

    }
}
