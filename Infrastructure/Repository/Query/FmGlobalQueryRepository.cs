using Core.Entities;
using Core.Entities.Views;
using Core.Repositories.Query;
using Dapper;
using Infrastructure.Repository.Query.Base;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Query
{
    public class FmGlobalQueryRepository : QueryRepository<ViewFmglobal>, IFmGlobalQueryRepository
    {
        public FmGlobalQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<IReadOnlyList<ViewFmglobal>> GetAllAsync()
        {
            try
            {
                var query = "select  * from View_Fmglobal";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ViewFmglobal>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ViewFmglobal> GetByIdAsync(long id)
        {
            try
            {
                var query = "SELECT * FROM View_Fmglobal  WHERE FmglobalId = @FmglobalId";
                var parameters = new DynamicParameters();
                parameters.Add("FmglobalId", id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<ViewFmglobal>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ViewFmglobal> GetBySessionIdAsync(Guid? SessionId)
        {
            try
            {
                var query = "SELECT * FROM View_Fmglobal  WHERE SessionId = @SessionId";
                var parameters = new DynamicParameters();
                parameters.Add("SessionId", SessionId, DbType.Guid);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<ViewFmglobal>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<View_FMGlobalAddess>> GetFmGlobalAddressdAsync(long? id, string billingType)
        {
            try
            {
                var query = "SELECT * FROM View_FMGlobalAddess  WHERE  AddressType=@AddressType AND FMGlobalID = @FMGlobalID";
                var parameters = new DynamicParameters();
                parameters.Add("FMGlobalID", id);
                parameters.Add("AddressType", billingType);
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<View_FMGlobalAddess>(query,parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<long> InsertFmAddress(View_FMGlobalAddess address)
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
                            parameters.Add("Address1", address.Address1);
                            parameters.Add("Address2", address.Address2);
                            parameters.Add("PostCode", address.PostCode);
                            parameters.Add("CountryID", address.CountryID);
                            parameters.Add("StateID", address.StateID);
                            parameters.Add("CityID", address.CityID);

                            var query = "INSERT INTO [Address](Address1,Address2,PostCode,CountryID,StateID,CityID) OUTPUT INSERTED.AddressID VALUES (@Address1,@Address2,@PostCode,@CountryID,@StateID,@CityID)";

                            var lastInsertedRecordId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters, transaction);

                            transaction.Commit();

                            return lastInsertedRecordId;
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
        public async Task<long> InsertFmGlobalAddress(FMGlobalAddess soCustomerAddress)
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
                            parameters.Add("AddressId", soCustomerAddress.AddressId);
                            parameters.Add("FmglobalId", soCustomerAddress.FmglobalId);
                            parameters.Add("AddressType", soCustomerAddress.AddressType);
                            parameters.Add("IsBilling", soCustomerAddress.isBilling, (DbType?)SqlDbType.Bit);
                            parameters.Add("IsShipping", soCustomerAddress.isShipping, (DbType?)SqlDbType.Bit);
                            var query = "INSERT INTO FMGlobalAddess(AddressId,FmglobalId,AddressType,IsBilling,IsShipping) OUTPUT INSERTED.FMGlobalAddessId VALUES (@AddressId,@FmglobalId,@AddressType,@IsBilling,@IsShipping)";

                            var lastInsertedRecordId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters, transaction);

                            transaction.Commit();

                            return lastInsertedRecordId;
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
        public async Task<long> EditAddress(View_FMGlobalAddess address)
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
                            
                            var parametersAddress = new DynamicParameters();
                            parametersAddress.Add("FMGlobalAddessId", address.FMGlobalAddessId);
                            parametersAddress.Add("IsBilling", address.isBilling, (DbType?)SqlDbType.Bit);
                            parametersAddress.Add("IsShipping", address.isShipping, (DbType?)SqlDbType.Bit);
                            var querys = "UPDATE FMGlobalAddess SET IsBilling = @IsBilling,IsShipping = @IsShipping WHERE FMGlobalAddessId = @FMGlobalAddessId";
                            await connection.QuerySingleOrDefaultAsync<long>(querys, parametersAddress, transaction);

                            var parameters = new DynamicParameters();
                            parameters.Add("AddressID", address.AddressID);
                            parameters.Add("Address1", address.Address1);
                            parameters.Add("Address2", address.Address2);
                            parameters.Add("PostCode", address.PostCode);
                            parameters.Add("CountryID", address.CountryID);
                            parameters.Add("StateID", address.StateID);
                            parameters.Add("CityID", address.CityID);


                            var query = "UPDATE Address SET Address1 = @Address1,Address2 = @Address2,PostCode = @PostCode,CountryID=@CountryID,StateID= @StateID,CityID =@CityID WHERE AddressID = @AddressID";

                            var lastInsertedRecordId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters, transaction);

                            transaction.Commit();

                            return lastInsertedRecordId;
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
        public async Task<long> DeleteAddressAsync(long? AddressID, long? FMGlobalID)
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
                            var parameterss = new DynamicParameters();
                            parameterss.Add("FMGlobalAddessId", FMGlobalID);
                            var querys = "Delete from FMGlobalAddess where FMGlobalAddessId = @FMGlobalAddessId";
                            await connection.QueryAsync<long>(querys, parameterss, transaction);


                            var parameters = new DynamicParameters();
                            parameters.Add("AddressID", AddressID);
                            var Addquerys = "Delete from Address where AddressID = @AddressID";
                            await connection.QueryAsync<long>(Addquerys, parameters, transaction);

                            transaction.Commit();
                            return 0;
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


