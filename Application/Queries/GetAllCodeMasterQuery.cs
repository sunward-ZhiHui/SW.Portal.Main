using Application.Queries.Base;
using Core.Entities;
using Core.EntityModel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetAllCodeMasterQuery : IRequest<List<CodeMaster>>
    {
        public string Name { get; private set; }

        public GetAllCodeMasterQuery(string name)
        {
            this.Name = name;
        }
    }
    public class GetAllCodQuery : IRequest<List<CodeMaster>>
    {
        
    }
}
