using Core.Entities;
using Core.Repositories.Command;
using Infrastructure.Data;
using Infrastructure.Repository.Command.Base;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repository.Command
{
    public class EmployeeOtherDutyInformationCommandRepository : CommandRepository<EmployeeOtherDutyInformation>, IEmployeeOtherDutyInformationCommandRepository
    {
        public EmployeeOtherDutyInformationCommandRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<EmployeeOtherDutyInformation> InsertAsync(EmployeeOtherDutyInformation applicationRole)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    await connection.InsertAsync(applicationRole);
                }
                return applicationRole;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}
