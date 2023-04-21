using AutoMapper;
using Application.Commands;
using Application.Response;
using Core.Entities;

namespace Application.Common.Mapper
{
    internal class CMSMappingProfile : Profile
    {
        public CMSMappingProfile()
        {
            CreateMap<ApplicationRole, RoleResponse>().ReverseMap();
            CreateMap<ApplicationRole, CreateRoleCommand>().ReverseMap();
            CreateMap<ApplicationRole, EditRoleCommand>().ReverseMap();
        }
    }
}
