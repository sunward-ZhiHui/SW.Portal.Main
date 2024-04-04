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
using System.ComponentModel.Design;
using System.Collections;
using Core.EntityModel;

namespace Infrastructure.Repository.Query
{
    public class DynamicFormDataSourceQueryRepository : QueryRepository<AttributeDetails>, IDynamicFormDataSourceQueryRepository
    {
        public DynamicFormDataSourceQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<IReadOnlyList<AttributeDetails>> GetDataSourceDropDownList(long? CompanyId, List<string?> dataSourceTableIds, string? plantCode, List<long?> applicationMasterIds)
        {
            var dataSourceDropDownList = new List<AttributeDetails>(); List<long> plantIds = new List<long>();
            var plantsData = await GetPlantDataSource();
            if (dataSourceTableIds != null && dataSourceTableIds.Count() > 0)
            {
                List<string?> plantNames = new List<string?>() { "SWMY", "SWSG" };

                if (plantsData != null && plantsData.Count > 0)
                {
                    plantIds = plantsData.Where(w => plantNames.Contains(w.AttributeDetailName)).Select(s => s.AttributeDetailID).ToList();
                }
                if (dataSourceTableIds.Contains("Plant"))
                {
                    dataSourceDropDownList.AddRange(plantsData);
                }
                if (dataSourceTableIds.Contains("NavItems"))
                {
                    dataSourceDropDownList.AddRange(await GetNavItemsDataSource(CompanyId, plantCode, plantIds));
                }
                if (dataSourceTableIds.Contains("Employee"))
                {
                    dataSourceDropDownList.AddRange(await GetEmployeeDataSource(CompanyId, plantCode, plantIds));
                }
                if (dataSourceTableIds.Contains("Division"))
                {
                    dataSourceDropDownList.AddRange(await GetDivisionDataSource(CompanyId, plantCode, plantIds));
                }
                if (dataSourceTableIds.Contains("Department"))
                {
                    dataSourceDropDownList.AddRange(await GetDepartmentDataSource(CompanyId, plantCode, plantIds));
                }
                if (dataSourceTableIds.Contains("Section"))
                {
                    dataSourceDropDownList.AddRange(await GetSectionDataSource(CompanyId, plantCode, plantIds));
                }
                if (dataSourceTableIds.Contains("SubSection"))
                {
                    dataSourceDropDownList.AddRange(await GetSubSectionDataSource(CompanyId, plantCode, plantIds));
                }
                var soCustomerType = new List<string?>() { "Clinic", "Vendor", "Customer" };
                var soCustomerList = soCustomerType.Intersect(dataSourceTableIds).ToList();
                if (soCustomerList.Count() > 0)
                {
                    dataSourceDropDownList.AddRange(await GetSoCustomerDataSource(soCustomerList));
                }
                var rawMatItemType = new List<string?>() { "RawMatItem", "PackagingItem", "ProcessItem" };
                var rawMatItemList = rawMatItemType.Intersect(dataSourceTableIds).ToList();
                if (rawMatItemList.Count() > 0)
                {
                    dataSourceDropDownList.AddRange(await GetRawMatItemListDataSource(rawMatItemList, CompanyId, plantCode, plantIds));
                }

                if (dataSourceTableIds.Contains("ApplicationMaster") && applicationMasterIds.Count>0)
                {
                    dataSourceDropDownList.AddRange(await GetApplicationMasterDataSource(applicationMasterIds));
                }
            }
            else
            {
                dataSourceDropDownList.AddRange(plantsData);
                dataSourceDropDownList.AddRange(await GetNavItemsDataSource(CompanyId, plantCode, plantIds));
                dataSourceDropDownList.AddRange(await GetEmployeeDataSource(CompanyId, plantCode, plantIds));
                dataSourceDropDownList.AddRange(await GetDivisionDataSource(CompanyId, plantCode, plantIds));
                dataSourceDropDownList.AddRange(await GetDepartmentDataSource(CompanyId, plantCode, plantIds));
                dataSourceDropDownList.AddRange(await GetSectionDataSource(CompanyId, plantCode, plantIds));
                dataSourceDropDownList.AddRange(await GetSubSectionDataSource(CompanyId, plantCode, plantIds));
                var soCustomerType = new List<string?>() { "Clinic", "Vendor", "Customer" };
                var soCustomerList = soCustomerType.Intersect(dataSourceTableIds).ToList();
                if (soCustomerList.Count() > 0)
                {
                    dataSourceDropDownList.AddRange(await GetSoCustomerDataSource(soCustomerList));
                }
                var rawMatItemType = new List<string?>() { "RawMatItem", "PackagingItem", "ProcessItem" };
                var rawMatItemList = rawMatItemType.Intersect(dataSourceTableIds).ToList();
                if (rawMatItemList.Count() > 0)
                {
                    dataSourceDropDownList.AddRange(await GetRawMatItemListDataSource(rawMatItemList, CompanyId, plantCode, plantIds));
                }
                if (applicationMasterIds.Count > 0)
                {
                    dataSourceDropDownList.AddRange(await GetApplicationMasterDataSource(applicationMasterIds));
                }
            }
            return dataSourceDropDownList;
        }
        private async Task<IReadOnlyList<AttributeDetails>> GetApplicationMasterDataSource(List<long?> applicationMasterIds)
        {
            var attributeDetails = new List<AttributeDetails>();
            try
            {
                var query = string.Empty;
                query += "select 'ApplicationMaster' as DropDownTypeId,t1.ApplicationMasterId, t1.ApplicationMasterDetailID as AttributeDetailID,Value as AttributeDetailName,t1.Description,t2.ApplicationMasterName,t2.ApplicationMasterCodeID as ApplicationMasterCodeID from ApplicationMasterDetail t1 JOIN ApplicationMaster t2 ON t1.ApplicationMasterID=t2.ApplicationMasterID\r\n";
                if (applicationMasterIds != null && applicationMasterIds.Count > 0)
                {
                    applicationMasterIds = applicationMasterIds != null && applicationMasterIds.Count() > 0 ? applicationMasterIds : new List<long?>() { -1 };
                    query += "where t1.ApplicationMasterId in(" + string.Join(',', applicationMasterIds) + ")";
                }
                using (var connection = CreateConnection())
                {
                    var result = (await connection.QueryAsync<AttributeDetails>(query)).ToList();
                    attributeDetails = result != null && result.Count() > 0 ? result : new List<AttributeDetails>();
                }
                return attributeDetails;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        private async Task<IReadOnlyList<AttributeDetails>> GetPlantDataSource()
        {
            var attributeDetails = new List<AttributeDetails>();
            try
            {
                var query = string.Empty;
                query += "select 'Plant' as DropDownTypeId, PlantID as AttributeDetailID,PlantCode as AttributeDetailName,Description as Description from Plant\r\n";
                using (var connection = CreateConnection())
                {
                    var result = (await connection.QueryAsync<AttributeDetails>(query)).ToList();
                    attributeDetails = result != null && result.Count() > 0 ? result : new List<AttributeDetails>();
                }
                return attributeDetails;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        private async Task<IReadOnlyList<AttributeDetails>> GetNavItemsDataSource(long? CompanyId, string? plantCode, List<long> plantIds)
        {
            var attributeDetails = new List<AttributeDetails>();
            try
            {
                var query = string.Empty;
                query += "select 'NavItems' as DropDownTypeId,t1.ItemId as AttributeDetailID,t1.CompanyId,No as AttributeDetailName,t2.PlantCode as CompanyName,CONCAT(t1.Description,(case when ISNULL(NULLIF(t1.Description2, ''), null) is NULL then  t1.Description2 ELSE  CONCAT(' | ',t1.Description2) END)) as Description\r\n from Navitems t1 JOIN Plant t2 ON t1.CompanyId=t2.PlantID\r\n";
                if (CompanyId > 0)
                {
                    if (plantCode == "swgp")
                    {
                        plantIds = plantIds != null && plantIds.Count() > 0 ? plantIds : new List<long>() { -1 };
                        query += "where t1.no like 'FP%' AND t1.CompanyId in(" + string.Join(',', plantIds) + ")";
                    }
                    else
                    {
                        query += "Where t1.no like 'FP%' AND t1.CompanyId=" + CompanyId + "\r\n";
                    }
                }
                else
                {
                    query += "Where t1.no like 'FP%';\r\n";
                }
                using (var connection = CreateConnection())
                {
                    var result = (await connection.QueryAsync<AttributeDetails>(query)).ToList();
                    attributeDetails = result != null && result.Count() > 0 ? result : new List<AttributeDetails>();
                }
                return attributeDetails;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        private async Task<IReadOnlyList<AttributeDetails>> GetEmployeeDataSource(long? CompanyId, string? plantCode, List<long> plantIds)
        {
            var attributeDetails = new List<AttributeDetails>();
            try
            {
                var query = string.Empty;
                query += "select 'Employee' as DropDownTypeId,t1.EmployeeID as AttributeDetailID,t1.PlantId as CompanyId,t2.PlantCode as CompanyName, t1.FirstName as AttributeDetailName,CONCAT(case when t1.NickName is NULL then  t1.FirstName ELSE  t1.NickName END,' | ',t1.LastName) as Description from Employee t1 JOIN Plant t2 ON t1.PlantID=t2.PlantID \r\n";
                if (CompanyId > 0)
                {
                    if (plantCode == "swgp")
                    {
                        plantIds = plantIds != null && plantIds.Count() > 0 ? plantIds : new List<long>() { -1 };
                        query += "where t1.PlantId in(" + string.Join(',', plantIds) + ")";
                    }
                    else
                    {
                        query += "Where t1.PlantId=" + CompanyId + "\r\n";
                    }
                }
                using (var connection = CreateConnection())
                {
                    var result = (await connection.QueryAsync<AttributeDetails>(query)).ToList();
                    attributeDetails = result != null && result.Count() > 0 ? result : new List<AttributeDetails>();
                }
                return attributeDetails;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        private async Task<IReadOnlyList<AttributeDetails>> GetDivisionDataSource(long? CompanyId, string? plantCode, List<long> plantIds)
        {
            var attributeDetails = new List<AttributeDetails>();
            try
            {
                var query = string.Empty;
                query += "select 'Division' as DropDownTypeId,t1.DivisionID as AttributeDetailID,t1.CompanyID as CompanyId,t2.PlantCode as CompanyName, t1.Name as AttributeDetailName,t1.Description as Description from Division t1 JOIN Plant t2 ON t1.CompanyID=t2.PlantID\r\n \r\n";
                if (CompanyId > 0)
                {
                    if (plantCode == "swgp")
                    {
                        plantIds = plantIds != null && plantIds.Count() > 0 ? plantIds : new List<long>() { -1 };
                        query += "where t1.CompanyID in(" + string.Join(',', plantIds) + ")";
                    }
                    else
                    {
                        query += "Where t1.CompanyID=" + CompanyId + "\r\n";
                    }
                }
                using (var connection = CreateConnection())
                {
                    var result = (await connection.QueryAsync<AttributeDetails>(query)).ToList();
                    attributeDetails = result != null && result.Count() > 0 ? result : new List<AttributeDetails>();
                }
                return attributeDetails;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        private async Task<IReadOnlyList<AttributeDetails>> GetDepartmentDataSource(long? CompanyId, string? plantCode, List<long> plantIds)
        {
            var attributeDetails = new List<AttributeDetails>();
            try
            {
                var query = string.Empty;
                query += "select 'Department' as DropDownTypeId,t1.DepartmentID as AttributeDetailID,t1.CompanyID as CompanyId,t2.PlantCode as CompanyName, t1.Name as AttributeDetailName,t1.Description as Description from Department t1 JOIN Plant t2 ON t1.CompanyID=t2.PlantID\r\n";
                if (CompanyId > 0)
                {
                    if (plantCode == "swgp")
                    {
                        plantIds = plantIds != null && plantIds.Count() > 0 ? plantIds : new List<long>() { -1 };
                        query += "where t1.CompanyID in(" + string.Join(',', plantIds) + ")";
                    }
                    else
                    {
                        query += "Where t1.CompanyID=" + CompanyId + "\r\n";
                    }
                }
                using (var connection = CreateConnection())
                {
                    var result = (await connection.QueryAsync<AttributeDetails>(query)).ToList();
                    attributeDetails = result != null && result.Count() > 0 ? result : new List<AttributeDetails>();
                }
                return attributeDetails;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        private async Task<IReadOnlyList<AttributeDetails>> GetSectionDataSource(long? CompanyId, string? plantCode, List<long> plantIds)
        {
            var attributeDetails = new List<AttributeDetails>();
            try
            {
                var query = string.Empty;
                query += "select 'Section' as DropDownTypeId,t1.SectionID as AttributeDetailID,t2.CompanyID as CompanyId,t3.PlantCode as CompanyName, t1.Name as AttributeDetailName,CONCAT(t2.Name,'||',t1.Description) as Description from Section t1 JOIN Department t2 ON t2.DepartmentID=t1.DepartmentID JOIN Plant t3 ON t2.CompanyID=t3.PlantID\r\n";
                if (CompanyId > 0)
                {
                    if (plantCode == "swgp")
                    {
                        plantIds = plantIds != null && plantIds.Count() > 0 ? plantIds : new List<long>() { -1 };
                        query += "where t2.CompanyID in(" + string.Join(',', plantIds) + ")";
                    }
                    else
                    {
                        query += "Where t2.CompanyID=" + CompanyId + "\r\n";
                    }
                }
                using (var connection = CreateConnection())
                {
                    var result = (await connection.QueryAsync<AttributeDetails>(query)).ToList();
                    attributeDetails = result != null && result.Count() > 0 ? result : new List<AttributeDetails>();
                }
                return attributeDetails;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        private async Task<IReadOnlyList<AttributeDetails>> GetSubSectionDataSource(long? CompanyId, string? plantCode, List<long> plantIds)
        {
            var attributeDetails = new List<AttributeDetails>();
            try
            {
                var query = string.Empty;
                query += "select 'SubSection' as DropDownTypeId,t1.SectionID as AttributeDetailID,t4.CompanyID as CompanyId,t4.PlantCode as CompanyName, t1.Name as AttributeDetailName,CONCAT(t2.Name,'||',t3.Name,'||',t1.Description) as Description from SubSection t1 JOIN Section t2 ON t2.SectionID=t1.SectionID  JOIN Department t3 ON t3.DepartmentID=t2.DepartmentID  JOIN Plant t4 ON t3.CompanyID=t4.PlantID\r\n";
                if (CompanyId > 0)
                {
                    if (plantCode == "swgp")
                    {
                        plantIds = plantIds != null && plantIds.Count() > 0 ? plantIds : new List<long>() { -1 };
                        query += "where t3.CompanyID in(" + string.Join(',', plantIds) + ")";
                    }
                    else
                    {
                        query += "Where t3.CompanyID=" + CompanyId + "\r\n";
                    }
                }
                using (var connection = CreateConnection())
                {
                    var result = (await connection.QueryAsync<AttributeDetails>(query)).ToList();
                    attributeDetails = result != null && result.Count() > 0 ? result : new List<AttributeDetails>();
                }
                return attributeDetails;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        private async Task<IReadOnlyList<AttributeDetails>> GetSoCustomerDataSource(List<string?> dataSourceTableIds)
        {
            var attributeDetails = new List<AttributeDetails>();
            try
            {
                var query = string.Empty;
                dataSourceTableIds = dataSourceTableIds != null && dataSourceTableIds.Count > 0 ? dataSourceTableIds : new List<string?>() { "a" };
                query += "select Type as DropDownTypeId, SoCustomerID as AttributeDetailID,CustomerName as AttributeDetailName,Address1 as Description from SoCustomer Where type  in(" + string.Join(",", dataSourceTableIds.Select(x => string.Format("'{0}'", x.ToString().Replace("'", "''")))) + ");";

                using (var connection = CreateConnection())
                {
                    var result = (await connection.QueryAsync<AttributeDetails>(query)).ToList();
                    attributeDetails = result != null && result.Count() > 0 ? result : new List<AttributeDetails>();
                }
                return attributeDetails;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        private async Task<IReadOnlyList<AttributeDetails>> GetRawMatItemListDataSource(List<string?> dataSourceTableIds, long? CompanyId, string? plantCode, List<long> plantIds)
        {
            var attributeDetails = new List<AttributeDetails>();
            try
            {
                var query = string.Empty;
                dataSourceTableIds = dataSourceTableIds != null && dataSourceTableIds.Count > 0 ? dataSourceTableIds : new List<string?>() { "a" };
                query += "select Type as DropDownTypeId,t1.ID as AttributeDetailID,t1.CompanyId,ItemNo as AttributeDetailName,t2.PlantCode as CompanyName,CONCAT(t1.Description,(case when ISNULL(NULLIF(t1.Description2, ''), null) is NULL then  t1.Description2 ELSE  CONCAT(' | ',t1.Description2) END)) as Description from RawMatItemList t1 JOIN Plant t2 ON t1.CompanyId=t2.PlantID Where t1.type  in(" + string.Join(",", dataSourceTableIds.Select(x => string.Format("'{0}'", x.ToString().Replace("'", "''")))) + ")\n\r";
                if (CompanyId > 0)
                {
                    if (plantCode == "swgp")
                    {
                        plantIds = plantIds != null && plantIds.Count() > 0 ? plantIds : new List<long>() { -1 };
                        query += "AND t1.CompanyID in(" + string.Join(',', plantIds) + ")";
                    }
                    else
                    {
                        query += "AND t1.CompanyID=" + CompanyId + "\r\n";
                    }
                }
                using (var connection = CreateConnection())
                {
                    var result = (await connection.QueryAsync<AttributeDetails>(query)).ToList();
                    attributeDetails = result != null && result.Count() > 0 ? result : new List<AttributeDetails>();
                }
                return attributeDetails;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DataSourceAttributeDetails> GetAllDropDownDataSources()
        {
            DataSourceAttributeDetails attributeDetails = new DataSourceAttributeDetails();
            try
            {
                var query = string.Empty;
                query += "select 'Plant' as DropDownTypeId, PlantID as AttributeDetailID,PlantCode as AttributeDetailName,Description as Description from Plant;\r\n";
                query += "select 'Employee' as DropDownTypeId,t1.EmployeeID as AttributeDetailID,t1.PlantId as CompanyId,t2.PlantCode as CompanyName, t1.FirstName as AttributeDetailName,CONCAT(case when t1.NickName is NULL then  t1.FirstName ELSE  t1.NickName END,' | ',t1.LastName) as Description from Employee t1 JOIN Plant t2 ON t1.PlantID=t2.PlantID;\r\n";
                query += "select 'NavItems' as DropDownTypeId,t1.ItemId as AttributeDetailID,t1.CompanyId,No as AttributeDetailName,t2.PlantCode as CompanyName,CONCAT(t1.Description,(case when ISNULL(NULLIF(t1.Description2, ''), null) is NULL then  t1.Description2 ELSE  CONCAT(' | ',t1.Description2) END)) as Description\r\n from Navitems t1 JOIN Plant t2 ON t1.CompanyId=t2.PlantID;\r\n";
                query += "select Type as DropDownTypeId, SoCustomerID as AttributeDetailID,CustomerName as AttributeDetailName,Address1 as Description from SoCustomer;";
                using (var connection = CreateConnection())
                {
                    var result = await connection.QueryMultipleAsync(query);
                    attributeDetails.AllAttributeDetails.AddRange(result.Read<AttributeDetails>().ToList());
                    attributeDetails.AllAttributeDetails.AddRange(result.Read<AttributeDetails>().ToList());
                    attributeDetails.AllAttributeDetails.AddRange(result.Read<AttributeDetails>().ToList());
                    attributeDetails.AllAttributeDetails.AddRange(result.Read<AttributeDetails>().ToList());
                }
                return attributeDetails;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }

}
