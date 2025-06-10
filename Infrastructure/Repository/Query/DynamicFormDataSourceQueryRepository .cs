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
        public async Task<IReadOnlyList<AttributeDetails>> GetDataSourceDropDownList(long? CompanyId, List<string?> dataSourceTableIds, string? plantCode, List<long?> applicationMasterIds, List<long?> applicationMasterParentIds)
        {
            var query = string.Empty;
            int i = 0;
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
                    if (plantsData != null && plantsData.Count > 0)
                    {
                        dataSourceDropDownList.AddRange(plantsData);
                    }
                }
                if (dataSourceTableIds.Contains("NavItems"))
                {
                    i++;
                    query += GetNavItemsDataSource(CompanyId, plantCode, plantIds);
                    //dataSourceDropDownList.AddRange(await GetNavItemsDataSource(CompanyId, plantCode, plantIds));
                }
                if (dataSourceTableIds.Contains("Employee"))
                {
                    i++;
                    query += GetEmployeeDataSource(CompanyId, plantCode, plantIds);
                    // dataSourceDropDownList.AddRange(await GetEmployeeDataSource(CompanyId, plantCode, plantIds));
                }
                if (dataSourceTableIds.Contains("Division"))
                {
                    i++;
                    query += GetDivisionDataSource(CompanyId, plantCode, plantIds);
                    //dataSourceDropDownList.AddRange(await GetDivisionDataSource(CompanyId, plantCode, plantIds));
                }
                if (dataSourceTableIds.Contains("Department"))
                {
                    i++;
                    query += GetDepartmentDataSource(CompanyId, plantCode, plantIds);
                    //dataSourceDropDownList.AddRange(await GetDepartmentDataSource(CompanyId, plantCode, plantIds));
                }
                if (dataSourceTableIds.Contains("Designation"))
                {
                    i++;
                    query += GetDesignationDataSource(CompanyId, plantCode, plantIds);
                    //dataSourceDropDownList.AddRange(await GetDepartmentDataSource(CompanyId, plantCode, plantIds));
                }
                if (dataSourceTableIds.Contains("Section"))
                {
                    i++;
                    query += GetSectionDataSource(CompanyId, plantCode, plantIds);
                    //dataSourceDropDownList.AddRange(await GetSectionDataSource(CompanyId, plantCode, plantIds));
                }
                if (dataSourceTableIds.Contains("SubSection"))
                {
                    i++;
                    query += GetSubSectionDataSource(CompanyId, plantCode, plantIds);
                    //dataSourceDropDownList.AddRange(await GetSubSectionDataSource(CompanyId, plantCode, plantIds));
                }
                if (dataSourceTableIds.Contains("ItemBatchInfo"))
                {
                    i++;
                    query += GetItemBatchInfoDataSource(CompanyId, plantCode, plantIds);
                    //dataSourceDropDownList.AddRange(await GetItemBatchInfoDataSource(CompanyId, plantCode, plantIds));
                }
                if (dataSourceTableIds.Contains("FinishedProdOrderLine"))
                {
                    i++;
                    query += GetFinishedProdOrderLineDataSource(CompanyId, plantCode, plantIds);
                    //dataSourceDropDownList.AddRange(await GetFinishedProdOrderLineDataSource(CompanyId, plantCode, plantIds));
                }
                if (dataSourceTableIds.Contains("Vendor"))
                {
                    i++;
                    query += GetNavVendorDataSource(CompanyId, plantCode, plantIds);
                }
                if (dataSourceTableIds.Contains("RawMatPurch"))
                {
                    i++;
                    query += GetRawMatPurchDataSource(CompanyId, plantCode, plantIds);
                }
                if (dataSourceTableIds.Contains("ReleaseProdOrderLine"))
                {
                    i++;
                    query += GetReleaseProdOrderLineDataSource(CompanyId, plantCode, plantIds);
                }
                if (dataSourceTableIds.Contains("AllProdOrderLine"))
                {
                    i++;
                    query += GetAllProdOrderLineDataSource(CompanyId, plantCode, plantIds);
                }
                var soCustomerType = new List<string?>() { "Clinic", "Customer" };
                var soCustomerList = soCustomerType.Intersect(dataSourceTableIds).ToList();
                if (soCustomerList.Count() > 0)
                {
                    i++;
                    query += GetSoCustomerDataSource(soCustomerList);
                    //dataSourceDropDownList.AddRange(await GetSoCustomerDataSource(soCustomerList));
                }
                var rawMatItemType = new List<string?>() { "RawMatItem", "PackagingItem", "ProcessItem" };
                var rawMatItemList = rawMatItemType.Intersect(dataSourceTableIds).ToList();
                if (rawMatItemList.Count() > 0)
                {
                    i++;
                    query += GetRawMatItemListDataSource(rawMatItemList, CompanyId, plantCode, plantIds);
                    //dataSourceDropDownList.AddRange(await GetRawMatItemListDataSource(rawMatItemList, CompanyId, plantCode, plantIds));
                }

                if (dataSourceTableIds.Contains("ApplicationMaster") && applicationMasterIds.Count > 0)
                {
                    i++;
                    query += GetApplicationMasterDataSource(applicationMasterIds);
                    //dataSourceDropDownList.AddRange(await GetApplicationMasterDataSource(applicationMasterIds));
                }
                if (dataSourceTableIds.Contains("ApplicationMasterParent") && applicationMasterParentIds.Count > 0)
                {
                    i++;
                    query += GetApplicationMasterParentDataSource(applicationMasterParentIds);
                    //dataSourceDropDownList.AddRange(await GetApplicationMasterParentDataSource(applicationMasterParentIds));
                }
                if (dataSourceTableIds.Contains("Site"))
                {
                    i++;
                    query += GetSiteDataSource(CompanyId, plantCode, plantIds);
                    //dataSourceDropDownList.AddRange(await GetSiteDataSource(CompanyId, plantCode, plantIds));
                }
                if (dataSourceTableIds.Contains("Zone"))
                {
                    i++;
                    query += GetZoneDataSource(CompanyId, plantCode, plantIds);
                    //dataSourceDropDownList.AddRange(await GetZoneDataSource(CompanyId, plantCode, plantIds));
                }
                if (dataSourceTableIds.Contains("Location"))
                {
                    i++;
                    query += GetLocationDataSource(CompanyId, plantCode, plantIds);
                    //dataSourceDropDownList.AddRange(await GetLocationDataSource(CompanyId, plantCode, plantIds));
                }
                if (dataSourceTableIds.Contains("Area"))
                {
                    i++;
                    query += GetAreaDataSource(CompanyId, plantCode, plantIds);
                    //dataSourceDropDownList.AddRange(await GetAreaDataSource(CompanyId, plantCode, plantIds));
                }
                if (dataSourceTableIds.Contains("SpecificArea"))
                {
                    i++;
                    query += GetSpecificAreaDataSource(CompanyId, plantCode, plantIds);
                    //dataSourceDropDownList.AddRange(await GetSpecificAreaDataSource(CompanyId, plantCode, plantIds));
                }
                if (!string.IsNullOrEmpty(query))
                {
                    if (!string.IsNullOrEmpty(query))
                    {
                        dataSourceDropDownList.AddRange(await GetAllDataSource(query, i));
                    }
                }
            }
            else
            {
                dataSourceDropDownList.AddRange(plantsData);
                //dataSourceDropDownList.AddRange(await GetNavItemsDataSource(CompanyId, plantCode, plantIds));
                //dataSourceDropDownList.AddRange(await GetEmployeeDataSource(CompanyId, plantCode, plantIds));
                //dataSourceDropDownList.AddRange(await GetDivisionDataSource(CompanyId, plantCode, plantIds));
                //dataSourceDropDownList.AddRange(await GetDepartmentDataSource(CompanyId, plantCode, plantIds));
                //dataSourceDropDownList.AddRange(await GetSectionDataSource(CompanyId, plantCode, plantIds));
                //dataSourceDropDownList.AddRange(await GetSubSectionDataSource(CompanyId, plantCode, plantIds));
                //dataSourceDropDownList.AddRange(await GetSiteDataSource(CompanyId, plantCode, plantIds));
                //dataSourceDropDownList.AddRange(await GetZoneDataSource(CompanyId, plantCode, plantIds));
                //dataSourceDropDownList.AddRange(await GetLocationDataSource(CompanyId, plantCode, plantIds));
                //dataSourceDropDownList.AddRange(await GetAreaDataSource(CompanyId, plantCode, plantIds));
                //dataSourceDropDownList.AddRange(await GetSpecificAreaDataSource(CompanyId, plantCode, plantIds));
                //dataSourceDropDownList.AddRange(await GetItemBatchInfoDataSource(CompanyId, plantCode, plantIds));
                //var soCustomerType = new List<string?>() { "Clinic", "Vendor", "Customer" };
                //var soCustomerList = soCustomerType.Intersect(dataSourceTableIds).ToList();
                //if (soCustomerList.Count() > 0)
                //{
                //    dataSourceDropDownList.AddRange(await GetSoCustomerDataSource(soCustomerList));
                //}
                //var rawMatItemType = new List<string?>() { "RawMatItem", "PackagingItem", "ProcessItem" };
                //var rawMatItemList = rawMatItemType.Intersect(dataSourceTableIds).ToList();
                //if (rawMatItemList.Count() > 0)
                //{
                //    dataSourceDropDownList.AddRange(await GetRawMatItemListDataSource(rawMatItemList, CompanyId, plantCode, plantIds));
                //}
                //if (applicationMasterIds.Count > 0)
                //{
                //    dataSourceDropDownList.AddRange(await GetApplicationMasterDataSource(applicationMasterIds));
                //}
                //if (dataSourceTableIds.Contains("ApplicationMasterParent") && applicationMasterParentIds.Count > 0)
                //{
                //    dataSourceDropDownList.AddRange(await GetApplicationMasterParentDataSource(applicationMasterParentIds));
                //}
            }
            return dataSourceDropDownList;
        }
        private async Task<IReadOnlyList<AttributeDetails>> GetAllDataSource(string query, int i)
        {
            var attributeDetails = new List<AttributeDetails>();
            try
            {
                using (var connection = CreateConnection())
                {
                    var result = (await connection.QueryMultipleAsync(query));
                    for (int j = 0; j < i; j++)
                    {
                        var results = result.Read<AttributeDetails>().ToList();
                        var attributeDetailss = results != null && results.Count() > 0 ? results : new List<AttributeDetails>();
                        attributeDetails.AddRange(attributeDetailss);
                    }
                    //attributeDetails = result != null && result.Count() > 0 ? result : new List<AttributeDetails>();
                }
                return attributeDetails;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        private string GetApplicationMasterDataSource(List<long?> applicationMasterIds)
        {
            // var attributeDetails = new List<AttributeDetails>();
            try
            {
                var query = string.Empty;
                query += "select CONCAT('ApplicationMaster_',t1.ApplicationMasterDetailID) as AttributeDetailNameId,CONCAT(t1.Value,'|',t1.Description) as FullName,'ApplicationMaster' as DropDownTypeId,t1.ApplicationMasterId, t1.ApplicationMasterDetailID as AttributeDetailID,Value as AttributeDetailName,t1.Description,t2.ApplicationMasterName,t2.ApplicationMasterCodeID as ApplicationMasterCodeID from ApplicationMasterDetail t1 JOIN ApplicationMaster t2 ON t1.ApplicationMasterID=t2.ApplicationMasterID\r\n";
                if (applicationMasterIds != null && applicationMasterIds.Count > 0)
                {
                    applicationMasterIds = applicationMasterIds != null && applicationMasterIds.Count() > 0 ? applicationMasterIds : new List<long?>() { -1 };
                    query += "where (t1.StatusCodeID=1 OR t1.StatusCodeID IS Null) AND t1.ApplicationMasterId in(" + string.Join(',', applicationMasterIds) + ");";
                }
                else
                {
                    query += "(t1.StatusCodeID=1 OR t1.StatusCodeID IS Null);";
                }
                //using (var connection = CreateConnection())
                //{
                //    var result = (await connection.QueryAsync<AttributeDetails>(query)).ToList();
                //    attributeDetails = result != null && result.Count() > 0 ? result : new List<AttributeDetails>();
                //}
                //return attributeDetails;
                return query;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        private string GetApplicationMasterParentDataSource(List<long?> applicationMasterIds)
        {
            // var attributeDetails = new List<AttributeDetails>();
            try
            {
                var query = string.Empty;
                if (applicationMasterIds != null && applicationMasterIds.Count > 0)
                {
                    applicationMasterIds = applicationMasterIds != null && applicationMasterIds.Count() > 0 ? applicationMasterIds : new List<long?>() { -1 };
                    query += "WITH ApplicationMasterParent_cte AS (\r\n  SELECT\r\n    ApplicationMasterParentID,\r\n    ApplicationMasterName,\r\n    ApplicationMasterParentCodeID,\r\n    ParentID,\r\n    1 AS level\r\n  FROM ApplicationMasterParent\r\n  WHERE ApplicationMasterParentCodeID in(" + string.Join(',', applicationMasterIds) + ")\r\n  UNION ALL\r\n  SELECT\r\n    e.ApplicationMasterParentID,\r\n    e.ApplicationMasterName,\r\n    e.ApplicationMasterParentCodeID,\r\n    e.ParentID,\r\n    level + 1\r\n  FROM ApplicationMasterParent e\r\n  INNER JOIN ApplicationMasterParent_cte r\r\n    ON e.ParentID = r.ApplicationMasterParentCodeID\r\n)select CONCAT('ApplicationMasterParent_',t1.ApplicationMasterChildID) as AttributeDetailNameId,'ApplicationMasterParent' as DropDownTypeId,t1.ApplicationMasterChildID as AttributeDetailID,t1.Value as AttributeDetailName,t1.Description,t1.ApplicationMasterParentID as ApplicationMasterParentCodeId,t2.ApplicationMasterName as ApplicationMasterName,t1.ParentId,t3.Value as ParentName from ApplicationMasterChild  t1 JOIN ApplicationMasterParent t2 ON t1.ApplicationMasterParentID=t2.ApplicationMasterParentCodeID LEFT JOIN ApplicationMasterChild t3 ON t1.ParentID=t3.ApplicationMasterChildID where t1.StatusCodeID=1 AND t1.ApplicationMasterParentID in(SELECT ApplicationMasterParentCodeID FROM ApplicationMasterParent_cte);";
                }
                else
                {
                    query += "select CONCAT('ApplicationMasterParent_',t1.ApplicationMasterChildID) as AttributeDetailNameId,'ApplicationMasterParent' as DropDownTypeId,t1.ApplicationMasterChildID as AttributeDetailID,t1.Value as AttributeDetailName,t1.Description,t1.ApplicationMasterParentID as ApplicationMasterParentCodeId,t2.ApplicationMasterName as ApplicationMasterName,t1.ParentId,t3.Value as ParentName from ApplicationMasterChild  t1 JOIN ApplicationMasterParent t2 ON t1.ApplicationMasterParentID=t2.ApplicationMasterParentCodeID LEFT JOIN ApplicationMasterChild t3 ON t1.ParentID=t3.ApplicationMasterChildID where t1.StatusCodeID=1;";
                }
                //using (var connection = CreateConnection())
                //{
                //    var result = (await connection.QueryAsync<AttributeDetails>(query)).ToList();
                //    attributeDetails = result != null && result.Count() > 0 ? result : new List<AttributeDetails>();
                //}
                //return attributeDetails;
                return query;
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
                query += "select CONCAT('Plant_',PlantID) as AttributeDetailNameId,'Plant' as DropDownTypeId, PlantID as AttributeDetailID,PlantCode as AttributeDetailName,Description as Description from Plant;\r\n";
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
        private string GetNavItemsDataSource(long? CompanyId, string? plantCode, List<long> plantIds)
        {
            //var attributeDetails = new List<AttributeDetails>();
            try
            {
                var query = string.Empty;
                query += "select CONCAT('NavItems_',t1.ItemId) as AttributeDetailNameId,'NavItems' as DropDownTypeId,t1.ItemId as AttributeDetailID,t1.CompanyId,No as AttributeDetailName,t2.PlantCode as CompanyName,CONCAT(t1.Description,(case when ISNULL(NULLIF(t1.Description2, ''), null) is NULL then  t1.Description2 ELSE  CONCAT(' | ',t1.Description2) END)) as Description\r\n from Navitems t1 JOIN Plant t2 ON t1.CompanyId=t2.PlantID\r\n";
                if (CompanyId > 0)
                {
                    if (plantCode == "swgp")
                    {
                        plantIds = plantIds != null && plantIds.Count() > 0 ? plantIds : new List<long>() { -1 };
                        query += "where t1.no like 'FP%' AND t1.CompanyId in(" + string.Join(',', plantIds) + ");";
                    }
                    else
                    {
                        query += "Where t1.no like 'FP%' AND t1.CompanyId=" + CompanyId + ";\r\n";
                    }
                }
                else
                {
                    query += "Where t1.no like 'FP%';\r\n";
                }
                return query;
                //using (var connection = CreateConnection())
                //{
                //    var result = (await connection.QueryAsync<AttributeDetails>(query)).ToList();
                //    attributeDetails = result != null && result.Count() > 0 ? result : new List<AttributeDetails>();
                //}
                //return attributeDetails;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        private string GetEmployeeDataSource(long? CompanyId, string? plantCode, List<long> plantIds)
        {
            //var attributeDetails = new List<AttributeDetails>();
            try
            {
                var query = string.Empty;
                query += "select CONCAT('Employee_',t1.EmployeeID) as AttributeDetailNameId,'Employee' as DropDownTypeId,t1.EmployeeID as AttributeDetailID,t1.PlantId as CompanyId,t2.PlantCode as CompanyName, t1.FirstName as AttributeDetailName,CONCAT(case when t1.NickName is NULL then  t1.FirstName ELSE  t1.NickName END,' | ',t1.LastName) as Description,t3.Name as DesignationName from Employee t1 JOIN Plant t2 ON t1.PlantID=t2.PlantID LEFT JOIN Designation t3 ON t3.DesignationID=t1.DesignationID LEFT JOIN ApplicationMasterDetail ag ON ag.ApplicationMasterDetailID = t1.AcceptanceStatus\r\n";
                if (CompanyId > 0)
                {
                    if (plantCode == "swgp")
                    {
                        plantIds = plantIds != null && plantIds.Count() > 0 ? plantIds : new List<long>() { -1 };
                        query += "where (ag.Value!='Resign' or ag.Value is null) AND t1.PlantId in(" + string.Join(',', plantIds) + ");";
                    }
                    else
                    {
                        query += "Where (ag.Value!='Resign' or ag.Value is null) AND t1.PlantId=" + CompanyId + ";\r\n";
                    }
                }
                else
                {
                    query += "WHERE (ag.Value!='Resign' or ag.Value is null);\r\n";
                }
                //using (var connection = CreateConnection())
                //{
                //    var result = (await connection.QueryAsync<AttributeDetails>(query)).ToList();
                //    attributeDetails = result != null && result.Count() > 0 ? result : new List<AttributeDetails>();
                //}
                //return attributeDetails;
                return query;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        private string GetDivisionDataSource(long? CompanyId, string? plantCode, List<long> plantIds)
        {
            //var attributeDetails = new List<AttributeDetails>();
            try
            {
                var query = string.Empty;
                query += "select  CONCAT('Division_',t1.DivisionID) as AttributeDetailNameId,'Division' as DropDownTypeId,t1.DivisionID as AttributeDetailID,t1.CompanyID as CompanyId,t2.PlantCode as CompanyName, t1.Name as AttributeDetailName,t1.Description as Description from Division t1 JOIN Plant t2 ON t1.CompanyID=t2.PlantID\r\n";
                if (CompanyId > 0)
                {
                    if (plantCode == "swgp")
                    {
                        plantIds = plantIds != null && plantIds.Count() > 0 ? plantIds : new List<long>() { -1 };
                        query += "where t1.CompanyID in(" + string.Join(',', plantIds) + ");";
                    }
                    else
                    {
                        query += "Where t1.CompanyID=" + CompanyId + ";\r\n";
                    }
                }
                else
                {
                    query += ";\r\n";
                }
                //using (var connection = CreateConnection())
                //{
                //    var result = (await connection.QueryAsync<AttributeDetails>(query)).ToList();
                //    attributeDetails = result != null && result.Count() > 0 ? result : new List<AttributeDetails>();
                //}
                //return attributeDetails;
                return query;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        private string GetDepartmentDataSource(long? CompanyId, string? plantCode, List<long> plantIds)
        {
            // var attributeDetails = new List<AttributeDetails>();
            try
            {
                var query = string.Empty;
                query += "select CONCAT('Department_',t1.DepartmentID) as AttributeDetailNameId,'Department' as DropDownTypeId,t1.DepartmentID as AttributeDetailID,t1.CompanyID as CompanyId,t2.PlantCode as CompanyName, t1.Name as AttributeDetailName,t1.Description as Description from Department t1 JOIN Plant t2 ON t1.CompanyID=t2.PlantID\r\n";
                if (CompanyId > 0)
                {
                    if (plantCode == "swgp")
                    {
                        plantIds = plantIds != null && plantIds.Count() > 0 ? plantIds : new List<long>() { -1 };
                        query += "where t1.CompanyID in(" + string.Join(',', plantIds) + ");";
                    }
                    else
                    {
                        query += "Where t1.CompanyID=" + CompanyId + ";\r\n";
                    }
                }
                else
                {
                    query += ";\r\n";
                }
                //using (var connection = CreateConnection())
                //{
                //    var result = (await connection.QueryAsync<AttributeDetails>(query)).ToList();
                //    attributeDetails = result != null && result.Count() > 0 ? result : new List<AttributeDetails>();
                //}
                //return attributeDetails;
                return query;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        private string GetDesignationDataSource(long? CompanyId, string? plantCode, List<long> plantIds)
        {
            // var attributeDetails = new List<AttributeDetails>();
            try
            {
                var query = string.Empty;
                query += "select CONCAT('Designation_',t1.DesignationID) as AttributeDetailNameId,'Designation' as DropDownTypeId,t1.DesignationID as AttributeDetailID,t1.CompanyID as CompanyId,t3.PlantCode as CompanyName, t1.Name as AttributeDetailName,CONCAT(t2.Name,'||',t1.Description) as Description from Designation t1 JOIN SubSection t2 ON t2.SubSectionID=t1.SubSectionTID JOIN Plant t3 ON t1.CompanyID=t3.PlantID\r\n";
                if (CompanyId > 0)
                {
                    if (plantCode == "swgp")
                    {
                        plantIds = plantIds != null && plantIds.Count() > 0 ? plantIds : new List<long>() { -1 };
                        query += "where t1.CompanyID in(" + string.Join(',', plantIds) + ");";
                    }
                    else
                    {
                        query += "Where t1.CompanyID=" + CompanyId + ";\r\n";
                    }
                }
                else
                {
                    query += ";\r\n";
                }
                //using (var connection = CreateConnection())
                //{
                //    var result = (await connection.QueryAsync<AttributeDetails>(query)).ToList();
                //    attributeDetails = result != null && result.Count() > 0 ? result : new List<AttributeDetails>();
                //}
                //return attributeDetails;
                return query;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        private string GetSectionDataSource(long? CompanyId, string? plantCode, List<long> plantIds)
        {
            //var attributeDetails = new List<AttributeDetails>();
            try
            {
                var query = string.Empty;
                query += "select CONCAT('Section_',t1.SectionID) as AttributeDetailNameId,'Section' as DropDownTypeId,t1.SectionID as AttributeDetailID,t2.CompanyID as CompanyId,t3.PlantCode as CompanyName, t1.Name as AttributeDetailName,CONCAT(t2.Name,'||',t1.Description) as Description from Section t1 JOIN Department t2 ON t2.DepartmentID=t1.DepartmentID JOIN Plant t3 ON t2.CompanyID=t3.PlantID\r\n";
                if (CompanyId > 0)
                {
                    if (plantCode == "swgp")
                    {
                        plantIds = plantIds != null && plantIds.Count() > 0 ? plantIds : new List<long>() { -1 };
                        query += "where t2.CompanyID in(" + string.Join(',', plantIds) + ");";
                    }
                    else
                    {
                        query += "Where t2.CompanyID=" + CompanyId + ";\r\n";
                    }
                }
                else
                {
                    query += ";\r\n";
                }
                //using (var connection = CreateConnection())
                //{
                //    var result = (await connection.QueryAsync<AttributeDetails>(query)).ToList();
                //    attributeDetails = result != null && result.Count() > 0 ? result : new List<AttributeDetails>();
                //}
                //return attributeDetails;
                return query;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        private string GetSubSectionDataSource(long? CompanyId, string? plantCode, List<long> plantIds)
        {
            // var attributeDetails = new List<AttributeDetails>();
            try
            {
                var query = string.Empty;
                query += "select CONCAT('SubSection_',t1.SubSectionID) as AttributeDetailNameId,'SubSection' as DropDownTypeId,t1.SubSectionID as AttributeDetailID,t4.CompanyID as CompanyId,t4.PlantCode as CompanyName, t1.Name as AttributeDetailName,CONCAT(t2.Name,'||',t3.Name,'||',t1.Description) as Description from SubSection t1 JOIN Section t2 ON t2.SectionID=t1.SectionID  JOIN Department t3 ON t3.DepartmentID=t2.DepartmentID  JOIN Plant t4 ON t3.CompanyID=t4.PlantID\r\n";
                if (CompanyId > 0)
                {
                    if (plantCode == "swgp")
                    {
                        plantIds = plantIds != null && plantIds.Count() > 0 ? plantIds : new List<long>() { -1 };
                        query += "where t3.CompanyID in(" + string.Join(',', plantIds) + ");";
                    }
                    else
                    {
                        query += "Where t3.CompanyID=" + CompanyId + ";\r\n";
                    }
                }
                else
                {
                    query += ";\r\n";
                }
                //using (var connection = CreateConnection())
                //{
                //    var result = (await connection.QueryAsync<AttributeDetails>(query)).ToList();
                //    attributeDetails = result != null && result.Count() > 0 ? result : new List<AttributeDetails>();
                //}
                //return attributeDetails;
                return query;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        private string GetItemBatchInfoDataSource(long? CompanyId, string? plantCode, List<long> plantIds)
        {
            //var attributeDetails = new List<AttributeDetails>();
            try
            {
                var query1 = string.Empty;
                if (CompanyId > 0)
                {
                    if (plantCode == "swgp")
                    {
                        plantIds = plantIds != null && plantIds.Count() > 0 ? plantIds : new List<long>() { -1 };
                        query1 += "Where b1.CompanyID in(" + string.Join(',', plantIds) + ")\r";
                    }
                    else
                    {
                        query1 += "Where b1.CompanyID=" + CompanyId + "\r";
                    }
                }
                var queryIn = "\r\nselect \r\n(case when (SUBSTRING(bb2.ProductName, 0, CHARINDEX(',', bb2.ProductName)))='' then  bb2.ProductName ELSE  (SUBSTRING(bb2.ProductName, 0, CHARINDEX(',', bb2.ProductName))) END) as ItemBatchNoId\r\nfrom\r\n(select bb1.*,\r\nProductName = STUFF(( SELECT ',' + CAST(md.ItemBatchId AS VARCHAR(MAX)) FROM ItemBatchInfo md   WHERE md.CompanyID=bb1.CompanyId AND md.BatchNo=bb1.BatchNo   Order by md.ItemBatchId asc FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, '')\r\nfrom(select b1.BatchNo,b1.CompanyId from ItemBatchInfo b1  " + query1 + " group by b1.BatchNo,b1.CompanyId)bb1) bb2\r\n\r\n";
                var query = string.Empty;
                query += "select CONCAT('ItemBatchInfo_',t1.ItemBatchId) as AttributeDetailNameId,'ItemBatchInfo' as DropDownTypeId,t1.ItemBatchId as AttributeDetailID,t1.CompanyID as CompanyId,t2.PlantCode as CompanyName, t1.BatchNo as AttributeDetailName,CONCAT(t3.No,'||',t3.Description) as Description from ItemBatchInfo t1 JOIN Plant t2 ON t1.CompanyId=t2.PlantID JOIN NavItems t3 ON t3.ItemId=t1.ItemId\r\n";
                if (CompanyId > 0)
                {
                    if (plantCode == "swgp")
                    {
                        plantIds = plantIds != null && plantIds.Count() > 0 ? plantIds : new List<long>() { -1 };
                        query += "where t1.ItemBatchId IN(" + queryIn + ") AND t1.CompanyID in(" + string.Join(',', plantIds) + ");";
                    }
                    else
                    {
                        query += "Where t1.ItemBatchId IN(" + queryIn + ") AND t1.CompanyID=" + CompanyId + ";\r\n";
                    }
                }
                else
                {
                    query += "Where t1.ItemBatchId IN(" + queryIn + ");\r\n";
                }
                //using (var connection = CreateConnection())
                //{
                //    var result = (await connection.QueryAsync<AttributeDetails>(query)).ToList();
                //    attributeDetails = result != null && result.Count() > 0 ? result : new List<AttributeDetails>();
                //}
                //return attributeDetails;
                return query;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        private string GetFinishedProdOrderLineProductionInProgressDataSource(long? CompanyId, string? plantCode, List<long> plantIds)
        {
            //var attributeDetails = new List<AttributeDetails>();
            try
            {
                var query = string.Empty;
                query += "select CONCAT('FinishedProdOrderLineProductionInProgress_',t1.FinishedProdOrderLineID) as AttributeDetailNameId,'FinishedProdOrderLineProductionInProgress' as DropDownTypeId,t1.OptStatus,\r\nt1.FinishedProdOrderLineID as AttributeDetailID,t1.CompanyID as CompanyId,t2.PlantCode as CompanyName, t1.ItemNo as AttributeDetailName,\r\nt1.Description,t1.Description2,t1.ReplanRefNo,t1.BatchNo,FORMAT(t1.StartingDate, 'dd-MMM-yyyy') as StartingDate,FORMAT(t1.ExpirationDate, 'dd-MMM-yyyy') as ExpirationDate,FORMAT(t1.ManufacturingDate, 'dd-MMM-yyyy') as ManufacturingDate,t1.ProductCode,t1.ProductName,CONCAT(t1.ItemNo,'|',t1.Description,'|',t1.Description2,'|',t1.ReplanRefNo) as NameList  from FinishedProdOrderLine t1 \r\nJOIN Plant t2 ON t1.CompanyId=t2.PlantID\n\r";
                if (CompanyId > 0)
                {
                    if (plantCode == "swgp")
                    {
                        plantIds = plantIds != null && plantIds.Count() > 0 ? plantIds : new List<long>() { -1 };
                        query += "where t1.OptStatus='Production In Progress'  AND CAST(t1.StartingDate as DATE)>'05-31-2023' AND (t1.BatchNo is not null AND t1.BatchNo!='') AND t1.CompanyID in(" + string.Join(',', plantIds) + ");";
                    }
                    else
                    {
                        query += "Where t1.OptStatus='Production In Progress'   AND CAST(t1.StartingDate as DATE)>'05-31-2023' AND (t1.BatchNo is not null AND t1.BatchNo!='') AND t1.CompanyID=" + CompanyId + ";\r\n";
                    }
                }
                else
                {
                    query += "\n\rWhere t1.OptStatus='Production In Progress'  AND CAST(t1.StartingDate as DATE)>'05-31-2023' AND (t1.BatchNo is not null AND t1.BatchNo!='');\r\n";
                }
                //using (var connection = CreateConnection())
                //{
                //    var result = (await connection.QueryAsync<AttributeDetails>(query)).ToList();
                //    attributeDetails = result != null && result.Count() > 0 ? result : new List<AttributeDetails>();
                //}
                //return attributeDetails;
                return query;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        private string GetFinishedProdOrderLineDataSource(long? CompanyId, string? plantCode, List<long> plantIds)
        {
            //var attributeDetails = new List<AttributeDetails>();
            try
            {
                var query = string.Empty;
                query += "select CONCAT('FinishedProdOrderLine_',t1.FinishedProdOrderLineID) as AttributeDetailNameId,'FinishedProdOrderLine' as DropDownTypeId,t1.OptStatus,\r\nt1.FinishedProdOrderLineID as AttributeDetailID,t1.CompanyID as CompanyId,t2.PlantCode as CompanyName, t1.ItemNo as AttributeDetailName,\r\nt1.Description,t1.Description2,t1.ReplanRefNo,t1.BatchNo,FORMAT(t1.StartingDate, 'dd-MMM-yyyy') as StartingDate,FORMAT(t1.ExpirationDate, 'dd-MMM-yyyy') as ExpirationDate,FORMAT(t1.ManufacturingDate, 'dd-MMM-yyyy') as ManufacturingDate,t1.ProductCode,t1.ProductName,CONCAT(t1.ItemNo,'|',t1.Description,'|',t1.Description2,'|',t1.ReplanRefNo) as NameList  from FinishedProdOrderLine t1 \r\nJOIN Plant t2 ON t1.CompanyId=t2.PlantID\n\r";
                if (CompanyId > 0)
                {
                    if (plantCode == "swgp")
                    {
                        plantIds = plantIds != null && plantIds.Count() > 0 ? plantIds : new List<long>() { -1 };
                        query += "where (t1.OptStatus='quarantine' OR t1.OptStatus='' OR t1.OptStatus is null) AND (t1.BatchNo is not null AND t1.BatchNo!='')  AND CAST(t1.StartingDate as DATE)>'05-31-2023' AND t1.CompanyID in(" + string.Join(',', plantIds) + ");";
                    }
                    else
                    {
                        query += "Where (t1.OptStatus='quarantine' OR t1.OptStatus='' OR t1.OptStatus is null) AND (t1.BatchNo is not null AND t1.BatchNo!='')  AND CAST(t1.StartingDate as DATE)>'05-31-2023' AND t1.CompanyID=" + CompanyId + ";\r\n";
                    }
                }
                else
                {
                    query += "\n\rWhere (t1.OptStatus='quarantine' OR t1.OptStatus='' OR t1.OptStatus is null)  AND CAST(t1.StartingDate as DATE)>'05-31-2023'  AND (t1.BatchNo is not null AND t1.BatchNo!='');\r\n";
                }
                //using (var connection = CreateConnection())
                //{
                //    var result = (await connection.QueryAsync<AttributeDetails>(query)).ToList();
                //    attributeDetails = result != null && result.Count() > 0 ? result : new List<AttributeDetails>();
                //}
                //return attributeDetails;
                return query;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        private string GetSoCustomerDataSource(List<string?> dataSourceTableIds)
        {
            //var attributeDetails = new List<AttributeDetails>();
            try
            {
                var query = string.Empty;
                dataSourceTableIds = dataSourceTableIds != null && dataSourceTableIds.Count > 0 ? dataSourceTableIds : new List<string?>() { "a" };
                query += "select CONCAT(Type,'_',t1.SoCustomerID) as AttributeDetailNameId,t1.Type as DropDownTypeId, t1.SoCustomerID as AttributeDetailID,t1.CustomerName as AttributeDetailName,t1.Address1 as Description,t2.PlantCode as CompanyName from SoCustomer t1 LEFT JOIN Plant t2 ON t1.CompanyId=t2.PlantID Where t1.type  in(" + string.Join(",", dataSourceTableIds.Select(x => string.Format("'{0}'", x.ToString().Replace("'", "''")))) + ");";
                return query;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        private string GetNavVendorDataSource(long? CompanyId, string? plantCode, List<long> plantIds)
        {
            try
            {
                var query = string.Empty;
                query += "select CONCAT(Type,'_',t1.SoCustomerID) as AttributeDetailNameId,t1.Type as DropDownTypeId, t1.SoCustomerID as AttributeDetailID,t1.CustomerName as AttributeDetailName,t1.Address1 as Description,t1.CompanyID as CompanyId,t2.PlantCode as CompanyName from SoCustomer t1 LEFT JOIN Plant t2 ON t1.CompanyId=t2.PlantID  Where t1.type  in('Vendor')\n\r";
                if (CompanyId > 0)
                {
                    if (plantCode == "swgp")
                    {
                        plantIds = plantIds != null && plantIds.Count() > 0 ? plantIds : new List<long>() { -1 };
                        query += "AND t2.CompanyID in(" + string.Join(',', plantIds) + ");";
                    }
                    else
                    {
                        query += "AND t2.CompanyID=" + CompanyId + ";\r\n";
                    }
                }
                else
                {
                    query += ";\r\n";
                }
                return query;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        private string GetRawMatPurchDataSource(long? CompanyId, string? plantCode, List<long> plantIds)
        {
            try
            {
                var query = string.Empty;
                query += "select CONCAT('RawMatPurch','_',t1.RawMatPurchID) as AttributeDetailNameId,'RawMatPurch' as DropDownTypeId,t1.QcRefNo ,t1.RawMatPurchID as AttributeDetailID,t1.ItemNo as AttributeDetailName,t1.Description as Description,t1.CompanyID as CompanyId,t2.PlantCode as CompanyName,t1.BatchNo,FORMAT(t1.ExpirationDate, 'dd-MMM-yyyy') as ExpirationDate,FORMAT(t1.ManufacturingDate, 'dd-MMM-yyyy') as ManufacturingDate from RawMatPurch t1 LEFT JOIN Plant t2 ON t1.CompanyId=t2.PlantID\n\r";
                if (CompanyId > 0)
                {
                    if (plantCode == "swgp")
                    {
                        plantIds = plantIds != null && plantIds.Count() > 0 ? plantIds : new List<long>() { -1 };
                        query += "AND t2.CompanyID in(" + string.Join(',', plantIds) + ");";
                    }
                    else
                    {
                        query += "AND t2.CompanyID=" + CompanyId + ";\r\n";
                    }
                }
                else
                {
                    query += ";\r\n";
                }
                return query;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        private string GetReleaseProdOrderLineDataSource(long? CompanyId, string? plantCode, List<long> plantIds)
        {
            try
            {
                var query = string.Empty;
                query += "select CONCAT('ReleaseProdOrderLine','_',t1.ReleaseProdOrderLineId) as AttributeDetailNameId,'ReleaseProdOrderLine' as DropDownTypeId, t1.ReleaseProdOrderLineId as AttributeDetailID,t1.ItemNo as AttributeDetailName,t1.Description as Description,t1.Description2 as Description2,t1.ReplanRefNo,t1.CompanyID as CompanyId,t2.PlantCode as CompanyName,t1.BatchNo,t1.BatchSize,t1.ProdOrderNo,t1.UnitOfMeasureCode,t1.Status,t1.SubStatus,FORMAT(t1.StartingDate, 'dd-MMM-yyyy') as StartingDate from ReleaseProdOrderLine t1 LEFT JOIN Plant t2 ON t1.CompanyId=t2.PlantID\r\n";
                if (CompanyId > 0)
                {
                    if (plantCode == "swgp")
                    {
                        plantIds = plantIds != null && plantIds.Count() > 0 ? plantIds : new List<long>() { -1 };
                        query += "where CAST(t1.StartingDate as DATE)>='01-01-2020' AND t1.Status IN('Released','Finished') AND t2.CompanyID in(" + string.Join(',', plantIds) + ");";
                    }
                    else
                    {
                        query += "where CAST(t1.StartingDate as DATE)>='01-01-2020' AND t1.Status IN('Released','Finished') AND t2.CompanyID=" + CompanyId + ";\r\n";
                    }
                }
                else
                {
                    query += "where CAST(t1.StartingDate as DATE)>='01-01-2020' AND t1.Status IN('Released','Finished')\r\n";
                }
                return query;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        private string GetAllProdOrderLineDataSource(long? CompanyId, string? plantCode, List<long> plantIds)
        {
            try
            {
                var query = string.Empty;
                query += "select CONCAT('AllProdOrderLine','_',t1.AllProdOrderLineId) as AttributeDetailNameId,'AllProdOrderLine' as DropDownTypeId, t1.AllProdOrderLineId as AttributeDetailID,t1.ItemNo as AttributeDetailName,t1.Description as Description,t1.Description2 as Description2,t1.ReplanRefNo,t1.CompanyID as CompanyId,t2.PlantCode as CompanyName,t1.BatchNo,t1.BatchSize,t1.ProdOrderNo,t1.UnitOfMeasureCode,t1.Status,t1.SubStatus,FORMAT(t1.StartingDate, 'dd-MMM-yyyy') as StartingDate from AllProdOrderLine t1 LEFT JOIN Plant t2 ON t1.CompanyId=t2.PlantID\r\n";
                if (CompanyId > 0)
                {
                    if (plantCode == "swgp")
                    {
                        plantIds = plantIds != null && plantIds.Count() > 0 ? plantIds : new List<long>() { -1 };
                        query += "where t1.Status IN('Released','Finished') AND t2.CompanyID in(" + string.Join(',', plantIds) + ");";
                    }
                    else
                    {
                        query += "where t1.Status IN('Released','Finished') AND t2.CompanyID=" + CompanyId + ";\r\n";
                    }
                }
                else
                {
                    query += "where t1.Status IN('Released','Finished')\r\n";
                }
                return query;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        

        private string GetRawMatItemListDataSource(List<string?> dataSourceTableIds, long? CompanyId, string? plantCode, List<long> plantIds)
        {
            //var attributeDetails = new List<AttributeDetails>();
            try
            {
                var query = string.Empty;
                dataSourceTableIds = dataSourceTableIds != null && dataSourceTableIds.Count > 0 ? dataSourceTableIds : new List<string?>() { "a" };
                query += "select CONCAT(Type,'_',t1.ID) as AttributeDetailNameId,Type as DropDownTypeId,t1.ID as AttributeDetailID,t1.CompanyId,ItemNo as AttributeDetailName,t2.PlantCode as CompanyName,CONCAT(t1.Description,(case when ISNULL(NULLIF(t1.Description2, ''), null) is NULL then  t1.Description2 ELSE  CONCAT(' | ',t1.Description2) END)) as Description from RawMatItemList t1 JOIN Plant t2 ON t1.CompanyId=t2.PlantID Where t1.type  in(" + string.Join(",", dataSourceTableIds.Select(x => string.Format("'{0}'", x.ToString().Replace("'", "''")))) + ")\n\r";
                if (CompanyId > 0)
                {
                    if (plantCode == "swgp")
                    {
                        plantIds = plantIds != null && plantIds.Count() > 0 ? plantIds : new List<long>() { -1 };
                        query += "AND t1.CompanyID in(" + string.Join(',', plantIds) + ");";
                    }
                    else
                    {
                        query += "AND t1.CompanyID=" + CompanyId + ";\r\n";
                    }
                }
                else
                {
                    query += ";\r\n";
                }
                return query;
                //using (var connection = CreateConnection())
                //{
                //    var result = (await connection.QueryAsync<AttributeDetails>(query)).ToList();
                //    attributeDetails = result != null && result.Count() > 0 ? result : new List<AttributeDetails>();
                //}
                //return attributeDetails;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        private string GetSiteDataSource(long? CompanyId, string? plantCode, List<long> plantIds)
        {
            //var attributeDetails = new List<AttributeDetails>();
            try
            {
                var query = string.Empty;
                query += "select CONCAT('Site_',t1.IctmasterId) as AttributeDetailNameId,'Site' as DropDownTypeId,t1.IctmasterId as AttributeDetailID,t1.CompanyID as CompanyId,t2.PlantCode as CompanyName, t1.Name as AttributeDetailName,t1.Description as Description from Ictmaster t1 JOIN Plant t2 ON t1.CompanyID=t2.PlantID\r\n \r\n";
                if (CompanyId > 0)
                {
                    if (plantCode == "swgp")
                    {
                        plantIds = plantIds != null && plantIds.Count() > 0 ? plantIds : new List<long>() { -1 };
                        query += "where t1.CompanyID in(" + string.Join(',', plantIds) + ") AND t1.MasterType=570;";
                    }
                    else
                    {
                        query += "Where t1.MasterType=570 AND t1.CompanyID=" + CompanyId + ";\r\n";
                    }
                }
                else
                {
                    query += "Where t1.MasterType=570;";
                }
                //using (var connection = CreateConnection())
                //{
                //    var result = (await connection.QueryAsync<AttributeDetails>(query)).ToList();
                //    attributeDetails = result != null && result.Count() > 0 ? result : new List<AttributeDetails>();
                //}
                //return attributeDetails;
                return query;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        private string GetZoneDataSource(long? CompanyId, string? plantCode, List<long> plantIds)
        {
            //var attributeDetails = new List<AttributeDetails>();
            try
            {
                var query = string.Empty;
                query += "select CONCAT('Zone_',t1.IctmasterId) as AttributeDetailNameId,'Zone' as DropDownTypeId,t1.IctmasterId as AttributeDetailID,t1.CompanyID as CompanyId,t2.PlantCode as CompanyName, t1.Name as AttributeDetailName,t1.Description as Description,t3.Name as SiteName from Ictmaster t1 JOIN Plant t2 ON t1.CompanyID=t2.PlantID JOIN ICTMaster t3 ON t3.ICTMasterID=t1.ParentICTID\r\n";
                if (CompanyId > 0)
                {
                    if (plantCode == "swgp")
                    {
                        plantIds = plantIds != null && plantIds.Count() > 0 ? plantIds : new List<long>() { -1 };
                        query += "where t1.CompanyID in(" + string.Join(',', plantIds) + ") AND t1.MasterType=571;";
                    }
                    else
                    {
                        query += "Where t1.MasterType=571 AND t1.CompanyID=" + CompanyId + ";\r\n";
                    }
                }
                else
                {
                    query += "Where t1.MasterType=571;";
                }
                //using (var connection = CreateConnection())
                //{
                //    var result = (await connection.QueryAsync<AttributeDetails>(query)).ToList();
                //    attributeDetails = result != null && result.Count() > 0 ? result : new List<AttributeDetails>();
                //}
                //return attributeDetails;
                return query;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        private string GetLocationDataSource(long? CompanyId, string? plantCode, List<long> plantIds)
        {
            var attributeDetails = new List<AttributeDetails>();
            try
            {
                var query = string.Empty;
                query += "select CONCAT(t1.Name,'|',t1.Description) as NameList,CONCAT('Location_',t1.IctmasterId) as AttributeDetailNameId,'Location' as DropDownTypeId,t1.IctmasterId as AttributeDetailID,t1.CompanyID as CompanyId,t2.PlantCode as CompanyName, t1.Name as AttributeDetailName,t1.Description as Description,t3.Name as ZoneName,t4.Name as SiteName from Ictmaster t1 JOIN Plant t2 ON t1.CompanyID=t2.PlantID JOIN ICTMaster t3 ON t3.ICTMasterID=t1.ParentICTID JOIN ICTMaster t4 ON t4.ICTMasterID=t1.SiteID\r\n";
                if (CompanyId > 0)
                {
                    if (plantCode == "swgp")
                    {
                        plantIds = plantIds != null && plantIds.Count() > 0 ? plantIds : new List<long>() { -1 };
                        query += "where t1.CompanyID in(" + string.Join(',', plantIds) + ") AND t1.MasterType=572;";
                    }
                    else
                    {
                        query += "Where t1.MasterType=572 AND t1.CompanyID=" + CompanyId + ";\r\n";
                    }
                }
                else
                {
                    query += "Where t1.MasterType=572;";
                }
                //using (var connection = CreateConnection())
                //{
                //    var result = (await connection.QueryAsync<AttributeDetails>(query)).ToList();
                //    attributeDetails = result != null && result.Count() > 0 ? result : new List<AttributeDetails>();
                //}
                //return attributeDetails;
                return query;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        private string GetAreaDataSource(long? CompanyId, string? plantCode, List<long> plantIds)
        {
            var attributeDetails = new List<AttributeDetails>();
            try
            {
                var query = string.Empty;
                query += "select CONCAT('Area_',t1.IctmasterId) as AttributeDetailNameId,'Area' as DropDownTypeId,t1.IctmasterId as AttributeDetailID,t1.CompanyID as CompanyId,t2.PlantCode as CompanyName, t1.Name as AttributeDetailName,t1.Description as Description,t3.Name as LocationName,t4.Name as SiteName,t5.Name as ZoneName from Ictmaster t1 JOIN Plant t2 ON t1.CompanyID=t2.PlantID\r\n JOIN ICTMaster t3 ON t3.ICTMasterID=t1.ParentICTID JOIN ICTMaster t4 ON t4.ICTMasterID=t1.SiteID\r\n JOIN ICTMaster t5 ON t5.ICTMasterID=t1.ZoneID\r\n";
                if (CompanyId > 0)
                {
                    if (plantCode == "swgp")
                    {
                        plantIds = plantIds != null && plantIds.Count() > 0 ? plantIds : new List<long>() { -1 };
                        query += "where t1.CompanyID in(" + string.Join(',', plantIds) + ") AND t1.MasterType=573;";
                    }
                    else
                    {
                        query += "Where t1.MasterType=573 AND t1.CompanyID=" + CompanyId + ";\r\n";
                    }
                }
                else
                {
                    query += "Where t1.MasterType=573;";
                }
                //using (var connection = CreateConnection())
                //{
                //    var result = (await connection.QueryAsync<AttributeDetails>(query)).ToList();
                //    attributeDetails = result != null && result.Count() > 0 ? result : new List<AttributeDetails>();
                //}
                //return attributeDetails;
                return query;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        private string GetSpecificAreaDataSource(long? CompanyId, string? plantCode, List<long> plantIds)
        {
            //var attributeDetails = new List<AttributeDetails>();
            try
            {
                var query = string.Empty;
                query += "select CONCAT('SpecificArea_',t1.IctmasterId) as AttributeDetailNameId,'SpecificArea' as DropDownTypeId,t1.IctmasterId as AttributeDetailID,t1.CompanyID as CompanyId,t2.PlantCode as CompanyName, t1.Name as AttributeDetailName,t1.Description as Description,t3.Name as AreaName,t4.Name as SiteName,t5.Name as ZoneName,t6.Name as LocationName from Ictmaster t1 JOIN Plant t2 ON t1.CompanyID=t2.PlantID\r\n JOIN ICTMaster t3 ON t3.ICTMasterID=t1.ParentICTID JOIN ICTMaster t4 ON t4.ICTMasterID=t1.SiteID\r\n JOIN ICTMaster t5 ON t5.ICTMasterID=t1.ZoneID\r\n JOIN ICTMaster t6 ON t6.ICTMasterID=t1.LocationID\r\n";
                if (CompanyId > 0)
                {
                    if (plantCode == "swgp")
                    {
                        plantIds = plantIds != null && plantIds.Count() > 0 ? plantIds : new List<long>() { -1 };
                        query += "where t1.CompanyID in(" + string.Join(',', plantIds) + ") AND t1.MasterType=574;";
                    }
                    else
                    {
                        query += "Where t1.MasterType=574 AND t1.CompanyID=" + CompanyId + ";\r\n";
                    }
                }
                else
                {
                    query += "Where t1.MasterType=574;";
                }
                //using (var connection = CreateConnection())
                //{
                //    var result = (await connection.QueryAsync<AttributeDetails>(query)).ToList();
                //    attributeDetails = result != null && result.Count() > 0 ? result : new List<AttributeDetails>();
                //}
                //return attributeDetails;
                return query;
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
                query += "select 'Employee' as DropDownTypeId,t1.EmployeeID as AttributeDetailID,t1.PlantId as CompanyId,t2.PlantCode as CompanyName, t1.FirstName as AttributeDetailName,CONCAT(case when t1.NickName is NULL then  t1.FirstName ELSE  t1.NickName END,' | ',t1.LastName) as Description,t3.Name as DesignationName from Employee t1 JOIN Plant t2 ON t1.PlantID=t2.PlantID LEFT JOIN Designation t3 ON t3.DesignationID=t1.DesignationID;\r\n";
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
        public async Task<DynamicFormFilterDataSoureList> GetAllFilterDropDownDataSources()
        {
            DynamicFormFilterDataSoureList attributeDetails = new DynamicFormFilterDataSoureList();
            try
            {
                var query = string.Empty;
                query += "Select * from employee\r\n";
                query += "select * from navItems\r\n";
                using (var connection = CreateConnection())
                {
                    var result = await connection.QueryMultipleAsync(query);
                    attributeDetails.Employee = result.Read<ViewEmployee>().ToList();
                    attributeDetails.NavItems = result.Read<View_NavItems>().ToList();
                }
                return attributeDetails;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<DynamicFormFilterBy>> GetDynamicFormFilterByDataSource(List<string?> dataSourceTableIds)
        {
            var attributeDetails = new List<DynamicFormFilterBy>();
            try
            {
                var query = string.Empty;
                dataSourceTableIds = dataSourceTableIds != null && dataSourceTableIds.Count > 0 ? dataSourceTableIds : new List<string?>() { "a" };
                query += "select t1.*,t2.DataSourceTable,t2.DisplayName from DynamicFormFilterBy t1\r\nJOIN AttributeHeaderDataSource t2 ON  t1.AttributeHeaderDataSourceId=t2.AttributeHeaderDataSourceID\r\nWhere t2.DataSourceTable in(" + string.Join(",", dataSourceTableIds.Select(x => string.Format("'{0}'", x.ToString().Replace("'", "''")))) + ") order by t1.sortby asc;\n\r";
                using (var connection = CreateConnection())
                {
                    var result = (await connection.QueryAsync<DynamicFormFilterBy>(query)).ToList();
                    attributeDetails = result != null && result.Count() > 0 ? result : new List<DynamicFormFilterBy>();
                }
                return attributeDetails;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<AttributeDetails>> GetFilterDataSourceLists(List<DynamicFormFilterBy> dynamicFormFilterBies)
        {
            List<AttributeDetails> attributeDetails = new List<AttributeDetails>();
            try
            {
                var soCustomerType = new List<string?>() { "Clinic", "Vendor", "Customer" };
                List<string?> plantNames = new List<string?>() { "SWMY", "SWSG" };
                var rawMatItemType = new List<string?>() { "RawMatItem", "PackagingItem", "ProcessItem" };
                var LocationType = new List<string?>() { "Site", "Zone", "Location", "Area", "SpecificArea" };
                var query = string.Empty;
                if (dynamicFormFilterBies != null && dynamicFormFilterBies.Count() > 0)
                {
                    var counts = dynamicFormFilterBies.Count();
                    DynamicFormFilterBy last = dynamicFormFilterBies.ToList().Last();
                    dynamicFormFilterBies.ForEach(s =>
                    {
                        var DataSource = s.FilterTableName;
                        if (LocationType.Contains(s.FilterTableName))
                        {
                            if (DataSource == "Site")
                            {
                                query += "select 'Site' as DropDownTypeId,t1.CompanyID as ParentId,t1.MasterType as ApplicationMasterName,t1.IctmasterId as AttributeDetailID,t1.CompanyID as CompanyId,t2.PlantCode as CompanyName, t1.Name as AttributeDetailName,t1.Description as Description from Ictmaster t1 JOIN Plant t2 ON t1.CompanyID=t2.PlantID\r\n \r\n";
                            }
                            if (DataSource == "Zone")
                            {
                                query += "select 'Zone' as DropDownTypeId,t1.ParentICTID as ParentId,t1.MasterType as ApplicationMasterName,t1.IctmasterId as AttributeDetailID,t1.CompanyID as CompanyId,t2.PlantCode as CompanyName, t1.Name as AttributeDetailName,t1.Description as Description,t3.Name as SiteName from Ictmaster t1 JOIN Plant t2 ON t1.CompanyID=t2.PlantID JOIN ICTMaster t3 ON t3.ICTMasterID=t1.ParentICTID\r";
                            }
                            if (DataSource == "Location")
                            {
                                query += "select 'Location' as DropDownTypeId,t1.ParentICTID as ParentId,t1.MasterType as ApplicationMasterName,t1.IctmasterId as AttributeDetailID,t1.CompanyID as CompanyId,t2.PlantCode as CompanyName, t1.Name as AttributeDetailName,t1.Description as Description,t3.Name as ZoneName,t4.Name as SiteName from Ictmaster t1 JOIN Plant t2 ON t1.CompanyID=t2.PlantID JOIN ICTMaster t3 ON t3.ICTMasterID=t1.ParentICTID JOIN ICTMaster t4 ON t4.ICTMasterID=t1.SiteID\r";
                            }
                            if (DataSource == "Area")
                            {
                                query += "select 'Area' as DropDownTypeId,t1.ParentICTID as ParentId,t1.MasterType as ApplicationMasterName,t1.IctmasterId as AttributeDetailID,t1.CompanyID as CompanyId,t2.PlantCode as CompanyName, t1.Name as AttributeDetailName,t1.Description as Description,t3.Name as LocationName,t4.Name as SiteName,t5.Name as ZoneName from Ictmaster t1 JOIN Plant t2 ON t1.CompanyID=t2.PlantID\r\n JOIN ICTMaster t3 ON t3.ICTMasterID=t1.ParentICTID JOIN ICTMaster t4 ON t4.ICTMasterID=t1.SiteID\r\n JOIN ICTMaster t5 ON t5.ICTMasterID=t1.ZoneID\r";
                            }
                            if (DataSource == "SpecificArea")
                            {
                                query += "select 'SpecificArea' as DropDownTypeId,t1.ParentICTID as ParentId,t1.MasterType as ApplicationMasterName,t1.IctmasterId as AttributeDetailID,t1.CompanyID as CompanyId,t2.PlantCode as CompanyName, t1.Name as AttributeDetailName,t1.Description as Description,t3.Name as AreaName,t4.Name as SiteName,t5.Name as ZoneName,t6.Name as LocationName from Ictmaster t1 JOIN Plant t2 ON t1.CompanyID=t2.PlantID\r\n JOIN ICTMaster t3 ON t3.ICTMasterID=t1.ParentICTID JOIN ICTMaster t4 ON t4.ICTMasterID=t1.SiteID\r\n JOIN ICTMaster t5 ON t5.ICTMasterID=t1.ZoneID\r\n JOIN ICTMaster t6 ON t6.ICTMasterID=t1.LocationID\r";
                            }
                        }
                        else
                        {
                            if (DataSource == "Employee")
                            {
                                query += "select \r'Employee' as DropDownTypeId,t1.EmployeeID as AttributeDetailID,t1.PlantId as ParentId t1.PlantId as CompanyId,t2.PlantCode as CompanyName, t1.FirstName as AttributeDetailName,CONCAT(case when t1.NickName is NULL then  t1.FirstName ELSE  t1.NickName END,' | ',t1.LastName) as Description,t3.Name as DesignationName\r from Employee t1 JOIN Plant t2 ON t1.PlantID = t2.PlantID LEFT JOIN Designation t3 ON t3.DesignationID=t1.DesignationID LEFT JOIN ApplicationMasterDetail ag ON ag.ApplicationMasterDetailID = t1.AcceptanceStatus\r";
                            }
                            else if (DataSource == "NavItems")
                            {
                                query += "select\r'NavItems' as DropDownTypeId,t1.ItemId as AttributeDetailID,t1.CompanyId as ParentId,t1.CompanyId,No as AttributeDetailName,t2.PlantCode as CompanyName,CONCAT(t1.Description,(case when ISNULL(NULLIF(t1.Description2, ''), null) is NULL then  t1.Description2 ELSE  CONCAT(' | ',t1.Description2) END)) as Description from NavItems t1 JOIN Plant t2 ON t1.CompanyID = t2.PlantID\r";
                            }
                            else if (DataSource == "Division")
                            {
                                query += "select\r'Division' as DropDownTypeId,t1.DivisionID as AttributeDetailID,t1.CompanyID as CompanyId,t2.PlantCode as CompanyName, t1.Name as AttributeDetailName,t1.Description as Description from Division t1 JOIN Plant t2 ON t1.CompanyID = t2.PlantID\r";

                            }
                            else if (DataSource == "Department")
                            {
                                query += "select\r'Department' as DropDownTypeId,t1.DepartmentID as AttributeDetailID,t1.CompanyID as CompanyId,t2.PlantCode as CompanyName, t1.Name as AttributeDetailName,t1.Description as Description from Department t1 JOIN Plant t2 ON t1.CompanyID = t2.PlantID\r";

                            }
                            else if (DataSource == "Section")
                            {
                                query += "select\r'Section' as DropDownTypeId,t1.SectionID as AttributeDetailID,t2.CompanyID as CompanyId,t3.PlantCode as CompanyName, t1.Name as AttributeDetailName,CONCAT(t2.Name,'||',t1.Description) as Description from Section t1 JOIN Department t2 ON t2.DepartmentID=t1.DepartmentID JOIN Plant t3 ON t2.CompanyID=t3.PlantID\r";

                            }
                            else if (DataSource == "SubSection")
                            {
                                query += "select\r'SubSection' as DropDownTypeId,t1.SectionID as AttributeDetailID,t4.CompanyID as CompanyId,t4.PlantCode as CompanyName, t1.Name as AttributeDetailName,CONCAT(t2.Name,'||',t3.Name,'||',t1.Description) as Description from SubSection t1 JOIN Section t2 ON t2.SectionID=t1.SectionID  JOIN Department t3 ON t3.DepartmentID=t2.DepartmentID  JOIN Plant t4 ON t3.CompanyID=t4.PlantID\r";

                            }
                            else if (DataSource == "Designation")
                            {
                                query += "select 'Designation' as DropDownTypeId,t1.SectionID as AttributeDetailID,t1.CompanyID as CompanyId,t3.PlantCode as CompanyName, t1.Name as AttributeDetailName,CONCAT(t2.Name,'||',t1.Description) as Description from Designation t1 JOIN SubSection t2 ON t2.SubSectionID=t1.SubSectionTID JOIN Plant t3 ON t1.CompanyID=t3.PlantID\r";

                            }
                            else if (DataSource == "ItemBatchInfo")
                            {
                                query += "select 'ItemBatchInfo' as DropDownTypeId,t1.ItemBatchId as AttributeDetailID,t1.CompanyID as CompanyId,t2.PlantCode as CompanyName, t1.BatchNo as AttributeDetailName,CONCAT(t3.No,'||',t3.Description) as Description from ItemBatchInfo t1 JOIN Plant t2 ON t1.CompanyId=t2.PlantID JOIN NavItems t3 ON t3.ItemId=t1.ItemId\r";

                            }
                            else
                            {
                                query += "select '" + s.FilterTableName + "' as DropDownTypeId, t1." + s.ToDropDownFieldId + " as AttributeDetailID,t1." + s.ToDisplayDropDownName + " as AttributeDetailName,";
                                if (string.IsNullOrEmpty(s.ToDisplayDropDownDescription))
                                {
                                    query += "null as Description,";
                                }
                                else
                                {
                                    query += "t1." + s.ToDisplayDropDownDescription + " as Description,";
                                }
                                if (string.IsNullOrEmpty(s.ApplicationMasterCodeId))
                                {
                                    query += "null as ApplicationMasterName from " + s.FilterTableName;
                                }
                                else
                                {
                                    query += "'" + s.ApplicationMasterCodeId + "' as ApplicationMasterName from " + s.FilterTableName;
                                }
                                query += " t1 \r\n";
                            }
                        }

                        if (s.FilterTableName == "CodeMaster" && !string.IsNullOrEmpty(s.ApplicationMasterCodeId))
                        {
                            query += "where t1.CodeType='" + s.ApplicationMasterCodeId + "'\n\r";
                        }
                        if (LocationType.Contains(s.FilterTableName) && !string.IsNullOrEmpty(s.ApplicationMasterCodeId))
                        {
                            query += "where t1.MasterType='" + s.ApplicationMasterCodeId + "'\n\r";
                        }
                        if (s.FilterTableName == "ApplicationMasterDetail" && !string.IsNullOrEmpty(s.ApplicationMasterCodeId))
                        {
                            query += "JOIN ApplicationMaster t2 ON t1.ApplicationMasterID=t2.ApplicationMasterID WHERE (t1.StatusCodeID=1 OR t1.StatusCodeID IS Null) AND t2.ApplicationMasterCodeID=" + s.ApplicationMasterCodeId + "";
                        }
                        if (last.Equals(s))
                        {
                        }
                        else
                        {
                            // query += "UNION ALL\n\r";
                        }
                        query += ";";
                    });

                    if (!string.IsNullOrEmpty(query))
                    {
                        using (var connection = CreateConnection())
                        {
                            var result = (await connection.QueryMultipleAsync(query));
                            for (int j = 0; j < counts; j++)
                            {
                                var results = result.Read<AttributeDetails>().ToList();
                                var attributeDetailss = results != null && results.Count() > 0 ? results : new List<AttributeDetails>();
                                attributeDetails.AddRange(attributeDetailss);
                            }
                            // var result = (await connection.QueryAsync<AttributeDetails>(query)).ToList();
                            // attributeDetails = result != null && result.Count() > 0 ? result : new List<AttributeDetails>();
                        }
                    }
                }
                return attributeDetails;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<AttributeDetails>> GetFilterByDataSourceLists(List<DynamicFormFilterBy> dynamicFormFilterBies, object Data, string? DataSource)
        {
            List<AttributeDetails> attributeDetails = new List<AttributeDetails>();
            try
            {
                var soCustomerType = new List<string?>() { "Clinic", "Customer" };
                List<string?> plantNames = new List<string?>() { "SWMY", "SWSG" };
                var rawMatItemType = new List<string?>() { "RawMatItem", "PackagingItem", "ProcessItem" };
                var LocationType = new List<string?>() { "Site", "Zone", "Location", "Area", "SpecificArea" };
                List<long> plantIds = new List<long>();
                var plantsData = await GetPlantDataSource();
                plantIds = plantsData.Where(w => plantNames.Contains(w.AttributeDetailName)).Select(s => s.AttributeDetailID).ToList();
                var query = string.Empty;
                query += "\r";
                if (DataSource == "Employee")
                {
                    query += "select \r'Employee' as DropDownTypeId,t1.EmployeeID as AttributeDetailID,t1.PlantId as CompanyId,t2.PlantCode as CompanyName, t1.FirstName as AttributeDetailName,CONCAT(case when t1.NickName is NULL then  t1.FirstName ELSE  t1.NickName END,' | ',t1.LastName) as Description,t3.Name as DesignationName\r from Employee t1 JOIN Plant t2 ON t1.PlantID = t2.PlantID LEFT JOIN Designation t3 ON t3.DesignationID=t1.DesignationID LEFT JOIN ApplicationMasterDetail ag ON ag.ApplicationMasterDetailID = t1.AcceptanceStatus\r";
                }
                else if (DataSource == "NavItems")
                {
                    query += "select\r'NavItems' as DropDownTypeId,t1.ItemId as AttributeDetailID,t1.CompanyId,No as AttributeDetailName,t2.PlantCode as CompanyName,CONCAT(t1.Description,(case when ISNULL(NULLIF(t1.Description2, ''), null) is NULL then  t1.Description2 ELSE  CONCAT(' | ',t1.Description2) END)) as Description from NavItems t1 JOIN Plant t2 ON t1.CompanyID = t2.PlantID\r";
                }
                else if (DataSource == "Division")
                {
                    query += "select\r'Division' as DropDownTypeId,t1.DivisionID as AttributeDetailID,t1.CompanyID as CompanyId,t2.PlantCode as CompanyName, t1.Name as AttributeDetailName,t1.Description as Description from Division t1 JOIN Plant t2 ON t1.CompanyID = t2.PlantID\r";

                }
                else if (DataSource == "Department")
                {
                    query += "select\r'Department' as DropDownTypeId,t1.DepartmentID as AttributeDetailID,t1.CompanyID as CompanyId,t2.PlantCode as CompanyName, t1.Name as AttributeDetailName,t1.Description as Description from Department t1 JOIN Plant t2 ON t1.CompanyID = t2.PlantID\r";

                }
                else if (DataSource == "Section")
                {
                    query += "select\r'Section' as DropDownTypeId,t1.SectionID as AttributeDetailID,t2.CompanyID as CompanyId,t3.PlantCode as CompanyName, t1.Name as AttributeDetailName,CONCAT(t2.Name,'||',t1.Description) as Description from Section t1 JOIN Department t2 ON t2.DepartmentID=t1.DepartmentID JOIN Plant t3 ON t2.CompanyID=t3.PlantID\r";

                }
                else if (DataSource == "SubSection")
                {
                    query += "select\r'SubSection' as DropDownTypeId,t1.SectionID as AttributeDetailID,t4.CompanyID as CompanyId,t4.PlantCode as CompanyName, t1.Name as AttributeDetailName,CONCAT(t2.Name,'||',t3.Name,'||',t1.Description) as Description from SubSection t1 JOIN Section t2 ON t2.SectionID=t1.SectionID  JOIN Department t3 ON t3.DepartmentID=t2.DepartmentID  JOIN Plant t4 ON t3.CompanyID=t4.PlantID\r";

                }
                else if (DataSource == "Designation")
                {
                    query += "select 'Designation' as DropDownTypeId,t1.SectionID as AttributeDetailID,t1.CompanyID as CompanyId,t3.PlantCode as CompanyName, t1.Name as AttributeDetailName,CONCAT(t2.Name,'||',t1.Description) as Description from Designation t1 JOIN SubSection t2 ON t2.SubSectionID=t1.SubSectionTID JOIN Plant t3 ON t1.CompanyID=t3.PlantID\r";

                }
                else if (DataSource == "RawMatPurch")
                {
                    query += "select CONCAT('RawMatPurch','_',t1.RawMatPurchID) as AttributeDetailNameId,'RawMatPurch' as DropDownTypeId, t1.QcRefNo,t1.RawMatPurchID as AttributeDetailID,t1.ItemNo as AttributeDetailName,t1.Description as Description,t1.CompanyID as CompanyId,t2.PlantCode as CompanyName,t1.BatchNo,FORMAT(t1.ExpirationDate, 'dd-MMM-yyyy') as ExpirationDate,FORMAT(t1.ManufacturingDate, 'dd-MMM-yyyy') as ManufacturingDate from RawMatPurch t1 LEFT JOIN Plant t2 ON t1.CompanyId=t2.PlantID\r";
                }
                else if (DataSource == "ReleaseProdOrderLine")
                {
                    query += "select CONCAT('ReleaseProdOrderLine','_',t1.ReleaseProdOrderLineId) as AttributeDetailNameId,'ReleaseProdOrderLine' as DropDownTypeId, t1.ReleaseProdOrderLineId as AttributeDetailID,t1.ItemNo as AttributeDetailName,t1.Description as Description,t1.Description2 as Description2,t1.ReplanRefNo,t1.CompanyID as CompanyId,t2.PlantCode as CompanyName,t1.BatchNo,t1.BatchSize,t1.ProdOrderNo,t1.UnitOfMeasureCode,t1.Status,t1.SubStatus,FORMAT(t1.StartingDate, 'dd-MMM-yyyy') as StartingDate from ReleaseProdOrderLine t1 LEFT JOIN Plant t2 ON t1.CompanyId=t2.PlantID Where CAST(t1.StartingDate as DATE)>='01-01-2020' AND t1.Status IN('Released','Finished')\r";

                }
                else if (DataSource == "AllProdOrderLine")
                {
                    query += "select CONCAT('AllProdOrderLine','_',t1.AllProdOrderLineId) as AttributeDetailNameId,'AllProdOrderLine' as DropDownTypeId, t1.AllProdOrderLineId as AttributeDetailID,t1.ItemNo as AttributeDetailName,t1.Description as Description,t1.Description2 as Description2,t1.ReplanRefNo,t1.CompanyID as CompanyId,t2.PlantCode as CompanyName,t1.BatchNo,t1.BatchSize,t1.ProdOrderNo,t1.UnitOfMeasureCode,t1.Status,t1.SubStatus,FORMAT(t1.StartingDate, 'dd-MMM-yyyy') as StartingDate from AllProdOrderLine t1 LEFT JOIN Plant t2 ON t1.CompanyId=t2.PlantID Where t1.Status IN('Released','Finished')\r";

                }
                else if (DataSource == "ItemBatchInfo")
                {
                    query += "select 'ItemBatchInfo' as DropDownTypeId,t1.ItemBatchId as AttributeDetailID,t1.CompanyID as CompanyId,t2.PlantCode as CompanyName, t1.BatchNo as AttributeDetailName,CONCAT(t3.No,'||',t3.Description) as Description from ItemBatchInfo t1 JOIN Plant t2 ON t1.CompanyId=t2.PlantID JOIN NavItems t3 ON t3.ItemId=t1.ItemId\r";

                }
                else if (DataSource == "FinishedProdOrderLine")
                {
                    query += "select 'FinishedProdOrderLine' as DropDownTypeId,\r\nt1.FinishedProdOrderLineID as AttributeDetailID,t1.CompanyID as CompanyId,t2.PlantCode as CompanyName, t1.ItemNo as AttributeDetailName,\r\nt1.Description,t1.Description2,t1.ReplanRefNo,t1.BatchNo,FORMAT(t1.StartingDate, 'dd-MMM-yyyy') as StartingDate,FORMAT(t1.ExpirationDate, 'dd-MMM-yyyy') as ExpirationDate,FORMAT(t1.ManufacturingDate, 'dd-MMM-yyyy') as ManufacturingDate,t1.ProductCode,t1.ProductName  from FinishedProdOrderLine t1 \r\nJOIN Plant t2 ON t1.CompanyId=t2.PlantID\n\r";
                    query += "Where (t1.OptStatus='quarantine' OR t1.OptStatus='' OR t1.OptStatus is null)  AND CAST(t1.StartingDate as DATE)>'05-31-2023' AND (t1.BatchNo is not null AND t1.BatchNo!='')\r";
                }
                else if (DataSource == "FinishedProdOrderLineProductionInProgress")
                {
                    query += "select 'FinishedProdOrderLineProductionInProgress' as DropDownTypeId,\r\nt1.FinishedProdOrderLineID as AttributeDetailID,t1.CompanyID as CompanyId,t2.PlantCode as CompanyName, t1.ItemNo as AttributeDetailName,\r\nt1.Description,t1.Description2,t1.ReplanRefNo,t1.BatchNo,FORMAT(t1.StartingDate, 'dd-MMM-yyyy') as StartingDate,FORMAT(t1.ExpirationDate, 'dd-MMM-yyyy') as ExpirationDate,FORMAT(t1.ManufacturingDate, 'dd-MMM-yyyy') as ManufacturingDate,t1.ProductCode,t1.ProductName  from FinishedProdOrderLine t1 \r\nJOIN Plant t2 ON t1.CompanyId=t2.PlantID\n\r";
                    query += "Where t1.OptStatus='Production In Progress'  AND CAST(t1.StartingDate as DATE)>'05-31-2023' AND (t1.BatchNo is not null AND t1.BatchNo!='')\r";
                }
                else
                {
                    if (DataSource == "Site")
                    {
                        query += "select 'Site' as DropDownTypeId,t1.IctmasterId as AttributeDetailID,t1.CompanyID as CompanyId,t2.PlantCode as CompanyName, t1.Name as AttributeDetailName,t1.Description as Description from Ictmaster t1 JOIN Plant t2 ON t1.CompanyID=t2.PlantID\r\n \r\n";
                        query += "Where t1.MasterType=570\rAND\r";
                    }
                    if (DataSource == "Zone")
                    {
                        query += "select 'Zone' as DropDownTypeId,t1.IctmasterId as AttributeDetailID,t1.CompanyID as CompanyId,t2.PlantCode as CompanyName, t1.Name as AttributeDetailName,t1.Description as Description,t3.Name as SiteName from Ictmaster t1 JOIN Plant t2 ON t1.CompanyID=t2.PlantID JOIN ICTMaster t3 ON t3.ICTMasterID=t1.ParentICTID\r";
                        query += "Where t1.MasterType=571\rAND\r";
                    }
                    if (DataSource == "Location")
                    {
                        query += "select 'Location' as DropDownTypeId,t1.IctmasterId as AttributeDetailID,t1.CompanyID as CompanyId,t2.PlantCode as CompanyName, t1.Name as AttributeDetailName,t1.Description as Description,t3.Name as ZoneName,t4.Name as SiteName from Ictmaster t1 JOIN Plant t2 ON t1.CompanyID=t2.PlantID JOIN ICTMaster t3 ON t3.ICTMasterID=t1.ParentICTID JOIN ICTMaster t4 ON t4.ICTMasterID=t1.SiteID\r";
                        query += "Where t1.MasterType=572\rAND\r";
                    }
                    if (DataSource == "Area")
                    {
                        query += "select 'Area' as DropDownTypeId,t1.IctmasterId as AttributeDetailID,t1.CompanyID as CompanyId,t2.PlantCode as CompanyName, t1.Name as AttributeDetailName,t1.Description as Description,t3.Name as LocationName,t4.Name as SiteName,t5.Name as ZoneName from Ictmaster t1 JOIN Plant t2 ON t1.CompanyID=t2.PlantID\r\n JOIN ICTMaster t3 ON t3.ICTMasterID=t1.ParentICTID JOIN ICTMaster t4 ON t4.ICTMasterID=t1.SiteID\r\n JOIN ICTMaster t5 ON t5.ICTMasterID=t1.ZoneID\r";
                        query += "Where t1.MasterType=573\rAND\r";
                    }
                    if (DataSource == "SpecificArea")
                    {
                        query += "select 'SpecificArea' as DropDownTypeId,t1.IctmasterId as AttributeDetailID,t1.CompanyID as CompanyId,t2.PlantCode as CompanyName, t1.Name as AttributeDetailName,t1.Description as Description,t3.Name as AreaName,t4.Name as SiteName,t5.Name as ZoneName,t6.Name as LocationName from Ictmaster t1 JOIN Plant t2 ON t1.CompanyID=t2.PlantID\r\n JOIN ICTMaster t3 ON t3.ICTMasterID=t1.ParentICTID JOIN ICTMaster t4 ON t4.ICTMasterID=t1.SiteID\r\n JOIN ICTMaster t5 ON t5.ICTMasterID=t1.ZoneID\r\n JOIN ICTMaster t6 ON t6.ICTMasterID=t1.LocationID\r";
                        query += "Where t1.MasterType=574\r AND\r";
                    }
                    if (DataSource == "Vendor")
                    {
                        query += "select Type as DropDownTypeId, t1.SoCustomerID as AttributeDetailID,t1.CustomerName as AttributeDetailName,t1.Address1 as Description,t1.CompanyID as CompanyId,t2.PlantCode as CompanyName from SoCustomer t1 Left JOIN Plant t2 ON t1.CompanyID=t2.PlantID\r";
                        query += "Where t1.Type='Vendor'\r AND\r";
                    }
                    else
                    {
                        if (soCustomerType.Contains(DataSource))
                        {
                            query += "select Type as DropDownTypeId, t1.SoCustomerID as AttributeDetailID,t1.CustomerName as AttributeDetailName,t1.Address1 as Description from SoCustomer t1\r";
                            if (dynamicFormFilterBies == null)
                            {
                                query += "where t1.type='" + DataSource + "'\rAND\r";
                            }
                            if (dynamicFormFilterBies != null && dynamicFormFilterBies.Count == 0)
                            {
                                query += "where t1.type='" + DataSource + "'\rAND\r";
                            }
                        }
                        else
                        {
                            if (rawMatItemType.Contains(DataSource))
                            {
                                query += "select Type as DropDownTypeId,t1.ID as AttributeDetailID,t1.CompanyId,ItemNo as AttributeDetailName,t2.PlantCode as CompanyName,CONCAT(t1.Description,(case when ISNULL(NULLIF(t1.Description2, ''), null) is NULL then  t1.Description2 ELSE  CONCAT(' | ',t1.Description2) END)) as Description from RawMatItemList t1 JOIN Plant t2 ON t1.CompanyId=t2.PlantID\r";
                                if (dynamicFormFilterBies == null)
                                {
                                    query += "where t1.type='" + DataSource + "'\rAND\r";
                                }
                                if (dynamicFormFilterBies != null && dynamicFormFilterBies.Count == 0)
                                {
                                    query += "where t1.type='" + DataSource + "'\rAND\r";
                                }
                            }
                        }
                    }
                }
                if (dynamicFormFilterBies != null && dynamicFormFilterBies.Count() > 0)
                {

                    if (soCustomerType.Contains(DataSource))
                    {
                        query += "where t1.type='" + DataSource + "' AND\r";
                    }
                    else
                    {
                        if (rawMatItemType.Contains(DataSource))
                        {
                            query += "where t1.type='" + DataSource + "' AND\r";
                        }
                        if (LocationType.Contains(DataSource))
                        {
                            query += "";
                        }
                        else
                        {
                            if (DataSource == "AllProdOrderLine" ||  DataSource == "ReleaseProdOrderLine" ||  DataSource == "FinishedProdOrderLine" || DataSource == "FinishedProdOrderLineProductionInProgress")
                            {
                                query += "AND\r";
                            }
                            else
                            {
                                if (DataSource != "Vendor")
                                {
                                    query += "where";
                                }
                            }
                        }
                    }
                    var query1 = string.Empty;
                    DynamicFormFilterBy first = dynamicFormFilterBies.ToList().First();
                    dynamicFormFilterBies.ForEach(s =>
                    {
                        var proNames = s.DynamicFormFilterId.ToString();
                        if (s.FilterDataType == "DateField")
                        {
                            proNames = "From_" + s.DynamicFormFilterId.ToString();
                        }
                        var proName = Data.GetType().GetProperty(proNames);
                        if (proName != null)
                        {
                            var andOrName = Data.GetType().GetProperty("AndOr_" + s.DynamicFormFilterId);
                            var likeName = Data.GetType().GetProperty("Like_" + s.DynamicFormFilterId);
                            var AndOrValue = (string)andOrName.GetValue(Data);
                            string likeValueData = null;
                            if (likeName != null)
                            {
                                likeValueData = (string)likeName.GetValue(Data);
                            }
                            if (s.FilterDataType == "DropDown")
                            {
                                var valueData = (long?)proName.GetValue(Data);
                                if (valueData != null)
                                {
                                    if (!first.Equals(valueData))
                                    {
                                        if (!string.IsNullOrEmpty(query1))
                                        {
                                            query1 += "\r" + AndOrValue;
                                        }
                                    }
                                    if (s.FilterTableName == "Plant" || s.FilterTableName == "NavItems")
                                    {
                                        if (valueData > 0)
                                        {
                                            var plantCode = plantsData.FirstOrDefault(f => f.AttributeDetailID == valueData)?.AttributeDetailName;
                                            if (plantCode != null)
                                            {
                                                plantCode = plantCode.ToLower();
                                                if (plantCode == "swgp")
                                                {
                                                    plantIds = plantIds != null && plantIds.Count() > 0 ? plantIds : new List<long>() { -1 };
                                                    if (s.FilterTableName == "Plant" || s.FilterTableName == "Division" || DataSource == "Department" || DataSource == "Section" || DataSource == "SubSection")
                                                    {
                                                        query1 += "\rt1." + s.FromFilterFieldName + "\rin(" + string.Join(',', plantIds) + ")\r";
                                                    }
                                                    else if (DataSource == "NavItems")
                                                    {
                                                        query1 += "\r t1.no like 'FP%' AND t1.CompanyId" + "\rin(" + string.Join(',', plantIds) + ")\r";
                                                    }
                                                    else
                                                    {
                                                        if (rawMatItemType.Contains(DataSource))
                                                        {
                                                            query1 += "\rt1." + s.FromFilterFieldName + "\rin(" + string.Join(',', plantIds) + ")\r";
                                                        }
                                                        else if (LocationType.Contains(DataSource))
                                                        {
                                                            query1 += "\rt1." + s.FromFilterFieldName + "\rin(" + string.Join(',', plantIds) + ")\r";
                                                        }
                                                        else
                                                        {
                                                            query1 += "\rt1." + s.FromFilterFieldName + "\r='" + valueData + "'\r";
                                                        }
                                                    }
                                                }
                                                else
                                                {

                                                    if (DataSource == "NavItems")
                                                    {
                                                        query1 += "\r t1.no like 'FP%' AND t1.CompanyId = '" + valueData + "'\r";
                                                    }
                                                    else
                                                    {
                                                        query1 += "\rt1." + s.FromFilterFieldName + "\r='" + valueData + "'\r";
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (DataSource == "NavItems")
                                                {
                                                    query1 += "\r t1.no like 'FP%' AND t1.CompanyId = '" + valueData + "'\r";
                                                }
                                                else
                                                {
                                                    query1 += "\rt1." + s.FromFilterFieldName + "\r='" + valueData + "'\r";
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        query1 += "\rt1." + s.FromFilterFieldName + "\r='" + valueData + "'\r";
                                    }
                                }
                            }
                            if (s.FilterDataType == "TextBox")
                            {
                                var valueData = (string)proName.GetValue(Data);
                                if (valueData != null)
                                {
                                    valueData = valueData.Trim();
                                    valueData = valueData.Replace("'", @"''");
                                    if (!first.Equals(valueData))
                                    {
                                        if (!string.IsNullOrEmpty(query1))
                                        {
                                            query1 += "\r" + AndOrValue;
                                        }
                                    }
                                    query1 += "\rt1." + s.FromFilterFieldName + "\r";
                                    if (likeValueData == "Contains")
                                    {
                                        query1 += "Like '%" + valueData + "%'\r";
                                    }
                                    if (likeValueData == "EndsWith")
                                    {
                                        query1 += "Like\r'" + valueData + "%'\r";
                                    }
                                    if (likeValueData == "StartsWith")
                                    {
                                        query1 += "Like\r'%" + valueData + "'\r";
                                    }
                                    if (likeValueData == "Equal")
                                    {
                                        query1 += "='" + valueData + "'\r";
                                    }
                                }

                            }
                            if (s.FilterDataType == "DateField")
                            {
                                DateTime? TovalueDatas = null;
                                var FromvalueData = (DateTime?)proName.GetValue(Data);
                                var proToName = Data.GetType().GetProperty("To_" + s.DynamicFormFilterId.ToString());
                                if (proToName != null)
                                {
                                    TovalueDatas = (DateTime?)proToName.GetValue(Data);
                                }
                                if (FromvalueData != null)
                                {
                                    if (!first.Equals(FromvalueData))
                                    {
                                        if (!string.IsNullOrEmpty(query1))
                                        {
                                            query1 += "\r" + AndOrValue;
                                        }
                                    }
                                    var from = FromvalueData.Value.ToString("yyyy-MM-dd");
                                    string? TovalueDate = null;
                                    if (TovalueDatas != null)
                                    {
                                        TovalueDate = TovalueDatas.Value.ToString("yyyy-MM-dd");
                                    }
                                    if (FromvalueData != null && TovalueDatas == null)
                                    {
                                        query1 += "\rCAST(t1." + s.FromFilterFieldName + " AS Date)\r='" + from + "'\r";
                                    }
                                    if (FromvalueData != null && TovalueDatas != null)
                                    {
                                        query1 += "\rCAST(t1." + s.FromFilterFieldName + " AS Date)\r>='" + from + "'\r";
                                        query1 += AndOrValue + "\rCAST(t1." + s.FromFilterFieldName + " AS Date)\r<='" + TovalueDate + "'\r";
                                    }
                                }
                            }
                        }


                    });
                    query += query1;
                }
                if (!string.IsNullOrEmpty(query))
                {
                    query = query.Trim();
                    if (query.EndsWith("AND"))
                    {
                        query = query.Remove(query.Length - 3);
                    }
                    if (query.EndsWith("OR"))
                    {
                        query = query.Remove(query.Length - 2);
                    }
                    if (query.EndsWith("where"))
                    {
                        query = query.Remove(query.Length - 5);
                    }
                    if (DataSource == "ItemBatchInfo")
                    {
                        var queryIn = "\r\nselect \r\n(case when (SUBSTRING(bb2.ProductName, 0, CHARINDEX(',', bb2.ProductName)))='' then  bb2.ProductName ELSE  (SUBSTRING(bb2.ProductName, 0, CHARINDEX(',', bb2.ProductName))) END) as ItemBatchNoId\r\nfrom\r\n(select bb1.*,\r\nProductName = STUFF(( SELECT ',' + CAST(md.ItemBatchId AS VARCHAR(MAX)) FROM ItemBatchInfo md   WHERE md.CompanyID=bb1.CompanyId AND md.BatchNo=bb1.BatchNo   Order by md.ItemBatchId asc FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, '')\r\nfrom(select b1.BatchNo,b1.CompanyId from ItemBatchInfo b1   group by b1.BatchNo,b1.CompanyId)bb1) bb2\r\n\r\n";

                        query += "AND t1.ItemBatchId IN(" + queryIn + ")";
                    }
                    if (DataSource == "Employee")
                    {
                        query += "AND (ag.Value!='Resign' or ag.Value is null)";
                    }
                    using (var connection = CreateConnection())
                    {
                        var result = (await connection.QueryAsync<AttributeDetails>(query)).ToList();
                        attributeDetails = result != null && result.Count() > 0 ? result : new List<AttributeDetails>();
                    }
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
