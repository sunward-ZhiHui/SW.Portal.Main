using Application.Queries.Base;
using Core.Entities;
using Core.Entities.Views;
using Core.EntityModel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetAllApplicationMasterDetailQuery : IRequest<List<View_ApplicationMasterDetail>>
    {
        public long? Id { get; private set; }

        public GetAllApplicationMasterDetailQuery(long? Id)
        {
            this.Id = Id;
        }
    }
    public class GetAllApplicationMasterAllDetailQuery : IRequest<List<View_ApplicationMasterDetail>>
    {
        public long? Id { get; private set; }

        public GetAllApplicationMasterAllDetailQuery(long? Id)
        {
            this.Id = Id;
        }
    }
    public class GetAllApplicationMasterQuery : IRequest<List<ApplicationMaster>>
    {
        public string? SearchString { get; private set; }

    }
    public class GetAllApplicationMasterresultQuery : IRequest<List<ApplicationMasterDetail>>
    {
        public string? SearchString { get; private set; }

    }
    public class InsertApplicationMaster : ApplicationMaster, IRequest<ApplicationMaster>
    {
        
    }
    public class GetApplicationMasterAccessSecurityList : IRequest<List<ApplicationMasterAccess>>
    {
        public long? Id { get; private set; }
        public string? AccessTypeFrom { get; set; }
        public GetApplicationMasterAccessSecurityList(long? Id, string? accessTypeFrom)
        {
            this.Id = Id;
            this.AccessTypeFrom = accessTypeFrom;
        }
    }
    public class InsertApplicationMasterAccessSecurity : ApplicationMasterAccess, IRequest<ApplicationMasterAccess>
    {
        public ApplicationMasterAccess ApplicationMasterAccess { get; private set; }

        public InsertApplicationMasterAccessSecurity(ApplicationMasterAccess applicationMasterAccess)
        {
            this.ApplicationMasterAccess = applicationMasterAccess;
        }
    }
    public class DeleteApplicationMasterAccess : ApplicationMasterAccess, IRequest<long?>
    {
        public long? Id { get; set; }
        public List<long?> Ids { get; set; }

        public DeleteApplicationMasterAccess(long? id, List<long?> ids)
        {
            this.Id = id;
            this.Ids = ids;
        }
    }
    public class GetApplicationMasterAccessSecurityByMaster : IRequest<List<ApplicationMasterAccess>>
    {
        public long? Id { get; private set; }
        public string? AccessTypeFrom { get; set; }
        public GetApplicationMasterAccessSecurityByMaster(long? Id, string? accessTypeFrom)
        {
            this.Id = Id;
            this.AccessTypeFrom = accessTypeFrom;
        }
    }
    
}
