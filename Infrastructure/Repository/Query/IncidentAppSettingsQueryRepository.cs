using Core.Entities;
using Core.Repositories.Query;
using Dapper;
using Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Query
{
    public class IncidentAppSettingsQueryRepository : DbConnector, IIncidentAppSettingsQueryRepository
    {
        public IncidentAppSettingsQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }

        public async Task<IReadOnlyList<IncidentAppSettings>> GetAllAsync()
        {
            try
            {
                var query = @"SELECT FT.Name,IA.* FROM IncidentAppSettings IA
                    Left Join FileProfileType FT ON FT.FileProfileTypeID = IA.SupportingDocFileProfileID";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<IncidentAppSettings>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public  async Task<long> Insert(IncidentAppSettings IncidentAppSettings)
        {
             try
            {
                using (var connection = CreateConnection())
                {


                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("SupportingDocFileProfileID", IncidentAppSettings.SupportingDocFileProfileID);
                        

                        var query = @"INSERT INTO IncidentAppSettings(SupportingDocFileProfileID)
                         OUTPUT  INSERTED.IncidentAppSettingsID
                         VALUES (@SupportingDocFileProfileID)";
                        var insertedId = await connection.ExecuteScalarAsync<long>(query, parameters);
                        var id = insertedId;

                        return id;
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

        public async Task<long> Update(IncidentAppSettings IncidentAppSettings)
        {
            try
            {
                using (var connection = CreateConnection())
                {


                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("SupportingDocFileProfileID", IncidentAppSettings.SupportingDocFileProfileID);
                        parameters.Add("IncidentAppSettingsID", IncidentAppSettings.IncidentAppSettingsID);

                        var query = @"Update IncidentAppSettings set SupportingDocFileProfileID = @SupportingDocFileProfileID where IncidentAppSettingsID =@IncidentAppSettingsID
                        ";
                        var insertedId = await connection.ExecuteScalarAsync<long>(query, parameters);
                        var id = insertedId;

                        return IncidentAppSettings.SupportingDocFileProfileID.Value;
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
}
