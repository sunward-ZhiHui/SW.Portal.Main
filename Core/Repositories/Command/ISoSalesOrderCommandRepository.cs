using Core.Entities;
using Core.Entities.Views;
using Core.Repositories.Command.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Command
{
    public interface ISoSalesOrderCommandRepository : ICommandRepository<SoSalesOrder>
    {
    }
}
