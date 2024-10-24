using Core.Repositories.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Query
{
    public class PageContext : IPageContext
    {
        public string? PageName { get; set; }
    }
}
