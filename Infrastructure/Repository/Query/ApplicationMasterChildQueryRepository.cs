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

namespace Infrastructure.Repository.Query
{
    public class ApplicationMasterChildQueryRepository : QueryRepository<ApplicationMasterChildModel>, IApplicationMasterChildQueryRepository
    {
        public ApplicationMasterChildQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<IReadOnlyList<ApplicationMasterChildModel>> GetAllByAsync(string Ids)
        {
            try
            {
                var query = "select * from ApplicationMasterChild where ApplicationMasterParentID in(" + Ids + ")";
                using (var connection = CreateConnection())
                {
                    return  (await connection.QueryAsync<ApplicationMasterChildModel>(query)).ToList();
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
                var query = "select * from ApplicationMasterChild where ApplicationMasterParentID in(" + Ids + ")";
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
                                Value = s.Value,
                                StatusCodeId = s.StatusCodeId,
                                Description = s.Description,
                                AddedByUserId = s.AddedByUserId,
                                ParentId = s.ParentId,
                                Label = s.Value,
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
                                    Value = s.Value,
                                    StatusCodeId = s.StatusCodeId,
                                    Description = s.Description,
                                    AddedByUserId = s.AddedByUserId,
                                    ParentId = s.ParentId,
                                    ID = s.ApplicationMasterChildId,
                                    Label = s.Value,
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
                    });
                }
                else
                {
                    AddChildLevelData(parent, childData);
                }
            });
        }
    }
}
