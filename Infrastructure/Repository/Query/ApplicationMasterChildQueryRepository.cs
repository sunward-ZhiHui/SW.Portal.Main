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

namespace Infrastructure.Repository.Query
{
    public class ApplicationMasterChildQueryRepository : DbConnector, IApplicationMasterChildQueryRepository
    {
        public ApplicationMasterChildQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<IReadOnlyList<ApplicationMasterChildModel>> GetAllByAsync(string Ids)
        {
            try
            {
                var query = "select t1.*,t2.CodeValue as StatusCode from ApplicationMasterChild t1 JOIN CodeMaster t2 ON t1.StatusCodeID=t2.CodeID where t1.StatusCodeID=1 AND t1.ApplicationMasterParentID in(" + Ids + ")";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ApplicationMasterChildModel>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ApplicationMasterChildModel> GetAllByChildIDAsync(long? Id)
        {
            try
            {
                var query = "select t1.*,t2.CodeValue as StatusCode from ApplicationMasterChild t1 JOIN CodeMaster t2 ON t1.StatusCodeID=t2.CodeID where t1.StatusCodeID=1 AND t1.ApplicationMasterChildId =" + Id + "";
                using (var connection = CreateConnection())
                {
                    return await connection.QueryFirstOrDefaultAsync<ApplicationMasterChildModel>(query);
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp.InnerException);
            }
        }
        public async Task<IReadOnlyList<ApplicationMasterChildModel>> GetAllByIdAsync(long? Id)
        {
            try
            {
                var query = "select t1.*,t2.CodeValue as StatusCode,t3.Value as ParentName,t4.Value as ApplicationMasterName,t5.ApplicationMasterName as MainMasterName from ApplicationMasterChild t1 JOIN CodeMaster t2 ON t1.StatusCodeID=t2.CodeID JOIN ApplicationMasterChild t3 ON t3.ApplicationMasterChildID=t1.ParentID LEFT JOIN ApplicationMasterChild t4 ON t4.ApplicationMasterChildID=t3.ParentID  LEFT JOIN ApplicationMasterParent t5 ON t5.ApplicationMasterParentCodeID=t4.ApplicationMasterParentID\r\n where t1.StatusCodeID=1 AND t1.ParentId =" + Id + "";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ApplicationMasterChildModel>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<ApplicationMasterChildModel>> GetAllAsync(string Ids)
        {
            List<ApplicationMasterChildModel> applicationChildData = new List<ApplicationMasterChildModel>();
            try
            {
                var query = "select t1.*,t2.CodeValue as StatusCode,t3.ApplicationMasterName as MainMasterName from ApplicationMasterChild t1 JOIN CodeMaster t2 ON t1.StatusCodeID=t2.CodeID JOIN ApplicationMasterParent t3 ON t3.ApplicationMasterParentCodeID=t1.ApplicationMasterParentID where  t1.ApplicationMasterParentID in(" + Ids + ")";
                using (var connection = CreateConnection())
                {
                    var result = (await connection.QueryAsync<ApplicationMasterChildModel>(query)).ToList();
                    result.ForEach(s =>
                    {
                        if (!s.ParentId.HasValue)
                        {
                            ApplicationMasterChildModel applicationChildDataResponse = new ApplicationMasterChildModel
                            {
                                ID = s.ApplicationMasterChildId,
                                ApplicationMasterChildId = s.ApplicationMasterChildId,
                                ApplicationMasterParentId = s.ApplicationMasterParentId,
                                MainMasterName=s.MainMasterName,
                                Value = s.Value,
                                StatusCodeId = s.StatusCodeId,
                                Description = s.Description,
                                AddedByUserId = s.AddedByUserId,
                                ParentId = s.ParentId,
                                Label = s.Value,
                                StatusCode=s.StatusCode,
                            };
                            applicationChildData.Add(applicationChildDataResponse);
                        }
                        else
                        {
                            var applicationChild = applicationChildData.FirstOrDefault(a => a.ApplicationMasterChildId == s.ParentId);
                            if (applicationChild != null)
                            {
                                applicationChild.Children.Add(new ApplicationMasterChildModel
                                {
                                    ApplicationMasterChildId = s.ApplicationMasterChildId,
                                    ApplicationMasterParentId = s.ApplicationMasterParentId,
                                    MainMasterName = s.MainMasterName,
                                    Value = s.Value,
                                    StatusCodeId = s.StatusCodeId,
                                    Description = s.Description,
                                    AddedByUserId = s.AddedByUserId,
                                    ParentId = s.ParentId,
                                    ID = s.ApplicationMasterChildId,
                                    Label = s.Value,
                                    StatusCode = s.StatusCode,
                                });
                            }
                            else
                            {
                                applicationChildData.ToList().ForEach(applicationChildModel =>
                                {
                                    AddChildLevelData(applicationChildModel, s);
                                });
                            }
                        }
                    });
                }
                return applicationChildData;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        private void AddChildLevelData(ApplicationMasterChildModel applicationChildModel, ApplicationMasterChildModel childData)
        {
            applicationChildModel.Children.ToList().ForEach(parent =>
            {
                if (parent.ApplicationMasterChildId == childData.ParentId)
                {
                    parent.Children.Add(new ApplicationMasterChildModel
                    {
                        ApplicationMasterChildId = childData.ApplicationMasterChildId,
                        ApplicationMasterParentId = childData.ApplicationMasterParentId,
                        Value = childData.Value,
                        StatusCodeId = childData.StatusCodeId,
                        Description = childData.Description,
                        AddedByUserId = childData.AddedByUserId,
                        ParentId = childData.ParentId,
                        ID = childData.ApplicationMasterChildId,
                        Label = childData.Value,
                        StatusCode = childData.StatusCode,
                    });
                }
                else
                {
                    AddChildLevelData(parent, childData);
                }
            });
        }

        public async Task<IReadOnlyList<ApplicationMasterChildModel>> GetAllByProAsync()
        {
            try
            {

                var query = "select t1.*,t2.CodeValue as StatusCode from ApplicationMasterChild t1 JOIN CodeMaster t2 ON t1.StatusCodeID=t2.CodeID where t1.StatusCodeID=1";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ApplicationMasterChildModel>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }

        }
    }

}
