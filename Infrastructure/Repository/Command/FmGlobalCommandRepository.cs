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
using System.Data;

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

        public string DeleteFMGlobal(long Id)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("Id", Id, DbType.Int64);         

                        connection.Open();
                        var task = connection.ExecuteAsync("sp_Del_FMGlobal", parameters, commandType: CommandType.StoredProcedure);
                        task.Wait(); // Synchronously wait for the task to complete
                        var rowsAffected = "FM Global Line information has been deleted!"; // Retrieve the result


                        return rowsAffected;
                    }
                    catch (Exception exp)
                    {

                        throw new Exception(exp.Message, exp);
                    }

                }

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
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

