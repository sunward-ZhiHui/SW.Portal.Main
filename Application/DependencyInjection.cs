using Application.Commands;
using Application.Common.Behaviours;
using CMS.Application.Handlers.QueryHandlers;
using Core.Entities;
using Core.Repositories.Command.Base;
using Core.Repositories.Query;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;


using System.Reflection;


namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            //services.AddScoped(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehaviour<,>));

            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            //services.AddScoped(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
            //
            services.AddScoped<LoginHandler>();
            return services;
        }
    }
}
