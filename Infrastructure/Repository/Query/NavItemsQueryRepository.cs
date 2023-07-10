using Core.Repositories.Query;
using Infrastructure.Repository.Query.Base;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.EntityModel;
using Core.Entities.Views;
using Core.Entities;

namespace Infrastructure.Repository.Query
{
    public class NavItemsQueryRepository : QueryRepository<Navitems>, INavItemsQueryRepository
    {
        public NavItemsQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<IReadOnlyList<Navitems>> GetAllAsync()
        {
            try
            {
                var query = "select  * from Navitems";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<Navitems>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<Navitems> GetByItemSerialNoAsync(string ItemSerialNo)
        {
            try
            {
                var query = "SELECT * FROM Navitems WHERE ItemSerialNo!=null and ItemSerialNo =" + ItemSerialNo;
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<Navitems>(query));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

    }
}
