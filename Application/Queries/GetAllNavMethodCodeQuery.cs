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
    public class GetAllNavMethodCodeQuery : PagedRequest, IRequest<List<NavMethodCodeModel>>
    {
        public string? SearchString { get; set; }
    }

    public class InsertOrUpdateNavMethodCode : NavMethodCodeModel, IRequest<NavMethodCodeModel>
    {

    }
    public class DeleteNavMethodCode : NavMethodCodeModel, IRequest<NavMethodCodeModel>
    {
        public NavMethodCodeModel NavMethodCodeModel { get; private set; }
        public DeleteNavMethodCode(NavMethodCodeModel navMethodCodeModel)
        {
            this.NavMethodCodeModel = navMethodCodeModel;
        }
    }
}
