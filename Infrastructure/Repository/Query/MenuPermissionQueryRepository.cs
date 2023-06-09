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
using Core.EntityModels;
using Microsoft.EntityFrameworkCore;
using Core.Entities;
using Microsoft.JSInterop;
using System.Text.Json;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Net;

namespace Infrastructure.Repository.Query
{
    public class MenuPermissionQueryRepository : QueryRepository<PortalMenuModel>, IMenuPermissionQueryRepository
    {
        private readonly ILocalStorageService<ApplicationUser> _localStorageService;
        private IJSRuntime _jsRuntime;
        private Blazored.SessionStorage.ISessionStorageService _sessionStorage;
        public MenuPermissionQueryRepository(IConfiguration configuration, ILocalStorageService<ApplicationUser> localStorageService, IJSRuntime jsRuntime, Blazored.SessionStorage.ISessionStorageService sessionStorage)
            : base(configuration)
        {
            _localStorageService = localStorageService;
            _jsRuntime = jsRuntime;
            _sessionStorage = sessionStorage;
        }
        public IReadOnlyList<PortalMenuModel> GetAllByPermissionAsync(long? Id)
        {
            /* try
             {*/
            /*var result = await GetAsync();*/
            var query = "Select  * from view_UserPermission where UserID = @UserID and IsNewPortal =1 and IsCmsApp =1  and (IsMobile is null or IsMobile=0) ORDER BY PermissionOrder";
                var parameters = new DynamicParameters();
                parameters.Add("UserID", Id, DbType.Int64);
                using (var connection = CreateConnection())
                {
                    var applicationUser = (connection.Query<SpUserPermission>(query, parameters)).ToList();
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
                    return menuList.Where(w => w.Header != "CMS App").ToList();
                }
            /*}
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }*/
        }
        public async Task<long?> GetAsync()
        {
            var result = await _localStorageService.GetItem<ApplicationUser>("user");
            return result.UserID;
            
        }
        public async Task<IReadOnlyList<PortalMenuModel>> GetAllAsync(long? Id)
        {
            /*try
            {*/
                var result = await _sessionStorage.GetItemAsync<string>("UserID");
                var query = "Select  * from view_UserPermission where UserID = @UserID and IsNewPortal =1 and IsCmsApp =1  and (IsMobile is null or IsMobile=0) ORDER BY PermissionOrder";
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
            /*}
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }*/
        }

    }
}
