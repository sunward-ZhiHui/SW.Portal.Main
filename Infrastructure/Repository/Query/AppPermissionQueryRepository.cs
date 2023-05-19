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
using Core.EntityModel;
using Core.Entities.Views;
using Core.Entities;
using Application.Response;
using Core.EntityModels;

namespace Infrastructure.Repository.Query
{
    public class AppPermissionQueryRepository : QueryRepository<AppPermissionModel>, IAppPermissionQueryRepository
    {
        public AppPermissionQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<IReadOnlyList<AppPermissionModel>> GetAllAsync()
        {
            List<AppPermissionModel> applicationPermissions = new List<AppPermissionModel>();
          /*  try
            {*/
                var query = "select  * from AppPermission";

                using (var connection = CreateConnection())
                {
                    var permissions = (await connection.QueryAsync<AppPermission>(query)).ToList();
                    if (permissions.Count > 0)
                    {
                        permissions.ForEach(s =>
                        {
                            if (!s.ParentID.HasValue)
                            {
                                AppPermissionModel appPermissionResponse = new AppPermissionModel
                                {
                                    AppPermissionID = s.AppPermissionID,
                                    ParentID = s.ParentID,
                                    AppID = s.AppID,
                                    Title = s.Title,
                                    Description = s.Description,
                                    Url = s.Url,
                                };
                                applicationPermissions.Add(appPermissionResponse);
                            }
                            else
                            {
                                var applicationPermission = applicationPermissions.FirstOrDefault(a => a.AppID == s.ParentID);
                                if (applicationPermission != null)
                                {
                                    applicationPermission.Children.Add(new AppPermissionModel
                                    {
                                        AppPermissionID = s.AppPermissionID,
                                        ParentID = s.ParentID,
                                        AppID = s.AppID,
                                        Title = s.Title,
                                        Description = s.Description,
                                        Url = s.Url,
                                    });
                                }
                                else
                                {
                                    applicationPermissions.ToList().ForEach(applicationPermissionModel =>
                                    {
                                        AddChildLevelPermission(applicationPermissionModel, s);
                                    });
                                }
                            }
                        });
                    }
                    return applicationPermissions.ToList();
            }
            /*}
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }*/
        }
        private void AddChildLevelPermission(AppPermissionModel applicationPermissionModel, AppPermission childPermission)
        {
            applicationPermissionModel.Children.ToList().ForEach(parent =>
            {
                if (parent.AppID == childPermission.ParentID)
                {
                    parent.Children.Add(new AppPermissionModel
                    {
                        AppPermissionID = childPermission.AppPermissionID,
                        ParentID = childPermission.ParentID,
                        AppID = childPermission.AppID,
                        Title = childPermission.Title,
                        Description = childPermission.Description,
                        Url = childPermission.Url,
                    });
                }
                else
                {
                    AddChildLevelPermission(parent, childPermission);
                }
            });
        }
        public List<AppPermissionModel> GetAllByAsync()
        {
            List<AppPermissionModel> applicationPermissions = new List<AppPermissionModel>();
            /*  try
              {*/
            var query = "select  * from AppPermission";

            using (var connection = CreateConnection())
            {
                var permissions = (connection.Query<AppPermission>(query)).ToList();
                if (permissions.Count > 0)
                {
                    permissions.ForEach(s =>
                    {
                        if (!s.ParentID.HasValue)
                        {
                            AppPermissionModel appPermissionResponse = new AppPermissionModel
                            {
                                AppPermissionID = s.AppPermissionID,
                                ParentID = s.ParentID,
                                AppID = s.AppID,
                                Title = s.Title,
                                Description = s.Description,
                                Url = s.Url,
                            };
                            applicationPermissions.Add(appPermissionResponse);
                        }
                        else
                        {
                            var applicationPermission = applicationPermissions.FirstOrDefault(a => a.AppID == s.ParentID);
                            if (applicationPermission != null)
                            {
                                applicationPermission.Children.Add(new AppPermissionModel
                                {
                                    AppPermissionID = s.AppPermissionID,
                                    ParentID = s.ParentID,
                                    AppID = s.AppID,
                                    Title = s.Title,
                                    Description = s.Description,
                                    Url = s.Url,
                                });
                            }
                            else
                            {
                                applicationPermissions.ToList().ForEach(applicationPermissionModel =>
                                {
                                    AddChildLevelPermission(applicationPermissionModel, s);
                                });
                            }
                        }
                    });
                }
                return applicationPermissions.ToList();
            }
            /*}
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }*/
        }
        public async Task<AppPermissionModel> GetByIdAsync(long id)
        {
            try
            {
                var query = "SELECT * FROM AppPermission WHERE AppID = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<AppPermissionModel>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

    }
}
