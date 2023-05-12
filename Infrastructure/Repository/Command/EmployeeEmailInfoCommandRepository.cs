using Core.Entities;
using Core.Repositories.Command;
using Infrastructure.Data;
using Infrastructure.Repository.Command.Base;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repository.Command
{
    public class EmployeeEmailInfoCommandRepository : CommandRepository<EmployeeEmailInfo>, IEmployeeEmailInfoCommandRepository
    {
        public EmployeeEmailInfoCommandRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
    }
    public class EmployeeEmailInfoForwardCommandRepository : CommandRepository<EmployeeEmailInfoForward>, IEmployeeEmailInfoForwardCommandRepository
    {
        public EmployeeEmailInfoForwardCommandRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
    }
    public class EmployeeEmailInfoAuthorityCommandRepository : CommandRepository<EmployeeEmailInfoAuthority>, IEmployeeEmailInfoAuthorityCommandRepository
    {
        public EmployeeEmailInfoAuthorityCommandRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
    }
    public class EmployeeICTInformationCommandRepository : CommandRepository<EmployeeICTInformation>, IEmployeeICTInformationCommandRepository
    {
        public EmployeeICTInformationCommandRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
    }
    public class EmployeeICTHardInformationCommandRepository : CommandRepository<EmployeeICTHardInformation>, IEmployeeICTHardInformationCommandRepository
    {
        public EmployeeICTHardInformationCommandRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
    }
}
