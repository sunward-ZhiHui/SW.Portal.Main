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
    public class GetAllTenderOrderQuery : PagedRequest, IRequest<List<TenderOrderModel>>
    {
        public string? SearchString { get; set; }
    }
    public class InsertOrUpdateTenderOrder : TenderOrderModel, IRequest<TenderOrderModel>
    {

    }
    public class DeleteTenderOrder : TenderOrderModel, IRequest<TenderOrderModel>
    {
        public TenderOrderModel TenderOrderModel { get; private set; }
        public DeleteTenderOrder(TenderOrderModel tenderOrderModel)
        {
            this.TenderOrderModel = tenderOrderModel;
        }
    }
}
