using Application.Queries.Base;
using Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetAllSamplingQuery  : PagedRequest, IRequest<List<AppSampling>>
    {
       // public int Id { get; private set; }

        //public GetAllSamplingQuery(int id)
        //{
        //    this.Id = id;
        //}
    }
    public class GetAllSamplingLineQuery : PagedRequest, IRequest<List<AppSamplingLine>>
    {

    }
}
