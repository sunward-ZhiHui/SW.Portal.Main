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

                if (dataSourceTableIds.Contains("ApplicationMaster") && applicationMasterIds.Count > 0)
                {
                    dataSourceDropDownList.AddRange(await GetApplicationMasterDataSource(applicationMasterIds));
                }
                if (dataSourceTableIds.Contains("ApplicationMasterParent") && applicationMasterParentIds.Count > 0)
                {
                    dataSourceDropDownList.AddRange(await GetApplicationMasterParentDataSource(applicationMasterParentIds));
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
                if (dataSourceTableIds.Contains("ApplicationMasterParent") && applicationMasterParentIds.Count > 0)
                {
                    dataSourceDropDownList.AddRange(await GetApplicationMasterParentDataSource(applicationMasterParentIds));
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
        private async Task<IReadOnlyList<AttributeDetails>> GetApplicationMasterParentDataSource(List<long?> applicationMasterIds)
        {
            var attributeDetails = new List<AttributeDetails>();
            try
            {
                var query = string.Empty;
                if (applicationMasterIds != null && applicationMasterIds.Count > 0)
                {
                    applicationMasterIds = applicationMasterIds != null && applicationMasterIds.Count() > 0 ? applicationMasterIds : new List<long?>() { -1 };
                    query += "WITH ApplicationMasterParent_cte AS (\r\n  SELECT\r\n    ApplicationMasterParentID,\r\n    ApplicationMasterName,\r\n    ApplicationMasterParentCodeID,\r\n    ParentID,\r\n    1 AS level\r\n  FROM ApplicationMasterParent\r\n  WHERE ApplicationMasterParentCodeID in(" + string.Join(',', applicationMasterIds) + ")\r\n  UNION ALL\r\n  SELECT\r\n    e.ApplicationMasterParentID,\r\n    e.ApplicationMasterName,\r\n    e.ApplicationMasterParentCodeID,\r\n    e.ParentID,\r\n    level + 1\r\n  FROM ApplicationMasterParent e\r\n  INNER JOIN ApplicationMasterParent_cte r\r\n    ON e.ParentID = r.ApplicationMasterParentCodeID\r\n)select 'ApplicationMasterParent' as DropDownTypeId,t1.ApplicationMasterChildID as AttributeDetailID,t1.Value as AttributeDetailName,t1.Description,t1.ApplicationMasterParentID as ApplicationMasterParentCodeId,t2.ApplicationMasterName as ApplicationMasterName,t1.ParentId,t3.Value as ParentName from ApplicationMasterChild  t1 JOIN ApplicationMasterParent t2 ON t1.ApplicationMasterParentID=t2.ApplicationMasterParentCodeID LEFT JOIN ApplicationMasterChild t3 ON t1.ParentID=t3.ApplicationMasterChildID where t1.ApplicationMasterParentID in(SELECT ApplicationMasterParentCodeID FROM ApplicationMasterParent_cte);";
                }
                else
                {
                    query += "select 'ApplicationMasterParent' as DropDownTypeId,t1.ApplicationMasterChildID as AttributeDetailID,t1.Value as AttributeDetailName,t1.Description,t1.ApplicationMasterParentID as ApplicationMasterParentCodeId,t2.ApplicationMasterName as ApplicationMasterName,t1.ParentId,t3.Value as ParentName from ApplicationMasterChild  t1 JOIN ApplicationMasterParent t2 ON t1.ApplicationMasterParentID=t2.ApplicationMasterParentCodeID LEFT JOIN ApplicationMasterChild t3 ON t1.ParentID=t3.ApplicationMasterChildID";
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
                query += "select t1.*,t2.DataSourceTable,t2.DisplayName from DynamicFormFilterBy t1\r\nJOIN AttributeHeaderDataSource t2 ON  t1.AttributeHeaderDataSourceId=t2.AttributeHeaderDataSourceID\r\nWhere t2.DataSourceTable in(" + string.Join(",", dataSourceTableIds.Select(x => string.Format("'{0}'", x.ToString().Replace("'", "''")))) + ") order by t1.DynamicFormFilterID;\n\r";
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

                var query = string.Empty;
                if (dynamicFormFilterBies != null && dynamicFormFilterBies.Count() > 0)
                {
                    DynamicFormFilterBy last = dynamicFormFilterBies.ToList().Last();
                    dynamicFormFilterBies.ForEach(s =>
                    {
                        query += "select '" + s.FilterTableName + "' as DropDownTypeId, t1." + s.ToDropDownFieldId + " as AttributeDetailID,t1." + s.ToDisplayDropDownName + " as AttributeDetailName,";
                        if (string.IsNullOrEmpty(s.ToDisplayDropDownDescription))
                        {
                            query += "null as Description,";
                        }
                        else
                        {
                            query += "" + s.ToDisplayDropDownDescription + " as Description,";
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
                        if (s.FilterTableName == "CodeMaster" && !string.IsNullOrEmpty(s.ApplicationMasterCodeId))
                        {
                            query += "where CodeType='" + s.ApplicationMasterCodeId + "'\n\r";
                        }
                        if (s.FilterTableName == "ApplicationMasterDetail" && !string.IsNullOrEmpty(s.ApplicationMasterCodeId))
                        {
                            query += "JOIN ApplicationMaster t2 ON t1.ApplicationMasterID=t2.ApplicationMasterID WHERE t2.ApplicationMasterCodeID=" + s.ApplicationMasterCodeId + "";
                        }
                        if (last.Equals(s))
                        {
                        }
                        else
                        {
                            query += "UNION ALL\n\r";
                        }
                    });
                }
                if (!string.IsNullOrEmpty(query))
                {
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
        public async Task<IReadOnlyList<AttributeDetails>> GetFilterByDataSourceLists(List<DynamicFormFilterBy> dynamicFormFilterBies, object Data, string? DataSource)
        {
            List<AttributeDetails> attributeDetails = new List<AttributeDetails>();
            try
            {
                var soCustomerType = new List<string?>() { "Clinic", "Vendor", "Customer" };
                List<string?> plantNames = new List<string?>() { "SWMY", "SWSG" };
                var rawMatItemType = new List<string?>() { "RawMatItem", "PackagingItem", "ProcessItem" };
                List<long> plantIds = new List<long>();
                var plantsData = await GetPlantDataSource();
                plantIds = plantsData.Where(w => plantNames.Contains(w.AttributeDetailName)).Select(s => s.AttributeDetailID).ToList();
                var query = string.Empty;
                query += "\r";
                if (DataSource == "Employee")
                {
                    query += "select \r'Employee' as DropDownTypeId,t1.EmployeeID as AttributeDetailID,t1.PlantId as CompanyId,t2.PlantCode as CompanyName, t1.FirstName as AttributeDetailName,CONCAT(case when t1.NickName is NULL then  t1.FirstName ELSE  t1.NickName END,' | ',t1.LastName) as Description\r from Employee t1 JOIN Plant t2 ON t1.PlantID = t2.PlantID\r";
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
                        else
                        {
                            query += "where";
                        }
                    }
                    var query1 = string.Empty;
                    DynamicFormFilterBy first = dynamicFormFilterBies.ToList().First();
                    dynamicFormFilterBies.ForEach(s =>
                    {
                        var proNames = s.DynamicFormFilterById.ToString();
                        if (s.FilterDataType == "DateField")
                        {
                            proNames = "From_" + s.DynamicFormFilterById.ToString();
                        }
                        var proName = Data.GetType().GetProperty(proNames);
                        if (proName != null)
                        {
                            var andOrName = Data.GetType().GetProperty("AndOr_" + s.DynamicFormFilterById);
                            var likeName = Data.GetType().GetProperty("Like_" + s.DynamicFormFilterById);
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
                                                        else
                                                        {
                                                            query1 += "\rt1." + s.FromFilterFieldName + "\r='" + valueData + "'\r";
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    //if (DataSource == "Plant")
                                                    //{
                                                    //    query1 += "\rt1." + s.FromFilterFieldName + "\r='" + valueData + "'\r";
                                                    //}
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
                                                //if (DataSource == "Plant")
                                                //{
                                                //    query1 += "\rt1." + s.FromFilterFieldName + "\r='" + valueData + "'\r";
                                                //}
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
                                var proToName = Data.GetType().GetProperty("To_" + s.DynamicFormFilterById.ToString());
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
