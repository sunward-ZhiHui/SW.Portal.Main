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
using Application.Command.EmployeeOtherDutyInformations;
using Application.Command.EmployeeEmailInfos;
using Core.Repositories.Command;
using Application.Command.EmployeeICTInformations;
using Application.Handlers.CommandHandler.AssetCatalogMasters;
using Application.Command.AssetCatalogMasters;
using Application.Command.AssetPartsMaintenaceMasters;
using Application.Command.SoSalesOrder;
using Application.Command.SoSalesOrderLine;

namespace Application.Common.Mapper
{
    internal class CMSMappingProfile : Profile
    {
        public CMSMappingProfile()
        {
            CreateMap<ApplicationRole, RoleResponse>().ReverseMap();
            CreateMap<ApplicationRole, CreateRoleCommand>().ReverseMap();
            CreateMap<ApplicationRole, EditRoleCommand>().ReverseMap();

            CreateMap<Documents, DocumentsResponse>().ReverseMap();

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
            CreateMap<EmailTopics, EmailTopicsResponse>().ReverseMap();
            CreateMap<ActivityEmailTopics, ActivityEmailTopicsResponse>().ReverseMap();

            CreateMap<SoCustomer, SoCustomerResponse>().ReverseMap();

            CreateMap<ApplicationUser, ApplicationUserResponse>().ReverseMap();
            CreateMap<ForumTypes, ForumTypeResponse>().ReverseMap();


            CreateMap<LevelMaster, CreateLevelMasterCommand>().ReverseMap();
            CreateMap<LevelMaster, EditLevelMasterCommand>().ReverseMap();
            CreateMap<LevelMaster, DeleteLevelMasterCommand>().ReverseMap();

            CreateMap<Designation, CreateDesignationCommand>().ReverseMap();
            CreateMap<Designation, EditDesignationCommand>().ReverseMap();
            CreateMap<Designation, DeleteDesignationCommand>().ReverseMap();

            CreateMap<Ictmaster, CreateIctmasterCommand>().ReverseMap();
            CreateMap<Ictmaster, EditIctmasterCommand>().ReverseMap();
            CreateMap<Ictmaster, DeleteIctmasterCommand>().ReverseMap();


            CreateMap<LayoutPlanType, CreateLayOutPlanTypeMasterCommand>().ReverseMap();
            CreateMap<LayoutPlanType, EditLayOutPlanTypeMasterCommand>().ReverseMap();
            CreateMap<LayoutPlanType, DeleteLayOutPlanTypeMasterCommand>().ReverseMap();

            CreateMap<EmployeeOtherDutyInformation, CreateEmployeeOtherDutyInformationCommand>().ReverseMap();
            CreateMap<EmployeeOtherDutyInformation, EditEmployeeOtherDutyInformationCommand>().ReverseMap();
            CreateMap<EmployeeOtherDutyInformation, DeleteEmployeeOtherDutyInformationCommand>().ReverseMap();

            CreateMap<EmployeeEmailInfo, CreateEmployeeEmailInfoCommand>().ReverseMap();
            CreateMap<EmployeeEmailInfo, EditEmployeeEmailInfoCommand>().ReverseMap();
            CreateMap<EmployeeEmailInfo, DeleteEmployeeEmailInfoCommand>().ReverseMap();

            CreateMap<EmployeeEmailInfoForward, CreateEmployeeEmailInfoForwardCommand>().ReverseMap();
            CreateMap<EmployeeEmailInfoForward, EditEmployeeEmailInfoForwardCommand>().ReverseMap();
            CreateMap<EmployeeEmailInfoForward, DeleteEmployeeEmailInfoForwardCommand>().ReverseMap();

            CreateMap<EmployeeEmailInfoAuthority, CreateEmployeeEmailInfoAuthorityCommand>().ReverseMap();
            CreateMap<EmployeeEmailInfoAuthority, EditEmployeeEmailInfoAuthorityCommand>().ReverseMap();
            CreateMap<EmployeeEmailInfoAuthority, DeleteEmployeeEmailInfoAuthorityCommand>().ReverseMap();

            CreateMap<EmployeeICTInformation, CreateEmployeeICTInformationCommand>().ReverseMap();
            CreateMap<EmployeeICTInformation, EditEmployeeICTInformationCommand>().ReverseMap();
            CreateMap<EmployeeICTInformation, DeleteEmployeeICTInformationCommand>().ReverseMap();

            CreateMap<EmployeeICTHardInformation, CreateEmployeeICTHardInformationCommand>().ReverseMap();
            CreateMap<EmployeeICTHardInformation, EditEmployeeICTHardInformationCommand>().ReverseMap();
            CreateMap<EmployeeICTHardInformation, DeleteEmployeeICTHardInformationCommand>().ReverseMap();


            CreateMap<AssetCatalogMaster, CreateAssetCatalogMasterCommand>().ReverseMap();
            CreateMap<AssetCatalogMaster, EditAssetCatalogMasterCommand>().ReverseMap();
            CreateMap<AssetCatalogMaster, DeleteAssetCatalogMasterCommand>().ReverseMap();

            CreateMap<AssetPartsMaintenaceMaster, CreateAssetPartsMaintenaceMasterCommand>().ReverseMap();
            CreateMap<AssetPartsMaintenaceMaster, EditAssetPartsMaintenaceMasterCommand>().ReverseMap();
            CreateMap<AssetPartsMaintenaceMaster, DeleteAssetPartsMaintenaceMasterCommand>().ReverseMap();

            CreateMap<AssetEquipmentMaintenaceMaster, CreateAssetEquipmentMaintenaceMasterCommand>().ReverseMap();
            CreateMap<AssetEquipmentMaintenaceMaster, EditAssetEquipmentMaintenaceMasterCommand>().ReverseMap();
            CreateMap<AssetEquipmentMaintenaceMaster, DeleteAssetEquipmentMaintenaceMasterCommand>().ReverseMap();

            CreateMap<SoSalesOrder, SoSalesOrderResponse>().ReverseMap();
            CreateMap<SoSalesOrder, CreateSoSalesOrderCommand>().ReverseMap();
            CreateMap<SoSalesOrder, EditSoSalesOrderCommand>().ReverseMap();
            CreateMap<SoSalesOrder, DeleteSoSalesOrderCommand>().ReverseMap();

            CreateMap<SoSalesOrderLine, SoSalesOrderLineResponse>().ReverseMap();
            CreateMap<SoSalesOrderLine, CreateSoSalesOrderLineCommand>().ReverseMap();
            CreateMap<SoSalesOrderLine, EditSoSalesOrderLineCommand>().ReverseMap();
            CreateMap<SoSalesOrderLine, DeleteSoSalesOrderLineCommand>().ReverseMap();
        }
    }
}
