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
    public class AttributeHeaderCommandRepository : CommandRepository<AttributeHeader>, IAttributeHeaderCommandRepository
    {
        public AttributeHeaderCommandRepository(IConfiguration configuration)
          : base(configuration)
        {

        }
        public async Task<AttributeHeader> InsertAsync(AttributeHeader attributeHeader)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    await connection.InsertAsync(attributeHeader);
                }
                return attributeHeader;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<AttributeHeader> UpdateAsync(AttributeHeader attributeHeader)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    await connection.UpdateAsync(attributeHeader);
                }
                return attributeHeader;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<AttributeHeader> DeleteAsync(AttributeHeader attributeHeader)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    await connection.DeleteAsync(attributeHeader);
                }
                return attributeHeader;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}
