using Core.Entities;
using Core.Entities.Base;
using Core.Repositories.Command.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Command
{
    public  interface IFileUploadCommandRepository : ICommandRepository<BaseEntity>
    {

    }
}
