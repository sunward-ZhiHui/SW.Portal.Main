using AC.SD.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components;
using System.Net;
using System;
using System.Linq.Expressions;
using Core.Entities;
using Core.Repositories.Query;

namespace AC.SD.Core.Helpers
{
    public class AppRouteView : RouteView
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public ILocalStorageService<ApplicationUser> _localStorageService { get; set; }

        protected override async void Render(RenderTreeBuilder builder)
        {
            var authorize = Attribute.GetCustomAttribute(RouteData.PageType, typeof(AuthorizeAttribute)) != null;
            //var checkauthorize = await _localStorageService.GetItem<ApplicationUser>("user");
            
            if (authorize)
            {
                //var returnUrl = WebUtility.UrlEncode(new Uri(NavigationManager.Uri).PathAndQuery);               
                NavigationManager.NavigateTo($"/Login");
            }
            else
            {
                base.Render(builder);
            }
        }
    }
}
