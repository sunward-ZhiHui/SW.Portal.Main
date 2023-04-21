using Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetRoleByNameQuery :IRequest<ApplicationRole>
    {
        public string Name { get; private set; }

        public GetRoleByNameQuery(string name)
        {
            this.Name = name;
        }
    }
}
