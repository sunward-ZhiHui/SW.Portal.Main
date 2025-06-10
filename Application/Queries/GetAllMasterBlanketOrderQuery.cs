using Application.Queries.Base;
using Core.Entities;
using Core.Entities.Views;
using Core.EntityModels;
using Core.Repositories.Query;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetAllMasterBlanketOrderQuery : PagedRequest, IRequest<List<MasterBlanketOrderModel>>
    {
        public string? SearchString { get; set; }
    }
    public class InsertOrUpdateMasterBlanketOrder : MasterBlanketOrderModel, IRequest<MasterBlanketOrderModel>
    {

    }
    public class DeleteMasterBlanketOrder : MasterBlanketOrderModel, IRequest<MasterBlanketOrderModel>
    {
        public MasterBlanketOrderModel MasterBlanketOrderModel { get; set; }
        public DeleteMasterBlanketOrder(MasterBlanketOrderModel masterBlanketOrderModel)
        {
            this.MasterBlanketOrderModel = masterBlanketOrderModel;
        }
    }
}
