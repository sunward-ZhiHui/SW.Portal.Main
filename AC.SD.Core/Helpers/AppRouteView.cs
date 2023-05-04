using AC.SD.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components;
using System.Net;
using System;
using System.Linq.Expressions;



namespace AC.SD.Core.Helpers
{
    public class AppRouteView : RouteView
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public IAccountService AccountService { get; set; }

        protected override void Render(RenderTreeBuilder builder)
        {
            var authorize = Attribute.GetCustomAttribute(RouteData.PageType, typeof(AuthorizeAttribute)) != null;
            if (authorize && AccountService.User == null)
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
