using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.EntityModels
{
    public class INPCalendarPivotModel : BaseModel
    {
        public int IntMonth { get; set; }
        public string VersionName { get; set; }
        public DateTime ReportDate { get; set; }
        public long? CompanyId { get; set; }
        public long ItemId { get; set; }
        public bool IsSteroid { get; set; }
        public string GrouoItemNo { get; set; }
        public string ItemNo { get; set; }
        public string DistNo { get; set; }
        public long DistAcId { get; set; }
        public string Description { get; set; }
        public string ItemDescriptions { get; set; }
        public string Remarks { get; set; }
        public string MethodCode { get; set; }
        public long? MethodCodeId { get; set; }
        public decimal? SalesCategoryId { get; set; }
        public string UOM { get; set; }
        public string Packuom { get; set; }
        public decimal Quantity { get; set; }
        public string Month { get; set; }
        public long PackSize { get; set; }
        public string Customer { get; set; }
        public string AddhocCust { get; set; }
        public decimal ACQty { get; set; }
        public decimal? AntahQty { get; set; }
        public decimal? ApexQty { get; set; }
        public decimal? MissQty { get; set; }
        public decimal? PxQty { get; set; }
        public decimal? AlpsQty { get; set; }
        public decimal? POQty { get; set; }
        public decimal? SgtQty { get; set; }
        public decimal? PsbQty { get; set; }
        public decimal? ProdNotStartQty { get; set; }
        public string ProdNotStartTickets { get; set; }
        public string ShelfLife { get; set; }
        public string Itemgrouping { get; set; }
        public decimal? SymlQty { get; set; }
        public decimal AcSum { get; set; }
        public decimal UnitQty { get; set; }
        public decimal ThreeMonthACQty { get; set; }
        public string ProdRecipe { get; set; }
        public string BatchSize { get; set; }
        public string NoofTickets { get; set; }
        public string NoOfDays { get; set; }
        public decimal DistStockBalance { get; set; }
        public decimal ApexStockBalance { get; set; }
        public decimal AntahStockBalance { get; set; }
        public decimal SgTenderStockBalance { get; set; }
        public decimal PsbStockBalance { get; set; }
        public decimal MsbStockBalance { get; set; }

        public decimal PreApexStockBalance { get; set; }
        public decimal PreAntahStockBalance { get; set; }
        public decimal PreSgTenderStockBalance { get; set; }
        public decimal PrePsbStockBalance { get; set; }
        public decimal PreMsbStockBalance { get; set; }
        public decimal PreMyStockBalance { get; set; }
        public decimal PreOtherStoreQty { get; set; }
        public decimal PreStockBalance { get; set; }
        public decimal PreStockHoldingBalance { get; set; }
        public decimal PrewipQty { get; set; }
        public decimal PreNotStartInvQty { get; set; }
        public decimal NAVStockBalance { get; set; }
        public decimal SgStockBalance { get; set; }
        public decimal MyStockBalance { get; set; }
        public decimal OtherStoreQty { get; set; }
        public decimal InterCompanyTransitQty { get; set; }
        public decimal WipQty { get; set; }
        public decimal NotStartInvQty { get; set; }
        public decimal Rework { get; set; }
        public decimal NoOfTicket { get; set; }
        public decimal kivQty { get; set; }
        public decimal SWKIVQty { get; set; }
        public decimal fbStock { get; set; }
        public decimal adhocStock { get; set; }
        public decimal StockBalance { get; set; }
        public decimal ActualStockBalance { get; set; }
        public decimal StockHoldingBalance { get; set; }
        public decimal StockHoldingPackSize { get; set; }
        public decimal Month1 { get; set; }
        public decimal Month2 { get; set; }
        public decimal Month3 { get; set; }
        public decimal Month4 { get; set; }
        public decimal Month5 { get; set; }
        public decimal Month6 { get; set; }
        public decimal Month7 { get; set; }
        public decimal Month8 { get; set; }
        public decimal Month9 { get; set; }
        public decimal Month10 { get; set; }
        public decimal Month11 { get; set; }
        public decimal Month12 { get; set; }

        public decimal QtyMonth1 { get; set; }
        public decimal QtyMonth2 { get; set; }
        public decimal QtyMonth3 { get; set; }
        public decimal QtyMonth4 { get; set; }
        public decimal QtyMonth5 { get; set; }
        public decimal QtyMonth6 { get; set; }
        public decimal QtyMonth7 { get; set; }
        public decimal QtyMonth8 { get; set; }
        public decimal QtyMonth9 { get; set; }
        public decimal QtyMonth10 { get; set; }
        public decimal QtyMonth11 { get; set; }
        public decimal QtyMonth12 { get; set; }

        public decimal BatchSize450 { get; set; }
        public string BatchSize90 { get; set; }
        public decimal Roundup1 { get; set; }
        public decimal Roundup2 { get; set; }
        public decimal PackSize1 { get; set; }
        public decimal PackSize2 { get; set; }
        public DateTime ReportMonth { get; set; }
        public string Replenishment { get; set; }
        public List<string> RecipeLists { get; set; }
        public List<NAVRecipesModel> ItemRecipeLists { get; set; }
        public List<NAVRecipesModel> OrderRecipeLists { get; set; }
        public List<INPCalendarPivotModel> Children { get; set; }
        public int? StatusCodeId { get; set; }

        public decimal DistTotal { get; set; }
        public decimal? DeliverynotReceived { get; set; }
        public List<string> ItemList { get; set; }
        public List<GenericCodeReport> GenericCodeReport { get; set; }
        public string SalesCategory { get; set; }
        public long? LocationID { get; set; }
        public long? GenericCodeID { get; set; }

        public string Ticket1 { get; set; }
        public string Ticket2 { get; set; }
        public string Ticket3 { get; set; }
        public string Ticket4 { get; set; }
        public string Ticket5 { get; set; }
        public string Ticket6 { get; set; }
        public string Ticket7 { get; set; }
        public string Ticket8 { get; set; }
        public string Ticket9 { get; set; }
        public string Ticket10 { get; set; }
        public string Ticket11 { get; set; }
        public string Ticket12 { get; set; }

        public string OutputTicket1 { get; set; }
        public string OutputTicket2 { get; set; }
        public string OutputTicket3 { get; set; }
        public string OutputTicket4 { get; set; }
        public string OutputTicket5 { get; set; }
        public string OutputTicket6 { get; set; }
        public string OutputTicket7 { get; set; }
        public string OutputTicket8 { get; set; }
        public string OutputTicket9 { get; set; }
        public string OutputTicket10 { get; set; }
        public string OutputTicket11 { get; set; }
        public string OutputTicket12 { get; set; }

        public decimal ProductionTicket1 { get; set; }
        public decimal ProductionTicket2 { get; set; }
        public decimal ProductionTicket3 { get; set; }
        public decimal ProductionTicket4 { get; set; }
        public decimal ProductionTicket5 { get; set; }
        public decimal ProductionTicket6 { get; set; }
        public decimal ProductionTicket7 { get; set; }
        public decimal ProductionTicket8 { get; set; }
        public decimal ProductionTicket9 { get; set; }
        public decimal ProductionTicket10 { get; set; }
        public decimal ProductionTicket11 { get; set; }
        public decimal ProductionTicket12 { get; set; }

        public decimal TicketHoldingStock1 { get; set; }
        public decimal TicketHoldingStock2 { get; set; }
        public decimal TicketHoldingStock3 { get; set; }
        public decimal TicketHoldingStock4 { get; set; }
        public decimal TicketHoldingStock5 { get; set; }
        public decimal TicketHoldingStock6 { get; set; }
        public decimal TicketHoldingStock7 { get; set; }
        public decimal TicketHoldingStock8 { get; set; }
        public decimal TicketHoldingStock9 { get; set; }
        public decimal TicketHoldingStock10 { get; set; }
        public decimal TicketHoldingStock11 { get; set; }
        public decimal TicketHoldingStock12 { get; set; }

        public decimal ProjectedHoldingStock1 { get; set; }
        public decimal ProjectedHoldingStock2 { get; set; }
        public decimal ProjectedHoldingStock3 { get; set; }
        public decimal ProjectedHoldingStock4 { get; set; }
        public decimal ProjectedHoldingStock5 { get; set; }
        public decimal ProjectedHoldingStock6 { get; set; }
        public decimal ProjectedHoldingStock7 { get; set; }
        public decimal ProjectedHoldingStock8 { get; set; }
        public decimal ProjectedHoldingStock9 { get; set; }
        public decimal ProjectedHoldingStock10 { get; set; }
        public decimal ProjectedHoldingStock11 { get; set; }
        public decimal ProjectedHoldingStock12 { get; set; }

        public decimal ProjectedHoldingStockQty1 { get; set; }
        public decimal ProjectedHoldingStockQty2 { get; set; }
        public decimal ProjectedHoldingStockQty3 { get; set; }
        public decimal ProjectedHoldingStockQty4 { get; set; }
        public decimal ProjectedHoldingStockQty5 { get; set; }
        public decimal ProjectedHoldingStockQty6 { get; set; }
        public decimal ProjectedHoldingStockQty7 { get; set; }
        public decimal ProjectedHoldingStockQty8 { get; set; }
        public decimal ProjectedHoldingStockQty9 { get; set; }
        public decimal ProjectedHoldingStockQty10 { get; set; }
        public decimal ProjectedHoldingStockQty11 { get; set; }
        public decimal ProjectedHoldingStockQty12 { get; set; }

        public decimal OutputProjectedHoldingStockQty1 { get; set; }
        public decimal OutputProjectedHoldingStockQty2 { get; set; }
        public decimal OutputProjectedHoldingStockQty3 { get; set; }
        public decimal OutputProjectedHoldingStockQty4 { get; set; }
        public decimal OutputProjectedHoldingStockQty5 { get; set; }
        public decimal OutputProjectedHoldingStockQty6 { get; set; }
        public decimal OutputProjectedHoldingStockQty7 { get; set; }
        public decimal OutputProjectedHoldingStockQty8 { get; set; }
        public decimal OutputProjectedHoldingStockQty9 { get; set; }
        public decimal OutputProjectedHoldingStockQty10 { get; set; }
        public decimal OutputProjectedHoldingStockQty11 { get; set; }
        public decimal OutputProjectedHoldingStockQty12 { get; set; }

        public decimal OutputProjectedHoldingStock1 { get; set; }
        public decimal OutputProjectedHoldingStock2 { get; set; }
        public decimal OutputProjectedHoldingStock3 { get; set; }
        public decimal OutputProjectedHoldingStock4 { get; set; }
        public decimal OutputProjectedHoldingStock5 { get; set; }
        public decimal OutputProjectedHoldingStock6 { get; set; }
        public decimal OutputProjectedHoldingStock7 { get; set; }
        public decimal OutputProjectedHoldingStock8 { get; set; }
        public decimal OutputProjectedHoldingStock9 { get; set; }
        public decimal OutputProjectedHoldingStock10 { get; set; }
        public decimal OutputProjectedHoldingStock11 { get; set; }
        public decimal OutputProjectedHoldingStock12 { get; set; }

        public decimal ProductionProjected1 { get; set; }
        public decimal ProductionProjected2 { get; set; }
        public decimal ProductionProjected3 { get; set; }
        public decimal ProductionProjected4 { get; set; }
        public decimal ProductionProjected5 { get; set; }
        public decimal ProductionProjected6 { get; set; }
        public decimal ProductionProjected7 { get; set; }
        public decimal ProductionProjected8 { get; set; }
        public decimal ProductionProjected9 { get; set; }
        public decimal ProductionProjected10 { get; set; }
        public decimal ProductionProjected11 { get; set; }
        public decimal ProductionProjected12 { get; set; }

        public decimal QtyProductionProjected1 { get; set; }
        public decimal QtyProductionProjected2 { get; set; }
        public decimal QtyProductionProjected3 { get; set; }
        public decimal QtyProductionProjected4 { get; set; }
        public decimal QtyProductionProjected5 { get; set; }
        public decimal QtyProductionProjected6 { get; set; }
        public decimal QtyProductionProjected7 { get; set; }
        public decimal QtyProductionProjected8 { get; set; }
        public decimal QtyProductionProjected9 { get; set; }
        public decimal QtyProductionProjected10 { get; set; }
        public decimal QtyProductionProjected11 { get; set; }
        public decimal QtyProductionProjected12 { get; set; }

        public decimal BlanketAddhoc1 { get; set; }
        public decimal BlanketAddhoc2 { get; set; }
        public decimal BlanketAddhoc3 { get; set; }
        public decimal BlanketAddhoc4 { get; set; }
        public decimal BlanketAddhoc5 { get; set; }
        public decimal BlanketAddhoc6 { get; set; }
        public decimal BlanketAddhoc7 { get; set; }
        public decimal BlanketAddhoc8 { get; set; }
        public decimal BlanketAddhoc9 { get; set; }
        public decimal BlanketAddhoc10 { get; set; }
        public decimal BlanketAddhoc11 { get; set; }
        public decimal BlanketAddhoc12 { get; set; }

        public bool isTenderExist { get; set; }
        public decimal TenderSum { get; set; }

        public bool IsTenderExist1 { get; set; }
        public bool IsTenderExist2 { get; set; }
        public bool IsTenderExist3 { get; set; }
        public bool IsTenderExist4 { get; set; }
        public bool IsTenderExist5 { get; set; }
        public bool IsTenderExist6 { get; set; }
        public bool IsTenderExist7 { get; set; }
        public bool IsTenderExist8 { get; set; }
        public bool IsTenderExist9 { get; set; }
        public bool IsTenderExist10 { get; set; }
        public bool IsTenderExist11 { get; set; }
        public bool IsTenderExist12 { get; set; }


        public bool IsAcExist { get; set; }

        public decimal? Qty { get; set; }
        public string Comment { get; set; }
        public int? Week { get; set; }
        public bool? IsApproval { get; set; }

        public decimal? ExistingTicket { get; set; }
        public int TicketMonth { get; set; }
        public string TicketMonthName { get; set; }
        public decimal? FullTicket { get; set; }
        public decimal? SplitTicket { get; set; }
        public decimal? QtyTicket { get; set; }
        public decimal? Amount { get; set; }
        public decimal? ManagerAmount { get; set; }
        public decimal? NewAcMonth { get; set; }

        public decimal? ProdQty1 { get; set; }
        public decimal? ProdQty2 { get; set; }
        public decimal? ProdQty3 { get; set; }
        public decimal? ProdQty4 { get; set; }
        public decimal? ProdQty5 { get; set; }
        public decimal? ProdQty6 { get; set; }
        public decimal? ProdQty7 { get; set; }
        public decimal? ProdQty8 { get; set; }
        public decimal? ProdQty9 { get; set; }
        public decimal? ProdQty10 { get; set; }
        public decimal? ProdQty11 { get; set; }
        public decimal? ProdQty12 { get; set; }

        public bool IsTicketCalculated { get; set; }

        public decimal? ProdFrequency { get; set; }
        public decimal? DistReplenishHs { get; set; }
        public decimal? DistAcmonth { get; set; }
        public decimal? AdhocReplenishHs { get; set; }
        public int? AdhocMonthStandAlone { get; set; }
        public decimal? AdhocPlanQty { get; set; }

        public long? DosageFormId { get; set; }
        public long? DrugClassificationId { get; set; }

        public decimal ProductionRefresh1 { get; set; }
        public decimal ProductionRefresh2 { get; set; }
        public decimal ProductionRefresh3 { get; set; }
        public decimal ProductionRefresh4 { get; set; }
        public decimal ProductionRefresh5 { get; set; }
        public decimal ProductionRefresh6 { get; set; }
        public decimal ProductionRefresh7 { get; set; }

        public decimal ahQty { get; set; }
        public decimal ahMonth { get; set; }
        public decimal pdtQty { get; set; }
        public decimal pdtMonth { get; set; }
        public decimal noOfMonth { get; set; }
        public decimal adjnoOfTicket { get; set; }
        public decimal addedQty { get; set; }
        public decimal addedMonth { get; set; }

        public decimal groupticketMonth { get; set; }
        public decimal groupticketQty { get; set; }

        public decimal? groupticketMonth1 { get; set; }
        public decimal? groupticketQty1 { get; set; }
        public decimal? groupticketMonth2 { get; set; }
        public decimal? groupticketQty2 { get; set; }
        public decimal? groupticketMonth3 { get; set; }
        public decimal? groupticketQty3 { get; set; }
        public decimal? groupticketMonth4 { get; set; }
        public decimal? groupticketQty4 { get; set; }
        public decimal? groupticketMonth5 { get; set; }
        public decimal? groupticketQty5 { get; set; }
        public decimal? groupticketMonth6 { get; set; }
        public decimal? groupticketQty6 { get; set; }
        public decimal? groupticketMonth7 { get; set; }
        public decimal? groupticketQty7 { get; set; }
        public decimal? groupticketMonth8 { get; set; }
        public decimal? groupticketQty8 { get; set; }
        public decimal? groupticketMonth9 { get; set; }
        public decimal? groupticketQty9 { get; set; }
        public decimal? groupticketMonth10 { get; set; }
        public decimal? groupticketQty10 { get; set; }
        public decimal? groupticketMonth11 { get; set; }
        public decimal? groupticketQty11 { get; set; }
        public decimal? groupticketMonth12 { get; set; }
        public decimal? groupticketQty12 { get; set; }

        public decimal? workingQty1 { get; set; }
        public decimal? workingQty2 { get; set; }
        public decimal? workingQty3 { get; set; }
        public decimal? workingQty4 { get; set; }
        public decimal? workingQty5 { get; set; }
        public decimal? workingQty6 { get; set; }
        public decimal? workingQty7 { get; set; }
        public decimal? workingQty8 { get; set; }
        public decimal? workingQty9 { get; set; }
        public decimal? workingQty10 { get; set; }
        public decimal? workingQty11 { get; set; }
        public decimal? workingQty12 { get; set; }


        public List<ProposedAddhocModel> ProposedAddhocOrders { get; set; }


        public decimal? AcSum_ { get; set; }
        public decimal? ThreeMonthACQty_ { get; set; }
        public decimal? Roundup1_ { get; set; }
        public decimal? Roundup2_ { get; set; }
        public decimal? PreApexStockBalance_ { get; set; }
        public decimal? PreAntahStockBalance_ { get; set; }
        public decimal? PreMsbStockBalance_ { get; set; }
        public decimal? PrePsbStockBalance_ { get; set; }
        public decimal? PreSgTenderStockBalance_ { get; set; }
        public decimal? WipQty_ { get; set; }
        public decimal? NotStartInvQty_ { get; set; }
        public decimal? PreMyStockBalance_ { get; set; }
        public decimal? PreOtherStoreQty_ { get; set; }
        public decimal? PrewipQty_ { get; set; }
        public decimal? PreStockBalance_ { get; set; }
        public decimal? PreStockHoldingBalance_ { get; set; }
        public decimal? ApexStockBalance_ { get; set; }
        public decimal? AntahStockBalance_ { get; set; }
        public decimal? MsbStockBalance_ { get; set; }
        public decimal? PsbStockBalance_ { get; set; }
        public decimal? SgTenderStockBalance_ { get; set; }
        public decimal? MyStockBalance_ { get; set; }
        public decimal? OtherStoreQty_ { get; set; }
        public decimal? InterCompanyTransitQty_ { get; set; }
        public decimal? StockBalance_ { get; set; }
        public decimal? StockHoldingBalance_ { get; set; }
        public decimal? BlanketAddhoc1_ { get; set; }
        public decimal? Month1_ { get; set; }
        public decimal? ProjectedHoldingStock1_ { get; set; }
        public decimal? ProductionProjected1_ { get; set; }
        public decimal? BlanketAddhoc2_ { get; set; }
        public decimal? Month2_ { get; set; }
        public decimal? ProjectedHoldingStock2_ { get; set; }
        public decimal? ProductionProjected2_ { get; set; }
        public decimal? BlanketAddhoc3_ { get; set; }
        public decimal? Month3_ { get; set; }
        public decimal? ProjectedHoldingStock3_ { get; set; }
        public decimal? ProductionProjected3_ { get; set; }
        public decimal? BlanketAddhoc4_ { get; set; }
        public decimal? Month4_ { get; set; }
        public decimal? ProjectedHoldingStock4_ { get; set; }
        public decimal? ProductionProjected4_ { get; set; }
        public decimal? BlanketAddhoc5_ { get; set; }
        public decimal? Month5_ { get; set; }
        public decimal? ProjectedHoldingStock5_ { get; set; }
        public decimal? ProductionProjected5_ { get; set; }
        public decimal? BlanketAddhoc6_ { get; set; }
        public decimal? Month6_ { get; set; }
        public decimal? ProjectedHoldingStock6_ { get; set; }
        public decimal? ProductionProjected6_ { get; set; }
        public decimal? BlanketAddhoc7_ { get; set; }
        public decimal? Month7_ { get; set; }
        public decimal? ProjectedHoldingStock7_ { get; set; }
        public decimal? ProductionProjected7_ { get; set; }
        public decimal? BlanketAddhoc8_ { get; set; }
        public decimal? Month8_ { get; set; }
        public decimal? ProjectedHoldingStock8_ { get; set; }
        public decimal? ProductionProjected8_ { get; set; }
        public decimal? BlanketAddhoc9_ { get; set; }
        public decimal? Month9_ { get; set; }
        public decimal? ProjectedHoldingStock9_ { get; set; }
        public decimal? ProductionProjected9_ { get; set; }
        public decimal? BlanketAddhoc10_ { get; set; }
        public decimal? Month10_ { get; set; }
        public decimal? ProjectedHoldingStock10_ { get; set; }
        public decimal? ProductionProjected10_ { get; set; }
        public decimal? BlanketAddhoc11_ { get; set; }
        public decimal? Month11_ { get; set; }
        public decimal? ProjectedHoldingStock11_ { get; set; }
        public decimal? ProductionProjected11_ { get; set; }
        public decimal? BlanketAddhoc12_ { get; set; }
        public decimal? Month12_ { get; set; }
        public decimal? ProjectedHoldingStock12_ { get; set; }
        public decimal? ProductionProjected12_ { get; set; }
        public decimal? Rework_ { get; set; }

        public string GroupTicket1 { get; set; }
        public string GroupTicket2 { get; set; }
        public string GroupTicket3 { get; set; }
        public string GroupTicket4 { get; set; }
        public string GroupTicket5 { get; set; }
        public string GroupTicket6 { get; set; }
        public string GroupTicket7 { get; set; }
        public string GroupTicket8 { get; set; }
        public string GroupTicket9 { get; set; }
        public string GroupTicket10 { get; set; }
        public string GroupTicket11 { get; set; }
        public string GroupTicket12 { get; set; }

        public string ProTicket1 { get; set; }
        public string ProTicket2 { get; set; }
        public string ProTicket3 { get; set; }
        public string ProTicket4 { get; set; }
        public string ProTicket5 { get; set; }
        public string ProTicket6 { get; set; }
        public string ProTicket7 { get; set; }
        public string ProTicket8 { get; set; }
        public string ProTicket9 { get; set; }
        public string ProTicket10 { get; set; }
        public string ProTicket11 { get; set; }
        public string ProTicket12 { get; set; }

        public string GroupItemTicket1 { get; set; }
        public string GroupItemTicket2 { get; set; }
        public string GroupItemTicket3 { get; set; }
        public string GroupItemTicket4 { get; set; }
        public string GroupItemTicket5 { get; set; }
        public string GroupItemTicket6 { get; set; }
        public string GroupItemTicket7 { get; set; }
        public string GroupItemTicket8 { get; set; }
        public string GroupItemTicket9 { get; set; }
        public string GroupItemTicket10 { get; set; }
        public string GroupItemTicket11 { get; set; }
        public string GroupItemTicket12 { get; set; }

        public string NoOfTicket1 { get; set; }
        public string NoOfTicket2 { get; set; }
        public string NoOfTicket3 { get; set; }
        public string NoOfTicket4 { get; set; }
        public string NoOfTicket5 { get; set; }
        public string NoOfTicket6 { get; set; }
        public string NoOfTicket7 { get; set; }
        public string NoOfTicket8 { get; set; }
        public string NoOfTicket9 { get; set; }
        public string NoOfTicket10 { get; set; }
        public string NoOfTicket11 { get; set; }
        public string NoOfTicket12 { get; set; }



        public string ProdOrderNo1 { get; set; }
        public string ProdOrderNo2 { get; set; }
        public string ProdOrderNo3 { get; set; }
        public string ProdOrderNo4 { get; set; }
        public string ProdOrderNo5 { get; set; }
        public string ProdOrderNo6 { get; set; }
        public string ProdOrderNo7 { get; set; }
        public string ProdOrderNo8 { get; set; }
        public string ProdOrderNo9 { get; set; }
        public string ProdOrderNo10 { get; set; }
        public string ProdOrderNo11 { get; set; }
        public string ProdOrderNo12 { get; set; }

    }
    public class NAVRecipesModel
    {
        public long ItemRecipeId { get; set; }
        public string ParentItemNo { get; set; }
        public string ItemNo { get; set; }
        public string RecipeNo { get; set; }
        public string RecipeName { get; set; }
        public string Description { get; set; }
        public string BatchSize { get; set; }
        public string Remark { get; set; }
        public decimal UnitQTY { get; set; }
        public string UOM { get; set; }
        public decimal ProductionTime { get; set; }
        public long? CompanyId { get; set; }
        public string DefaultBatch { get; set; }
        public string MachineCenterCode { get; set; }
        public List<NAVRecipesModel> Children { get; set; } = new List<NAVRecipesModel>();
    }
    public class GenericCodeReport
    {
        public string ItemNo { get; set; }
        public string Description { get; set; }
        public string Description2 { get; set; }
        public string ItemCategory { get; set; }
        public string InternalRefNo { get; set; }
        public string MethodCode { get; set; }
        public string DistItem { get; set; }
        public decimal StockBalance { get; set; }

        public string Whse { get; set; }
        public string LotNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string SumOfOnHandQty { get; set; }

        public string OpeningStockValuecompany { get; set; }
        public string OpeningStockValuesupplyBy { get; set; }
        public string OpeningStockValuedosage { get; set; }
        public string OpeningStockValueclassification { get; set; }
        public string OpeningStockValuetotalValue { get; set; }

        public string NewForcastStockValuecompany { get; set; }
        public string NewForcastStockValuesupplyBy { get; set; }
        public string NewForcastStockValuedosage { get; set; }
        public string NewForcastStockValueclassification { get; set; }
        public string NewForcastStockValuetotalValue { get; set; }
    }
    public class ProposedAddhocModel
    {
        public string VersionNo { get; set; }
        public string Itemgrouping { get; set; }
        public DateTime ReportDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public long CompanyId { get; set; }
        public long ItemId { get; set; }
        public string ItemNo { get; set; }
        public string GrouoItemNo { get; set; }
        public long PackSize { get; set; }
        public decimal WipQty { get; set; }
        public decimal TotalStock { get; set; }
        public decimal fbStock { get; set; }
        public decimal addhocStock { get; set; }
        public string FromMonth { get; set; }
        public string ToMonth { get; set; }
        public decimal TotalAddhocStock { get; set; }
        public decimal TotalPropStock { get; set; }
        public decimal TotalPropFbStock { get; set; }
        public decimal TotalPropAddhocStock { get; set; }

        public string allocateStock { get; set; }
        public decimal adjustStock { get; set; }
    }
    public class DateRangeModel
    {
        public int intmonth { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string ItemNo { get; set; }
        public long? MethodCodeId { get; set; }
        public long? SalesCategoryId { get; set; }
        public string Replenishment { get; set; }
        public bool? IsSteroid { get; set; }
        public DateTime StockMonth { get; set; }
        public long? ChangeMethodCodeId { get; set; }
        public string Receipe { get; set; }
        public string Remarks { get; set; }
        public decimal Roundup2 { get; set; }
        public bool IsUpdate { get; set; }
        public int? Ticketformula { get; set; }
        public int? Ticketvalue { get; set; }

        public long? CompanyId { get; set; }
        public string Ref { get; set; }
        public long? VersionId { get; set; }
        public long? CustomerId { get; set; }
        public DateTime? SelectedMonth { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<long?> NavItemIds { get; set; }

        public bool ShowSmallestUnit { get; set; }

        public bool ShowACOnly { get; set; }

        public long? DosageFormId { get; set; }
        public long? DrugClassificationId { get; set; }

        public bool isAc { get; set; }

        public string VersionNo { get; set; }
        public bool IsSteroids { get; set; } = false;
        public bool IsNonSteroids { get; set; } = false;
    }

    public class NavmethodCodeBatch
    {

        public long MethodCodeBatchId { get; set; }
        public long NavMethodCodeId { get; set; }
        public long? BatchSize { get; set; }
        public long? DefaultBatchSize { get; set; }
        public decimal? BatchUnitSize { get; set; }

    }
    public class DistStockBalModel
    {
        public string DistName { get; set; }
        public long CustomerId { get; set; }
        public decimal QtyOnHand { get; set; }
        public decimal POQty { get; set; }
        public decimal Avg6MontQty { get; set; }
        public string ItemDesc { get; set; }
        public string ItemNo { get; set; }
        public long DistAcid { get; set; }
        public long? CompanyId { get; set; }
        public long? DistItemId { get; set; }

        public long? NavItemId { get; set; }
        public DateTime? StockBalanceMonth { get; set; }
    }
    public class ACItemsModel : BaseModel
    {
        public long DistACID { get; set; }
        public string DistName { get; set; }
        public long DistId { get; set; }
        public string ItemGroup { get; set; }
        public string ItemNo { get; set; }
        public long? SWItemId { get; set; }
        public List<long?> ItemIds { get; set; }
        public string SWItemNo { get; set; }
        public string SWItemDesc { get; set; }
        public string SWItemDesc2 { get; set; }
        public string GenericCode { get; set; }
        public string Packuom { get; set; }
        public string Steriod { get; set; }
        public string ShelfLife { get; set; }
        public string Quota { get; set; }
        public string Status { get; set; }
        public string ItemDesc { get; set; }
        public string PackSize { get; set; }
        public decimal ACQty { get; set; }
        public DateTime ACMonth { get; set; }
        public long? CustomerId { get; set; }
        public long? CompanyId { get; set; }
        public string InternalRefNo { get; set; }

        public string CategoryCode { get; set; }
        public long? NavItemCustomerItemId { get; set; }
        public long? NavItemId { get; set; }
    }
    public class OrderRequirementLineModel : BaseModel
    {
        public long OrderRequirementLineId { get; set; }
        public long? OrderRequirementId { get; set; }
        public long? ProductId { get; set; }
        public string TicketBatchSizeId { get; set; }
        public decimal? NoOfTicket { get; set; }
        public string Remarks { get; set; }
        public DateTime? ExpectedStartDate { get; set; }
        public bool? RequireToSplit { get; set; }
        public string TicketBatchSizeName { get; set; }
        public string ProductName { get; set; }
        public string RequireToSplitFlag { get; set; }
        public decimal? ProductQty { get; set; }
        public string UOM { get; set; }
        public long? NavLocationId { get; set; }
        public long? NavUomid { get; set; }
        public string NavLocationName { get; set; }
        public string NavUomName { get; set; }
        public bool IsNavSync { get; set; }
        public long? SplitProductId { get; set; }
        public decimal? SplitProductQty { get; set; }
    }
    public class NavpostedShipment
    {
        public long ShipmentId { get; set; }
        public string Company { get; set; }
        public long? CompanyId { get; set; }
        public DateTime? StockBalanceMonth { get; set; }
        public DateTime? PostingDate { get; set; }
        public string Customer { get; set; }
        public string CustomerNo { get; set; }
        public long? CustomerId { get; set; }
        public string DeliveryOrderNo { get; set; }
        public int? DolineNo { get; set; }
        public string ItemNo { get; set; }
        public string Description { get; set; }
        public bool? IsRecived { get; set; }
        public decimal? DoQty { get; set; }
        public int StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }

    }
    public class NavItemCitemList
    {
        public long NavItemCitemId { get; set; }
        public long? NavItemId { get; set; }
        public long? NavItemCustomerItemId { get; set; }
        public long DistAcid { get; set; }
        public long? CompanyId { get; set; }
        public long? CustomerId { get; set; }
        public string DistName { get; set; }
        public string ItemGroup { get; set; }
        public string Steriod { get; set; }
        public string ShelfLife { get; set; }
        public string Quota { get; set; }
        public string Status { get; set; }
        public string ItemDesc { get; set; }
        public string PackSize { get; set; }
        public decimal? Acqty { get; set; }
        public DateTime? Acmonth { get; set; }
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ItemNo { get; set; }
    }
    public class NavSaleCategory
    {
        public long SaleCategoryId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string NoSeries { get; set; }
        public long? LocationId { get; set; }
        public string SgnoSeries { get; set; }
        public int StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
    public class NavitemLinks
    {
        public long ItemLinkId { get; set; }
        public long? MyItemId { get; set; }
        public long? SgItemId { get; set; }
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }

    }
    public class SimulationAddhoc
    {
        public long SimualtionAddhocId { get; set; }
        public string DocumantType { get; set; }
        public string SelltoCustomerNo { get; set; }
        public string CustomerName { get; set; }
        public string Categories { get; set; }
        public string DocumentNo { get; set; }
        public string ExternalDocNo { get; set; }
        public long? ItemId { get; set; }
        public string ItemNo { get; set; }
        public string Description { get; set; }
        public string Description1 { get; set; }
        public decimal? OutstandingQty { get; set; }
        public DateTime? PromisedDate { get; set; }
        public DateTime? ShipmentDate { get; set; }
        public string Uomcode { get; set; }
    }
    public class GroupPlaningTicket
    {
        public long GroupPlanningId { get; set; }
        public int? CompanyId { get; set; }
        public string BatchName { get; set; }
        public string ProductGroupCode { get; set; }
        public DateTime? StartDate { get; set; }
        public string ItemNo { get; set; }
        public string Description { get; set; }
        public string Description1 { get; set; }
        public string BatchSize { get; set; }
        public decimal? Quantity { get; set; }
        public string Uom { get; set; }
        public int? NoOfTicket { get; set; }
        public decimal? TotalQuantity { get; set; }
    }
    public class Acitems
    {
        public long DistAcid { get; set; }
        public long? CompanyId { get; set; }
        public long? CustomerId { get; set; }
        public string DistName { get; set; }
        public string ItemGroup { get; set; }
        public string Steriod { get; set; }
        public string ShelfLife { get; set; }
        public string Quota { get; set; }
        public string Status { get; set; }
        public string ItemDesc { get; set; }
        public string PackSize { get; set; }
        public decimal? Acqty { get; set; }
        public DateTime? Acmonth { get; set; }
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ItemNo { get; set; }
        public long? NavItemCustomerItemId { get; set; }
        public long? NavItemId { get; set; }
    }
    public class NavMethodCodeModel : BaseModel
    {
        public long MethodCodeID { get; set; }
        public string MethodName { get; set; }
        public string MethodDescription { get; set; }
        public List<long?> ItemIds { get; set; }
        public List<long?> BatchSizeIds { get; set; } = new List<long?>();
        public long? BatchSizeId { get; set; }
        public decimal? BatchSizeNo { get; set; }
        public long? NavinpCategoryID { get; set; }
        public long? CompanyId { get; set; }
        public string ItemNos { get; set; }
        public decimal? ProdFrequency { get; set; }
        public decimal? DistReplenishHs { get; set; }
        public decimal? DistAcmonth { get; set; }
        public decimal? AdhocReplenishHs { get; set; }
        public int? AdhocMonthStandAlone { get; set; }
        public decimal? AdhocPlanQty { get; set; }
        public string DropDownName { get; set; }
    }
    public class NavMethodCodeLines
    {
        public long MethodCodeLineId { get; set; }
        public long? MethodCodeId { get; set; }
        public long? ItemId { get; set; }
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string MethodName { get; set; }
        public string MethodDescription { get; set; }
        public long? NavinpcategoryId { get; set; }
        public long? CompanyId { get; set; }
        public decimal? ProdFrequency { get; set; }
        public decimal? DistReplenishHs { get; set; }
        public decimal? DistAcmonth { get; set; }
        public decimal? AdhocReplenishHs { get; set; }
        public int? AdhocMonthStandAlone { get; set; }
        public decimal? AdhocPlanQty { get; set; }
    }
    public class StockBalanceSearch
    {
        public DateTime StkMonth { get; set; }
        public string? ItemNo { get; set; }
        public long? CompanyId { get; set; }
        public long? UserId { get; set; }
    }
    public class SotckBalanceItemsList
    {
        public List<Plant> PlantData { get; set; } = new List<Plant>();
        public List<Navitems> NavitemsData { get; set; } = new List<Navitems>();
        public List<NavitemStockBalance> NavitemStockBalance { get; set; } = new List<NavitemStockBalance>();
        public List<Navcustomer> Navcustomer { get; set; } = new List<Navcustomer>();
        public List<DistStockBalanceKiv> DistStockBalanceKiv { get; set; } = new List<DistStockBalanceKiv>();
    }
}
