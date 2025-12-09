using Application.Queries.Base;
using Core.Entities;
using Core.Entities.Views;
using Core.EntityModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetAllPlantQuery : PagedRequest, IRequest<List<ViewPlants>>
    {
        public string SearchString { get; set; }
    }
    public class GetAllPlantByNavCompanyQuery : PagedRequest, IRequest<List<ViewPlants>>
    {
        public string SearchString { get; set; }
    }
    public class GetHRMasterAuditList : PagedRequest, IRequest<List<HRMasterAuditTrail>>
    {
        public long? MasterId { get; set; }
        public string? MasterType { get; set; }
        public bool? IsDeleted { get; set; } = false;
        public Guid? SessionId { get; set; }
        public string? AddTypeId { get; set; }
        public GetHRMasterAuditList(long? masterId, string? masterType, bool? isDeleted, Guid? SessionId, string? addTypeId = "")
        {
            this.MasterId = masterId;
            this.MasterType = masterType;
            this.IsDeleted = isDeleted;
            this.SessionId = SessionId;
            this.AddTypeId = addTypeId;
        }
    }
    public class GetHRMasterSWAuditList : PagedRequest, IRequest<List<FileProfileTypeModel>>
    {
        public string? MasterType { get; set; }
        public bool? IsDeleted { get; set; } = false;

        public GetHRMasterSWAuditList(string? masterType, bool? isDeleted)
        {
            this.MasterType = masterType;
            this.IsDeleted = isDeleted;
        }
    }
    public class GetHRMasterApplicationPermissionAuditList : PagedRequest, IRequest<List<ApplicationPermission>>
    {

    }

}
