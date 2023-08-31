using Core.Entities;
using Core.Entities.Views;
using Core.Repositories.Query;
using Dapper;
using Infrastructure.Repository.Query.Base;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Query
{
    public class ChangeControlFormQueryRepository:QueryRepository<ChangeControlForm>, IChangeControlFormQueryRepository
    {
        public ChangeControlFormQueryRepository(IConfiguration configuration) : base(configuration)
        {

        }
    

    public async Task<IReadOnlyList<ChangeControlForm>> GetAllAsync()
    {
        try
        {
            var query = "select  * from ChangeControlForm";

            using (var connection = CreateConnection())
            {
                return (await connection.QueryAsync<ChangeControlForm>(query)).ToList();
            }
        }
        catch (Exception exp)
        {
            throw new Exception(exp.Message, exp);
        }
    }

        public async  Task<long> Insert(ChangeControlForm changeControlForm)
        {
           

                try
                {
                    using (var connection = CreateConnection())
                    {

                        connection.Open();
                        using (var transaction = connection.BeginTransaction())
                        {

                            try
                            {
                                var parameters = new DynamicParameters();
                                parameters.Add("VersionNo", changeControlForm.VersionNo);
                                parameters.Add("DocNo", changeControlForm.DocNo);
                                parameters.Add("SessionId", changeControlForm.SessionId);
                                parameters.Add("AddedByUserID", changeControlForm.AddedByUserID);
                                parameters.Add("AddedDate", changeControlForm.AddedDate);
                                parameters.Add("CCNumber", changeControlForm.CCNumber);
                                parameters.Add("StatusCodeID", changeControlForm.StatusCodeID);

                                var query = "INSERT INTO ChangeControlForm(VersionNo,DocNo,SessionId,AddedByUserID,AddedDate,StatusCodeID,CCNumber) VALUES (@VersionNo,@DocNo,@SessionId,@AddedByUserID,@AddedDate,@StatusCodeID,@CCNumber)";

                                var rowsAffected = await connection.ExecuteAsync(query, parameters, transaction);

                                transaction.Commit();

                                return rowsAffected;
                            }


                            catch (Exception exp)
                            {
                                transaction.Rollback();
                                throw new Exception(exp.Message, exp);
                            }

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
