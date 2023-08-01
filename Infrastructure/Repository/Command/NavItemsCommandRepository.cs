using Core.Entities;
using Core.Repositories.Command;
using Infrastructure.Data;
using Infrastructure.Repository.Command.Base;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repository.Command
{
    public class NavItemsCommandRepository : CommandRepository<Navitems>, INavItemsCommandRepository
    {
        public NavItemsCommandRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
    }
    public class NavProductionInformationCommandRepository : CommandRepository<NavProductionInformation>, INavProductionInformationCommandRepository
    {
        public NavProductionInformationCommandRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
    }
    public class NavCrossReferenceCommandRepository : CommandRepository<NavCrossReference>, INavCrossReferenceCommandRepository
    {
        public NavCrossReferenceCommandRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
    }
}
