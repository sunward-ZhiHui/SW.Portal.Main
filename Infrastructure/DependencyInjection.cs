using Application.Commands;
using Application.Handlers.CommandHandler;
using Application.Response;
using Core.Entities;
using Core.Repositories.Command;
using Core.Repositories.Command.Base;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using Infrastructure.Repository.Command;
using Infrastructure.Repository.Command.Base;
using Infrastructure.Repository.Query;
using Infrastructure.Repository.Query.Base;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped(typeof(IQueryRepository<>), typeof(QueryRepository<>));
            services.AddTransient<IRoleQueryRepository, RoleQueryRepository>();
            services.AddScoped(typeof(ICommandRepository<>), typeof(CommandRepository<>));
            services.AddTransient<IRoleCommandRepository, RoleCommandRepository>();

            //services.AddScoped(typeof(ICommandRepository<ForumTypeResponse>), typeof(CommandRepository<ForumTypeResponse>));

            services.AddScoped(typeof(IRequestHandler<>), typeof(CreateHandler<>));
            services.AddTransient<IPlantQueryRepository, PlantQueryRepository>();
            services.AddTransient<IDivisionQueryRepository, DivisionQueryRepository>();
            services.AddTransient<ICodeMasterQueryRepository, CodeMasterQueryRepository>();
            services.AddTransient<IDepartmentQueryRepository, DepartmentQueryRepository>();
            services.AddTransient<ISectionQueryRepository, SectionQueryRepository>();
            services.AddTransient<ISubSectionQueryRepository, SubSectionQueryRepository>();
            services.AddTransient<IDesignationQueryRepository, DesignationQueryRepository>();
            services.AddTransient<ILayOutPlanTypeQueryRepository,LayOutPlanTypeQueryRepository>();
            services.AddTransient<IEmployeeQueryRepository, EmployeeQueryRepository>();

            services.AddTransient<IFourmCategoryQueryRepository, FourmCategoryQueryRepository>();
            services.AddTransient<ILevelMasterQueryRepository, LevelMasterQueryRepository>();
            services.AddTransient<IForumTypeQueryRepository, ForumTypeQueryRepository>();
            services.AddTransient<IForumTypeCommandRepository, ForumTypeCommandRepository>();
            services.AddTransient<IFourmCategoryCommandRepository, FourmCategoryCommandRepository>();
            services.AddTransient<IForumTopicsQueryRepository, ForumTopicsQueryRepository>();



            services.AddTransient<IPlantCommandRepository, PlantCommandRepository>();
            services.AddTransient<IDivisionCommandRepository, DivisionCommandRepository>();
            services.AddTransient<IDepartmentCommandRepository, DepartmentCommandRepository>();
            services.AddTransient<ISubSectionCommandRepository, SubSectionCommandRepository>();
            services.AddTransient<IDesignationCommandRepository, DesignationCommandRepository>();


            services.AddTransient<IApplicationUserQueryRepository, ApplicationUserQueryRepository>();
            services.AddScoped(typeof(ILocalStorageService<>), typeof(LocalStorageService<>));




            return services;
        }
    }
}
