using Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetRoleByIdQuery : IRequest<ApplicationRole>
    {
        public Int64 Id { get; private set; }

        public GetRoleByIdQuery(Int64 Id)
        {
            this.Id = Id;
        }
    }
}
