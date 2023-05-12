using Core.Entities;
using Core.Repositories.Command.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Command
{
    public interface IEmployeeOtherDutyInformationCommandRepository : ICommandRepository<EmployeeOtherDutyInformation>
    {

    }
    public interface IEmployeeICTInformationCommandRepository : ICommandRepository<EmployeeICTInformation>
    {

    }
    public interface IEmployeeICTHardInformationCommandRepository : ICommandRepository<EmployeeICTHardInformation>
    {

    }
}
