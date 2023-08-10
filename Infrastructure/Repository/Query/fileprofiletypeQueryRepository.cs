using Core.Entities;
using Core.Entities.Views;
using Core.EntityModels;
using Core.Repositories.Query;
using Dapper;
using Infrastructure.Repository.Query.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DevExpress.Xpo.DB.DataStoreLongrunnersWatch;

namespace Infrastructure.Repository.Query
{
    public class FileprofiletypeQueryRepository : QueryRepository<Fileprofiletype>, IFileprofileQueryRepository
    {
        public FileprofiletypeQueryRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<IReadOnlyList<Fileprofiletype>> GetAllAsync(long fileProfileTypeID)
        {
            List<Fileprofiletype> fileprofiletype = new List<Fileprofiletype>();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("FileProfileTypeId", fileProfileTypeID);

                var query = @"WITH RecursiveHierarchy AS(
                                SELECT
                                    FileProfileTypeID,
                                    ParentID,
                                    Name,
                                    FileProfileTypeID AS RootID

                                FROM
                                    FileProfileType
                                WHERE
                                    ParentID IS NULL

                                UNION ALL

                                SELECT
                                    t.FileProfileTypeID,
                                    t.ParentID,
                                    t.Name,
                                    rh.RootID

                                FROM
                                    FileProfileType t
                                JOIN
                                    RecursiveHierarchy rh ON t.ParentID = rh.FileProfileTypeID
                            )
                            SELECT
                                FileProfileTypeID,
                                ParentID,
                                Name,
                                RootID


                            FROM
                                RecursiveHierarchy
                            WHERE
                                RootID = @FileProfileTypeId";

                                
                using (var connection = CreateConnection())
                {
                    var result = (await connection.QueryAsync<Fileprofiletype>(query, parameters)).ToList();
                    result.ForEach(s =>
                    {
                        if (!s.ParentId.HasValue)
                        {
                            Fileprofiletype applicationChildDataResponse = new Fileprofiletype
                            {
                                FileProfileTypeId = s.FileProfileTypeId,
                                ProfileId = s.ProfileId,
                                ParentId = s.ParentId,
                                Name = s.Name,
                                Label = s.Name,
                            };
                            fileprofiletype.Add(applicationChildDataResponse);
                        }
                        else
                        {
                            var applicationChild = fileprofiletype.FirstOrDefault(a => a.FileProfileTypeId == s.ParentId);
                            if (applicationChild != null)
                            {
                                applicationChild.Children.Add(new Fileprofiletype
                                {
                                    FileProfileTypeId = s.FileProfileTypeId,
                                    ProfileId = s.ProfileId,
                                    ParentId = s.ParentId,
                                    Name = s.Name,
                                    Label = s.Name,
                                });
                            }
                            else
                            {
                                fileprofiletype.ToList().ForEach(applicationChildModel =>
                                {
                                    AddChildLevelData(applicationChildModel, s);
                                });
                            }
                        }
                    });
                }
               
                
                return fileprofiletype;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        private void AddChildLevelData(Fileprofiletype applicationChildModel, Fileprofiletype childData)
        {
            applicationChildModel.Children.ToList().ForEach(parent =>
            {
                if (parent.FileProfileTypeId == childData.ParentId)
                {
                    parent.Children.Add(new Fileprofiletype
                    {
                        FileProfileTypeId = childData.FileProfileTypeId,
                        ProfileId = childData.ProfileId,
                        ParentId = childData.ParentId,
                        Name = childData.Name,
                        Label = childData.Name,
                    });
                }
                else
                {
                    AddChildLevelData(parent, childData);
                }
            });
        }

       
        public async Task<IReadOnlyList<view_GetFileProfileTypeDocument>> GetAllSelectedFileAsync(long selectedFileProfileTypeID)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("FileProfileTypeId", selectedFileProfileTypeID);

                var query = "select  * from view_GetFileProfileTypeDocument where FileProfileTypeId=@FileProfileTypeId";

                using (var connection = CreateConnection())
                {
                    return connection.Query<view_GetFileProfileTypeDocument>(query, parameters).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}
