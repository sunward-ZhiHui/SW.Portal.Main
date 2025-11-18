using Core.Repositories.Query;
using Infrastructure.Repository.Query.Base;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data;
using Core.Entities.Views;
using Core.EntityModels;
using Core.Entities;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;


namespace Infrastructure.Repository.Query
{
    public class MenuPermissionQueryRepository : QueryRepository<PortalMenuModel>, IMenuPermissionQueryRepository
    {

        public MenuPermissionQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<IReadOnlyList<PortalMenuModel>> GetAllByAsync(long? Id)
        {
            try
            {
                var dynamicForm = new List<DynamicForm>();
                var menuList = new List<PortalMenuModel>();
                var permissionQuery = @"Select  * from view_UserPermission where   UserID = @UserID AND  IsDisplay=1 and IsNewPortal =1 and IsCmsApp =1  and (IsMobile is null or IsMobile=0) ORDER BY PermissionOrder;";
                var DashboardQuery = @"Select  * from ApplicationPermission where PermissionID>=60001;";
                var FormQuery = @"select ID,SessionID,Name,ScreenID,IsDeleted from DynamicForm where (IsDeleted is null OR IsDeleted=0);";
                var parameters = new DynamicParameters();
                parameters.Add("UserID", Id, DbType.Int64);
                using (var connection = CreateConnection())
                {
                    var query = permissionQuery + DashboardQuery + FormQuery;
                    var results = await connection.QueryMultipleAsync(query, parameters);
                    var spUserPermissionList = new List<SpUserPermission>();
                    var applicationUser = results.Read<SpUserPermission>().ToList();
                    var applicationAllUser = results.Read<SpUserPermission>().ToList();
                    dynamicForm = results.Read<DynamicForm>().ToList();
                    if (applicationUser != null && applicationUser.Count > 0)
                    {
                        var dynamicFormMenu = applicationUser.Where(w => w.ParentID == 60248).ToList();
                        if (dynamicFormMenu != null && dynamicFormMenu.Count > 0)
                        {
                            dynamicFormMenu.ForEach(d =>
                            {
                                var exits = dynamicForm.Where(w => w.SessionID.ToString().ToLower() == d.PermissionURL.ToLower()).Count();
                                if (exits > 0)
                                {

                                }
                                else
                                {
                                    applicationUser.Remove(d);
                                }
                            });

                        }
                        applicationUser.ToList().ForEach(s =>
                        {
                            getNested(s, applicationAllUser, spUserPermissionList);
                        });
                        if (spUserPermissionList != null && spUserPermissionList.Count > 0)
                        {
                            applicationUser.AddRange(spUserPermissionList);
                            applicationUser = applicationUser.OrderBy(o => o.PermissionOrder).ToList();
                            List<long> userIds = new List<long>();
                            userIds = applicationUser.Select(o => o.PermissionID).Distinct().ToList();
                            if (userIds.Count() > 0)
                            {
                                applicationUser = applicationAllUser.Where(w => userIds.Contains(w.PermissionID)).OrderBy(o => o.PermissionOrder).ToList();
                            }
                        }
                        if (applicationAllUser != null && applicationAllUser.Count() > 0)
                        {
                            var Dashboard = applicationAllUser.FirstOrDefault(f => f.PermissionID == 60001);
                            if (Dashboard != null)
                            {
                                var exits = applicationUser.FirstOrDefault(a => a.PermissionID == Dashboard.PermissionID);
                                if (exits == null)
                                {
                                    applicationUser = applicationUser.Prepend(Dashboard).ToList();
                                }
                            }
                        }
                        applicationUser.ToList().ForEach(m =>
                        {
                            var menu = new PortalMenuModel
                            {
                                Header = m.PermissionName,
                                Title = m.PermissionName,
                                Group = null,
                                Component = m.Component,
                                Name = m.Name,
                                MenuOrder = null,
                                Icon = null,
                                Items = null,
                                ScreenID = m.ScreenID,
                                ParentID = m.ParentID,
                                IsPermissionURL = m.IsPermissionURL,
                                UniqueSessionID = m.UniqueSessionID,
                                PermissionID = m.PermissionID,
                                PermissionURL = m.PermissionURL,
                                Description = m.PermissionURL,
                            };
                            menuList.Add(menu);
                        });
                    }

                    return menuList.Where(w => w.Header != "HRMS App").ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public void getNested(SpUserPermission s, List<SpUserPermission> spUserPermissionsList, List<SpUserPermission> spUserPermissionList)
        {
            if (s.ParentID != null)
            {
                var exits = spUserPermissionsList.FirstOrDefault(f => f.PermissionID == s.ParentID);
                if (exits != null)
                {
                    var exiss = spUserPermissionList.FirstOrDefault(f => f.PermissionID == exits.PermissionID);
                    if (exiss == null)
                    {
                        spUserPermissionList.Add(exits);
                    }
                    getNested(exits, spUserPermissionsList, spUserPermissionList);
                }
            }
        }
        public async Task<PortalMenuModel> GetByDashboardAsync()
        {
            try
            {
                PortalMenuModel portalMenuModel = new PortalMenuModel();
                var query = "Select  * from ApplicationPermission where PermissionID=60001 ";
                using (var connection = CreateConnection())
                {
                    var result = (await connection.QueryFirstOrDefaultAsync<SpUserPermission>(query));
                    if (result != null)
                    {

                        portalMenuModel.Header = result.PermissionName;
                        portalMenuModel.Title = result.PermissionName;
                        portalMenuModel.Group = null;
                        portalMenuModel.Component = result.Component;
                        portalMenuModel.Name = result.Name;
                        portalMenuModel.MenuOrder = null;
                        portalMenuModel.Icon = null;
                        portalMenuModel.Items = null;
                        portalMenuModel.ScreenID = result.ScreenID;
                        portalMenuModel.ParentID = result.ParentID;
                        portalMenuModel.PermissionID = result.PermissionID;
                    }
                }
                return portalMenuModel;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<IReadOnlyList<PortalMenuModel>> GetAllAsync(long? Id)
        {
            try
            {
                var query = "Select  * from view_UserPermission where UserID = @UserID and IsDisplay=1 and IsNewPortal =1 and IsCmsApp =1  and (IsMobile is null or IsMobile=0) ORDER BY PermissionOrder";
                var parameters = new DynamicParameters();
                parameters.Add("UserID", Id, DbType.Int64);
                using (var connection = CreateConnection())
                {
                    var applicationUser = (await connection.QueryAsync<SpUserPermission>(query, parameters)).ToList();
                    var menuList = new List<PortalMenuModel>();
                    applicationUser.Where(p => p.ParentID == null && p.IsDisplay).ToList().ForEach(m =>
                    {
                        var childs = applicationUser.Where(c => c.ParentID == m.PermissionID && c.IsDisplay).ToList();
                        var menu = new PortalMenuModel
                        {
                            Header = m.PermissionName,
                            Title = null,
                            Group = null,
                            Component = null,
                            Name = null,
                            MenuOrder = null,
                            Icon = null,
                            Items = null,
                            ScreenID = null,
                        };

                        if (childs.Count > 0)
                        {
                            menuList.Add(menu);
                            childs.ForEach(ch =>
                            {
                                var pchilds = applicationUser.Where(c => c.ParentID == ch.PermissionID && ch.IsDisplay).ToList();
                                var childmenu = new PortalMenuModel
                                {
                                    Title = ch.PermissionName,
                                    Group = ch.PermissionGroup,
                                    Component = ch.Component,
                                    Name = ch.Name,
                                    MenuOrder = ch.PermissionOrder,
                                    Icon = ch.Icon,
                                    Items = null,
                                    ScreenID = ch.ScreenID,
                                    Description = ch.PermissionURL,
                                };

                                if (pchilds.Count > 0)
                                {
                                    childmenu.Items = new List<PortalMenuModel>();
                                    pchilds.ForEach(p =>
                                    {
                                        var ppchilds = applicationUser.Where(c => c.ParentID == p.PermissionID && p.IsDisplay).ToList();
                                        var pchildmenu = new PortalMenuModel
                                        {
                                            Title = p.PermissionName,
                                            Group = p.PermissionGroup,
                                            Component = p.Component,
                                            Name = p.Name,
                                            MenuOrder = p.PermissionOrder,
                                            Icon = p.Icon,
                                            Items = null,
                                            Header = (ppchilds.Count > 0) ? "Yes" : "No",
                                            ScreenID = p.ScreenID,
                                        };
                                        childmenu.Items.Add(pchildmenu);
                                        if (ppchilds.Count > 0 && ppchilds.All(d => d.IsDisplay))
                                        {
                                            pchildmenu.Items = new List<PortalMenuModel>();
                                            ppchilds.ForEach(pp =>
                                            {
                                                var ppchildmenu = new PortalMenuModel
                                                {
                                                    Title = pp.PermissionName,
                                                    Group = pp.PermissionGroup,
                                                    Component = pp.Component,
                                                    Name = pp.Name,
                                                    MenuOrder = pp.PermissionOrder,
                                                    Icon = pp.Icon,
                                                    Items = null,
                                                    ScreenID = pp.ScreenID,
                                                };
                                                pchildmenu.Items.Add(ppchildmenu);
                                            });
                                        }
                                    });
                                }
                                menuList.Add(childmenu);
                            });
                        }

                    });
                    return menuList;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }


    }
}
