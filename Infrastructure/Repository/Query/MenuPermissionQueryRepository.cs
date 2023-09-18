using Core.Repositories.Query;
using Infrastructure.Repository.Query.Base;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data;
using Core.Entities.Views;
using Core.EntityModels;
using Core.Entities;
using Newtonsoft.Json.Linq;


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

                var permissionQuery = @"Select  * from view_UserPermission where UserID = @UserID and IsDisplay=1 and IsNewPortal =1 and IsCmsApp =1  and (IsMobile is null or IsMobile=0) ORDER BY PermissionOrder;";
                var DashboardQuery = @"Select  * from ApplicationPermission where PermissionID=60001 ";
                var parameters = new DynamicParameters();
                parameters.Add("UserID", Id, DbType.Int64);
                using (var connection = CreateConnection())
                {
                    var query = permissionQuery + DashboardQuery;
                    var results = await connection.QueryMultipleAsync(query, parameters);

                    var applicationUser = results.Read<SpUserPermission>().ToList();
                    if (applicationUser != null && applicationUser.Count > 0)
                    {
                        var Dashboard = results.Read<SpUserPermission>().FirstOrDefault();
                        if (Dashboard != null)
                        {
                            var exits = applicationUser.FirstOrDefault(a => a.PermissionID == Dashboard.PermissionID);
                            if (exits == null)
                            {
                                applicationUser = applicationUser.Prepend(Dashboard).ToList();
                            }
                        }
                    }

                    var menuList = new List<PortalMenuModel>();
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
                            PermissionID = m.PermissionID
                        };
                        menuList.Add(menu);
                    });
                    return menuList.Where(w => w.Header != "HRMS App").ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
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
