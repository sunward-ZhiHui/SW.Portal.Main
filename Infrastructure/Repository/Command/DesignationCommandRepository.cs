using Core.Entities;
using Core.Repositories.Command;
using Infrastructure.Data;
using Infrastructure.Repository.Command.Base;
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
    public class DesignationCommandRepository : CommandRepository<Designation>, IDesignationCommandRepository
    {
        public DesignationCommandRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<Designation> InsertAsync(Designation designation)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    await connection.InsertAsync(designation);
                }
                return designation;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<Designation> UpdateAsync(Designation designation)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    await connection.UpdateAsync(designation);
                }
                return designation;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<Designation> DeleteAsync(Designation designation)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    await connection.DeleteAsync(designation);
                }
                return designation;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}

