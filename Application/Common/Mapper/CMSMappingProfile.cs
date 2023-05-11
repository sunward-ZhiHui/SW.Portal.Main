using AutoMapper;
using Application.Commands;
using Application.Response;
using Core.Entities;
using Application.Queries;
using CMS.Application.Handlers.QueryHandlers;
using Application.Command.Departments;
using Application.Command.Sections;
using Application.Command.SubSections;
using Application.Command.ForumCategory;
using Application.Command.LeveMasters;
using Application.Command.designations;
using Application.Command.LayoutPlanType;
using Application.Command.Ictmaster;

namespace Application.Common.Mapper
{
    internal class CMSMappingProfile : Profile
    {
        public CMSMappingProfile()
        {
            CreateMap<ApplicationRole, RoleResponse>().ReverseMap();
            CreateMap<ApplicationRole, CreateRoleCommand>().ReverseMap();
            CreateMap<ApplicationRole, EditRoleCommand>().ReverseMap();


            CreateMap<ForumTypes, ForumTypeResponse>().ReverseMap();
            CreateMap<ForumTypes, CreateForumTypeCommand>().ReverseMap();
            CreateMap<ForumTypes, EditForumTypeCommand>().ReverseMap();
            CreateMap<Plant, CreatePlantCommand>().ReverseMap();
            CreateMap<Plant, EditPlantCommand>().ReverseMap();
            CreateMap<Plant, DeletePlantCommand>().ReverseMap();

            CreateMap<Division, CreateDivisionCommand>().ReverseMap();
            CreateMap<Division, EditDivisionCommand>().ReverseMap();
            CreateMap<Division, DeleteDivisionCommand>().ReverseMap();

            CreateMap<Department, CreateDepartmentCommand>().ReverseMap();
            CreateMap<Department, EditDepartmentCommand>().ReverseMap();
            CreateMap<Department, DeleteDepartmentCommand>().ReverseMap();

            CreateMap<Section, CreateSectionCommand>().ReverseMap();
            CreateMap<Section, EditSectionCommand>().ReverseMap();
            CreateMap<Section, DeleteSectionCommand>().ReverseMap();

            CreateMap<SubSection, CreateSubSectionCommand>().ReverseMap();
            CreateMap<SubSection, EditSubSectionCommand>().ReverseMap();
            CreateMap<SubSection, DeleteSubSectionCommand>().ReverseMap();

          

            CreateMap<ForumCategorys, CreateForumCategoryCommand>().ReverseMap();
            CreateMap<ForumCategorys, EditFourmCategoryCommand>().ReverseMap();
            CreateMap<ForumCategorys, DeleteFourmCategoryCommand>().ReverseMap();

            CreateMap<ForumCategorys, ForumCategoryResponse>().ReverseMap();
            CreateMap<ForumTopics, ForumTopicsResponse>().ReverseMap();
            CreateMap<ApplicationUser, ApplicationUserResponse>().ReverseMap();


            CreateMap<LevelMaster, CreateLevelMasterCommand>().ReverseMap();
            CreateMap<LevelMaster, EditLevelMasterCommand>().ReverseMap();
            CreateMap<LevelMaster, DeleteLevelMasterCommand>().ReverseMap();

            CreateMap<Designation, CreateDesignationCommand>().ReverseMap();
            CreateMap<Designation, EditDesignationCommand>().ReverseMap();
            CreateMap<Designation, DeleteDesignationCommand>().ReverseMap();

            CreateMap<Ictmaster, CreateIctmasterCommand>().ReverseMap();
            CreateMap<Ictmaster, EditIctmasterCommand>().ReverseMap();
            CreateMap<Ictmaster, DeleteIctmasterCommand>().ReverseMap();


            CreateMap<LayoutPlanType,CreateLayOutPlanTypeMasterCommand>().ReverseMap();
            CreateMap<LayoutPlanType,EditLayOutPlanTypeMasterCommand>().ReverseMap();
            CreateMap<LayoutPlanType, DeleteLayOutPlanTypeMasterCommand>().ReverseMap();
        }
    }
}
