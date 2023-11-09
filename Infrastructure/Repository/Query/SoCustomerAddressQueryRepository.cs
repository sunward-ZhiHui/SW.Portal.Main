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
    public class SoCustomerAddressQueryRepository : QueryRepository<view_SoCustomerAddress>, ISoCustomerAddressQueryRepository
    {
        public SoCustomerAddressQueryRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<IReadOnlyList<view_SoCustomerAddress>> GetAllAsync()
        {
            try
            {
                var query = @"SELECT * from view_SoCustomerAddress";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<view_SoCustomerAddress>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<view_SoCustomerAddress>> GetAllByAddressTypeAsync(string AddressType, long? CustomerId)
        {
            try
            {
                var query = "SELECT * FROM view_SoCustomerAddress  WHERE AddressType = @AddressType and CustomerId = @CustomerId";
                var parameters = new DynamicParameters();
                parameters.Add("AddressType", AddressType);
                parameters.Add("CustomerId", CustomerId);
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<view_SoCustomerAddress>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<view_SoCustomerAddress>> GetByCustomerIdAsync(long? CustomerId)
        {
            try
            {
                var query = "SELECT * FROM view_SoCustomerAddress  WHERE CustomerId = @CustomerId";
                var parameters = new DynamicParameters();
                parameters.Add("CustomerId", CustomerId);
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<view_SoCustomerAddress>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<long> Insert(view_SoCustomerAddress address)
        {
            try
            {
                using (var connection = CreateConnection())
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

                        var lastInsertedRecordId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);


                        return lastInsertedRecordId;
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
        public async Task<long> EditAddress(view_SoCustomerAddress address)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        if (address.isShipping == true || address.isBilling == true)
                        {
                            var parametersIsAddress = new DynamicParameters();
                            parametersIsAddress.Add("CustomerId", address.CustomerId);
                            parametersIsAddress.Add("AddressType", address.AddressType);
                            parametersIsAddress.Add("IsBilling", 0, (DbType?)SqlDbType.Bit);
                            parametersIsAddress.Add("IsShipping", 0, (DbType?)SqlDbType.Bit);
                            var Addquerys = "UPDATE SoCustomerAddress SET IsBilling = @IsBilling,IsShipping = @IsShipping WHERE  AddressType = @AddressType AND CustomerId = @CustomerId";
                            await connection.QuerySingleOrDefaultAsync<long>(Addquerys, parametersIsAddress);
                        }


                        var parametersAddress = new DynamicParameters();
                        parametersAddress.Add("SoCustomerAddressId", address.SoCustomerAddressId);
                        parametersAddress.Add("IsBilling", address.isBilling, (DbType?)SqlDbType.Bit);
                        parametersAddress.Add("IsShipping", address.isShipping, (DbType?)SqlDbType.Bit);
                        var querys = "UPDATE SoCustomerAddress SET IsBilling = @IsBilling,IsShipping = @IsShipping WHERE SoCustomerAddressId = @SoCustomerAddressId";
                        await connection.QuerySingleOrDefaultAsync<long>(querys, parametersAddress);

                        var parameters = new DynamicParameters();
                        parameters.Add("AddressID", address.AddressID);
                        parameters.Add("Address1", address.Address1);
                        parameters.Add("Address2", address.Address2);
                        parameters.Add("PostCode", address.PostCode);
                        parameters.Add("CountryID", address.CountryID);
                        parameters.Add("StateID", address.StateID);
                        parameters.Add("CityID", address.CityID);


                        var query = "UPDATE Address SET Address1 = @Address1,Address2 = @Address2,PostCode = @PostCode,CountryID=@CountryID,StateID= @StateID,CityID =@CityID WHERE AddressID = @AddressID";

                        var lastInsertedRecordId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);


                        return lastInsertedRecordId;
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
        public async Task<long> InsertSoCustomerAddress(SoCustomerAddress soCustomerAddress)
        {
            try
            {
                using (var connection = CreateConnection())
                {


                    try
                    {
                        if (soCustomerAddress.isShipping == true || soCustomerAddress.isBilling == true)
                        {
                            var parametersIsAddress = new DynamicParameters();
                            parametersIsAddress.Add("CustomerId", soCustomerAddress.CustomerId);
                            parametersIsAddress.Add("AddressType", soCustomerAddress.AddressType);
                            parametersIsAddress.Add("IsBilling", 0, (DbType?)SqlDbType.Bit);
                            parametersIsAddress.Add("IsShipping", 0, (DbType?)SqlDbType.Bit);
                            var Addquerys = "UPDATE SoCustomerAddress SET IsBilling = @IsBilling,IsShipping = @IsShipping WHERE  AddressType = @AddressType AND CustomerId = @CustomerId";
                            await connection.QuerySingleOrDefaultAsync<long>(Addquerys, parametersIsAddress);
                        }
                        var parameters = new DynamicParameters();
                        parameters.Add("AddressId", soCustomerAddress.AddressId);
                        parameters.Add("CustomerId", soCustomerAddress.CustomerId);
                        parameters.Add("AddressType", soCustomerAddress.AddressType);
                        parameters.Add("IsBilling", soCustomerAddress.isBilling, (DbType?)SqlDbType.Bit);
                        parameters.Add("IsShipping", soCustomerAddress.isShipping, (DbType?)SqlDbType.Bit);
                        var query = "INSERT INTO SoCustomerAddress(AddressId,CustomerId,AddressType,IsBilling,IsShipping) OUTPUT INSERTED.SoCustomerAddressId VALUES (@AddressId,@CustomerId,@AddressType,@IsBilling,@IsShipping)";

                        var lastInsertedRecordId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);


                        return lastInsertedRecordId;
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

        public async Task<Address> GetByIdAsync(long AddressID)
        {
            try
            {
                var query = "SELECT * FROM Address WHERE AddressID = @AddressID";
                var parameters = new DynamicParameters();
                parameters.Add("AddressID", AddressID, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<Address>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<SoCustomerAddress> GetByCustomerAddressIdAsync(long SoCustomerAddressId)
        {
            try
            {
                var query = "SELECT * FROM SoCustomerAddress WHERE SoCustomerAddressId = @SoCustomerAddressId";
                var parameters = new DynamicParameters();
                parameters.Add("SoCustomerAddressId", SoCustomerAddressId, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<SoCustomerAddress>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<Address> DeleteAddressAsync(Address address)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    await connection.DeleteAsync(address);
                }
                return address;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<SoCustomerAddress> DeleteSoCustomerAddressAsync(SoCustomerAddress soCustomerAddress)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    await connection.DeleteAsync(soCustomerAddress);
                }
                return soCustomerAddress;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }

}
