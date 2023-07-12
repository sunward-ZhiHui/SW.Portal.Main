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
    public class SoCustomerAddressQueryRepository : QueryRepository<Address>, ISoCustomerAddressQueryRepository
    {
        public SoCustomerAddressQueryRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<IReadOnlyList<Address>> GetAllAsync()
        {
            try
            {
                var query = @"SELECT AD.Address1,AD.Address2,AD.AddressID,AD.PostCode,AD.City,AD.Country,AD.CountryCode,SCA.SoCustomerAddressId,SCA.CustomerId,SCA.AddressType,SCA.isBilling,SCA.isShipping from [Address] AD
                                INNER JOIN SoCustomerAddress SCA ON SCA.AddressId = AD.AddressID";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<Address>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<long> Insert(Address address)
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
                            parameters.Add("City", address.City);
                            parameters.Add("Country", address.Country);
                            parameters.Add("CountryCode", address.CountryCode);

                            var query = "INSERT INTO [Address](Address1,Address2,PostCode,City,Country,CountryCode) OUTPUT INSERTED.AddressID VALUES (@Address1,@Address2,@PostCode,@City,@Country,@CountryCode)";

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
        public async Task<long> EditAddress(Address address)
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
                            if (address.isShipping == true || address.isBilling == true)
                            {
                                var parametersIsAddress = new DynamicParameters();
                                parametersIsAddress.Add("CustomerId", address.CustomerId);
                                parametersIsAddress.Add("AddressType", address.AddressType);
                                parametersIsAddress.Add("IsBilling", 0, (DbType?)SqlDbType.Bit);
                                parametersIsAddress.Add("IsShipping", 0, (DbType?)SqlDbType.Bit);
                                var Addquerys = "UPDATE SoCustomerAddress SET IsBilling = @IsBilling,IsShipping = @IsShipping WHERE  AddressType = @AddressType AND CustomerId = @CustomerId";
                                await connection.QuerySingleOrDefaultAsync<long>(Addquerys, parametersIsAddress, transaction);
                            }


                            var parametersAddress = new DynamicParameters();
                            parametersAddress.Add("SoCustomerAddressId", address.SoCustomerAddressId);
                            parametersAddress.Add("IsBilling", address.isBilling, (DbType?)SqlDbType.Bit);
                            parametersAddress.Add("IsShipping", address.isShipping, (DbType?)SqlDbType.Bit);
                            var querys = "UPDATE SoCustomerAddress SET IsBilling = @IsBilling,IsShipping = @IsShipping WHERE SoCustomerAddressId = @SoCustomerAddressId";
                            await connection.QuerySingleOrDefaultAsync<long>(querys, parametersAddress, transaction);

                            var parameters = new DynamicParameters();
                            parameters.Add("AddressID", address.AddressID);
                            parameters.Add("Address1", address.Address1);
                            parameters.Add("Address2", address.Address2);
                            parameters.Add("PostCode", address.PostCode);
                            parameters.Add("City", address.City);
                            parameters.Add("Country", address.Country);
                            parameters.Add("CountryCode", address.CountryCode);


                            var query = "UPDATE Address SET Address1 = @Address1,Address2 = @Address2,PostCode = @PostCode,City=@City,Country= @Country,CountryCode =@CountryCode WHERE AddressID = @AddressID";

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
        public async Task<long> InsertSoCustomerAddress(SoCustomerAddress soCustomerAddress)
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
                            if (soCustomerAddress.isShipping == true || soCustomerAddress.isBilling == true)
                            {
                                var parametersIsAddress = new DynamicParameters();
                                parametersIsAddress.Add("CustomerId", soCustomerAddress.CustomerId);
                                parametersIsAddress.Add("AddressType", soCustomerAddress.AddressType);
                                parametersIsAddress.Add("IsBilling", 0, (DbType?)SqlDbType.Bit);
                                parametersIsAddress.Add("IsShipping", 0, (DbType?)SqlDbType.Bit);
                                var Addquerys = "UPDATE SoCustomerAddress SET IsBilling = @IsBilling,IsShipping = @IsShipping WHERE  AddressType = @AddressType AND CustomerId = @CustomerId";
                                await connection.QuerySingleOrDefaultAsync<long>(Addquerys, parametersIsAddress, transaction);
                            }
                            var parameters = new DynamicParameters();
                            parameters.Add("AddressId", soCustomerAddress.AddressId);
                            parameters.Add("CustomerId", soCustomerAddress.CustomerId);
                            parameters.Add("AddressType", soCustomerAddress.AddressType);
                            parameters.Add("IsBilling", soCustomerAddress.isBilling, (DbType?)SqlDbType.Bit);
                            parameters.Add("IsShipping", soCustomerAddress.isShipping, (DbType?)SqlDbType.Bit);
                            var query = "INSERT INTO SoCustomerAddress(AddressId,CustomerId,AddressType,IsBilling,IsShipping) OUTPUT INSERTED.SoCustomerAddressId VALUES (@AddressId,@CustomerId,@AddressType,@IsBilling,@IsShipping)";

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
