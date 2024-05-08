using AC.SD.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components;
using System.Net;
using System;
using System.Linq.Expressions;
using Core.Entities;
using Core.Repositories.Query;
using Infrastructure.Repository.Query;
using Microsoft.AspNetCore.Components.Authorization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace AC.SD.Core.Helpers
{
    public class AppRouteView : RouteView
    {
        [Inject]
        public required NavigationManager NavigationManager { get; set; }      
        [Inject]
        public required IApplicationUserQueryRepository ApplicationUserQueryRepository { get; set; }

        protected override void Render(RenderTreeBuilder builder)
        {
            var authorize = Attribute.GetCustomAttribute(RouteData.PageType, typeof(AuthorizeAttribute)) != null;   

            if (authorize && ApplicationUserQueryRepository.User == null)
            {                
                NavigationManager.NavigateTo($"Login");
            }
            else
            {
                base.Render(builder);
            }

        }
    }
}
