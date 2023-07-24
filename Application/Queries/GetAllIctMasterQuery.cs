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
    public class GetAllIctMasterQuery : PagedRequest, IRequest<List<ViewIctmaster>>
    {
        public string SearchString { get; set; }
    }
    public class GetByIctMasterSiteQuery : PagedRequest, IRequest<List<ViewIctmaster>>
    {
        public int MasterType { get; private set; }

        public GetByIctMasterSiteQuery(int MasterType)
        {
            this.MasterType = MasterType;
        }
    }
    public class GetAllMasterTypeQuery : IRequest<List<CodeMaster>>
    {
        public string MasterType { get; private set; }

        public GetAllMasterTypeQuery(string name)
        {
            this.MasterType = name;
        }
    }
    public class GetAllCodQuery : IRequest<List<CodeMaster>>
    {

    }
}
