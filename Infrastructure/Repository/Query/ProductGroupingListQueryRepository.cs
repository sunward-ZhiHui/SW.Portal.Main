using Core.Entities;
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
using Core.Entities.Views;
using Core.EntityModels;
using IdentityModel.Client;
using NAV;
using Application.Queries;
using Infrastructure.Data;
using Newtonsoft.Json;
using static Infrastructure.Repository.Query.ACEntrysQueryRepository;
using Google.Api.Gax.ResourceNames;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Infrastructure.Repository.Query
{
    public class ProductGroupingListQueryRepository : DbConnector, IProductGroupingListQueryRepository
    {
        private readonly IGenerateDocumentNoSeriesSeviceQueryRepository _generateDocumentNoSeriesSeviceQueryRepository;
        public ProductGroupingListQueryRepository(IConfiguration configuration, IGenerateDocumentNoSeriesSeviceQueryRepository generateDocumentNoSeriesSeviceQueryRepository)
            : base(configuration)
        {
            _generateDocumentNoSeriesSeviceQueryRepository = generateDocumentNoSeriesSeviceQueryRepository;
        }
        public async Task<IReadOnlyList<GenericCodesModel>> GetAllByAsync()
        {
            try
            {
                List<GenericCodesModel> genericCodesModels = new List<GenericCodesModel>();
                var query = "select t1.*,t6.Value as UOMName,t7.Value as ProductName,t8.Value as PackingUnits,t3.CodeValue as StatusCode,t4.UserName as AddedByUser,t5.UserName as ModifiedByUser\r\nfrom GenericCodes t1\r\nLEFT JOIN CodeMaster t3 ON t3.CodeID=t1.StatusCodeID\r\nLEFT JOIN ApplicationUser t4 ON t4.UserID=t1.AddedByUserID\r\nLEFT JOIN ApplicationUser t5 ON t5.UserID=t1.ModifiedByUserID\r\nLEFT JOIN ApplicationMasterDetail t6 ON t6.ApplicationMasterDetailID=t1.UOM \r\nLEFT JOIN ApplicationMasterDetail t7 ON t7.ApplicationMasterDetailID=t1.ProductNameId\r\nLEFT JOIN ApplicationMasterDetail t8 ON t8.ApplicationMasterDetailID=t1.PackingUnitsId;";
                query += "select *  FROM GenericCodeCountry;";
                query += "select *  FROM GenericCodeSupplyToMultiple;";
                List<GenericCodeCountry> countryList = new List<GenericCodeCountry>();
                List<GenericCodeSupplyToMultiple> genericCodeSupplyToMultiple = new List<GenericCodeSupplyToMultiple>();
                using (var connection = CreateConnection())
                {
                    var result = await connection.QueryMultipleAsync(query);
                    genericCodesModels = result.ReadAsync<GenericCodesModel>().Result.ToList();
                    countryList = result.ReadAsync<GenericCodeCountry>().Result.ToList();
                    genericCodeSupplyToMultiple = result.ReadAsync<GenericCodeSupplyToMultiple>().Result.ToList();
                }
                if (genericCodesModels != null)
                {
                    genericCodesModels.ForEach(x =>
                    {
                        x.ManufacturingIds = countryList.Where(w => w.GenericCodeId == x.GenericCodeID).Select(s => s.CountryId).ToList();
                        x.SupplyToIds = genericCodeSupplyToMultiple.Where(w => w.GenericCodeId == x.GenericCodeID).Select(s => s.SupplyToId).ToList();
                    });
                }
                return genericCodesModels;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<CompanyListingModel>> GetCompanyListingForProductGroupingManufacture()
        {
            try
            {
                var query = "select t1.* ,\r\nCONCAT(t1.CompanyName,' | ',STUFF(( SELECT ',' + CAST(tt2.Value AS VARCHAR(MAX))  CustomerCode from CompanyListingCustomerCode tt1 JOIN ApplicationMasterDetail tt2 ON tt1.CustomerCodeID=tt2.ApplicationMasterDetailID AND t1.CompanyListingID=t1.CompanyListingID FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, '')) as BlanketName\r\nfrom CompanyListing t1 Where t1.CompanyListingID IN(\r\nselect t1.CompanyListingID from CompanyListingLine t1\r\nJOIN CompanyListingLine t2 ON t1.CompanyListingID=t2.CompanyListingID\r\nJOIN ApplicationMasterDetail t3 ON t1.BusinessCategoryID=t3.ApplicationMasterDetailID\r\nWHERE t3.Value IN('sunward - group','supplier - principal'))"; using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<CompanyListingModel>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<CompanyListingModel>> GetCompanyListingList()
        {
            try
            {
                var query = "select tt1.*, CONCAT(tt1.CompanyName,' | ',STUFF(( SELECT ',' + CAST(t2.Value AS VARCHAR(MAX))  CustomerCode from CompanyListingCustomerCode t1 JOIN ApplicationMasterDetail t2 ON t1.CustomerCodeID=t2.ApplicationMasterDetailID AND t1.CompanyListingID=tt1.CompanyListingID FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, '')) as BlanketName\r\nfrom CompanyListing tt1";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<CompanyListingModel>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<CompanyListingCustomerCode>> GetCompanyListingCustomerCode(List<long?> CompanyListingIds)
        {
            try
            {
                CompanyListingIds = CompanyListingIds != null ? CompanyListingIds : new List<long?>() { -1 };
                var query = "select t1.*,t2.Value as CustomerCode from CompanyListingCustomerCode t1\r\nJOIN ApplicationMasterDetail t2 ON t1.CustomerCodeID=t2.ApplicationMasterDetailID Where t1.CompanyListingId IN(" + string.Join(',', CompanyListingIds) + ");";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<CompanyListingCustomerCode>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        private async Task<long> InsertOrUpdate(string? TableName, string? PrimareyKeyName, long PrimareyKeyId, DynamicParameters parameters)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    var query = string.Empty;
                    if (PrimareyKeyId > 0)
                    {
                        if (parameters is DynamicParameters subDynamic)
                        {
                            query += "UPDATE " + TableName + " SET\r";
                            var names = string.Empty;
                            if (subDynamic.ParameterNames is not null)
                            {
                                foreach (var keyValue in subDynamic.ParameterNames)
                                {
                                    names += keyValue + "=";
                                    names += "@" + keyValue + ",";
                                }
                            }
                            query += names.TrimEnd(',') + "\rwhere " + PrimareyKeyName + " = " + PrimareyKeyId + ";";
                        }
                    }
                    else
                    {
                        if (parameters is DynamicParameters subDynamic)
                        {
                            query += "INSERT INTO " + TableName + "(\r";
                            var values = string.Empty;
                            var names = string.Empty;
                            if (subDynamic.ParameterNames is not null)
                            {
                                foreach (var keyValue in subDynamic.ParameterNames)
                                {
                                    names += keyValue + ",";
                                    values += "@" + keyValue + ",";
                                }
                            }
                            query += names.TrimEnd(',') + ")\rOUTPUT INSERTED." + PrimareyKeyName + " VALUES(" + values.TrimEnd(',') + ");";
                        }
                    }
                    if (!string.IsNullOrEmpty(query))
                    {
                        if (PrimareyKeyId > 0)
                        {
                            await connection.ExecuteAsync(query, parameters);

                        }
                        else
                        {
                            PrimareyKeyId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        }
                    }
                }
                return PrimareyKeyId;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<GenericCodesModel> InsertOrUpdateProductGrouping(GenericCodesModel value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("Code", value.Code, DbType.String);
                    parameters.Add("Description", value.Description, DbType.String);
                    parameters.Add("Description2", value.Description2, DbType.String);
                    parameters.Add("StatusCodeID", value.StatusCodeID);
                    parameters.Add("PackingCode", value.PackingCode, DbType.String);
                    parameters.Add("Uom", value.Uom);
                    parameters.Add("ManufacringCountry", value.ManufacringCountry);
                    parameters.Add("IsSimulation", value.IsSimulation);
                    parameters.Add("ProductNameId", value.ProductNameId);
                    parameters.Add("PackingUnitsId", value.PackingUnitsId);
                    parameters.Add("AddedByUserID", value.AddedByUserID);
                    parameters.Add("ModifiedByUserID", value.ModifiedByUserID);
                    parameters.Add("AddedDate", value.AddedDate, DbType.DateTime);
                    parameters.Add("ModifiedDate", value.ModifiedDate, DbType.DateTime);
                    var query = string.Empty;
                    string? auditType = "Added";
                    if (value.GenericCodeID > 0)
                    {
                        auditType = "Modified";
                    }
                    else
                    {
                        var ProfileNo = string.Empty;
                        if (value.ProfileId > 0)
                        {
                            ProfileNo = await _generateDocumentNoSeriesSeviceQueryRepository.GenerateDocumentProfileAutoNumber(new DocumentNoSeriesModel { ProfileID = value.ProfileId, AddedByUserID = value.AddedByUserID, StatusCodeID = 710, Title = "Generic Code" });
                            value.ProfileNo = ProfileNo;
                        }
                        parameters.Add("ProfileNo", ProfileNo, DbType.String);
                    }
                    value.GenericCodeID = await InsertOrUpdate("GenericCodes", "GenericCodeID", value.GenericCodeID, parameters);
                    var query1 = string.Empty;
                    query1 += "select *  FROM GenericCodeCountry WHERE GenericCodeID =" + value.GenericCodeID + ";";
                    query1 += "select *  FROM GenericCodeSupplyToMultiple WHERE GenericCodeID =" + value.GenericCodeID + ";";
                    List<GenericCodeCountry> countryList = new List<GenericCodeCountry>();
                    List<GenericCodeSupplyToMultiple> genericCodeSupplyToMultiple = new List<GenericCodeSupplyToMultiple>();
                    if (!string.IsNullOrEmpty(query1))
                    {
                        var results = await connection.QueryMultipleAsync(query1);
                        countryList = results.ReadAsync<GenericCodeCountry>().Result.ToList();
                        genericCodeSupplyToMultiple = results.ReadAsync<GenericCodeSupplyToMultiple>().Result.ToList();
                    }
                    if (value.ManufacturingIds != null && value.ManufacturingIds.Count() > 0)
                    {

                        foreach (var item in value.ManufacturingIds)
                        {
                            var exits = countryList.Where(x => x.CountryId == item).FirstOrDefault();
                            if (exits != null)
                            {
                            }
                            else
                            {
                                query1 = string.Empty;
                                var parameters1 = new DynamicParameters();
                                parameters1.Add("GenericCodeID", value.GenericCodeID);
                                parameters1.Add("IsSellingToCountry", auditType == "Added" ? 0 : 1);
                                parameters1.Add("CountryId", item);
                                query1 += "\rINSERT INTO [GenericCodeCountry](IsSellingToCountry,GenericCodeID,CountryId) OUTPUT INSERTED.GenericCodeCountryId " +
                                   "VALUES (@IsSellingToCountry,@GenericCodeID,@CountryId);\r\n";
                                await connection.QuerySingleOrDefaultAsync<long>(query1, parameters1);
                            }
                        }
                    }
                    if (value.SupplyToIds != null && value.SupplyToIds.Count() > 0)
                    {
                        var companyList = await GetCompanyListingCustomerCode(value.SupplyToIds.ToList());
                        foreach (var item in value.SupplyToIds)
                        {
                            var exits = genericCodeSupplyToMultiple.Where(x => x.SupplyToId == item).FirstOrDefault();
                            if (exits != null)
                            {
                            }
                            else
                            {
                                var GenericCodeSupplyDescription = value.ProductName + " " + value.PackingUnits + " " + (string.Join(",", companyList.Where(w => w.CompanyListingId == item).Select(s => s.CustomerCode).ToList()));
                                query1 = string.Empty;
                                var parameters1 = new DynamicParameters();
                                parameters1.Add("GenericCodeID", value.GenericCodeID);
                                parameters1.Add("GenericCodeSupplyDescription", GenericCodeSupplyDescription, DbType.String);
                                parameters1.Add("SupplyToId", item);
                                query1 += "\rINSERT INTO [GenericCodeSupplyToMultiple](GenericCodeSupplyDescription,GenericCodeID,SupplyToId) OUTPUT INSERTED.GenericCodeSupplyToMultipleId " +
                                   "VALUES (@GenericCodeSupplyDescription,@GenericCodeID,@SupplyToId);\r\n";
                                await connection.QuerySingleOrDefaultAsync<long>(query1, parameters1);
                            }
                        }
                    }
                    return value;

                }

            }

            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }

        }
        public async Task<GenericCodesModel> DeleteProductGrouping(GenericCodesModel value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("GenericCodeID", value.GenericCodeID);
                        var query = "DELETE FROM GenericCodeCountry WHERE GenericCodeID= @GenericCodeID;\n";
                        query += "DELETE FROM GenericCodeSupplyToMultiple WHERE GenericCodeID= @GenericCodeID;\n";
                        query += "DELETE FROM ProductGroupingManufacture WHERE ProductGroupingId= @GenericCodeID;\n";
                        query += "DELETE FROM GenericCodes WHERE GenericCodeID= @GenericCodeID;\n";
                        await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        return value;
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
        public async Task<IReadOnlyList<ProductGroupingManufactureModel>> GetProductGroupingManufactureList(long? ProductGroupingId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("ProductGroupingId", ProductGroupingId);
                var query = "select t1.*,t3.CodeValue as StatusCode,t4.UserName as AddedByUser,t5.UserName as ModifiedByUser,t6.Value as DrugClassificationName,t7.Value as DosageFormName,t8.CompanyName as ManufactureBy\r\nfrom ProductGroupingManufacture t1 \r\nLEFT JOIN CodeMaster t3 ON t3.CodeID=t1.StatusCodeID\r\nLEFT JOIN ApplicationUser t4 ON t4.UserID=t1.AddedByUserID \r\nLEFT JOIN ApplicationUser t5 ON t5.UserID=t1.ModifiedByUserID\r\nLEFT JOIN ApplicationMasterDetail t6 ON t6.ApplicationMasterDetailID=t1.DrugClassificationID\r\nLEFT JOIN ApplicationMasterDetail t7 ON t7.ApplicationMasterDetailID=t1.DosageFormID\r\nLEFT JOIN CompanyListing t8 ON t8.CompanyListingID=t1.ManufactureByID WHERE t1.ProductGroupingId=@ProductGroupingId;";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ProductGroupingManufactureModel>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ProductGroupingManufactureModel> InsertOrUpdateProductGroupingManufacture(ProductGroupingManufactureModel value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("DosageFormId", value.DosageFormId);
                    parameters.Add("DrugClassificationId", value.DrugClassificationId);
                    parameters.Add("ProductGroupingId", value.ProductGroupingId);
                    parameters.Add("SupplyToId", value.SupplyToId);
                    parameters.Add("StatusCodeID", value.StatusCodeID);
                    parameters.Add("ManufactureById", value.ManufactureById);
                    parameters.Add("AddedByUserID", value.AddedByUserID);
                    parameters.Add("ModifiedByUserID", value.ModifiedByUserID);
                    parameters.Add("AddedDate", value.AddedDate, DbType.DateTime);
                    parameters.Add("ModifiedDate", value.ModifiedDate, DbType.DateTime);
                    value.ProductGroupingManufactureId = await InsertOrUpdate("ProductGroupingManufacture", "ProductGroupingManufactureId", value.ProductGroupingManufactureId, parameters);
                    return value;

                }

            }

            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }

        }
        public async Task<ProductGroupingManufactureModel> DeleteProductGroupingManufacture(ProductGroupingManufactureModel value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("ProductGroupingManufactureId", value.ProductGroupingManufactureId);
                        var query = "DELETE FROM ProductGroupingManufacture WHERE ProductGroupingManufactureId= @ProductGroupingManufactureId;\n";
                        await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        return value;
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


        public async Task<IReadOnlyList<ProductGroupingNavModel>> GetProductGroupingNavList(long? ProductGroupingManufactureId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("ProductGroupingManufactureId", ProductGroupingManufactureId);
                var query = "select t7.GenericCodeSupplyDescription as GenericCodeSupplyToMultiple,t1.*,t3.CodeValue as StatusCode,t4.UserName as AddedByUser,t5.UserName as ModifiedByUser,t6.No as NavNo,t6.VendorNo as ReplenishmentMethod,t6.Description,t6.InternalRef as ManufactureFor,t6.BaseUnitofMeasure as UOM,\r\nREPLACE(t6.ItemCategoryCode, 'FP', '') as ManufactureBy\r\nfrom ProductGroupingNav t1\r\nLEFT JOIN CodeMaster t3 ON t3.CodeID=t1.StatusCodeID\r\nLEFT JOIN ApplicationUser t4 ON t4.UserID=t1.AddedByUserID\r\nLEFT JOIN ApplicationUser t5 ON t5.UserID=t1.ModifiedByUserID\r\nLEFT JOIN NAVItems t6 ON t6.ItemId=t1.ItemID\r\nLEFT JOIN GenericCodeSupplyToMultiple t7 ON t7.GenericCodeSupplyToMultipleID=t1.GenericCodeSupplyToMultipleId\r\n  where t1.ProductGroupingManufactureId=@ProductGroupingManufactureId;";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ProductGroupingNavModel>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ProductGroupingNavModel> InsertOrUpdateProductGroupingNav(ProductGroupingNavModel value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("ProductGroupingManufactureId", value.ProductGroupingManufactureId);
                    parameters.Add("ItemId", value.ItemId);
                    parameters.Add("VarianceNo", value.VarianceNo, DbType.String);
                    parameters.Add("GenericCodeSupplyToMultipleId", value.GenericCodeSupplyToMultipleId);
                    parameters.Add("StatusCodeID", value.StatusCodeID);
                    parameters.Add("AddedByUserID", value.AddedByUserID);
                    parameters.Add("ModifiedByUserID", value.ModifiedByUserID);
                    parameters.Add("AddedDate", value.AddedDate, DbType.DateTime);
                    parameters.Add("ModifiedDate", value.ModifiedDate, DbType.DateTime);
                    value.ProductGroupingNavId = await InsertOrUpdate("ProductGroupingNav", "ProductGroupingNavId", value.ProductGroupingNavId, parameters);
                    return value;

                }

            }

            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }

        }
        public async Task<ProductGroupingNavModel> DeleteProductGroupingNav(ProductGroupingNavModel value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("ProductGroupingNavId", value.ProductGroupingNavId);
                        var query = "DELETE FROM ProductGroupingNavDifference WHERE ProductGroupingNavId= @ProductGroupingNavId;\n";
                        query += "DELETE FROM ProductGroupingNav WHERE ProductGroupingNavId= @ProductGroupingNavId;\n";
                        await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        return value;
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



        public async Task<IReadOnlyList<ProductGroupingNavDifferenceModel>> GetProductGroupingNavDifferenceList(long? ProductGroupingNavId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("ProductGroupingNavId", ProductGroupingNavId);
                var query = "select t1.*,t3.CodeValue as StatusCode,t4.UserName as AddedByUser,t5.UserName as ModifiedByUser,t6.Value as TypeOfDifference from ProductGroupingNavDifference t1\r\nLEFT JOIN CodeMaster t3 ON t3.CodeID=t1.StatusCodeID\r\nLEFT JOIN ApplicationUser t4 ON t4.UserID=t1.AddedByUserID\r\nLEFT JOIN ApplicationUser t5 ON t5.UserID=t1.ModifiedByUserID\r\nLEFT JOIN ApplicationMasterDetail t6 ON t6.ApplicationMasterDetailID=t1.TypeOfDifferenceID where t1.ProductGroupingNavId=@ProductGroupingNavId;";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ProductGroupingNavDifferenceModel>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ProductGroupingNavDifferenceModel> InsertOrUpdateProductGroupingNavDifference(ProductGroupingNavDifferenceModel value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("ProductGroupingNavId", value.ProductGroupingNavId);
                    parameters.Add("TypeOfDifferenceId", value.TypeOfDifferenceId);
                    parameters.Add("DifferenceInfo", value.DifferenceInfo, DbType.String);
                    parameters.Add("StatusCodeID", value.StatusCodeID);
                    parameters.Add("AddedByUserID", value.AddedByUserID);
                    parameters.Add("ModifiedByUserID", value.ModifiedByUserID);
                    parameters.Add("AddedDate", value.AddedDate, DbType.DateTime);
                    parameters.Add("ModifiedDate", value.ModifiedDate, DbType.DateTime);
                    value.ProductGroupingNavDifferenceId = await InsertOrUpdate("ProductGroupingNavDifference", "ProductGroupingNavDifferenceId", value.ProductGroupingNavDifferenceId, parameters);
                    return value;
                }

            }

            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }

        }
        public async Task<ProductGroupingNavDifferenceModel> DeleteProductGroupingNavDifference(ProductGroupingNavDifferenceModel value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("ProductGroupingNavDifferenceId", value.ProductGroupingNavDifferenceId);
                        var query = "DELETE FROM ProductGroupingNavDifference WHERE ProductGroupingNavDifferenceId= @ProductGroupingNavDifferenceId;\n";
                        await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        return value;
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
