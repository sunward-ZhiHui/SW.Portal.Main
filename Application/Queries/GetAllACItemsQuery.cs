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
    public class GetAllACItemsQuery : PagedRequest, IRequest<List<ACItemsModel>>
    {
        public string? SearchString { get; set; }
        public ACItemsModel ACItemsModel { get; set; }
        public GetAllACItemsQuery(ACItemsModel aCItemsModel)
        {
            this.ACItemsModel = aCItemsModel;
        }
    }

    public class GetDDACItems : PagedRequest, IRequest<List<ACItemsModel>>
    {
    }
    public class InsertOrUpdateAcitems : ACItemsModel, IRequest<ACItemsModel>
    {

    }
    public class GetNavItemCitemList : PagedRequest, IRequest<List<NavItemCitemList>>
    {
        public long? ItemId { get; set; }
        public GetNavItemCitemList(long? itemId)
        {
            this.ItemId = itemId;
        }
    }

    public class DeleteACItems : ACEntryModel, IRequest<ACItemsModel>
    {
        public ACItemsModel ACItemsModel { get; private set; }
        public DeleteACItems(ACItemsModel aCEntryModel)
        {
            this.ACItemsModel = aCEntryModel;
        }
    }
}
