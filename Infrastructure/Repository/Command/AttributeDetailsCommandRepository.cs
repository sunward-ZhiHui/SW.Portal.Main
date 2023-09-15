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
    public class AttributeDetailsCommandRepository :  CommandRepository<AttributeDetails>, IAttributeDetailsCommandRepository
    {
        public AttributeDetailsCommandRepository(IConfiguration configuration)
        : base(configuration)
        {

        }
        public async Task<AttributeDetails> InsertAsync(AttributeDetails attributeDetails)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    await connection.InsertAsync(attributeDetails);
                }
                return attributeDetails;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<AttributeDetails> UpdateAsync(AttributeDetails attributeDetails)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    await connection.UpdateAsync(attributeDetails);
                }
                return attributeDetails;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<AttributeDetails> DeleteAsync(AttributeDetails attributeDetails)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    await connection.DeleteAsync(attributeDetails);
                }
                return attributeDetails;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}
