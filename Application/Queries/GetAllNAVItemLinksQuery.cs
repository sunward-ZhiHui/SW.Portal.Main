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
using static Core.EntityModels.SyrupPlanning;

namespace Application.Queries
{
    public class GetAllStockInformationMasterQuery : PagedRequest, IRequest<List<StockInformationMaster>>
    {
        public string? SearchString { get; set; }
    }
    public class InsertOrUpdateStockInformationMaster : StockInformationMaster, IRequest<StockInformationMaster>
    {

    }
    public class DeleteStockInformationMaster : StockInformationMaster, IRequest<StockInformationMaster>
    {
        public StockInformationMaster StockInformationMaster { get; private set; }
        public DeleteStockInformationMaster(StockInformationMaster stockInformationMaster)
        {
            this.StockInformationMaster = stockInformationMaster;
        }
    }

    public class GetSyrupPlanningQuery : IRequest<List<SyrupPlanning>>
    {
        
    }

    public class GetSyrupProcessNameListQuery : IRequest<List<SyrupProcessNameList>>
    {

    }
    public class GetSyrupFillingListQuery : IRequest<List<SyrupFilling>>
    {

    }
    public class GetSyrupOtherProcessListQuery : IRequest<List<SyrupOtherProcess>>
    {

    }
    public class GetSyrupSimplexDataList : IRequest<SyrupPlanning>
    {
        public long MethodCodeID { get; set; }

        public GetSyrupSimplexDataList(long methodCodeID)
        {
            MethodCodeID = methodCodeID;
        }
    }
    public class GetSyruppreparationDataList : IRequest<SyrupPlanning>
    {
        public long MethodCodeID { get; set; }  
        public GetSyruppreparationDataList(long methodCodeID)
        {
            MethodCodeID = methodCodeID;
        }
    }
    

}
