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

namespace Infrastructure.Repository.Query
{

    public class TempSalesPackInformationQueryRepository : QueryRepository<TempSalesPackInformationReportModel>, ITempSalesPackInformationQueryRepository
    {
        private readonly IGenerateDocumentNoSeriesSeviceQueryRepository _generateDocumentNoSeriesSeviceQueryRepository;
        public TempSalesPackInformationQueryRepository(IConfiguration configuration, IGenerateDocumentNoSeriesSeviceQueryRepository generateDocumentNoSeriesSeviceQueryRepository)
            : base(configuration)
        {
            _generateDocumentNoSeriesSeviceQueryRepository = generateDocumentNoSeriesSeviceQueryRepository;
        }
        public async Task<IReadOnlyList<TempSalesPackInformationReportModel>> GetTempSalesPackInformationReport()
        {
            List<TempSalesPackInformationReportModel> TempSalesPackInformationReportModel = new List<TempSalesPackInformationReportModel>();
            try
            {
                var tempSalesitemInfo = await GetTempSalesPackInformation();
                var query = "select t1.FinishProductGeneralInfoLineId,t1.FinishProductGeneralInfoID,t4.Value as ManufacturingSite,\r\nt5.Value as ProductName,t6.Value as ProductOwner,t7.CompanyName as ProductionRegistrationHolderName,\r\nt8.Value as PackagingType,t1.PackQty as SmallestPackQty,t9.Value as SmallestQtyUnit,t10.Value as SmallestPerPack,\r\nt1.FactorOfSmallestProductionPack as RegistrationFactor,t11.Value as RegisterCountry\r\nfrom FinishProdcutGeneralInfoLine t1\r\nLEFT JOIN FinishProductGeneralInfo t2 ON t1.FinishProductGeneralInfoId=t2.FinishProductGeneralInfoId\r\nLEFT JOIN ApplicationMasterDetail t6 ON t6.ApplicationMasterDetailID=t2.RegisterProductOwnerId\r\nLEFT JOIN ApplicationMasterDetail t11 ON t11.ApplicationMasterDetailID=t2.RegisterCountry\r\nLEFT JOIN CompanyListing t7 ON t7.CompanyListingID=t2.ProductionRegistrationHolderId\r\nLEFT JOIN FinishProduct t3 ON t3.FinishProductID=t2.FinishProductID\r\nLEFT JOIN ApplicationMasterDetail t4 ON t4.ApplicationMasterDetailID=t3.ManufacturingSiteId\r\nLEFT JOIN ApplicationMasterDetail t5 ON t5.ApplicationMasterDetailID=t3.ProductId\r\nLEFT JOIN ApplicationMasterDetail t8 ON t8.ApplicationMasterDetailID=t1.PackagingTypeID\r\nLEFT JOIN ApplicationMasterDetail t9 ON t9.ApplicationMasterDetailID=t1.PackQtyunitId\r\nLEFT JOIN ApplicationMasterDetail t10 ON t10.ApplicationMasterDetailID=t1.PerPackId\r\nWHERE t1.FinishProductGeneralInfoLineId IN(select FinishProductGeneralInfoLineId from TempSalesPackInformation)";
                var result = new List<TempSalesPackInformationReportModel>();
                using (var connection = CreateConnection())
                {
                    result = (await connection.QueryAsync<TempSalesPackInformationReportModel>(query)).ToList();
                }
                if (result != null && result.Count > 0)
                {
                    result.ForEach(s =>
                    {

                        s.AddedDate = tempSalesitemInfo != null ? tempSalesitemInfo.Where(t => t.FinishProductGeneralInfoLineId == s.FinishProductGeneralInfoLineID).Select(t => t.AddedDate).FirstOrDefault() : null;
                        s.AddedByUser = tempSalesitemInfo != null ? tempSalesitemInfo.Where(t => t.FinishProductGeneralInfoLineId == s.FinishProductGeneralInfoLineID).Select(t => t.AddedByUser).FirstOrDefault() : "";
                        s.StatusCodeID = tempSalesitemInfo?.Where(t => t.FinishProductGeneralInfoLineId == s.FinishProductGeneralInfoLineID).Select(t => t.StatusCodeId).FirstOrDefault();
                        s.StatusCode = tempSalesitemInfo != null ? tempSalesitemInfo.Where(t => t.FinishProductGeneralInfoLineId == s.FinishProductGeneralInfoLineID).Select(t => t.StatusCode).FirstOrDefault() : "";
                        s.TempSalesPackInformationID = tempSalesitemInfo?.Where(t => t.FinishProductGeneralInfoLineId == s.FinishProductGeneralInfoLineID).Select(t => t.TempSalesPackInformationId).FirstOrDefault();
                        TempSalesPackInformationReportModel.Add(s);
                    });
                }
                return TempSalesPackInformationReportModel;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<TempSalesPackInformation>> GetTempSalesPackInformation()
        {
            try
            {
                var query = "\r\nselect t1.*,t2.CodeValue as StatusCode,t3.UserName as AddedByUser,t4.UserName as ModifiedByUser from TempSalesPackInformation t1\r\nLEFT JOIN CodeMaster  t2 ON t2.CodeID=t1.statusCodeID\r\nLEFT JOIN ApplicationUser  t3 ON t3.UserID=t1.AddedByUserID\r\nLEFT JOIN ApplicationUser  t4 ON t4.UserID=t1.ModifiedByUserID\r\n";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<TempSalesPackInformation>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<TempSalesPackInformationFactor>> GetTempSalesPackInformationFactor(long? Id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("TempSalesPackInformationID", Id);
                var query = "select t1.*,t2.UserName as AddedBy,t3.UserName as ModifiedBy,t4.CodeValue as StatusCode,t5.No as ItemName,t5.Description as ItemDescription from TempSalesPackInformationFactor t1\r\nJOIN ApplicationUser t2 ON t2.UserID=t1.AddedByUserID\r\nLEFT JOIN ApplicationUser t3 ON t3.UserID=t1.ModifiedByUserID\r\nJOIN CodeMaster t4 ON t4.CodeID=t1.StatusCodeID\r\nJOIN NAVItems t5 ON t5.ItemId=t1.ItemID WHERE t1.TempSalesPackInformationID=@TempSalesPackInformationID";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<TempSalesPackInformationFactor>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<TempSalesPackInformationReportModel> GetTempSalesPackInformationReportSync(TempSalesPackInformationReportModel tempSalesPackInformationReportModel)
        {
            var getData = await GetTempSalesPackInformationReportDataSync(tempSalesPackInformationReportModel);
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var query = string.Empty;
                        var parameters = new DynamicParameters();
                        parameters.Add("AddedDate", DateTime.Now, DbType.DateTime);
                        parameters.Add("AddedByUserId", tempSalesPackInformationReportModel.AddedByUserID);
                        parameters.Add("StatusCodeID", tempSalesPackInformationReportModel.StatusCodeID);
                        if (getData != null && getData.Count() > 0)
                        {
                            getData.ToList().ForEach(s =>
                            {
                                query += "INSERT INTO TempSalesPackInformation(FinishProductGeneralInfoLineId,FinishproductGeneralInfoId,AddedByUserId,AddedDate,StatusCodeID) VALUES " +
                                "('" + s.FinishProductGeneralInfoLineId + "'," + s.FinishProductGeneralInfoId + ",@AddedByUserId,@AddedDate,@StatusCodeID);";
                            });
                            await connection.ExecuteAsync(query, parameters);
                        }
                        return tempSalesPackInformationReportModel;
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
        public async Task<IReadOnlyList<FinishProdcutGeneralInfoLine>> GetTempSalesPackInformationReportDataSync(TempSalesPackInformationReportModel tempSalesPackInformationReportModel)
        {
            try
            {
                var query = "select * from FinishProdcutGeneralInfoLine where FinishProductGeneralInfoLineId not in(select FinishProductGeneralInfoLineId from TempSalesPackInformation)\r\n";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<FinishProdcutGeneralInfoLine>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<TempSalesPackInformationFactor> InsertTempSalesPackInformationFactor(TempSalesPackInformationFactor value)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("TempSalesPackInformationFactorId", value.TempSalesPackInformationFactorId);
                        parameters.Add("AddedDate", DateTime.Now, DbType.DateTime);
                        parameters.Add("AddedByUserID", value.AddedByUserId);
                        parameters.Add("ModifiedDate", DateTime.Now, DbType.DateTime);
                        parameters.Add("ModifiedByUserID", value.ModifiedByUserId);
                        parameters.Add("StatusCodeID", value.StatusCodeId);
                        parameters.Add("TempSalesPackInformationId", value.TempSalesPackInformationId);
                        parameters.Add("ProfileId", value.ProfileId);
                        parameters.Add("SalesFactor", value.SalesFactor);
                        parameters.Add("ItemId", value.ItemId);
                        parameters.Add("Fpname", value.Fpname, DbType.String);
                        parameters.Add("QtypackPerCarton", value.QtypackPerCarton);
                        parameters.Add("SalesPackSize", value.SalesPackSize);
                        parameters.Add("SellingPackUnit", value.SellingPackUnit);
                        if (value.TempSalesPackInformationFactorId > 0)
                        {
                            var query = " UPDATE TempSalesPackInformationFactor SET SalesPackSize = @SalesPackSize,SellingPackUnit =@SellingPackUnit,QtypackPerCarton=@QtypackPerCarton,Fpname=@Fpname,\n\r" +
                                "ModifiedByUserID=@ModifiedByUserID,ModifiedDate=@ModifiedDate,StatusCodeID=@StatusCodeID,TempSalesPackInformationId=@TempSalesPackInformationId,ItemId=@ItemId,SalesFactor=@SalesFactor\n\r" +
                                "WHERE TempSalesPackInformationFactorId = @TempSalesPackInformationFactorId";
                            await connection.ExecuteAsync(query, parameters);
                        }
                        else
                        {
                            value.ProfileNo = await _generateDocumentNoSeriesSeviceQueryRepository.GenerateDocumentProfileAutoNumber(new DocumentNoSeriesModel { ProfileID = value.ProfileId, AddedByUserID = value.AddedByUserId, StatusCodeID = 710 });
                            parameters.Add("ProfileNo", value.ProfileNo, DbType.String);
                            var query = "INSERT INTO TempSalesPackInformationFactor(ProfileNo,SalesPackSize,SellingPackUnit,QtypackPerCarton,Fpname,AddedByUserID,ModifiedByUserID,AddedDate,ModifiedDate,StatusCodeID,TempSalesPackInformationId,ProfileId,ItemId,SalesFactor)  " +
                                    "OUTPUT INSERTED.TempSalesPackInformationFactorId VALUES " +
                                    "(@ProfileNo,@SalesPackSize,@SellingPackUnit,@QtypackPerCarton,@Fpname,@AddedByUserID,@ModifiedByUserID,@AddedDate,@ModifiedDate,@StatusCodeID,@TempSalesPackInformationId,@ProfileId,@ItemId,@SalesFactor)";

                            value.TempSalesPackInformationFactorId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);

                        }
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
        public async Task<TempSalesPackInformationFactor> DeleteTempSalesPackInformationFactor(long? id)
        {
            TempSalesPackInformationFactor tempSalesPackInformationFactor = new TempSalesPackInformationFactor();
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        tempSalesPackInformationFactor.TempSalesPackInformationFactorId = id.Value;
                        var parameters = new DynamicParameters();
                        parameters.Add("id", id);

                        var query = "DELETE  FROM TempSalesPackInformationFactor WHERE TempSalesPackInformationFactorId = @id";
                        var rowsAffected = await connection.ExecuteAsync(query, parameters);


                        return tempSalesPackInformationFactor;
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
