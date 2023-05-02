using AutoMapper;
using Application.Commands;
using Application.Response;
using Core.Entities;
using Application.Queries;
using CMS.Application.Handlers.QueryHandlers;
using Application.Command.Departments;
using Application.Command.Sections;
using Application.Command.SubSections;

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

            CreateMap<ForumCategorys, ForumCategoryResponse>().ReverseMap();
            CreateMap<ForumTopics, ForumTopicsResponse>().ReverseMap();
        }
    }
}
