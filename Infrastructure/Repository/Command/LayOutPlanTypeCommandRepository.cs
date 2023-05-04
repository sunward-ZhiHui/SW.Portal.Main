using Core.Entities;
using Core.Repositories.Command;
using Dapper;
using Infrastructure.Repository.Command.Base;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Command
{
    public class LayOutPlanTypeCommandRepository : CommandRepository<LayoutPlanType>,ILayOutPlanCommandRepository
    {
        public LayOutPlanTypeCommandRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<LayoutPlanType> InsertAsync(LayoutPlanType layoutplantype)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    await connection.InsertAsync(layoutplantype);
                }
                return layoutplantype;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
   
}
