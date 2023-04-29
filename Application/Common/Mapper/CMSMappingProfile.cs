using AutoMapper;
using Application.Commands;
using Application.Response;
using Core.Entities;
using Application.Queries;
using CMS.Application.Handlers.QueryHandlers;

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


            CreateMap<ForumCategorys, ForumCategoryResponse>().ReverseMap();
        }
    }
}
