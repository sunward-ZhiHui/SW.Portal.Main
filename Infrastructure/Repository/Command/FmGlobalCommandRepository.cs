using Core.Entities;
using Core.Repositories.Command;
using Infrastructure.Data;
using Infrastructure.Repository.Command.Base;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Command
{
    public class FmGlobalCommandRepository : CommandRepository<Fmglobal>, IFmGlobalCommandRepository
    {
        public FmGlobalCommandRepository(IConfiguration configuration)
            : base(configuration)
        {

        }

    }
    public class FmGlobalLineCommandRepository : CommandRepository<FmglobalLine>, IFmGlobalLineCommandRepository
    {
        public FmGlobalLineCommandRepository(IConfiguration configuration)
            : base(configuration)
        {

        }

    }
    public class FmGlobalLineItemCommandRepository : CommandRepository<FmglobalLineItem>, IFmGlobalLineItemCommandRepository
    {
        public FmGlobalLineItemCommandRepository(IConfiguration configuration)
            : base(configuration)
        {

        }

    }
    public class FmGlobalMoveCommandRepository : CommandRepository<FmglobalMove>, IFmGlobalMoveCommandRepository
    {
        public FmGlobalMoveCommandRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
    }
}

