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
        public TempSalesPackInformationQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

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
    }

}
