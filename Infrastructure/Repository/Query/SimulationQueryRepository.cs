using Core.Entities;
using Core.Repositories.Query;
using Infrastructure.Repository.Query.Base;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.EntityModels;
using System.Text.RegularExpressions;

namespace Infrastructure.Repository.Query
{
    public class SimulationQueryRepository : QueryRepository<INPCalendarPivotModel>, ISimulationQueryRepository
    {
        public SimulationQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<IReadOnlyList<NavMethodCodeModel>> GetAllNavMethodCodeAsync()
        {
            try
            {
                var query = "SELECT * FROM NavMethodCode;";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<NavMethodCodeModel>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<INPCalendarPivotModel>> GetSimulationMidMonth(DateRangeModel dateRangeModel)
        {
            var acreports = new List<INPCalendarPivotModel>();
            try
            {
                var companyIds = new List<long?> { dateRangeModel.CompanyId };
                if (dateRangeModel.CompanyId == 3)
                {
                    companyIds = new List<long?> { 1, 2 };
                }
                List<NavmethodCodeBatch> methodCodeRecipe = new List<NavmethodCodeBatch>();
                List<ApplicationMasterDetail> applicationDetails = new List<ApplicationMasterDetail>();
                List<NAVRecipesModel> recipeList = new List<NAVRecipesModel>();
                var methodCodeRecipes = new List<NAVRecipesModel>();
                using (var connection = CreateConnection())
                {
                    var query = "SELECT * FROM NavmethodCodeBatch;";
                    query += "select t1.* from ApplicationMasterDetail t1 JOIN ApplicationMaster t2 ON t1.ApplicationMasterID=t2.ApplicationMasterID where t2.ApplicationMasterCodeID=175;";
                    query += "select RecipeNo,ItemNo,Description,BatchSize,ItemRecipeId,CONCAT(RecipeNo,'|',BatchSize) as RecipeName from Navrecipes Where CompanyId  in(" + string.Join(',', companyIds) + ") AND Status='Certified';";
                    var results = await connection.QueryMultipleAsync(query);
                    methodCodeRecipe = results.Read<NavmethodCodeBatch>().ToList();
                    applicationDetails = results.Read<ApplicationMasterDetail>().ToList();
                    recipeList = results.Read<NAVRecipesModel>().ToList();
                }
                methodCodeRecipe.ForEach(f =>
                {
                    var BatchSize = applicationDetails.FirstOrDefault(b => b.ApplicationMasterDetailId == f.BatchSize)?.Value;
                    var DefaultBatchSize = applicationDetails.FirstOrDefault(b => b.ApplicationMasterDetailId == f.DefaultBatchSize)?.Value;
                    methodCodeRecipes.Add(new NAVRecipesModel
                    {
                        RecipeNo = BatchSize,
                        BatchSize = BatchSize,
                        ItemRecipeId = f.NavMethodCodeId,
                        UnitQTY = f.BatchUnitSize.GetValueOrDefault(0),
                        ItemNo = DefaultBatchSize,
                        DefaultBatch = DefaultBatchSize,
                        RecipeName = BatchSize,
                    }); ;

                });

                if (acreports.Count == 0)
                {
                    acreports = await SimulationMidMonth(dateRangeModel, methodCodeRecipes, recipeList);
                }
                if (dateRangeModel.MethodCodeId > 0)
                    acreports = acreports.Where(r => r.MethodCodeId == dateRangeModel.MethodCodeId).ToList();
                if (dateRangeModel.SalesCategoryId > 0)
                {
                    acreports = acreports.Where(r => r.SalesCategoryId == dateRangeModel.SalesCategoryId).ToList();
                }
                if (dateRangeModel.IsSteroid.HasValue)
                {
                    acreports = acreports.Where(r => r.IsSteroid == dateRangeModel.IsSteroid.Value).ToList();
                }
                if (!string.IsNullOrEmpty(dateRangeModel.Replenishment))
                {
                    acreports = acreports.Where(r => r.Replenishment.Contains(dateRangeModel.Replenishment)).ToList();
                }

                if (dateRangeModel.Ticketformula.GetValueOrDefault(0) > 0)
                {
                    var ticketCondition = dateRangeModel.Ticketformula.GetValueOrDefault(0);
                    var ticketValue = dateRangeModel.Ticketvalue.GetValueOrDefault(0);
                    if (ticketCondition == 1)
                    {
                        acreports = acreports.Where(r => r.Month1 == ticketValue || r.Month1 == ticketValue || r.Month2 == ticketValue || r.Month3 == ticketValue || r.Month4 == ticketValue || r.Month5 == ticketValue || r.Month6 == ticketValue).ToList();
                    }
                    else if (ticketCondition == 2)
                    {
                        acreports = acreports.Where(r => r.Month1 > ticketValue || r.Month1 > ticketValue || r.Month2 > ticketValue || r.Month3 > ticketValue || r.Month4 > ticketValue || r.Month5 > ticketValue || r.Month6 > ticketValue).ToList();
                    }
                    else if (ticketCondition == 3)
                    {
                        acreports = acreports.Where(r => r.Month1 >= ticketValue || r.Month1 >= ticketValue || r.Month2 >= ticketValue || r.Month3 >= ticketValue || r.Month4 >= ticketValue || r.Month5 >= ticketValue || r.Month6 >= ticketValue).ToList();
                    }
                    else if (ticketCondition == 4)
                    {
                        acreports = acreports.Where(r => r.Month1 < ticketValue || r.Month1 < ticketValue || r.Month2 < ticketValue || r.Month3 < ticketValue || r.Month4 < ticketValue || r.Month5 < ticketValue || r.Month6 < ticketValue).ToList();
                    }
                    else if (ticketCondition == 5)
                    {
                        acreports = acreports.Where(r => r.Month1 <= ticketValue || r.Month1 <= ticketValue || r.Month2 <= ticketValue || r.Month3 <= ticketValue || r.Month4 <= ticketValue || r.Month5 <= ticketValue || r.Month6 <= ticketValue).ToList();
                    }
                    else
                    {
                        acreports = acreports.Where(r => r.Month1 != ticketValue || r.Month1 != ticketValue || r.Month2 != ticketValue || r.Month3 != ticketValue || r.Month4 != ticketValue || r.Month5 != ticketValue || r.Month6 != ticketValue).ToList();
                    }
                }

                var packSize2 = 90000;
                if (!string.IsNullOrEmpty(dateRangeModel.Receipe))
                {
                    string numberOnly = Regex.Replace(dateRangeModel.Receipe.Split("|")[0], "[^0-9.]", "");
                    packSize2 = int.Parse(numberOnly) * 1000;

                    acreports.Where(m => m.MethodCodeId == dateRangeModel.ChangeMethodCodeId).ToList().ForEach(f =>
                    {
                        f.PackSize2 = packSize2;
                        f.ProdRecipe = dateRangeModel.Receipe.Split("|")[0];
                    });

                }

                if (dateRangeModel.IsUpdate)
                {
                    acreports.Where(m => m.ItemNo == dateRangeModel.ItemNo).ToList().ForEach(f =>
                    {
                        f.Roundup2 = dateRangeModel.Roundup2;
                        f.Remarks = dateRangeModel.Remarks;
                    });
                }
                if (acreports != null && acreports.Count() > 0)
                {
                    int i = 0;
                    acreports.ForEach(s =>
                    {
                        s.Index = i++;
                        s.ApexQty = s.ApexQty == 0 ? null : s.ApexQty;
                        s.AntahQty = s.AntahQty == 0 ? null : s.AntahQty;
                        s.MissQty = s.MissQty == 0 ? null : s.MissQty;
                        s.PxQty = s.PxQty == 0 ? null : s.PxQty;
                        s.DeliverynotReceived = s.DeliverynotReceived == 0 ? null : s.DeliverynotReceived;
                        s.SymlQty = s.SymlQty == 0 ? null : s.SymlQty;
                        s.Rework_ = s.Rework == 0 ? null : s.Rework;
                        s.AcSum_ = s.AcSum == 0 ? null : s.AcSum;
                        s.ThreeMonthACQty_ = s.ThreeMonthACQty == 0 ? null : s.ThreeMonthACQty;
                        s.Roundup1_ = s.Roundup1 == 0 ? null : s.Roundup1;
                        s.Roundup2_ = s.Roundup2 == 0 ? null : s.Roundup2;
                        s.PreApexStockBalance_ = s.PreApexStockBalance == 0 ? null : s.PreApexStockBalance;
                        s.PreAntahStockBalance_ = s.PreAntahStockBalance == 0 ? null : s.PreAntahStockBalance;
                        s.PreMsbStockBalance_ = s.PreMsbStockBalance == 0 ? null : s.PreMsbStockBalance;
                        s.PrePsbStockBalance_ = s.PrePsbStockBalance == 0 ? null : s.PrePsbStockBalance;
                        s.PreSgTenderStockBalance_ = s.PreSgTenderStockBalance == 0 ? null : s.PreSgTenderStockBalance;
                        s.WipQty_ = s.WipQty == 0 ? null : s.WipQty;
                        s.NotStartInvQty_ = s.NotStartInvQty == 0 ? null : s.NotStartInvQty;
                        s.PreMyStockBalance_ = s.PreMyStockBalance == 0 ? null : s.PreMyStockBalance;
                        s.PreOtherStoreQty_ = s.PreOtherStoreQty == 0 ? null : s.PreOtherStoreQty;
                        s.PrewipQty_ = s.PrewipQty == 0 ? null : s.PrewipQty;
                        s.PreStockBalance_ = s.PreStockBalance == 0 ? null : s.PreStockBalance;
                        s.PreStockHoldingBalance_ = s.PreStockHoldingBalance == 0 ? null : s.PreStockHoldingBalance;
                        s.ApexStockBalance_ = s.ApexStockBalance == 0 ? null : s.ApexStockBalance;
                        s.AntahStockBalance_ = s.AntahStockBalance == 0 ? null : s.AntahStockBalance;
                        s.MsbStockBalance_ = s.MsbStockBalance == 0 ? null : s.MsbStockBalance;
                        s.PsbStockBalance_ = s.PsbStockBalance == 0 ? null : s.PsbStockBalance;
                        s.SgTenderStockBalance_ = s.SgTenderStockBalance == 0 ? null : s.SgTenderStockBalance;
                        s.MyStockBalance_ = s.MyStockBalance == 0 ? null : s.MyStockBalance;
                        s.OtherStoreQty_ = s.OtherStoreQty == 0 ? null : s.OtherStoreQty;
                        s.InterCompanyTransitQty_ = s.InterCompanyTransitQty == 0 ? null : s.InterCompanyTransitQty;
                        s.StockBalance_ = s.StockBalance == 0 ? null : s.StockBalance;
                        s.StockHoldingBalance_ = s.StockHoldingBalance == 0 ? null : s.StockHoldingBalance;
                        s.BlanketAddhoc1_ = s.BlanketAddhoc1 == 0 ? null : s.BlanketAddhoc1;
                        s.Month1_ = s.Month1 == 0 ? null : s.Month1;
                        s.ProjectedHoldingStock1_ = s.ProjectedHoldingStock1 == 0 ? null : s.ProjectedHoldingStock1;
                        //s.ProductionProjected1_ = s.ProductionProjected1 == 0 ? null : s.ProductionProjected1;
                        s.ProductionProjected1_ = (s.Month1 + s.ProjectedHoldingStock1) - s.BlanketAddhoc1;
                        s.BlanketAddhoc2_ = s.BlanketAddhoc2 == 0 ? null : s.BlanketAddhoc2;
                        s.Month2_ = s.Month2 == 0 ? null : s.Month2;
                        s.ProjectedHoldingStock2_ = s.ProjectedHoldingStock2 == 0 ? null : s.ProjectedHoldingStock2;
                        s.ProductionProjected2_ = (s.Month2 + s.ProjectedHoldingStock2) - s.BlanketAddhoc2;
                        //s.ProductionProjected2_ = s.ProductionProjected2 == 0 ? null : s.ProductionProjected2;
                        s.BlanketAddhoc3_ = s.BlanketAddhoc3 == 0 ? null : s.BlanketAddhoc3;
                        s.Month3_ = s.Month3 == 0 ? null : s.Month3;
                        s.ProjectedHoldingStock3_ = s.ProjectedHoldingStock3 == 0 ? null : s.ProjectedHoldingStock3;
                        s.ProductionProjected3_ = (s.Month3 + s.ProjectedHoldingStock3) - s.BlanketAddhoc3;
                        //s.ProductionProjected3_ = s.ProductionProjected3 == 0 ? null : s.ProductionProjected3;
                        s.BlanketAddhoc4_ = s.BlanketAddhoc4 == 0 ? null : s.BlanketAddhoc4;
                        s.Month4_ = s.Month4 == 0 ? null : s.Month4;
                        s.ProjectedHoldingStock4_ = s.ProjectedHoldingStock4 == 0 ? null : s.ProjectedHoldingStock4;
                        s.ProductionProjected4_ = (s.Month4 + s.ProjectedHoldingStock4) - s.BlanketAddhoc4;
                        //s.ProductionProjected4_ = s.ProductionProjected4 == 0 ? null : s.ProductionProjected4;
                        s.BlanketAddhoc5_ = s.BlanketAddhoc5 == 0 ? null : s.BlanketAddhoc5;
                        s.Month5_ = s.Month5 == 0 ? null : s.Month5;
                        s.ProjectedHoldingStock5_ = s.ProjectedHoldingStock5 == 0 ? null : s.ProjectedHoldingStock5;
                        s.ProductionProjected5_ = (s.Month5 + s.ProjectedHoldingStock5) - s.BlanketAddhoc5;
                        //s.ProductionProjected5_ = s.ProductionProjected5 == 0 ? null : s.ProductionProjected5;
                        s.BlanketAddhoc6_ = s.BlanketAddhoc6 == 0 ? null : s.BlanketAddhoc6;
                        s.Month6_ = s.Month6 == 0 ? null : s.Month6;
                        s.ProjectedHoldingStock6_ = s.ProjectedHoldingStock6 == 0 ? null : s.ProjectedHoldingStock6;
                        s.ProductionProjected6_ = (s.Month6 + s.ProjectedHoldingStock6) - s.BlanketAddhoc6;
                        //s.ProductionProjected6_ = s.ProductionProjected6 == 0 ? null : s.ProductionProjected6;
                        s.BlanketAddhoc7_ = s.BlanketAddhoc7 == 0 ? null : s.BlanketAddhoc7;
                        s.Month7_ = s.Month7 == 0 ? null : s.Month7;
                        s.ProjectedHoldingStock7_ = s.ProjectedHoldingStock7 == 0 ? null : s.ProjectedHoldingStock7;
                        //s.ProductionProjected7_ = s.ProductionProjected7 == 0 ? null : s.ProductionProjected7;
                        s.ProductionProjected7_ = (s.Month7 + s.ProjectedHoldingStock7) - s.BlanketAddhoc7;
                        s.BlanketAddhoc8_ = s.BlanketAddhoc8 == 0 ? null : s.BlanketAddhoc8;
                        s.Month8_ = s.Month8 == 0 ? null : s.Month8;
                        s.ProjectedHoldingStock8_ = s.ProjectedHoldingStock8 == 0 ? null : s.ProjectedHoldingStock8;
                        //s.ProductionProjected8_ = s.ProductionProjected8 == 0 ? null : s.ProductionProjected8;
                        s.ProductionProjected8_ = (s.Month8 + s.ProjectedHoldingStock8) - s.BlanketAddhoc8;
                        s.BlanketAddhoc9_ = s.BlanketAddhoc9 == 0 ? null : s.BlanketAddhoc9;
                        s.Month9_ = s.Month9 == 0 ? null : s.Month9;
                        s.ProjectedHoldingStock9_ = s.ProjectedHoldingStock9 == 0 ? null : s.ProjectedHoldingStock9;
                        //s.ProductionProjected9_ = s.ProductionProjected9 == 0 ? null : s.ProductionProjected9;
                        s.ProductionProjected9_ = (s.Month9 + s.ProjectedHoldingStock9) - s.BlanketAddhoc9;
                        s.BlanketAddhoc10_ = s.BlanketAddhoc10 == 0 ? null : s.BlanketAddhoc10;
                        s.Month10_ = s.Month10 == 0 ? null : s.Month10;
                        s.ProjectedHoldingStock10_ = s.ProjectedHoldingStock10 == 0 ? null : s.ProjectedHoldingStock10;
                        //s.ProductionProjected10_ = s.ProductionProjected10 == 0 ? null : s.ProductionProjected10;
                        s.ProductionProjected10_ = (s.Month10 + s.ProjectedHoldingStock10) - s.BlanketAddhoc10;
                        s.BlanketAddhoc11_ = s.BlanketAddhoc11 == 0 ? null : s.BlanketAddhoc11;
                        s.Month11_ = s.Month11 == 0 ? null : s.Month11;
                        s.ProjectedHoldingStock11_ = s.ProjectedHoldingStock11 == 0 ? null : s.ProjectedHoldingStock11;
                        //s.ProductionProjected11_ = s.ProductionProjected11 == 0 ? null : s.ProductionProjected11;
                        s.ProductionProjected11_ = (s.Month11 + s.ProjectedHoldingStock11) - s.BlanketAddhoc11;
                        s.BlanketAddhoc12_ = s.BlanketAddhoc12 == 0 ? null : s.BlanketAddhoc12;
                        s.Month12_ = s.Month12 == 0 ? null : s.Month12;
                        s.ProjectedHoldingStock12_ = s.ProjectedHoldingStock12 == 0 ? null : s.ProjectedHoldingStock12;
                        //s.ProductionProjected12_ = s.ProductionProjected12 == 0 ? null : s.ProductionProjected12;
                        s.ProductionProjected12_ = (s.Month12 + s.ProjectedHoldingStock12) - s.BlanketAddhoc12;
                        var GroupTicket1 = s.GroupItemTicket1 + "," + s.GroupTicket1;
                        if (GroupTicket1 != null)
                        {
                            var tic1 = GroupTicket1.Split(",").ToList().Where(w => w != "," && w != "").ToList();
                            var tick1_ = tic1.Distinct().ToList(); s.Ticket1 = "";
                            if (tick1_ != null && tick1_.Count > 0)
                            {
                                tick1_.ForEach(a =>
                                {
                                    if (!string.IsNullOrEmpty(a))
                                    {
                                        s.Ticket1 += tic1.Where(q => q == a).Count() + "*" + a.Split(".")[0] + "/";
                                        if (!string.IsNullOrEmpty(s.NoOfTicket1))
                                        {
                                            s.Ticket1 += s.NoOfTicket1 + "/";
                                        }
                                    }
                                });
                            }
                            s.Ticket1 += s.ProdOrderNo1;
                        }
                        var GroupTicket2 = s.GroupItemTicket2 + "," + s.GroupTicket2;
                        if (GroupTicket2 != null)
                        {
                            var tic2 = GroupTicket2.Split(",").ToList().Where(w => w != "," && w != "").ToList();
                            var tick2_ = tic2.Distinct().ToList(); s.Ticket2 = "";
                            if (tick2_ != null && tick2_.Count > 0)
                            {
                                tick2_.ForEach(a =>
                                {
                                    if (!string.IsNullOrEmpty(a))
                                    {
                                        s.Ticket2 += tic2.Where(q => q == a).Count() + "*" + a.Split(".")[0] + "/";
                                        if (!string.IsNullOrEmpty(s.NoOfTicket2))
                                        {
                                            s.Ticket2 += s.NoOfTicket2 + "/";
                                        }
                                    }
                                });
                            }
                            s.Ticket2 += s.ProdOrderNo2;
                        }
                        var GroupTicket3 = s.GroupItemTicket3 + "," + s.GroupTicket3;
                        if (GroupTicket3 != null)
                        {
                            var tic3 = GroupTicket3.Split(",").ToList().Where(w => w != "," && w != "").ToList();
                            var tick3_ = tic3.Distinct().ToList(); s.Ticket3 = "";
                            if (tick3_ != null && tick3_.Count > 0)
                            {
                                tick3_.ForEach(a =>
                                {
                                    if (!string.IsNullOrEmpty(a))
                                    {
                                        s.Ticket3 += tic3.Where(q => q == a).Count() + "*" + a.Split(".")[0] + "/";
                                        if (!string.IsNullOrEmpty(s.NoOfTicket3))
                                        {
                                            s.Ticket3 += s.NoOfTicket3 + "/";
                                        }
                                    }
                                });
                            }
                            s.Ticket3 += s.ProdOrderNo3;
                        }
                        var GroupTicket4 = s.GroupItemTicket4 + "," + s.GroupTicket4;
                        if (GroupTicket4 != null)
                        {
                            var tic4 = GroupTicket4.Split(",").ToList().Where(w => w != "," && w != "").ToList();
                            var tick4_ = tic4.Distinct().ToList(); s.Ticket4 = "";
                            if (tick4_ != null && tick4_.Count > 0)
                            {
                                tick4_.ForEach(a =>
                                {
                                    if (!string.IsNullOrEmpty(a))
                                    {
                                        s.Ticket4 += tic4.Where(q => q == a).Count() + "*" + a.Split(".")[0] + "/";
                                        if (!string.IsNullOrEmpty(s.NoOfTicket4))
                                        {
                                            s.Ticket4 += s.NoOfTicket4 + "/";
                                        }
                                    }
                                });
                            }
                            s.Ticket4 += s.ProdOrderNo4;
                        }
                        var GroupTicket5 = s.GroupItemTicket5 + "," + s.GroupTicket5;
                        if (GroupTicket5 != null)
                        {
                            var tic5 = GroupTicket5.Split(",").ToList().Where(w => w != "," && w != "").ToList();
                            var tick5_ = tic5.Distinct().ToList(); s.Ticket5 = "";
                            if (tick5_ != null && tick5_.Count > 0)
                            {
                                tick5_.ForEach(a =>
                                {
                                    if (!string.IsNullOrEmpty(a))
                                    {
                                        s.Ticket5 += tic5.Where(q => q == a).Count() + "*" + a.Split(".")[0] + "/";
                                        if (!string.IsNullOrEmpty(s.NoOfTicket5))
                                        {
                                            s.Ticket5 += s.NoOfTicket5 + "/";
                                        }
                                    }
                                });
                            }
                            s.Ticket5 += s.ProdOrderNo5;
                        }
                        var GroupTicket6 = s.GroupItemTicket6 + "," + s.GroupTicket6;
                        if (GroupTicket6 != null)
                        {
                            var tic6 = GroupTicket6.Split(",").ToList().Where(w => w != "," && w != "").ToList();
                            var tick6_ = tic6.Distinct().ToList(); s.Ticket6 = "";
                            if (tick6_ != null && tick6_.Count > 0)
                            {
                                tick6_.ForEach(a =>
                                {
                                    if (!string.IsNullOrEmpty(a))
                                    {
                                        s.Ticket6 += tic6.Where(q => q == a).Count() + "*" + a.Split(".")[0] + "/";
                                        if (!string.IsNullOrEmpty(s.NoOfTicket6))
                                        {
                                            s.Ticket6 += s.NoOfTicket6 + "/";
                                        }
                                    }
                                });
                            }
                            s.Ticket6 += s.ProdOrderNo6;
                        }
                        var GroupTicket7 = s.GroupItemTicket7 + "," + s.GroupTicket7;
                        if (GroupTicket7 != null)
                        {
                            var tic7 = GroupTicket7.Split(",").ToList().Where(w => w != "," && w != "").ToList();
                            var tick7_ = tic7.Distinct().ToList(); s.Ticket7 = "";
                            if (tick7_ != null && tick7_.Count > 0)
                            {
                                tick7_.ForEach(a =>
                                {
                                    if (!string.IsNullOrEmpty(a))
                                    {
                                        s.Ticket7 += tic7.Where(q => q == a).Count() + "*" + a.Split(".")[0] + "/";
                                        if (!string.IsNullOrEmpty(s.NoOfTicket7))
                                        {
                                            s.Ticket7 += s.NoOfTicket7 + "/";
                                        }
                                    }
                                });
                            }
                            s.Ticket7 += s.ProdOrderNo7;
                        }
                        var GroupTicket8 = s.GroupItemTicket8 + "," + s.GroupTicket8;
                        if (GroupTicket8 != null)
                        {
                            var tic8 = GroupTicket8.Split(",").ToList().Where(w => w != "," && w != "").ToList();
                            var tick8_ = tic8.Distinct().ToList(); s.Ticket8 = "";
                            if (tick8_ != null && tick8_.Count > 0)
                            {
                                tick8_.ForEach(a =>
                                {
                                    if (!string.IsNullOrEmpty(a))
                                    {
                                        s.Ticket8 += tic8.Where(q => q == a).Count() + "*" + a.Split(".")[0] + "/";
                                        if (!string.IsNullOrEmpty(s.NoOfTicket8))
                                        {
                                            s.Ticket8 += s.NoOfTicket8 + "/";
                                        }
                                    }
                                });
                            }
                            s.Ticket8 += s.ProdOrderNo8;
                        }
                        var GroupTicket9 = s.GroupItemTicket9 + "," + s.GroupTicket9;
                        if (GroupTicket9 != null)
                        {
                            var tic9 = GroupTicket9.Split(",").ToList().Where(w => w != "," && w != "").ToList();
                            var tick9_ = tic9.Distinct().ToList(); s.Ticket9 = "";
                            if (tick9_ != null && tick9_.Count > 0)
                            {
                                tick9_.ForEach(a =>
                                {
                                    if (!string.IsNullOrEmpty(a))
                                    {
                                        s.Ticket9 += tic9.Where(q => q == a).Count() + "*" + a.Split(".")[0] + "/";
                                        if (!string.IsNullOrEmpty(s.NoOfTicket9))
                                        {
                                            s.Ticket9 += s.NoOfTicket9 + "/";
                                        }
                                    }
                                });
                            }
                            s.Ticket9 += s.ProdOrderNo9;
                        }
                        var GroupTicket10 = s.GroupItemTicket10 + "," + s.GroupTicket10;
                        if (GroupTicket10 != null)
                        {
                            var tic10 = GroupTicket10.Split(",").ToList().Where(w => w != "," && w != "").ToList();
                            var tick10_ = tic10.Distinct().ToList(); s.Ticket10 = "";
                            if (tick10_ != null && tick10_.Count > 0)
                            {
                                tick10_.ForEach(a =>
                                {
                                    if (!string.IsNullOrEmpty(a))
                                    {
                                        s.Ticket10 += tic10.Where(q => q == a).Count() + "*" + a.Split(".")[0] + "/";
                                        if (!string.IsNullOrEmpty(s.NoOfTicket10))
                                        {
                                            s.Ticket10 += s.NoOfTicket10 + "/";
                                        }
                                    }
                                });
                            }
                            s.Ticket10 += s.ProdOrderNo10;
                        }
                        var GroupTicket11 = s.GroupItemTicket11 + "," + s.GroupTicket11;
                        if (GroupTicket11 != null)
                        {
                            var tic11 = GroupTicket11.Split(",").ToList().Where(w => w != "," && w != "").ToList();
                            var tick11_ = tic11.Distinct().ToList(); s.Ticket11 = "";
                            if (tick11_ != null && tick11_.Count > 0)
                            {
                                tick11_.ForEach(a =>
                                {
                                    if (!string.IsNullOrEmpty(a))
                                    {
                                        s.Ticket11 += tic11.Where(q => q == a).Count() + "*" + a.Split(".")[0] + "/";
                                        if (!string.IsNullOrEmpty(s.NoOfTicket11))
                                        {
                                            s.Ticket11 += s.NoOfTicket11 + "/";
                                        }
                                    }
                                });
                            }
                            s.Ticket11 += s.ProdOrderNo11;
                        }
                        var GroupTicket12 = s.GroupItemTicket12 + "," + s.GroupTicket12;
                        if (GroupTicket12 != null)
                        {
                            var tic12 = GroupTicket12.Split(",").ToList().Where(w => w != "," && w != "").ToList();
                            var tick12_ = tic12.Distinct().ToList(); s.Ticket12 = "";
                            if (tick12_ != null && tick12_.Count > 0)
                            {
                                tick12_.ForEach(a =>
                                {
                                    if (!string.IsNullOrEmpty(a))
                                    {
                                        s.Ticket12 += tic12.Where(q => q == a).Count() + "*" + a.Split(".")[0] + "/";
                                        if (!string.IsNullOrEmpty(s.NoOfTicket12))
                                        {
                                            s.Ticket12 += s.NoOfTicket12 + "/";
                                        }
                                    }
                                });
                            }
                            s.Ticket12 += s.ProdOrderNo12;
                        }
                    });
                }
                return acreports;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        private int GetWeekNumberOfMonth(DateTime date)
        {
            DateTime firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            int firstDay = (int)firstDayOfMonth.DayOfWeek;
            if (firstDay == 0)
            {
                firstDay = 7;
            }
            double d = (firstDay + date.Day - 1) / 7.0;
            return d > 5 ? (int)Math.Floor(d) : (int)Math.Ceiling(d);
        }
        private async Task<List<INPCalendarPivotModel>> SimulationMidMonth(DateRangeModel endDate, List<NAVRecipesModel> recipeList, List<NAVRecipesModel> _orderRecipeList)
        {
            var categoryList = new List<string>
            {
                "CAP",
                "CREAM",
                "DD",
                "SYRUP",
                "TABLET",
                "VET",
                "POWDER",
                "INJ"
            };
            var MethodCodeList = new List<INPCalendarPivotModel>();
            var companyIds = new List<long?> { endDate.CompanyId };
            if (endDate.CompanyId == 3)
            {
                companyIds = new List<long?> { 1, 2 };
            }
            var intercompanyIds = new List<long?> { 1, 2 };
            var month = endDate.StockMonth.Month;//== 1 ? 12 : endDate.StockMonth.Month - 1;
            var year = endDate.StockMonth.Year;// == 1 ? endDate.StockMonth.Year - 1 : endDate.StockMonth.Year;
            var weekofmonth = GetWeekNumberOfMonth(endDate.StockMonth);
            var intransitMonth = month == 1 ? 12 : month - 1;
            DateTime lastDay = new DateTime(endDate.StockMonth.Year, endDate.StockMonth.Month, 1).AddMonths(1).AddDays(-1);

            var doNotReceivedList = new List<NavpostedShipment>();
            var navMethodCodeLines = new List<NavMethodCodeLines>(); var acitems = new List<NavItemCitemList>();
            var acItemBalListResult = new List<DistStockBalModel>(); var acEntries = new List<ACItemsModel>(); var dismapeditems = new List<NavItemCitemList>();
            var categoryItems = new List<NavSaleCategory>(); var itemRelations = new List<NavitemLinks>(); var prodyctionTickets = new List<ProductionSimulation>();
            var prenavStkbalance = new List<NavitemStockBalance>(); var navStkbalance = new List<NavitemStockBalance>(); var prodyctionoutTickets = new List<ProductionSimulation>();
            var blanletOrders = new List<SimulationAddhoc>(); var pre_acItemBalListResult = new List<DistStockBalModel>(); var grouptickets = new List<GroupPlaningTicket>();
            var intercompanyItems = new List<Navitems>(); var itemMasterforReport = new List<Navitems>(); var orderRequirements = new List<OrderRequirementLineModel>(); var acEntriesList = new List<Acitems>();
            DateTime firstDayOfMonth = new DateTime(endDate.StockMonth.Year, endDate.StockMonth.Month, 1);
            var dateMonth1 = firstDayOfMonth;// endDate.StockMonth;
            var datemonth12 = endDate.StockMonth.AddMonths(12);
            using (var connection = CreateConnection())
            {
                var query = "select ShipmentId,\r\nCompany,\r\nCompanyId,\r\nStockBalanceMonth,\r\nPostingDate,\r\nCustomer,\r\nCustomerNo,\r\nCustomerId,\r\nDeliveryOrderNo,\r\nDOLineNo,\r\nItemNo,\r\nDescription,\r\nIsRecived,\r\nDoQty,\r\nStatusCodeID,\r\nAddedByUserID,\r\nAddedDate,\r\nModifiedByUserID from NavpostedShipment WHERE CompanyId  in(" + string.Join(',', intercompanyIds) + ") AND CAST(StockBalanceMonth AS Date) <='" + lastDay + "'  AND (IsRecived is null Or IsRecived=0)\r\n;";
                query += "select t2.DistAcid,(case when t1.Quantity is NULL then  0 ELSE t1.Quantity END) as QtyOnHand,t2.ItemNo,t2.ItemDesc,t2.DistName,t1.DistItemId,(case when t2.CustomerId is NULL then  -1 ELSE t2.CustomerId END) as CustomerId,t2.CompanyId from " +
                    "DistStockBalance t1 INNER JOIN Acitems t2  ON t1.DistItemId=t2.DistACID\r\n" +
                    "Where  (t1.StockBalWeek=" + weekofmonth + " OR t1.StockBalWeek is null ) AND MONTH(t1.StockBalMonth) = " + month + " AND YEAR(t1.StockBalMonth)=" + year + ";\r\n";
                query += "select t2.DistAcid,(case when t1.Quantity is NULL then  0 ELSE t1.Quantity END) as QtyOnHand,t2.ItemNo,t2.ItemDesc,t2.DistName,t1.DistItemId,(case when t2.CustomerId is NULL then  -1 ELSE t2.CustomerId END) as CustomerId,t2.CompanyId from " +
                    "DistStockBalance t1 INNER JOIN Acitems t2  ON t1.DistItemId=t2.DistACID\r\n" +
                    "Where   t1.StockBalWeek=1 AND MONTH(t1.StockBalMonth) = " + month + " AND YEAR(t1.StockBalMonth)=" + year + "\r\n;";
                query += "select t1.CustomerId as DistId,t1.ToDate as ACMonth,t3.No as ItemNo,t2.Quantity as ACQty,t2.ItemId as SWItemId,t3.Description as ItemDesc,t1.CustomerId from Acentry t1 INNER JOIN AcentryLines t2 ON t1.ACEntryId=t2.ACEntryId INNER JOIN NAVItems t3 ON t2.ItemId=t3.ItemId\r\n" +
                    "WHERE t1.CompanyId  in(" + string.Join(',', companyIds) + ") AND CAST(t1.ToDate AS Date)>='" + endDate.StockMonth + "'  AND CAST(t1.FromDate AS Date)<='" + endDate.StockMonth + "';\r\n";
                query += "select NavItemCItemId,\r\nNavItemId,\r\nNavItemCustomerItemId from NavItemCitemList;\r\n";
                query += "select SaleCategoryID,\r\nCode,\r\nDescription,\r\nNoSeries,\r\nLocationID,\r\nSGNoSeries,\r\nStatusCodeID,\r\nAddedByUserID,\r\nAddedDate,\r\nModifiedByUserID,\r\nModifiedDate from NavSaleCategory;\r\n";
                query += "select ItemLinkId,\r\nMyItemId,\r\nSgItemId,\r\nStatusCodeID,\r\nAddedByUserID,\r\nAddedDate,\r\nModifiedByUserID,\r\nModifiedDate from NavitemLinks where SgItemId is not null AND MyItemId is not null;\r\n";
                query += "select ProductionSimulationID,\r\nCompanyId,\r\nProdOrderNo,\r\nItemID,\r\nItemNo,\r\nDescription,\r\nPackSize,\r\nQuantity,\r\nUOM,\r\nPerQuantity,\r\nPerQtyUOM,\r\nBatchNo,\r\nPlannedQty,\r\nOutputQty,\r\nIsOutput,\r\nStartingDate,\r\nStatusCodeID,\r\nAddedByUserID,\r\nAddedDate,\r\nModifiedByUserID,\r\nModifiedDate,\r\nProcessDate,\r\nIsBMRTicket,\r\nRePlanRefNo,\r\nBatchSize,\r\nDispense from ProductionSimulation where IsBmrticket=0 AND CAST(StartingDate AS Date)>='" + dateMonth1 + "'  AND CAST(StartingDate AS Date)<='" + datemonth12 + "' order by StartingDate desc;\r\n";

                query += "select ProductionSimulationID,\r\nCompanyId,\r\nProdOrderNo,\r\nItemID,\r\nItemNo,\r\nDescription,\r\nPackSize,\r\nQuantity,\r\nUOM,\r\nPerQuantity,\r\nPerQtyUOM,\r\nBatchNo,\r\nPlannedQty,\r\nOutputQty,\r\nIsOutput,\r\nStartingDate,\r\nStatusCodeID,\r\nAddedByUserID,\r\nAddedDate,\r\nModifiedByUserID,\r\nModifiedDate,\r\nProcessDate,\r\nIsBMRTicket,\r\nRePlanRefNo,\r\nBatchSize,\r\nDispense from ProductionSimulation where IsBmrticket=0 AND CAST(ProcessDate AS Date)>='" + dateMonth1 + "'  AND CAST(ProcessDate AS Date)<='" + datemonth12 + "' order by ProcessDate desc;\r\n";

                query += "SELECT t1.NavStockBalanceId,\r\nt1.ItemId,\r\nt1.StockBalMonth,\r\nt1.StockBalWeek,\r\nt1.Quantity,\r\nt1.RejectQuantity,\r\nt1.StatusCodeID,\r\nt1.AddedByUserID,\r\nt1.AddedDate,\r\nt1.ModifiedByUserID,\r\nt1.ModifiedDate,\r\nt1.ReworkQty,\r\nt1.WIPQty,\r\nt1.GlobalQty,\r\nt1.KIVQty,\r\nt1.SupplyWIPQty,\r\nt1.Supply1ProcessQty,\r\nt1.NotStartInvQty,t2.PackQty from NavitemStockBalance t1 INNER JOIN navitems t2 ON t1.ItemId=t2.ItemId Where t1.StockBalWeek=1 AND t2.ItemId is Not NUll and MONTH(t1.StockBalMonth) =" + month + " AND YEAR(t1.StockBalMonth)=" + year + ";\r\n";

                query += "SELECT t1.NavStockBalanceId,\r\nt1.ItemId,\r\nt1.StockBalMonth,\r\nt1.StockBalWeek,\r\nt1.Quantity,\r\nt1.RejectQuantity,\r\nt1.StatusCodeID,\r\nt1.AddedByUserID,\r\nt1.AddedDate,\r\nt1.ModifiedByUserID,\r\nt1.ModifiedDate,\r\nt1.ReworkQty,\r\nt1.WIPQty,\r\nt1.GlobalQty,\r\nt1.KIVQty,\r\nt1.SupplyWIPQty,\r\nt1.Supply1ProcessQty,\r\nt1.NotStartInvQty,t2.PackQty from NavitemStockBalance t1 INNER JOIN navitems t2 ON t1.ItemId=t2.ItemId Where (t1.StockBalWeek is null OR t1.StockBalWeek=" + weekofmonth + ") AND t2.ItemId is Not NUll and MONTH(t1.StockBalMonth) =" + month + " AND YEAR(t1.StockBalMonth)=" + year + ";\r\n";
                query += "select t1.SimualtionAddhocID,\r\nt1.DocumantType,\r\nt1.SelltoCustomerNo,\r\nt1.CustomerName,\r\nt1.Categories,\r\nt1.DocumentNo,\r\nt1.ExternalDocNo,\r\nt1.ItemID,\r\nt1.ItemNo,\r\nt1.Description,\r\nt1.Description1,\r\nt1.OutstandingQty,\r\nt1.PromisedDate,\r\nt1.ShipmentDate,\r\nt1.UOMCode from SimulationAddhoc t1 where  t1.CompanyId in(" + string.Join(',', companyIds) + ") AND CAST(t1.ShipmentDate AS Date)>='" + dateMonth1 + "'  AND CAST(t1.ShipmentDate AS Date)<='" + datemonth12 + "';\r\n";

                query += "select GroupPlanningId,\r\nCompanyId,\r\nBatchName,\r\nProductGroupCode,\r\nStartDate,\r\nItemNo,\r\nDescription,\r\nDescription1,\r\nBatchSize,\r\nQuantity,\r\nUOM,\r\nNoOfTicket,\r\nTotalQuantity from GroupPlaningTicket where CompanyId=" + endDate.CompanyId + " AND CAST(StartDate AS Date)>='" + dateMonth1 + "'  AND CAST(StartDate AS Date)<='" + datemonth12 + "' order by StartDate desc;\r\n";
                query += "select s.No,s.ItemId,s.Description,s.PackQty,s.CompanyId from NAVItems s;";
                query += "select t1.*,t2.Description2 as GenericCodeDescription2 from NAVItems t1\r\nLEFT JOIN GenericCodes t2 ON t1.GenericCodeId=t2.GenericCodeId\r\n" +
                    "where t1.CompanyId  in(" + string.Join(',', companyIds) + ") AND  t1.StatusCodeId=1 AND t1.ItemId IN(select NavItemId from NavItemCitemList WHere NavItemId>0);\r\n";


                query += "select t1.ProductId,t1.ProductQty,t1.NoOfTicket,t1.ExpectedStartDate,t1.RequireToSplit,t2.SplitProductID,t2.SplitProductQty from OrderRequirementLine t1\r\nINNER JOIN OrderRequirementLineSplit t2 ON t1.OrderRequirementLineID=t2.OrderRequirementLineID\r\nwhere t1.IsNavSync=1 AND CAST(t1.ExpectedStartDate AS Date)>='" + dateMonth1 + "'  AND CAST(t1.ExpectedStartDate AS Date)<='" + datemonth12 + "' order by t1.ExpectedStartDate desc;\r\n";
                query += "select t1.*,t6.DistACID,\r\nt6.CompanyId,\r\nt6.CustomerId,\r\nt6.DistName,\r\nt6.ItemGroup,\r\nt6.Steriod,\r\nt6.ShelfLife,\r\nt6.Quota,\r\nt6.Status,\r\nt6.ItemDesc,\r\nt6.PackSize,\r\nt6.ACQty,\r\nt6.ACMonth,\r\nt6.StatusCodeID,\r\nt6.AddedByUserID,\r\nt6.AddedDate,\r\nt6.ModifiedByUserID,\r\nt6.ModifiedDate,\r\nt6.ItemNo from NavItemCitemList t1\r\nINNER JOIN Acitems t6 ON t1.NavItemCustomerItemId=t6.DistACID;\r\n";
                query += "select t1.MethodCodeLineId,t1.MethodCodeId,t1.ItemID,t1.StatusCodeID,t1.AddedByUserID,t1.AddedDate,t1.ModifiedByUserID,t1.MethodCodeLineID,\r\nt2.MethodName,t2.MethodDescription,t2.NAVINPCategoryID,t2.CompanyId,t2.ProdFrequency,t2.DistReplenishHS,t2.DistACMonth,t2.AdhocMonthStandAlone,t2.AdhocPlanQty from NavMethodCodeLines t1 INNER JOIN NavMethodCode t2 ON t2.MethodCodeId=t1.MethodCodeId \r\nWHERE t1.ItemID IN(select t3.ItemId from NAVItems t3 LEFT JOIN GenericCodes t4 ON t3.GenericCodeId=t4.GenericCodeId where t3.CompanyId  in(" + string.Join(',', companyIds) + ") AND  t3.StatusCodeId=1 AND t3.ItemId IN(select NavItemId from NavItemCitemList WHere NavItemId>0));\r\n";
                query += "select t1.NavItemCitemId,t1.NavItemId,t1.NavItemCustomerItemId,t2.DistACID,t2.CompanyId,t2.DistName,t2.ItemGroup,t2.Steriod,t2.ShelfLife,t2.Quota,t2.ItemDesc,t2.PackSize,t2.PackSize,t2.ACQty,t2.ItemNo from NavItemCitemList t1 INNER JOIN Acitems t2 ON t1.NavItemCustomerItemId=t2.DistACID\r\nWHERE t1.NavItemId IN(select t3.ItemId from NAVItems t3 LEFT JOIN GenericCodes t4 ON t3.GenericCodeId=t4.GenericCodeId where t3.CompanyId  in(" + string.Join(',', companyIds) + ") AND  t3.StatusCodeId=1 AND t3.ItemId IN(select NavItemId from NavItemCitemList WHere NavItemId>0));\r\n";
                var results = await connection.QueryMultipleAsync(query);
                doNotReceivedList = results.Read<NavpostedShipment>().ToList();
                acItemBalListResult = results.Read<DistStockBalModel>().ToList();
                pre_acItemBalListResult = results.Read<DistStockBalModel>().ToList();
                acEntries = results.Read<ACItemsModel>().ToList();
                dismapeditems = results.Read<NavItemCitemList>().ToList();
                categoryItems = results.Read<NavSaleCategory>().ToList();
                itemRelations = results.Read<NavitemLinks>().ToList();
                prodyctionTickets = results.Read<ProductionSimulation>().ToList();
                prodyctionoutTickets = results.Read<ProductionSimulation>().ToList();
                prenavStkbalance = results.Read<NavitemStockBalance>().ToList();
                navStkbalance = results.Read<NavitemStockBalance>().ToList();
                blanletOrders = results.Read<SimulationAddhoc>().ToList();
                grouptickets = results.Read<GroupPlaningTicket>().ToList();
                intercompanyItems = results.Read<Navitems>().ToList();
                itemMasterforReport = results.Read<Navitems>().ToList();
                orderRequirements = results.Read<OrderRequirementLineModel>().ToList();
                acEntriesList = results.Read<Acitems>().ToList();
                navMethodCodeLines = results.Read<NavMethodCodeLines>().ToList();
                acitems = results.Read<NavItemCitemList>().ToList();
            }
            List<long?> idss = new List<long?>();
            if (itemMasterforReport != null && itemMasterforReport.Count() > 0)
            {
                itemMasterforReport.ForEach(s =>
                {
                    var idsss = navMethodCodeLines.Where(w => w.ItemId == s.ItemId).ToList();

                    s.NavMethodCodeLines = idsss;

                    if (idsss.Count() == 0)
                    {
                        long? itemids = (long?)s.ItemId;
                        idss.Add(itemids);
                    }
                    s.NavItemCitemList = acitems.Where(w => w.NavItemId == s.ItemId).ToList();
                });
            }
            string a1 = string.Join(",", idss);
            var packSize = 900;
            decimal packSize2 = 0;
            var tenderExist1 = false;
            var tenderExist2 = false;
            var tenderExist3 = false;
            var tenderExist4 = false;
            var tenderExist5 = false;
            var tenderExist6 = false;
            var tenderExist7 = false;
            var tenderExist8 = false;
            var tenderExist9 = false;
            var tenderExist10 = false;
            var tenderExist11 = false;
            var tenderExist12 = false;


            var month1 = month;
            var month2 = month1 + 1 > 12 ? 1 : month1 + 1;
            var month3 = month2 + 1 > 12 ? 1 : month2 + 1;
            var month4 = month3 + 1 > 12 ? 1 : month3 + 1;
            var month5 = month4 + 1 > 12 ? 1 : month4 + 1;
            var month6 = month5 + 1 > 12 ? 1 : month5 + 1;
            var month7 = month6 + 1 > 12 ? 1 : month6 + 1;
            var month8 = month7 + 1 > 12 ? 1 : month7 + 1;
            var month9 = month8 + 1 > 12 ? 1 : month8 + 1;
            var month10 = month9 + 1 > 12 ? 1 : month9 + 1;
            var month11 = month10 + 1 > 12 ? 1 : month10 + 1;
            var month12 = month11 + 1 > 12 ? 1 : month11 + 1;

            var nextYear = year + 1;
            var year1 = year;
            var year2 = month2 > month ? year : nextYear;
            var year3 = month3 > month ? year : nextYear;
            var year4 = month4 > month ? year : nextYear;
            var year5 = month5 > month ? year : nextYear;
            var year6 = month6 > month ? year : nextYear;
            var year7 = month7 > month ? year : nextYear;
            var year8 = month8 > month ? year : nextYear;
            var year9 = month9 > month ? year : nextYear;
            var year10 = month10 > month ? year : nextYear;
            var year11 = month11 > month ? year : nextYear;
            var year12 = month12 > month ? year : nextYear;



            var preMonth = month - 1;
            var preYear = preMonth == 12 ? year - 1 : year;


            var acModel = new List<ACItemsModel>();
            if (acEntries != null && acEntries.Count > 0)
            {
                acEntries.ForEach(ac =>
                {
                    if (ac.CustomerId == 62)
                    {
                        ac.DistName = "Apex";
                    }
                    else if (ac.CustomerId == 51)
                    {
                        ac.DistName = "PSB PX";
                    }
                    else if (ac.CustomerId == 39)
                    {
                        ac.DistName = "MSS";
                    }
                    else if (ac.CustomerId == 60)
                    {
                        ac.DistName = "SG Tender";
                    }
                    else
                    {
                        ac.DistName = "Antah";
                    }
                    acModel.Add(ac);
                });

            }

            var dismapeditemIds = dismapeditems.Select(s => s.NavItemId).ToList();
            var navids = itemMasterforReport.Select(s => s.ItemId).ToList();

            if (prenavStkbalance != null && prenavStkbalance.Count() > 0)
            {
                prenavStkbalance.ForEach(f =>
                {
                    f.Quantity = f.Quantity * (long)(f.PackQty > 0 ? f.PackQty : 0);
                });
            }
            if (navStkbalance != null && navStkbalance.Count() > 0)
            {
                navStkbalance.ForEach(f =>
                {
                    f.Quantity = f.Quantity * (long)(f.PackQty > 0 ? f.PackQty : 0);
                });
            }

            doNotReceivedList.ForEach(d =>
            {
                var item = intercompanyItems.FirstOrDefault(f => f.No == d.ItemNo && f.CompanyId == d.CompanyId);
                if (item != null)
                {
                    d.DoQty *= item.PackQty;
                }
            });
            int? parent = null;
            var genericId = new List<long?>();

            var orederRequ = new List<OrderRequirementLineModel>();

            orderRequirements.ForEach(f =>
            {
                if (!f.RequireToSplit.GetValueOrDefault(false))
                {
                    orederRequ.Add(new OrderRequirementLineModel
                    {
                        ProductId = f.ProductId,
                        ProductQty = f.ProductQty * f.NoOfTicket,
                        NoOfTicket = f.NoOfTicket,
                        ExpectedStartDate = f.ExpectedStartDate
                    });
                }
                else
                {
                    orederRequ.Add(new OrderRequirementLineModel
                    {
                        ProductId = f.SplitProductId,
                        ProductQty = f.SplitProductQty * f.NoOfTicket,
                        NoOfTicket = f.NoOfTicket,
                        ExpectedStartDate = f.ExpectedStartDate
                    });
                }
            });

            itemMasterforReport.ForEach(ac =>
            {
                var dismapeditemsLines = dismapeditems.Where(w => w.NavItemId == ac.ItemId).ToList();
                if (ac.No == "FP-PP-TAB-244")
                {

                }

                if (!genericId.Exists(g => g == ac.GenericCodeId) && ac.GenericCodeId != null)
                {
                    var geericRptList = new List<GenericCodeReport>();
                    var itemNosRpt = itemMasterforReport.Where(i => i.GenericCodeId == ac.GenericCodeId).ToList();
                    var itemNos = itemMasterforReport.Where(i => i.GenericCodeId == ac.GenericCodeId).Select(s => s.ItemId).ToList();
                    var item_Nos = itemMasterforReport.Where(i => i.GenericCodeId == ac.GenericCodeId).Select(s => s.No).ToList();
                    //itemMasterforReport.Where(i => i.GenericCodeId == ac.GenericCodeId).Select(s => string.Format("{0} - {1} - {2}", s.No, s.Description, s.Description2)).ToList();
                    var itemCodes = itemMasterforReport.Where(i => i.GenericCodeId == ac.GenericCodeId).Select(s => s.No).ToList();
                    itemNosRpt.ForEach(rpt =>
                    {
                        var distId = dismapeditems.Where(d => d.NavItemId == rpt.ItemId).Select(s => s.NavItemCustomerItemId);
                        geericRptList.Add(new GenericCodeReport
                        {
                            ItemNo = rpt.No,
                            Description = rpt.Description,
                            Description2 = rpt.Description2,
                            ItemCategory = rpt.CategoryId.GetValueOrDefault(0) > 0 ? categoryList[int.Parse(rpt.CategoryId.ToString()) - 1] : string.Empty,
                            InternalRefNo = rpt.InternalRef,
                            //MethodCode = rpt.NavMethodCodeLines.Count > 0 ? rpt.NavMethodCodeLines.First().MethodCode.MethodName : "No MethodCode",
                            //DistItem = rpt.NavItemCitemList.Count > 0 ? rpt.NavItemCitemList.FirstOrDefault().NavItemCustomerItem.ItemDesc : string.Empty,
                            //StockBalance = acItemBalListResult.Where(a => distId.Contains(a.DistItemId)).Sum(s => s.QtyOnHand)
                        });

                    });
                    bool itemMapped = false;
                    decimal SgTenderStockBalance = 0;
                    decimal pre_SgTenderStockBalance = 0;
                    genericId.Add(ac.GenericCodeId);
                    decimal? preinventoryQty = 0;
                    decimal? inventoryQty = 0;//itemMasterforReport.Where(i => i.GenericCodeId == ac.GenericCodeId).Sum(s => s.Inventory);
                    var navItemIds = itemMasterforReport.Where(i => i.GenericCodeId == ac.GenericCodeId).Select(s => s.ItemId).ToList();

                    inventoryQty = (navStkbalance.Where(n => navItemIds.Contains(n.ItemId.Value)).Sum(s => s.Quantity));
                    decimal? wipQty = (navStkbalance.Where(n => navItemIds.Contains(n.ItemId.Value)).Sum(s => s.Wipqty));
                    decimal? notStartInvQty = (navStkbalance.Where(n => navItemIds.Contains(n.ItemId.Value)).Sum(s => s.NotStartInvQty));
                    var reWorkQty = (navStkbalance.Where(n => navItemIds.Contains(n.ItemId.Value)).Sum(s => s.ReworkQty));
                    var globalQty = (navStkbalance.Where(n => navItemIds.Contains(n.ItemId.Value)).Sum(s => s.GlobalQty));

                    //previous month NAV stock
                    preinventoryQty = (prenavStkbalance.Where(n => navItemIds.Contains(n.ItemId.Value)).Sum(s => s.Quantity));
                    var prewipQty = (prenavStkbalance.Where(n => navItemIds.Contains(n.ItemId.Value)).Sum(s => s.Wipqty));
                    var preNotStartInvQty = (prenavStkbalance.Where(n => navItemIds.Contains(n.ItemId.Value)).Sum(s => s.NotStartInvQty));
                    var prereWorkQty = (prenavStkbalance.Where(n => navItemIds.Contains(n.ItemId.Value)).Sum(s => s.ReworkQty));
                    var preglobalQty = (prenavStkbalance.Where(n => navItemIds.Contains(n.ItemId.Value)).Sum(s => s.GlobalQty));
                    if (ac.No == "FP-PP-CRM-100")
                    {

                    }
                    decimal? interCompanyTransitQty = 0;
                    if (endDate.CompanyId == 1)
                    {
                        var linkItemIds = itemRelations.Where(f => navItemIds.Contains(f.MyItemId.Value)).Select(s => s.SgItemId).ToList();
                        if (linkItemIds.Count > 0)
                        {
                            itemMapped = true;
                            SgTenderStockBalance = navStkbalance.Where(n => linkItemIds.Contains(n.ItemId.Value)).Sum(s => s.Quantity.GetValueOrDefault(0));
                            pre_SgTenderStockBalance = prenavStkbalance.Where(n => linkItemIds.Contains(n.ItemId.Value)).Sum(s => s.Quantity.GetValueOrDefault(0));
                            var itemNosIntertrans = intercompanyItems.Where(t => linkItemIds.Contains(t.ItemId)).Select(n => n.No).ToList();
                            interCompanyTransitQty = doNotReceivedList.Where(d => itemNosIntertrans.Contains(d.ItemNo) && d.IsRecived == false && d.CompanyId == 2).Sum(s => s.DoQty);
                        }


                    }
                    else if (endDate.CompanyId == 2)
                    {
                        var linkItemIds = itemRelations.Where(f => navItemIds.Contains(f.SgItemId.Value)).Select(s => s.MyItemId).ToList();
                        if (linkItemIds.Count > 0)
                        {
                            itemMapped = true;
                            SgTenderStockBalance = navStkbalance.Where(n => linkItemIds.Contains(n.ItemId.Value)).Sum(s => s.Quantity.GetValueOrDefault(0));
                            pre_SgTenderStockBalance = prenavStkbalance.Where(n => linkItemIds.Contains(n.ItemId.Value)).Sum(s => s.Quantity.GetValueOrDefault(0));

                            var itemNosIntertrans = intercompanyItems.Where(t => linkItemIds.Contains(t.ItemId)).Select(n => n.No).ToList();
                            interCompanyTransitQty = doNotReceivedList.Where(d => itemNosIntertrans.Contains(d.ItemNo) && d.IsRecived == false && d.CompanyId == 1).Sum(s => s.DoQty);
                        }
                    }
                    else if (endDate.CompanyId == 3)
                    {
                        var linkItemIds = itemRelations.Where(f => navItemIds.Contains(f.MyItemId.Value)).Select(s => s.SgItemId).ToList();
                        if (linkItemIds.Count > 0)
                        {
                            itemMapped = true;
                            inventoryQty = (navStkbalance.Where(n => linkItemIds.Contains(n.ItemId.Value)).Sum(s => s.Quantity.GetValueOrDefault(0)));
                            preinventoryQty = (prenavStkbalance.Where(n => linkItemIds.Contains(n.ItemId.Value)).Sum(s => s.Quantity.GetValueOrDefault(0)));
                        }

                        var sglinkItemIds = itemRelations.Where(f => navItemIds.Contains(f.SgItemId.Value)).Select(s => s.MyItemId).ToList();
                        if (linkItemIds.Count > 0)
                        {
                            SgTenderStockBalance = navStkbalance.Where(n => sglinkItemIds.Contains(n.ItemId.Value)).Sum(s => s.Quantity.GetValueOrDefault(0));
                            pre_SgTenderStockBalance = prenavStkbalance.Where(n => sglinkItemIds.Contains(n.ItemId.Value)).Sum(s => s.Quantity.GetValueOrDefault(0));
                        }
                    }

                    var custmerItem = ac.NavItemCitemList.Count > 0 ? ac.NavItemCitemList.FirstOrDefault() : new NavItemCitemList();
                    if (custmerItem.DistAcid > 0)
                    {
                        var distIds = dismapeditems.Where(di => navItemIds.Contains(di.NavItemId.Value)).Select(s => s.NavItemCustomerItemId).Distinct().ToList(); //ac.NavItemCitemList.Select(s => s.NavItemCustomerItemId).ToList();
                        var distStkBal = acItemBalListResult.Where(d => distIds.Contains(d.DistItemId) && d.CompanyId == endDate.CompanyId).Sum(s => s.QtyOnHand);
                        var val = ac.PackSize.HasValue ? ac.PackSize.Value : 900;
                        packSize = val;

                        var prodRecipe = "";
                        var currentBatch = "";
                        var methodeCodeID = ac.NavMethodCodeLines.Count() > 0 ? ac.NavMethodCodeLines.First().MethodCodeId : -1;
                        var itemreceips = recipeList.Where(r => r.ItemRecipeId == methodeCodeID).ToList();
                        if (itemreceips.Count > 0)
                        {
                            var batchSizedes = itemreceips.FirstOrDefault().DefaultBatch;
                            currentBatch = batchSizedes;
                            //string numberOnly = Regex.Replace(batchSizedes, "[^0-9.]", "");
                            //if (!string.IsNullOrEmpty(numberOnly))
                            packSize2 = itemreceips.FirstOrDefault().UnitQTY;
                            prodRecipe = string.Join(",", itemreceips.Select(r => r.RecipeNo).ToList());//string.Format("{0}", itemreceips.FirstOrDefault().DefaultBatch + " | " + batchSizedes);

                        }
                        var Orderitemreceips = _orderRecipeList.Where(r => r.ItemNo == ac.No).ToList();
                        //if (Orderitemreceips.Count > 0)
                        //{
                        //    //var batchSizedes = itemreceips.OrderByDescending(o => o.BatchSize).FirstOrDefault().BatchSize;
                        //    //string numberOnly = Regex.Replace(batchSizedes, "[^0-9.]", "");
                        //    //if (!string.IsNullOrEmpty(numberOnly))
                        //    //    packSize2 = decimal.Parse(numberOnly) * 1000;
                        //    //prodRecipe = string.Format("{0}", itemreceips.OrderByDescending(o => o.BatchSize).FirstOrDefault().RecipeNo + " | " + batchSizedes);

                        //}
                        var threeMonth = distStkBal * packSize * 3;
                        var BatchSize = 900000 / packSize / 50;
                        var StockBalance = distStkBal + inventoryQty;
                        var NoofTickets = (threeMonth / packSize * 1000);
                        //var custId = custmerItem.DistName == "Apex" ? 21 : 1;
                        var acitem = acModel.Where(f => navItemIds.Contains(f.SWItemId.Value)).ToList();
                        var distItemBal = acItemBalListResult.Where(f => distIds.Contains(f.DistAcid) && f.CompanyId == endDate.CompanyId).ToList();
                        var pre_distItemBal = pre_acItemBalListResult.Where(f => distIds.Contains(f.DistAcid) && f.CompanyId == endDate.CompanyId).ToList();

                        var apexQty = acitem.FirstOrDefault(f => f.CustomerId == 62) != null ? acitem.Where(f => f.CustomerId == 62).Sum(s => s.ACQty) : 0;
                        var antahQty = acitem.FirstOrDefault(f => f.CustomerId == 1) != null ? acitem.Where(f => f.CustomerId == 1).Sum(s => s.ACQty) : 0;
                        var sgtQty = acitem.FirstOrDefault(f => f.CustomerId == 60) != null ? acitem.Where(f => f.CustomerId == 60).Sum(s => s.ACQty) : 0;
                        var missQty = acitem.FirstOrDefault(f => f.CustomerId == 39) != null ? acitem.Where(f => f.CustomerId == 39).Sum(s => s.ACQty) : 0;
                        var pxQty = acitem.FirstOrDefault(f => f.CustomerId == 51) != null ? acitem.Where(f => f.CustomerId == 51).Sum(s => s.ACQty) : 0;
                        var symlQty = packSize * (apexQty + antahQty + 0 + missQty + pxQty);
                        threeMonth = symlQty * 3;
                        var distTotal = (apexQty + antahQty + 0 + missQty + pxQty);
                        var AntahStockBalance = distItemBal.FirstOrDefault(f => f.CustomerId == 1) != null ? distItemBal.Where(f => f.CustomerId == 1).Sum(s => s.QtyOnHand) : 0;
                        var ApexStockBalance = distItemBal.FirstOrDefault(f => f.CustomerId == 62) != null ? distItemBal.Where(f => f.CustomerId == 62).Sum(s => s.QtyOnHand) : 0;
                        if (!itemMapped) SgTenderStockBalance = distItemBal.FirstOrDefault(f => f.DistName == "SG Tender") != null ? distItemBal.Where(f => f.DistName == "SG Tender").Sum(s => s.QtyOnHand) : 0;
                        var MsbStockBalance = distItemBal.FirstOrDefault(f => f.CustomerId == 39) != null ? distItemBal.Where(f => f.CustomerId == 39).Sum(s => s.QtyOnHand) : 0;
                        var PsbStockBalance = distItemBal.FirstOrDefault(f => f.CustomerId == 51) != null ? distItemBal.Where(f => f.CustomerId == 51).Sum(s => s.QtyOnHand) : 0;

                        var pre_AntahStockBalance = pre_distItemBal.FirstOrDefault(f => f.CustomerId == 1) != null ? pre_distItemBal.Where(f => f.CustomerId == 1).Sum(s => s.QtyOnHand) : 0;
                        var pre_ApexStockBalance = pre_distItemBal.FirstOrDefault(f => f.CustomerId == 62) != null ? pre_distItemBal.Where(f => f.CustomerId == 62).Sum(s => s.QtyOnHand) : 0;
                        if (!itemMapped) pre_SgTenderStockBalance = pre_distItemBal.FirstOrDefault(f => f.DistName == "SG Tender") != null ? pre_distItemBal.Where(f => f.DistName == "SG Tender").Sum(s => s.QtyOnHand) : 0;
                        var pre_MsbStockBalance = pre_distItemBal.FirstOrDefault(f => f.CustomerId == 39) != null ? pre_distItemBal.Where(f => f.CustomerId == 39).Sum(s => s.QtyOnHand) : 0;
                        var pre_PsbStockBalance = pre_distItemBal.FirstOrDefault(f => f.CustomerId == 51) != null ? pre_distItemBal.Where(f => f.CustomerId == 51).Sum(s => s.QtyOnHand) : 0;


                        decimal stockHoldingBalance = 0;

                        //decimal premyStockBalance = 0;
                        decimal prestockHoldingBalance = 0;

                        var inTransitQty = doNotReceivedList.Where(d => itemCodes.Contains(d.ItemNo) && d.IsRecived == false && d.CompanyId == endDate.CompanyId).Sum(s => s.DoQty);
                        if (distTotal > 0)
                        {
                            stockHoldingBalance = (interCompanyTransitQty.GetValueOrDefault(0) + wipQty.GetValueOrDefault(0) + notStartInvQty.GetValueOrDefault(0) + 0 + globalQty.GetValueOrDefault(0) + AntahStockBalance + ApexStockBalance + MsbStockBalance + PsbStockBalance + SgTenderStockBalance + inventoryQty.GetValueOrDefault(0) + inTransitQty.GetValueOrDefault(0)) / distTotal;
                            prestockHoldingBalance = (prewipQty.GetValueOrDefault(0) + notStartInvQty.GetValueOrDefault(0) + 0 + preglobalQty.GetValueOrDefault(0) + pre_AntahStockBalance + pre_ApexStockBalance + pre_MsbStockBalance + pre_PsbStockBalance + pre_SgTenderStockBalance + preinventoryQty.GetValueOrDefault(0)) / distTotal;
                        }

                        var isTenderExist = blanletOrders.Any(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)));
                        tenderExist1 = blanletOrders.Any(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month1 && t.ShipmentDate.Value.Year == year1);
                        tenderExist2 = blanletOrders.Any(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month2 && t.ShipmentDate.Value.Year == year2);
                        tenderExist3 = blanletOrders.Any(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month3 && t.ShipmentDate.Value.Year == year3);
                        tenderExist4 = blanletOrders.Any(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month4 && t.ShipmentDate.Value.Year == year4);
                        tenderExist5 = blanletOrders.Any(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month5 && t.ShipmentDate.Value.Year == year5);
                        tenderExist6 = blanletOrders.Any(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month6 && t.ShipmentDate.Value.Year == year6);
                        tenderExist7 = blanletOrders.Any(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month7 && t.ShipmentDate.Value.Year == year7);
                        tenderExist8 = blanletOrders.Any(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month8 && t.ShipmentDate.Value.Year == year8);
                        tenderExist9 = blanletOrders.Any(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month9 && t.ShipmentDate.Value.Year == year9);
                        tenderExist10 = blanletOrders.Any(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month10 && t.ShipmentDate.Value.Year == year10);
                        tenderExist11 = blanletOrders.Any(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month11 && t.ShipmentDate.Value.Year == year11);
                        tenderExist12 = blanletOrders.Any(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month12 && t.ShipmentDate.Value.Year == year12);
                        var groupItemNo = itemMasterforReport.Where(i => i.GenericCodeId == ac.GenericCodeId).Select(s => s.No).ToList();
                        //var tenderSum = blanletOrders.Where(t => itemNos.Contains(t.ItemId.Value)).Sum(s => s.OutstandingQty.Value);
                        MethodCodeList.Add(new INPCalendarPivotModel
                        {
                            GenericCodeReport = geericRptList,
                            // ItemList = itemNos,
                            ItemId = ac.ItemId,
                            ItemNo = ac.GenericCodeDescription2,
                            RecipeLists = itemreceips.Select(s => string.Format("{0}", s.RecipeNo + " | " + s.BatchSize)).ToList(),
                            ItemRecipeLists = itemreceips,
                            OrderRecipeLists = Orderitemreceips,
                            IsSteroid = ac.Steroid.GetValueOrDefault(false),
                            SalesCategoryId = ac.CategoryId,
                            LocationID = categoryItems != null && categoryItems.FirstOrDefault(a => a.SaleCategoryId == ac.CategoryId) != null ? categoryItems.FirstOrDefault(a => a.SaleCategoryId == ac.CategoryId).LocationId : null,
                            SalesCategory = categoryItems != null && categoryItems.FirstOrDefault(a => a.SaleCategoryId == ac.CategoryId) != null ? categoryItems.FirstOrDefault(a => a.SaleCategoryId == ac.CategoryId).Code : "",
                            MethodCodeId = ac.NavMethodCodeLines.Count > 0 ? ac.NavMethodCodeLines.First().MethodCodeId : -1,
                            GenericCodeID = ac.GenericCodeId,
                            Customer = custmerItem.DistName,
                            Description = ac.Description + " " + ac.Description2,
                            PackSize = packSize,
                            PackSize2 = packSize2,
                            Quantity = distStkBal,
                            ApexQty = apexQty,
                            AntahQty = antahQty,
                            SgtQty = sgtQty,
                            PxQty = pxQty,
                            MissQty = missQty,
                            SymlQty = symlQty,
                            ACQty = distStkBal,
                            AcSum = distTotal,
                            UnitQty = distStkBal * packSize,
                            ThreeMonthACQty = threeMonth,
                            ProdRecipe = !string.IsNullOrEmpty(prodRecipe) ? prodRecipe : "",
                            BatchSize = "",
                            NoofTickets = "",
                            NoOfDays = "Hours " + (3 * NoofTickets) + "& Day" + (3 * NoofTickets / 8).ToString(),
                            DistStockBalance = (AntahStockBalance + ApexStockBalance + MsbStockBalance + PsbStockBalance + SgTenderStockBalance + inventoryQty.GetValueOrDefault(0)),
                            AntahStockBalance = AntahStockBalance,
                            ApexStockBalance = ApexStockBalance,
                            SgTenderStockBalance = endDate.CompanyId == 2 ? inventoryQty.GetValueOrDefault(0) : SgTenderStockBalance,
                            MsbStockBalance = MsbStockBalance,
                            PsbStockBalance = PsbStockBalance,
                            NAVStockBalance = inventoryQty.GetValueOrDefault(0),
                            WipQty = wipQty.GetValueOrDefault(0),
                            NotStartInvQty = notStartInvQty.GetValueOrDefault(0),
                            Rework = reWorkQty.GetValueOrDefault(0),
                            InterCompanyTransitQty = interCompanyTransitQty.GetValueOrDefault(0),
                            OtherStoreQty = globalQty.GetValueOrDefault(0),
                            StockBalance = (interCompanyTransitQty.GetValueOrDefault(0) + notStartInvQty.GetValueOrDefault(0) + wipQty.GetValueOrDefault(0) + 0 + globalQty.GetValueOrDefault(0) + AntahStockBalance + ApexStockBalance + MsbStockBalance + PsbStockBalance + SgTenderStockBalance + inventoryQty.GetValueOrDefault(0) + inTransitQty.Value),
                            StockHoldingPackSize = (interCompanyTransitQty.GetValueOrDefault(0) + notStartInvQty.GetValueOrDefault(0) + wipQty.GetValueOrDefault(0) + 0 + globalQty.GetValueOrDefault(0) + AntahStockBalance + ApexStockBalance + MsbStockBalance + PsbStockBalance + SgTenderStockBalance + inventoryQty.GetValueOrDefault(0) + inTransitQty.Value) * packSize,
                            StockHoldingBalance = stockHoldingBalance,
                            MyStockBalance = endDate.CompanyId == 1 || endDate.CompanyId == 3 ? inventoryQty.GetValueOrDefault(0) : SgTenderStockBalance,
                            SgStockBalance = endDate.CompanyId == 2 ? inventoryQty.GetValueOrDefault(0) : SgTenderStockBalance,
                            MethodCode = ac.NavMethodCodeLines.Count > 0 ? ac.NavMethodCodeLines.First().MethodName : "No MethodCode",
                            Month = endDate.StockMonth.ToString("MMMM"),
                            Remarks = "AC",
                            BatchSize90 = currentBatch,
                            Roundup1 = packSize2 > 0 ? threeMonth / packSize2 : 0,
                            Roundup2 = 0,
                            ReportMonth = endDate.StockMonth,
                            UOM = ac.BaseUnitofMeasure,
                            Packuom = ac.PackUom,
                            Replenishment = ac.VendorNo,
                            StatusCodeId = ac.StatusCodeId,
                            isTenderExist = isTenderExist,
                            //TenderSum = tenderSum,
                            Month1 = stockHoldingBalance > decimal.Parse(".5") ? stockHoldingBalance - decimal.Parse(".5") : 0,
                            Month2 = stockHoldingBalance > 1 ? stockHoldingBalance - 1 : 0,
                            Month3 = stockHoldingBalance > 3 ? stockHoldingBalance - 3 : 0,
                            Month4 = stockHoldingBalance > 4 ? stockHoldingBalance - 4 : 0,
                            Month5 = stockHoldingBalance > 5 ? stockHoldingBalance - 5 : 0,
                            Month6 = stockHoldingBalance > 6 ? stockHoldingBalance - 6 : 0,
                            Month7 = stockHoldingBalance > 7 ? stockHoldingBalance - 7 : 0,
                            Month8 = stockHoldingBalance > 8 ? stockHoldingBalance - 8 : 0,
                            Month9 = stockHoldingBalance > 9 ? stockHoldingBalance - 9 : 0,
                            Month10 = stockHoldingBalance > 10 ? stockHoldingBalance - 10 : 0,
                            Month11 = stockHoldingBalance > 11 ? stockHoldingBalance - 11 : 0,
                            Month12 = stockHoldingBalance > 12 ? stockHoldingBalance - 12 : 0,
                            DeliverynotReceived = inTransitQty,//doNotReceivedList.Where(d => d.ItemNo == ac.No && d.IsRecived == false).Sum(s => s.DoQty),

                            GroupItemTicket1 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month1 && t.StartDate.Value.Year == year1).Select(s => s.Quantity).ToList()),
                            GroupItemTicket2 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month2 && t.StartDate.Value.Year == year2).Select(s => s.Quantity).ToList()),
                            GroupItemTicket3 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month3 && t.StartDate.Value.Year == year3).Select(s => s.Quantity).ToList()),
                            GroupItemTicket4 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month4 && t.StartDate.Value.Year == year4).Select(s => s.Quantity).ToList()),
                            GroupItemTicket5 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month5 && t.StartDate.Value.Year == year5).Select(s => s.Quantity).ToList()),
                            GroupItemTicket6 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month6 && t.StartDate.Value.Year == year6).Select(s => s.Quantity).ToList()),
                            GroupItemTicket7 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month7 && t.StartDate.Value.Year == year7).Select(s => s.Quantity).ToList()),
                            GroupItemTicket8 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month8 && t.StartDate.Value.Year == year8).Select(s => s.Quantity).ToList()),
                            GroupItemTicket9 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month9 && t.StartDate.Value.Year == year9).Select(s => s.Quantity).ToList()),
                            GroupItemTicket10 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month10 && t.StartDate.Value.Year == year10).Select(s => s.Quantity).ToList()),
                            GroupItemTicket11 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month11 && t.StartDate.Value.Year == year11).Select(s => s.Quantity).ToList()),
                            GroupItemTicket12 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month12 && t.StartDate.Value.Year == year12).Select(s => s.Quantity).ToList()),

                            NoOfTicket1 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month1 && t.StartDate.Value.Year == year1).Select(s => s.NoOfTicket).ToList()),
                            NoOfTicket2 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month2 && t.StartDate.Value.Year == year2).Select(s => s.NoOfTicket).ToList()),
                            NoOfTicket3 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month3 && t.StartDate.Value.Year == year3).Select(s => s.NoOfTicket).ToList()),
                            NoOfTicket4 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month4 && t.StartDate.Value.Year == year4).Select(s => s.NoOfTicket).ToList()),
                            NoOfTicket5 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month5 && t.StartDate.Value.Year == year5).Select(s => s.NoOfTicket).ToList()),
                            NoOfTicket6 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month6 && t.StartDate.Value.Year == year6).Select(s => s.NoOfTicket).ToList()),
                            NoOfTicket7 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month7 && t.StartDate.Value.Year == year7).Select(s => s.NoOfTicket).ToList()),
                            NoOfTicket8 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month8 && t.StartDate.Value.Year == year8).Select(s => s.NoOfTicket).ToList()),
                            NoOfTicket9 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month9 && t.StartDate.Value.Year == year9).Select(s => s.NoOfTicket).ToList()),
                            NoOfTicket10 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month10 && t.StartDate.Value.Year == year10).Select(s => s.NoOfTicket).ToList()),
                            NoOfTicket11 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month11 && t.StartDate.Value.Year == year11).Select(s => s.NoOfTicket).ToList()),
                            NoOfTicket12 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month12 && t.StartDate.Value.Year == year12).Select(s => s.NoOfTicket).ToList()),



                            Ticket1 = string.Format("{0}/{1}", string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month1 && t.StartDate.Value.Year == year1).Select(s => s.Quantity).ToList()), string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month1 && t.StartDate.Value.Year == year1).Select(s => s.NoOfTicket).ToList())),
                            Ticket2 = string.Format("{0}/{1}", string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month2 && t.StartDate.Value.Year == year2).Select(s => s.Quantity).ToList()), string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month2 && t.StartDate.Value.Year == year2).Select(s => s.NoOfTicket).ToList())),
                            Ticket3 = string.Format("{0}/{1}", string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month3 && t.StartDate.Value.Year == year3).Select(s => s.Quantity).ToList()), string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month3 && t.StartDate.Value.Year == year3).Select(s => s.NoOfTicket).ToList())),
                            Ticket4 = string.Format("{0}/{1}", string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month4 && t.StartDate.Value.Year == year4).Select(s => s.Quantity).ToList()), string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month4 && t.StartDate.Value.Year == year4).Select(s => s.NoOfTicket).ToList())),
                            Ticket5 = string.Format("{0}/{1}", string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month5 && t.StartDate.Value.Year == year5).Select(s => s.Quantity).ToList()), string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month5 && t.StartDate.Value.Year == year5).Select(s => s.NoOfTicket).ToList())),
                            Ticket6 = string.Format("{0}/{1}", string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month6 && t.StartDate.Value.Year == year6).Select(s => s.Quantity).ToList()), string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month6 && t.StartDate.Value.Year == year6).Select(s => s.NoOfTicket).ToList())),
                            Ticket7 = string.Format("{0}/{1}", string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month7 && t.StartDate.Value.Year == year7).Select(s => s.Quantity).ToList()), string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month7 && t.StartDate.Value.Year == year7).Select(s => s.NoOfTicket).ToList())),
                            Ticket8 = string.Format("{0}/{1}", string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month8 && t.StartDate.Value.Year == year8).Select(s => s.Quantity).ToList()), string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month8 && t.StartDate.Value.Year == year8).Select(s => s.NoOfTicket).ToList())),
                            Ticket9 = string.Format("{0}/{1}", string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month9 && t.StartDate.Value.Year == year9).Select(s => s.Quantity).ToList()), string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month9 && t.StartDate.Value.Year == year9).Select(s => s.NoOfTicket).ToList())),
                            Ticket10 = string.Format("{0}/{1}", string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month10 && t.StartDate.Value.Year == year10).Select(s => s.Quantity).ToList()), string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month10 && t.StartDate.Value.Year == year10).Select(s => s.NoOfTicket).ToList())),
                            Ticket11 = string.Format("{0}/{1}", string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month11 && t.StartDate.Value.Year == year11).Select(s => s.Quantity).ToList()), string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month11 && t.StartDate.Value.Year == year11).Select(s => s.NoOfTicket).ToList())),
                            Ticket12 = string.Format("{0}/{1}", string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month12 && t.StartDate.Value.Year == year12).Select(s => s.Quantity).ToList()), string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month12 && t.StartDate.Value.Year == year12).Select(s => s.NoOfTicket).ToList())),




                            OutputTicket1 = string.Format("{0}/{1}", string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month1).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month1).Select(s => s.ProdOrderNo).ToList())),
                            OutputTicket2 = string.Format("{0}/{1}", string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month2).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month2).Select(s => s.ProdOrderNo).ToList())),
                            OutputTicket3 = string.Format("{0}/{1}", string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month3).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month3).Select(s => s.ProdOrderNo).ToList())),
                            OutputTicket4 = string.Format("{0}/{1}", string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month4).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month4).Select(s => s.ProdOrderNo).ToList())),
                            OutputTicket5 = string.Format("{0}/{1}", string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month5).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month5).Select(s => s.ProdOrderNo).ToList())),
                            OutputTicket6 = string.Format("{0}/{1}", string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month6).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month6).Select(s => s.ProdOrderNo).ToList())),
                            OutputTicket7 = string.Format("{0}/{1}", string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month7).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month7).Select(s => s.ProdOrderNo).ToList())),
                            OutputTicket8 = string.Format("{0}/{1}", string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month8).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month8).Select(s => s.ProdOrderNo).ToList())),
                            OutputTicket9 = string.Format("{0}/{1}", string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month9).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month9).Select(s => s.ProdOrderNo).ToList())),
                            OutputTicket10 = string.Format("{0}/{1}", string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month10).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month10).Select(s => s.ProdOrderNo).ToList())),
                            OutputTicket11 = string.Format("{0}/{1}", string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month11).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month11).Select(s => s.ProdOrderNo).ToList())),
                            OutputTicket12 = string.Format("{0}/{1}", string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month12).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month12).Select(s => s.ProdOrderNo).ToList())),

                            TicketHoldingStock1 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month1).Sum(s => s.NoOfTicket.GetValueOrDefault(0)),
                            TicketHoldingStock2 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month2).Sum(s => s.NoOfTicket.GetValueOrDefault(0)),
                            TicketHoldingStock3 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month3).Sum(s => s.NoOfTicket.GetValueOrDefault(0)),
                            TicketHoldingStock4 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month4).Sum(s => s.NoOfTicket.GetValueOrDefault(0)),
                            TicketHoldingStock5 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month5).Sum(s => s.NoOfTicket.GetValueOrDefault(0)),
                            TicketHoldingStock6 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month6).Sum(s => s.NoOfTicket.GetValueOrDefault(0)),
                            TicketHoldingStock7 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month7).Sum(s => s.NoOfTicket.GetValueOrDefault(0)),
                            TicketHoldingStock8 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month8).Sum(s => s.NoOfTicket.GetValueOrDefault(0)),
                            TicketHoldingStock9 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month9).Sum(s => s.NoOfTicket.GetValueOrDefault(0)),
                            TicketHoldingStock10 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month10).Sum(s => s.NoOfTicket.GetValueOrDefault(0)),
                            TicketHoldingStock11 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month11).Sum(s => s.NoOfTicket.GetValueOrDefault(0)),
                            TicketHoldingStock12 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month12).Sum(s => s.NoOfTicket.GetValueOrDefault(0)),

                            ProductionTicket1 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month1).Sum(s => s.ProductQty.GetValueOrDefault(0)),
                            ProductionTicket2 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month2).Sum(s => s.ProductQty.GetValueOrDefault(0)),
                            ProductionTicket3 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month3).Sum(s => s.ProductQty.GetValueOrDefault(0)),
                            ProductionTicket4 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month4).Sum(s => s.ProductQty.GetValueOrDefault(0)),
                            ProductionTicket5 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month5).Sum(s => s.ProductQty.GetValueOrDefault(0)),
                            ProductionTicket6 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month6).Sum(s => s.ProductQty.GetValueOrDefault(0)),
                            ProductionTicket7 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month7).Sum(s => s.ProductQty.GetValueOrDefault(0)),
                            ProductionTicket8 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month8).Sum(s => s.ProductQty.GetValueOrDefault(0)),
                            ProductionTicket9 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month9).Sum(s => s.ProductQty.GetValueOrDefault(0)),
                            ProductionTicket10 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month10).Sum(s => s.ProductQty.GetValueOrDefault(0)),
                            ProductionTicket11 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month11).Sum(s => s.ProductQty.GetValueOrDefault(0)),
                            ProductionTicket12 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month12).Sum(s => s.ProductQty.GetValueOrDefault(0)),



                            IsTenderExist1 = tenderExist1,//blanletOrders.Any(t => itemNos.Contains(t.ItemId.Value) && t.ShipmentDate.Value.Month == month1),
                            IsTenderExist2 = tenderExist2,// blanletOrders.Any(t => itemNos.Contains(t.ItemId.Value) && t.ShipmentDate.Value.Month == month2),
                            IsTenderExist3 = tenderExist3,// blanletOrders.Any(t => itemNos.Contains(t.ItemId.Value) && t.ShipmentDate.Value.Month == month3),
                            IsTenderExist4 = tenderExist4,//blanletOrders.Any(t => itemNos.Contains(t.ItemId.Value) && t.ShipmentDate.Value.Month == month4),
                            IsTenderExist5 = tenderExist5,//blanletOrders.Any(t => itemNos.Contains(t.ItemId.Value) && t.ShipmentDate.Value.Month == month5),
                            IsTenderExist6 = tenderExist6,//blanletOrders.Any(t => itemNos.Contains(t.ItemId.Value) && t.ShipmentDate.Value.Month == month6),
                            IsTenderExist7 = tenderExist7,//blanletOrders.Any(t => itemNos.Contains(t.ItemId.Value) && t.ShipmentDate.Value.Month == month7),
                            IsTenderExist8 = tenderExist8,//blanletOrders.Any(t => itemNos.Contains(t.ItemId.Value) && t.ShipmentDate.Value.Month == month8),
                            IsTenderExist9 = tenderExist9,//blanletOrders.Any(t => itemNos.Contains(t.ItemId.Value) && t.ShipmentDate.Value.Month == month9),
                            IsTenderExist10 = tenderExist10,//blanletOrders.Any(t => itemNos.Contains(t.ItemId.Value) && t.ShipmentDate.Value.Month == month10),
                            IsTenderExist11 = tenderExist11,//blanletOrders.Any(t => itemNos.Contains(t.ItemId.Value) && t.ShipmentDate.Value.Month == month11),
                            IsTenderExist12 = tenderExist12,//blanletOrders.Any(t => itemNos.Contains(t.ItemId.Value) && t.ShipmentDate.Value.Month == month12),


                            ProjectedHoldingStock1 = distTotal > 0 ? prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month1).Sum(s => s.Quantity) / (distTotal) : prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month1).Sum(s => s.Quantity),
                            ProjectedHoldingStock2 = distTotal > 0 ? prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month2).Sum(s => s.Quantity) / (distTotal) : prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month2).Sum(s => s.Quantity),
                            ProjectedHoldingStock3 = distTotal > 0 ? prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month3).Sum(s => s.Quantity) / (distTotal) : prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month3).Sum(s => s.Quantity),
                            ProjectedHoldingStock4 = distTotal > 0 ? prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month4).Sum(s => s.Quantity) / (distTotal) : prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month4).Sum(s => s.Quantity),
                            ProjectedHoldingStock5 = distTotal > 0 ? prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month5).Sum(s => s.Quantity) / (distTotal) : prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month5).Sum(s => s.Quantity),
                            ProjectedHoldingStock6 = distTotal > 0 ? prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month6).Sum(s => s.Quantity) / (distTotal) : prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month6).Sum(s => s.Quantity),
                            ProjectedHoldingStock7 = distTotal > 0 ? prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month7).Sum(s => s.Quantity) / (distTotal) : prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month7).Sum(s => s.Quantity),
                            ProjectedHoldingStock8 = distTotal > 0 ? prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month8).Sum(s => s.Quantity) / (distTotal) : prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month8).Sum(s => s.Quantity),
                            ProjectedHoldingStock9 = distTotal > 0 ? prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month9).Sum(s => s.Quantity) / (distTotal) : prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month9).Sum(s => s.Quantity),
                            ProjectedHoldingStock10 = distTotal > 0 ? prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month10).Sum(s => s.Quantity) / (distTotal) : prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month10).Sum(s => s.Quantity),
                            ProjectedHoldingStock11 = distTotal > 0 ? prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month11).Sum(s => s.Quantity) / (distTotal) : prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month11).Sum(s => s.Quantity),
                            ProjectedHoldingStock12 = distTotal > 0 ? prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month12).Sum(s => s.Quantity) / (distTotal) : prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month12).Sum(s => s.Quantity),

                            OutputProjectedHoldingStock1 = 0,// distTotal > 0 ? prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month1).Sum(s => s.Quantity) / (distTotal) : prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month1).Sum(s => s.Quantity),
                            OutputProjectedHoldingStock2 = 0,// distTotal > 0 ? prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month2).Sum(s => s.Quantity) / (distTotal) : prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month2).Sum(s => s.Quantity),
                            OutputProjectedHoldingStock3 = 0,// distTotal > 0 ? prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month3).Sum(s => s.Quantity) / (distTotal) : prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month3).Sum(s => s.Quantity),
                            OutputProjectedHoldingStock4 = 0,//distTotal > 0 ? prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month4).Sum(s => s.Quantity) / (distTotal) : prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month4).Sum(s => s.Quantity),
                            OutputProjectedHoldingStock5 = 0,// distTotal > 0 ? prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month5).Sum(s => s.Quantity) / (distTotal) : prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month5).Sum(s => s.Quantity),
                            OutputProjectedHoldingStock6 = 0,//distTotal > 0 ? prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month6).Sum(s => s.Quantity) / (distTotal) : prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month6).Sum(s => s.Quantity),
                            OutputProjectedHoldingStock7 = 0,//distTotal > 0 ? prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month7).Sum(s => s.Quantity) / (distTotal) : prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month7).Sum(s => s.Quantity),
                            OutputProjectedHoldingStock8 = 0,// distTotal > 0 ? prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month8).Sum(s => s.Quantity) / (distTotal) : prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month8).Sum(s => s.Quantity),
                            OutputProjectedHoldingStock9 = 0,//distTotal > 0 ? prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month9).Sum(s => s.Quantity) / (distTotal) : prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month9).Sum(s => s.Quantity),
                            OutputProjectedHoldingStock10 = 0,// distTotal > 0 ? prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month10).Sum(s => s.Quantity) / (distTotal) : prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month10).Sum(s => s.Quantity),
                            OutputProjectedHoldingStock11 = 0,// distTotal > 0 ? prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month11).Sum(s => s.Quantity) / (distTotal) : prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month11).Sum(s => s.Quantity),
                            OutputProjectedHoldingStock12 = 0,//distTotal > 0 ? prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month12).Sum(s => s.Quantity) / (distTotal) : prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month12).Sum(s => s.Quantity),


                            BlanketAddhoc1 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month1 && t.ShipmentDate.Value.Year == year1).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                            BlanketAddhoc2 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month2 && t.ShipmentDate.Value.Year == year2).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                            BlanketAddhoc3 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month3 && t.ShipmentDate.Value.Year == year3).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                            BlanketAddhoc4 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month4 && t.ShipmentDate.Value.Year == year4).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                            BlanketAddhoc5 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month5 && t.ShipmentDate.Value.Year == year5).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                            BlanketAddhoc6 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month6 && t.ShipmentDate.Value.Year == year6).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                            BlanketAddhoc7 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month7 && t.ShipmentDate.Value.Year == year7).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                            BlanketAddhoc8 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month8 && t.ShipmentDate.Value.Year == year8).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                            BlanketAddhoc9 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month9 && t.ShipmentDate.Value.Year == year9).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                            BlanketAddhoc10 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month10 && t.ShipmentDate.Value.Year == year10).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                            BlanketAddhoc11 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month11 && t.ShipmentDate.Value.Year == year11).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                            BlanketAddhoc12 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month12 && t.ShipmentDate.Value.Year == year12).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),

                            //previous month 
                            PreAntahStockBalance = pre_AntahStockBalance,
                            PreApexStockBalance = pre_ApexStockBalance,
                            PreMsbStockBalance = pre_MsbStockBalance,
                            PrePsbStockBalance = pre_PsbStockBalance,
                            PreSgTenderStockBalance = endDate.CompanyId == 2 ? preinventoryQty.GetValueOrDefault(0) : pre_SgTenderStockBalance,
                            PreMyStockBalance = endDate.CompanyId == 1 || endDate.CompanyId == 3 ? preinventoryQty.GetValueOrDefault(0) : pre_SgTenderStockBalance,
                            PreOtherStoreQty = preglobalQty.GetValueOrDefault(0),
                            PreStockBalance = (prewipQty.GetValueOrDefault(0) + preNotStartInvQty.GetValueOrDefault(0) + 0 + preglobalQty.GetValueOrDefault(0) + pre_AntahStockBalance + pre_ApexStockBalance + pre_MsbStockBalance + pre_PsbStockBalance + pre_SgTenderStockBalance + preinventoryQty.GetValueOrDefault(0)),
                            PreStockHoldingBalance = prestockHoldingBalance,
                            PrewipQty = prewipQty.GetValueOrDefault(0),
                            PreNotStartInvQty = preNotStartInvQty.GetValueOrDefault(0),
                        });
                    }
                }
            });
            MethodCodeList = MethodCodeList.Where(f => f.StatusCodeId == 1).ToList();

            MethodCodeList.ForEach(f =>
            {

                if (f.ItemId == 5799)
                {

                }
                var itemNos = itemMasterforReport.Where(i => i.GenericCodeId == f.GenericCodeID).Select(s => s.ItemId).ToList();
                var item_Nos = itemMasterforReport.Where(i => i.GenericCodeId == f.GenericCodeID).Select(s => s.No).ToList();


                f.GroupTicket1 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month1).Select(s => s.Quantity).ToList());
                f.GroupTicket2 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month2).Select(s => s.Quantity).ToList());
                f.GroupTicket3 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month3).Select(s => s.Quantity).ToList());
                f.GroupTicket4 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month4).Select(s => s.Quantity).ToList());
                f.GroupTicket5 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month5).Select(s => s.Quantity).ToList());
                f.GroupTicket6 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month6).Select(s => s.Quantity).ToList());
                f.GroupTicket7 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month7).Select(s => s.Quantity).ToList());
                f.GroupTicket8 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month8).Select(s => s.Quantity).ToList());
                f.GroupTicket9 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month9).Select(s => s.Quantity).ToList());
                f.GroupTicket10 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month10).Select(s => s.Quantity).ToList());
                f.GroupTicket11 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month11).Select(s => s.Quantity).ToList());
                f.GroupTicket12 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month12).Select(s => s.Quantity).ToList());

                f.ProdOrderNo1 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month1).Select(s => s.ProdOrderNo).ToList());
                f.ProdOrderNo2 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month2).Select(s => s.ProdOrderNo).ToList());
                f.ProdOrderNo3 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month3).Select(s => s.ProdOrderNo).ToList());
                f.ProdOrderNo4 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month4).Select(s => s.ProdOrderNo).ToList());
                f.ProdOrderNo5 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month5).Select(s => s.ProdOrderNo).ToList());
                f.ProdOrderNo6 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month6).Select(s => s.ProdOrderNo).ToList());
                f.ProdOrderNo7 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month7).Select(s => s.ProdOrderNo).ToList());
                f.ProdOrderNo8 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month8).Select(s => s.ProdOrderNo).ToList());
                f.ProdOrderNo9 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month9).Select(s => s.ProdOrderNo).ToList());
                f.ProdOrderNo10 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month10).Select(s => s.ProdOrderNo).ToList());
                f.ProdOrderNo11 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month11).Select(s => s.ProdOrderNo).ToList());
                f.ProdOrderNo12 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month12).Select(s => s.ProdOrderNo).ToList());

                f.ProTicket1 = string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month1).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month1).Select(s => s.ProdOrderNo).ToList()));
                f.ProTicket2 = string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month2).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month2).Select(s => s.ProdOrderNo).ToList()));
                f.ProTicket3 = string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month3).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month3).Select(s => s.ProdOrderNo).ToList()));
                f.ProTicket4 = string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month4).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month4).Select(s => s.ProdOrderNo).ToList()));
                f.ProTicket5 = string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month5).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month5).Select(s => s.ProdOrderNo).ToList()));
                f.ProTicket6 = string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month6).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month6).Select(s => s.ProdOrderNo).ToList()));
                f.ProTicket7 = string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month7).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month7).Select(s => s.ProdOrderNo).ToList()));
                f.ProTicket8 = string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month8).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month8).Select(s => s.ProdOrderNo).ToList()));
                f.ProTicket9 = string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month9).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month9).Select(s => s.ProdOrderNo).ToList()));
                f.ProTicket10 = string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month10).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month10).Select(s => s.ProdOrderNo).ToList()));
                f.ProTicket11 = string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month11).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month11).Select(s => s.ProdOrderNo).ToList()));
                f.ProTicket12 = string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month12).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month12).Select(s => s.ProdOrderNo).ToList()));


                f.Ticket1 = f.Ticket1 + f.ProTicket1;
                f.Ticket2 = f.Ticket2 + f.ProTicket2;
                f.Ticket3 = f.Ticket3 + f.ProTicket3;
                f.Ticket4 = f.Ticket4 + f.ProTicket4;
                f.Ticket5 = f.Ticket5 + f.ProTicket5;
                f.Ticket6 = f.Ticket6 + f.ProTicket6;
                f.Ticket7 = f.Ticket7 + f.ProTicket7;
                f.Ticket8 = f.Ticket8 + f.ProTicket8;
                f.Ticket9 = f.Ticket9 + f.ProTicket9;
                f.Ticket10 = f.Ticket10 + f.ProTicket10;
                f.Ticket11 = f.Ticket11 + f.ProTicket11;
                f.Ticket12 = f.Ticket12 + f.ProTicket12;

                f.ProjectedHoldingStock1 += f.AcSum > 0 ? grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month1 && t.StartDate.Value.Year == year1).Sum(s => s.TotalQuantity.Value) / (f.AcSum) : grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month1 && t.StartDate.Value.Year == year1).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStock2 += f.AcSum > 0 ? grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month2 && t.StartDate.Value.Year == year2).Sum(s => s.TotalQuantity.Value) / (f.AcSum) : grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month2 && t.StartDate.Value.Year == year2).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStock3 += f.AcSum > 0 ? grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month3 && t.StartDate.Value.Year == year3).Sum(s => s.TotalQuantity.Value) / (f.AcSum) : grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month3 && t.StartDate.Value.Year == year3).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStock4 += f.AcSum > 0 ? grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month4 && t.StartDate.Value.Year == year4).Sum(s => s.TotalQuantity.Value) / (f.AcSum) : grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month4 && t.StartDate.Value.Year == year4).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStock5 += f.AcSum > 0 ? grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month5 && t.StartDate.Value.Year == year5).Sum(s => s.TotalQuantity.Value) / (f.AcSum) : grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month5 && t.StartDate.Value.Year == year5).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStock6 += f.AcSum > 0 ? grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month6 && t.StartDate.Value.Year == year6).Sum(s => s.TotalQuantity.Value) / (f.AcSum) : grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month6 && t.StartDate.Value.Year == year6).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStock7 += f.AcSum > 0 ? grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month7 && t.StartDate.Value.Year == year7).Sum(s => s.TotalQuantity.Value) / (f.AcSum) : grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month7 && t.StartDate.Value.Year == year7).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStock8 += f.AcSum > 0 ? grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month8 && t.StartDate.Value.Year == year8).Sum(s => s.TotalQuantity.Value) / (f.AcSum) : grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month8 && t.StartDate.Value.Year == year8).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStock9 += f.AcSum > 0 ? grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month9 && t.StartDate.Value.Year == year9).Sum(s => s.TotalQuantity.Value) / (f.AcSum) : grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month9 && t.StartDate.Value.Year == year9).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStock10 += f.AcSum > 0 ? grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month10 && t.StartDate.Value.Year == year10).Sum(s => s.TotalQuantity.Value) / (f.AcSum) : grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month10 && t.StartDate.Value.Year == year10).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStock11 += f.AcSum > 0 ? grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month11 && t.StartDate.Value.Year == year11).Sum(s => s.TotalQuantity.Value) / (f.AcSum) : grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month11 && t.StartDate.Value.Year == year11).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStock12 += f.AcSum > 0 ? grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month12 && t.StartDate.Value.Year == year12).Sum(s => s.TotalQuantity.Value) / (f.AcSum) : grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month12 && t.StartDate.Value.Year == year12).Sum(s => s.TotalQuantity.Value);




                f.ProjectedHoldingStockQty1 = prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month1).Sum(s => s.Quantity);
                f.ProjectedHoldingStockQty2 = prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month2).Sum(s => s.Quantity);
                f.ProjectedHoldingStockQty3 = prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month3).Sum(s => s.Quantity);
                f.ProjectedHoldingStockQty4 = prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month4).Sum(s => s.Quantity);
                f.ProjectedHoldingStockQty5 = prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month5).Sum(s => s.Quantity);
                f.ProjectedHoldingStockQty6 = prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month6).Sum(s => s.Quantity);
                f.ProjectedHoldingStockQty7 = prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month7).Sum(s => s.Quantity);
                f.ProjectedHoldingStockQty8 = prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month8).Sum(s => s.Quantity);
                f.ProjectedHoldingStockQty9 = prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month9).Sum(s => s.Quantity);
                f.ProjectedHoldingStockQty10 = prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month10).Sum(s => s.Quantity);
                f.ProjectedHoldingStockQty11 = prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month11).Sum(s => s.Quantity);
                f.ProjectedHoldingStockQty12 = prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month12).Sum(s => s.Quantity);

                f.ProjectedHoldingStockQty1 += grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month1 && t.StartDate.Value.Year == year1).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStockQty2 += grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month2 && t.StartDate.Value.Year == year2).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStockQty3 += grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month3 && t.StartDate.Value.Year == year3).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStockQty4 += grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month4 && t.StartDate.Value.Year == year4).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStockQty5 += grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month5 && t.StartDate.Value.Year == year5).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStockQty6 += grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month6 && t.StartDate.Value.Year == year6).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStockQty7 += grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month7 && t.StartDate.Value.Year == year7).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStockQty8 += grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month8 && t.StartDate.Value.Year == year8).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStockQty9 += grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month9 && t.StartDate.Value.Year == year9).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStockQty10 += grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month10 && t.StartDate.Value.Year == year10).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStockQty11 += grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month11 && t.StartDate.Value.Year == year11).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStockQty12 += grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month12 && t.StartDate.Value.Year == year12).Sum(s => s.TotalQuantity.Value);


                f.OutputProjectedHoldingStockQty1 = 0;//prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month1).Sum(s => s.Quantity);
                f.OutputProjectedHoldingStockQty2 = 0;//prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month2).Sum(s => s.Quantity);
                f.OutputProjectedHoldingStockQty3 = 0;// prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month3).Sum(s => s.Quantity);
                f.OutputProjectedHoldingStockQty4 = 0;// prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month4).Sum(s => s.Quantity);
                f.OutputProjectedHoldingStockQty5 = 0;// prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month5).Sum(s => s.Quantity);
                f.OutputProjectedHoldingStockQty6 = 0;// prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month6).Sum(s => s.Quantity);
                f.OutputProjectedHoldingStockQty7 = 0;// prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month7).Sum(s => s.Quantity);
                f.OutputProjectedHoldingStockQty8 = 0;//prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month8).Sum(s => s.Quantity);
                f.OutputProjectedHoldingStockQty9 = 0;//prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month9).Sum(s => s.Quantity);
                f.OutputProjectedHoldingStockQty10 = 0;//prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month10).Sum(s => s.Quantity);
                f.OutputProjectedHoldingStockQty11 = 0;//prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month11).Sum(s => s.Quantity);
                f.OutputProjectedHoldingStockQty12 = 0;//prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month12).Sum(s => s.Quantity);



                f.TicketHoldingStock1 = f.ProductionTicket1 > 0 ? f.AcSum == 0 ? (f.ProductionTicket1) / 1 : (f.ProductionTicket1) / f.AcSum : 0;
                f.TicketHoldingStock2 = f.ProductionTicket2 > 0 ? f.AcSum == 0 ? (f.ProductionTicket2) / 1 : (f.ProductionTicket2) / f.AcSum : 0;
                f.TicketHoldingStock3 = f.ProductionTicket3 > 0 ? f.AcSum == 0 ? (f.ProductionTicket3) / 1 : (f.ProductionTicket3) / f.AcSum : 0;
                f.TicketHoldingStock4 = f.ProductionTicket4 > 0 ? f.AcSum == 0 ? (f.ProductionTicket4) / 1 : (f.ProductionTicket4) / f.AcSum : 0;
                f.TicketHoldingStock5 = f.ProductionTicket5 > 0 ? f.AcSum == 0 ? (f.ProductionTicket5) / 1 : (f.ProductionTicket5) / f.AcSum : 0;
                f.TicketHoldingStock6 = f.ProductionTicket6 > 0 ? f.AcSum == 0 ? (f.ProductionTicket6) / 1 : (f.ProductionTicket6) / f.AcSum : 0;
                f.TicketHoldingStock7 = f.ProductionTicket7 > 0 ? f.AcSum == 0 ? (f.ProductionTicket7) / 1 : (f.ProductionTicket7) / f.AcSum : 0;
                f.TicketHoldingStock8 = f.ProductionTicket8 > 0 ? f.AcSum == 0 ? (f.ProductionTicket8) / 1 : (f.ProductionTicket8) / f.AcSum : 0;
                f.TicketHoldingStock9 = f.ProductionTicket9 > 0 ? f.AcSum == 0 ? (f.ProductionTicket9) / 1 : (f.ProductionTicket9) / f.AcSum : 0;
                f.TicketHoldingStock10 = f.ProductionTicket10 > 0 ? f.AcSum == 0 ? (f.ProductionTicket10) / 1 : (f.ProductionTicket10) / f.AcSum : 0;
                f.TicketHoldingStock11 = f.ProductionTicket11 > 0 ? f.AcSum == 0 ? (f.ProductionTicket11) / 1 : (f.ProductionTicket11) / f.AcSum : 0;
                f.TicketHoldingStock12 = f.ProductionTicket12 > 0 ? f.AcSum == 0 ? (f.ProductionTicket12) / 1 : (f.ProductionTicket12) / f.AcSum : 0;




                f.QtyMonth1 = f.StockBalance - (f.AcSum / 2);
                f.QtyProductionProjected1 = (f.QtyMonth1 + f.ProjectedHoldingStockQty1 + f.OutputProjectedHoldingStockQty1) - f.BlanketAddhoc1;

                f.QtyMonth2 = f.QtyProductionProjected1 - (f.AcSum);
                f.QtyProductionProjected2 = (f.QtyMonth2 + f.ProjectedHoldingStockQty2 + f.OutputProjectedHoldingStockQty2) - f.BlanketAddhoc2;

                f.QtyMonth3 = f.QtyProductionProjected2 - (f.AcSum);
                f.QtyProductionProjected3 = (f.QtyMonth3 + f.ProjectedHoldingStockQty3 + f.OutputProjectedHoldingStockQty3) - f.BlanketAddhoc3;
                f.QtyMonth4 = f.QtyProductionProjected3 - (f.AcSum);
                f.QtyProductionProjected4 = (f.QtyMonth4 + f.ProjectedHoldingStockQty4 + f.OutputProjectedHoldingStockQty4) - f.BlanketAddhoc4;
                f.QtyMonth5 = f.QtyProductionProjected4 - (f.AcSum);
                f.QtyProductionProjected5 = (f.QtyMonth5 + f.ProjectedHoldingStockQty5 + f.OutputProjectedHoldingStockQty5) - f.BlanketAddhoc5;
                f.QtyMonth6 = f.QtyProductionProjected5 - (f.AcSum);
                f.QtyProductionProjected6 = (f.QtyMonth6 + f.ProjectedHoldingStockQty6 + f.OutputProjectedHoldingStockQty6) - f.BlanketAddhoc6;
                f.QtyMonth7 = f.QtyProductionProjected6 - (f.AcSum);
                f.QtyProductionProjected7 = (f.QtyMonth7 + f.ProjectedHoldingStockQty7 + f.OutputProjectedHoldingStockQty7) - f.BlanketAddhoc7;
                f.QtyMonth8 = f.QtyProductionProjected7 - (f.AcSum);
                f.QtyProductionProjected8 = (f.QtyMonth8 + f.ProjectedHoldingStockQty8 + f.OutputProjectedHoldingStockQty8) - f.BlanketAddhoc8;
                f.QtyMonth9 = f.QtyProductionProjected8 - (f.AcSum);
                f.QtyProductionProjected9 = (f.QtyMonth9 + f.ProjectedHoldingStockQty9 + f.OutputProjectedHoldingStockQty9) - f.BlanketAddhoc9;
                f.QtyMonth10 = f.QtyProductionProjected9 - (f.AcSum);
                f.QtyProductionProjected10 = (f.QtyMonth10 + f.ProjectedHoldingStockQty10 + f.OutputProjectedHoldingStockQty10) - f.BlanketAddhoc10;
                f.QtyMonth11 = f.QtyProductionProjected10 - (f.AcSum);
                f.QtyProductionProjected11 = (f.QtyMonth11 + f.ProjectedHoldingStockQty11 + f.OutputProjectedHoldingStockQty11) - f.BlanketAddhoc11;
                f.QtyMonth12 = f.QtyProductionProjected11 - (f.AcSum);
                f.QtyProductionProjected12 = (f.QtyMonth12 + f.ProjectedHoldingStockQty12 + f.OutputProjectedHoldingStockQty12) - f.BlanketAddhoc12;


                f.ProductionProjected1 = f.Month1 + f.ProjectedHoldingStock1 + 0 + f.OutputProjectedHoldingStock1;
                f.Month2 = f.AcSum > 0 ? f.ProductionProjected1 - decimal.Parse("1") > 0 ? f.ProductionProjected1 - decimal.Parse("1") : 0 : f.ProductionProjected1;
                f.ProductionProjected2 = f.Month2 + f.ProjectedHoldingStock2 + 0 + f.OutputProjectedHoldingStock2;
                f.Month3 = f.AcSum > 0 ? f.ProductionProjected2 - decimal.Parse("1") > 0 ? f.ProductionProjected2 - decimal.Parse("1") : 0 : f.ProductionProjected2;
                f.ProductionProjected3 = f.Month3 + f.ProjectedHoldingStock3 + 0 + f.OutputProjectedHoldingStock3;
                f.Month4 = f.AcSum > 0 ? f.ProductionProjected3 - decimal.Parse("1") > 0 ? f.ProductionProjected3 - decimal.Parse("1") : 0 : f.ProductionProjected3;
                f.ProductionProjected4 = f.Month4 + f.ProjectedHoldingStock4 + 0 + f.OutputProjectedHoldingStock4;
                f.Month5 = f.AcSum > 0 ? f.ProductionProjected4 - decimal.Parse("1") > 0 ? f.ProductionProjected4 - decimal.Parse("1") : 0 : f.ProductionProjected4;
                f.ProductionProjected5 = f.Month5 + f.ProjectedHoldingStock5 + 0 + f.OutputProjectedHoldingStock5;
                f.Month6 = f.AcSum > 0 ? f.ProductionProjected5 - decimal.Parse("1") > 0 ? f.ProductionProjected5 - decimal.Parse("1") : 0 : f.ProductionProjected5;
                f.ProductionProjected6 = f.Month6 + f.ProjectedHoldingStock6 + 0 + f.OutputProjectedHoldingStock6;
                f.Month7 = f.AcSum > 0 ? f.ProductionProjected6 - decimal.Parse("1") > 0 ? f.ProductionProjected6 - decimal.Parse("1") : 0 : f.ProductionProjected6;
                f.ProductionProjected7 = f.Month7 + f.ProjectedHoldingStock7 + 0 + f.OutputProjectedHoldingStock7;
                f.Month8 = f.AcSum > 0 ? f.ProductionProjected7 - decimal.Parse("1") > 0 ? f.ProductionProjected7 - decimal.Parse("1") : 0 : f.ProductionProjected7;
                f.ProductionProjected8 = f.Month8 + f.ProjectedHoldingStock8 + 0 + f.OutputProjectedHoldingStock8;
                f.Month9 = f.AcSum > 0 ? f.ProductionProjected8 - decimal.Parse("1") > 0 ? f.ProductionProjected8 - decimal.Parse("1") : 0 : f.ProductionProjected8;
                f.ProductionProjected9 = f.Month9 + f.ProjectedHoldingStock9 + 0 + f.OutputProjectedHoldingStock9;
                f.Month10 = f.AcSum > 0 ? f.ProductionProjected9 - decimal.Parse("1") > 0 ? f.ProductionProjected9 - decimal.Parse("1") : 0 : f.ProductionProjected9;
                f.ProductionProjected10 = f.Month10 + f.ProjectedHoldingStock10 + 0 + f.OutputProjectedHoldingStock10;
                f.Month11 = f.AcSum > 0 ? f.ProductionProjected10 - decimal.Parse("1") > 0 ? f.ProductionProjected10 - decimal.Parse("1") : 0 : f.ProductionProjected10;
                f.ProductionProjected11 = f.Month11 + f.ProjectedHoldingStock11 + 0 + f.OutputProjectedHoldingStock11;
                f.Month12 = f.AcSum > 0 ? f.ProductionProjected11 - decimal.Parse("1") > 0 ? f.ProductionProjected11 - decimal.Parse("1") : 0 : f.ProductionProjected11;
                f.ProductionProjected12 = f.Month12 + f.ProjectedHoldingStock12 + 0 + f.OutputProjectedHoldingStock12;

                if (f.IsTenderExist1 || f.AcSum <= 0)
                {
                    f.Month1 = f.QtyMonth1;
                    f.ProductionProjected1 = f.QtyProductionProjected1;
                    f.ProjectedHoldingStock1 = f.ProjectedHoldingStockQty1;
                    f.OutputProjectedHoldingStock1 = f.OutputProjectedHoldingStockQty1;
                }
                else
                {
                    if (f.isTenderExist)
                    {
                        f.Month1 = f.QtyMonth1 / f.AcSum;
                        f.ProductionProjected1 = f.QtyProductionProjected1 / f.AcSum;
                    }
                }
                if (f.IsTenderExist2 || f.AcSum <= 0)
                {
                    f.Month2 = f.QtyMonth2;
                    f.ProductionProjected2 = f.QtyProductionProjected2;
                    f.ProjectedHoldingStock2 = f.ProjectedHoldingStockQty2;
                    f.OutputProjectedHoldingStock2 = f.OutputProjectedHoldingStockQty2;
                }
                else
                {
                    if (f.isTenderExist)
                    {
                        f.Month2 = f.QtyMonth2 / f.AcSum;
                        f.ProductionProjected2 = f.QtyProductionProjected2 / f.AcSum;
                    }
                }

                if (f.IsTenderExist3 || f.AcSum <= 0)
                {
                    f.Month3 = f.QtyMonth3;
                    f.ProductionProjected3 = f.QtyProductionProjected3;
                    f.ProjectedHoldingStock3 = f.ProjectedHoldingStockQty3;
                    f.OutputProjectedHoldingStock3 = f.OutputProjectedHoldingStockQty3;
                }
                else
                {
                    if (f.isTenderExist)
                    {
                        f.Month3 = f.QtyMonth3 / f.AcSum;
                        f.ProductionProjected3 = f.QtyProductionProjected3 / f.AcSum;
                    }
                }
                if (f.IsTenderExist4 || f.AcSum <= 0)
                {
                    f.Month4 = f.QtyMonth4;
                    f.ProductionProjected4 = f.QtyProductionProjected4;
                    f.ProjectedHoldingStock4 = f.ProjectedHoldingStockQty4;
                    f.OutputProjectedHoldingStock4 = f.OutputProjectedHoldingStockQty4;
                }
                else
                {
                    if (f.isTenderExist)
                    {
                        f.Month4 = f.QtyMonth4 / f.AcSum;
                        f.ProductionProjected4 = f.QtyProductionProjected4 / f.AcSum;
                    }
                }
                if (f.IsTenderExist5 || f.AcSum <= 0)
                {
                    f.Month5 = f.QtyMonth5;
                    f.ProductionProjected5 = f.QtyProductionProjected5;
                    f.ProjectedHoldingStock5 = f.ProjectedHoldingStockQty5;
                    f.OutputProjectedHoldingStock5 = f.OutputProjectedHoldingStockQty5;
                }
                else
                {
                    if (f.isTenderExist)
                    {
                        f.Month5 = f.QtyMonth5 / f.AcSum;
                        f.ProductionProjected5 = f.QtyProductionProjected5 / f.AcSum;
                    }
                }
                if (f.IsTenderExist6 || f.AcSum <= 0)
                {
                    f.Month6 = f.QtyMonth6;
                    f.ProductionProjected6 = f.QtyProductionProjected6;
                    f.ProjectedHoldingStock6 = f.ProjectedHoldingStockQty6;
                    f.OutputProjectedHoldingStock6 = f.OutputProjectedHoldingStockQty6;
                }
                else
                {
                    if (f.isTenderExist)
                    {
                        f.Month6 = f.QtyMonth6 / f.AcSum;
                        f.ProductionProjected6 = f.QtyProductionProjected6 / f.AcSum;
                    }
                }
                if (f.IsTenderExist7 || f.AcSum <= 0)
                {
                    f.Month7 = f.QtyMonth7;
                    f.ProductionProjected7 = f.QtyProductionProjected7;
                    f.ProjectedHoldingStock7 = f.ProjectedHoldingStockQty7;
                    f.OutputProjectedHoldingStock7 = f.OutputProjectedHoldingStockQty7;
                }
                else
                {
                    if (f.isTenderExist)
                    {
                        f.Month7 = f.QtyMonth7 / f.AcSum;
                        f.ProductionProjected7 = f.QtyProductionProjected7 / f.AcSum;
                    }
                }
                if (f.IsTenderExist8 || f.AcSum <= 0)
                {
                    f.Month8 = f.QtyMonth8;
                    f.ProductionProjected8 = f.QtyProductionProjected8;
                    f.ProjectedHoldingStock8 = f.ProjectedHoldingStockQty8;
                    f.OutputProjectedHoldingStock8 = f.OutputProjectedHoldingStockQty8;
                }
                else
                {
                    if (f.isTenderExist)
                    {
                        f.Month8 = f.QtyMonth8 / f.AcSum;
                        f.ProductionProjected8 = f.QtyProductionProjected8 / f.AcSum;
                    }
                }
                if (f.IsTenderExist9 || f.AcSum <= 0)
                {
                    f.Month9 = f.QtyMonth9;
                    f.ProductionProjected9 = f.QtyProductionProjected9;
                    f.ProjectedHoldingStock9 = f.ProjectedHoldingStockQty9;
                    f.OutputProjectedHoldingStock9 = f.OutputProjectedHoldingStockQty9;
                }
                else
                {
                    if (f.isTenderExist)
                    {
                        f.Month9 = f.QtyMonth9 / f.AcSum;
                        f.ProductionProjected9 = f.QtyProductionProjected9 / f.AcSum;
                    }
                }
                if (f.IsTenderExist10 || f.AcSum <= 0)
                {
                    f.Month10 = f.QtyMonth10;
                    f.ProductionProjected10 = f.QtyProductionProjected10;
                    f.ProjectedHoldingStock10 = f.ProjectedHoldingStockQty10;
                    f.OutputProjectedHoldingStock10 = f.OutputProjectedHoldingStockQty10;
                }
                else
                {
                    if (f.isTenderExist)
                    {
                        f.Month10 = f.QtyMonth10 / f.AcSum;
                        f.ProductionProjected10 = f.QtyProductionProjected10 / f.AcSum;
                    }
                }
                if (f.IsTenderExist11 || f.AcSum <= 0)
                {
                    f.Month11 = f.QtyMonth11;
                    f.ProductionProjected11 = f.QtyProductionProjected11;
                    f.ProjectedHoldingStock11 = f.ProjectedHoldingStockQty11;
                    f.OutputProjectedHoldingStock11 = f.OutputProjectedHoldingStockQty11;
                }
                else
                {
                    if (f.isTenderExist)
                    {
                        f.Month11 = f.QtyMonth11 / f.AcSum;
                        f.ProductionProjected11 = f.QtyProductionProjected11 / f.AcSum;
                    }
                }
                if (f.IsTenderExist12 || f.AcSum <= 0)
                {
                    f.Month12 = f.QtyMonth12;
                    f.ProductionProjected12 = f.QtyProductionProjected12;
                    f.ProjectedHoldingStock12 = f.ProjectedHoldingStockQty12;
                    f.OutputProjectedHoldingStock12 = f.OutputProjectedHoldingStockQty12;
                }
                else
                {
                    if (f.isTenderExist)
                    {
                        f.Month12 = f.QtyMonth12 / f.AcSum;
                        f.ProductionProjected12 = f.QtyProductionProjected12 / f.AcSum;
                    }
                }

            });

            genericId = new List<long?>();
            //var customer = new List<string>();
            itemMasterforReport.ForEach(ac =>
            {
                var customer = new List<string>();
                if (ac.No == "FP-PP-CRM-100")
                {

                }
                if (!genericId.Exists(g => g == ac.GenericCodeId) && ac.GenericCodeId != null)
                {
                    genericId.Add(ac.GenericCodeId);
                    var itemNos = itemMasterforReport.Where(i => i.GenericCodeId == ac.GenericCodeId).Select(s => s.ItemId).ToList();
                    blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1))).ToList().ForEach(adhoc =>
                    {
                        if (!customer.Exists(c => c == adhoc.Categories) && !string.IsNullOrEmpty(adhoc.Categories))
                        {
                            customer.Add(adhoc.Categories);
                            MethodCodeList.Add(new INPCalendarPivotModel
                            {
                                ItemId = ac.ItemId,
                                ItemNo = ac.GenericCodeDescription2,
                                IsSteroid = ac.Steroid.GetValueOrDefault(false),
                                SalesCategoryId = ac.CategoryId,
                                LocationID = categoryItems != null && categoryItems.FirstOrDefault(a => a.SaleCategoryId == ac.CategoryId) != null ? categoryItems.FirstOrDefault(a => a.SaleCategoryId == ac.CategoryId).LocationId : null,
                                SalesCategory = categoryItems != null && categoryItems.FirstOrDefault(a => a.SaleCategoryId == ac.CategoryId) != null ? categoryItems.FirstOrDefault(a => a.SaleCategoryId == ac.CategoryId).Code : "",
                                MethodCodeId = ac.NavMethodCodeLines.Count > 0 ? ac.NavMethodCodeLines.First().MethodCodeId : -1,
                                GenericCodeID = ac.GenericCodeId,
                                AddhocCust = adhoc.Categories,
                                Description = ac.Description + " " + ac.Description2,
                                MethodCode = ac.NavMethodCodeLines.Count > 0 ? ac.NavMethodCodeLines.First().MethodName : "No MethodCode",
                                Month = endDate.StockMonth.ToString("MMMM"),
                                Remarks = "Tender",
                                PackSize = ac.PackSize.HasValue ? ac.PackSize.Value : 900,
                                PackSize2 = packSize2,
                                BlanketAddhoc1 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.Categories == adhoc.Categories && t.ShipmentDate.Value.Month == month1 && t.ShipmentDate.Value.Year == year1).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                                BlanketAddhoc2 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.Categories == adhoc.Categories && t.ShipmentDate.Value.Month == month2 && t.ShipmentDate.Value.Year == year2).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                                BlanketAddhoc3 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.Categories == adhoc.Categories && t.ShipmentDate.Value.Month == month3 && t.ShipmentDate.Value.Year == year3).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                                BlanketAddhoc4 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.Categories == adhoc.Categories && t.ShipmentDate.Value.Month == month4 && t.ShipmentDate.Value.Year == year4).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                                BlanketAddhoc5 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.Categories == adhoc.Categories && t.ShipmentDate.Value.Month == month5 && t.ShipmentDate.Value.Year == year5).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                                BlanketAddhoc6 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.Categories == adhoc.Categories && t.ShipmentDate.Value.Month == month6 && t.ShipmentDate.Value.Year == year6).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                                BlanketAddhoc7 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.Categories == adhoc.Categories && t.ShipmentDate.Value.Month == month7 && t.ShipmentDate.Value.Year == year7).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                                BlanketAddhoc8 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.Categories == adhoc.Categories && t.ShipmentDate.Value.Month == month8 && t.ShipmentDate.Value.Year == year8).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                                BlanketAddhoc9 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.Categories == adhoc.Categories && t.ShipmentDate.Value.Month == month9 && t.ShipmentDate.Value.Year == year9).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                                BlanketAddhoc10 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.Categories == adhoc.Categories && t.ShipmentDate.Value.Month == month10 && t.ShipmentDate.Value.Year == year10).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                                BlanketAddhoc11 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.Categories == adhoc.Categories && t.ShipmentDate.Value.Month == month11 && t.ShipmentDate.Value.Year == year11).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                                BlanketAddhoc12 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.Categories == adhoc.Categories && t.ShipmentDate.Value.Month == month12 && t.ShipmentDate.Value.Year == year12).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),


                            });
                        }
                    });
                }

            });

            var groupedResult = MethodCodeList.GroupBy(g => g.MethodCode).ToList();
            var resultData = new List<INPCalendarPivotModel>();
            groupedResult.ForEach(f =>
            {
                if (f.Key.Contains("Paratab Tablet BMP1 (SG)"))
                {

                }
                f.ToList().ForEach(g =>
                {
                    if (g.SymlQty > 0 && g.Remarks == "AC")
                    {
                        resultData.Add(g);
                    }
                    else
                    {
                        if (g.Remarks == "AC")
                        {
                            var tenderExist = f.Any(t => t.Remarks == "Tender" && t.ItemNo == g.ItemNo && (t.BlanketAddhoc1 > 0 || t.BlanketAddhoc2 > 0 || t.BlanketAddhoc3 > 0 || t.BlanketAddhoc4 > 0 || t.BlanketAddhoc5 > 0
                            || t.BlanketAddhoc6 > 0 || t.BlanketAddhoc7 > 0 || t.BlanketAddhoc8 > 0 || t.BlanketAddhoc9 > 0 || t.BlanketAddhoc10 > 0 || t.BlanketAddhoc11 > 0 || t.BlanketAddhoc12 > 0));
                            if (tenderExist)
                            {
                                resultData.Add(g);
                            }
                            else
                            {
                                if (g.ProjectedHoldingStock1 > 0 || g.ProjectedHoldingStock2 > 0 || g.ProjectedHoldingStock3 > 0 || g.ProjectedHoldingStock4 > 0 || g.ProjectedHoldingStock5 > 0
                      || g.ProjectedHoldingStock6 > 0 || g.ProjectedHoldingStock7 > 0 || g.ProjectedHoldingStock8 > 0 || g.ProjectedHoldingStock9 > 0 || g.ProjectedHoldingStock10 > 0 || g.ProjectedHoldingStock11 > 0 || g.ProjectedHoldingStock12 > 0)
                                {
                                    resultData.Add(g);
                                }
                            }
                        }
                        else
                        {
                            if (g.BlanketAddhoc1 > 0 || g.BlanketAddhoc2 > 0 || g.BlanketAddhoc3 > 0 || g.BlanketAddhoc4 > 0 || g.BlanketAddhoc5 > 0
                           || g.BlanketAddhoc6 > 0 || g.BlanketAddhoc7 > 0 || g.BlanketAddhoc8 > 0 || g.BlanketAddhoc9 > 0 || g.BlanketAddhoc10 > 0 || g.BlanketAddhoc11 > 0 || g.BlanketAddhoc12 > 0)
                            {
                                resultData.Add(g);
                            }
                        }
                    }
                });

            });

            return resultData.ToList();
        }
        private List<DropDownOptionsModel> salesCatList()
        {
            List<DropDownOptionsModel> _salesCategoryItems = new List<DropDownOptionsModel>();
            DropDownOptionsModel dropDownOptionsModel1 = new DropDownOptionsModel();
            dropDownOptionsModel1.Text = "CAP";
            dropDownOptionsModel1.Id = 1;
            _salesCategoryItems.Add(dropDownOptionsModel1);
            DropDownOptionsModel dropDownOptionsModel2 = new DropDownOptionsModel();
            dropDownOptionsModel2.Text = "CREAM";
            dropDownOptionsModel2.Id = 2;
            _salesCategoryItems.Add(dropDownOptionsModel2);

            DropDownOptionsModel dropDownOptionsModel3 = new DropDownOptionsModel();
            dropDownOptionsModel3.Text = "DD";
            dropDownOptionsModel3.Id = 3;
            _salesCategoryItems.Add(dropDownOptionsModel3);
            DropDownOptionsModel dropDownOptionsModel4 = new DropDownOptionsModel();
            dropDownOptionsModel4.Text = "SYRUP";
            dropDownOptionsModel4.Id = 4;
            _salesCategoryItems.Add(dropDownOptionsModel4);
            DropDownOptionsModel dropDownOptionsModel5 = new DropDownOptionsModel();
            dropDownOptionsModel5.Text = "TABLET";
            dropDownOptionsModel5.Id = 5;
            _salesCategoryItems.Add(dropDownOptionsModel5);
            DropDownOptionsModel dropDownOptionsModel6 = new DropDownOptionsModel();
            dropDownOptionsModel6.Text = "VET";
            dropDownOptionsModel6.Id = 6;
            _salesCategoryItems.Add(dropDownOptionsModel6);
            DropDownOptionsModel dropDownOptionsModel7 = new DropDownOptionsModel();
            dropDownOptionsModel7.Text = "POWDER";
            dropDownOptionsModel7.Id = 7;
            _salesCategoryItems.Add(dropDownOptionsModel7);
            DropDownOptionsModel dropDownOptionsModel8 = new DropDownOptionsModel();
            dropDownOptionsModel8.Text = "INJ";
            dropDownOptionsModel8.Id = 8;
            _salesCategoryItems.Add(dropDownOptionsModel8);
            return _salesCategoryItems;
        }
        public async Task<IReadOnlyList<INPCalendarPivotModel>> GetSimulationAddhocV3(DateRangeModel dateRangeModel)
        {
            var companyIds = new List<long?> { dateRangeModel.CompanyId };
            if (dateRangeModel.CompanyId == 3)
            {
                companyIds = new List<long?> { 1, 2 };
            }

            var methodCodeRecipes = new List<NAVRecipesModel>();

            List<NavmethodCodeBatch> methodCodeRecipe = new List<NavmethodCodeBatch>();
            List<ApplicationMasterDetail> applicationDetails = new List<ApplicationMasterDetail>();
            List<NAVRecipesModel> recipeList = new List<NAVRecipesModel>();
            using (var connection = CreateConnection())
            {
                var query = "SELECT * FROM NavmethodCodeBatch;";
                query += "select t1.* from ApplicationMasterDetail t1 JOIN ApplicationMaster t2 ON t1.ApplicationMasterID=t2.ApplicationMasterID where t2.ApplicationMasterCodeID=175;";
                query += "select RecipeNo,ItemNo,Description,BatchSize,ItemRecipeId,CONCAT(RecipeNo,'|',BatchSize) as RecipeName from Navrecipes Where CompanyId  in(" + string.Join(',', companyIds) + ") AND Status='Certified';";
                var results = await connection.QueryMultipleAsync(query);
                methodCodeRecipe = results.Read<NavmethodCodeBatch>().ToList();
                applicationDetails = results.Read<ApplicationMasterDetail>().ToList();
                recipeList = results.Read<NAVRecipesModel>().ToList();
            }

            methodCodeRecipe.ForEach(f =>
            {
                var BatchSize = applicationDetails.FirstOrDefault(b => b.ApplicationMasterDetailId == f.BatchSize)?.Value;
                var DefaultBatchSize = applicationDetails.FirstOrDefault(b => b.ApplicationMasterDetailId == f.DefaultBatchSize)?.Value;
                methodCodeRecipes.Add(new NAVRecipesModel
                {
                    RecipeNo = BatchSize,
                    BatchSize = BatchSize,// f.BatchUnitSize.GetValueOrDefault(0).ToString(),
                    ItemRecipeId = f.NavMethodCodeId,
                    UnitQTY = f.BatchUnitSize.GetValueOrDefault(0),
                    ItemNo = DefaultBatchSize,
                    DefaultBatch = DefaultBatchSize,
                    RecipeName = BatchSize,
                }); ;

            });


            var acreports = new List<INPCalendarPivotModel>();

            if (acreports.Count == 0)
            {
                acreports = await SimulationAddhocV33(dateRangeModel, methodCodeRecipes, recipeList);
            }
            if (dateRangeModel.MethodCodeId > 0)
                acreports = acreports.Where(r => r.MethodCodeId == dateRangeModel.MethodCodeId).ToList();
            if (dateRangeModel.SalesCategoryId > 0)
            {
                acreports = acreports.Where(r => r.SalesCategoryId == dateRangeModel.SalesCategoryId).ToList();
            }
            if (dateRangeModel.IsSteroid.HasValue)
            {
                acreports = acreports.Where(r => r.IsSteroid == dateRangeModel.IsSteroid.Value).ToList();
            }
            if (!string.IsNullOrEmpty(dateRangeModel.Replenishment))
            {
                acreports = acreports.Where(r => r.Replenishment.Contains(dateRangeModel.Replenishment)).ToList();
            }

            if (dateRangeModel.Ticketformula.GetValueOrDefault(0) > 0)
            {
                var ticketCondition = dateRangeModel.Ticketformula.GetValueOrDefault(0);
                var ticketValue = dateRangeModel.Ticketvalue.GetValueOrDefault(0);
                if (ticketCondition == 1)
                {
                    acreports = acreports.Where(r => r.Month1 == ticketValue || r.Month1 == ticketValue || r.Month2 == ticketValue || r.Month3 == ticketValue || r.Month4 == ticketValue || r.Month5 == ticketValue || r.Month6 == ticketValue).ToList();
                }
                else if (ticketCondition == 2)
                {
                    acreports = acreports.Where(r => r.Month1 > ticketValue || r.Month1 > ticketValue || r.Month2 > ticketValue || r.Month3 > ticketValue || r.Month4 > ticketValue || r.Month5 > ticketValue || r.Month6 > ticketValue).ToList();
                }
                else if (ticketCondition == 3)
                {
                    acreports = acreports.Where(r => r.Month1 >= ticketValue || r.Month1 >= ticketValue || r.Month2 >= ticketValue || r.Month3 >= ticketValue || r.Month4 >= ticketValue || r.Month5 >= ticketValue || r.Month6 >= ticketValue).ToList();
                }
                else if (ticketCondition == 4)
                {
                    acreports = acreports.Where(r => r.Month1 < ticketValue || r.Month1 < ticketValue || r.Month2 < ticketValue || r.Month3 < ticketValue || r.Month4 < ticketValue || r.Month5 < ticketValue || r.Month6 < ticketValue).ToList();
                }
                else if (ticketCondition == 5)
                {
                    acreports = acreports.Where(r => r.Month1 <= ticketValue || r.Month1 <= ticketValue || r.Month2 <= ticketValue || r.Month3 <= ticketValue || r.Month4 <= ticketValue || r.Month5 <= ticketValue || r.Month6 <= ticketValue).ToList();
                }
                else
                {
                    acreports = acreports.Where(r => r.Month1 != ticketValue || r.Month1 != ticketValue || r.Month2 != ticketValue || r.Month3 != ticketValue || r.Month4 != ticketValue || r.Month5 != ticketValue || r.Month6 != ticketValue).ToList();
                }
            }

            var packSize2 = 90000;
            if (!string.IsNullOrEmpty(dateRangeModel.Receipe))
            {
                string numberOnly = Regex.Replace(dateRangeModel.Receipe.Split("|")[0], "[^0-9.]", "");
                packSize2 = int.Parse(numberOnly) * 1000;

                acreports.Where(m => m.MethodCodeId == dateRangeModel.ChangeMethodCodeId).ToList().ForEach(f =>
                {
                    f.PackSize2 = packSize2;
                    f.ProdRecipe = dateRangeModel.Receipe.Split("|")[0];
                });

            }

            if (dateRangeModel.IsUpdate)
            {
                acreports.Where(m => m.ItemNo == dateRangeModel.ItemNo).ToList().ForEach(f =>
                {
                    f.Roundup2 = dateRangeModel.Roundup2;
                    f.Remarks = dateRangeModel.Remarks;
                });
            }
            if (acreports != null && acreports.Count() > 0)
            {
                acreports.ForEach(s =>
                {
                    s.IsNonSteroids = s.IsSteroid == false ? "Non Steroid" : "";
                    s.IsSteroids = s.IsSteroid == true ? "Steroid" : "Non Steroid";
                    s.ApexQty = s.ApexQty == 0 ? null : s.ApexQty;
                    s.AntahQty = s.AntahQty == 0 ? null : s.AntahQty;
                    s.MissQty = s.MissQty == 0 ? null : s.MissQty;
                    s.PxQty = s.PxQty == 0 ? null : s.PxQty;
                    s.DeliverynotReceived = s.DeliverynotReceived == 0 ? null : s.DeliverynotReceived;
                    s.SymlQty = s.SymlQty == 0 ? null : s.SymlQty;
                    s.Rework_ = s.Rework == 0 ? null : s.Rework;
                    s.AcSum_ = s.AcSum == 0 ? null : s.AcSum;
                    s.ThreeMonthACQty_ = s.ThreeMonthACQty == 0 ? null : s.ThreeMonthACQty;
                    s.Roundup1_ = s.Roundup1 == 0 ? null : s.Roundup1;
                    s.Roundup2_ = s.Roundup2 == 0 ? null : s.Roundup2;
                    s.PreApexStockBalance_ = s.PreApexStockBalance == 0 ? null : s.PreApexStockBalance;
                    s.PreAntahStockBalance_ = s.PreAntahStockBalance == 0 ? null : s.PreAntahStockBalance;
                    s.PreMsbStockBalance_ = s.PreMsbStockBalance == 0 ? null : s.PreMsbStockBalance;
                    s.PrePsbStockBalance_ = s.PrePsbStockBalance == 0 ? null : s.PrePsbStockBalance;
                    s.PreSgTenderStockBalance_ = s.PreSgTenderStockBalance == 0 ? null : s.PreSgTenderStockBalance;
                    s.WipQty_ = s.WipQty == 0 ? null : s.WipQty;
                    s.NotStartInvQty_ = s.NotStartInvQty == 0 ? null : s.NotStartInvQty;
                    s.PreMyStockBalance_ = s.PreMyStockBalance == 0 ? null : s.PreMyStockBalance;
                    s.PreOtherStoreQty_ = s.PreOtherStoreQty == 0 ? null : s.PreOtherStoreQty;
                    s.PrewipQty_ = s.PrewipQty == 0 ? null : s.PrewipQty;
                    s.PreStockBalance_ = s.PreStockBalance == 0 ? null : s.PreStockBalance;
                    s.PreStockHoldingBalance_ = s.PreStockHoldingBalance == 0 ? null : s.PreStockHoldingBalance;
                    s.ApexStockBalance_ = s.ApexStockBalance == 0 ? null : s.ApexStockBalance;
                    s.AntahStockBalance_ = s.AntahStockBalance == 0 ? null : s.AntahStockBalance;
                    s.MsbStockBalance_ = s.MsbStockBalance == 0 ? null : s.MsbStockBalance;
                    s.PsbStockBalance_ = s.PsbStockBalance == 0 ? null : s.PsbStockBalance;
                    s.SgTenderStockBalance_ = s.SgTenderStockBalance == 0 ? null : s.SgTenderStockBalance;
                    s.MyStockBalance_ = s.MyStockBalance == 0 ? null : s.MyStockBalance;
                    s.OtherStoreQty_ = s.OtherStoreQty == 0 ? null : s.OtherStoreQty;
                    s.InterCompanyTransitQty_ = s.InterCompanyTransitQty == 0 ? null : s.InterCompanyTransitQty;
                    s.StockBalance_ = s.StockBalance == 0 ? null : s.StockBalance;
                    s.StockHoldingBalance_ = s.StockHoldingBalance == 0 ? null : s.StockHoldingBalance;
                    s.BlanketAddhoc1_ = s.BlanketAddhoc1 == 0 ? null : s.BlanketAddhoc1;
                    s.Month1_ = s.Month1 == 0 ? null : s.Month1;
                    s.ProjectedHoldingStock1_ = s.ProjectedHoldingStock1 == 0 ? null : s.ProjectedHoldingStock1;
                    //s.ProductionProjected1_ = s.ProductionProjected1 == 0 ? null : s.ProductionProjected1;
                    s.ProductionProjected1_ = (s.Month1 + s.ProjectedHoldingStock1) - s.BlanketAddhoc1;
                    s.BlanketAddhoc2_ = s.BlanketAddhoc2 == 0 ? null : s.BlanketAddhoc2;
                    s.Month2_ = s.Month2 == 0 ? null : s.Month2;
                    s.ProjectedHoldingStock2_ = s.ProjectedHoldingStock2 == 0 ? null : s.ProjectedHoldingStock2;
                    s.ProductionProjected2_ = (s.Month2 + s.ProjectedHoldingStock2) - s.BlanketAddhoc2;
                    //s.ProductionProjected2_ = s.ProductionProjected2 == 0 ? null : s.ProductionProjected2;
                    s.BlanketAddhoc3_ = s.BlanketAddhoc3 == 0 ? null : s.BlanketAddhoc3;
                    s.Month3_ = s.Month3 == 0 ? null : s.Month3;
                    s.ProjectedHoldingStock3_ = s.ProjectedHoldingStock3 == 0 ? null : s.ProjectedHoldingStock3;
                    s.ProductionProjected3_ = (s.Month3 + s.ProjectedHoldingStock3) - s.BlanketAddhoc3;
                    //s.ProductionProjected3_ = s.ProductionProjected3 == 0 ? null : s.ProductionProjected3;
                    s.BlanketAddhoc4_ = s.BlanketAddhoc4 == 0 ? null : s.BlanketAddhoc4;
                    s.Month4_ = s.Month4 == 0 ? null : s.Month4;
                    s.ProjectedHoldingStock4_ = s.ProjectedHoldingStock4 == 0 ? null : s.ProjectedHoldingStock4;
                    s.ProductionProjected4_ = (s.Month4 + s.ProjectedHoldingStock4) - s.BlanketAddhoc4;
                    //s.ProductionProjected4_ = s.ProductionProjected4 == 0 ? null : s.ProductionProjected4;
                    s.BlanketAddhoc5_ = s.BlanketAddhoc5 == 0 ? null : s.BlanketAddhoc5;
                    s.Month5_ = s.Month5 == 0 ? null : s.Month5;
                    s.ProjectedHoldingStock5_ = s.ProjectedHoldingStock5 == 0 ? null : s.ProjectedHoldingStock5;
                    s.ProductionProjected5_ = (s.Month5 + s.ProjectedHoldingStock5) - s.BlanketAddhoc5;
                    //s.ProductionProjected5_ = s.ProductionProjected5 == 0 ? null : s.ProductionProjected5;
                    s.BlanketAddhoc6_ = s.BlanketAddhoc6 == 0 ? null : s.BlanketAddhoc6;
                    s.Month6_ = s.Month6 == 0 ? null : s.Month6;
                    s.ProjectedHoldingStock6_ = s.ProjectedHoldingStock6 == 0 ? null : s.ProjectedHoldingStock6;
                    s.ProductionProjected6_ = (s.Month6 + s.ProjectedHoldingStock6) - s.BlanketAddhoc6;
                    //s.ProductionProjected6_ = s.ProductionProjected6 == 0 ? null : s.ProductionProjected6;
                    s.BlanketAddhoc7_ = s.BlanketAddhoc7 == 0 ? null : s.BlanketAddhoc7;
                    s.Month7_ = s.Month7 == 0 ? null : s.Month7;
                    s.ProjectedHoldingStock7_ = s.ProjectedHoldingStock7 == 0 ? null : s.ProjectedHoldingStock7;
                    //s.ProductionProjected7_ = s.ProductionProjected7 == 0 ? null : s.ProductionProjected7;
                    s.ProductionProjected7_ = (s.Month7 + s.ProjectedHoldingStock7) - s.BlanketAddhoc7;
                    s.BlanketAddhoc8_ = s.BlanketAddhoc8 == 0 ? null : s.BlanketAddhoc8;
                    s.Month8_ = s.Month8 == 0 ? null : s.Month8;
                    s.ProjectedHoldingStock8_ = s.ProjectedHoldingStock8 == 0 ? null : s.ProjectedHoldingStock8;
                    //s.ProductionProjected8_ = s.ProductionProjected8 == 0 ? null : s.ProductionProjected8;
                    s.ProductionProjected8_ = (s.Month8 + s.ProjectedHoldingStock8) - s.BlanketAddhoc8;
                    s.BlanketAddhoc9_ = s.BlanketAddhoc9 == 0 ? null : s.BlanketAddhoc9;
                    s.Month9_ = s.Month9 == 0 ? null : s.Month9;
                    s.ProjectedHoldingStock9_ = s.ProjectedHoldingStock9 == 0 ? null : s.ProjectedHoldingStock9;
                    //s.ProductionProjected9_ = s.ProductionProjected9 == 0 ? null : s.ProductionProjected9;
                    s.ProductionProjected9_ = (s.Month9 + s.ProjectedHoldingStock9) - s.BlanketAddhoc9;
                    s.BlanketAddhoc10_ = s.BlanketAddhoc10 == 0 ? null : s.BlanketAddhoc10;
                    s.Month10_ = s.Month10 == 0 ? null : s.Month10;
                    s.ProjectedHoldingStock10_ = s.ProjectedHoldingStock10 == 0 ? null : s.ProjectedHoldingStock10;
                    //s.ProductionProjected10_ = s.ProductionProjected10 == 0 ? null : s.ProductionProjected10;
                    s.ProductionProjected10_ = (s.Month10 + s.ProjectedHoldingStock10) - s.BlanketAddhoc10;
                    s.BlanketAddhoc11_ = s.BlanketAddhoc11 == 0 ? null : s.BlanketAddhoc11;
                    s.Month11_ = s.Month11 == 0 ? null : s.Month11;
                    s.ProjectedHoldingStock11_ = s.ProjectedHoldingStock11 == 0 ? null : s.ProjectedHoldingStock11;
                    //s.ProductionProjected11_ = s.ProductionProjected11 == 0 ? null : s.ProductionProjected11;
                    s.ProductionProjected11_ = (s.Month11 + s.ProjectedHoldingStock11) - s.BlanketAddhoc11;
                    s.BlanketAddhoc12_ = s.BlanketAddhoc12 == 0 ? null : s.BlanketAddhoc12;
                    s.Month12_ = s.Month12 == 0 ? null : s.Month12;
                    s.ProjectedHoldingStock12_ = s.ProjectedHoldingStock12 == 0 ? null : s.ProjectedHoldingStock12;
                    //s.ProductionProjected12_ = s.ProductionProjected12 == 0 ? null : s.ProductionProjected12;
                    s.ProductionProjected12_ = (s.Month12 + s.ProjectedHoldingStock12) - s.BlanketAddhoc12;
                    s.NoOfTicket1_ = s.NoOfTicket1;
                    s.NoOfTicket2_ = s.NoOfTicket2;
                    s.NoOfTicket3_ = s.NoOfTicket3;
                    s.NoOfTicket4_ = s.NoOfTicket4;
                    s.NoOfTicket5_ = s.NoOfTicket5;
                    s.NoOfTicket6_ = s.NoOfTicket6; s.NoOfTicket7_ = s.NoOfTicket7; s.NoOfTicket8_ = s.NoOfTicket8;
                    s.NoOfTicket9_ = s.NoOfTicket9; s.NoOfTicket10_ = s.NoOfTicket10; s.NoOfTicket11_ = s.NoOfTicket11; s.NoOfTicket12_ = s.NoOfTicket12;
                    var GroupTicket1 = s.GroupItemTicket1 + "," + s.GroupTicket1;
                    if (GroupTicket1 != null)
                    {
                        var tic1 = GroupTicket1.Split(",").ToList().Where(w => w != "," && w != "").ToList();
                        var tick1_ = tic1.Distinct().ToList(); s.Ticket1 = "";
                        if (tick1_ != null && tick1_.Count > 0)
                        {
                            tick1_.ForEach(a =>
                            {
                                if (!string.IsNullOrEmpty(a))
                                {
                                    s.Ticket1 += tic1.Where(q => q == a).Count() + "*" + a.Split(".")[0] + "/";
                                    if (!string.IsNullOrEmpty(s.NoOfTicket1))
                                    {
                                        s.Ticket1 += s.NoOfTicket1 + "/";
                                    }
                                }
                            });
                        }
                        s.Ticket1 += s.ProdOrderNo1;
                    }
                    var GroupTicket2 = s.GroupItemTicket2 + "," + s.GroupTicket2;
                    if (GroupTicket2 != null)
                    {
                        var tic2 = GroupTicket2.Split(",").ToList().Where(w => w != "," && w != "").ToList();
                        var tick2_ = tic2.Distinct().ToList(); s.Ticket2 = "";
                        if (tick2_ != null && tick2_.Count > 0)
                        {
                            tick2_.ForEach(a =>
                            {
                                if (!string.IsNullOrEmpty(a))
                                {
                                    s.Ticket2 += tic2.Where(q => q == a).Count() + "*" + a.Split(".")[0] + "/";
                                    if (!string.IsNullOrEmpty(s.NoOfTicket2))
                                    {
                                        s.Ticket2 += s.NoOfTicket2 + "/";
                                    }
                                }
                            });
                        }
                        s.Ticket2 += s.ProdOrderNo2;
                    }
                    var GroupTicket3 = s.GroupItemTicket3 + "," + s.GroupTicket3;
                    if (GroupTicket3 != null)
                    {
                        var tic3 = GroupTicket3.Split(",").ToList().Where(w => w != "," && w != "").ToList();
                        var tick3_ = tic3.Distinct().ToList(); s.Ticket3 = "";
                        if (tick3_ != null && tick3_.Count > 0)
                        {
                            tick3_.ForEach(a =>
                            {
                                if (!string.IsNullOrEmpty(a))
                                {
                                    s.Ticket3 += tic3.Where(q => q == a).Count() + "*" + a.Split(".")[0] + "/";
                                    if (!string.IsNullOrEmpty(s.NoOfTicket3))
                                    {
                                        s.Ticket3 += s.NoOfTicket3 + "/";
                                    }
                                }
                            });
                        }
                        s.Ticket3 += s.ProdOrderNo3;
                    }
                    var GroupTicket4 = s.GroupItemTicket4 + "," + s.GroupTicket4;
                    if (GroupTicket4 != null)
                    {
                        var tic4 = GroupTicket4.Split(",").ToList().Where(w => w != "," && w != "").ToList();
                        var tick4_ = tic4.Distinct().ToList(); s.Ticket4 = "";
                        if (tick4_ != null && tick4_.Count > 0)
                        {
                            tick4_.ForEach(a =>
                            {
                                if (!string.IsNullOrEmpty(a))
                                {
                                    s.Ticket4 += tic4.Where(q => q == a).Count() + "*" + a.Split(".")[0] + "/";
                                    if (!string.IsNullOrEmpty(s.NoOfTicket4))
                                    {
                                        s.Ticket4 += s.NoOfTicket4 + "/";
                                    }
                                }
                            });
                        }
                        s.Ticket4 += s.ProdOrderNo4;
                    }
                    var GroupTicket5 = s.GroupItemTicket5 + "," + s.GroupTicket5;
                    if (GroupTicket5 != null)
                    {
                        var tic5 = GroupTicket5.Split(",").ToList().Where(w => w != "," && w != "").ToList();
                        var tick5_ = tic5.Distinct().ToList(); s.Ticket5 = "";
                        if (tick5_ != null && tick5_.Count > 0)
                        {
                            tick5_.ForEach(a =>
                            {
                                if (!string.IsNullOrEmpty(a))
                                {
                                    s.Ticket5 += tic5.Where(q => q == a).Count() + "*" + a.Split(".")[0] + "/";
                                    if (!string.IsNullOrEmpty(s.NoOfTicket5))
                                    {
                                        s.Ticket5 += s.NoOfTicket5 + "/";
                                    }
                                }
                            });
                        }
                        s.Ticket5 += s.ProdOrderNo5;
                    }
                    var GroupTicket6 = s.GroupItemTicket6 + "," + s.GroupTicket6;
                    if (GroupTicket6 != null)
                    {
                        var tic6 = GroupTicket6.Split(",").ToList().Where(w => w != "," && w != "").ToList();
                        var tick6_ = tic6.Distinct().ToList(); s.Ticket6 = "";
                        if (tick6_ != null && tick6_.Count > 0)
                        {
                            tick6_.ForEach(a =>
                            {
                                if (!string.IsNullOrEmpty(a))
                                {
                                    s.Ticket6 += tic6.Where(q => q == a).Count() + "*" + a.Split(".")[0] + "/";
                                    if (!string.IsNullOrEmpty(s.NoOfTicket6))
                                    {
                                        s.Ticket6 += s.NoOfTicket6 + "/";
                                    }
                                }
                            });
                        }
                        s.Ticket6 += s.ProdOrderNo6;
                    }
                    var GroupTicket7 = s.GroupItemTicket7 + "," + s.GroupTicket7;
                    if (GroupTicket7 != null)
                    {
                        var tic7 = GroupTicket7.Split(",").ToList().Where(w => w != "," && w != "").ToList();
                        var tick7_ = tic7.Distinct().ToList(); s.Ticket7 = "";
                        if (tick7_ != null && tick7_.Count > 0)
                        {
                            tick7_.ForEach(a =>
                            {
                                if (!string.IsNullOrEmpty(a))
                                {
                                    s.Ticket7 += tic7.Where(q => q == a).Count() + "*" + a.Split(".")[0] + "/";
                                    if (!string.IsNullOrEmpty(s.NoOfTicket7))
                                    {
                                        s.Ticket7 += s.NoOfTicket7 + "/";
                                    }
                                }
                            });
                        }
                        s.Ticket7 += s.ProdOrderNo7;
                    }
                    var GroupTicket8 = s.GroupItemTicket8 + "," + s.GroupTicket8;
                    if (GroupTicket8 != null)
                    {
                        var tic8 = GroupTicket8.Split(",").ToList().Where(w => w != "," && w != "").ToList();
                        var tick8_ = tic8.Distinct().ToList(); s.Ticket8 = "";
                        if (tick8_ != null && tick8_.Count > 0)
                        {
                            tick8_.ForEach(a =>
                            {
                                if (!string.IsNullOrEmpty(a))
                                {
                                    s.Ticket8 += tic8.Where(q => q == a).Count() + "*" + a.Split(".")[0] + "/";
                                    if (!string.IsNullOrEmpty(s.NoOfTicket8))
                                    {
                                        s.Ticket8 += s.NoOfTicket8 + "/";
                                    }
                                }
                            });
                        }
                        s.Ticket8 += s.ProdOrderNo8;
                    }
                    var GroupTicket9 = s.GroupItemTicket9 + "," + s.GroupTicket9;
                    if (GroupTicket9 != null)
                    {
                        var tic9 = GroupTicket9.Split(",").ToList().Where(w => w != "," && w != "").ToList();
                        var tick9_ = tic9.Distinct().ToList(); s.Ticket9 = "";
                        if (tick9_ != null && tick9_.Count > 0)
                        {
                            tick9_.ForEach(a =>
                            {
                                if (!string.IsNullOrEmpty(a))
                                {
                                    s.Ticket9 += tic9.Where(q => q == a).Count() + "*" + a.Split(".")[0] + "/";
                                    if (!string.IsNullOrEmpty(s.NoOfTicket9))
                                    {
                                        s.Ticket9 += s.NoOfTicket9 + "/";
                                    }
                                }
                            });
                        }
                        s.Ticket9 += s.ProdOrderNo9;
                    }
                    var GroupTicket10 = s.GroupItemTicket10 + "," + s.GroupTicket10;
                    if (GroupTicket10 != null)
                    {
                        var tic10 = GroupTicket10.Split(",").ToList().Where(w => w != "," && w != "").ToList();
                        var tick10_ = tic10.Distinct().ToList(); s.Ticket10 = "";
                        if (tick10_ != null && tick10_.Count > 0)
                        {
                            tick10_.ForEach(a =>
                            {
                                if (!string.IsNullOrEmpty(a))
                                {
                                    s.Ticket10 += tic10.Where(q => q == a).Count() + "*" + a.Split(".")[0] + "/";
                                    if (!string.IsNullOrEmpty(s.NoOfTicket10))
                                    {
                                        s.Ticket10 += s.NoOfTicket10 + "/";
                                    }
                                }
                            });
                        }
                        s.Ticket10 += s.ProdOrderNo10;
                    }
                    var GroupTicket11 = s.GroupItemTicket11 + "," + s.GroupTicket11;
                    if (GroupTicket11 != null)
                    {
                        var tic11 = GroupTicket11.Split(",").ToList().Where(w => w != "," && w != "").ToList();
                        var tick11_ = tic11.Distinct().ToList(); s.Ticket11 = "";
                        if (tick11_ != null && tick11_.Count > 0)
                        {
                            tick11_.ForEach(a =>
                            {
                                if (!string.IsNullOrEmpty(a))
                                {
                                    s.Ticket11 += tic11.Where(q => q == a).Count() + "*" + a.Split(".")[0] + "/";
                                    if (!string.IsNullOrEmpty(s.NoOfTicket11))
                                    {
                                        s.Ticket11 += s.NoOfTicket11 + "/";
                                    }
                                }
                            });
                        }
                        s.Ticket11 += s.ProdOrderNo11;
                    }
                    var GroupTicket12 = s.GroupItemTicket12 + "," + s.GroupTicket12;
                    if (GroupTicket12 != null)
                    {
                        var tic12 = GroupTicket12.Split(",").ToList().Where(w => w != "," && w != "").ToList();
                        var tick12_ = tic12.Distinct().ToList(); s.Ticket12 = "";
                        if (tick12_ != null && tick12_.Count > 0)
                        {
                            tick12_.ForEach(a =>
                            {
                                if (!string.IsNullOrEmpty(a))
                                {
                                    s.Ticket12 += tic12.Where(q => q == a).Count() + "*" + a.Split(".")[0] + "/";
                                    if (!string.IsNullOrEmpty(s.NoOfTicket12))
                                    {
                                        s.Ticket12 += s.NoOfTicket12 + "/";
                                    }
                                }
                            });
                        }
                        s.Ticket12 += s.ProdOrderNo12;
                    }
                });
            }
            return acreports.OrderBy(o => o.ItemNo).ToList();

        }

        private async Task<List<INPCalendarPivotModel>> SimulationAddhocV33(DateRangeModel endDate, List<NAVRecipesModel> recipeList, List<NAVRecipesModel> _orderRecipeList)
        {
            var categoryList = new List<string>
            {
                "CAP",
                "CREAM",
                "DD",
                "SYRUP",
                "TABLET",
                "VET",
                "POWDER",
                "INJ"
            };
            var salesCatLists = salesCatList();
            var itemdict = new Dictionary<string, decimal>();
            var MethodCodeList = new List<INPCalendarPivotModel>();
            var companyIds = new List<long?> { endDate.CompanyId };
            if (endDate.CompanyId == 3)
            {
                companyIds = new List<long?> { 1, 2 };
            }
            var intercompanyIds = new List<long?> { 1, 2 };
            var month = endDate.StockMonth.Month;//== 1 ? 12 : endDate.StockMonth.Month - 1;
            var year = endDate.StockMonth.Year;// == 1 ? endDate.StockMonth.Year - 1 : endDate.StockMonth.Year;
            var weekofmonth = GetWeekNumberOfMonth(endDate.StockMonth);
            var intransitMonth = month == 1 ? 12 : month - 1;
            DateTime lastDay = new DateTime(endDate.StockMonth.Year, endDate.StockMonth.Month, 1).AddMonths(1).AddDays(-1);

            var doNotReceivedList = new List<NavpostedShipment>();
            var navMethodCodeLines = new List<NavMethodCodeLines>(); var acitems = new List<NavItemCitemList>();
            var acItemBalListResult = new List<DistStockBalModel>(); var acEntries = new List<ACItemsModel>(); var dismapeditems = new List<NavItemCitemList>();
            var categoryItems = new List<NavSaleCategory>(); var itemRelations = new List<NavitemLinks>(); var prodyctionTickets = new List<ProductionSimulation>();
            var prenavStkbalance = new List<NavitemStockBalance>(); var navStkbalance = new List<NavitemStockBalance>(); var prodyctionoutTickets = new List<ProductionSimulation>();
            var blanletOrders = new List<SimulationAddhoc>(); var pre_acItemBalListResult = new List<DistStockBalModel>(); var grouptickets = new List<GroupPlaningTicket>();
            var intercompanyItems = new List<Navitems>(); var itemMasterforReport = new List<Navitems>(); var orderRequirements = new List<OrderRequirementLineModel>(); var acEntriesList = new List<Acitems>();
            DateTime firstDayOfMonth = new DateTime(endDate.StockMonth.Year, endDate.StockMonth.Month, 1);
            var dateMonth1 = firstDayOfMonth;// endDate.StockMonth;
            var datemonth12 = endDate.StockMonth.AddMonths(12);
            using (var connection = CreateConnection())
            {

                var query = "select ShipmentId,\r\nCompany,\r\nCompanyId,\r\nStockBalanceMonth,\r\nPostingDate,\r\nCustomer,\r\nCustomerNo,\r\nCustomerId,\r\nDeliveryOrderNo,\r\nDOLineNo,\r\nItemNo,\r\nDescription,\r\nIsRecived,\r\nDoQty,\r\nStatusCodeID,\r\nAddedByUserID,\r\nAddedDate,\r\nModifiedByUserID from NavpostedShipment WHERE CompanyId  in(" + string.Join(',', intercompanyIds) + ") AND CAST(StockBalanceMonth AS Date) <='" + lastDay + "'  AND (IsRecived is null Or IsRecived=0)\r\n;";
                query += "select t2.DistAcid,(case when t1.Quantity is NULL then  0 ELSE t1.Quantity END) as QtyOnHand,t2.ItemNo,t2.ItemDesc,t2.DistName,t1.DistItemId,(case when t2.CustomerId is NULL then  -1 ELSE t2.CustomerId END) as CustomerId,t2.CompanyId from " +
                    "DistStockBalance t1 INNER JOIN Acitems t2  ON t1.DistItemId=t2.DistACID\r\n" +
                    "Where  (t1.StockBalWeek=" + weekofmonth + " OR t1.StockBalWeek is null ) AND MONTH(t1.StockBalMonth) = " + month + " AND YEAR(t1.StockBalMonth)=" + year + ";\r\n";
                query += "select t2.DistAcid,(case when t1.Quantity is NULL then  0 ELSE t1.Quantity END) as QtyOnHand,t2.ItemNo,t2.ItemDesc,t2.DistName,t1.DistItemId,(case when t2.CustomerId is NULL then  -1 ELSE t2.CustomerId END) as CustomerId,t2.CompanyId from " +
                    "DistStockBalance t1 INNER JOIN Acitems t2  ON t1.DistItemId=t2.DistACID\r\n" +
                    "Where   t1.StockBalWeek=1 AND MONTH(t1.StockBalMonth) = " + month + " AND YEAR(t1.StockBalMonth)=" + year + "\r\n;";
                query += "select t1.CustomerId as DistId,t1.ToDate as ACMonth,t3.No as ItemNo,t2.Quantity as ACQty,t2.ItemId as SWItemId,t3.Description as ItemDesc,t1.CustomerId from Acentry t1 INNER JOIN AcentryLines t2 ON t1.ACEntryId=t2.ACEntryId INNER JOIN NAVItems t3 ON t2.ItemId=t3.ItemId\r\n" +
                    "WHERE t1.CompanyId  in(" + string.Join(',', companyIds) + ") AND CAST(t1.ToDate AS Date)>='" + endDate.StockMonth + "'  AND CAST(t1.FromDate AS Date)<='" + endDate.StockMonth + "';\r\n";
                query += "select NavItemCItemId,\r\nNavItemId,\r\nNavItemCustomerItemId from NavItemCitemList;\r\n";
                query += "select SaleCategoryID,\r\nCode,\r\nDescription,\r\nNoSeries,\r\nLocationID,\r\nSGNoSeries,\r\nStatusCodeID,\r\nAddedByUserID,\r\nAddedDate,\r\nModifiedByUserID,\r\nModifiedDate from NavSaleCategory;\r\n";
                query += "select ItemLinkId,\r\nMyItemId,\r\nSgItemId,\r\nStatusCodeID,\r\nAddedByUserID,\r\nAddedDate,\r\nModifiedByUserID,\r\nModifiedDate from NavitemLinks where SgItemId is not null AND MyItemId is not null;\r\n";
                query += "select ProductionSimulationID,\r\nCompanyId,\r\nProdOrderNo,\r\nItemID,\r\nItemNo,\r\nDescription,\r\nPackSize,\r\nQuantity,\r\nUOM,\r\nPerQuantity,\r\nPerQtyUOM,\r\nBatchNo,\r\nPlannedQty,\r\nOutputQty,\r\nIsOutput,\r\nStartingDate,\r\nStatusCodeID,\r\nAddedByUserID,\r\nAddedDate,\r\nModifiedByUserID,\r\nModifiedDate,\r\nProcessDate,\r\nIsBMRTicket,\r\nRePlanRefNo,\r\nBatchSize,\r\nDispense from ProductionSimulation where IsBmrticket=0 AND CAST(StartingDate AS Date)>='" + dateMonth1 + "'  AND CAST(StartingDate AS Date)<='" + datemonth12 + "' order by StartingDate desc;\r\n";

                query += "select ProductionSimulationID,\r\nCompanyId,\r\nProdOrderNo,\r\nItemID,\r\nItemNo,\r\nDescription,\r\nPackSize,\r\nQuantity,\r\nUOM,\r\nPerQuantity,\r\nPerQtyUOM,\r\nBatchNo,\r\nPlannedQty,\r\nOutputQty,\r\nIsOutput,\r\nStartingDate,\r\nStatusCodeID,\r\nAddedByUserID,\r\nAddedDate,\r\nModifiedByUserID,\r\nModifiedDate,\r\nProcessDate,\r\nIsBMRTicket,\r\nRePlanRefNo,\r\nBatchSize,\r\nDispense from ProductionSimulation where IsBmrticket=0 AND CAST(ProcessDate AS Date)>='" + dateMonth1 + "'  AND CAST(ProcessDate AS Date)<='" + datemonth12 + "' order by ProcessDate desc;\r\n";

                query += "SELECT t1.NavStockBalanceId,\r\nt1.ItemId,\r\nt1.StockBalMonth,\r\nt1.StockBalWeek,\r\nt1.Quantity,\r\nt1.RejectQuantity,\r\nt1.StatusCodeID,\r\nt1.AddedByUserID,\r\nt1.AddedDate,\r\nt1.ModifiedByUserID,\r\nt1.ModifiedDate,\r\nt1.ReworkQty,\r\nt1.WIPQty,\r\nt1.GlobalQty,\r\nt1.KIVQty,\r\nt1.SupplyWIPQty,\r\nt1.Supply1ProcessQty,\r\nt1.NotStartInvQty,t2.PackQty from NavitemStockBalance t1 INNER JOIN navitems t2 ON t1.ItemId=t2.ItemId Where t1.StockBalWeek=1 AND t2.ItemId is Not NUll and MONTH(t1.StockBalMonth) =" + month + " AND YEAR(t1.StockBalMonth)=" + year + ";\r\n";

                query += "SELECT t1.NavStockBalanceId,\r\nt1.ItemId,\r\nt1.StockBalMonth,\r\nt1.StockBalWeek,\r\nt1.Quantity,\r\nt1.RejectQuantity,\r\nt1.StatusCodeID,\r\nt1.AddedByUserID,\r\nt1.AddedDate,\r\nt1.ModifiedByUserID,\r\nt1.ModifiedDate,\r\nt1.ReworkQty,\r\nt1.WIPQty,\r\nt1.GlobalQty,\r\nt1.KIVQty,\r\nt1.SupplyWIPQty,\r\nt1.Supply1ProcessQty,\r\nt1.NotStartInvQty,t2.PackQty from NavitemStockBalance t1 INNER JOIN navitems t2 ON t1.ItemId=t2.ItemId Where (t1.StockBalWeek is null OR t1.StockBalWeek=" + weekofmonth + ") AND t2.ItemId is Not NUll and MONTH(t1.StockBalMonth) =" + month + " AND YEAR(t1.StockBalMonth)=" + year + ";\r\n";
                query += "select t1.SimualtionAddhocID,\r\nt1.DocumantType,\r\nt1.SelltoCustomerNo,\r\nt1.CustomerName,\r\nt1.Categories,\r\nt1.DocumentNo,\r\nt1.ExternalDocNo,\r\nt1.ItemID,\r\nt1.ItemNo,\r\nt1.Description,\r\nt1.Description1,\r\nt1.OutstandingQty,\r\nt1.PromisedDate,\r\nt1.ShipmentDate,\r\nt1.UOMCode from SimulationAddhoc t1 where t1.CompanyId in(" + string.Join(',', companyIds) + ") AND CAST(t1.ShipmentDate AS Date)>='" + dateMonth1 + "'  AND CAST(t1.ShipmentDate AS Date)<='" + datemonth12 + "';\r\n";

                query += "select GroupPlanningId,\r\nCompanyId,\r\nBatchName,\r\nProductGroupCode,\r\nStartDate,\r\nItemNo,\r\nDescription,\r\nDescription1,\r\nBatchSize,\r\nQuantity,\r\nUOM,\r\nNoOfTicket,\r\nTotalQuantity from GroupPlaningTicket where CompanyId=" + endDate.CompanyId + " AND CAST(StartDate AS Date)>='" + dateMonth1 + "'  AND CAST(StartDate AS Date)<='" + datemonth12 + "' order by StartDate desc;\r\n";
                query += "select s.No,s.ItemId,s.Description,s.PackQty,s.CompanyId from NAVItems s;";
                query += "select t1.*,t2.Description2 as GenericCodeDescription2 from NAVItems t1\r\nLEFT JOIN GenericCodes t2 ON t1.GenericCodeId=t2.GenericCodeId\r\n" +
                    "where t1.CompanyId  in(" + string.Join(',', companyIds) + ") AND  t1.StatusCodeId=1 AND t1.ItemId IN(select NavItemId from NavItemCitemList WHere NavItemId>0);\r\n";


                query += "select t1.ProductId,t1.ProductQty,t1.NoOfTicket,t1.ExpectedStartDate,t1.RequireToSplit,t2.SplitProductID,t2.SplitProductQty from OrderRequirementLine t1\r\nINNER JOIN OrderRequirementLineSplit t2 ON t1.OrderRequirementLineID=t2.OrderRequirementLineID\r\nwhere t1.IsNavSync=1 AND CAST(t1.ExpectedStartDate AS Date)>='" + dateMonth1 + "'  AND CAST(t1.ExpectedStartDate AS Date)<='" + datemonth12 + "' order by t1.ExpectedStartDate desc;\r\n";
                query += "select t1.*,t6.DistACID,\r\nt6.CompanyId,\r\nt6.CustomerId,\r\nt6.DistName,\r\nt6.ItemGroup,\r\nt6.Steriod,\r\nt6.ShelfLife,\r\nt6.Quota,\r\nt6.Status,\r\nt6.ItemDesc,\r\nt6.PackSize,\r\nt6.ACQty,\r\nt6.ACMonth,\r\nt6.StatusCodeID,\r\nt6.AddedByUserID,\r\nt6.AddedDate,\r\nt6.ModifiedByUserID,\r\nt6.ModifiedDate,\r\nt6.ItemNo from NavItemCitemList t1\r\nINNER JOIN Acitems t6 ON t1.NavItemCustomerItemId=t6.DistACID;\r\n";
                query += "select t1.MethodCodeLineId,t1.MethodCodeId,t1.ItemID,t1.StatusCodeID,t1.AddedByUserID,t1.AddedDate,t1.ModifiedByUserID,t1.MethodCodeLineID,\r\nt2.MethodName,t2.MethodDescription,t2.NAVINPCategoryID,t2.CompanyId,t2.ProdFrequency,t2.DistReplenishHS,t2.DistACMonth,t2.AdhocMonthStandAlone,t2.AdhocPlanQty from NavMethodCodeLines t1 INNER JOIN NavMethodCode t2 ON t2.MethodCodeId=t1.MethodCodeId \r\nWHERE t1.ItemID IN(select t3.ItemId from NAVItems t3 LEFT JOIN GenericCodes t4 ON t3.GenericCodeId=t4.GenericCodeId where t3.CompanyId  in(" + string.Join(',', companyIds) + ") AND  t3.StatusCodeId=1 AND t3.ItemId IN(select NavItemId from NavItemCitemList WHere NavItemId>0));\r\n";
                query += "select t1.NavItemCitemId,t1.NavItemId,t1.NavItemCustomerItemId,t2.DistACID,t2.CompanyId,t2.DistName,t2.ItemGroup,t2.Steriod,t2.ShelfLife,t2.Quota,t2.ItemDesc,t2.PackSize,t2.PackSize,t2.ACQty,t2.ItemNo from NavItemCitemList t1 INNER JOIN Acitems t2 ON t1.NavItemCustomerItemId=t2.DistACID\r\nWHERE t1.NavItemId IN(select t3.ItemId from NAVItems t3 LEFT JOIN GenericCodes t4 ON t3.GenericCodeId=t4.GenericCodeId where t3.CompanyId  in(" + string.Join(',', companyIds) + ") AND  t3.StatusCodeId=1 AND t3.ItemId IN(select NavItemId from NavItemCitemList WHere NavItemId>0));\r\n";
                var results = await connection.QueryMultipleAsync(query);
                doNotReceivedList = results.Read<NavpostedShipment>().ToList();
                acItemBalListResult = results.Read<DistStockBalModel>().ToList();
                pre_acItemBalListResult = results.Read<DistStockBalModel>().ToList();
                acEntries = results.Read<ACItemsModel>().ToList();
                dismapeditems = results.Read<NavItemCitemList>().ToList();
                categoryItems = results.Read<NavSaleCategory>().ToList();
                itemRelations = results.Read<NavitemLinks>().ToList();
                prodyctionTickets = results.Read<ProductionSimulation>().ToList();
                prodyctionoutTickets = results.Read<ProductionSimulation>().ToList();
                prenavStkbalance = results.Read<NavitemStockBalance>().ToList();
                navStkbalance = results.Read<NavitemStockBalance>().ToList();
                blanletOrders = results.Read<SimulationAddhoc>().ToList();
                grouptickets = results.Read<GroupPlaningTicket>().ToList();
                intercompanyItems = results.Read<Navitems>().ToList();
                itemMasterforReport = results.Read<Navitems>().ToList();
                orderRequirements = results.Read<OrderRequirementLineModel>().ToList();
                acEntriesList = results.Read<Acitems>().ToList();
                navMethodCodeLines = results.Read<NavMethodCodeLines>().ToList();
                acitems = results.Read<NavItemCitemList>().ToList();
            }

            if (itemMasterforReport != null && itemMasterforReport.Count() > 0)
            {
                itemMasterforReport.ForEach(s =>
                {
                    s.NavMethodCodeLines = navMethodCodeLines.Where(w => w.ItemId == s.ItemId).ToList();
                    s.NavItemCitemList = acitems.Where(w => w.NavItemId == s.ItemId).ToList();
                });
            }

            var packSize = 900;
            decimal packSize2 = 0;
            var tenderExist1 = false;
            var tenderExist2 = false;
            var tenderExist3 = false;
            var tenderExist4 = false;
            var tenderExist5 = false;
            var tenderExist6 = false;
            var tenderExist7 = false;
            var tenderExist8 = false;
            var tenderExist9 = false;
            var tenderExist10 = false;
            var tenderExist11 = false;
            var tenderExist12 = false;


            var month1 = month;
            var month2 = month1 + 1 > 12 ? 1 : month1 + 1;
            var month3 = month2 + 1 > 12 ? 1 : month2 + 1;
            var month4 = month3 + 1 > 12 ? 1 : month3 + 1;
            var month5 = month4 + 1 > 12 ? 1 : month4 + 1;
            var month6 = month5 + 1 > 12 ? 1 : month5 + 1;
            var month7 = month6 + 1 > 12 ? 1 : month6 + 1;
            var month8 = month7 + 1 > 12 ? 1 : month7 + 1;
            var month9 = month8 + 1 > 12 ? 1 : month8 + 1;
            var month10 = month9 + 1 > 12 ? 1 : month9 + 1;
            var month11 = month10 + 1 > 12 ? 1 : month10 + 1;
            var month12 = month11 + 1 > 12 ? 1 : month11 + 1;

            var nextYear = year + 1;
            var year1 = year;
            var year2 = month2 > month ? year : nextYear;
            var year3 = month3 > month ? year : nextYear;
            var year4 = month4 > month ? year : nextYear;
            var year5 = month5 > month ? year : nextYear;
            var year6 = month6 > month ? year : nextYear;
            var year7 = month7 > month ? year : nextYear;
            var year8 = month8 > month ? year : nextYear;
            var year9 = month9 > month ? year : nextYear;
            var year10 = month10 > month ? year : nextYear;
            var year11 = month11 > month ? year : nextYear;
            var year12 = month12 > month ? year : nextYear;


            var acModel = new List<ACItemsModel>();
            if (acEntries != null && acEntries.Count > 0)
            {
                acEntries.ForEach(ac =>
                {
                    if (ac.CustomerId == 62)
                    {
                        ac.DistName = "Apex";
                    }
                    else if (ac.CustomerId == 51)
                    {
                        ac.DistName = "PSB PX";
                    }
                    else if (ac.CustomerId == 39)
                    {
                        ac.DistName = "MSS";
                    }
                    else if (ac.CustomerId == 60)
                    {
                        ac.DistName = "SG Tender";
                    }
                    else
                    {
                        ac.DistName = "Antah";
                    }
                    acModel.Add(ac);
                });

            }

            if (prenavStkbalance != null && prenavStkbalance.Count() > 0)
            {
                prenavStkbalance.ForEach(f =>
                {
                    f.Quantity = f.Quantity * (long)(f.PackQty > 0 ? f.PackQty : 0);
                });
            }
            if (navStkbalance != null && navStkbalance.Count() > 0)
            {
                navStkbalance.ForEach(f =>
                {
                    f.Quantity = f.Quantity * (long)(f.PackQty > 0 ? f.PackQty : 0);
                });
            }

            doNotReceivedList.ForEach(d =>
            {
                var item = intercompanyItems.FirstOrDefault(f => f.No == d.ItemNo && f.CompanyId == d.CompanyId);
                if (item != null)
                {
                    d.DoQty *= item.PackQty;
                }
            });
            int? parent = null;
            var genericId = new List<long?>();

            var orederRequ = new List<OrderRequirementLineModel>();

            orderRequirements.ForEach(f =>
            {
                if (!f.RequireToSplit.GetValueOrDefault(false))
                {
                    orederRequ.Add(new OrderRequirementLineModel
                    {
                        ProductId = f.ProductId,
                        ProductQty = f.ProductQty * f.NoOfTicket,
                        NoOfTicket = f.NoOfTicket,
                        ExpectedStartDate = f.ExpectedStartDate
                    });
                }
                else
                {
                    orederRequ.Add(new OrderRequirementLineModel
                    {
                        ProductId = f.SplitProductId,
                        ProductQty = f.SplitProductQty * f.NoOfTicket,
                        NoOfTicket = f.NoOfTicket,
                        ExpectedStartDate = f.ExpectedStartDate
                    });
                }
            });
            itemMasterforReport.ForEach(ac =>
            {
                if (ac.No == "FP-PP-TAB-302")
                {

                }

                if (!genericId.Exists(g => g == ac.GenericCodeId) && ac.GenericCodeId != null)
                {
                    var geericRptList = new List<GenericCodeReport>();
                    var itemNosRpt = itemMasterforReport.Where(i => i.GenericCodeId == ac.GenericCodeId).ToList();
                    var itemNos = itemMasterforReport.Where(i => i.GenericCodeId == ac.GenericCodeId).Select(s => s.ItemId).ToList();
                    //itemMasterforReport.Where(i => i.GenericCodeId == ac.GenericCodeId).Select(s => string.Format("{0} - {1} - {2}", s.No, s.Description, s.Description2)).ToList();
                    var itemCodes = itemMasterforReport.Where(i => i.GenericCodeId == ac.GenericCodeId).Select(s => s.No).ToList();
                    itemNosRpt.ForEach(rpt =>
                    {
                        var distId = dismapeditems.Where(d => d.NavItemId == rpt.ItemId).Select(s => s.NavItemCustomerItemId);
                        geericRptList.Add(new GenericCodeReport
                        {
                            ItemNo = rpt.No,
                            Description = rpt.Description,
                            Description2 = rpt.Description2,
                            ItemCategory = rpt.CategoryId.GetValueOrDefault(0) > 0 ? categoryList[int.Parse(rpt.CategoryId.ToString()) - 1] : string.Empty,
                            InternalRefNo = rpt.InternalRef,
                            //MethodCode = rpt.NavMethodCodeLines.Count > 0 ? rpt.NavMethodCodeLines.First().MethodCode.MethodName : "No MethodCode",
                            //DistItem = rpt.NavItemCitemList.Count > 0 ? rpt.NavItemCitemList.FirstOrDefault().NavItemCustomerItem.ItemDesc : string.Empty,
                            //StockBalance = acItemBalListResult.Where(a => distId.Contains(a.DistItemId)).Sum(s => s.QtyOnHand)
                        });

                    });
                    bool itemMapped = false;
                    decimal SgTenderStockBalance = 0;
                    genericId.Add(ac.GenericCodeId);
                    decimal? inventoryQty = 0;//itemMasterforReport.Where(i => i.GenericCodeId == ac.GenericCodeId).Sum(s => s.Inventory);
                    var navItemIds = itemMasterforReport.Where(i => i.GenericCodeId == ac.GenericCodeId).Select(s => s.ItemId).ToList();
                    inventoryQty = (navStkbalance.Where(n => navItemIds.Contains(n.ItemId.Value)).Sum(s => s.Quantity));
                    var wipQty = (navStkbalance.Where(n => navItemIds.Contains(n.ItemId.Value)).Sum(s => s.Wipqty));
                    decimal? notStartInvQty = (navStkbalance.Where(n => navItemIds.Contains(n.ItemId.Value)).Sum(s => s.NotStartInvQty));
                    var reWorkQty = (navStkbalance.Where(n => navItemIds.Contains(n.ItemId.Value)).Sum(s => s.ReworkQty));
                    var globalQty = (navStkbalance.Where(n => navItemIds.Contains(n.ItemId.Value)).Sum(s => s.GlobalQty));
                    decimal? interCompanyTransitQty = 0;

                    if (ac.No == "FP-PP-TAB-302")
                    {

                    }

                    if (endDate.CompanyId == 1)
                    {
                        var linkItemIds = itemRelations.Where(f => navItemIds.Contains(f.MyItemId.Value)).Select(s => s.SgItemId).ToList();
                        if (linkItemIds.Count > 0)
                        {
                            itemMapped = true;
                            SgTenderStockBalance = navStkbalance.Where(n => linkItemIds.Contains(n.ItemId.Value)).Sum(s => s.Quantity.GetValueOrDefault(0));

                            var itemNosIntertrans = intercompanyItems.Where(t => linkItemIds.Contains(t.ItemId) && t.CompanyId == 2).Select(n => n.No).ToList();
                            interCompanyTransitQty = doNotReceivedList.Where(d => itemNosIntertrans.Contains(d.ItemNo) && d.IsRecived == false && d.CompanyId == 2).Sum(s => s.DoQty);
                        }


                    }
                    else if (endDate.CompanyId == 2)
                    {
                        var linkItemIds = itemRelations.Where(f => navItemIds.Contains(f.SgItemId.Value)).Select(s => s.MyItemId).ToList();
                        if (linkItemIds.Count > 0)
                        {
                            itemMapped = true;
                            SgTenderStockBalance = navStkbalance.Where(n => linkItemIds.Contains(n.ItemId.Value)).Sum(s => s.Quantity.GetValueOrDefault(0));

                            var itemNosIntertrans = intercompanyItems.Where(t => linkItemIds.Contains(t.ItemId) && t.CompanyId == 1).Select(n => n.No).ToList();
                            interCompanyTransitQty = doNotReceivedList.Where(d => itemNosIntertrans.Contains(d.ItemNo) && d.IsRecived == false && d.CompanyId == 1).Sum(s => s.DoQty);
                        }
                    }
                    else if (endDate.CompanyId == 3)
                    {
                        var linkItemIds = itemRelations.Where(f => navItemIds.Contains(f.MyItemId.Value)).Select(s => s.SgItemId).ToList();
                        if (linkItemIds.Count > 0)
                        {
                            itemMapped = true;
                            inventoryQty = (navStkbalance.Where(n => linkItemIds.Contains(n.ItemId.Value)).Sum(s => s.Quantity.GetValueOrDefault(0)));
                        }

                        var sglinkItemIds = itemRelations.Where(f => navItemIds.Contains(f.SgItemId.Value)).Select(s => s.MyItemId).ToList();
                        if (linkItemIds.Count > 0) SgTenderStockBalance = navStkbalance.Where(n => sglinkItemIds.Contains(n.ItemId.Value)).Sum(s => s.Quantity.GetValueOrDefault(0));
                    }
                    var custmerItem = ac.NavItemCitemList.Count > 0 ? ac.NavItemCitemList.FirstOrDefault() : new NavItemCitemList();
                    if (custmerItem.DistAcid > 0)
                    {
                        var distIds = dismapeditems.Where(di => navItemIds.Contains(di.NavItemId.Value)).Select(s => s.NavItemCustomerItemId).Distinct().ToList(); //ac.NavItemCitemList.Select(s => s.NavItemCustomerItemId).ToList();
                        var distStkBal = acItemBalListResult.Where(d => distIds.Contains(d.DistItemId) && d.CompanyId == endDate.CompanyId).Sum(s => s.QtyOnHand);
                        var val = ac.PackSize.HasValue ? ac.PackSize.Value : 900;
                        packSize = val;

                        var prodRecipe = "";
                        var currentBatch = "";
                        var methodeCodeID = ac.NavMethodCodeLines.Count() > 0 ? ac.NavMethodCodeLines.First().MethodCodeId : -1;
                        var itemreceips = recipeList.Where(r => r.ItemRecipeId == methodeCodeID).ToList();
                        if (itemreceips.Count > 0)
                        {
                            var batchSizedes = itemreceips.FirstOrDefault().DefaultBatch;
                            currentBatch = batchSizedes;
                            //string numberOnly = Regex.Replace(batchSizedes, "[^0-9.]", "");
                            //if (!string.IsNullOrEmpty(numberOnly))
                            packSize2 = itemreceips.FirstOrDefault().UnitQTY;
                            prodRecipe = string.Join(",", itemreceips.Select(r => r.RecipeNo).ToList());//string.Format("{0}", itemreceips.FirstOrDefault().DefaultBatch + " | " + batchSizedes);

                        }
                        var Orderitemreceips = _orderRecipeList.Where(r => r.ItemNo == ac.No).ToList();
                        //if (Orderitemreceips.Count > 0)
                        //{
                        //    //var batchSizedes = itemreceips.OrderByDescending(o => o.BatchSize).FirstOrDefault().BatchSize;
                        //    //string numberOnly = Regex.Replace(batchSizedes, "[^0-9.]", "");
                        //    //if (!string.IsNullOrEmpty(numberOnly))
                        //    //    packSize2 = decimal.Parse(numberOnly) * 1000;
                        //    //prodRecipe = string.Format("{0}", itemreceips.OrderByDescending(o => o.BatchSize).FirstOrDefault().RecipeNo + " | " + batchSizedes);

                        //}
                        var threeMonth = distStkBal * packSize * 3;
                        var BatchSize = 900000 / packSize / 50;
                        var StockBalance = distStkBal + inventoryQty;
                        var NoofTickets = (threeMonth / packSize * 1000);
                        //var custId = custmerItem.DistName == "Apex" ? 21 : 1;
                        var acitem = acModel.Where(f => navItemIds.Contains(f.SWItemId.Value)).ToList();
                        var distItemBal = acItemBalListResult.Where(f => distIds.Contains(f.DistAcid) && f.CompanyId == endDate.CompanyId).ToList();
                        var apexQty = acitem.FirstOrDefault(f => f.CustomerId == 62) != null ? acitem.Where(f => f.CustomerId == 62).Sum(s => s.ACQty) : 0;
                        var antahQty = acitem.FirstOrDefault(f => f.CustomerId == 1) != null ? acitem.Where(f => f.CustomerId == 1).Sum(s => s.ACQty) : 0;
                        var sgtQty = acitem.FirstOrDefault(f => f.CustomerId == 60) != null ? acitem.Where(f => f.CustomerId == 60).Sum(s => s.ACQty) : 0;
                        var missQty = acitem.FirstOrDefault(f => f.CustomerId == 39) != null ? acitem.Where(f => f.CustomerId == 39).Sum(s => s.ACQty) : 0;
                        var pxQty = acitem.FirstOrDefault(f => f.CustomerId == 51) != null ? acitem.Where(f => f.CustomerId == 51).Sum(s => s.ACQty) : 0;
                        //sgtQty
                        var symlQty = packSize * (apexQty + antahQty + 0 + missQty + pxQty);
                        threeMonth = symlQty * 3;
                        var distTotal = (apexQty + antahQty + 0 + missQty + pxQty);
                        var AntahStockBalance = distItemBal.FirstOrDefault(f => f.CustomerId == 1) != null ? distItemBal.Where(f => f.CustomerId == 1).Sum(s => s.QtyOnHand) : 0;
                        var ApexStockBalance = distItemBal.FirstOrDefault(f => f.CustomerId == 62) != null ? distItemBal.Where(f => f.CustomerId == 62).Sum(s => s.QtyOnHand) : 0;
                        if (!itemMapped) SgTenderStockBalance = distItemBal.FirstOrDefault(f => f.DistName == "SG Tender") != null ? distItemBal.Where(f => f.DistName == "SG Tender").Sum(s => s.QtyOnHand) : 0;
                        var MsbStockBalance = distItemBal.FirstOrDefault(f => f.CustomerId == 39) != null ? distItemBal.Where(f => f.CustomerId == 39).Sum(s => s.QtyOnHand) : 0;
                        var PsbStockBalance = distItemBal.FirstOrDefault(f => f.CustomerId == 51) != null ? distItemBal.Where(f => f.CustomerId == 51).Sum(s => s.QtyOnHand) : 0;
                        decimal stockHoldingBalance = 0;
                        // decimal myStockBalance = 0;

                        var inTransitQty = doNotReceivedList.Where(d => itemCodes.Contains(d.ItemNo) && d.IsRecived == false && d.CompanyId == endDate.CompanyId).Sum(s => s.DoQty);
                        if (distTotal > 0)
                        {
                            stockHoldingBalance = (interCompanyTransitQty.GetValueOrDefault(0) + notStartInvQty.GetValueOrDefault(0) + wipQty.GetValueOrDefault(0) + 0 + globalQty.GetValueOrDefault(0) + AntahStockBalance + ApexStockBalance + MsbStockBalance + PsbStockBalance + SgTenderStockBalance + inventoryQty.GetValueOrDefault(0) + inTransitQty.GetValueOrDefault(0)) / distTotal;
                        }

                        var isTenderExist = blanletOrders.Any(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)));
                        tenderExist1 = blanletOrders.Any(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month1 && t.ShipmentDate.Value.Year == year1);
                        tenderExist2 = blanletOrders.Any(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month2 && t.ShipmentDate.Value.Year == year2);
                        tenderExist3 = blanletOrders.Any(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month3 && t.ShipmentDate.Value.Year == year3);
                        tenderExist4 = blanletOrders.Any(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month4 && t.ShipmentDate.Value.Year == year4);
                        tenderExist5 = blanletOrders.Any(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month5 && t.ShipmentDate.Value.Year == year5);
                        tenderExist6 = blanletOrders.Any(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month6 && t.ShipmentDate.Value.Year == year6);
                        tenderExist7 = blanletOrders.Any(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month7 && t.ShipmentDate.Value.Year == year7);
                        tenderExist8 = blanletOrders.Any(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month8 && t.ShipmentDate.Value.Year == year8);
                        tenderExist9 = blanletOrders.Any(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month9 && t.ShipmentDate.Value.Year == year9);
                        tenderExist10 = blanletOrders.Any(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month10 && t.ShipmentDate.Value.Year == year10);
                        tenderExist11 = blanletOrders.Any(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month11 && t.ShipmentDate.Value.Year == year11);
                        tenderExist12 = blanletOrders.Any(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month12 && t.ShipmentDate.Value.Year == year12);
                        var groupItemNo = itemMasterforReport.Where(i => i.GenericCodeId == ac.GenericCodeId).Select(s => s.No).ToList();
                        itemdict.Add(ac.No, symlQty);
                        //var tenderSum = blanletOrders.Where(t => itemNos.Contains(t.ItemId.Value)).Sum(s => s.OutstandingQty.Value);
                        MethodCodeList.Add(new INPCalendarPivotModel
                        {
                            GenericCodeReport = geericRptList,
                            GrouoItemNo = ac.No,
                            ItemId = ac.ItemId,
                            ItemNo = ac.GenericCodeDescription2,
                            RecipeLists = itemreceips.Select(s => string.Format("{0}", s.RecipeNo + " | " + s.BatchSize)).ToList(),
                            ItemRecipeLists = itemreceips,
                            OrderRecipeLists = Orderitemreceips,
                            IsSteroid = ac.Steroid.GetValueOrDefault(false),
                            SalesCategoryId = ac.CategoryId,
                            SalesCategory = salesCatLists.Where(w => w.Id == ac.CategoryId).FirstOrDefault()?.Text,
                            LocationID = categoryItems != null && categoryItems.FirstOrDefault(a => a.SaleCategoryId == ac.CategoryId) != null ? categoryItems.FirstOrDefault(a => a.SaleCategoryId == ac.CategoryId).LocationId : null,
                            // SalesCategory = categoryItems != null && categoryItems.FirstOrDefault(a => a.SaleCategoryId == ac.CategoryId) != null ? categoryItems.FirstOrDefault(a => a.SaleCategoryId == ac.CategoryId).Code : "",
                            MethodCodeId = ac.NavMethodCodeLines.Count > 0 ? ac.NavMethodCodeLines.First().MethodCodeId : -1,
                            GenericCodeID = ac.GenericCodeId,
                            Customer = custmerItem.DistName,
                            Description = ac.Description + " " + ac.Description2,
                            PackSize = packSize,
                            PackSize2 = packSize2,
                            Quantity = distStkBal,
                            ApexQty = apexQty,
                            AntahQty = antahQty,
                            SgtQty = sgtQty,
                            PxQty = pxQty,
                            MissQty = missQty,
                            SymlQty = symlQty,
                            ACQty = distStkBal,
                            AcSum = distTotal,
                            UnitQty = distStkBal * packSize,
                            ThreeMonthACQty = threeMonth,
                            ProdRecipe = !string.IsNullOrEmpty(prodRecipe) ? prodRecipe : "",
                            BatchSize = "",
                            NoofTickets = "",
                            NoOfDays = "Hours " + (3 * NoofTickets) + "& Day" + (3 * NoofTickets / 8).ToString(),
                            DistStockBalance = (AntahStockBalance + ApexStockBalance + MsbStockBalance + PsbStockBalance + SgTenderStockBalance + inventoryQty.GetValueOrDefault(0)),
                            AntahStockBalance = AntahStockBalance,
                            ApexStockBalance = ApexStockBalance,
                            SgTenderStockBalance = endDate.CompanyId == 2 ? inventoryQty.GetValueOrDefault(0) : SgTenderStockBalance,
                            MsbStockBalance = MsbStockBalance,
                            PsbStockBalance = PsbStockBalance,
                            NAVStockBalance = inventoryQty.GetValueOrDefault(0),
                            WipQty = wipQty.GetValueOrDefault(0),
                            NotStartInvQty = notStartInvQty.GetValueOrDefault(0),
                            Rework = reWorkQty.GetValueOrDefault(0),
                            InterCompanyTransitQty = interCompanyTransitQty.GetValueOrDefault(0),
                            OtherStoreQty = globalQty.GetValueOrDefault(0),
                            StockBalance = (interCompanyTransitQty.GetValueOrDefault(0) + notStartInvQty.GetValueOrDefault(0) + wipQty.GetValueOrDefault(0) + 0 + globalQty.GetValueOrDefault(0) + AntahStockBalance + ApexStockBalance + MsbStockBalance + PsbStockBalance + SgTenderStockBalance + inventoryQty.GetValueOrDefault(0) + inTransitQty.Value),
                            StockHoldingPackSize = (interCompanyTransitQty.GetValueOrDefault(0) + notStartInvQty.GetValueOrDefault(0) + wipQty.GetValueOrDefault(0) + 0 + globalQty.GetValueOrDefault(0) + AntahStockBalance + ApexStockBalance + MsbStockBalance + PsbStockBalance + SgTenderStockBalance + inventoryQty.GetValueOrDefault(0) + inTransitQty.Value) * packSize,
                            StockHoldingBalance = stockHoldingBalance,
                            MyStockBalance = endDate.CompanyId == 1 || endDate.CompanyId == 3 ? inventoryQty.GetValueOrDefault(0) : SgTenderStockBalance,
                            SgStockBalance = endDate.CompanyId == 2 ? inventoryQty.GetValueOrDefault(0) : SgTenderStockBalance,
                            MethodCode = ac.NavMethodCodeLines.Count > 0 ? ac.NavMethodCodeLines.First().MethodName : "No MethodCode",
                            Month = endDate.StockMonth.ToString("MMMM"),
                            Remarks = "AC",
                            BatchSize90 = currentBatch,
                            Roundup1 = packSize2 > 0 ? threeMonth / packSize2 : 0,
                            Roundup2 = 0,
                            ReportMonth = endDate.StockMonth,
                            UOM = ac.BaseUnitofMeasure,
                            Packuom = ac.PackUom,
                            Replenishment = ac.VendorNo,
                            StatusCodeId = ac.StatusCodeId,
                            isTenderExist = isTenderExist,
                            Itemgrouping = distTotal > 0 ? "Item with AC" : "Item without AC",
                            //TenderSum = tenderSum,
                            //Month1 = stockHoldingBalance > 1 ? stockHoldingBalance - 1 : 0,
                            //Month2 = stockHoldingBalance > 2 ? stockHoldingBalance - 2 : 0,
                            //Month3 = stockHoldingBalance > 3 ? stockHoldingBalance - 3 : 0,
                            //Month4 = stockHoldingBalance > 4 ? stockHoldingBalance - 4 : 0,
                            //Month5 = stockHoldingBalance > 5 ? stockHoldingBalance - 5 : 0,
                            //Month6 = stockHoldingBalance > 6 ? stockHoldingBalance - 6 : 0,
                            //Month7 = stockHoldingBalance > 7 ? stockHoldingBalance - 7 : 0,
                            //Month8 = stockHoldingBalance > 8 ? stockHoldingBalance - 8 : 0,
                            //Month9 = stockHoldingBalance > 9 ? stockHoldingBalance - 9 : 0,
                            //Month10 = stockHoldingBalance > 10 ? stockHoldingBalance - 10 : 0,
                            //Month11 = stockHoldingBalance > 11 ? stockHoldingBalance - 11 : 0,
                            //Month12 = stockHoldingBalance > 12 ? stockHoldingBalance - 12 : 0,

                            Month1 = stockHoldingBalance - 1,
                            Month2 = stockHoldingBalance - 2,
                            Month3 = stockHoldingBalance - 3,
                            Month4 = stockHoldingBalance - 4,
                            Month5 = stockHoldingBalance - 5,
                            Month6 = stockHoldingBalance - 6,
                            Month7 = stockHoldingBalance - 7,
                            Month8 = stockHoldingBalance - 8,
                            Month9 = stockHoldingBalance - 9,
                            Month10 = stockHoldingBalance - 10,
                            Month11 = stockHoldingBalance - 11,
                            Month12 = stockHoldingBalance - 12,
                            DeliverynotReceived = inTransitQty,//doNotReceivedList.Where(d => d.ItemNo == ac.No && d.IsRecived == false).Sum(s => s.DoQty),


                            GroupItemTicket1 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month1 && t.StartDate.Value.Year == year1).Select(s => s.Quantity).ToList()),
                            GroupItemTicket2 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month2 && t.StartDate.Value.Year == year2).Select(s => s.Quantity).ToList()),
                            GroupItemTicket3 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month3 && t.StartDate.Value.Year == year3).Select(s => s.Quantity).ToList()),
                            GroupItemTicket4 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month4 && t.StartDate.Value.Year == year4).Select(s => s.Quantity).ToList()),
                            GroupItemTicket5 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month5 && t.StartDate.Value.Year == year5).Select(s => s.Quantity).ToList()),
                            GroupItemTicket6 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month6 && t.StartDate.Value.Year == year6).Select(s => s.Quantity).ToList()),
                            GroupItemTicket7 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month7 && t.StartDate.Value.Year == year7).Select(s => s.Quantity).ToList()),
                            GroupItemTicket8 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month8 && t.StartDate.Value.Year == year8).Select(s => s.Quantity).ToList()),
                            GroupItemTicket9 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month9 && t.StartDate.Value.Year == year9).Select(s => s.Quantity).ToList()),
                            GroupItemTicket10 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month10 && t.StartDate.Value.Year == year10).Select(s => s.Quantity).ToList()),
                            GroupItemTicket11 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month11 && t.StartDate.Value.Year == year11).Select(s => s.Quantity).ToList()),
                            GroupItemTicket12 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month12 && t.StartDate.Value.Year == year12).Select(s => s.Quantity).ToList()),

                            NoOfTicket1 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month1 && t.StartDate.Value.Year == year1).Select(s => s.NoOfTicket).ToList()),
                            NoOfTicket2 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month2 && t.StartDate.Value.Year == year2).Select(s => s.NoOfTicket).ToList()),
                            NoOfTicket3 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month3 && t.StartDate.Value.Year == year3).Select(s => s.NoOfTicket).ToList()),
                            NoOfTicket4 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month4 && t.StartDate.Value.Year == year4).Select(s => s.NoOfTicket).ToList()),
                            NoOfTicket5 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month5 && t.StartDate.Value.Year == year5).Select(s => s.NoOfTicket).ToList()),
                            NoOfTicket6 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month6 && t.StartDate.Value.Year == year6).Select(s => s.NoOfTicket).ToList()),
                            NoOfTicket7 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month7 && t.StartDate.Value.Year == year7).Select(s => s.NoOfTicket).ToList()),
                            NoOfTicket8 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month8 && t.StartDate.Value.Year == year8).Select(s => s.NoOfTicket).ToList()),
                            NoOfTicket9 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month9 && t.StartDate.Value.Year == year9).Select(s => s.NoOfTicket).ToList()),
                            NoOfTicket10 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month10 && t.StartDate.Value.Year == year10).Select(s => s.NoOfTicket).ToList()),
                            NoOfTicket11 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month11 && t.StartDate.Value.Year == year11).Select(s => s.NoOfTicket).ToList()),
                            NoOfTicket12 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month12 && t.StartDate.Value.Year == year12).Select(s => s.NoOfTicket).ToList()),



                            Ticket1 = string.Format("{0}/{1}", string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month1 && t.StartDate.Value.Year == year1).Select(s => s.Quantity).ToList()), string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month1 && t.StartDate.Value.Year == year1).Select(s => s.NoOfTicket).ToList())),
                            Ticket2 = string.Format("{0}/{1}", string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month2 && t.StartDate.Value.Year == year2).Select(s => s.Quantity).ToList()), string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month2 && t.StartDate.Value.Year == year2).Select(s => s.NoOfTicket).ToList())),
                            Ticket3 = string.Format("{0}/{1}", string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month3 && t.StartDate.Value.Year == year3).Select(s => s.Quantity).ToList()), string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month3 && t.StartDate.Value.Year == year3).Select(s => s.NoOfTicket).ToList())),
                            Ticket4 = string.Format("{0}/{1}", string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month4 && t.StartDate.Value.Year == year4).Select(s => s.Quantity).ToList()), string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month4 && t.StartDate.Value.Year == year4).Select(s => s.NoOfTicket).ToList())),
                            Ticket5 = string.Format("{0}/{1}", string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month5 && t.StartDate.Value.Year == year5).Select(s => s.Quantity).ToList()), string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month5 && t.StartDate.Value.Year == year5).Select(s => s.NoOfTicket).ToList())),
                            Ticket6 = string.Format("{0}/{1}", string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month6 && t.StartDate.Value.Year == year6).Select(s => s.Quantity).ToList()), string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month6 && t.StartDate.Value.Year == year6).Select(s => s.NoOfTicket).ToList())),
                            Ticket7 = string.Format("{0}/{1}", string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month7 && t.StartDate.Value.Year == year7).Select(s => s.Quantity).ToList()), string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month7 && t.StartDate.Value.Year == year7).Select(s => s.NoOfTicket).ToList())),
                            Ticket8 = string.Format("{0}/{1}", string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month8 && t.StartDate.Value.Year == year8).Select(s => s.Quantity).ToList()), string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month8 && t.StartDate.Value.Year == year8).Select(s => s.NoOfTicket).ToList())),
                            Ticket9 = string.Format("{0}/{1}", string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month9 && t.StartDate.Value.Year == year9).Select(s => s.Quantity).ToList()), string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month9 && t.StartDate.Value.Year == year9).Select(s => s.NoOfTicket).ToList())),
                            Ticket10 = string.Format("{0}/{1}", string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month10 && t.StartDate.Value.Year == year10).Select(s => s.Quantity).ToList()), string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month10 && t.StartDate.Value.Year == year10).Select(s => s.NoOfTicket).ToList())),
                            Ticket11 = string.Format("{0}/{1}", string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month11 && t.StartDate.Value.Year == year11).Select(s => s.Quantity).ToList()), string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month11 && t.StartDate.Value.Year == year11).Select(s => s.NoOfTicket).ToList())),
                            Ticket12 = string.Format("{0}/{1}", string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month12 && t.StartDate.Value.Year == year12).Select(s => s.Quantity).ToList()), string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month12 && t.StartDate.Value.Year == year12).Select(s => s.NoOfTicket).ToList())),

                            OutputTicket1 = string.Format("{0}/{1}", string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month1).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month1).Select(s => s.ProdOrderNo).ToList())),
                            OutputTicket2 = string.Format("{0}/{1}", string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month2).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month2).Select(s => s.ProdOrderNo).ToList())),
                            OutputTicket3 = string.Format("{0}/{1}", string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month3).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month3).Select(s => s.ProdOrderNo).ToList())),
                            OutputTicket4 = string.Format("{0}/{1}", string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month4).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month4).Select(s => s.ProdOrderNo).ToList())),
                            OutputTicket5 = string.Format("{0}/{1}", string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month5).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month5).Select(s => s.ProdOrderNo).ToList())),
                            OutputTicket6 = string.Format("{0}/{1}", string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month6).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month6).Select(s => s.ProdOrderNo).ToList())),
                            OutputTicket7 = string.Format("{0}/{1}", string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month7).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month7).Select(s => s.ProdOrderNo).ToList())),
                            OutputTicket8 = string.Format("{0}/{1}", string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month8).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month8).Select(s => s.ProdOrderNo).ToList())),
                            OutputTicket9 = string.Format("{0}/{1}", string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month9).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month9).Select(s => s.ProdOrderNo).ToList())),
                            OutputTicket10 = string.Format("{0}/{1}", string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month10).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month10).Select(s => s.ProdOrderNo).ToList())),
                            OutputTicket11 = string.Format("{0}/{1}", string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month11).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month11).Select(s => s.ProdOrderNo).ToList())),
                            OutputTicket12 = string.Format("{0}/{1}", string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month12).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month12).Select(s => s.ProdOrderNo).ToList())),

                            TicketHoldingStock1 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month1).Sum(s => s.NoOfTicket.GetValueOrDefault(0)),
                            TicketHoldingStock2 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month2).Sum(s => s.NoOfTicket.GetValueOrDefault(0)),
                            TicketHoldingStock3 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month3).Sum(s => s.NoOfTicket.GetValueOrDefault(0)),
                            TicketHoldingStock4 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month4).Sum(s => s.NoOfTicket.GetValueOrDefault(0)),
                            TicketHoldingStock5 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month5).Sum(s => s.NoOfTicket.GetValueOrDefault(0)),
                            TicketHoldingStock6 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month6).Sum(s => s.NoOfTicket.GetValueOrDefault(0)),
                            TicketHoldingStock7 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month7).Sum(s => s.NoOfTicket.GetValueOrDefault(0)),
                            TicketHoldingStock8 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month8).Sum(s => s.NoOfTicket.GetValueOrDefault(0)),
                            TicketHoldingStock9 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month9).Sum(s => s.NoOfTicket.GetValueOrDefault(0)),
                            TicketHoldingStock10 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month10).Sum(s => s.NoOfTicket.GetValueOrDefault(0)),
                            TicketHoldingStock11 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month11).Sum(s => s.NoOfTicket.GetValueOrDefault(0)),
                            TicketHoldingStock12 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month12).Sum(s => s.NoOfTicket.GetValueOrDefault(0)),

                            ProductionTicket1 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month1).Sum(s => s.ProductQty.GetValueOrDefault(0)),
                            ProductionTicket2 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month2).Sum(s => s.ProductQty.GetValueOrDefault(0)),
                            ProductionTicket3 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month3).Sum(s => s.ProductQty.GetValueOrDefault(0)),
                            ProductionTicket4 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month4).Sum(s => s.ProductQty.GetValueOrDefault(0)),
                            ProductionTicket5 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month5).Sum(s => s.ProductQty.GetValueOrDefault(0)),
                            ProductionTicket6 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month6).Sum(s => s.ProductQty.GetValueOrDefault(0)),
                            ProductionTicket7 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month7).Sum(s => s.ProductQty.GetValueOrDefault(0)),
                            ProductionTicket8 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month8).Sum(s => s.ProductQty.GetValueOrDefault(0)),
                            ProductionTicket9 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month9).Sum(s => s.ProductQty.GetValueOrDefault(0)),
                            ProductionTicket10 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month10).Sum(s => s.ProductQty.GetValueOrDefault(0)),
                            ProductionTicket11 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month11).Sum(s => s.ProductQty.GetValueOrDefault(0)),
                            ProductionTicket12 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month12).Sum(s => s.ProductQty.GetValueOrDefault(0)),



                            IsTenderExist1 = tenderExist1,//blanletOrders.Any(t => itemNos.Contains(t.ItemId.Value) && t.ShipmentDate.Value.Month == month1),
                            IsTenderExist2 = tenderExist2,// blanletOrders.Any(t => itemNos.Contains(t.ItemId.Value) && t.ShipmentDate.Value.Month == month2),
                            IsTenderExist3 = tenderExist3,// blanletOrders.Any(t => itemNos.Contains(t.ItemId.Value) && t.ShipmentDate.Value.Month == month3),
                            IsTenderExist4 = tenderExist4,//blanletOrders.Any(t => itemNos.Contains(t.ItemId.Value) && t.ShipmentDate.Value.Month == month4),
                            IsTenderExist5 = tenderExist5,//blanletOrders.Any(t => itemNos.Contains(t.ItemId.Value) && t.ShipmentDate.Value.Month == month5),
                            IsTenderExist6 = tenderExist6,//blanletOrders.Any(t => itemNos.Contains(t.ItemId.Value) && t.ShipmentDate.Value.Month == month6),
                            IsTenderExist7 = tenderExist7,//blanletOrders.Any(t => itemNos.Contains(t.ItemId.Value) && t.ShipmentDate.Value.Month == month7),
                            IsTenderExist8 = tenderExist8,//blanletOrders.Any(t => itemNos.Contains(t.ItemId.Value) && t.ShipmentDate.Value.Month == month8),
                            IsTenderExist9 = tenderExist9,//blanletOrders.Any(t => itemNos.Contains(t.ItemId.Value) && t.ShipmentDate.Value.Month == month9),
                            IsTenderExist10 = tenderExist10,//blanletOrders.Any(t => itemNos.Contains(t.ItemId.Value) && t.ShipmentDate.Value.Month == month10),
                            IsTenderExist11 = tenderExist11,//blanletOrders.Any(t => itemNos.Contains(t.ItemId.Value) && t.ShipmentDate.Value.Month == month11),
                            IsTenderExist12 = tenderExist12,//blanletOrders.Any(t => itemNos.Contains(t.ItemId.Value) && t.ShipmentDate.Value.Month == month12),


                            ProjectedHoldingStock1 = distTotal > 0 ? prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month1).Sum(s => s.Quantity) / (distTotal) : prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month1).Sum(s => s.Quantity),
                            ProjectedHoldingStock2 = distTotal > 0 ? prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month2).Sum(s => s.Quantity) / (distTotal) : prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month2).Sum(s => s.Quantity),
                            ProjectedHoldingStock3 = distTotal > 0 ? prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month3).Sum(s => s.Quantity) / (distTotal) : prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month3).Sum(s => s.Quantity),
                            ProjectedHoldingStock4 = distTotal > 0 ? prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month4).Sum(s => s.Quantity) / (distTotal) : prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month4).Sum(s => s.Quantity),
                            ProjectedHoldingStock5 = distTotal > 0 ? prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month5).Sum(s => s.Quantity) / (distTotal) : prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month5).Sum(s => s.Quantity),
                            ProjectedHoldingStock6 = distTotal > 0 ? prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month6).Sum(s => s.Quantity) / (distTotal) : prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month6).Sum(s => s.Quantity),
                            ProjectedHoldingStock7 = distTotal > 0 ? prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month7).Sum(s => s.Quantity) / (distTotal) : prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month7).Sum(s => s.Quantity),
                            ProjectedHoldingStock8 = distTotal > 0 ? prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month8).Sum(s => s.Quantity) / (distTotal) : prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month8).Sum(s => s.Quantity),
                            ProjectedHoldingStock9 = distTotal > 0 ? prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month9).Sum(s => s.Quantity) / (distTotal) : prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month9).Sum(s => s.Quantity),
                            ProjectedHoldingStock10 = distTotal > 0 ? prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month10).Sum(s => s.Quantity) / (distTotal) : prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month10).Sum(s => s.Quantity),
                            ProjectedHoldingStock11 = distTotal > 0 ? prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month11).Sum(s => s.Quantity) / (distTotal) : prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month11).Sum(s => s.Quantity),
                            ProjectedHoldingStock12 = distTotal > 0 ? prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month12).Sum(s => s.Quantity) / (distTotal) : prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month12).Sum(s => s.Quantity),

                            OutputProjectedHoldingStock1 = distTotal > 0 ? prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month1).Sum(s => s.Quantity) / (distTotal) : prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month1).Sum(s => s.Quantity),
                            OutputProjectedHoldingStock2 = distTotal > 0 ? prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month2).Sum(s => s.Quantity) / (distTotal) : prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month2).Sum(s => s.Quantity),
                            OutputProjectedHoldingStock3 = distTotal > 0 ? prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month3).Sum(s => s.Quantity) / (distTotal) : prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month3).Sum(s => s.Quantity),
                            OutputProjectedHoldingStock4 = distTotal > 0 ? prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month4).Sum(s => s.Quantity) / (distTotal) : prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month4).Sum(s => s.Quantity),
                            OutputProjectedHoldingStock5 = distTotal > 0 ? prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month5).Sum(s => s.Quantity) / (distTotal) : prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month5).Sum(s => s.Quantity),
                            OutputProjectedHoldingStock6 = distTotal > 0 ? prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month6).Sum(s => s.Quantity) / (distTotal) : prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month6).Sum(s => s.Quantity),
                            OutputProjectedHoldingStock7 = distTotal > 0 ? prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month7).Sum(s => s.Quantity) / (distTotal) : prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month7).Sum(s => s.Quantity),
                            OutputProjectedHoldingStock8 = distTotal > 0 ? prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month8).Sum(s => s.Quantity) / (distTotal) : prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month8).Sum(s => s.Quantity),
                            OutputProjectedHoldingStock9 = distTotal > 0 ? prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month9).Sum(s => s.Quantity) / (distTotal) : prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month9).Sum(s => s.Quantity),
                            OutputProjectedHoldingStock10 = distTotal > 0 ? prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month10).Sum(s => s.Quantity) / (distTotal) : prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month10).Sum(s => s.Quantity),
                            OutputProjectedHoldingStock11 = distTotal > 0 ? prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month11).Sum(s => s.Quantity) / (distTotal) : prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month11).Sum(s => s.Quantity),
                            OutputProjectedHoldingStock12 = distTotal > 0 ? prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month12).Sum(s => s.Quantity) / (distTotal) : prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month12).Sum(s => s.Quantity),


                            BlanketAddhoc1 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month1 && t.ShipmentDate.Value.Year == year1).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                            BlanketAddhoc2 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month2 && t.ShipmentDate.Value.Year == year2).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                            BlanketAddhoc3 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month3 && t.ShipmentDate.Value.Year == year3).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                            BlanketAddhoc4 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month4 && t.ShipmentDate.Value.Year == year4).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                            BlanketAddhoc5 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month5 && t.ShipmentDate.Value.Year == year5).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                            BlanketAddhoc6 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month6 && t.ShipmentDate.Value.Year == year6).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                            BlanketAddhoc7 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month7 && t.ShipmentDate.Value.Year == year7).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                            BlanketAddhoc8 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month8 && t.ShipmentDate.Value.Year == year8).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                            BlanketAddhoc9 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month9 && t.ShipmentDate.Value.Year == year9).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                            BlanketAddhoc10 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month10 && t.ShipmentDate.Value.Year == year10).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                            BlanketAddhoc11 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month11 && t.ShipmentDate.Value.Year == year11).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                            BlanketAddhoc12 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month12 && t.ShipmentDate.Value.Year == year12).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),


                        });
                    }
                }
            });
            MethodCodeList = MethodCodeList.Where(f => f.StatusCodeId == 1).ToList();

            MethodCodeList.ForEach(f =>
            {

                if (f.ItemNo.Contains("Folic Acid Tablet"))
                {

                }
                var itemNos = itemMasterforReport.Where(i => i.GenericCodeId == f.GenericCodeID).Select(s => s.ItemId).ToList();

                var item_Nos = itemMasterforReport.Where(i => i.GenericCodeId == f.GenericCodeID).Select(s => s.No).ToList();


                f.GroupTicket1 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month1).Select(s => s.Quantity).ToList());
                f.GroupTicket2 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month2).Select(s => s.Quantity).ToList());
                f.GroupTicket3 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month3).Select(s => s.Quantity).ToList());
                f.GroupTicket4 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month4).Select(s => s.Quantity).ToList());
                f.GroupTicket5 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month5).Select(s => s.Quantity).ToList());
                f.GroupTicket6 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month6).Select(s => s.Quantity).ToList());
                f.GroupTicket7 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month7).Select(s => s.Quantity).ToList());
                f.GroupTicket8 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month8).Select(s => s.Quantity).ToList());
                f.GroupTicket9 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month9).Select(s => s.Quantity).ToList());
                f.GroupTicket10 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month10).Select(s => s.Quantity).ToList());
                f.GroupTicket11 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month11).Select(s => s.Quantity).ToList());
                f.GroupTicket12 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month12).Select(s => s.Quantity).ToList());

                f.ProdOrderNo1 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month1).Select(s => s.ProdOrderNo).ToList());
                f.ProdOrderNo2 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month2).Select(s => s.ProdOrderNo).ToList());
                f.ProdOrderNo3 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month3).Select(s => s.ProdOrderNo).ToList());
                f.ProdOrderNo4 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month4).Select(s => s.ProdOrderNo).ToList());
                f.ProdOrderNo5 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month5).Select(s => s.ProdOrderNo).ToList());
                f.ProdOrderNo6 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month6).Select(s => s.ProdOrderNo).ToList());
                f.ProdOrderNo7 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month7).Select(s => s.ProdOrderNo).ToList());
                f.ProdOrderNo8 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month8).Select(s => s.ProdOrderNo).ToList());
                f.ProdOrderNo9 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month9).Select(s => s.ProdOrderNo).ToList());
                f.ProdOrderNo10 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month10).Select(s => s.ProdOrderNo).ToList());
                f.ProdOrderNo11 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month11).Select(s => s.ProdOrderNo).ToList());
                f.ProdOrderNo12 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month12).Select(s => s.ProdOrderNo).ToList());

                f.ProTicket1 = string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month1).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month1).Select(s => s.ProdOrderNo).ToList()));
                f.ProTicket2 = string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month2).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month2).Select(s => s.ProdOrderNo).ToList()));
                f.ProTicket3 = string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month3).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month3).Select(s => s.ProdOrderNo).ToList()));
                f.ProTicket4 = string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month4).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month4).Select(s => s.ProdOrderNo).ToList()));
                f.ProTicket5 = string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month5).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month5).Select(s => s.ProdOrderNo).ToList()));
                f.ProTicket6 = string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month6).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month6).Select(s => s.ProdOrderNo).ToList()));
                f.ProTicket7 = string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month7).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month7).Select(s => s.ProdOrderNo).ToList()));
                f.ProTicket8 = string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month8).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month8).Select(s => s.ProdOrderNo).ToList()));
                f.ProTicket9 = string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month9).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month9).Select(s => s.ProdOrderNo).ToList()));
                f.ProTicket10 = string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month10).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month10).Select(s => s.ProdOrderNo).ToList()));
                f.ProTicket11 = string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month11).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month11).Select(s => s.ProdOrderNo).ToList()));
                f.ProTicket12 = string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month12).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month12).Select(s => s.ProdOrderNo).ToList()));


                f.Ticket1 = f.Ticket1 + f.ProTicket1;
                f.Ticket2 = f.Ticket2 + f.ProTicket2;
                f.Ticket3 = f.Ticket3 + f.ProTicket3;
                f.Ticket4 = f.Ticket4 + f.ProTicket4;
                f.Ticket5 = f.Ticket5 + f.ProTicket5;
                f.Ticket6 = f.Ticket6 + f.ProTicket6;
                f.Ticket7 = f.Ticket7 + f.ProTicket7;
                f.Ticket8 = f.Ticket8 + f.ProTicket8;
                f.Ticket9 = f.Ticket9 + f.ProTicket9;
                f.Ticket10 = f.Ticket10 + f.ProTicket10;
                f.Ticket11 = f.Ticket11 + f.ProTicket11;
                f.Ticket12 = f.Ticket12 + f.ProTicket12;

                /* f.Ticket1 = f.Ticket1 + string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month1).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month1).Select(s => s.ProdOrderNo).ToList()));
                 f.Ticket2 = f.Ticket2 + string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month2).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month2).Select(s => s.ProdOrderNo).ToList()));
                 f.Ticket3 = f.Ticket3 + string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month3).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month3).Select(s => s.ProdOrderNo).ToList()));
                 f.Ticket4 = f.Ticket4 + string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month4).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month4).Select(s => s.ProdOrderNo).ToList()));
                 f.Ticket5 = f.Ticket5 + string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month5).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month5).Select(s => s.ProdOrderNo).ToList()));
                 f.Ticket6 = f.Ticket6 + string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month6).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month6).Select(s => s.ProdOrderNo).ToList()));
                 f.Ticket7 = f.Ticket7 + string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month7).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month7).Select(s => s.ProdOrderNo).ToList()));
                 f.Ticket8 = f.Ticket8 + string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month8).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month8).Select(s => s.ProdOrderNo).ToList()));
                 f.Ticket9 = f.Ticket9 + string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month9).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month9).Select(s => s.ProdOrderNo).ToList()));
                 f.Ticket10 = f.Ticket10 + string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month10).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month10).Select(s => s.ProdOrderNo).ToList()));
                 f.Ticket11 = f.Ticket11 + string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month11).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month11).Select(s => s.ProdOrderNo).ToList()));
                 f.Ticket12 = f.Ticket12 + string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month12).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month12).Select(s => s.ProdOrderNo).ToList()));
 */

                f.ProjectedHoldingStock1 += f.AcSum > 0 ? grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month1 && t.StartDate.Value.Year == year1).Sum(s => s.TotalQuantity.Value) / (f.AcSum) : grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month1 && t.StartDate.Value.Year == year1).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStock2 += f.AcSum > 0 ? grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month2 && t.StartDate.Value.Year == year2).Sum(s => s.TotalQuantity.Value) / (f.AcSum) : grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month2 && t.StartDate.Value.Year == year2).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStock3 += f.AcSum > 0 ? grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month3 && t.StartDate.Value.Year == year3).Sum(s => s.TotalQuantity.Value) / (f.AcSum) : grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month3 && t.StartDate.Value.Year == year3).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStock4 += f.AcSum > 0 ? grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month4 && t.StartDate.Value.Year == year4).Sum(s => s.TotalQuantity.Value) / (f.AcSum) : grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month4 && t.StartDate.Value.Year == year4).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStock5 += f.AcSum > 0 ? grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month5 && t.StartDate.Value.Year == year5).Sum(s => s.TotalQuantity.Value) / (f.AcSum) : grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month5 && t.StartDate.Value.Year == year5).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStock6 += f.AcSum > 0 ? grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month6 && t.StartDate.Value.Year == year6).Sum(s => s.TotalQuantity.Value) / (f.AcSum) : grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month6 && t.StartDate.Value.Year == year6).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStock7 += f.AcSum > 0 ? grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month7 && t.StartDate.Value.Year == year7).Sum(s => s.TotalQuantity.Value) / (f.AcSum) : grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month7 && t.StartDate.Value.Year == year7).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStock8 += f.AcSum > 0 ? grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month8 && t.StartDate.Value.Year == year8).Sum(s => s.TotalQuantity.Value) / (f.AcSum) : grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month8 && t.StartDate.Value.Year == year8).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStock9 += f.AcSum > 0 ? grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month9 && t.StartDate.Value.Year == year9).Sum(s => s.TotalQuantity.Value) / (f.AcSum) : grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month9 && t.StartDate.Value.Year == year9).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStock10 += f.AcSum > 0 ? grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month10 && t.StartDate.Value.Year == year10).Sum(s => s.TotalQuantity.Value) / (f.AcSum) : grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month10 && t.StartDate.Value.Year == year10).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStock11 += f.AcSum > 0 ? grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month11 && t.StartDate.Value.Year == year11).Sum(s => s.TotalQuantity.Value) / (f.AcSum) : grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month11 && t.StartDate.Value.Year == year11).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStock12 += f.AcSum > 0 ? grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month12 && t.StartDate.Value.Year == year12).Sum(s => s.TotalQuantity.Value) / (f.AcSum) : grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month12 && t.StartDate.Value.Year == year12).Sum(s => s.TotalQuantity.Value);




                f.ProjectedHoldingStockQty1 = prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month1).Sum(s => s.Quantity);
                f.ProjectedHoldingStockQty2 = prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month2).Sum(s => s.Quantity);
                f.ProjectedHoldingStockQty3 = prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month3).Sum(s => s.Quantity);
                f.ProjectedHoldingStockQty4 = prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month4).Sum(s => s.Quantity);
                f.ProjectedHoldingStockQty5 = prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month5).Sum(s => s.Quantity);
                f.ProjectedHoldingStockQty6 = prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month6).Sum(s => s.Quantity);
                f.ProjectedHoldingStockQty7 = prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month7).Sum(s => s.Quantity);
                f.ProjectedHoldingStockQty8 = prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month8).Sum(s => s.Quantity);
                f.ProjectedHoldingStockQty9 = prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month9).Sum(s => s.Quantity);
                f.ProjectedHoldingStockQty10 = prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month10).Sum(s => s.Quantity);
                f.ProjectedHoldingStockQty11 = prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month11).Sum(s => s.Quantity);
                f.ProjectedHoldingStockQty12 = prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month12).Sum(s => s.Quantity);

                f.ProjectedHoldingStockQty1 += grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month1 && t.StartDate.Value.Year == year1).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStockQty2 += grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month2 && t.StartDate.Value.Year == year2).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStockQty3 += grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month3 && t.StartDate.Value.Year == year3).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStockQty4 += grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month4 && t.StartDate.Value.Year == year4).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStockQty5 += grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month5 && t.StartDate.Value.Year == year5).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStockQty6 += grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month6 && t.StartDate.Value.Year == year6).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStockQty7 += grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month7 && t.StartDate.Value.Year == year7).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStockQty8 += grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month8 && t.StartDate.Value.Year == year8).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStockQty9 += grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month9 && t.StartDate.Value.Year == year9).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStockQty10 += grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month10 && t.StartDate.Value.Year == year10).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStockQty11 += grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month11 && t.StartDate.Value.Year == year11).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStockQty12 += grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month12 && t.StartDate.Value.Year == year12).Sum(s => s.TotalQuantity.Value);

                f.OutputProjectedHoldingStockQty1 = prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month1).Sum(s => s.Quantity);
                f.OutputProjectedHoldingStockQty2 = prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month2).Sum(s => s.Quantity);
                f.OutputProjectedHoldingStockQty3 = prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month3).Sum(s => s.Quantity);
                f.OutputProjectedHoldingStockQty4 = prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month4).Sum(s => s.Quantity);
                f.OutputProjectedHoldingStockQty5 = prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month5).Sum(s => s.Quantity);
                f.OutputProjectedHoldingStockQty6 = prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month6).Sum(s => s.Quantity);
                f.OutputProjectedHoldingStockQty7 = prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month7).Sum(s => s.Quantity);
                f.OutputProjectedHoldingStockQty8 = prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month8).Sum(s => s.Quantity);
                f.OutputProjectedHoldingStockQty9 = prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month9).Sum(s => s.Quantity);
                f.OutputProjectedHoldingStockQty10 = prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month10).Sum(s => s.Quantity);
                f.OutputProjectedHoldingStockQty11 = prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month11).Sum(s => s.Quantity);
                f.OutputProjectedHoldingStockQty12 = prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month12).Sum(s => s.Quantity);



                f.TicketHoldingStock1 = f.ProductionTicket1 > 0 ? f.AcSum == 0 ? (f.ProductionTicket1) / 1 : (f.ProductionTicket1) / f.AcSum : 0;
                f.TicketHoldingStock2 = f.ProductionTicket2 > 0 ? f.AcSum == 0 ? (f.ProductionTicket2) / 1 : (f.ProductionTicket2) / f.AcSum : 0;
                f.TicketHoldingStock3 = f.ProductionTicket3 > 0 ? f.AcSum == 0 ? (f.ProductionTicket3) / 1 : (f.ProductionTicket3) / f.AcSum : 0;
                f.TicketHoldingStock4 = f.ProductionTicket4 > 0 ? f.AcSum == 0 ? (f.ProductionTicket4) / 1 : (f.ProductionTicket4) / f.AcSum : 0;
                f.TicketHoldingStock5 = f.ProductionTicket5 > 0 ? f.AcSum == 0 ? (f.ProductionTicket5) / 1 : (f.ProductionTicket5) / f.AcSum : 0;
                f.TicketHoldingStock6 = f.ProductionTicket6 > 0 ? f.AcSum == 0 ? (f.ProductionTicket6) / 1 : (f.ProductionTicket6) / f.AcSum : 0;
                f.TicketHoldingStock7 = f.ProductionTicket7 > 0 ? f.AcSum == 0 ? (f.ProductionTicket7) / 1 : (f.ProductionTicket7) / f.AcSum : 0;
                f.TicketHoldingStock8 = f.ProductionTicket8 > 0 ? f.AcSum == 0 ? (f.ProductionTicket8) / 1 : (f.ProductionTicket8) / f.AcSum : 0;
                f.TicketHoldingStock9 = f.ProductionTicket9 > 0 ? f.AcSum == 0 ? (f.ProductionTicket9) / 1 : (f.ProductionTicket9) / f.AcSum : 0;
                f.TicketHoldingStock10 = f.ProductionTicket10 > 0 ? f.AcSum == 0 ? (f.ProductionTicket10) / 1 : (f.ProductionTicket10) / f.AcSum : 0;
                f.TicketHoldingStock11 = f.ProductionTicket11 > 0 ? f.AcSum == 0 ? (f.ProductionTicket11) / 1 : (f.ProductionTicket11) / f.AcSum : 0;
                f.TicketHoldingStock12 = f.ProductionTicket12 > 0 ? f.AcSum == 0 ? (f.ProductionTicket12) / 1 : (f.ProductionTicket12) / f.AcSum : 0;



                //if (f.isTenderExist)
                //{
                //    f.StockHoldingBalance = 0;
                //}

                //f.QtyMonth1 = (f.StockBalance - f.AcSum) > 0 ? f.StockBalance - f.AcSum : 0;
                //f.QtyProductionProjected1 = (f.QtyMonth1 + f.ProjectedHoldingStockQty1 + f.OutputProjectedHoldingStockQty1) - f.BlanketAddhoc1;

                //f.QtyMonth2 = (f.QtyProductionProjected1 - f.AcSum) > 0 ? f.QtyProductionProjected1 - f.AcSum : 0;
                //f.QtyProductionProjected2 = (f.QtyMonth2 + f.ProjectedHoldingStockQty2 + f.OutputProjectedHoldingStockQty2) - f.BlanketAddhoc2;

                //f.QtyMonth3 = (f.QtyProductionProjected2 - f.AcSum) > 0 ? f.QtyProductionProjected2 - f.AcSum : 0;
                //f.QtyProductionProjected3 = (f.QtyMonth3 + f.ProjectedHoldingStockQty3 + f.OutputProjectedHoldingStockQty3) - f.BlanketAddhoc3;

                //f.QtyMonth4 = (f.QtyProductionProjected3 - f.AcSum) > 0 ? f.QtyProductionProjected3 - f.AcSum : 0;
                //f.QtyProductionProjected4 = (f.QtyMonth4 + f.ProjectedHoldingStockQty4 + f.OutputProjectedHoldingStockQty4) - f.BlanketAddhoc4;

                //f.QtyMonth5 = (f.QtyProductionProjected4 - f.AcSum) > 0 ? f.QtyProductionProjected4 - f.AcSum : 0;
                //f.QtyProductionProjected5 = (f.QtyMonth5 + f.ProjectedHoldingStockQty5 + f.OutputProjectedHoldingStockQty5) - f.BlanketAddhoc5;

                //f.QtyMonth6 = (f.QtyProductionProjected5 - f.AcSum) > 0 ? f.QtyProductionProjected5 - f.AcSum : 0;
                //f.QtyProductionProjected6 = (f.QtyMonth6 + f.ProjectedHoldingStockQty6 + f.OutputProjectedHoldingStockQty6) - f.BlanketAddhoc6;

                //f.QtyMonth7 = (f.QtyProductionProjected6 - f.AcSum) > 0 ? f.QtyProductionProjected6 - f.AcSum : 0;
                //f.QtyProductionProjected7 = (f.QtyMonth7 + f.ProjectedHoldingStockQty7 + f.OutputProjectedHoldingStockQty7) - f.BlanketAddhoc7;

                //f.QtyMonth8 = (f.QtyProductionProjected7 - f.AcSum) > 0 ? f.QtyProductionProjected7 - f.AcSum : 0;
                //f.QtyProductionProjected8 = (f.QtyMonth8 + f.ProjectedHoldingStockQty8 + f.OutputProjectedHoldingStockQty8) - f.BlanketAddhoc8;

                //f.QtyMonth9 = (f.QtyProductionProjected8 - f.AcSum) > 0 ? f.QtyProductionProjected8 - f.AcSum : 0;
                //f.QtyProductionProjected9 = (f.QtyMonth9 + f.ProjectedHoldingStockQty9 + f.OutputProjectedHoldingStockQty9) - f.BlanketAddhoc9;

                //f.QtyMonth10 = (f.QtyProductionProjected9 - f.AcSum) > 0 ? f.QtyProductionProjected9 - f.AcSum : 0;
                //f.QtyProductionProjected10 = (f.QtyMonth10 + f.ProjectedHoldingStockQty10 + f.OutputProjectedHoldingStockQty10) - f.BlanketAddhoc10;

                //f.QtyMonth11 = (f.QtyProductionProjected10 - f.AcSum) > 0 ? f.QtyProductionProjected10 - f.AcSum : 0;
                //f.QtyProductionProjected11 = (f.QtyMonth11 + f.ProjectedHoldingStockQty11 + f.OutputProjectedHoldingStockQty11) - f.BlanketAddhoc11;

                //f.QtyMonth12 = (f.QtyProductionProjected11 - f.AcSum) > 0 ? f.QtyProductionProjected11 - f.AcSum : 0;
                //f.QtyProductionProjected12 = (f.QtyMonth12 + f.ProjectedHoldingStockQty12 + f.OutputProjectedHoldingStockQty12) - f.BlanketAddhoc12;


                f.QtyMonth1 = f.StockBalance - f.AcSum;
                f.QtyProductionProjected1 = (f.QtyMonth1 + f.ProjectedHoldingStockQty1 + f.OutputProjectedHoldingStockQty1) - f.BlanketAddhoc1;

                f.QtyMonth2 = f.QtyProductionProjected1 - f.AcSum;
                f.QtyProductionProjected2 = (f.QtyMonth2 + f.ProjectedHoldingStockQty2 + f.OutputProjectedHoldingStockQty2) - f.BlanketAddhoc2;

                f.QtyMonth3 = f.QtyProductionProjected2 - f.AcSum;
                f.QtyProductionProjected3 = (f.QtyMonth3 + f.ProjectedHoldingStockQty3 + f.OutputProjectedHoldingStockQty3) - f.BlanketAddhoc3;

                f.QtyMonth4 = f.QtyProductionProjected3 - f.AcSum;
                f.QtyProductionProjected4 = (f.QtyMonth4 + f.ProjectedHoldingStockQty4 + f.OutputProjectedHoldingStockQty4) - f.BlanketAddhoc4;

                f.QtyMonth5 = f.QtyProductionProjected4 - f.AcSum;
                f.QtyProductionProjected5 = (f.QtyMonth5 + f.ProjectedHoldingStockQty5 + f.OutputProjectedHoldingStockQty5) - f.BlanketAddhoc5;

                f.QtyMonth6 = f.QtyProductionProjected5 - f.AcSum;
                f.QtyProductionProjected6 = (f.QtyMonth6 + f.ProjectedHoldingStockQty6 + f.OutputProjectedHoldingStockQty6) - f.BlanketAddhoc6;

                f.QtyMonth7 = f.QtyProductionProjected6 - f.AcSum;
                f.QtyProductionProjected7 = (f.QtyMonth7 + f.ProjectedHoldingStockQty7 + f.OutputProjectedHoldingStockQty7) - f.BlanketAddhoc7;

                f.QtyMonth8 = f.QtyProductionProjected7 - f.AcSum;
                f.QtyProductionProjected8 = (f.QtyMonth8 + f.ProjectedHoldingStockQty8 + f.OutputProjectedHoldingStockQty8) - f.BlanketAddhoc8;

                f.QtyMonth9 = f.QtyProductionProjected8 - f.AcSum;
                f.QtyProductionProjected9 = (f.QtyMonth9 + f.ProjectedHoldingStockQty9 + f.OutputProjectedHoldingStockQty9) - f.BlanketAddhoc9;

                f.QtyMonth10 = f.QtyProductionProjected9 - f.AcSum;
                f.QtyProductionProjected10 = (f.QtyMonth10 + f.ProjectedHoldingStockQty10 + f.OutputProjectedHoldingStockQty10) - f.BlanketAddhoc10;

                f.QtyMonth11 = f.QtyProductionProjected10 - f.AcSum;
                f.QtyProductionProjected11 = (f.QtyMonth11 + f.ProjectedHoldingStockQty11 + f.OutputProjectedHoldingStockQty11) - f.BlanketAddhoc11;

                f.QtyMonth12 = f.QtyProductionProjected11 - f.AcSum;
                f.QtyProductionProjected12 = (f.QtyMonth12 + f.ProjectedHoldingStockQty12 + f.OutputProjectedHoldingStockQty12) - f.BlanketAddhoc12;

                //f.ProductionProjected1 = f.Month1 + f.ProjectedHoldingStock1 + f.TicketHoldingStock1 + f.OutputProjectedHoldingStock1;
                //f.Month2 = f.AcSum > 0 ? f.ProductionProjected1 - 1 > 0 ? f.ProductionProjected1 - 1 : 0 : f.ProductionProjected1;
                //f.ProductionProjected2 = f.Month2 + f.ProjectedHoldingStock2 + f.TicketHoldingStock2 + f.OutputProjectedHoldingStock2;
                //f.Month3 = f.AcSum > 0 ? f.ProductionProjected2 - 1 > 0 ? f.ProductionProjected2 - 1 : 0 : f.ProductionProjected2;
                //f.ProductionProjected3 = f.Month3 + f.ProjectedHoldingStock3 + f.TicketHoldingStock3 + f.OutputProjectedHoldingStock3;
                //f.Month4 = f.AcSum > 0 ? f.ProductionProjected3 - 1 > 0 ? f.ProductionProjected3 - 1 : 0 : f.ProductionProjected3;
                //f.ProductionProjected4 = f.Month4 + f.ProjectedHoldingStock4 + f.TicketHoldingStock4 + f.OutputProjectedHoldingStock4;
                //f.Month5 = f.AcSum > 0 ? f.ProductionProjected4 - 1 > 0 ? f.ProductionProjected4 - 1 : 0 : f.ProductionProjected4;
                //f.ProductionProjected5 = f.Month5 + f.ProjectedHoldingStock5 + f.TicketHoldingStock5 + f.OutputProjectedHoldingStock5;
                //f.Month6 = f.AcSum > 0 ? f.ProductionProjected5 - 1 > 0 ? f.ProductionProjected5 - 1 : 0 : f.ProductionProjected5;
                //f.ProductionProjected6 = f.Month6 + f.ProjectedHoldingStock6 + f.TicketHoldingStock6 + f.OutputProjectedHoldingStock6;
                //f.Month7 = f.AcSum > 0 ? f.ProductionProjected6 - 1 > 0 ? f.ProductionProjected6 - 1 : 0 : f.ProductionProjected6;
                //f.ProductionProjected7 = f.Month7 + f.ProjectedHoldingStock7 + f.TicketHoldingStock7 + f.OutputProjectedHoldingStock7;
                //f.Month8 = f.AcSum > 0 ? f.ProductionProjected7 - 1 > 0 ? f.ProductionProjected7 - 1 : 0 : f.ProductionProjected7;
                //f.ProductionProjected8 = f.Month8 + f.ProjectedHoldingStock8 + f.TicketHoldingStock8 + f.OutputProjectedHoldingStock8;
                //f.Month9 = f.AcSum > 0 ? f.ProductionProjected8 - 1 > 0 ? f.ProductionProjected8 - 1 : 0 : f.ProductionProjected8;
                //f.ProductionProjected9 = f.Month9 + f.ProjectedHoldingStock9 + f.TicketHoldingStock9 + f.OutputProjectedHoldingStock9;
                //f.Month10 = f.AcSum > 0 ? f.ProductionProjected9 - 1 > 0 ? f.ProductionProjected9 - 1 : 0 : f.ProductionProjected9;
                //f.ProductionProjected10 = f.Month10 + f.ProjectedHoldingStock10 + f.TicketHoldingStock10 + f.OutputProjectedHoldingStock10;
                //f.Month11 = f.AcSum > 0 ? f.ProductionProjected10 - 1 > 0 ? f.ProductionProjected10 - 1 : 0 : f.ProductionProjected10;
                //f.ProductionProjected11 = f.Month11 + f.ProjectedHoldingStock11 + f.TicketHoldingStock11 + f.OutputProjectedHoldingStock11;
                //f.Month12 = f.AcSum > 0 ? f.ProductionProjected11 - 1 > 0 ? f.ProductionProjected11 - 1 : 0 : f.ProductionProjected11;
                //f.ProductionProjected12 = f.Month12 + f.ProjectedHoldingStock12 + f.TicketHoldingStock12 + f.OutputProjectedHoldingStock12;

                f.ProductionProjected1 = f.Month1 + f.ProjectedHoldingStock1 + 0 + f.OutputProjectedHoldingStock1;
                f.Month2 = f.AcSum > 0 ? f.ProductionProjected1 - 1 > 0 ? f.ProductionProjected1 - 1 : 0 : f.ProductionProjected1;
                f.ProductionProjected2 = f.Month2 + f.ProjectedHoldingStock2 + 0 + f.OutputProjectedHoldingStock2;
                f.Month3 = f.AcSum > 0 ? f.ProductionProjected2 - 1 > 0 ? f.ProductionProjected2 - 1 : 0 : f.ProductionProjected2;
                f.ProductionProjected3 = f.Month3 + f.ProjectedHoldingStock3 + 0 + f.OutputProjectedHoldingStock3;
                f.Month4 = f.AcSum > 0 ? f.ProductionProjected3 - 1 > 0 ? f.ProductionProjected3 - 1 : 0 : f.ProductionProjected3;
                f.ProductionProjected4 = f.Month4 + f.ProjectedHoldingStock4 + 0 + f.OutputProjectedHoldingStock4;
                f.Month5 = f.AcSum > 0 ? f.ProductionProjected4 - 1 > 0 ? f.ProductionProjected4 - 1 : 0 : f.ProductionProjected4;
                f.ProductionProjected5 = f.Month5 + f.ProjectedHoldingStock5 + 0 + f.OutputProjectedHoldingStock5;
                f.Month6 = f.AcSum > 0 ? f.ProductionProjected5 - 1 > 0 ? f.ProductionProjected5 - 1 : 0 : f.ProductionProjected5;
                f.ProductionProjected6 = f.Month6 + f.ProjectedHoldingStock6 + 0 + f.OutputProjectedHoldingStock6;
                f.Month7 = f.AcSum > 0 ? f.ProductionProjected6 - 1 > 0 ? f.ProductionProjected6 - 1 : 0 : f.ProductionProjected6;
                f.ProductionProjected7 = f.Month7 + f.ProjectedHoldingStock7 + 0 + f.OutputProjectedHoldingStock7;
                f.Month8 = f.AcSum > 0 ? f.ProductionProjected7 - 1 > 0 ? f.ProductionProjected7 - 1 : 0 : f.ProductionProjected7;
                f.ProductionProjected8 = f.Month8 + f.ProjectedHoldingStock8 + 0 + f.OutputProjectedHoldingStock8;
                f.Month9 = f.AcSum > 0 ? f.ProductionProjected8 - 1 > 0 ? f.ProductionProjected8 - 1 : 0 : f.ProductionProjected8;
                f.ProductionProjected9 = f.Month9 + f.ProjectedHoldingStock9 + 0 + f.OutputProjectedHoldingStock9;
                f.Month10 = f.AcSum > 0 ? f.ProductionProjected9 - 1 > 0 ? f.ProductionProjected9 - 1 : 0 : f.ProductionProjected9;
                f.ProductionProjected10 = f.Month10 + f.ProjectedHoldingStock10 + 0 + f.OutputProjectedHoldingStock10;
                f.Month11 = f.AcSum > 0 ? f.ProductionProjected10 - 1 > 0 ? f.ProductionProjected10 - 1 : 0 : f.ProductionProjected10;
                f.ProductionProjected11 = f.Month11 + f.ProjectedHoldingStock11 + 0 + f.OutputProjectedHoldingStock11;
                f.Month12 = f.AcSum > 0 ? f.ProductionProjected11 - 1 > 0 ? f.ProductionProjected11 - 1 : 0 : f.ProductionProjected11;
                f.ProductionProjected12 = f.Month12 + f.ProjectedHoldingStock12 + 0 + f.OutputProjectedHoldingStock12;




                if (f.IsTenderExist1 || f.AcSum <= 0)
                {
                    f.Month1 = f.QtyMonth1;
                    f.ProductionProjected1 = f.QtyProductionProjected1;
                    f.ProjectedHoldingStock1 = f.ProjectedHoldingStockQty1;
                    f.OutputProjectedHoldingStock1 = f.OutputProjectedHoldingStockQty1;
                }
                else
                {
                    if (f.isTenderExist)
                    {
                        f.Month1 = f.QtyMonth1 / f.AcSum;
                        f.ProductionProjected1 = f.QtyProductionProjected1 / f.AcSum;
                    }
                }
                if (f.IsTenderExist2 || f.AcSum <= 0)
                {
                    f.Month2 = f.QtyMonth2;
                    f.ProductionProjected2 = f.QtyProductionProjected2;
                    f.ProjectedHoldingStock2 = f.ProjectedHoldingStockQty2;
                    f.OutputProjectedHoldingStock2 = f.OutputProjectedHoldingStockQty2;
                }
                else
                {
                    if (f.isTenderExist)
                    {
                        f.Month2 = f.QtyMonth2 / f.AcSum;
                        f.ProductionProjected2 = f.QtyProductionProjected2 / f.AcSum;
                    }
                }
                if (f.IsTenderExist3 || f.AcSum <= 0)
                {
                    f.Month3 = f.QtyMonth3;
                    f.ProductionProjected3 = f.QtyProductionProjected3;
                    f.ProjectedHoldingStock3 = f.ProjectedHoldingStockQty3;
                    f.OutputProjectedHoldingStock3 = f.OutputProjectedHoldingStockQty3;
                }
                else
                {
                    if (f.isTenderExist)
                    {
                        f.Month3 = f.QtyMonth3 / f.AcSum;
                        f.ProductionProjected3 = f.QtyProductionProjected3 / f.AcSum;
                    }
                }
                if (f.IsTenderExist4 || f.AcSum <= 0)
                {
                    f.Month4 = f.QtyMonth4;
                    f.ProductionProjected4 = f.QtyProductionProjected4;
                    f.ProjectedHoldingStock4 = f.ProjectedHoldingStockQty4;
                    f.OutputProjectedHoldingStock4 = f.OutputProjectedHoldingStockQty4;
                }
                else
                {
                    if (f.isTenderExist)
                    {
                        f.Month4 = f.QtyMonth4 / f.AcSum;
                        f.ProductionProjected4 = f.QtyProductionProjected4 / f.AcSum;
                    }
                }
                if (f.IsTenderExist5 || f.AcSum <= 0)
                {
                    f.Month5 = f.QtyMonth5;
                    f.ProductionProjected5 = f.QtyProductionProjected5;
                    f.ProjectedHoldingStock5 = f.ProjectedHoldingStockQty5;
                    f.OutputProjectedHoldingStock5 = f.OutputProjectedHoldingStockQty5;
                }
                else
                {
                    if (f.isTenderExist)
                    {
                        f.Month5 = f.QtyMonth5 / f.AcSum;
                        f.ProductionProjected5 = f.QtyProductionProjected5 / f.AcSum;
                    }
                }
                if (f.IsTenderExist6 || f.AcSum <= 0)
                {
                    f.Month6 = f.QtyMonth6;
                    f.ProductionProjected6 = f.QtyProductionProjected6;
                    f.ProjectedHoldingStock6 = f.ProjectedHoldingStockQty6;
                    f.OutputProjectedHoldingStock6 = f.OutputProjectedHoldingStockQty6;
                }
                else
                {
                    if (f.isTenderExist)
                    {
                        f.Month6 = f.QtyMonth6 / f.AcSum;
                        f.ProductionProjected6 = f.QtyProductionProjected6 / f.AcSum;
                    }
                }
                if (f.IsTenderExist7 || f.AcSum <= 0)
                {
                    f.Month7 = f.QtyMonth7;
                    f.ProductionProjected7 = f.QtyProductionProjected7;
                    f.ProjectedHoldingStock7 = f.ProjectedHoldingStockQty7;
                    f.OutputProjectedHoldingStock7 = f.OutputProjectedHoldingStockQty7;
                }
                else
                {
                    if (f.isTenderExist)
                    {
                        f.Month7 = f.QtyMonth7 / f.AcSum;
                        f.ProductionProjected7 = f.QtyProductionProjected7 / f.AcSum;
                    }
                }
                if (f.IsTenderExist8 || f.AcSum <= 0)
                {
                    f.Month8 = f.QtyMonth8;
                    f.ProductionProjected8 = f.QtyProductionProjected8;
                    f.ProjectedHoldingStock8 = f.ProjectedHoldingStockQty8;
                    f.OutputProjectedHoldingStock8 = f.OutputProjectedHoldingStockQty8;
                }
                else
                {
                    if (f.isTenderExist)
                    {
                        f.Month8 = f.QtyMonth8 / f.AcSum;
                        f.ProductionProjected8 = f.QtyProductionProjected8 / f.AcSum;
                    }
                }
                if (f.IsTenderExist9 || f.AcSum <= 0)
                {
                    f.Month9 = f.QtyMonth9;
                    f.ProductionProjected9 = f.QtyProductionProjected9;
                    f.ProjectedHoldingStock9 = f.ProjectedHoldingStockQty9;
                    f.OutputProjectedHoldingStock9 = f.OutputProjectedHoldingStockQty9;
                }
                else
                {
                    if (f.isTenderExist)
                    {
                        f.Month9 = f.QtyMonth9 / f.AcSum;
                        f.ProductionProjected9 = f.QtyProductionProjected9 / f.AcSum;
                    }
                }
                if (f.IsTenderExist10 || f.AcSum <= 0)
                {
                    f.Month10 = f.QtyMonth10;
                    f.ProductionProjected10 = f.QtyProductionProjected10;
                    f.ProjectedHoldingStock10 = f.ProjectedHoldingStockQty10;
                    f.OutputProjectedHoldingStock10 = f.OutputProjectedHoldingStockQty10;
                }
                else
                {
                    if (f.isTenderExist)
                    {
                        f.Month10 = f.QtyMonth10 / f.AcSum;
                        f.ProductionProjected10 = f.QtyProductionProjected10 / f.AcSum;
                    }
                }
                if (f.IsTenderExist11 || f.AcSum <= 0)
                {
                    f.Month11 = f.QtyMonth11;
                    f.ProductionProjected11 = f.QtyProductionProjected11;
                    f.ProjectedHoldingStock11 = f.ProjectedHoldingStockQty11;
                }
                else
                {
                    if (f.isTenderExist)
                    {
                        f.Month11 = f.QtyMonth11 / f.AcSum;
                        f.ProductionProjected11 = f.QtyProductionProjected11 / f.AcSum;
                    }
                }
                if (f.IsTenderExist12 || f.AcSum <= 0)
                {
                    f.Month12 = f.QtyMonth12;
                    f.ProductionProjected12 = f.QtyProductionProjected12;
                    f.ProjectedHoldingStock12 = f.ProjectedHoldingStockQty12;
                    f.OutputProjectedHoldingStock12 = f.OutputProjectedHoldingStockQty12;
                }
                else
                {
                    if (f.isTenderExist)
                    {
                        f.Month12 = f.QtyMonth12 / f.AcSum;
                        f.ProductionProjected12 = f.QtyProductionProjected12 / f.AcSum;
                    }
                }

                //f.Month1 = f.Month1 < 0 ? 0 : f.Month1;
                //f.Month2 = f.Month2 < 0 ? 0 : f.Month2;
                //f.Month3 = f.Month3 < 0 ? 0 : f.Month3;
                //f.Month4 = f.Month4 < 0 ? 0 : f.Month4;
                //f.Month5 = f.Month5 < 0 ? 0 : f.Month5;
                //f.Month6 = f.Month6 < 0 ? 0 : f.Month6;
                //f.Month7 = f.Month7 < 0 ? 0 : f.Month7;
                //f.Month8 = f.Month8 < 0 ? 0 : f.Month8;
                //f.Month9 = f.Month9 < 0 ? 0 : f.Month9;
                //f.Month10 = f.Month10 < 0 ? 0 : f.Month10;
                //f.Month11 = f.Month11 < 0 ? 0 : f.Month11;
                //f.Month12 = f.Month12 < 0 ? 0 : f.Month12;

                //f.ProductionProjected1 = f.ProductionProjected1 < 0 ? 0 : f.ProductionProjected1;
                //f.ProductionProjected2 = f.ProductionProjected2 < 0 ? 0 : f.ProductionProjected2;
                //f.ProductionProjected3 = f.ProductionProjected3 < 0 ? 0 : f.ProductionProjected3;
                //f.ProductionProjected4 = f.ProductionProjected4 < 0 ? 0 : f.ProductionProjected4;
                //f.ProductionProjected5 = f.ProductionProjected5 < 0 ? 0 : f.ProductionProjected5;
                //f.ProductionProjected6 = f.ProductionProjected6 < 0 ? 0 : f.ProductionProjected6;
                //f.ProductionProjected7 = f.ProductionProjected7 < 0 ? 0 : f.ProductionProjected7;
                //f.ProductionProjected8 = f.ProductionProjected8 < 0 ? 0 : f.ProductionProjected8;
                //f.ProductionProjected9 = f.ProductionProjected9 < 0 ? 0 : f.ProductionProjected9;
                //f.ProductionProjected10 = f.ProductionProjected10 < 0 ? 0 : f.ProductionProjected10;
                //f.ProductionProjected11 = f.ProductionProjected11 < 0 ? 0 : f.ProductionProjected11;
                //f.ProductionProjected12 = f.ProductionProjected12 < 0 ? 0 : f.ProductionProjected12;

            });

            genericId = new List<long?>();
            //var customer = new List<string>();
            itemMasterforReport.ForEach(ac =>
            {
                var customer = new List<string>();

                var symlQty = itemdict.FirstOrDefault(f => f.Key == ac.No).Value;

                if (!genericId.Exists(g => g == ac.GenericCodeId) && ac.GenericCodeId != null)
                {
                    genericId.Add(ac.GenericCodeId);
                    var itemNos = itemMasterforReport.Where(i => i.GenericCodeId == ac.GenericCodeId).Select(s => s.ItemId).ToList();
                    // var acQty = MethodCodeList.Where(f => f.ItemId == ac.ItemId).Sum(s=>s.AcSum);
                    blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1))).ToList().ForEach(adhoc =>
                    {

                        if (!customer.Exists(c => c == adhoc.Categories) && !string.IsNullOrEmpty(adhoc.Categories))
                        {
                            customer.Add(adhoc.Categories);
                            MethodCodeList.Add(new INPCalendarPivotModel
                            {
                                ItemId = ac.ItemId,
                                ItemNo = ac.GenericCodeDescription2,
                                IsSteroid = ac.Steroid.GetValueOrDefault(false),
                                SalesCategoryId = ac.CategoryId,
                                SalesCategory = salesCatLists.Where(w => w.Id == ac.CategoryId).FirstOrDefault()?.Text,
                                LocationID = categoryItems != null && categoryItems.FirstOrDefault(a => a.SaleCategoryId == ac.CategoryId) != null ? categoryItems.FirstOrDefault(a => a.SaleCategoryId == ac.CategoryId).LocationId : null,
                                // SalesCategory = categoryItems != null && categoryItems.FirstOrDefault(a => a.SaleCategoryId == ac.CategoryId) != null ? categoryItems.FirstOrDefault(a => a.SaleCategoryId == ac.CategoryId).Code : "",
                                MethodCodeId = ac.NavMethodCodeLines.Count > 0 ? ac.NavMethodCodeLines.First().MethodCodeId : -1,
                                GenericCodeID = ac.GenericCodeId,
                                AddhocCust = adhoc.Categories,
                                Description = ac.Description + " " + ac.Description2,
                                MethodCode = ac.NavMethodCodeLines.Count > 0 ? ac.NavMethodCodeLines.First().MethodName : "No MethodCode",
                                Month = endDate.StockMonth.ToString("MMMM"),
                                Remarks = "Tender",
                                PackSize = ac.PackSize.HasValue ? ac.PackSize.Value : 900,
                                PackSize2 = packSize2,
                                BlanketAddhoc1 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.Categories == adhoc.Categories && t.ShipmentDate.Value.Month == month1 && t.ShipmentDate.Value.Year == year1).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                                BlanketAddhoc2 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.Categories == adhoc.Categories && t.ShipmentDate.Value.Month == month2 && t.ShipmentDate.Value.Year == year2).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                                BlanketAddhoc3 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.Categories == adhoc.Categories && t.ShipmentDate.Value.Month == month3 && t.ShipmentDate.Value.Year == year3).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                                BlanketAddhoc4 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.Categories == adhoc.Categories && t.ShipmentDate.Value.Month == month4 && t.ShipmentDate.Value.Year == year4).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                                BlanketAddhoc5 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.Categories == adhoc.Categories && t.ShipmentDate.Value.Month == month5 && t.ShipmentDate.Value.Year == year5).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                                BlanketAddhoc6 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.Categories == adhoc.Categories && t.ShipmentDate.Value.Month == month6 && t.ShipmentDate.Value.Year == year6).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                                BlanketAddhoc7 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.Categories == adhoc.Categories && t.ShipmentDate.Value.Month == month7 && t.ShipmentDate.Value.Year == year7).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                                BlanketAddhoc8 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.Categories == adhoc.Categories && t.ShipmentDate.Value.Month == month8 && t.ShipmentDate.Value.Year == year8).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                                BlanketAddhoc9 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.Categories == adhoc.Categories && t.ShipmentDate.Value.Month == month9 && t.ShipmentDate.Value.Year == year9).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                                BlanketAddhoc10 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.Categories == adhoc.Categories && t.ShipmentDate.Value.Month == month10 && t.ShipmentDate.Value.Year == year10).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                                BlanketAddhoc11 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.Categories == adhoc.Categories && t.ShipmentDate.Value.Month == month11 && t.ShipmentDate.Value.Year == year11).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                                BlanketAddhoc12 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.Categories == adhoc.Categories && t.ShipmentDate.Value.Month == month12 && t.ShipmentDate.Value.Year == year12).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                                isTenderExist = true,
                                SymlQty = symlQty,
                                Itemgrouping = symlQty > 0 ? "Adhoc orders - Item with AC" : "Stand alone"
                            });
                        }
                    });
                }

            });

            var groupedResult = MethodCodeList.GroupBy(g => g.MethodCode).ToList();
            var resultData = new List<INPCalendarPivotModel>();
            groupedResult.ForEach(f =>
            {
                if (f.Key.Contains("SW Vitamin C Tablet"))
                {

                }
                f.ToList().ForEach(g =>
                {
                    if (g.SymlQty > 0 && g.Remarks == "AC")
                    {
                        resultData.Add(g);
                    }
                    else
                    {
                        if (g.Remarks == "AC")
                        {
                            var tenderExist = f.Any(t => t.Remarks == "Tender" && t.ItemNo == g.ItemNo && (t.BlanketAddhoc1 > 0 || t.BlanketAddhoc2 > 0 || t.BlanketAddhoc3 > 0 || t.BlanketAddhoc4 > 0 || t.BlanketAddhoc5 > 0
                            || t.BlanketAddhoc6 > 0 || t.BlanketAddhoc7 > 0 || t.BlanketAddhoc8 > 0 || t.BlanketAddhoc9 > 0 || t.BlanketAddhoc10 > 0 || t.BlanketAddhoc11 > 0 || t.BlanketAddhoc12 > 0));
                            if (tenderExist)
                            {
                                resultData.Add(g);
                            }
                            else
                            {
                                if (g.ProjectedHoldingStock1 > 0 || g.ProjectedHoldingStock2 > 0 || g.ProjectedHoldingStock3 > 0 || g.ProjectedHoldingStock4 > 0 || g.ProjectedHoldingStock5 > 0
                      || g.ProjectedHoldingStock6 > 0 || g.ProjectedHoldingStock7 > 0 || g.ProjectedHoldingStock8 > 0 || g.ProjectedHoldingStock9 > 0 || g.ProjectedHoldingStock10 > 0 || g.ProjectedHoldingStock11 > 0 || g.ProjectedHoldingStock12 > 0)
                                {
                                    resultData.Add(g);
                                }
                            }
                        }
                        else
                        {
                            if (g.BlanketAddhoc1 > 0 || g.BlanketAddhoc2 > 0 || g.BlanketAddhoc3 > 0 || g.BlanketAddhoc4 > 0 || g.BlanketAddhoc5 > 0
                            || g.BlanketAddhoc6 > 0 || g.BlanketAddhoc7 > 0 || g.BlanketAddhoc8 > 0 || g.BlanketAddhoc9 > 0 || g.BlanketAddhoc10 > 0 || g.BlanketAddhoc11 > 0 || g.BlanketAddhoc12 > 0)
                            {
                                resultData.Add(g);
                            }
                        }
                    }
                });

            });

            return resultData.ToList();
        }
        public async Task<IReadOnlyList<INPCalendarPivotModel>> GetSimulationAddhocV4(DateRangeModel dateRangeModel)
        {
            var companyIds = new List<long?> { dateRangeModel.CompanyId };
            if (dateRangeModel.CompanyId == 3)
            {
                companyIds = new List<long?> { 1, 2 };
            }

            var methodCodeRecipes = new List<NAVRecipesModel>();

            List<NavmethodCodeBatch> methodCodeRecipe = new List<NavmethodCodeBatch>();
            List<ApplicationMasterDetail> applicationDetails = new List<ApplicationMasterDetail>();
            List<NAVRecipesModel> recipeList = new List<NAVRecipesModel>();
            using (var connection = CreateConnection())
            {
                var query = "SELECT * FROM NavmethodCodeBatch;";
                query += "select t1.* from ApplicationMasterDetail t1 JOIN ApplicationMaster t2 ON t1.ApplicationMasterID=t2.ApplicationMasterID where t2.ApplicationMasterCodeID=175;";
                query += "select RecipeNo,ItemNo,Description,BatchSize,ItemRecipeId,CONCAT(RecipeNo,'|',BatchSize) as RecipeName from Navrecipes Where CompanyId  in(" + string.Join(',', companyIds) + ") AND Status='Certified';";
                var results = await connection.QueryMultipleAsync(query);
                methodCodeRecipe = results.Read<NavmethodCodeBatch>().ToList();
                applicationDetails = results.Read<ApplicationMasterDetail>().ToList();
                recipeList = results.Read<NAVRecipesModel>().ToList();
            }

            methodCodeRecipe.ForEach(f =>
            {
                var BatchSize = applicationDetails.FirstOrDefault(b => b.ApplicationMasterDetailId == f.BatchSize)?.Value;
                var DefaultBatchSize = applicationDetails.FirstOrDefault(b => b.ApplicationMasterDetailId == f.DefaultBatchSize)?.Value;
                methodCodeRecipes.Add(new NAVRecipesModel
                {
                    RecipeNo = BatchSize,
                    BatchSize = BatchSize,// f.BatchUnitSize.GetValueOrDefault(0).ToString(),
                    ItemRecipeId = f.NavMethodCodeId,
                    UnitQTY = f.BatchUnitSize.GetValueOrDefault(0),
                    ItemNo = DefaultBatchSize,
                    DefaultBatch = DefaultBatchSize,
                    RecipeName = BatchSize,
                }); ;

            });


            var acreports = new List<INPCalendarPivotModel>();

            if (acreports.Count == 0)
            {
                acreports = await SimulationAddhocV44(dateRangeModel, methodCodeRecipes, recipeList);
            }
            if (dateRangeModel.MethodCodeId > 0)
                acreports = acreports.Where(r => r.MethodCodeId == dateRangeModel.MethodCodeId).ToList();
            if (dateRangeModel.SalesCategoryId > 0)
            {
                acreports = acreports.Where(r => r.SalesCategoryId == dateRangeModel.SalesCategoryId).ToList();
            }
            if (dateRangeModel.IsSteroid.HasValue)
            {
                acreports = acreports.Where(r => r.IsSteroid == dateRangeModel.IsSteroid.Value).ToList();
            }
            if (!string.IsNullOrEmpty(dateRangeModel.Replenishment))
            {
                acreports = acreports.Where(r => r.Replenishment.Contains(dateRangeModel.Replenishment)).ToList();
            }

            if (dateRangeModel.Ticketformula.GetValueOrDefault(0) > 0)
            {
                var ticketCondition = dateRangeModel.Ticketformula.GetValueOrDefault(0);
                var ticketValue = dateRangeModel.Ticketvalue.GetValueOrDefault(0);
                if (ticketCondition == 1)
                {
                    acreports = acreports.Where(r => r.Month1 == ticketValue || r.Month1 == ticketValue || r.Month2 == ticketValue || r.Month3 == ticketValue || r.Month4 == ticketValue || r.Month5 == ticketValue || r.Month6 == ticketValue).ToList();
                }
                else if (ticketCondition == 2)
                {
                    acreports = acreports.Where(r => r.Month1 > ticketValue || r.Month1 > ticketValue || r.Month2 > ticketValue || r.Month3 > ticketValue || r.Month4 > ticketValue || r.Month5 > ticketValue || r.Month6 > ticketValue).ToList();
                }
                else if (ticketCondition == 3)
                {
                    acreports = acreports.Where(r => r.Month1 >= ticketValue || r.Month1 >= ticketValue || r.Month2 >= ticketValue || r.Month3 >= ticketValue || r.Month4 >= ticketValue || r.Month5 >= ticketValue || r.Month6 >= ticketValue).ToList();
                }
                else if (ticketCondition == 4)
                {
                    acreports = acreports.Where(r => r.Month1 < ticketValue || r.Month1 < ticketValue || r.Month2 < ticketValue || r.Month3 < ticketValue || r.Month4 < ticketValue || r.Month5 < ticketValue || r.Month6 < ticketValue).ToList();
                }
                else if (ticketCondition == 5)
                {
                    acreports = acreports.Where(r => r.Month1 <= ticketValue || r.Month1 <= ticketValue || r.Month2 <= ticketValue || r.Month3 <= ticketValue || r.Month4 <= ticketValue || r.Month5 <= ticketValue || r.Month6 <= ticketValue).ToList();
                }
                else
                {
                    acreports = acreports.Where(r => r.Month1 != ticketValue || r.Month1 != ticketValue || r.Month2 != ticketValue || r.Month3 != ticketValue || r.Month4 != ticketValue || r.Month5 != ticketValue || r.Month6 != ticketValue).ToList();
                }
            }

            var packSize2 = 90000;
            if (!string.IsNullOrEmpty(dateRangeModel.Receipe))
            {
                string numberOnly = Regex.Replace(dateRangeModel.Receipe.Split("|")[0], "[^0-9.]", "");
                packSize2 = int.Parse(numberOnly) * 1000;

                acreports.Where(m => m.MethodCodeId == dateRangeModel.ChangeMethodCodeId).ToList().ForEach(f =>
                {
                    f.PackSize2 = packSize2;
                    f.ProdRecipe = dateRangeModel.Receipe.Split("|")[0];
                });

            }

            if (dateRangeModel.IsUpdate)
            {
                acreports.Where(m => m.ItemNo == dateRangeModel.ItemNo).ToList().ForEach(f =>
                {
                    f.Roundup2 = dateRangeModel.Roundup2;
                    f.Remarks = dateRangeModel.Remarks;
                });
            }
            if (acreports != null && acreports.Count() > 0)
            {
                acreports.ForEach(s =>
                {
                    s.IsNonSteroids = s.IsSteroid == false ? "Non Steroid" : "";
                    s.IsSteroids = s.IsSteroid == true ? "Steroid" : "Non Steroid";
                    s.ApexQty = s.ApexQty == 0 ? null : s.ApexQty;
                    s.AntahQty = s.AntahQty == 0 ? null : s.AntahQty;
                    s.MissQty = s.MissQty == 0 ? null : s.MissQty;
                    s.PxQty = s.PxQty == 0 ? null : s.PxQty;
                    s.DeliverynotReceived = s.DeliverynotReceived == 0 ? null : s.DeliverynotReceived;
                    s.SymlQty = s.SymlQty == 0 ? null : s.SymlQty;
                    s.Rework_ = s.Rework == 0 ? null : s.Rework;
                    s.AcSum_ = s.AcSum == 0 ? null : s.AcSum;
                    s.ThreeMonthACQty_ = s.ThreeMonthACQty == 0 ? null : s.ThreeMonthACQty;
                    s.Roundup1_ = s.Roundup1 == 0 ? null : s.Roundup1;
                    s.Roundup2_ = s.Roundup2 == 0 ? null : s.Roundup2;
                    s.PreApexStockBalance_ = s.PreApexStockBalance == 0 ? null : s.PreApexStockBalance;
                    s.PreAntahStockBalance_ = s.PreAntahStockBalance == 0 ? null : s.PreAntahStockBalance;
                    s.PreMsbStockBalance_ = s.PreMsbStockBalance == 0 ? null : s.PreMsbStockBalance;
                    s.PrePsbStockBalance_ = s.PrePsbStockBalance == 0 ? null : s.PrePsbStockBalance;
                    s.PreSgTenderStockBalance_ = s.PreSgTenderStockBalance == 0 ? null : s.PreSgTenderStockBalance;
                    s.WipQty_ = s.WipQty == 0 ? null : s.WipQty;
                    s.NotStartInvQty_ = s.NotStartInvQty == 0 ? null : s.NotStartInvQty;
                    s.PreMyStockBalance_ = s.PreMyStockBalance == 0 ? null : s.PreMyStockBalance;
                    s.PreOtherStoreQty_ = s.PreOtherStoreQty == 0 ? null : s.PreOtherStoreQty;
                    s.PrewipQty_ = s.PrewipQty == 0 ? null : s.PrewipQty;
                    s.PreStockBalance_ = s.PreStockBalance == 0 ? null : s.PreStockBalance;
                    s.PreStockHoldingBalance_ = s.PreStockHoldingBalance == 0 ? null : s.PreStockHoldingBalance;
                    s.ApexStockBalance_ = s.ApexStockBalance == 0 ? null : s.ApexStockBalance;
                    s.AntahStockBalance_ = s.AntahStockBalance == 0 ? null : s.AntahStockBalance;
                    s.MsbStockBalance_ = s.MsbStockBalance == 0 ? null : s.MsbStockBalance;
                    s.PsbStockBalance_ = s.PsbStockBalance == 0 ? null : s.PsbStockBalance;
                    s.SgTenderStockBalance_ = s.SgTenderStockBalance == 0 ? null : s.SgTenderStockBalance;
                    s.MyStockBalance_ = s.MyStockBalance == 0 ? null : s.MyStockBalance;
                    s.OtherStoreQty_ = s.OtherStoreQty == 0 ? null : s.OtherStoreQty;
                    s.InterCompanyTransitQty_ = s.InterCompanyTransitQty == 0 ? null : s.InterCompanyTransitQty;
                    s.StockBalance_ = s.StockBalance == 0 ? null : s.StockBalance;
                    s.StockHoldingBalance_ = s.StockHoldingBalance == 0 ? null : s.StockHoldingBalance;
                    s.BlanketAddhoc1_ = s.BlanketAddhoc1 == 0 ? null : s.BlanketAddhoc1;
                    s.Month1_ = s.Month1 == 0 ? null : s.Month1;
                    s.ProjectedHoldingStock1_ = s.ProjectedHoldingStock1 == 0 ? null : s.ProjectedHoldingStock1;
                    //s.ProductionProjected1_ = s.ProductionProjected1 == 0 ? null : s.ProductionProjected1;
                    s.ProductionProjected1_ = (s.Month1 + s.ProjectedHoldingStock1) - s.BlanketAddhoc1;
                    s.BlanketAddhoc2_ = s.BlanketAddhoc2 == 0 ? null : s.BlanketAddhoc2;
                    s.Month2_ = s.Month2 == 0 ? null : s.Month2;
                    s.ProjectedHoldingStock2_ = s.ProjectedHoldingStock2 == 0 ? null : s.ProjectedHoldingStock2;
                    s.ProductionProjected2_ = (s.Month2 + s.ProjectedHoldingStock2) - s.BlanketAddhoc2;
                    //s.ProductionProjected2_ = s.ProductionProjected2 == 0 ? null : s.ProductionProjected2;
                    s.BlanketAddhoc3_ = s.BlanketAddhoc3 == 0 ? null : s.BlanketAddhoc3;
                    s.Month3_ = s.Month3 == 0 ? null : s.Month3;
                    s.ProjectedHoldingStock3_ = s.ProjectedHoldingStock3 == 0 ? null : s.ProjectedHoldingStock3;
                    s.ProductionProjected3_ = (s.Month3 + s.ProjectedHoldingStock3) - s.BlanketAddhoc3;
                    //s.ProductionProjected3_ = s.ProductionProjected3 == 0 ? null : s.ProductionProjected3;
                    s.BlanketAddhoc4_ = s.BlanketAddhoc4 == 0 ? null : s.BlanketAddhoc4;
                    s.Month4_ = s.Month4 == 0 ? null : s.Month4;
                    s.ProjectedHoldingStock4_ = s.ProjectedHoldingStock4 == 0 ? null : s.ProjectedHoldingStock4;
                    s.ProductionProjected4_ = (s.Month4 + s.ProjectedHoldingStock4) - s.BlanketAddhoc4;
                    //s.ProductionProjected4_ = s.ProductionProjected4 == 0 ? null : s.ProductionProjected4;
                    s.BlanketAddhoc5_ = s.BlanketAddhoc5 == 0 ? null : s.BlanketAddhoc5;
                    s.Month5_ = s.Month5 == 0 ? null : s.Month5;
                    s.ProjectedHoldingStock5_ = s.ProjectedHoldingStock5 == 0 ? null : s.ProjectedHoldingStock5;
                    s.ProductionProjected5_ = (s.Month5 + s.ProjectedHoldingStock5) - s.BlanketAddhoc5;
                    //s.ProductionProjected5_ = s.ProductionProjected5 == 0 ? null : s.ProductionProjected5;
                    s.BlanketAddhoc6_ = s.BlanketAddhoc6 == 0 ? null : s.BlanketAddhoc6;
                    s.Month6_ = s.Month6 == 0 ? null : s.Month6;
                    s.ProjectedHoldingStock6_ = s.ProjectedHoldingStock6 == 0 ? null : s.ProjectedHoldingStock6;
                    s.ProductionProjected6_ = (s.Month6 + s.ProjectedHoldingStock6) - s.BlanketAddhoc6;
                    //s.ProductionProjected6_ = s.ProductionProjected6 == 0 ? null : s.ProductionProjected6;
                    s.BlanketAddhoc7_ = s.BlanketAddhoc7 == 0 ? null : s.BlanketAddhoc7;
                    s.Month7_ = s.Month7 == 0 ? null : s.Month7;
                    s.ProjectedHoldingStock7_ = s.ProjectedHoldingStock7 == 0 ? null : s.ProjectedHoldingStock7;
                    //s.ProductionProjected7_ = s.ProductionProjected7 == 0 ? null : s.ProductionProjected7;
                    s.ProductionProjected7_ = (s.Month7 + s.ProjectedHoldingStock7) - s.BlanketAddhoc7;
                    s.BlanketAddhoc8_ = s.BlanketAddhoc8 == 0 ? null : s.BlanketAddhoc8;
                    s.Month8_ = s.Month8 == 0 ? null : s.Month8;
                    s.ProjectedHoldingStock8_ = s.ProjectedHoldingStock8 == 0 ? null : s.ProjectedHoldingStock8;
                    //s.ProductionProjected8_ = s.ProductionProjected8 == 0 ? null : s.ProductionProjected8;
                    s.ProductionProjected8_ = (s.Month8 + s.ProjectedHoldingStock8) - s.BlanketAddhoc8;
                    s.BlanketAddhoc9_ = s.BlanketAddhoc9 == 0 ? null : s.BlanketAddhoc9;
                    s.Month9_ = s.Month9 == 0 ? null : s.Month9;
                    s.ProjectedHoldingStock9_ = s.ProjectedHoldingStock9 == 0 ? null : s.ProjectedHoldingStock9;
                    //s.ProductionProjected9_ = s.ProductionProjected9 == 0 ? null : s.ProductionProjected9;
                    s.ProductionProjected9_ = (s.Month9 + s.ProjectedHoldingStock9) - s.BlanketAddhoc9;
                    s.BlanketAddhoc10_ = s.BlanketAddhoc10 == 0 ? null : s.BlanketAddhoc10;
                    s.Month10_ = s.Month10 == 0 ? null : s.Month10;
                    s.ProjectedHoldingStock10_ = s.ProjectedHoldingStock10 == 0 ? null : s.ProjectedHoldingStock10;
                    //s.ProductionProjected10_ = s.ProductionProjected10 == 0 ? null : s.ProductionProjected10;
                    s.ProductionProjected10_ = (s.Month10 + s.ProjectedHoldingStock10) - s.BlanketAddhoc10;
                    s.BlanketAddhoc11_ = s.BlanketAddhoc11 == 0 ? null : s.BlanketAddhoc11;
                    s.Month11_ = s.Month11 == 0 ? null : s.Month11;
                    s.ProjectedHoldingStock11_ = s.ProjectedHoldingStock11 == 0 ? null : s.ProjectedHoldingStock11;
                    //s.ProductionProjected11_ = s.ProductionProjected11 == 0 ? null : s.ProductionProjected11;
                    s.ProductionProjected11_ = (s.Month11 + s.ProjectedHoldingStock11) - s.BlanketAddhoc11;
                    s.BlanketAddhoc12_ = s.BlanketAddhoc12 == 0 ? null : s.BlanketAddhoc12;
                    s.Month12_ = s.Month12 == 0 ? null : s.Month12;
                    s.ProjectedHoldingStock12_ = s.ProjectedHoldingStock12 == 0 ? null : s.ProjectedHoldingStock12;
                    //s.ProductionProjected12_ = s.ProductionProjected12 == 0 ? null : s.ProductionProjected12;
                    s.ProductionProjected12_ = (s.Month12 + s.ProjectedHoldingStock12) - s.BlanketAddhoc12;
                    s.NoOfTicket1_ = s.NoOfTicket1;
                    s.NoOfTicket2_ = s.NoOfTicket2;
                    s.NoOfTicket3_ = s.NoOfTicket3;
                    s.NoOfTicket4_ = s.NoOfTicket4;
                    s.NoOfTicket5_ = s.NoOfTicket5;
                    s.NoOfTicket6_ = s.NoOfTicket6; s.NoOfTicket7_ = s.NoOfTicket7; s.NoOfTicket8_ = s.NoOfTicket8;
                    s.NoOfTicket9_ = s.NoOfTicket9; s.NoOfTicket10_ = s.NoOfTicket10; s.NoOfTicket11_ = s.NoOfTicket11; s.NoOfTicket12_ = s.NoOfTicket12;
                    var GroupTicket1 = s.GroupItemTicket1 + "," + s.GroupTicket1;
                    if (GroupTicket1 != null)
                    {
                        var tic1 = GroupTicket1.Split(",").ToList().Where(w => w != "," && w != "").ToList();
                        s.NoOfTicketCount1_ = tic1.Count() > 0 ? tic1.Count() : null; ;
                        var tick1_ = tic1.Distinct().ToList(); s.Ticket1 = "";
                        if (tick1_ != null && tick1_.Count > 0)
                        {
                            tick1_.ForEach(a =>
                            {
                                if (!string.IsNullOrEmpty(a))
                                {
                                    s.Ticket1 += tic1.Where(q => q == a).Count() + "*" + a.Split(".")[0] + "/";
                                    if (!string.IsNullOrEmpty(s.NoOfTicket1))
                                    {
                                        s.Ticket1 += s.NoOfTicket1 + "/";
                                    }
                                }
                            });
                        }
                        s.Ticket1 += s.ProdOrderNo1;
                    }
                    var GroupTicket2 = s.GroupItemTicket2 + "," + s.GroupTicket2;
                    if (GroupTicket2 != null)
                    {
                        var tic2 = GroupTicket2.Split(",").ToList().Where(w => w != "," && w != "").ToList();
                        s.NoOfTicketCount2_ = tic2.Count() > 0 ? tic2.Count() : null;
                        var tick2_ = tic2.Distinct().ToList(); s.Ticket2 = "";
                        if (tick2_ != null && tick2_.Count > 0)
                        {
                            tick2_.ForEach(a =>
                            {
                                if (!string.IsNullOrEmpty(a))
                                {
                                    s.Ticket2 += tic2.Where(q => q == a).Count() + "*" + a.Split(".")[0] + "/";
                                    if (!string.IsNullOrEmpty(s.NoOfTicket2))
                                    {
                                        s.Ticket2 += s.NoOfTicket2 + "/";
                                    }
                                }
                            });
                        }
                        s.Ticket2 += s.ProdOrderNo2;
                    }
                    var GroupTicket3 = s.GroupItemTicket3 + "," + s.GroupTicket3;
                    if (GroupTicket3 != null)
                    {
                        var tic3 = GroupTicket3.Split(",").ToList().Where(w => w != "," && w != "").ToList();
                        s.NoOfTicketCount3_ = tic3.Count() > 0 ? tic3.Count() : null;
                        var tick3_ = tic3.Distinct().ToList(); s.Ticket3 = "";
                        if (tick3_ != null && tick3_.Count > 0)
                        {
                            tick3_.ForEach(a =>
                            {
                                if (!string.IsNullOrEmpty(a))
                                {
                                    s.Ticket3 += tic3.Where(q => q == a).Count() + "*" + a.Split(".")[0] + "/";
                                    if (!string.IsNullOrEmpty(s.NoOfTicket3))
                                    {
                                        s.Ticket3 += s.NoOfTicket3 + "/";
                                    }
                                }
                            });
                        }
                        s.Ticket3 += s.ProdOrderNo3;
                    }
                    var GroupTicket4 = s.GroupItemTicket4 + "," + s.GroupTicket4;
                    if (GroupTicket4 != null)
                    {
                        var tic4 = GroupTicket4.Split(",").ToList().Where(w => w != "," && w != "").ToList();
                        s.NoOfTicketCount4_ = tic4.Count() > 0 ? tic4.Count() : null;
                        var tick4_ = tic4.Distinct().ToList(); s.Ticket4 = "";
                        if (tick4_ != null && tick4_.Count > 0)
                        {
                            tick4_.ForEach(a =>
                            {
                                if (!string.IsNullOrEmpty(a))
                                {
                                    s.Ticket4 += tic4.Where(q => q == a).Count() + "*" + a.Split(".")[0] + "/";
                                    if (!string.IsNullOrEmpty(s.NoOfTicket4))
                                    {
                                        s.Ticket4 += s.NoOfTicket4 + "/";
                                    }
                                }
                            });
                        }
                        s.Ticket4 += s.ProdOrderNo4;
                    }
                    var GroupTicket5 = s.GroupItemTicket5 + "," + s.GroupTicket5;
                    if (GroupTicket5 != null)
                    {
                        var tic5 = GroupTicket5.Split(",").ToList().Where(w => w != "," && w != "").ToList();
                        s.NoOfTicketCount5_ = tic5.Count() > 0 ? tic5.Count() : null;
                        var tick5_ = tic5.Distinct().ToList(); s.Ticket5 = "";
                        if (tick5_ != null && tick5_.Count > 0)
                        {
                            tick5_.ForEach(a =>
                            {
                                if (!string.IsNullOrEmpty(a))
                                {
                                    s.Ticket5 += tic5.Where(q => q == a).Count() + "*" + a.Split(".")[0] + "/";
                                    if (!string.IsNullOrEmpty(s.NoOfTicket5))
                                    {
                                        s.Ticket5 += s.NoOfTicket5 + "/";
                                    }
                                }
                            });
                        }
                        s.Ticket5 += s.ProdOrderNo5;
                    }
                    var GroupTicket6 = s.GroupItemTicket6 + "," + s.GroupTicket6;
                    if (GroupTicket6 != null)
                    {
                        var tic6 = GroupTicket6.Split(",").ToList().Where(w => w != "," && w != "").ToList();
                        s.NoOfTicketCount6_ = tic6.Count() > 0 ? tic6.Count() : null;
                        var tick6_ = tic6.Distinct().ToList(); s.Ticket6 = "";
                        if (tick6_ != null && tick6_.Count > 0)
                        {
                            tick6_.ForEach(a =>
                            {
                                if (!string.IsNullOrEmpty(a))
                                {
                                    s.Ticket6 += tic6.Where(q => q == a).Count() + "*" + a.Split(".")[0] + "/";
                                    if (!string.IsNullOrEmpty(s.NoOfTicket6))
                                    {
                                        s.Ticket6 += s.NoOfTicket6 + "/";
                                    }
                                }
                            });
                        }
                        s.Ticket6 += s.ProdOrderNo6;
                    }
                    var GroupTicket7 = s.GroupItemTicket7 + "," + s.GroupTicket7;
                    if (GroupTicket7 != null)
                    {
                        var tic7 = GroupTicket7.Split(",").ToList().Where(w => w != "," && w != "").ToList();
                        s.NoOfTicketCount7_ = tic7.Count() > 0 ? tic7.Count() : null;
                        var tick7_ = tic7.Distinct().ToList(); s.Ticket7 = "";
                        if (tick7_ != null && tick7_.Count > 0)
                        {
                            tick7_.ForEach(a =>
                            {
                                if (!string.IsNullOrEmpty(a))
                                {
                                    s.Ticket7 += tic7.Where(q => q == a).Count() + "*" + a.Split(".")[0] + "/";
                                    if (!string.IsNullOrEmpty(s.NoOfTicket7))
                                    {
                                        s.Ticket7 += s.NoOfTicket7 + "/";
                                    }
                                }
                            });
                        }
                        s.Ticket7 += s.ProdOrderNo7;
                    }
                    var GroupTicket8 = s.GroupItemTicket8 + "," + s.GroupTicket8;
                    if (GroupTicket8 != null)
                    {
                        var tic8 = GroupTicket8.Split(",").ToList().Where(w => w != "," && w != "").ToList();
                        s.NoOfTicketCount8_ = tic8.Count() > 0 ? tic8.Count() : null;
                        var tick8_ = tic8.Distinct().ToList(); s.Ticket8 = "";
                        if (tick8_ != null && tick8_.Count > 0)
                        {
                            tick8_.ForEach(a =>
                            {
                                if (!string.IsNullOrEmpty(a))
                                {
                                    s.Ticket8 += tic8.Where(q => q == a).Count() + "*" + a.Split(".")[0] + "/";
                                    if (!string.IsNullOrEmpty(s.NoOfTicket8))
                                    {
                                        s.Ticket8 += s.NoOfTicket8 + "/";
                                    }
                                }
                            });
                        }
                        s.Ticket8 += s.ProdOrderNo8;
                    }
                    var GroupTicket9 = s.GroupItemTicket9 + "," + s.GroupTicket9;
                    if (GroupTicket9 != null)
                    {
                        var tic9 = GroupTicket9.Split(",").ToList().Where(w => w != "," && w != "").ToList();
                        s.NoOfTicketCount9_ = tic9.Count() > 0 ? tic9.Count() : null;
                        var tick9_ = tic9.Distinct().ToList(); s.Ticket9 = "";
                        if (tick9_ != null && tick9_.Count > 0)
                        {
                            tick9_.ForEach(a =>
                            {
                                if (!string.IsNullOrEmpty(a))
                                {
                                    s.Ticket9 += tic9.Where(q => q == a).Count() + "*" + a.Split(".")[0] + "/";
                                    if (!string.IsNullOrEmpty(s.NoOfTicket9))
                                    {
                                        s.Ticket9 += s.NoOfTicket9 + "/";
                                    }
                                }
                            });
                        }
                        s.Ticket9 += s.ProdOrderNo9;
                    }
                    var GroupTicket10 = s.GroupItemTicket10 + "," + s.GroupTicket10;
                    if (GroupTicket10 != null)
                    {
                        var tic10 = GroupTicket10.Split(",").ToList().Where(w => w != "," && w != "").ToList();
                        s.NoOfTicketCount10_ = tic10.Count() > 0 ? tic10.Count() : null;
                        var tick10_ = tic10.Distinct().ToList(); s.Ticket10 = "";
                        if (tick10_ != null && tick10_.Count > 0)
                        {
                            tick10_.ForEach(a =>
                            {
                                if (!string.IsNullOrEmpty(a))
                                {
                                    s.Ticket10 += tic10.Where(q => q == a).Count() + "*" + a.Split(".")[0] + "/";
                                    if (!string.IsNullOrEmpty(s.NoOfTicket10))
                                    {
                                        s.Ticket10 += s.NoOfTicket10 + "/";
                                    }
                                }
                            });
                        }
                        s.Ticket10 += s.ProdOrderNo10;
                    }
                    var GroupTicket11 = s.GroupItemTicket11 + "," + s.GroupTicket11;
                    if (GroupTicket11 != null)
                    {
                        var tic11 = GroupTicket11.Split(",").ToList().Where(w => w != "," && w != "").ToList();
                        s.NoOfTicketCount11_ = tic11.Count() > 0 ? tic11.Count() : null;
                        var tick11_ = tic11.Distinct().ToList(); s.Ticket11 = "";
                        if (tick11_ != null && tick11_.Count > 0)
                        {
                            tick11_.ForEach(a =>
                            {
                                if (!string.IsNullOrEmpty(a))
                                {
                                    s.Ticket11 += tic11.Where(q => q == a).Count() + "*" + a.Split(".")[0] + "/";
                                    if (!string.IsNullOrEmpty(s.NoOfTicket11))
                                    {
                                        s.Ticket11 += s.NoOfTicket11 + "/";
                                    }
                                }
                            });
                        }
                        s.Ticket11 += s.ProdOrderNo11;
                    }
                    var GroupTicket12 = s.GroupItemTicket12 + "," + s.GroupTicket12;
                    if (GroupTicket12 != null)
                    {
                        var tic12 = GroupTicket12.Split(",").ToList().Where(w => w != "," && w != "").ToList();
                        s.NoOfTicketCount12_ = tic12.Count() > 0 ? tic12.Count() : null;
                        var tick12_ = tic12.Distinct().ToList(); s.Ticket12 = "";
                        if (tick12_ != null && tick12_.Count > 0)
                        {
                            tick12_.ForEach(a =>
                            {
                                if (!string.IsNullOrEmpty(a))
                                {
                                    s.Ticket12 += tic12.Where(q => q == a).Count() + "*" + a.Split(".")[0] + "/";
                                    if (!string.IsNullOrEmpty(s.NoOfTicket12))
                                    {
                                        s.Ticket12 += s.NoOfTicket12 + "/";
                                    }
                                }
                            });
                        }
                        s.Ticket12 += s.ProdOrderNo12;
                    }
                });
            }
            return acreports.OrderBy(o => o.ItemNo).ToList();

        }

        private async Task<List<INPCalendarPivotModel>> SimulationAddhocV44(DateRangeModel endDate, List<NAVRecipesModel> recipeList, List<NAVRecipesModel> _orderRecipeList)
        {
            var categoryList = new List<string>
            {
                "CAP",
                "CREAM",
                "DD",
                "SYRUP",
                "TABLET",
                "VET",
                "POWDER",
                "INJ"
            };
            var salesCatLists = salesCatList();
            var itemdict = new Dictionary<string, decimal>();
            var MethodCodeList = new List<INPCalendarPivotModel>();
            var companyIds = new List<long?> { endDate.CompanyId };
            if (endDate.CompanyId == 3)
            {
                companyIds = new List<long?> { 1, 2 };
            }
            var intercompanyIds = new List<long?> { 1, 2 };
            var month = endDate.StockMonth.Month;//== 1 ? 12 : endDate.StockMonth.Month - 1;
            var year = endDate.StockMonth.Year;// == 1 ? endDate.StockMonth.Year - 1 : endDate.StockMonth.Year;
            var weekofmonth = GetWeekNumberOfMonth(endDate.StockMonth);
            var intransitMonth = month == 1 ? 12 : month - 1;
            DateTime lastDay = new DateTime(endDate.StockMonth.Year, endDate.StockMonth.Month, 1).AddMonths(1).AddDays(-1);

            var doNotReceivedList = new List<NavpostedShipment>();
            var navMethodCodeLines = new List<NavMethodCodeLines>(); var acitems = new List<NavItemCitemList>();
            var acItemBalListResult = new List<DistStockBalModel>(); var acEntries = new List<ACItemsModel>(); var dismapeditems = new List<NavItemCitemList>();
            var categoryItems = new List<NavSaleCategory>(); var itemRelations = new List<NavitemLinks>(); var prodyctionTickets = new List<ProductionSimulation>();
            var prenavStkbalance = new List<NavitemStockBalance>(); var navStkbalance = new List<NavitemStockBalance>(); var prodyctionoutTickets = new List<ProductionSimulation>();
            var blanletOrders = new List<SimulationAddhoc>(); var pre_acItemBalListResult = new List<DistStockBalModel>(); var grouptickets = new List<GroupPlaningTicket>();
            var intercompanyItems = new List<Navitems>(); var itemMasterforReport = new List<Navitems>(); var orderRequirements = new List<OrderRequirementLineModel>(); var acEntriesList = new List<Acitems>();
            DateTime firstDayOfMonth = new DateTime(endDate.StockMonth.Year, endDate.StockMonth.Month, 1);
            var dateMonth1 = firstDayOfMonth;// endDate.StockMonth;
            var datemonth12 = endDate.StockMonth.AddMonths(12);
            using (var connection = CreateConnection())
            {

                var query = "select ShipmentId,\r\nCompany,\r\nCompanyId,\r\nStockBalanceMonth,\r\nPostingDate,\r\nCustomer,\r\nCustomerNo,\r\nCustomerId,\r\nDeliveryOrderNo,\r\nDOLineNo,\r\nItemNo,\r\nDescription,\r\nIsRecived,\r\nDoQty,\r\nStatusCodeID,\r\nAddedByUserID,\r\nAddedDate,\r\nModifiedByUserID from NavpostedShipment WHERE CompanyId  in(" + string.Join(',', intercompanyIds) + ") AND CAST(StockBalanceMonth AS Date) <='" + lastDay + "'  AND (IsRecived is null Or IsRecived=0)\r\n;";
                query += "select t2.DistAcid,(case when t1.Quantity is NULL then  0 ELSE t1.Quantity END) as QtyOnHand,t2.ItemNo,t2.ItemDesc,t2.DistName,t1.DistItemId,(case when t2.CustomerId is NULL then  -1 ELSE t2.CustomerId END) as CustomerId,t2.CompanyId from " +
                    "DistStockBalance t1 INNER JOIN Acitems t2  ON t1.DistItemId=t2.DistACID\r\n" +
                    "Where  (t1.StockBalWeek=" + weekofmonth + " OR t1.StockBalWeek is null ) AND MONTH(t1.StockBalMonth) = " + month + " AND YEAR(t1.StockBalMonth)=" + year + ";\r\n";
                query += "select t2.DistAcid,(case when t1.Quantity is NULL then  0 ELSE t1.Quantity END) as QtyOnHand,t2.ItemNo,t2.ItemDesc,t2.DistName,t1.DistItemId,(case when t2.CustomerId is NULL then  -1 ELSE t2.CustomerId END) as CustomerId,t2.CompanyId from " +
                    "DistStockBalance t1 INNER JOIN Acitems t2  ON t1.DistItemId=t2.DistACID\r\n" +
                    "Where   t1.StockBalWeek=1 AND MONTH(t1.StockBalMonth) = " + month + " AND YEAR(t1.StockBalMonth)=" + year + "\r\n;";
                query += "select t1.CustomerId as DistId,t1.ToDate as ACMonth,t3.No as ItemNo,t2.Quantity as ACQty,t2.ItemId as SWItemId,t3.Description as ItemDesc,t1.CustomerId from Acentry t1 INNER JOIN AcentryLines t2 ON t1.ACEntryId=t2.ACEntryId INNER JOIN NAVItems t3 ON t2.ItemId=t3.ItemId\r\n" +
                    "WHERE t1.CompanyId  in(" + string.Join(',', companyIds) + ") AND CAST(t1.ToDate AS Date)>='" + endDate.StockMonth + "'  AND CAST(t1.FromDate AS Date)<='" + endDate.StockMonth + "';\r\n";
                query += "select NavItemCItemId,\r\nNavItemId,\r\nNavItemCustomerItemId from NavItemCitemList;\r\n";
                query += "select SaleCategoryID,\r\nCode,\r\nDescription,\r\nNoSeries,\r\nLocationID,\r\nSGNoSeries,\r\nStatusCodeID,\r\nAddedByUserID,\r\nAddedDate,\r\nModifiedByUserID,\r\nModifiedDate from NavSaleCategory;\r\n";
                query += "select ItemLinkId,\r\nMyItemId,\r\nSgItemId,\r\nStatusCodeID,\r\nAddedByUserID,\r\nAddedDate,\r\nModifiedByUserID,\r\nModifiedDate from NavitemLinks where SgItemId is not null AND MyItemId is not null;\r\n";
                query += "select ProductionSimulationID,\r\nCompanyId,\r\nProdOrderNo,\r\nItemID,\r\nItemNo,\r\nDescription,\r\nPackSize,\r\nQuantity,\r\nUOM,\r\nPerQuantity,\r\nPerQtyUOM,\r\nBatchNo,\r\nPlannedQty,\r\nOutputQty,\r\nIsOutput,\r\nStartingDate,\r\nStatusCodeID,\r\nAddedByUserID,\r\nAddedDate,\r\nModifiedByUserID,\r\nModifiedDate,\r\nProcessDate,\r\nIsBMRTicket,\r\nRePlanRefNo,\r\nBatchSize,\r\nDispense from ProductionSimulation where IsBmrticket=0 AND CAST(StartingDate AS Date)>='" + dateMonth1 + "'  AND CAST(StartingDate AS Date)<='" + datemonth12 + "' order by StartingDate desc;\r\n";

                query += "select ProductionSimulationID,\r\nCompanyId,\r\nProdOrderNo,\r\nItemID,\r\nItemNo,\r\nDescription,\r\nPackSize,\r\nQuantity,\r\nUOM,\r\nPerQuantity,\r\nPerQtyUOM,\r\nBatchNo,\r\nPlannedQty,\r\nOutputQty,\r\nIsOutput,\r\nStartingDate,\r\nStatusCodeID,\r\nAddedByUserID,\r\nAddedDate,\r\nModifiedByUserID,\r\nModifiedDate,\r\nProcessDate,\r\nIsBMRTicket,\r\nRePlanRefNo,\r\nBatchSize,\r\nDispense from ProductionSimulation where IsBmrticket=0 AND CAST(ProcessDate AS Date)>='" + dateMonth1 + "'  AND CAST(ProcessDate AS Date)<='" + datemonth12 + "' order by ProcessDate desc;\r\n";

                query += "SELECT t1.NavStockBalanceId,\r\nt1.ItemId,\r\nt1.StockBalMonth,\r\nt1.StockBalWeek,\r\nt1.Quantity,\r\nt1.RejectQuantity,\r\nt1.StatusCodeID,\r\nt1.AddedByUserID,\r\nt1.AddedDate,\r\nt1.ModifiedByUserID,\r\nt1.ModifiedDate,\r\nt1.ReworkQty,\r\nt1.WIPQty,\r\nt1.GlobalQty,\r\nt1.KIVQty,\r\nt1.SupplyWIPQty,\r\nt1.Supply1ProcessQty,\r\nt1.NotStartInvQty,t2.PackQty from NavitemStockBalance t1 INNER JOIN navitems t2 ON t1.ItemId=t2.ItemId Where t1.StockBalWeek=1 AND t2.ItemId is Not NUll and MONTH(t1.StockBalMonth) =" + month + " AND YEAR(t1.StockBalMonth)=" + year + ";\r\n";

                query += "SELECT t1.NavStockBalanceId,\r\nt1.ItemId,\r\nt1.StockBalMonth,\r\nt1.StockBalWeek,\r\nt1.Quantity,\r\nt1.RejectQuantity,\r\nt1.StatusCodeID,\r\nt1.AddedByUserID,\r\nt1.AddedDate,\r\nt1.ModifiedByUserID,\r\nt1.ModifiedDate,\r\nt1.ReworkQty,\r\nt1.WIPQty,\r\nt1.GlobalQty,\r\nt1.KIVQty,\r\nt1.SupplyWIPQty,\r\nt1.Supply1ProcessQty,\r\nt1.NotStartInvQty,t2.PackQty from NavitemStockBalance t1 INNER JOIN navitems t2 ON t1.ItemId=t2.ItemId Where (t1.StockBalWeek is null OR t1.StockBalWeek=" + weekofmonth + ") AND t2.ItemId is Not NUll and MONTH(t1.StockBalMonth) =" + month + " AND YEAR(t1.StockBalMonth)=" + year + ";\r\n";
                query += "select t1.SimualtionAddhocID,\r\nt1.DocumantType,\r\nt1.SelltoCustomerNo,\r\nt1.CustomerName,\r\nt1.Categories,\r\nt1.DocumentNo,\r\nt1.ExternalDocNo,\r\nt1.ItemID,\r\nt1.ItemNo,\r\nt1.Description,\r\nt1.Description1,\r\nt1.OutstandingQty,\r\nt1.PromisedDate,\r\nt1.ShipmentDate,\r\nt1.UOMCode from SimulationAddhoc t1 where  t1.CompanyId in(" + string.Join(',', companyIds) + ") AND CAST(t1.ShipmentDate AS Date)>='" + dateMonth1 + "'  AND CAST(t1.ShipmentDate AS Date)<='" + datemonth12 + "';\r\n";

                query += "select GroupPlanningId,\r\nCompanyId,\r\nBatchName,\r\nProductGroupCode,\r\nStartDate,\r\nItemNo,\r\nDescription,\r\nDescription1,\r\nBatchSize,\r\nQuantity,\r\nUOM,\r\nNoOfTicket,\r\nTotalQuantity from GroupPlaningTicket where CompanyId=" + endDate.CompanyId + " AND CAST(StartDate AS Date)>='" + dateMonth1 + "'  AND CAST(StartDate AS Date)<='" + datemonth12 + "' order by StartDate desc;\r\n";
                query += "select s.No,s.ItemId,s.Description,s.PackQty,s.CompanyId from NAVItems s;";
                query += "select t1.*,t2.Description2 as GenericCodeDescription2 from NAVItems t1\r\nLEFT JOIN GenericCodes t2 ON t1.GenericCodeId=t2.GenericCodeId\r\n" +
                    "where t1.CompanyId  in(" + string.Join(',', companyIds) + ") AND  t1.StatusCodeId=1 AND t1.ItemId IN(select NavItemId from NavItemCitemList WHere NavItemId>0);\r\n";


                query += "select t1.ProductId,t1.ProductQty,t1.NoOfTicket,t1.ExpectedStartDate,t1.RequireToSplit,t2.SplitProductID,t2.SplitProductQty from OrderRequirementLine t1\r\nINNER JOIN OrderRequirementLineSplit t2 ON t1.OrderRequirementLineID=t2.OrderRequirementLineID\r\nwhere t1.IsNavSync=1 AND CAST(t1.ExpectedStartDate AS Date)>='" + dateMonth1 + "'  AND CAST(t1.ExpectedStartDate AS Date)<='" + datemonth12 + "' order by t1.ExpectedStartDate desc;\r\n";
                query += "select t1.*,t6.DistACID,\r\nt6.CompanyId,\r\nt6.CustomerId,\r\nt6.DistName,\r\nt6.ItemGroup,\r\nt6.Steriod,\r\nt6.ShelfLife,\r\nt6.Quota,\r\nt6.Status,\r\nt6.ItemDesc,\r\nt6.PackSize,\r\nt6.ACQty,\r\nt6.ACMonth,\r\nt6.StatusCodeID,\r\nt6.AddedByUserID,\r\nt6.AddedDate,\r\nt6.ModifiedByUserID,\r\nt6.ModifiedDate,\r\nt6.ItemNo from NavItemCitemList t1\r\nINNER JOIN Acitems t6 ON t1.NavItemCustomerItemId=t6.DistACID;\r\n";
                query += "select t1.MethodCodeLineId,t1.MethodCodeId,t1.ItemID,t1.StatusCodeID,t1.AddedByUserID,t1.AddedDate,t1.ModifiedByUserID,t1.MethodCodeLineID,\r\nt2.MethodName,t2.MethodDescription,t2.NAVINPCategoryID,t2.CompanyId,t2.ProdFrequency,t2.DistReplenishHS,t2.DistACMonth,t2.AdhocMonthStandAlone,t2.AdhocPlanQty from NavMethodCodeLines t1 INNER JOIN NavMethodCode t2 ON t2.MethodCodeId=t1.MethodCodeId \r\nWHERE t1.ItemID IN(select t3.ItemId from NAVItems t3 LEFT JOIN GenericCodes t4 ON t3.GenericCodeId=t4.GenericCodeId where t3.CompanyId  in(" + string.Join(',', companyIds) + ") AND  t3.StatusCodeId=1 AND t3.ItemId IN(select NavItemId from NavItemCitemList WHere NavItemId>0));\r\n";
                query += "select t1.NavItemCitemId,t1.NavItemId,t1.NavItemCustomerItemId,t2.DistACID,t2.CompanyId,t2.DistName,t2.ItemGroup,t2.Steriod,t2.ShelfLife,t2.Quota,t2.ItemDesc,t2.PackSize,t2.PackSize,t2.ACQty,t2.ItemNo from NavItemCitemList t1 INNER JOIN Acitems t2 ON t1.NavItemCustomerItemId=t2.DistACID\r\nWHERE t1.NavItemId IN(select t3.ItemId from NAVItems t3 LEFT JOIN GenericCodes t4 ON t3.GenericCodeId=t4.GenericCodeId where t3.CompanyId  in(" + string.Join(',', companyIds) + ") AND  t3.StatusCodeId=1 AND t3.ItemId IN(select NavItemId from NavItemCitemList WHere NavItemId>0));\r\n";
                var results = await connection.QueryMultipleAsync(query);
                doNotReceivedList = results.Read<NavpostedShipment>().ToList();
                acItemBalListResult = results.Read<DistStockBalModel>().ToList();
                pre_acItemBalListResult = results.Read<DistStockBalModel>().ToList();
                acEntries = results.Read<ACItemsModel>().ToList();
                dismapeditems = results.Read<NavItemCitemList>().ToList();
                categoryItems = results.Read<NavSaleCategory>().ToList();
                itemRelations = results.Read<NavitemLinks>().ToList();
                prodyctionTickets = results.Read<ProductionSimulation>().ToList();
                prodyctionoutTickets = results.Read<ProductionSimulation>().ToList();
                prenavStkbalance = results.Read<NavitemStockBalance>().ToList();
                navStkbalance = results.Read<NavitemStockBalance>().ToList();
                blanletOrders = results.Read<SimulationAddhoc>().ToList();
                grouptickets = results.Read<GroupPlaningTicket>().ToList();
                intercompanyItems = results.Read<Navitems>().ToList();
                itemMasterforReport = results.Read<Navitems>().ToList();
                orderRequirements = results.Read<OrderRequirementLineModel>().ToList();
                acEntriesList = results.Read<Acitems>().ToList();
                navMethodCodeLines = results.Read<NavMethodCodeLines>().ToList();
                acitems = results.Read<NavItemCitemList>().ToList();
            }

            if (itemMasterforReport != null && itemMasterforReport.Count() > 0)
            {
                itemMasterforReport.ForEach(s =>
                {
                    s.NavMethodCodeLines = navMethodCodeLines.Where(w => w.ItemId == s.ItemId).ToList();
                    s.NavItemCitemList = acitems.Where(w => w.NavItemId == s.ItemId).ToList();
                });
            }

            var packSize = 900;
            decimal packSize2 = 0;
            var tenderExist1 = false;
            var tenderExist2 = false;
            var tenderExist3 = false;
            var tenderExist4 = false;
            var tenderExist5 = false;
            var tenderExist6 = false;
            var tenderExist7 = false;
            var tenderExist8 = false;
            var tenderExist9 = false;
            var tenderExist10 = false;
            var tenderExist11 = false;
            var tenderExist12 = false;


            var month1 = month;
            var month2 = month1 + 1 > 12 ? 1 : month1 + 1;
            var month3 = month2 + 1 > 12 ? 1 : month2 + 1;
            var month4 = month3 + 1 > 12 ? 1 : month3 + 1;
            var month5 = month4 + 1 > 12 ? 1 : month4 + 1;
            var month6 = month5 + 1 > 12 ? 1 : month5 + 1;
            var month7 = month6 + 1 > 12 ? 1 : month6 + 1;
            var month8 = month7 + 1 > 12 ? 1 : month7 + 1;
            var month9 = month8 + 1 > 12 ? 1 : month8 + 1;
            var month10 = month9 + 1 > 12 ? 1 : month9 + 1;
            var month11 = month10 + 1 > 12 ? 1 : month10 + 1;
            var month12 = month11 + 1 > 12 ? 1 : month11 + 1;

            var nextYear = year + 1;
            var year1 = year;
            var year2 = month2 > month ? year : nextYear;
            var year3 = month3 > month ? year : nextYear;
            var year4 = month4 > month ? year : nextYear;
            var year5 = month5 > month ? year : nextYear;
            var year6 = month6 > month ? year : nextYear;
            var year7 = month7 > month ? year : nextYear;
            var year8 = month8 > month ? year : nextYear;
            var year9 = month9 > month ? year : nextYear;
            var year10 = month10 > month ? year : nextYear;
            var year11 = month11 > month ? year : nextYear;
            var year12 = month12 > month ? year : nextYear;


            var acModel = new List<ACItemsModel>();
            if (acEntries != null && acEntries.Count > 0)
            {
                acEntries.ForEach(ac =>
                {
                    if (ac.CustomerId == 62)
                    {
                        ac.DistName = "Apex";
                    }
                    else if (ac.CustomerId == 51)
                    {
                        ac.DistName = "PSB PX";
                    }
                    else if (ac.CustomerId == 39)
                    {
                        ac.DistName = "MSS";
                    }
                    else if (ac.CustomerId == 60)
                    {
                        ac.DistName = "SG Tender";
                    }
                    else
                    {
                        ac.DistName = "Antah";
                    }
                    acModel.Add(ac);
                });

            }

            if (prenavStkbalance != null && prenavStkbalance.Count() > 0)
            {
                prenavStkbalance.ForEach(f =>
                {
                    f.Quantity = f.Quantity * (long)(f.PackQty > 0 ? f.PackQty : 0);
                });
            }
            if (navStkbalance != null && navStkbalance.Count() > 0)
            {
                navStkbalance.ForEach(f =>
                {
                    f.Quantity = f.Quantity * (long)(f.PackQty > 0 ? f.PackQty : 0);
                });
            }

            doNotReceivedList.ForEach(d =>
            {
                var item = intercompanyItems.FirstOrDefault(f => f.No == d.ItemNo && f.CompanyId == d.CompanyId);
                if (item != null)
                {
                    d.DoQty *= item.PackQty;
                }
            });
            int? parent = null;
            var genericId = new List<long?>();

            var orederRequ = new List<OrderRequirementLineModel>();

            orderRequirements.ForEach(f =>
            {
                if (!f.RequireToSplit.GetValueOrDefault(false))
                {
                    orederRequ.Add(new OrderRequirementLineModel
                    {
                        ProductId = f.ProductId,
                        ProductQty = f.ProductQty * f.NoOfTicket,
                        NoOfTicket = f.NoOfTicket,
                        ExpectedStartDate = f.ExpectedStartDate
                    });
                }
                else
                {
                    orederRequ.Add(new OrderRequirementLineModel
                    {
                        ProductId = f.SplitProductId,
                        ProductQty = f.SplitProductQty * f.NoOfTicket,
                        NoOfTicket = f.NoOfTicket,
                        ExpectedStartDate = f.ExpectedStartDate
                    });
                }
            });
            itemMasterforReport.ForEach(ac =>
            {
                if (ac.No == "FP-PP-TAB-302")
                {

                }

                if (!genericId.Exists(g => g == ac.GenericCodeId) && ac.GenericCodeId != null)
                {
                    var geericRptList = new List<GenericCodeReport>();
                    var itemNosRpt = itemMasterforReport.Where(i => i.GenericCodeId == ac.GenericCodeId).ToList();
                    var itemNos = itemMasterforReport.Where(i => i.GenericCodeId == ac.GenericCodeId).Select(s => s.ItemId).ToList();
                    //itemMasterforReport.Where(i => i.GenericCodeId == ac.GenericCodeId).Select(s => string.Format("{0} - {1} - {2}", s.No, s.Description, s.Description2)).ToList();
                    var itemCodes = itemMasterforReport.Where(i => i.GenericCodeId == ac.GenericCodeId).Select(s => s.No).ToList();
                    itemNosRpt.ForEach(rpt =>
                    {
                        var distId = dismapeditems.Where(d => d.NavItemId == rpt.ItemId).Select(s => s.NavItemCustomerItemId);
                        geericRptList.Add(new GenericCodeReport
                        {
                            ItemNo = rpt.No,
                            Description = rpt.Description,
                            Description2 = rpt.Description2,
                            ItemCategory = rpt.CategoryId.GetValueOrDefault(0) > 0 ? categoryList[int.Parse(rpt.CategoryId.ToString()) - 1] : string.Empty,
                            InternalRefNo = rpt.InternalRef,
                            //MethodCode = rpt.NavMethodCodeLines.Count > 0 ? rpt.NavMethodCodeLines.First().MethodCode.MethodName : "No MethodCode",
                            //DistItem = rpt.NavItemCitemList.Count > 0 ? rpt.NavItemCitemList.FirstOrDefault().NavItemCustomerItem.ItemDesc : string.Empty,
                            //StockBalance = acItemBalListResult.Where(a => distId.Contains(a.DistItemId)).Sum(s => s.QtyOnHand)
                        });

                    });
                    bool itemMapped = false;
                    decimal SgTenderStockBalance = 0;
                    genericId.Add(ac.GenericCodeId);
                    decimal? inventoryQty = 0;//itemMasterforReport.Where(i => i.GenericCodeId == ac.GenericCodeId).Sum(s => s.Inventory);
                    var navItemIds = itemMasterforReport.Where(i => i.GenericCodeId == ac.GenericCodeId).Select(s => s.ItemId).ToList();
                    inventoryQty = (navStkbalance.Where(n => navItemIds.Contains(n.ItemId.Value)).Sum(s => s.Quantity));
                    var wipQty = (navStkbalance.Where(n => navItemIds.Contains(n.ItemId.Value)).Sum(s => s.Wipqty));
                    decimal? notStartInvQty = (navStkbalance.Where(n => navItemIds.Contains(n.ItemId.Value)).Sum(s => s.NotStartInvQty));
                    var reWorkQty = (navStkbalance.Where(n => navItemIds.Contains(n.ItemId.Value)).Sum(s => s.ReworkQty));
                    var globalQty = (navStkbalance.Where(n => navItemIds.Contains(n.ItemId.Value)).Sum(s => s.GlobalQty));
                    decimal? interCompanyTransitQty = 0;

                    if (ac.No == "FP-PP-TAB-302")
                    {

                    }

                    if (endDate.CompanyId == 1)
                    {
                        var linkItemIds = itemRelations.Where(f => navItemIds.Contains(f.MyItemId.Value)).Select(s => s.SgItemId).ToList();
                        if (linkItemIds.Count > 0)
                        {
                            itemMapped = true;
                            SgTenderStockBalance = navStkbalance.Where(n => linkItemIds.Contains(n.ItemId.Value)).Sum(s => s.Quantity.GetValueOrDefault(0));

                            var itemNosIntertrans = intercompanyItems.Where(t => linkItemIds.Contains(t.ItemId) && t.CompanyId == 2).Select(n => n.No).ToList();
                            interCompanyTransitQty = doNotReceivedList.Where(d => itemNosIntertrans.Contains(d.ItemNo) && d.IsRecived == false && d.CompanyId == 2).Sum(s => s.DoQty);
                        }


                    }
                    else if (endDate.CompanyId == 2)
                    {
                        var linkItemIds = itemRelations.Where(f => navItemIds.Contains(f.SgItemId.Value)).Select(s => s.MyItemId).ToList();
                        if (linkItemIds.Count > 0)
                        {
                            itemMapped = true;
                            SgTenderStockBalance = navStkbalance.Where(n => linkItemIds.Contains(n.ItemId.Value)).Sum(s => s.Quantity.GetValueOrDefault(0));

                            var itemNosIntertrans = intercompanyItems.Where(t => linkItemIds.Contains(t.ItemId) && t.CompanyId == 1).Select(n => n.No).ToList();
                            interCompanyTransitQty = doNotReceivedList.Where(d => itemNosIntertrans.Contains(d.ItemNo) && d.IsRecived == false && d.CompanyId == 1).Sum(s => s.DoQty);
                        }
                    }
                    else if (endDate.CompanyId == 3)
                    {
                        var linkItemIds = itemRelations.Where(f => navItemIds.Contains(f.MyItemId.Value)).Select(s => s.SgItemId).ToList();
                        if (linkItemIds.Count > 0)
                        {
                            itemMapped = true;
                            inventoryQty = (navStkbalance.Where(n => linkItemIds.Contains(n.ItemId.Value)).Sum(s => s.Quantity.GetValueOrDefault(0)));
                        }

                        var sglinkItemIds = itemRelations.Where(f => navItemIds.Contains(f.SgItemId.Value)).Select(s => s.MyItemId).ToList();
                        if (linkItemIds.Count > 0) SgTenderStockBalance = navStkbalance.Where(n => sglinkItemIds.Contains(n.ItemId.Value)).Sum(s => s.Quantity.GetValueOrDefault(0));
                    }
                    var custmerItem = ac.NavItemCitemList.Count > 0 ? ac.NavItemCitemList.FirstOrDefault() : new NavItemCitemList();
                    if (custmerItem.DistAcid > 0)
                    {
                        var distIds = dismapeditems.Where(di => navItemIds.Contains(di.NavItemId.Value)).Select(s => s.NavItemCustomerItemId).Distinct().ToList(); //ac.NavItemCitemList.Select(s => s.NavItemCustomerItemId).ToList();
                        var distStkBal = acItemBalListResult.Where(d => distIds.Contains(d.DistItemId) && d.CompanyId == endDate.CompanyId).Sum(s => s.QtyOnHand);
                        var val = ac.PackSize.HasValue ? ac.PackSize.Value : 900;
                        packSize = val;

                        var prodRecipe = "";
                        var currentBatch = "";
                        var methodeCodeID = ac.NavMethodCodeLines.Count() > 0 ? ac.NavMethodCodeLines.First().MethodCodeId : -1;
                        var itemreceips = recipeList.Where(r => r.ItemRecipeId == methodeCodeID).ToList();
                        if (itemreceips.Count > 0)
                        {
                            var batchSizedes = itemreceips.FirstOrDefault().DefaultBatch;
                            currentBatch = batchSizedes;
                            //string numberOnly = Regex.Replace(batchSizedes, "[^0-9.]", "");
                            //if (!string.IsNullOrEmpty(numberOnly))
                            packSize2 = itemreceips.FirstOrDefault().UnitQTY;
                            prodRecipe = string.Join(",", itemreceips.Select(r => r.RecipeNo).ToList());//string.Format("{0}", itemreceips.FirstOrDefault().DefaultBatch + " | " + batchSizedes);

                        }
                        var Orderitemreceips = _orderRecipeList.Where(r => r.ItemNo == ac.No).ToList();
                        //if (Orderitemreceips.Count > 0)
                        //{
                        //    //var batchSizedes = itemreceips.OrderByDescending(o => o.BatchSize).FirstOrDefault().BatchSize;
                        //    //string numberOnly = Regex.Replace(batchSizedes, "[^0-9.]", "");
                        //    //if (!string.IsNullOrEmpty(numberOnly))
                        //    //    packSize2 = decimal.Parse(numberOnly) * 1000;
                        //    //prodRecipe = string.Format("{0}", itemreceips.OrderByDescending(o => o.BatchSize).FirstOrDefault().RecipeNo + " | " + batchSizedes);

                        //}
                        var threeMonth = distStkBal * packSize * 3;
                        var BatchSize = 900000 / packSize / 50;
                        var StockBalance = distStkBal + inventoryQty;
                        var NoofTickets = (threeMonth / packSize * 1000);
                        //var custId = custmerItem.DistName == "Apex" ? 21 : 1;
                        var acitem = acModel.Where(f => navItemIds.Contains(f.SWItemId.Value)).ToList();
                        var distItemBal = acItemBalListResult.Where(f => distIds.Contains(f.DistAcid) && f.CompanyId == endDate.CompanyId).ToList();
                        var apexQty = acitem.FirstOrDefault(f => f.CustomerId == 62) != null ? acitem.Where(f => f.CustomerId == 62).Sum(s => s.ACQty) : 0;
                        var antahQty = acitem.FirstOrDefault(f => f.CustomerId == 1) != null ? acitem.Where(f => f.CustomerId == 1).Sum(s => s.ACQty) : 0;
                        var sgtQty = acitem.FirstOrDefault(f => f.CustomerId == 60) != null ? acitem.Where(f => f.CustomerId == 60).Sum(s => s.ACQty) : 0;
                        var missQty = acitem.FirstOrDefault(f => f.CustomerId == 39) != null ? acitem.Where(f => f.CustomerId == 39).Sum(s => s.ACQty) : 0;
                        var pxQty = acitem.FirstOrDefault(f => f.CustomerId == 51) != null ? acitem.Where(f => f.CustomerId == 51).Sum(s => s.ACQty) : 0;
                        //sgtQty
                        var symlQty = packSize * (apexQty + antahQty + 0 + missQty + pxQty);
                        threeMonth = symlQty * 3;
                        var distTotal = (apexQty + antahQty + 0 + missQty + pxQty);
                        var AntahStockBalance = distItemBal.FirstOrDefault(f => f.CustomerId == 1) != null ? distItemBal.Where(f => f.CustomerId == 1).Sum(s => s.QtyOnHand) : 0;
                        var ApexStockBalance = distItemBal.FirstOrDefault(f => f.CustomerId == 62) != null ? distItemBal.Where(f => f.CustomerId == 62).Sum(s => s.QtyOnHand) : 0;
                        if (!itemMapped) SgTenderStockBalance = distItemBal.FirstOrDefault(f => f.DistName == "SG Tender") != null ? distItemBal.Where(f => f.DistName == "SG Tender").Sum(s => s.QtyOnHand) : 0;
                        var MsbStockBalance = distItemBal.FirstOrDefault(f => f.CustomerId == 39) != null ? distItemBal.Where(f => f.CustomerId == 39).Sum(s => s.QtyOnHand) : 0;
                        var PsbStockBalance = distItemBal.FirstOrDefault(f => f.CustomerId == 51) != null ? distItemBal.Where(f => f.CustomerId == 51).Sum(s => s.QtyOnHand) : 0;
                        decimal stockHoldingBalance = 0;
                        // decimal myStockBalance = 0;

                        var inTransitQty = doNotReceivedList.Where(d => itemCodes.Contains(d.ItemNo) && d.IsRecived == false && d.CompanyId == endDate.CompanyId).Sum(s => s.DoQty);
                        if (distTotal > 0)
                        {
                            stockHoldingBalance = (interCompanyTransitQty.GetValueOrDefault(0) + notStartInvQty.GetValueOrDefault(0) + wipQty.GetValueOrDefault(0) + 0 + globalQty.GetValueOrDefault(0) + AntahStockBalance + ApexStockBalance + MsbStockBalance + PsbStockBalance + SgTenderStockBalance + inventoryQty.GetValueOrDefault(0) + inTransitQty.GetValueOrDefault(0)) / distTotal;
                        }

                        var isTenderExist = blanletOrders.Any(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)));
                        tenderExist1 = blanletOrders.Any(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month1 && t.ShipmentDate.Value.Year == year1);
                        tenderExist2 = blanletOrders.Any(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month2 && t.ShipmentDate.Value.Year == year2);
                        tenderExist3 = blanletOrders.Any(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month3 && t.ShipmentDate.Value.Year == year3);
                        tenderExist4 = blanletOrders.Any(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month4 && t.ShipmentDate.Value.Year == year4);
                        tenderExist5 = blanletOrders.Any(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month5 && t.ShipmentDate.Value.Year == year5);
                        tenderExist6 = blanletOrders.Any(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month6 && t.ShipmentDate.Value.Year == year6);
                        tenderExist7 = blanletOrders.Any(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month7 && t.ShipmentDate.Value.Year == year7);
                        tenderExist8 = blanletOrders.Any(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month8 && t.ShipmentDate.Value.Year == year8);
                        tenderExist9 = blanletOrders.Any(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month9 && t.ShipmentDate.Value.Year == year9);
                        tenderExist10 = blanletOrders.Any(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month10 && t.ShipmentDate.Value.Year == year10);
                        tenderExist11 = blanletOrders.Any(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month11 && t.ShipmentDate.Value.Year == year11);
                        tenderExist12 = blanletOrders.Any(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month12 && t.ShipmentDate.Value.Year == year12);
                        var groupItemNo = itemMasterforReport.Where(i => i.GenericCodeId == ac.GenericCodeId).Select(s => s.No).ToList();
                        itemdict.Add(ac.No, symlQty);
                        //var tenderSum = blanletOrders.Where(t => itemNos.Contains(t.ItemId.Value)).Sum(s => s.OutstandingQty.Value);
                        MethodCodeList.Add(new INPCalendarPivotModel
                        {
                            GenericCodeReport = geericRptList,
                            GrouoItemNo = ac.No,
                            ItemId = ac.ItemId,
                            ItemNo = ac.GenericCodeDescription2,
                            RecipeLists = itemreceips.Select(s => string.Format("{0}", s.RecipeNo + " | " + s.BatchSize)).ToList(),
                            ItemRecipeLists = itemreceips,
                            OrderRecipeLists = Orderitemreceips,
                            IsSteroid = ac.Steroid.GetValueOrDefault(false),
                            SalesCategoryId = ac.CategoryId,
                            SalesCategory = salesCatLists.Where(w => w.Id == ac.CategoryId).FirstOrDefault()?.Text,
                            LocationID = categoryItems != null && categoryItems.FirstOrDefault(a => a.SaleCategoryId == ac.CategoryId) != null ? categoryItems.FirstOrDefault(a => a.SaleCategoryId == ac.CategoryId).LocationId : null,
                            // SalesCategory = categoryItems != null && categoryItems.FirstOrDefault(a => a.SaleCategoryId == ac.CategoryId) != null ? categoryItems.FirstOrDefault(a => a.SaleCategoryId == ac.CategoryId).Code : "",
                            MethodCodeId = ac.NavMethodCodeLines.Count > 0 ? ac.NavMethodCodeLines.First().MethodCodeId : -1,
                            GenericCodeID = ac.GenericCodeId,
                            Customer = custmerItem.DistName,
                            Description = ac.Description + " " + ac.Description2,
                            PackSize = packSize,
                            PackSize2 = packSize2,
                            Quantity = distStkBal,
                            ApexQty = apexQty,
                            AntahQty = antahQty,
                            SgtQty = sgtQty,
                            PxQty = pxQty,
                            MissQty = missQty,
                            SymlQty = symlQty,
                            ACQty = distStkBal,
                            AcSum = distTotal,
                            UnitQty = distStkBal * packSize,
                            ThreeMonthACQty = threeMonth,
                            ProdRecipe = !string.IsNullOrEmpty(prodRecipe) ? prodRecipe : "",
                            BatchSize = "",
                            NoofTickets = "",
                            NoOfDays = "Hours " + (3 * NoofTickets) + "& Day" + (3 * NoofTickets / 8).ToString(),
                            DistStockBalance = (AntahStockBalance + ApexStockBalance + MsbStockBalance + PsbStockBalance + SgTenderStockBalance + inventoryQty.GetValueOrDefault(0)),
                            AntahStockBalance = AntahStockBalance,
                            ApexStockBalance = ApexStockBalance,
                            SgTenderStockBalance = endDate.CompanyId == 2 ? inventoryQty.GetValueOrDefault(0) : SgTenderStockBalance,
                            MsbStockBalance = MsbStockBalance,
                            PsbStockBalance = PsbStockBalance,
                            NAVStockBalance = inventoryQty.GetValueOrDefault(0),
                            WipQty = wipQty.GetValueOrDefault(0),
                            NotStartInvQty = notStartInvQty.GetValueOrDefault(0),
                            Rework = reWorkQty.GetValueOrDefault(0),
                            InterCompanyTransitQty = interCompanyTransitQty.GetValueOrDefault(0),
                            OtherStoreQty = globalQty.GetValueOrDefault(0),
                            StockBalance = (interCompanyTransitQty.GetValueOrDefault(0) + notStartInvQty.GetValueOrDefault(0) + wipQty.GetValueOrDefault(0) + 0 + globalQty.GetValueOrDefault(0) + AntahStockBalance + ApexStockBalance + MsbStockBalance + PsbStockBalance + SgTenderStockBalance + inventoryQty.GetValueOrDefault(0) + inTransitQty.Value),
                            StockHoldingPackSize = (interCompanyTransitQty.GetValueOrDefault(0) + notStartInvQty.GetValueOrDefault(0) + wipQty.GetValueOrDefault(0) + 0 + globalQty.GetValueOrDefault(0) + AntahStockBalance + ApexStockBalance + MsbStockBalance + PsbStockBalance + SgTenderStockBalance + inventoryQty.GetValueOrDefault(0) + inTransitQty.Value) * packSize,
                            StockHoldingBalance = stockHoldingBalance,
                            MyStockBalance = endDate.CompanyId == 1 || endDate.CompanyId == 3 ? inventoryQty.GetValueOrDefault(0) : SgTenderStockBalance,
                            SgStockBalance = endDate.CompanyId == 2 ? inventoryQty.GetValueOrDefault(0) : SgTenderStockBalance,
                            MethodCode = ac.NavMethodCodeLines.Count > 0 ? ac.NavMethodCodeLines.First().MethodName : "No MethodCode",
                            Month = endDate.StockMonth.ToString("MMMM"),
                            Remarks = "AC",
                            BatchSize90 = currentBatch,
                            Roundup1 = packSize2 > 0 ? threeMonth / packSize2 : 0,
                            Roundup2 = 0,
                            ReportMonth = endDate.StockMonth,
                            UOM = ac.BaseUnitofMeasure,
                            Packuom = ac.PackUom,
                            Replenishment = ac.VendorNo,
                            StatusCodeId = ac.StatusCodeId,
                            isTenderExist = isTenderExist,
                            Itemgrouping = distTotal > 0 ? "Item with AC" : "Item without AC",
                            //TenderSum = tenderSum,
                            //Month1 = stockHoldingBalance > 1 ? stockHoldingBalance - 1 : 0,
                            //Month2 = stockHoldingBalance > 2 ? stockHoldingBalance - 2 : 0,
                            //Month3 = stockHoldingBalance > 3 ? stockHoldingBalance - 3 : 0,
                            //Month4 = stockHoldingBalance > 4 ? stockHoldingBalance - 4 : 0,
                            //Month5 = stockHoldingBalance > 5 ? stockHoldingBalance - 5 : 0,
                            //Month6 = stockHoldingBalance > 6 ? stockHoldingBalance - 6 : 0,
                            //Month7 = stockHoldingBalance > 7 ? stockHoldingBalance - 7 : 0,
                            //Month8 = stockHoldingBalance > 8 ? stockHoldingBalance - 8 : 0,
                            //Month9 = stockHoldingBalance > 9 ? stockHoldingBalance - 9 : 0,
                            //Month10 = stockHoldingBalance > 10 ? stockHoldingBalance - 10 : 0,
                            //Month11 = stockHoldingBalance > 11 ? stockHoldingBalance - 11 : 0,
                            //Month12 = stockHoldingBalance > 12 ? stockHoldingBalance - 12 : 0,

                            Month1 = stockHoldingBalance - 1,
                            Month2 = stockHoldingBalance - 2,
                            Month3 = stockHoldingBalance - 3,
                            Month4 = stockHoldingBalance - 4,
                            Month5 = stockHoldingBalance - 5,
                            Month6 = stockHoldingBalance - 6,
                            Month7 = stockHoldingBalance - 7,
                            Month8 = stockHoldingBalance - 8,
                            Month9 = stockHoldingBalance - 9,
                            Month10 = stockHoldingBalance - 10,
                            Month11 = stockHoldingBalance - 11,
                            Month12 = stockHoldingBalance - 12,
                            DeliverynotReceived = inTransitQty,//doNotReceivedList.Where(d => d.ItemNo == ac.No && d.IsRecived == false).Sum(s => s.DoQty),


                            GroupItemTicket1 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month1 && t.StartDate.Value.Year == year1).Select(s => s.Quantity).ToList()),
                            GroupItemTicket2 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month2 && t.StartDate.Value.Year == year2).Select(s => s.Quantity).ToList()),
                            GroupItemTicket3 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month3 && t.StartDate.Value.Year == year3).Select(s => s.Quantity).ToList()),
                            GroupItemTicket4 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month4 && t.StartDate.Value.Year == year4).Select(s => s.Quantity).ToList()),
                            GroupItemTicket5 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month5 && t.StartDate.Value.Year == year5).Select(s => s.Quantity).ToList()),
                            GroupItemTicket6 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month6 && t.StartDate.Value.Year == year6).Select(s => s.Quantity).ToList()),
                            GroupItemTicket7 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month7 && t.StartDate.Value.Year == year7).Select(s => s.Quantity).ToList()),
                            GroupItemTicket8 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month8 && t.StartDate.Value.Year == year8).Select(s => s.Quantity).ToList()),
                            GroupItemTicket9 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month9 && t.StartDate.Value.Year == year9).Select(s => s.Quantity).ToList()),
                            GroupItemTicket10 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month10 && t.StartDate.Value.Year == year10).Select(s => s.Quantity).ToList()),
                            GroupItemTicket11 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month11 && t.StartDate.Value.Year == year11).Select(s => s.Quantity).ToList()),
                            GroupItemTicket12 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month12 && t.StartDate.Value.Year == year12).Select(s => s.Quantity).ToList()),

                            NoOfTicket1 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month1 && t.StartDate.Value.Year == year1).Select(s => s.NoOfTicket).ToList()),
                            NoOfTicket2 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month2 && t.StartDate.Value.Year == year2).Select(s => s.NoOfTicket).ToList()),
                            NoOfTicket3 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month3 && t.StartDate.Value.Year == year3).Select(s => s.NoOfTicket).ToList()),
                            NoOfTicket4 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month4 && t.StartDate.Value.Year == year4).Select(s => s.NoOfTicket).ToList()),
                            NoOfTicket5 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month5 && t.StartDate.Value.Year == year5).Select(s => s.NoOfTicket).ToList()),
                            NoOfTicket6 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month6 && t.StartDate.Value.Year == year6).Select(s => s.NoOfTicket).ToList()),
                            NoOfTicket7 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month7 && t.StartDate.Value.Year == year7).Select(s => s.NoOfTicket).ToList()),
                            NoOfTicket8 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month8 && t.StartDate.Value.Year == year8).Select(s => s.NoOfTicket).ToList()),
                            NoOfTicket9 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month9 && t.StartDate.Value.Year == year9).Select(s => s.NoOfTicket).ToList()),
                            NoOfTicket10 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month10 && t.StartDate.Value.Year == year10).Select(s => s.NoOfTicket).ToList()),
                            NoOfTicket11 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month11 && t.StartDate.Value.Year == year11).Select(s => s.NoOfTicket).ToList()),
                            NoOfTicket12 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month12 && t.StartDate.Value.Year == year12).Select(s => s.NoOfTicket).ToList()),



                            Ticket1 = string.Format("{0}/{1}", string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month1 && t.StartDate.Value.Year == year1).Select(s => s.Quantity).ToList()), string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month1 && t.StartDate.Value.Year == year1).Select(s => s.NoOfTicket).ToList())),
                            Ticket2 = string.Format("{0}/{1}", string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month2 && t.StartDate.Value.Year == year2).Select(s => s.Quantity).ToList()), string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month2 && t.StartDate.Value.Year == year2).Select(s => s.NoOfTicket).ToList())),
                            Ticket3 = string.Format("{0}/{1}", string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month3 && t.StartDate.Value.Year == year3).Select(s => s.Quantity).ToList()), string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month3 && t.StartDate.Value.Year == year3).Select(s => s.NoOfTicket).ToList())),
                            Ticket4 = string.Format("{0}/{1}", string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month4 && t.StartDate.Value.Year == year4).Select(s => s.Quantity).ToList()), string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month4 && t.StartDate.Value.Year == year4).Select(s => s.NoOfTicket).ToList())),
                            Ticket5 = string.Format("{0}/{1}", string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month5 && t.StartDate.Value.Year == year5).Select(s => s.Quantity).ToList()), string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month5 && t.StartDate.Value.Year == year5).Select(s => s.NoOfTicket).ToList())),
                            Ticket6 = string.Format("{0}/{1}", string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month6 && t.StartDate.Value.Year == year6).Select(s => s.Quantity).ToList()), string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month6 && t.StartDate.Value.Year == year6).Select(s => s.NoOfTicket).ToList())),
                            Ticket7 = string.Format("{0}/{1}", string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month7 && t.StartDate.Value.Year == year7).Select(s => s.Quantity).ToList()), string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month7 && t.StartDate.Value.Year == year7).Select(s => s.NoOfTicket).ToList())),
                            Ticket8 = string.Format("{0}/{1}", string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month8 && t.StartDate.Value.Year == year8).Select(s => s.Quantity).ToList()), string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month8 && t.StartDate.Value.Year == year8).Select(s => s.NoOfTicket).ToList())),
                            Ticket9 = string.Format("{0}/{1}", string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month9 && t.StartDate.Value.Year == year9).Select(s => s.Quantity).ToList()), string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month9 && t.StartDate.Value.Year == year9).Select(s => s.NoOfTicket).ToList())),
                            Ticket10 = string.Format("{0}/{1}", string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month10 && t.StartDate.Value.Year == year10).Select(s => s.Quantity).ToList()), string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month10 && t.StartDate.Value.Year == year10).Select(s => s.NoOfTicket).ToList())),
                            Ticket11 = string.Format("{0}/{1}", string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month11 && t.StartDate.Value.Year == year11).Select(s => s.Quantity).ToList()), string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month11 && t.StartDate.Value.Year == year11).Select(s => s.NoOfTicket).ToList())),
                            Ticket12 = string.Format("{0}/{1}", string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month12 && t.StartDate.Value.Year == year12).Select(s => s.Quantity).ToList()), string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month12 && t.StartDate.Value.Year == year12).Select(s => s.NoOfTicket).ToList())),

                            OutputTicket1 = string.Format("{0}/{1}", string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month1).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month1).Select(s => s.ProdOrderNo).ToList())),
                            OutputTicket2 = string.Format("{0}/{1}", string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month2).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month2).Select(s => s.ProdOrderNo).ToList())),
                            OutputTicket3 = string.Format("{0}/{1}", string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month3).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month3).Select(s => s.ProdOrderNo).ToList())),
                            OutputTicket4 = string.Format("{0}/{1}", string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month4).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month4).Select(s => s.ProdOrderNo).ToList())),
                            OutputTicket5 = string.Format("{0}/{1}", string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month5).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month5).Select(s => s.ProdOrderNo).ToList())),
                            OutputTicket6 = string.Format("{0}/{1}", string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month6).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month6).Select(s => s.ProdOrderNo).ToList())),
                            OutputTicket7 = string.Format("{0}/{1}", string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month7).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month7).Select(s => s.ProdOrderNo).ToList())),
                            OutputTicket8 = string.Format("{0}/{1}", string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month8).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month8).Select(s => s.ProdOrderNo).ToList())),
                            OutputTicket9 = string.Format("{0}/{1}", string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month9).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month9).Select(s => s.ProdOrderNo).ToList())),
                            OutputTicket10 = string.Format("{0}/{1}", string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month10).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month10).Select(s => s.ProdOrderNo).ToList())),
                            OutputTicket11 = string.Format("{0}/{1}", string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month11).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month11).Select(s => s.ProdOrderNo).ToList())),
                            OutputTicket12 = string.Format("{0}/{1}", string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month12).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month12).Select(s => s.ProdOrderNo).ToList())),

                            TicketHoldingStock1 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month1).Sum(s => s.NoOfTicket.GetValueOrDefault(0)),
                            TicketHoldingStock2 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month2).Sum(s => s.NoOfTicket.GetValueOrDefault(0)),
                            TicketHoldingStock3 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month3).Sum(s => s.NoOfTicket.GetValueOrDefault(0)),
                            TicketHoldingStock4 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month4).Sum(s => s.NoOfTicket.GetValueOrDefault(0)),
                            TicketHoldingStock5 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month5).Sum(s => s.NoOfTicket.GetValueOrDefault(0)),
                            TicketHoldingStock6 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month6).Sum(s => s.NoOfTicket.GetValueOrDefault(0)),
                            TicketHoldingStock7 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month7).Sum(s => s.NoOfTicket.GetValueOrDefault(0)),
                            TicketHoldingStock8 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month8).Sum(s => s.NoOfTicket.GetValueOrDefault(0)),
                            TicketHoldingStock9 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month9).Sum(s => s.NoOfTicket.GetValueOrDefault(0)),
                            TicketHoldingStock10 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month10).Sum(s => s.NoOfTicket.GetValueOrDefault(0)),
                            TicketHoldingStock11 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month11).Sum(s => s.NoOfTicket.GetValueOrDefault(0)),
                            TicketHoldingStock12 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month12).Sum(s => s.NoOfTicket.GetValueOrDefault(0)),

                            ProductionTicket1 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month1).Sum(s => s.ProductQty.GetValueOrDefault(0)),
                            ProductionTicket2 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month2).Sum(s => s.ProductQty.GetValueOrDefault(0)),
                            ProductionTicket3 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month3).Sum(s => s.ProductQty.GetValueOrDefault(0)),
                            ProductionTicket4 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month4).Sum(s => s.ProductQty.GetValueOrDefault(0)),
                            ProductionTicket5 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month5).Sum(s => s.ProductQty.GetValueOrDefault(0)),
                            ProductionTicket6 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month6).Sum(s => s.ProductQty.GetValueOrDefault(0)),
                            ProductionTicket7 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month7).Sum(s => s.ProductQty.GetValueOrDefault(0)),
                            ProductionTicket8 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month8).Sum(s => s.ProductQty.GetValueOrDefault(0)),
                            ProductionTicket9 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month9).Sum(s => s.ProductQty.GetValueOrDefault(0)),
                            ProductionTicket10 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month10).Sum(s => s.ProductQty.GetValueOrDefault(0)),
                            ProductionTicket11 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month11).Sum(s => s.ProductQty.GetValueOrDefault(0)),
                            ProductionTicket12 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month12).Sum(s => s.ProductQty.GetValueOrDefault(0)),



                            IsTenderExist1 = tenderExist1,//blanletOrders.Any(t => itemNos.Contains(t.ItemId.Value) && t.ShipmentDate.Value.Month == month1),
                            IsTenderExist2 = tenderExist2,// blanletOrders.Any(t => itemNos.Contains(t.ItemId.Value) && t.ShipmentDate.Value.Month == month2),
                            IsTenderExist3 = tenderExist3,// blanletOrders.Any(t => itemNos.Contains(t.ItemId.Value) && t.ShipmentDate.Value.Month == month3),
                            IsTenderExist4 = tenderExist4,//blanletOrders.Any(t => itemNos.Contains(t.ItemId.Value) && t.ShipmentDate.Value.Month == month4),
                            IsTenderExist5 = tenderExist5,//blanletOrders.Any(t => itemNos.Contains(t.ItemId.Value) && t.ShipmentDate.Value.Month == month5),
                            IsTenderExist6 = tenderExist6,//blanletOrders.Any(t => itemNos.Contains(t.ItemId.Value) && t.ShipmentDate.Value.Month == month6),
                            IsTenderExist7 = tenderExist7,//blanletOrders.Any(t => itemNos.Contains(t.ItemId.Value) && t.ShipmentDate.Value.Month == month7),
                            IsTenderExist8 = tenderExist8,//blanletOrders.Any(t => itemNos.Contains(t.ItemId.Value) && t.ShipmentDate.Value.Month == month8),
                            IsTenderExist9 = tenderExist9,//blanletOrders.Any(t => itemNos.Contains(t.ItemId.Value) && t.ShipmentDate.Value.Month == month9),
                            IsTenderExist10 = tenderExist10,//blanletOrders.Any(t => itemNos.Contains(t.ItemId.Value) && t.ShipmentDate.Value.Month == month10),
                            IsTenderExist11 = tenderExist11,//blanletOrders.Any(t => itemNos.Contains(t.ItemId.Value) && t.ShipmentDate.Value.Month == month11),
                            IsTenderExist12 = tenderExist12,//blanletOrders.Any(t => itemNos.Contains(t.ItemId.Value) && t.ShipmentDate.Value.Month == month12),


                            ProjectedHoldingStock1 = distTotal > 0 ? prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month1).Sum(s => s.Quantity) / (distTotal) : prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month1).Sum(s => s.Quantity),
                            ProjectedHoldingStock2 = distTotal > 0 ? prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month2).Sum(s => s.Quantity) / (distTotal) : prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month2).Sum(s => s.Quantity),
                            ProjectedHoldingStock3 = distTotal > 0 ? prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month3).Sum(s => s.Quantity) / (distTotal) : prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month3).Sum(s => s.Quantity),
                            ProjectedHoldingStock4 = distTotal > 0 ? prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month4).Sum(s => s.Quantity) / (distTotal) : prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month4).Sum(s => s.Quantity),
                            ProjectedHoldingStock5 = distTotal > 0 ? prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month5).Sum(s => s.Quantity) / (distTotal) : prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month5).Sum(s => s.Quantity),
                            ProjectedHoldingStock6 = distTotal > 0 ? prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month6).Sum(s => s.Quantity) / (distTotal) : prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month6).Sum(s => s.Quantity),
                            ProjectedHoldingStock7 = distTotal > 0 ? prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month7).Sum(s => s.Quantity) / (distTotal) : prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month7).Sum(s => s.Quantity),
                            ProjectedHoldingStock8 = distTotal > 0 ? prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month8).Sum(s => s.Quantity) / (distTotal) : prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month8).Sum(s => s.Quantity),
                            ProjectedHoldingStock9 = distTotal > 0 ? prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month9).Sum(s => s.Quantity) / (distTotal) : prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month9).Sum(s => s.Quantity),
                            ProjectedHoldingStock10 = distTotal > 0 ? prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month10).Sum(s => s.Quantity) / (distTotal) : prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month10).Sum(s => s.Quantity),
                            ProjectedHoldingStock11 = distTotal > 0 ? prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month11).Sum(s => s.Quantity) / (distTotal) : prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month11).Sum(s => s.Quantity),
                            ProjectedHoldingStock12 = distTotal > 0 ? prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month12).Sum(s => s.Quantity) / (distTotal) : prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month12).Sum(s => s.Quantity),

                            OutputProjectedHoldingStock1 = distTotal > 0 ? prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month1).Sum(s => s.Quantity) / (distTotal) : prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month1).Sum(s => s.Quantity),
                            OutputProjectedHoldingStock2 = distTotal > 0 ? prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month2).Sum(s => s.Quantity) / (distTotal) : prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month2).Sum(s => s.Quantity),
                            OutputProjectedHoldingStock3 = distTotal > 0 ? prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month3).Sum(s => s.Quantity) / (distTotal) : prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month3).Sum(s => s.Quantity),
                            OutputProjectedHoldingStock4 = distTotal > 0 ? prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month4).Sum(s => s.Quantity) / (distTotal) : prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month4).Sum(s => s.Quantity),
                            OutputProjectedHoldingStock5 = distTotal > 0 ? prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month5).Sum(s => s.Quantity) / (distTotal) : prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month5).Sum(s => s.Quantity),
                            OutputProjectedHoldingStock6 = distTotal > 0 ? prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month6).Sum(s => s.Quantity) / (distTotal) : prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month6).Sum(s => s.Quantity),
                            OutputProjectedHoldingStock7 = distTotal > 0 ? prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month7).Sum(s => s.Quantity) / (distTotal) : prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month7).Sum(s => s.Quantity),
                            OutputProjectedHoldingStock8 = distTotal > 0 ? prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month8).Sum(s => s.Quantity) / (distTotal) : prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month8).Sum(s => s.Quantity),
                            OutputProjectedHoldingStock9 = distTotal > 0 ? prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month9).Sum(s => s.Quantity) / (distTotal) : prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month9).Sum(s => s.Quantity),
                            OutputProjectedHoldingStock10 = distTotal > 0 ? prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month10).Sum(s => s.Quantity) / (distTotal) : prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month10).Sum(s => s.Quantity),
                            OutputProjectedHoldingStock11 = distTotal > 0 ? prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month11).Sum(s => s.Quantity) / (distTotal) : prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month11).Sum(s => s.Quantity),
                            OutputProjectedHoldingStock12 = distTotal > 0 ? prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month12).Sum(s => s.Quantity) / (distTotal) : prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month12).Sum(s => s.Quantity),


                            BlanketAddhoc1 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month1 && t.ShipmentDate.Value.Year == year1).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                            BlanketAddhoc2 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month2 && t.ShipmentDate.Value.Year == year2).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                            BlanketAddhoc3 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month3 && t.ShipmentDate.Value.Year == year3).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                            BlanketAddhoc4 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month4 && t.ShipmentDate.Value.Year == year4).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                            BlanketAddhoc5 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month5 && t.ShipmentDate.Value.Year == year5).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                            BlanketAddhoc6 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month6 && t.ShipmentDate.Value.Year == year6).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                            BlanketAddhoc7 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month7 && t.ShipmentDate.Value.Year == year7).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                            BlanketAddhoc8 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month8 && t.ShipmentDate.Value.Year == year8).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                            BlanketAddhoc9 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month9 && t.ShipmentDate.Value.Year == year9).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                            BlanketAddhoc10 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month10 && t.ShipmentDate.Value.Year == year10).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                            BlanketAddhoc11 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month11 && t.ShipmentDate.Value.Year == year11).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                            BlanketAddhoc12 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month12 && t.ShipmentDate.Value.Year == year12).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),


                        });
                    }
                }
            });
            MethodCodeList = MethodCodeList.Where(f => f.StatusCodeId == 1).ToList();

            MethodCodeList.ForEach(f =>
            {

                if (f.ItemNo.Contains("Folic Acid Tablet"))
                {

                }
                var itemNos = itemMasterforReport.Where(i => i.GenericCodeId == f.GenericCodeID).Select(s => s.ItemId).ToList();

                var item_Nos = itemMasterforReport.Where(i => i.GenericCodeId == f.GenericCodeID).Select(s => s.No).ToList();


                f.GroupTicket1 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month1).Select(s => s.Quantity).ToList());
                f.GroupTicket2 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month2).Select(s => s.Quantity).ToList());
                f.GroupTicket3 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month3).Select(s => s.Quantity).ToList());
                f.GroupTicket4 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month4).Select(s => s.Quantity).ToList());
                f.GroupTicket5 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month5).Select(s => s.Quantity).ToList());
                f.GroupTicket6 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month6).Select(s => s.Quantity).ToList());
                f.GroupTicket7 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month7).Select(s => s.Quantity).ToList());
                f.GroupTicket8 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month8).Select(s => s.Quantity).ToList());
                f.GroupTicket9 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month9).Select(s => s.Quantity).ToList());
                f.GroupTicket10 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month10).Select(s => s.Quantity).ToList());
                f.GroupTicket11 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month11).Select(s => s.Quantity).ToList());
                f.GroupTicket12 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month12).Select(s => s.Quantity).ToList());

                f.ProdOrderNo1 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month1).Select(s => s.ProdOrderNo).ToList());
                f.ProdOrderNo2 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month2).Select(s => s.ProdOrderNo).ToList());
                f.ProdOrderNo3 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month3).Select(s => s.ProdOrderNo).ToList());
                f.ProdOrderNo4 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month4).Select(s => s.ProdOrderNo).ToList());
                f.ProdOrderNo5 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month5).Select(s => s.ProdOrderNo).ToList());
                f.ProdOrderNo6 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month6).Select(s => s.ProdOrderNo).ToList());
                f.ProdOrderNo7 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month7).Select(s => s.ProdOrderNo).ToList());
                f.ProdOrderNo8 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month8).Select(s => s.ProdOrderNo).ToList());
                f.ProdOrderNo9 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month9).Select(s => s.ProdOrderNo).ToList());
                f.ProdOrderNo10 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month10).Select(s => s.ProdOrderNo).ToList());
                f.ProdOrderNo11 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month11).Select(s => s.ProdOrderNo).ToList());
                f.ProdOrderNo12 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month12).Select(s => s.ProdOrderNo).ToList());

                f.ProTicket1 = string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month1).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month1).Select(s => s.ProdOrderNo).ToList()));
                f.ProTicket2 = string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month2).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month2).Select(s => s.ProdOrderNo).ToList()));
                f.ProTicket3 = string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month3).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month3).Select(s => s.ProdOrderNo).ToList()));
                f.ProTicket4 = string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month4).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month4).Select(s => s.ProdOrderNo).ToList()));
                f.ProTicket5 = string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month5).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month5).Select(s => s.ProdOrderNo).ToList()));
                f.ProTicket6 = string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month6).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month6).Select(s => s.ProdOrderNo).ToList()));
                f.ProTicket7 = string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month7).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month7).Select(s => s.ProdOrderNo).ToList()));
                f.ProTicket8 = string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month8).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month8).Select(s => s.ProdOrderNo).ToList()));
                f.ProTicket9 = string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month9).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month9).Select(s => s.ProdOrderNo).ToList()));
                f.ProTicket10 = string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month10).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month10).Select(s => s.ProdOrderNo).ToList()));
                f.ProTicket11 = string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month11).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month11).Select(s => s.ProdOrderNo).ToList()));
                f.ProTicket12 = string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month12).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month12).Select(s => s.ProdOrderNo).ToList()));


                f.Ticket1 = f.Ticket1 + f.ProTicket1;
                f.Ticket2 = f.Ticket2 + f.ProTicket2;
                f.Ticket3 = f.Ticket3 + f.ProTicket3;
                f.Ticket4 = f.Ticket4 + f.ProTicket4;
                f.Ticket5 = f.Ticket5 + f.ProTicket5;
                f.Ticket6 = f.Ticket6 + f.ProTicket6;
                f.Ticket7 = f.Ticket7 + f.ProTicket7;
                f.Ticket8 = f.Ticket8 + f.ProTicket8;
                f.Ticket9 = f.Ticket9 + f.ProTicket9;
                f.Ticket10 = f.Ticket10 + f.ProTicket10;
                f.Ticket11 = f.Ticket11 + f.ProTicket11;
                f.Ticket12 = f.Ticket12 + f.ProTicket12;

                /* f.Ticket1 = f.Ticket1 + string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month1).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month1).Select(s => s.ProdOrderNo).ToList()));
                 f.Ticket2 = f.Ticket2 + string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month2).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month2).Select(s => s.ProdOrderNo).ToList()));
                 f.Ticket3 = f.Ticket3 + string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month3).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month3).Select(s => s.ProdOrderNo).ToList()));
                 f.Ticket4 = f.Ticket4 + string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month4).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month4).Select(s => s.ProdOrderNo).ToList()));
                 f.Ticket5 = f.Ticket5 + string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month5).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month5).Select(s => s.ProdOrderNo).ToList()));
                 f.Ticket6 = f.Ticket6 + string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month6).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month6).Select(s => s.ProdOrderNo).ToList()));
                 f.Ticket7 = f.Ticket7 + string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month7).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month7).Select(s => s.ProdOrderNo).ToList()));
                 f.Ticket8 = f.Ticket8 + string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month8).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month8).Select(s => s.ProdOrderNo).ToList()));
                 f.Ticket9 = f.Ticket9 + string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month9).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month9).Select(s => s.ProdOrderNo).ToList()));
                 f.Ticket10 = f.Ticket10 + string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month10).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month10).Select(s => s.ProdOrderNo).ToList()));
                 f.Ticket11 = f.Ticket11 + string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month11).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month11).Select(s => s.ProdOrderNo).ToList()));
                 f.Ticket12 = f.Ticket12 + string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month12).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month12).Select(s => s.ProdOrderNo).ToList()));
 */

                f.ProjectedHoldingStock1 += f.AcSum > 0 ? grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month1 && t.StartDate.Value.Year == year1).Sum(s => s.TotalQuantity.Value) / (f.AcSum) : grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month1 && t.StartDate.Value.Year == year1).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStock2 += f.AcSum > 0 ? grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month2 && t.StartDate.Value.Year == year2).Sum(s => s.TotalQuantity.Value) / (f.AcSum) : grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month2 && t.StartDate.Value.Year == year2).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStock3 += f.AcSum > 0 ? grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month3 && t.StartDate.Value.Year == year3).Sum(s => s.TotalQuantity.Value) / (f.AcSum) : grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month3 && t.StartDate.Value.Year == year3).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStock4 += f.AcSum > 0 ? grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month4 && t.StartDate.Value.Year == year4).Sum(s => s.TotalQuantity.Value) / (f.AcSum) : grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month4 && t.StartDate.Value.Year == year4).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStock5 += f.AcSum > 0 ? grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month5 && t.StartDate.Value.Year == year5).Sum(s => s.TotalQuantity.Value) / (f.AcSum) : grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month5 && t.StartDate.Value.Year == year5).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStock6 += f.AcSum > 0 ? grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month6 && t.StartDate.Value.Year == year6).Sum(s => s.TotalQuantity.Value) / (f.AcSum) : grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month6 && t.StartDate.Value.Year == year6).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStock7 += f.AcSum > 0 ? grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month7 && t.StartDate.Value.Year == year7).Sum(s => s.TotalQuantity.Value) / (f.AcSum) : grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month7 && t.StartDate.Value.Year == year7).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStock8 += f.AcSum > 0 ? grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month8 && t.StartDate.Value.Year == year8).Sum(s => s.TotalQuantity.Value) / (f.AcSum) : grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month8 && t.StartDate.Value.Year == year8).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStock9 += f.AcSum > 0 ? grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month9 && t.StartDate.Value.Year == year9).Sum(s => s.TotalQuantity.Value) / (f.AcSum) : grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month9 && t.StartDate.Value.Year == year9).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStock10 += f.AcSum > 0 ? grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month10 && t.StartDate.Value.Year == year10).Sum(s => s.TotalQuantity.Value) / (f.AcSum) : grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month10 && t.StartDate.Value.Year == year10).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStock11 += f.AcSum > 0 ? grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month11 && t.StartDate.Value.Year == year11).Sum(s => s.TotalQuantity.Value) / (f.AcSum) : grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month11 && t.StartDate.Value.Year == year11).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStock12 += f.AcSum > 0 ? grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month12 && t.StartDate.Value.Year == year12).Sum(s => s.TotalQuantity.Value) / (f.AcSum) : grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month12 && t.StartDate.Value.Year == year12).Sum(s => s.TotalQuantity.Value);




                f.ProjectedHoldingStockQty1 = prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month1).Sum(s => s.Quantity);
                f.ProjectedHoldingStockQty2 = prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month2).Sum(s => s.Quantity);
                f.ProjectedHoldingStockQty3 = prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month3).Sum(s => s.Quantity);
                f.ProjectedHoldingStockQty4 = prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month4).Sum(s => s.Quantity);
                f.ProjectedHoldingStockQty5 = prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month5).Sum(s => s.Quantity);
                f.ProjectedHoldingStockQty6 = prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month6).Sum(s => s.Quantity);
                f.ProjectedHoldingStockQty7 = prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month7).Sum(s => s.Quantity);
                f.ProjectedHoldingStockQty8 = prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month8).Sum(s => s.Quantity);
                f.ProjectedHoldingStockQty9 = prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month9).Sum(s => s.Quantity);
                f.ProjectedHoldingStockQty10 = prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month10).Sum(s => s.Quantity);
                f.ProjectedHoldingStockQty11 = prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month11).Sum(s => s.Quantity);
                f.ProjectedHoldingStockQty12 = prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month12).Sum(s => s.Quantity);

                f.ProjectedHoldingStockQty1 += grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month1 && t.StartDate.Value.Year == year1).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStockQty2 += grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month2 && t.StartDate.Value.Year == year2).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStockQty3 += grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month3 && t.StartDate.Value.Year == year3).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStockQty4 += grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month4 && t.StartDate.Value.Year == year4).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStockQty5 += grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month5 && t.StartDate.Value.Year == year5).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStockQty6 += grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month6 && t.StartDate.Value.Year == year6).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStockQty7 += grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month7 && t.StartDate.Value.Year == year7).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStockQty8 += grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month8 && t.StartDate.Value.Year == year8).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStockQty9 += grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month9 && t.StartDate.Value.Year == year9).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStockQty10 += grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month10 && t.StartDate.Value.Year == year10).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStockQty11 += grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month11 && t.StartDate.Value.Year == year11).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStockQty12 += grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month12 && t.StartDate.Value.Year == year12).Sum(s => s.TotalQuantity.Value);

                f.OutputProjectedHoldingStockQty1 = prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month1).Sum(s => s.Quantity);
                f.OutputProjectedHoldingStockQty2 = prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month2).Sum(s => s.Quantity);
                f.OutputProjectedHoldingStockQty3 = prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month3).Sum(s => s.Quantity);
                f.OutputProjectedHoldingStockQty4 = prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month4).Sum(s => s.Quantity);
                f.OutputProjectedHoldingStockQty5 = prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month5).Sum(s => s.Quantity);
                f.OutputProjectedHoldingStockQty6 = prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month6).Sum(s => s.Quantity);
                f.OutputProjectedHoldingStockQty7 = prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month7).Sum(s => s.Quantity);
                f.OutputProjectedHoldingStockQty8 = prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month8).Sum(s => s.Quantity);
                f.OutputProjectedHoldingStockQty9 = prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month9).Sum(s => s.Quantity);
                f.OutputProjectedHoldingStockQty10 = prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month10).Sum(s => s.Quantity);
                f.OutputProjectedHoldingStockQty11 = prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month11).Sum(s => s.Quantity);
                f.OutputProjectedHoldingStockQty12 = prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month12).Sum(s => s.Quantity);



                f.TicketHoldingStock1 = f.ProductionTicket1 > 0 ? f.AcSum == 0 ? (f.ProductionTicket1) / 1 : (f.ProductionTicket1) / f.AcSum : 0;
                f.TicketHoldingStock2 = f.ProductionTicket2 > 0 ? f.AcSum == 0 ? (f.ProductionTicket2) / 1 : (f.ProductionTicket2) / f.AcSum : 0;
                f.TicketHoldingStock3 = f.ProductionTicket3 > 0 ? f.AcSum == 0 ? (f.ProductionTicket3) / 1 : (f.ProductionTicket3) / f.AcSum : 0;
                f.TicketHoldingStock4 = f.ProductionTicket4 > 0 ? f.AcSum == 0 ? (f.ProductionTicket4) / 1 : (f.ProductionTicket4) / f.AcSum : 0;
                f.TicketHoldingStock5 = f.ProductionTicket5 > 0 ? f.AcSum == 0 ? (f.ProductionTicket5) / 1 : (f.ProductionTicket5) / f.AcSum : 0;
                f.TicketHoldingStock6 = f.ProductionTicket6 > 0 ? f.AcSum == 0 ? (f.ProductionTicket6) / 1 : (f.ProductionTicket6) / f.AcSum : 0;
                f.TicketHoldingStock7 = f.ProductionTicket7 > 0 ? f.AcSum == 0 ? (f.ProductionTicket7) / 1 : (f.ProductionTicket7) / f.AcSum : 0;
                f.TicketHoldingStock8 = f.ProductionTicket8 > 0 ? f.AcSum == 0 ? (f.ProductionTicket8) / 1 : (f.ProductionTicket8) / f.AcSum : 0;
                f.TicketHoldingStock9 = f.ProductionTicket9 > 0 ? f.AcSum == 0 ? (f.ProductionTicket9) / 1 : (f.ProductionTicket9) / f.AcSum : 0;
                f.TicketHoldingStock10 = f.ProductionTicket10 > 0 ? f.AcSum == 0 ? (f.ProductionTicket10) / 1 : (f.ProductionTicket10) / f.AcSum : 0;
                f.TicketHoldingStock11 = f.ProductionTicket11 > 0 ? f.AcSum == 0 ? (f.ProductionTicket11) / 1 : (f.ProductionTicket11) / f.AcSum : 0;
                f.TicketHoldingStock12 = f.ProductionTicket12 > 0 ? f.AcSum == 0 ? (f.ProductionTicket12) / 1 : (f.ProductionTicket12) / f.AcSum : 0;



                //if (f.isTenderExist)
                //{
                //    f.StockHoldingBalance = 0;
                //}

                //f.QtyMonth1 = (f.StockBalance - f.AcSum) > 0 ? f.StockBalance - f.AcSum : 0;
                //f.QtyProductionProjected1 = (f.QtyMonth1 + f.ProjectedHoldingStockQty1 + f.OutputProjectedHoldingStockQty1) - f.BlanketAddhoc1;

                //f.QtyMonth2 = (f.QtyProductionProjected1 - f.AcSum) > 0 ? f.QtyProductionProjected1 - f.AcSum : 0;
                //f.QtyProductionProjected2 = (f.QtyMonth2 + f.ProjectedHoldingStockQty2 + f.OutputProjectedHoldingStockQty2) - f.BlanketAddhoc2;

                //f.QtyMonth3 = (f.QtyProductionProjected2 - f.AcSum) > 0 ? f.QtyProductionProjected2 - f.AcSum : 0;
                //f.QtyProductionProjected3 = (f.QtyMonth3 + f.ProjectedHoldingStockQty3 + f.OutputProjectedHoldingStockQty3) - f.BlanketAddhoc3;

                //f.QtyMonth4 = (f.QtyProductionProjected3 - f.AcSum) > 0 ? f.QtyProductionProjected3 - f.AcSum : 0;
                //f.QtyProductionProjected4 = (f.QtyMonth4 + f.ProjectedHoldingStockQty4 + f.OutputProjectedHoldingStockQty4) - f.BlanketAddhoc4;

                //f.QtyMonth5 = (f.QtyProductionProjected4 - f.AcSum) > 0 ? f.QtyProductionProjected4 - f.AcSum : 0;
                //f.QtyProductionProjected5 = (f.QtyMonth5 + f.ProjectedHoldingStockQty5 + f.OutputProjectedHoldingStockQty5) - f.BlanketAddhoc5;

                //f.QtyMonth6 = (f.QtyProductionProjected5 - f.AcSum) > 0 ? f.QtyProductionProjected5 - f.AcSum : 0;
                //f.QtyProductionProjected6 = (f.QtyMonth6 + f.ProjectedHoldingStockQty6 + f.OutputProjectedHoldingStockQty6) - f.BlanketAddhoc6;

                //f.QtyMonth7 = (f.QtyProductionProjected6 - f.AcSum) > 0 ? f.QtyProductionProjected6 - f.AcSum : 0;
                //f.QtyProductionProjected7 = (f.QtyMonth7 + f.ProjectedHoldingStockQty7 + f.OutputProjectedHoldingStockQty7) - f.BlanketAddhoc7;

                //f.QtyMonth8 = (f.QtyProductionProjected7 - f.AcSum) > 0 ? f.QtyProductionProjected7 - f.AcSum : 0;
                //f.QtyProductionProjected8 = (f.QtyMonth8 + f.ProjectedHoldingStockQty8 + f.OutputProjectedHoldingStockQty8) - f.BlanketAddhoc8;

                //f.QtyMonth9 = (f.QtyProductionProjected8 - f.AcSum) > 0 ? f.QtyProductionProjected8 - f.AcSum : 0;
                //f.QtyProductionProjected9 = (f.QtyMonth9 + f.ProjectedHoldingStockQty9 + f.OutputProjectedHoldingStockQty9) - f.BlanketAddhoc9;

                //f.QtyMonth10 = (f.QtyProductionProjected9 - f.AcSum) > 0 ? f.QtyProductionProjected9 - f.AcSum : 0;
                //f.QtyProductionProjected10 = (f.QtyMonth10 + f.ProjectedHoldingStockQty10 + f.OutputProjectedHoldingStockQty10) - f.BlanketAddhoc10;

                //f.QtyMonth11 = (f.QtyProductionProjected10 - f.AcSum) > 0 ? f.QtyProductionProjected10 - f.AcSum : 0;
                //f.QtyProductionProjected11 = (f.QtyMonth11 + f.ProjectedHoldingStockQty11 + f.OutputProjectedHoldingStockQty11) - f.BlanketAddhoc11;

                //f.QtyMonth12 = (f.QtyProductionProjected11 - f.AcSum) > 0 ? f.QtyProductionProjected11 - f.AcSum : 0;
                //f.QtyProductionProjected12 = (f.QtyMonth12 + f.ProjectedHoldingStockQty12 + f.OutputProjectedHoldingStockQty12) - f.BlanketAddhoc12;


                f.QtyMonth1 = f.StockBalance - f.AcSum;
                f.QtyProductionProjected1 = (f.QtyMonth1 + f.ProjectedHoldingStockQty1 + f.OutputProjectedHoldingStockQty1) - f.BlanketAddhoc1;

                f.QtyMonth2 = f.QtyProductionProjected1 - f.AcSum;
                f.QtyProductionProjected2 = (f.QtyMonth2 + f.ProjectedHoldingStockQty2 + f.OutputProjectedHoldingStockQty2) - f.BlanketAddhoc2;

                f.QtyMonth3 = f.QtyProductionProjected2 - f.AcSum;
                f.QtyProductionProjected3 = (f.QtyMonth3 + f.ProjectedHoldingStockQty3 + f.OutputProjectedHoldingStockQty3) - f.BlanketAddhoc3;

                f.QtyMonth4 = f.QtyProductionProjected3 - f.AcSum;
                f.QtyProductionProjected4 = (f.QtyMonth4 + f.ProjectedHoldingStockQty4 + f.OutputProjectedHoldingStockQty4) - f.BlanketAddhoc4;

                f.QtyMonth5 = f.QtyProductionProjected4 - f.AcSum;
                f.QtyProductionProjected5 = (f.QtyMonth5 + f.ProjectedHoldingStockQty5 + f.OutputProjectedHoldingStockQty5) - f.BlanketAddhoc5;

                f.QtyMonth6 = f.QtyProductionProjected5 - f.AcSum;
                f.QtyProductionProjected6 = (f.QtyMonth6 + f.ProjectedHoldingStockQty6 + f.OutputProjectedHoldingStockQty6) - f.BlanketAddhoc6;

                f.QtyMonth7 = f.QtyProductionProjected6 - f.AcSum;
                f.QtyProductionProjected7 = (f.QtyMonth7 + f.ProjectedHoldingStockQty7 + f.OutputProjectedHoldingStockQty7) - f.BlanketAddhoc7;

                f.QtyMonth8 = f.QtyProductionProjected7 - f.AcSum;
                f.QtyProductionProjected8 = (f.QtyMonth8 + f.ProjectedHoldingStockQty8 + f.OutputProjectedHoldingStockQty8) - f.BlanketAddhoc8;

                f.QtyMonth9 = f.QtyProductionProjected8 - f.AcSum;
                f.QtyProductionProjected9 = (f.QtyMonth9 + f.ProjectedHoldingStockQty9 + f.OutputProjectedHoldingStockQty9) - f.BlanketAddhoc9;

                f.QtyMonth10 = f.QtyProductionProjected9 - f.AcSum;
                f.QtyProductionProjected10 = (f.QtyMonth10 + f.ProjectedHoldingStockQty10 + f.OutputProjectedHoldingStockQty10) - f.BlanketAddhoc10;

                f.QtyMonth11 = f.QtyProductionProjected10 - f.AcSum;
                f.QtyProductionProjected11 = (f.QtyMonth11 + f.ProjectedHoldingStockQty11 + f.OutputProjectedHoldingStockQty11) - f.BlanketAddhoc11;

                f.QtyMonth12 = f.QtyProductionProjected11 - f.AcSum;
                f.QtyProductionProjected12 = (f.QtyMonth12 + f.ProjectedHoldingStockQty12 + f.OutputProjectedHoldingStockQty12) - f.BlanketAddhoc12;

                //f.ProductionProjected1 = f.Month1 + f.ProjectedHoldingStock1 + f.TicketHoldingStock1 + f.OutputProjectedHoldingStock1;
                //f.Month2 = f.AcSum > 0 ? f.ProductionProjected1 - 1 > 0 ? f.ProductionProjected1 - 1 : 0 : f.ProductionProjected1;
                //f.ProductionProjected2 = f.Month2 + f.ProjectedHoldingStock2 + f.TicketHoldingStock2 + f.OutputProjectedHoldingStock2;
                //f.Month3 = f.AcSum > 0 ? f.ProductionProjected2 - 1 > 0 ? f.ProductionProjected2 - 1 : 0 : f.ProductionProjected2;
                //f.ProductionProjected3 = f.Month3 + f.ProjectedHoldingStock3 + f.TicketHoldingStock3 + f.OutputProjectedHoldingStock3;
                //f.Month4 = f.AcSum > 0 ? f.ProductionProjected3 - 1 > 0 ? f.ProductionProjected3 - 1 : 0 : f.ProductionProjected3;
                //f.ProductionProjected4 = f.Month4 + f.ProjectedHoldingStock4 + f.TicketHoldingStock4 + f.OutputProjectedHoldingStock4;
                //f.Month5 = f.AcSum > 0 ? f.ProductionProjected4 - 1 > 0 ? f.ProductionProjected4 - 1 : 0 : f.ProductionProjected4;
                //f.ProductionProjected5 = f.Month5 + f.ProjectedHoldingStock5 + f.TicketHoldingStock5 + f.OutputProjectedHoldingStock5;
                //f.Month6 = f.AcSum > 0 ? f.ProductionProjected5 - 1 > 0 ? f.ProductionProjected5 - 1 : 0 : f.ProductionProjected5;
                //f.ProductionProjected6 = f.Month6 + f.ProjectedHoldingStock6 + f.TicketHoldingStock6 + f.OutputProjectedHoldingStock6;
                //f.Month7 = f.AcSum > 0 ? f.ProductionProjected6 - 1 > 0 ? f.ProductionProjected6 - 1 : 0 : f.ProductionProjected6;
                //f.ProductionProjected7 = f.Month7 + f.ProjectedHoldingStock7 + f.TicketHoldingStock7 + f.OutputProjectedHoldingStock7;
                //f.Month8 = f.AcSum > 0 ? f.ProductionProjected7 - 1 > 0 ? f.ProductionProjected7 - 1 : 0 : f.ProductionProjected7;
                //f.ProductionProjected8 = f.Month8 + f.ProjectedHoldingStock8 + f.TicketHoldingStock8 + f.OutputProjectedHoldingStock8;
                //f.Month9 = f.AcSum > 0 ? f.ProductionProjected8 - 1 > 0 ? f.ProductionProjected8 - 1 : 0 : f.ProductionProjected8;
                //f.ProductionProjected9 = f.Month9 + f.ProjectedHoldingStock9 + f.TicketHoldingStock9 + f.OutputProjectedHoldingStock9;
                //f.Month10 = f.AcSum > 0 ? f.ProductionProjected9 - 1 > 0 ? f.ProductionProjected9 - 1 : 0 : f.ProductionProjected9;
                //f.ProductionProjected10 = f.Month10 + f.ProjectedHoldingStock10 + f.TicketHoldingStock10 + f.OutputProjectedHoldingStock10;
                //f.Month11 = f.AcSum > 0 ? f.ProductionProjected10 - 1 > 0 ? f.ProductionProjected10 - 1 : 0 : f.ProductionProjected10;
                //f.ProductionProjected11 = f.Month11 + f.ProjectedHoldingStock11 + f.TicketHoldingStock11 + f.OutputProjectedHoldingStock11;
                //f.Month12 = f.AcSum > 0 ? f.ProductionProjected11 - 1 > 0 ? f.ProductionProjected11 - 1 : 0 : f.ProductionProjected11;
                //f.ProductionProjected12 = f.Month12 + f.ProjectedHoldingStock12 + f.TicketHoldingStock12 + f.OutputProjectedHoldingStock12;

                f.ProductionProjected1 = f.Month1 + f.ProjectedHoldingStock1 + 0 + f.OutputProjectedHoldingStock1;
                f.Month2 = f.AcSum > 0 ? f.ProductionProjected1 - 1 > 0 ? f.ProductionProjected1 - 1 : 0 : f.ProductionProjected1;
                f.ProductionProjected2 = f.Month2 + f.ProjectedHoldingStock2 + 0 + f.OutputProjectedHoldingStock2;
                f.Month3 = f.AcSum > 0 ? f.ProductionProjected2 - 1 > 0 ? f.ProductionProjected2 - 1 : 0 : f.ProductionProjected2;
                f.ProductionProjected3 = f.Month3 + f.ProjectedHoldingStock3 + 0 + f.OutputProjectedHoldingStock3;
                f.Month4 = f.AcSum > 0 ? f.ProductionProjected3 - 1 > 0 ? f.ProductionProjected3 - 1 : 0 : f.ProductionProjected3;
                f.ProductionProjected4 = f.Month4 + f.ProjectedHoldingStock4 + 0 + f.OutputProjectedHoldingStock4;
                f.Month5 = f.AcSum > 0 ? f.ProductionProjected4 - 1 > 0 ? f.ProductionProjected4 - 1 : 0 : f.ProductionProjected4;
                f.ProductionProjected5 = f.Month5 + f.ProjectedHoldingStock5 + 0 + f.OutputProjectedHoldingStock5;
                f.Month6 = f.AcSum > 0 ? f.ProductionProjected5 - 1 > 0 ? f.ProductionProjected5 - 1 : 0 : f.ProductionProjected5;
                f.ProductionProjected6 = f.Month6 + f.ProjectedHoldingStock6 + 0 + f.OutputProjectedHoldingStock6;
                f.Month7 = f.AcSum > 0 ? f.ProductionProjected6 - 1 > 0 ? f.ProductionProjected6 - 1 : 0 : f.ProductionProjected6;
                f.ProductionProjected7 = f.Month7 + f.ProjectedHoldingStock7 + 0 + f.OutputProjectedHoldingStock7;
                f.Month8 = f.AcSum > 0 ? f.ProductionProjected7 - 1 > 0 ? f.ProductionProjected7 - 1 : 0 : f.ProductionProjected7;
                f.ProductionProjected8 = f.Month8 + f.ProjectedHoldingStock8 + 0 + f.OutputProjectedHoldingStock8;
                f.Month9 = f.AcSum > 0 ? f.ProductionProjected8 - 1 > 0 ? f.ProductionProjected8 - 1 : 0 : f.ProductionProjected8;
                f.ProductionProjected9 = f.Month9 + f.ProjectedHoldingStock9 + 0 + f.OutputProjectedHoldingStock9;
                f.Month10 = f.AcSum > 0 ? f.ProductionProjected9 - 1 > 0 ? f.ProductionProjected9 - 1 : 0 : f.ProductionProjected9;
                f.ProductionProjected10 = f.Month10 + f.ProjectedHoldingStock10 + 0 + f.OutputProjectedHoldingStock10;
                f.Month11 = f.AcSum > 0 ? f.ProductionProjected10 - 1 > 0 ? f.ProductionProjected10 - 1 : 0 : f.ProductionProjected10;
                f.ProductionProjected11 = f.Month11 + f.ProjectedHoldingStock11 + 0 + f.OutputProjectedHoldingStock11;
                f.Month12 = f.AcSum > 0 ? f.ProductionProjected11 - 1 > 0 ? f.ProductionProjected11 - 1 : 0 : f.ProductionProjected11;
                f.ProductionProjected12 = f.Month12 + f.ProjectedHoldingStock12 + 0 + f.OutputProjectedHoldingStock12;




                if (f.IsTenderExist1 || f.AcSum <= 0)
                {
                    f.Month1 = f.QtyMonth1;
                    f.ProductionProjected1 = f.QtyProductionProjected1;
                    f.ProjectedHoldingStock1 = f.ProjectedHoldingStockQty1;
                    f.OutputProjectedHoldingStock1 = f.OutputProjectedHoldingStockQty1;
                }
                else
                {
                    if (f.isTenderExist)
                    {
                        f.Month1 = f.QtyMonth1 / f.AcSum;
                        f.ProductionProjected1 = f.QtyProductionProjected1 / f.AcSum;
                    }
                }
                if (f.IsTenderExist2 || f.AcSum <= 0)
                {
                    f.Month2 = f.QtyMonth2;
                    f.ProductionProjected2 = f.QtyProductionProjected2;
                    f.ProjectedHoldingStock2 = f.ProjectedHoldingStockQty2;
                    f.OutputProjectedHoldingStock2 = f.OutputProjectedHoldingStockQty2;
                }
                else
                {
                    if (f.isTenderExist)
                    {
                        f.Month2 = f.QtyMonth2 / f.AcSum;
                        f.ProductionProjected2 = f.QtyProductionProjected2 / f.AcSum;
                    }
                }
                if (f.IsTenderExist3 || f.AcSum <= 0)
                {
                    f.Month3 = f.QtyMonth3;
                    f.ProductionProjected3 = f.QtyProductionProjected3;
                    f.ProjectedHoldingStock3 = f.ProjectedHoldingStockQty3;
                    f.OutputProjectedHoldingStock3 = f.OutputProjectedHoldingStockQty3;
                }
                else
                {
                    if (f.isTenderExist)
                    {
                        f.Month3 = f.QtyMonth3 / f.AcSum;
                        f.ProductionProjected3 = f.QtyProductionProjected3 / f.AcSum;
                    }
                }
                if (f.IsTenderExist4 || f.AcSum <= 0)
                {
                    f.Month4 = f.QtyMonth4;
                    f.ProductionProjected4 = f.QtyProductionProjected4;
                    f.ProjectedHoldingStock4 = f.ProjectedHoldingStockQty4;
                    f.OutputProjectedHoldingStock4 = f.OutputProjectedHoldingStockQty4;
                }
                else
                {
                    if (f.isTenderExist)
                    {
                        f.Month4 = f.QtyMonth4 / f.AcSum;
                        f.ProductionProjected4 = f.QtyProductionProjected4 / f.AcSum;
                    }
                }
                if (f.IsTenderExist5 || f.AcSum <= 0)
                {
                    f.Month5 = f.QtyMonth5;
                    f.ProductionProjected5 = f.QtyProductionProjected5;
                    f.ProjectedHoldingStock5 = f.ProjectedHoldingStockQty5;
                    f.OutputProjectedHoldingStock5 = f.OutputProjectedHoldingStockQty5;
                }
                else
                {
                    if (f.isTenderExist)
                    {
                        f.Month5 = f.QtyMonth5 / f.AcSum;
                        f.ProductionProjected5 = f.QtyProductionProjected5 / f.AcSum;
                    }
                }
                if (f.IsTenderExist6 || f.AcSum <= 0)
                {
                    f.Month6 = f.QtyMonth6;
                    f.ProductionProjected6 = f.QtyProductionProjected6;
                    f.ProjectedHoldingStock6 = f.ProjectedHoldingStockQty6;
                    f.OutputProjectedHoldingStock6 = f.OutputProjectedHoldingStockQty6;
                }
                else
                {
                    if (f.isTenderExist)
                    {
                        f.Month6 = f.QtyMonth6 / f.AcSum;
                        f.ProductionProjected6 = f.QtyProductionProjected6 / f.AcSum;
                    }
                }
                if (f.IsTenderExist7 || f.AcSum <= 0)
                {
                    f.Month7 = f.QtyMonth7;
                    f.ProductionProjected7 = f.QtyProductionProjected7;
                    f.ProjectedHoldingStock7 = f.ProjectedHoldingStockQty7;
                    f.OutputProjectedHoldingStock7 = f.OutputProjectedHoldingStockQty7;
                }
                else
                {
                    if (f.isTenderExist)
                    {
                        f.Month7 = f.QtyMonth7 / f.AcSum;
                        f.ProductionProjected7 = f.QtyProductionProjected7 / f.AcSum;
                    }
                }
                if (f.IsTenderExist8 || f.AcSum <= 0)
                {
                    f.Month8 = f.QtyMonth8;
                    f.ProductionProjected8 = f.QtyProductionProjected8;
                    f.ProjectedHoldingStock8 = f.ProjectedHoldingStockQty8;
                    f.OutputProjectedHoldingStock8 = f.OutputProjectedHoldingStockQty8;
                }
                else
                {
                    if (f.isTenderExist)
                    {
                        f.Month8 = f.QtyMonth8 / f.AcSum;
                        f.ProductionProjected8 = f.QtyProductionProjected8 / f.AcSum;
                    }
                }
                if (f.IsTenderExist9 || f.AcSum <= 0)
                {
                    f.Month9 = f.QtyMonth9;
                    f.ProductionProjected9 = f.QtyProductionProjected9;
                    f.ProjectedHoldingStock9 = f.ProjectedHoldingStockQty9;
                    f.OutputProjectedHoldingStock9 = f.OutputProjectedHoldingStockQty9;
                }
                else
                {
                    if (f.isTenderExist)
                    {
                        f.Month9 = f.QtyMonth9 / f.AcSum;
                        f.ProductionProjected9 = f.QtyProductionProjected9 / f.AcSum;
                    }
                }
                if (f.IsTenderExist10 || f.AcSum <= 0)
                {
                    f.Month10 = f.QtyMonth10;
                    f.ProductionProjected10 = f.QtyProductionProjected10;
                    f.ProjectedHoldingStock10 = f.ProjectedHoldingStockQty10;
                    f.OutputProjectedHoldingStock10 = f.OutputProjectedHoldingStockQty10;
                }
                else
                {
                    if (f.isTenderExist)
                    {
                        f.Month10 = f.QtyMonth10 / f.AcSum;
                        f.ProductionProjected10 = f.QtyProductionProjected10 / f.AcSum;
                    }
                }
                if (f.IsTenderExist11 || f.AcSum <= 0)
                {
                    f.Month11 = f.QtyMonth11;
                    f.ProductionProjected11 = f.QtyProductionProjected11;
                    f.ProjectedHoldingStock11 = f.ProjectedHoldingStockQty11;
                }
                else
                {
                    if (f.isTenderExist)
                    {
                        f.Month11 = f.QtyMonth11 / f.AcSum;
                        f.ProductionProjected11 = f.QtyProductionProjected11 / f.AcSum;
                    }
                }
                if (f.IsTenderExist12 || f.AcSum <= 0)
                {
                    f.Month12 = f.QtyMonth12;
                    f.ProductionProjected12 = f.QtyProductionProjected12;
                    f.ProjectedHoldingStock12 = f.ProjectedHoldingStockQty12;
                    f.OutputProjectedHoldingStock12 = f.OutputProjectedHoldingStockQty12;
                }
                else
                {
                    if (f.isTenderExist)
                    {
                        f.Month12 = f.QtyMonth12 / f.AcSum;
                        f.ProductionProjected12 = f.QtyProductionProjected12 / f.AcSum;
                    }
                }

                //f.Month1 = f.Month1 < 0 ? 0 : f.Month1;
                //f.Month2 = f.Month2 < 0 ? 0 : f.Month2;
                //f.Month3 = f.Month3 < 0 ? 0 : f.Month3;
                //f.Month4 = f.Month4 < 0 ? 0 : f.Month4;
                //f.Month5 = f.Month5 < 0 ? 0 : f.Month5;
                //f.Month6 = f.Month6 < 0 ? 0 : f.Month6;
                //f.Month7 = f.Month7 < 0 ? 0 : f.Month7;
                //f.Month8 = f.Month8 < 0 ? 0 : f.Month8;
                //f.Month9 = f.Month9 < 0 ? 0 : f.Month9;
                //f.Month10 = f.Month10 < 0 ? 0 : f.Month10;
                //f.Month11 = f.Month11 < 0 ? 0 : f.Month11;
                //f.Month12 = f.Month12 < 0 ? 0 : f.Month12;

                //f.ProductionProjected1 = f.ProductionProjected1 < 0 ? 0 : f.ProductionProjected1;
                //f.ProductionProjected2 = f.ProductionProjected2 < 0 ? 0 : f.ProductionProjected2;
                //f.ProductionProjected3 = f.ProductionProjected3 < 0 ? 0 : f.ProductionProjected3;
                //f.ProductionProjected4 = f.ProductionProjected4 < 0 ? 0 : f.ProductionProjected4;
                //f.ProductionProjected5 = f.ProductionProjected5 < 0 ? 0 : f.ProductionProjected5;
                //f.ProductionProjected6 = f.ProductionProjected6 < 0 ? 0 : f.ProductionProjected6;
                //f.ProductionProjected7 = f.ProductionProjected7 < 0 ? 0 : f.ProductionProjected7;
                //f.ProductionProjected8 = f.ProductionProjected8 < 0 ? 0 : f.ProductionProjected8;
                //f.ProductionProjected9 = f.ProductionProjected9 < 0 ? 0 : f.ProductionProjected9;
                //f.ProductionProjected10 = f.ProductionProjected10 < 0 ? 0 : f.ProductionProjected10;
                //f.ProductionProjected11 = f.ProductionProjected11 < 0 ? 0 : f.ProductionProjected11;
                //f.ProductionProjected12 = f.ProductionProjected12 < 0 ? 0 : f.ProductionProjected12;

            });

            genericId = new List<long?>();
            //var customer = new List<string>();
            itemMasterforReport.ForEach(ac =>
            {
                var customer = new List<string>();

                var symlQty = itemdict.FirstOrDefault(f => f.Key == ac.No).Value;

                if (!genericId.Exists(g => g == ac.GenericCodeId) && ac.GenericCodeId != null)
                {
                    genericId.Add(ac.GenericCodeId);
                    var itemNos = itemMasterforReport.Where(i => i.GenericCodeId == ac.GenericCodeId).Select(s => s.ItemId).ToList();
                    // var acQty = MethodCodeList.Where(f => f.ItemId == ac.ItemId).Sum(s=>s.AcSum);
                    blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1))).ToList().ForEach(adhoc =>
                    {

                        if (!customer.Exists(c => c == adhoc.Categories) && !string.IsNullOrEmpty(adhoc.Categories))
                        {
                            customer.Add(adhoc.Categories);
                            MethodCodeList.Add(new INPCalendarPivotModel
                            {
                                ItemId = ac.ItemId,
                                ItemNo = ac.GenericCodeDescription2,
                                IsSteroid = ac.Steroid.GetValueOrDefault(false),
                                SalesCategoryId = ac.CategoryId,
                                SalesCategory = salesCatLists.Where(w => w.Id == ac.CategoryId).FirstOrDefault()?.Text,
                                LocationID = categoryItems != null && categoryItems.FirstOrDefault(a => a.SaleCategoryId == ac.CategoryId) != null ? categoryItems.FirstOrDefault(a => a.SaleCategoryId == ac.CategoryId).LocationId : null,
                                // SalesCategory = categoryItems != null && categoryItems.FirstOrDefault(a => a.SaleCategoryId == ac.CategoryId) != null ? categoryItems.FirstOrDefault(a => a.SaleCategoryId == ac.CategoryId).Code : "",
                                MethodCodeId = ac.NavMethodCodeLines.Count > 0 ? ac.NavMethodCodeLines.First().MethodCodeId : -1,
                                GenericCodeID = ac.GenericCodeId,
                                AddhocCust = adhoc.Categories,
                                Description = ac.Description + " " + ac.Description2,
                                MethodCode = ac.NavMethodCodeLines.Count > 0 ? ac.NavMethodCodeLines.First().MethodName : "No MethodCode",
                                Month = endDate.StockMonth.ToString("MMMM"),
                                Remarks = "Tender",
                                PackSize = ac.PackSize.HasValue ? ac.PackSize.Value : 900,
                                PackSize2 = packSize2,
                                BlanketAddhoc1 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.Categories == adhoc.Categories && t.ShipmentDate.Value.Month == month1 && t.ShipmentDate.Value.Year == year1).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                                BlanketAddhoc2 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.Categories == adhoc.Categories && t.ShipmentDate.Value.Month == month2 && t.ShipmentDate.Value.Year == year2).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                                BlanketAddhoc3 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.Categories == adhoc.Categories && t.ShipmentDate.Value.Month == month3 && t.ShipmentDate.Value.Year == year3).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                                BlanketAddhoc4 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.Categories == adhoc.Categories && t.ShipmentDate.Value.Month == month4 && t.ShipmentDate.Value.Year == year4).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                                BlanketAddhoc5 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.Categories == adhoc.Categories && t.ShipmentDate.Value.Month == month5 && t.ShipmentDate.Value.Year == year5).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                                BlanketAddhoc6 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.Categories == adhoc.Categories && t.ShipmentDate.Value.Month == month6 && t.ShipmentDate.Value.Year == year6).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                                BlanketAddhoc7 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.Categories == adhoc.Categories && t.ShipmentDate.Value.Month == month7 && t.ShipmentDate.Value.Year == year7).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                                BlanketAddhoc8 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.Categories == adhoc.Categories && t.ShipmentDate.Value.Month == month8 && t.ShipmentDate.Value.Year == year8).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                                BlanketAddhoc9 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.Categories == adhoc.Categories && t.ShipmentDate.Value.Month == month9 && t.ShipmentDate.Value.Year == year9).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                                BlanketAddhoc10 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.Categories == adhoc.Categories && t.ShipmentDate.Value.Month == month10 && t.ShipmentDate.Value.Year == year10).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                                BlanketAddhoc11 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.Categories == adhoc.Categories && t.ShipmentDate.Value.Month == month11 && t.ShipmentDate.Value.Year == year11).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                                BlanketAddhoc12 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.Categories == adhoc.Categories && t.ShipmentDate.Value.Month == month12 && t.ShipmentDate.Value.Year == year12).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                                isTenderExist = true,
                                SymlQty = symlQty,
                                Itemgrouping = symlQty > 0 ? "Adhoc orders - Item with AC" : "Stand alone"
                            });
                        }
                    });
                }

            });

            var groupedResult = MethodCodeList.GroupBy(g => g.MethodCode).ToList();
            var resultData = new List<INPCalendarPivotModel>();
            groupedResult.ForEach(f =>
            {
                if (f.Key.Contains("SW Vitamin C Tablet"))
                {

                }
                f.ToList().ForEach(g =>
                {
                    if (g.SymlQty > 0 && g.Remarks == "AC")
                    {
                        resultData.Add(g);
                    }
                    else
                    {
                        if (g.Remarks == "AC")
                        {
                            var tenderExist = f.Any(t => t.Remarks == "Tender" && t.ItemNo == g.ItemNo && (t.BlanketAddhoc1 > 0 || t.BlanketAddhoc2 > 0 || t.BlanketAddhoc3 > 0 || t.BlanketAddhoc4 > 0 || t.BlanketAddhoc5 > 0
                            || t.BlanketAddhoc6 > 0 || t.BlanketAddhoc7 > 0 || t.BlanketAddhoc8 > 0 || t.BlanketAddhoc9 > 0 || t.BlanketAddhoc10 > 0 || t.BlanketAddhoc11 > 0 || t.BlanketAddhoc12 > 0));
                            if (tenderExist)
                            {
                                resultData.Add(g);
                            }
                            else
                            {
                                if (g.ProjectedHoldingStock1 > 0 || g.ProjectedHoldingStock2 > 0 || g.ProjectedHoldingStock3 > 0 || g.ProjectedHoldingStock4 > 0 || g.ProjectedHoldingStock5 > 0
                      || g.ProjectedHoldingStock6 > 0 || g.ProjectedHoldingStock7 > 0 || g.ProjectedHoldingStock8 > 0 || g.ProjectedHoldingStock9 > 0 || g.ProjectedHoldingStock10 > 0 || g.ProjectedHoldingStock11 > 0 || g.ProjectedHoldingStock12 > 0)
                                {
                                    resultData.Add(g);
                                }
                            }
                        }
                        else
                        {
                            if (g.BlanketAddhoc1 > 0 || g.BlanketAddhoc2 > 0 || g.BlanketAddhoc3 > 0 || g.BlanketAddhoc4 > 0 || g.BlanketAddhoc5 > 0
                            || g.BlanketAddhoc6 > 0 || g.BlanketAddhoc7 > 0 || g.BlanketAddhoc8 > 0 || g.BlanketAddhoc9 > 0 || g.BlanketAddhoc10 > 0 || g.BlanketAddhoc11 > 0 || g.BlanketAddhoc12 > 0)
                            {
                                resultData.Add(g);
                            }
                        }
                    }
                });

            });

            return resultData.ToList();
        }
        public async Task<IReadOnlyList<INPCalendarPivotModel>> GetSimulationAddhocV5(DateRangeModel dateRangeModel)
        {
            var companyIds = new List<long?> { dateRangeModel.CompanyId };
            if (dateRangeModel.CompanyId == 3)
            {
                companyIds = new List<long?> { 1, 2 };
            }

            var methodCodeRecipes = new List<NAVRecipesModel>();

            List<NavmethodCodeBatch> methodCodeRecipe = new List<NavmethodCodeBatch>();
            List<ApplicationMasterDetail> applicationDetails = new List<ApplicationMasterDetail>();
            List<NAVRecipesModel> recipeList = new List<NAVRecipesModel>();
            using (var connection = CreateConnection())
            {
                var query = "SELECT * FROM NavmethodCodeBatch;";
                query += "select t1.* from ApplicationMasterDetail t1 JOIN ApplicationMaster t2 ON t1.ApplicationMasterID=t2.ApplicationMasterID where t2.ApplicationMasterCodeID=175;";
                query += "select RecipeNo,ItemNo,Description,BatchSize,ItemRecipeId,CONCAT(RecipeNo,'|',BatchSize) as RecipeName from Navrecipes Where CompanyId  in(" + string.Join(',', companyIds) + ") AND Status='Certified';";
                var results = await connection.QueryMultipleAsync(query);
                methodCodeRecipe = results.Read<NavmethodCodeBatch>().ToList();
                applicationDetails = results.Read<ApplicationMasterDetail>().ToList();
                recipeList = results.Read<NAVRecipesModel>().ToList();
            }

            methodCodeRecipe.ForEach(f =>
            {
                var BatchSize = applicationDetails.FirstOrDefault(b => b.ApplicationMasterDetailId == f.BatchSize)?.Value;
                var DefaultBatchSize = applicationDetails.FirstOrDefault(b => b.ApplicationMasterDetailId == f.DefaultBatchSize)?.Value;
                methodCodeRecipes.Add(new NAVRecipesModel
                {
                    RecipeNo = BatchSize,
                    BatchSize = BatchSize,// f.BatchUnitSize.GetValueOrDefault(0).ToString(),
                    ItemRecipeId = f.NavMethodCodeId,
                    UnitQTY = f.BatchUnitSize.GetValueOrDefault(0),
                    ItemNo = DefaultBatchSize,
                    DefaultBatch = DefaultBatchSize,
                    RecipeName = BatchSize,
                }); ;

            });


            var acreports = new List<INPCalendarPivotModel>();

            if (acreports.Count == 0)
            {
                acreports = await SimulationAddhocV55(dateRangeModel, methodCodeRecipes, recipeList);
            }
            if (dateRangeModel.MethodCodeId > 0)
                acreports = acreports.Where(r => r.MethodCodeId == dateRangeModel.MethodCodeId).ToList();
            if (dateRangeModel.SalesCategoryId > 0)
            {
                acreports = acreports.Where(r => r.SalesCategoryId == dateRangeModel.SalesCategoryId).ToList();
            }
            if (dateRangeModel.IsSteroid.HasValue)
            {
                acreports = acreports.Where(r => r.IsSteroid == dateRangeModel.IsSteroid.Value).ToList();
            }
            if (!string.IsNullOrEmpty(dateRangeModel.Replenishment))
            {
                acreports = acreports.Where(r => r.Replenishment.Contains(dateRangeModel.Replenishment)).ToList();
            }

            if (dateRangeModel.Ticketformula.GetValueOrDefault(0) > 0)
            {
                var ticketCondition = dateRangeModel.Ticketformula.GetValueOrDefault(0);
                var ticketValue = dateRangeModel.Ticketvalue.GetValueOrDefault(0);
                if (ticketCondition == 1)
                {
                    acreports = acreports.Where(r => r.Month1 == ticketValue || r.Month1 == ticketValue || r.Month2 == ticketValue || r.Month3 == ticketValue || r.Month4 == ticketValue || r.Month5 == ticketValue || r.Month6 == ticketValue).ToList();
                }
                else if (ticketCondition == 2)
                {
                    acreports = acreports.Where(r => r.Month1 > ticketValue || r.Month1 > ticketValue || r.Month2 > ticketValue || r.Month3 > ticketValue || r.Month4 > ticketValue || r.Month5 > ticketValue || r.Month6 > ticketValue).ToList();
                }
                else if (ticketCondition == 3)
                {
                    acreports = acreports.Where(r => r.Month1 >= ticketValue || r.Month1 >= ticketValue || r.Month2 >= ticketValue || r.Month3 >= ticketValue || r.Month4 >= ticketValue || r.Month5 >= ticketValue || r.Month6 >= ticketValue).ToList();
                }
                else if (ticketCondition == 4)
                {
                    acreports = acreports.Where(r => r.Month1 < ticketValue || r.Month1 < ticketValue || r.Month2 < ticketValue || r.Month3 < ticketValue || r.Month4 < ticketValue || r.Month5 < ticketValue || r.Month6 < ticketValue).ToList();
                }
                else if (ticketCondition == 5)
                {
                    acreports = acreports.Where(r => r.Month1 <= ticketValue || r.Month1 <= ticketValue || r.Month2 <= ticketValue || r.Month3 <= ticketValue || r.Month4 <= ticketValue || r.Month5 <= ticketValue || r.Month6 <= ticketValue).ToList();
                }
                else
                {
                    acreports = acreports.Where(r => r.Month1 != ticketValue || r.Month1 != ticketValue || r.Month2 != ticketValue || r.Month3 != ticketValue || r.Month4 != ticketValue || r.Month5 != ticketValue || r.Month6 != ticketValue).ToList();
                }
            }

            var packSize2 = 90000;
            if (!string.IsNullOrEmpty(dateRangeModel.Receipe))
            {
                string numberOnly = Regex.Replace(dateRangeModel.Receipe.Split("|")[0], "[^0-9.]", "");
                packSize2 = int.Parse(numberOnly) * 1000;

                acreports.Where(m => m.MethodCodeId == dateRangeModel.ChangeMethodCodeId).ToList().ForEach(f =>
                {
                    f.PackSize2 = packSize2;
                    f.ProdRecipe = dateRangeModel.Receipe.Split("|")[0];
                });

            }

            if (dateRangeModel.IsUpdate)
            {
                acreports.Where(m => m.ItemNo == dateRangeModel.ItemNo).ToList().ForEach(f =>
                {
                    f.Roundup2 = dateRangeModel.Roundup2;
                    f.Remarks = dateRangeModel.Remarks;
                });
            }
            if (acreports != null && acreports.Count() > 0)
            {
                acreports.ForEach(s =>
                {
                    s.IsNonSteroids = s.IsSteroid == false ? "Non Steroid" : "";
                    s.IsSteroids = s.IsSteroid == true ? "Steroid" : "Non Steroid";
                    s.ApexQty = s.ApexQty == 0 ? null : s.ApexQty;
                    s.AntahQty = s.AntahQty == 0 ? null : s.AntahQty;
                    s.MissQty = s.MissQty == 0 ? null : s.MissQty;
                    s.PxQty = s.PxQty == 0 ? null : s.PxQty;
                    s.DeliverynotReceived = s.DeliverynotReceived == 0 ? null : s.DeliverynotReceived;
                    s.SymlQty = s.SymlQty == 0 ? null : s.SymlQty;
                    s.Rework_ = s.Rework == 0 ? null : s.Rework;
                    s.AcSum_ = s.AcSum == 0 ? null : s.AcSum;
                    s.ThreeMonthACQty_ = s.ThreeMonthACQty == 0 ? null : s.ThreeMonthACQty;
                    s.Roundup1_ = s.Roundup1 == 0 ? null : s.Roundup1;
                    s.Roundup2_ = s.Roundup2 == 0 ? null : s.Roundup2;
                    s.PreApexStockBalance_ = s.PreApexStockBalance == 0 ? null : s.PreApexStockBalance;
                    s.PreAntahStockBalance_ = s.PreAntahStockBalance == 0 ? null : s.PreAntahStockBalance;
                    s.PreMsbStockBalance_ = s.PreMsbStockBalance == 0 ? null : s.PreMsbStockBalance;
                    s.PrePsbStockBalance_ = s.PrePsbStockBalance == 0 ? null : s.PrePsbStockBalance;
                    s.PreSgTenderStockBalance_ = s.PreSgTenderStockBalance == 0 ? null : s.PreSgTenderStockBalance;
                    s.WipQty_ = s.WipQty == 0 ? null : s.WipQty;
                    s.NotStartInvQty_ = s.NotStartInvQty == 0 ? null : s.NotStartInvQty;
                    s.PreMyStockBalance_ = s.PreMyStockBalance == 0 ? null : s.PreMyStockBalance;
                    s.PreOtherStoreQty_ = s.PreOtherStoreQty == 0 ? null : s.PreOtherStoreQty;
                    s.PrewipQty_ = s.PrewipQty == 0 ? null : s.PrewipQty;
                    s.PreStockBalance_ = s.PreStockBalance == 0 ? null : s.PreStockBalance;
                    s.PreStockHoldingBalance_ = s.PreStockHoldingBalance == 0 ? null : s.PreStockHoldingBalance;
                    s.ApexStockBalance_ = s.ApexStockBalance == 0 ? null : s.ApexStockBalance;
                    s.AntahStockBalance_ = s.AntahStockBalance == 0 ? null : s.AntahStockBalance;
                    s.MsbStockBalance_ = s.MsbStockBalance == 0 ? null : s.MsbStockBalance;
                    s.PsbStockBalance_ = s.PsbStockBalance == 0 ? null : s.PsbStockBalance;
                    s.SgTenderStockBalance_ = s.SgTenderStockBalance == 0 ? null : s.SgTenderStockBalance;
                    s.MyStockBalance_ = s.MyStockBalance == 0 ? null : s.MyStockBalance;
                    s.OtherStoreQty_ = s.OtherStoreQty == 0 ? null : s.OtherStoreQty;
                    s.InterCompanyTransitQty_ = s.InterCompanyTransitQty == 0 ? null : s.InterCompanyTransitQty;
                    s.StockBalance_ = s.StockBalance == 0 ? null : s.StockBalance;
                    s.StockHoldingBalance_ = s.StockHoldingBalance == 0 ? null : s.StockHoldingBalance;
                    s.BlanketAddhoc1_ = s.BlanketAddhoc1 == 0 ? null : s.BlanketAddhoc1;
                    s.Month1_ = s.Month1 == 0 ? null : s.Month1;
                    s.ProjectedHoldingStock1_ = s.ProjectedHoldingStock1 == 0 ? null : s.ProjectedHoldingStock1;
                    //s.ProductionProjected1_ = s.ProductionProjected1 == 0 ? null : s.ProductionProjected1;
                    s.ProductionProjected1_ = (s.Month1 + s.ProjectedHoldingStock1) - s.BlanketAddhoc1;
                    s.BlanketAddhoc2_ = s.BlanketAddhoc2 == 0 ? null : s.BlanketAddhoc2;
                    s.Month2_ = s.Month2 == 0 ? null : s.Month2;
                    s.ProjectedHoldingStock2_ = s.ProjectedHoldingStock2 == 0 ? null : s.ProjectedHoldingStock2;
                    s.ProductionProjected2_ = (s.Month2 + s.ProjectedHoldingStock2) - s.BlanketAddhoc2;
                    //s.ProductionProjected2_ = s.ProductionProjected2 == 0 ? null : s.ProductionProjected2;
                    s.BlanketAddhoc3_ = s.BlanketAddhoc3 == 0 ? null : s.BlanketAddhoc3;
                    s.Month3_ = s.Month3 == 0 ? null : s.Month3;
                    s.ProjectedHoldingStock3_ = s.ProjectedHoldingStock3 == 0 ? null : s.ProjectedHoldingStock3;
                    s.ProductionProjected3_ = (s.Month3 + s.ProjectedHoldingStock3) - s.BlanketAddhoc3;
                    //s.ProductionProjected3_ = s.ProductionProjected3 == 0 ? null : s.ProductionProjected3;
                    s.BlanketAddhoc4_ = s.BlanketAddhoc4 == 0 ? null : s.BlanketAddhoc4;
                    s.Month4_ = s.Month4 == 0 ? null : s.Month4;
                    s.ProjectedHoldingStock4_ = s.ProjectedHoldingStock4 == 0 ? null : s.ProjectedHoldingStock4;
                    s.ProductionProjected4_ = (s.Month4 + s.ProjectedHoldingStock4) - s.BlanketAddhoc4;
                    //s.ProductionProjected4_ = s.ProductionProjected4 == 0 ? null : s.ProductionProjected4;
                    s.BlanketAddhoc5_ = s.BlanketAddhoc5 == 0 ? null : s.BlanketAddhoc5;
                    s.Month5_ = s.Month5 == 0 ? null : s.Month5;
                    s.ProjectedHoldingStock5_ = s.ProjectedHoldingStock5 == 0 ? null : s.ProjectedHoldingStock5;
                    s.ProductionProjected5_ = (s.Month5 + s.ProjectedHoldingStock5) - s.BlanketAddhoc5;
                    //s.ProductionProjected5_ = s.ProductionProjected5 == 0 ? null : s.ProductionProjected5;
                    s.BlanketAddhoc6_ = s.BlanketAddhoc6 == 0 ? null : s.BlanketAddhoc6;
                    s.Month6_ = s.Month6 == 0 ? null : s.Month6;
                    s.ProjectedHoldingStock6_ = s.ProjectedHoldingStock6 == 0 ? null : s.ProjectedHoldingStock6;
                    s.ProductionProjected6_ = (s.Month6 + s.ProjectedHoldingStock6) - s.BlanketAddhoc6;
                    //s.ProductionProjected6_ = s.ProductionProjected6 == 0 ? null : s.ProductionProjected6;
                    s.BlanketAddhoc7_ = s.BlanketAddhoc7 == 0 ? null : s.BlanketAddhoc7;
                    s.Month7_ = s.Month7 == 0 ? null : s.Month7;
                    s.ProjectedHoldingStock7_ = s.ProjectedHoldingStock7 == 0 ? null : s.ProjectedHoldingStock7;
                    //s.ProductionProjected7_ = s.ProductionProjected7 == 0 ? null : s.ProductionProjected7;
                    s.ProductionProjected7_ = (s.Month7 + s.ProjectedHoldingStock7) - s.BlanketAddhoc7;
                    s.BlanketAddhoc8_ = s.BlanketAddhoc8 == 0 ? null : s.BlanketAddhoc8;
                    s.Month8_ = s.Month8 == 0 ? null : s.Month8;
                    s.ProjectedHoldingStock8_ = s.ProjectedHoldingStock8 == 0 ? null : s.ProjectedHoldingStock8;
                    //s.ProductionProjected8_ = s.ProductionProjected8 == 0 ? null : s.ProductionProjected8;
                    s.ProductionProjected8_ = (s.Month8 + s.ProjectedHoldingStock8) - s.BlanketAddhoc8;
                    s.BlanketAddhoc9_ = s.BlanketAddhoc9 == 0 ? null : s.BlanketAddhoc9;
                    s.Month9_ = s.Month9 == 0 ? null : s.Month9;
                    s.ProjectedHoldingStock9_ = s.ProjectedHoldingStock9 == 0 ? null : s.ProjectedHoldingStock9;
                    //s.ProductionProjected9_ = s.ProductionProjected9 == 0 ? null : s.ProductionProjected9;
                    s.ProductionProjected9_ = (s.Month9 + s.ProjectedHoldingStock9) - s.BlanketAddhoc9;
                    s.BlanketAddhoc10_ = s.BlanketAddhoc10 == 0 ? null : s.BlanketAddhoc10;
                    s.Month10_ = s.Month10 == 0 ? null : s.Month10;
                    s.ProjectedHoldingStock10_ = s.ProjectedHoldingStock10 == 0 ? null : s.ProjectedHoldingStock10;
                    //s.ProductionProjected10_ = s.ProductionProjected10 == 0 ? null : s.ProductionProjected10;
                    s.ProductionProjected10_ = (s.Month10 + s.ProjectedHoldingStock10) - s.BlanketAddhoc10;
                    s.BlanketAddhoc11_ = s.BlanketAddhoc11 == 0 ? null : s.BlanketAddhoc11;
                    s.Month11_ = s.Month11 == 0 ? null : s.Month11;
                    s.ProjectedHoldingStock11_ = s.ProjectedHoldingStock11 == 0 ? null : s.ProjectedHoldingStock11;
                    //s.ProductionProjected11_ = s.ProductionProjected11 == 0 ? null : s.ProductionProjected11;
                    s.ProductionProjected11_ = (s.Month11 + s.ProjectedHoldingStock11) - s.BlanketAddhoc11;
                    s.BlanketAddhoc12_ = s.BlanketAddhoc12 == 0 ? null : s.BlanketAddhoc12;
                    s.Month12_ = s.Month12 == 0 ? null : s.Month12;
                    s.ProjectedHoldingStock12_ = s.ProjectedHoldingStock12 == 0 ? null : s.ProjectedHoldingStock12;
                    //s.ProductionProjected12_ = s.ProductionProjected12 == 0 ? null : s.ProductionProjected12;
                    s.ProductionProjected12_ = (s.Month12 + s.ProjectedHoldingStock12) - s.BlanketAddhoc12;
                    s.NoOfTicket1_ = s.NoOfTicket1;
                    s.NoOfTicket2_ = s.NoOfTicket2;
                    s.NoOfTicket3_ = s.NoOfTicket3;
                    s.NoOfTicket4_ = s.NoOfTicket4;
                    s.NoOfTicket5_ = s.NoOfTicket5;
                    s.NoOfTicket6_ = s.NoOfTicket6; s.NoOfTicket7_ = s.NoOfTicket7; s.NoOfTicket8_ = s.NoOfTicket8;
                    s.NoOfTicket9_ = s.NoOfTicket9; s.NoOfTicket10_ = s.NoOfTicket10; s.NoOfTicket11_ = s.NoOfTicket11; s.NoOfTicket12_ = s.NoOfTicket12;
                    var GroupTicket1 = s.GroupItemTicket1 + "," + s.GroupTicket1;
                    if (GroupTicket1 != null)
                    {
                        var tic1 = GroupTicket1.Split(",").ToList().Where(w => w != "," && w != "").ToList();
                        s.NoOfTicketCount1_ = tic1.Count() > 0 ? tic1.Count() : null; ;
                        var tick1_ = tic1.Distinct().ToList(); s.Ticket1 = "";
                        if (tick1_ != null && tick1_.Count > 0)
                        {
                            tick1_.ForEach(a =>
                            {
                                if (!string.IsNullOrEmpty(a))
                                {
                                    s.Ticket1 += tic1.Where(q => q == a).Count() + "*" + a.Split(".")[0] + "/";
                                    if (!string.IsNullOrEmpty(s.NoOfTicket1))
                                    {
                                        s.Ticket1 += s.NoOfTicket1 + "/";
                                    }
                                }
                            });
                        }
                        s.Ticket1 += s.ProdOrderNo1;
                    }
                    var GroupTicket2 = s.GroupItemTicket2 + "," + s.GroupTicket2;
                    if (GroupTicket2 != null)
                    {
                        var tic2 = GroupTicket2.Split(",").ToList().Where(w => w != "," && w != "").ToList();
                        s.NoOfTicketCount2_ = tic2.Count() > 0 ? tic2.Count() : null; ;
                        var tick2_ = tic2.Distinct().ToList(); s.Ticket2 = "";
                        if (tick2_ != null && tick2_.Count > 0)
                        {
                            tick2_.ForEach(a =>
                            {
                                if (!string.IsNullOrEmpty(a))
                                {
                                    s.Ticket2 += tic2.Where(q => q == a).Count() + "*" + a.Split(".")[0] + "/";
                                    if (!string.IsNullOrEmpty(s.NoOfTicket2))
                                    {
                                        s.Ticket2 += s.NoOfTicket2 + "/";
                                    }
                                }
                            });
                        }
                        s.Ticket2 += s.ProdOrderNo2;
                    }
                    var GroupTicket3 = s.GroupItemTicket3 + "," + s.GroupTicket3;
                    if (GroupTicket3 != null)
                    {
                        var tic3 = GroupTicket3.Split(",").ToList().Where(w => w != "," && w != "").ToList();
                        s.NoOfTicketCount3_ = tic3.Count() > 0 ? tic3.Count() : null;
                        var tick3_ = tic3.Distinct().ToList(); s.Ticket3 = "";
                        if (tick3_ != null && tick3_.Count > 0)
                        {
                            tick3_.ForEach(a =>
                            {
                                if (!string.IsNullOrEmpty(a))
                                {
                                    s.Ticket3 += tic3.Where(q => q == a).Count() + "*" + a.Split(".")[0] + "/";
                                    if (!string.IsNullOrEmpty(s.NoOfTicket3))
                                    {
                                        s.Ticket3 += s.NoOfTicket3 + "/";
                                    }
                                }
                            });
                        }
                        s.Ticket3 += s.ProdOrderNo3;
                    }
                    var GroupTicket4 = s.GroupItemTicket4 + "," + s.GroupTicket4;
                    if (GroupTicket4 != null)
                    {
                        var tic4 = GroupTicket4.Split(",").ToList().Where(w => w != "," && w != "").ToList();
                        s.NoOfTicketCount4_ = tic4.Count() > 0 ? tic4.Count() : null;
                        var tick4_ = tic4.Distinct().ToList(); s.Ticket4 = "";
                        if (tick4_ != null && tick4_.Count > 0)
                        {
                            tick4_.ForEach(a =>
                            {
                                if (!string.IsNullOrEmpty(a))
                                {
                                    s.Ticket4 += tic4.Where(q => q == a).Count() + "*" + a.Split(".")[0] + "/";
                                    if (!string.IsNullOrEmpty(s.NoOfTicket4))
                                    {
                                        s.Ticket4 += s.NoOfTicket4 + "/";
                                    }
                                }
                            });
                        }
                        s.Ticket4 += s.ProdOrderNo4;
                    }
                    var GroupTicket5 = s.GroupItemTicket5 + "," + s.GroupTicket5;
                    if (GroupTicket5 != null)
                    {
                        var tic5 = GroupTicket5.Split(",").ToList().Where(w => w != "," && w != "").ToList();
                        s.NoOfTicketCount5_ = tic5.Count() > 0 ? tic5.Count() : null;
                        var tick5_ = tic5.Distinct().ToList(); s.Ticket5 = "";
                        if (tick5_ != null && tick5_.Count > 0)
                        {
                            tick5_.ForEach(a =>
                            {
                                if (!string.IsNullOrEmpty(a))
                                {
                                    s.Ticket5 += tic5.Where(q => q == a).Count() + "*" + a.Split(".")[0] + "/";
                                    if (!string.IsNullOrEmpty(s.NoOfTicket5))
                                    {
                                        s.Ticket5 += s.NoOfTicket5 + "/";
                                    }
                                }
                            });
                        }
                        s.Ticket5 += s.ProdOrderNo5;
                    }
                    var GroupTicket6 = s.GroupItemTicket6 + "," + s.GroupTicket6;
                    if (GroupTicket6 != null)
                    {
                        var tic6 = GroupTicket6.Split(",").ToList().Where(w => w != "," && w != "").ToList();
                        s.NoOfTicketCount6_ = tic6.Count() > 0 ? tic6.Count() : null;
                        var tick6_ = tic6.Distinct().ToList(); s.Ticket6 = "";
                        if (tick6_ != null && tick6_.Count > 0)
                        {
                            tick6_.ForEach(a =>
                            {
                                if (!string.IsNullOrEmpty(a))
                                {
                                    s.Ticket6 += tic6.Where(q => q == a).Count() + "*" + a.Split(".")[0] + "/";
                                    if (!string.IsNullOrEmpty(s.NoOfTicket6))
                                    {
                                        s.Ticket6 += s.NoOfTicket6 + "/";
                                    }
                                }
                            });
                        }
                        s.Ticket6 += s.ProdOrderNo6;
                    }
                    var GroupTicket7 = s.GroupItemTicket7 + "," + s.GroupTicket7;
                    if (GroupTicket7 != null)
                    {
                        var tic7 = GroupTicket7.Split(",").ToList().Where(w => w != "," && w != "").ToList();
                        s.NoOfTicketCount7_ = tic7.Count() > 0 ? tic7.Count() : null;
                        var tick7_ = tic7.Distinct().ToList(); s.Ticket7 = "";
                        if (tick7_ != null && tick7_.Count > 0)
                        {
                            tick7_.ForEach(a =>
                            {
                                if (!string.IsNullOrEmpty(a))
                                {
                                    s.Ticket7 += tic7.Where(q => q == a).Count() + "*" + a.Split(".")[0] + "/";
                                    if (!string.IsNullOrEmpty(s.NoOfTicket7))
                                    {
                                        s.Ticket7 += s.NoOfTicket7 + "/";
                                    }
                                }
                            });
                        }
                        s.Ticket7 += s.ProdOrderNo7;
                    }
                    var GroupTicket8 = s.GroupItemTicket8 + "," + s.GroupTicket8;
                    if (GroupTicket8 != null)
                    {
                        var tic8 = GroupTicket8.Split(",").ToList().Where(w => w != "," && w != "").ToList();
                        s.NoOfTicketCount8_ = tic8.Count() > 0 ? tic8.Count() : null;
                        var tick8_ = tic8.Distinct().ToList(); s.Ticket8 = "";
                        if (tick8_ != null && tick8_.Count > 0)
                        {
                            tick8_.ForEach(a =>
                            {
                                if (!string.IsNullOrEmpty(a))
                                {
                                    s.Ticket8 += tic8.Where(q => q == a).Count() + "*" + a.Split(".")[0] + "/";
                                    if (!string.IsNullOrEmpty(s.NoOfTicket8))
                                    {
                                        s.Ticket8 += s.NoOfTicket8 + "/";
                                    }
                                }
                            });
                        }
                        s.Ticket8 += s.ProdOrderNo8;
                    }
                    var GroupTicket9 = s.GroupItemTicket9 + "," + s.GroupTicket9;
                    if (GroupTicket9 != null)
                    {
                        var tic9 = GroupTicket9.Split(",").ToList().Where(w => w != "," && w != "").ToList();
                        s.NoOfTicketCount9_ = tic9.Count() > 0 ? tic9.Count() : null;
                        var tick9_ = tic9.Distinct().ToList(); s.Ticket9 = "";
                        if (tick9_ != null && tick9_.Count > 0)
                        {
                            tick9_.ForEach(a =>
                            {
                                if (!string.IsNullOrEmpty(a))
                                {
                                    s.Ticket9 += tic9.Where(q => q == a).Count() + "*" + a.Split(".")[0] + "/";
                                    if (!string.IsNullOrEmpty(s.NoOfTicket9))
                                    {
                                        s.Ticket9 += s.NoOfTicket9 + "/";
                                    }
                                }
                            });
                        }
                        s.Ticket9 += s.ProdOrderNo9;
                    }
                    var GroupTicket10 = s.GroupItemTicket10 + "," + s.GroupTicket10;
                    if (GroupTicket10 != null)
                    {
                        var tic10 = GroupTicket10.Split(",").ToList().Where(w => w != "," && w != "").ToList();
                        s.NoOfTicketCount10_ = tic10.Count() > 0 ? tic10.Count() : null;
                        var tick10_ = tic10.Distinct().ToList(); s.Ticket10 = "";
                        if (tick10_ != null && tick10_.Count > 0)
                        {
                            tick10_.ForEach(a =>
                            {
                                if (!string.IsNullOrEmpty(a))
                                {
                                    s.Ticket10 += tic10.Where(q => q == a).Count() + "*" + a.Split(".")[0] + "/";
                                    if (!string.IsNullOrEmpty(s.NoOfTicket10))
                                    {
                                        s.Ticket10 += s.NoOfTicket10 + "/";
                                    }
                                }
                            });
                        }
                        s.Ticket10 += s.ProdOrderNo10;
                    }
                    var GroupTicket11 = s.GroupItemTicket11 + "," + s.GroupTicket11;
                    if (GroupTicket11 != null)
                    {
                        var tic11 = GroupTicket11.Split(",").ToList().Where(w => w != "," && w != "").ToList();
                        s.NoOfTicketCount11_ = tic11.Count() > 0 ? tic11.Count() : null;
                        var tick11_ = tic11.Distinct().ToList(); s.Ticket11 = "";
                        if (tick11_ != null && tick11_.Count > 0)
                        {
                            tick11_.ForEach(a =>
                            {
                                if (!string.IsNullOrEmpty(a))
                                {
                                    s.Ticket11 += tic11.Where(q => q == a).Count() + "*" + a.Split(".")[0] + "/";
                                    if (!string.IsNullOrEmpty(s.NoOfTicket11))
                                    {
                                        s.Ticket11 += s.NoOfTicket11 + "/";
                                    }
                                }
                            });
                        }
                        s.Ticket11 += s.ProdOrderNo11;
                    }
                    var GroupTicket12 = s.GroupItemTicket12 + "," + s.GroupTicket12;
                    if (GroupTicket12 != null)
                    {
                        var tic12 = GroupTicket12.Split(",").ToList().Where(w => w != "," && w != "").ToList();
                        s.NoOfTicketCount12_ = tic12.Count() > 0 ? tic12.Count() : null;
                        var tick12_ = tic12.Distinct().ToList(); s.Ticket12 = "";
                        if (tick12_ != null && tick12_.Count > 0)
                        {
                            tick12_.ForEach(a =>
                            {
                                if (!string.IsNullOrEmpty(a))
                                {
                                    s.Ticket12 += tic12.Where(q => q == a).Count() + "*" + a.Split(".")[0] + "/";
                                    if (!string.IsNullOrEmpty(s.NoOfTicket12))
                                    {
                                        s.Ticket12 += s.NoOfTicket12 + "/";
                                    }
                                }
                            });
                        }
                        s.Ticket12 += s.ProdOrderNo12;
                    }
                });
            }
            return acreports.OrderBy(o => o.ItemNo).ToList();

        }

        private async Task<List<INPCalendarPivotModel>> SimulationAddhocV55(DateRangeModel endDate, List<NAVRecipesModel> recipeList, List<NAVRecipesModel> _orderRecipeList)
        {
            var categoryList = new List<string>
            {
                "CAP",
                "CREAM",
                "DD",
                "SYRUP",
                "TABLET",
                "VET",
                "POWDER",
                "INJ"
            };
            var salesCatLists = salesCatList();
            var itemdict = new Dictionary<string, decimal>();
            var MethodCodeList = new List<INPCalendarPivotModel>();
            var companyIds = new List<long?> { endDate.CompanyId };
            if (endDate.CompanyId == 3)
            {
                companyIds = new List<long?> { 1, 2 };
            }
            var intercompanyIds = new List<long?> { 1, 2 };
            var month = endDate.StockMonth.Month;//== 1 ? 12 : endDate.StockMonth.Month - 1;
            var year = endDate.StockMonth.Year;// == 1 ? endDate.StockMonth.Year - 1 : endDate.StockMonth.Year;
            var weekofmonth = GetWeekNumberOfMonth(endDate.StockMonth);
            var intransitMonth = month == 1 ? 12 : month - 1;
            DateTime lastDay = new DateTime(endDate.StockMonth.Year, endDate.StockMonth.Month, 1).AddMonths(1).AddDays(-1);

            var doNotReceivedList = new List<NavpostedShipment>();
            var navMethodCodeLines = new List<NavMethodCodeLines>(); var acitems = new List<NavItemCitemList>();
            var acItemBalListResult = new List<DistStockBalModel>(); var acEntries = new List<ACItemsModel>(); var dismapeditems = new List<NavItemCitemList>();
            var categoryItems = new List<NavSaleCategory>(); var itemRelations = new List<NavitemLinks>(); var prodyctionTickets = new List<ProductionSimulation>();
            var prenavStkbalance = new List<NavitemStockBalance>(); var navStkbalance = new List<NavitemStockBalance>(); var prodyctionoutTickets = new List<ProductionSimulation>();
            var blanletOrders = new List<SimulationAddhoc>(); var pre_acItemBalListResult = new List<DistStockBalModel>(); var grouptickets = new List<GroupPlaningTicket>();
            var intercompanyItems = new List<Navitems>(); var itemMasterforReport = new List<Navitems>(); var orderRequirements = new List<OrderRequirementLineModel>(); var acEntriesList = new List<Acitems>();
            DateTime firstDayOfMonth = new DateTime(endDate.StockMonth.Year, endDate.StockMonth.Month, 1);
            var dateMonth1 = firstDayOfMonth;// endDate.StockMonth;
            var datemonth12 = endDate.StockMonth.AddMonths(12);
            using (var connection = CreateConnection())
            {

                var query = "select ShipmentId,\r\nCompany,\r\nCompanyId,\r\nStockBalanceMonth,\r\nPostingDate,\r\nCustomer,\r\nCustomerNo,\r\nCustomerId,\r\nDeliveryOrderNo,\r\nDOLineNo,\r\nItemNo,\r\nDescription,\r\nIsRecived,\r\nDoQty,\r\nStatusCodeID,\r\nAddedByUserID,\r\nAddedDate,\r\nModifiedByUserID from NavpostedShipment WHERE CompanyId  in(" + string.Join(',', intercompanyIds) + ") AND CAST(StockBalanceMonth AS Date) <='" + lastDay + "'  AND (IsRecived is null Or IsRecived=0)\r\n;";
                query += "select t2.DistAcid,(case when t1.Quantity is NULL then  0 ELSE t1.Quantity END) as QtyOnHand,t2.ItemNo,t2.ItemDesc,t2.DistName,t1.DistItemId,(case when t2.CustomerId is NULL then  -1 ELSE t2.CustomerId END) as CustomerId,t2.CompanyId from " +
                    "DistStockBalance t1 INNER JOIN Acitems t2  ON t1.DistItemId=t2.DistACID\r\n" +
                    "Where  (t1.StockBalWeek=" + weekofmonth + " OR t1.StockBalWeek is null ) AND MONTH(t1.StockBalMonth) = " + month + " AND YEAR(t1.StockBalMonth)=" + year + ";\r\n";
                query += "select t2.DistAcid,(case when t1.Quantity is NULL then  0 ELSE t1.Quantity END) as QtyOnHand,t2.ItemNo,t2.ItemDesc,t2.DistName,t1.DistItemId,(case when t2.CustomerId is NULL then  -1 ELSE t2.CustomerId END) as CustomerId,t2.CompanyId from " +
                    "DistStockBalance t1 INNER JOIN Acitems t2  ON t1.DistItemId=t2.DistACID\r\n" +
                    "Where   t1.StockBalWeek=1 AND MONTH(t1.StockBalMonth) = " + month + " AND YEAR(t1.StockBalMonth)=" + year + "\r\n;";
                query += "select t1.CustomerId as DistId,t1.ToDate as ACMonth,t3.No as ItemNo,t2.Quantity as ACQty,t2.ItemId as SWItemId,t3.Description as ItemDesc,t1.CustomerId from Acentry t1 INNER JOIN AcentryLines t2 ON t1.ACEntryId=t2.ACEntryId INNER JOIN NAVItems t3 ON t2.ItemId=t3.ItemId\r\n" +
                    "WHERE t1.CompanyId  in(" + string.Join(',', companyIds) + ") AND CAST(t1.ToDate AS Date)>='" + endDate.StockMonth + "'  AND CAST(t1.FromDate AS Date)<='" + endDate.StockMonth + "';\r\n";
                query += "select NavItemCItemId,\r\nNavItemId,\r\nNavItemCustomerItemId from NavItemCitemList;\r\n";
                query += "select SaleCategoryID,\r\nCode,\r\nDescription,\r\nNoSeries,\r\nLocationID,\r\nSGNoSeries,\r\nStatusCodeID,\r\nAddedByUserID,\r\nAddedDate,\r\nModifiedByUserID,\r\nModifiedDate from NavSaleCategory;\r\n";
                query += "select ItemLinkId,\r\nMyItemId,\r\nSgItemId,\r\nStatusCodeID,\r\nAddedByUserID,\r\nAddedDate,\r\nModifiedByUserID,\r\nModifiedDate from NavitemLinks where SgItemId is not null AND MyItemId is not null;\r\n";
                query += "select ProductionSimulationID,\r\nCompanyId,\r\nProdOrderNo,\r\nItemID,\r\nItemNo,\r\nDescription,\r\nPackSize,\r\nQuantity,\r\nUOM,\r\nPerQuantity,\r\nPerQtyUOM,\r\nBatchNo,\r\nPlannedQty,\r\nOutputQty,\r\nIsOutput,\r\nStartingDate,\r\nStatusCodeID,\r\nAddedByUserID,\r\nAddedDate,\r\nModifiedByUserID,\r\nModifiedDate,\r\nProcessDate,\r\nIsBMRTicket,\r\nRePlanRefNo,\r\nBatchSize,\r\nDispense from ProductionSimulation where IsBmrticket=0 AND CAST(StartingDate AS Date)>='" + dateMonth1 + "'  AND CAST(StartingDate AS Date)<='" + datemonth12 + "' order by StartingDate desc;\r\n";

                query += "select ProductionSimulationID,\r\nCompanyId,\r\nProdOrderNo,\r\nItemID,\r\nItemNo,\r\nDescription,\r\nPackSize,\r\nQuantity,\r\nUOM,\r\nPerQuantity,\r\nPerQtyUOM,\r\nBatchNo,\r\nPlannedQty,\r\nOutputQty,\r\nIsOutput,\r\nStartingDate,\r\nStatusCodeID,\r\nAddedByUserID,\r\nAddedDate,\r\nModifiedByUserID,\r\nModifiedDate,\r\nProcessDate,\r\nIsBMRTicket,\r\nRePlanRefNo,\r\nBatchSize,\r\nDispense from ProductionSimulation where IsBmrticket=0 AND CAST(ProcessDate AS Date)>='" + dateMonth1 + "'  AND CAST(ProcessDate AS Date)<='" + datemonth12 + "' order by ProcessDate desc;\r\n";

                query += "SELECT t1.NavStockBalanceId,\r\nt1.ItemId,\r\nt1.StockBalMonth,\r\nt1.StockBalWeek,\r\nt1.Quantity,\r\nt1.RejectQuantity,\r\nt1.StatusCodeID,\r\nt1.AddedByUserID,\r\nt1.AddedDate,\r\nt1.ModifiedByUserID,\r\nt1.ModifiedDate,\r\nt1.ReworkQty,\r\nt1.WIPQty,\r\nt1.GlobalQty,\r\nt1.KIVQty,\r\nt1.SupplyWIPQty,\r\nt1.Supply1ProcessQty,\r\nt1.NotStartInvQty,t2.PackQty from NavitemStockBalance t1 INNER JOIN navitems t2 ON t1.ItemId=t2.ItemId Where t1.StockBalWeek=1 AND t2.ItemId is Not NUll and MONTH(t1.StockBalMonth) =" + month + " AND YEAR(t1.StockBalMonth)=" + year + ";\r\n";

                query += "SELECT t1.NavStockBalanceId,\r\nt1.ItemId,\r\nt1.StockBalMonth,\r\nt1.StockBalWeek,\r\nt1.Quantity,\r\nt1.RejectQuantity,\r\nt1.StatusCodeID,\r\nt1.AddedByUserID,\r\nt1.AddedDate,\r\nt1.ModifiedByUserID,\r\nt1.ModifiedDate,\r\nt1.ReworkQty,\r\nt1.WIPQty,\r\nt1.GlobalQty,\r\nt1.KIVQty,\r\nt1.SupplyWIPQty,\r\nt1.Supply1ProcessQty,\r\nt1.NotStartInvQty,t2.PackQty from NavitemStockBalance t1 INNER JOIN navitems t2 ON t1.ItemId=t2.ItemId Where (t1.StockBalWeek is null OR t1.StockBalWeek=" + weekofmonth + ") AND t2.ItemId is Not NUll and MONTH(t1.StockBalMonth) =" + month + " AND YEAR(t1.StockBalMonth)=" + year + ";\r\n";
                query += "select t1.SimualtionAddhocID,\r\nt1.DocumantType,\r\nt1.SelltoCustomerNo,\r\nt1.CustomerName,\r\nt1.Categories,\r\nt1.DocumentNo,\r\nt1.ExternalDocNo,\r\nt1.ItemID,\r\nt1.ItemNo,\r\nt1.Description,\r\nt1.Description1,\r\nt1.OutstandingQty,\r\nt1.PromisedDate,\r\nt1.ShipmentDate,\r\nt1.UOMCode from SimulationAddhoc t1 where  t1.CompanyId in(" + string.Join(',', companyIds) + ") AND CAST(t1.ShipmentDate AS Date)>='" + dateMonth1 + "'  AND CAST(t1.ShipmentDate AS Date)<='" + datemonth12 + "';\r\n";

                query += "select GroupPlanningId,\r\nCompanyId,\r\nBatchName,\r\nProductGroupCode,\r\nStartDate,\r\nItemNo,\r\nDescription,\r\nDescription1,\r\nBatchSize,\r\nQuantity,\r\nUOM,\r\nNoOfTicket,\r\nTotalQuantity from GroupPlaningTicket where CompanyId=" + endDate.CompanyId + " AND CAST(StartDate AS Date)>='" + dateMonth1 + "'  AND CAST(StartDate AS Date)<='" + datemonth12 + "' order by StartDate desc;\r\n";
                query += "select s.No,s.ItemId,s.Description,s.PackQty,s.CompanyId from NAVItems s;";
                query += "select t1.*,t2.Description2 as GenericCodeDescription2 from NAVItems t1\r\nLEFT JOIN GenericCodes t2 ON t1.GenericCodeId=t2.GenericCodeId\r\n" +
                    "where t1.CompanyId  in(" + string.Join(',', companyIds) + ") AND  t1.StatusCodeId=1 AND t1.ItemId IN(select NavItemId from NavItemCitemList WHere NavItemId>0);\r\n";


                query += "select t1.ProductId,t1.ProductQty,t1.NoOfTicket,t1.ExpectedStartDate,t1.RequireToSplit,t2.SplitProductID,t2.SplitProductQty from OrderRequirementLine t1\r\nINNER JOIN OrderRequirementLineSplit t2 ON t1.OrderRequirementLineID=t2.OrderRequirementLineID\r\nwhere t1.IsNavSync=1 AND CAST(t1.ExpectedStartDate AS Date)>='" + dateMonth1 + "'  AND CAST(t1.ExpectedStartDate AS Date)<='" + datemonth12 + "' order by t1.ExpectedStartDate desc;\r\n";
                query += "select t1.*,t6.DistACID,\r\nt6.CompanyId,\r\nt6.CustomerId,\r\nt6.DistName,\r\nt6.ItemGroup,\r\nt6.Steriod,\r\nt6.ShelfLife,\r\nt6.Quota,\r\nt6.Status,\r\nt6.ItemDesc,\r\nt6.PackSize,\r\nt6.ACQty,\r\nt6.ACMonth,\r\nt6.StatusCodeID,\r\nt6.AddedByUserID,\r\nt6.AddedDate,\r\nt6.ModifiedByUserID,\r\nt6.ModifiedDate,\r\nt6.ItemNo from NavItemCitemList t1\r\nINNER JOIN Acitems t6 ON t1.NavItemCustomerItemId=t6.DistACID;\r\n";
                query += "select t1.MethodCodeLineId,t1.MethodCodeId,t1.ItemID,t1.StatusCodeID,t1.AddedByUserID,t1.AddedDate,t1.ModifiedByUserID,t1.MethodCodeLineID,\r\nt2.MethodName,t2.MethodDescription,t2.NAVINPCategoryID,t2.CompanyId,t2.ProdFrequency,t2.DistReplenishHS,t2.DistACMonth,t2.AdhocMonthStandAlone,t2.AdhocPlanQty from NavMethodCodeLines t1 INNER JOIN NavMethodCode t2 ON t2.MethodCodeId=t1.MethodCodeId \r\nWHERE t1.ItemID IN(select t3.ItemId from NAVItems t3 LEFT JOIN GenericCodes t4 ON t3.GenericCodeId=t4.GenericCodeId where t3.CompanyId  in(" + string.Join(',', companyIds) + ") AND  t3.StatusCodeId=1 AND t3.ItemId IN(select NavItemId from NavItemCitemList WHere NavItemId>0));\r\n";
                query += "select t1.NavItemCitemId,t1.NavItemId,t1.NavItemCustomerItemId,t2.DistACID,t2.CompanyId,t2.DistName,t2.ItemGroup,t2.Steriod,t2.ShelfLife,t2.Quota,t2.ItemDesc,t2.PackSize,t2.PackSize,t2.ACQty,t2.ItemNo from NavItemCitemList t1 INNER JOIN Acitems t2 ON t1.NavItemCustomerItemId=t2.DistACID\r\nWHERE t1.NavItemId IN(select t3.ItemId from NAVItems t3 LEFT JOIN GenericCodes t4 ON t3.GenericCodeId=t4.GenericCodeId where t3.CompanyId  in(" + string.Join(',', companyIds) + ") AND  t3.StatusCodeId=1 AND t3.ItemId IN(select NavItemId from NavItemCitemList WHere NavItemId>0));\r\n";
                var results = await connection.QueryMultipleAsync(query);
                doNotReceivedList = results.Read<NavpostedShipment>().ToList();
                acItemBalListResult = results.Read<DistStockBalModel>().ToList();
                pre_acItemBalListResult = results.Read<DistStockBalModel>().ToList();
                acEntries = results.Read<ACItemsModel>().ToList();
                dismapeditems = results.Read<NavItemCitemList>().ToList();
                categoryItems = results.Read<NavSaleCategory>().ToList();
                itemRelations = results.Read<NavitemLinks>().ToList();
                prodyctionTickets = results.Read<ProductionSimulation>().ToList();
                prodyctionoutTickets = results.Read<ProductionSimulation>().ToList();
                prenavStkbalance = results.Read<NavitemStockBalance>().ToList();
                navStkbalance = results.Read<NavitemStockBalance>().ToList();
                blanletOrders = results.Read<SimulationAddhoc>().ToList();
                grouptickets = results.Read<GroupPlaningTicket>().ToList();
                intercompanyItems = results.Read<Navitems>().ToList();
                itemMasterforReport = results.Read<Navitems>().ToList();
                orderRequirements = results.Read<OrderRequirementLineModel>().ToList();
                acEntriesList = results.Read<Acitems>().ToList();
                navMethodCodeLines = results.Read<NavMethodCodeLines>().ToList();
                acitems = results.Read<NavItemCitemList>().ToList();
            }

            if (itemMasterforReport != null && itemMasterforReport.Count() > 0)
            {
                itemMasterforReport.ForEach(s =>
                {
                    s.NavMethodCodeLines = navMethodCodeLines.Where(w => w.ItemId == s.ItemId).ToList();
                    s.NavItemCitemList = acitems.Where(w => w.NavItemId == s.ItemId).ToList();
                });
            }

            var packSize = 900;
            decimal packSize2 = 0;
            var tenderExist1 = false;
            var tenderExist2 = false;
            var tenderExist3 = false;
            var tenderExist4 = false;
            var tenderExist5 = false;
            var tenderExist6 = false;
            var tenderExist7 = false;
            var tenderExist8 = false;
            var tenderExist9 = false;
            var tenderExist10 = false;
            var tenderExist11 = false;
            var tenderExist12 = false;


            var month1 = month;
            var month2 = month1 + 1 > 12 ? 1 : month1 + 1;
            var month3 = month2 + 1 > 12 ? 1 : month2 + 1;
            var month4 = month3 + 1 > 12 ? 1 : month3 + 1;
            var month5 = month4 + 1 > 12 ? 1 : month4 + 1;
            var month6 = month5 + 1 > 12 ? 1 : month5 + 1;
            var month7 = month6 + 1 > 12 ? 1 : month6 + 1;
            var month8 = month7 + 1 > 12 ? 1 : month7 + 1;
            var month9 = month8 + 1 > 12 ? 1 : month8 + 1;
            var month10 = month9 + 1 > 12 ? 1 : month9 + 1;
            var month11 = month10 + 1 > 12 ? 1 : month10 + 1;
            var month12 = month11 + 1 > 12 ? 1 : month11 + 1;

            var nextYear = year + 1;
            var year1 = year;
            var year2 = month2 > month ? year : nextYear;
            var year3 = month3 > month ? year : nextYear;
            var year4 = month4 > month ? year : nextYear;
            var year5 = month5 > month ? year : nextYear;
            var year6 = month6 > month ? year : nextYear;
            var year7 = month7 > month ? year : nextYear;
            var year8 = month8 > month ? year : nextYear;
            var year9 = month9 > month ? year : nextYear;
            var year10 = month10 > month ? year : nextYear;
            var year11 = month11 > month ? year : nextYear;
            var year12 = month12 > month ? year : nextYear;


            var acModel = new List<ACItemsModel>();
            if (acEntries != null && acEntries.Count > 0)
            {
                acEntries.ForEach(ac =>
                {
                    if (ac.CustomerId == 62)
                    {
                        ac.DistName = "Apex";
                    }
                    else if (ac.CustomerId == 51)
                    {
                        ac.DistName = "PSB PX";
                    }
                    else if (ac.CustomerId == 39)
                    {
                        ac.DistName = "MSS";
                    }
                    else if (ac.CustomerId == 60)
                    {
                        ac.DistName = "SG Tender";
                    }
                    else
                    {
                        ac.DistName = "Antah";
                    }
                    acModel.Add(ac);
                });

            }

            if (prenavStkbalance != null && prenavStkbalance.Count() > 0)
            {
                prenavStkbalance.ForEach(f =>
                {
                    f.Quantity = f.Quantity * (long)(f.PackQty > 0 ? f.PackQty : 0);
                });
            }
            if (navStkbalance != null && navStkbalance.Count() > 0)
            {
                navStkbalance.ForEach(f =>
                {
                    f.Quantity = f.Quantity * (long)(f.PackQty > 0 ? f.PackQty : 0);
                });
            }

            doNotReceivedList.ForEach(d =>
            {
                var item = intercompanyItems.FirstOrDefault(f => f.No == d.ItemNo && f.CompanyId == d.CompanyId);
                if (item != null)
                {
                    d.DoQty *= item.PackQty;
                }
            });
            int? parent = null;
            var genericId = new List<long?>();

            var orederRequ = new List<OrderRequirementLineModel>();

            orderRequirements.ForEach(f =>
            {
                if (!f.RequireToSplit.GetValueOrDefault(false))
                {
                    orederRequ.Add(new OrderRequirementLineModel
                    {
                        ProductId = f.ProductId,
                        ProductQty = f.ProductQty * f.NoOfTicket,
                        NoOfTicket = f.NoOfTicket,
                        ExpectedStartDate = f.ExpectedStartDate
                    });
                }
                else
                {
                    orederRequ.Add(new OrderRequirementLineModel
                    {
                        ProductId = f.SplitProductId,
                        ProductQty = f.SplitProductQty * f.NoOfTicket,
                        NoOfTicket = f.NoOfTicket,
                        ExpectedStartDate = f.ExpectedStartDate
                    });
                }
            });
            itemMasterforReport.ForEach(ac =>
            {
                if (ac.No == "FP-PP-TAB-302")
                {

                }

                if (!genericId.Exists(g => g == ac.GenericCodeId) && ac.GenericCodeId != null)
                {
                    var geericRptList = new List<GenericCodeReport>();
                    var itemNosRpt = itemMasterforReport.Where(i => i.GenericCodeId == ac.GenericCodeId).ToList();
                    var itemNos = itemMasterforReport.Where(i => i.GenericCodeId == ac.GenericCodeId).Select(s => s.ItemId).ToList();
                    //itemMasterforReport.Where(i => i.GenericCodeId == ac.GenericCodeId).Select(s => string.Format("{0} - {1} - {2}", s.No, s.Description, s.Description2)).ToList();
                    var itemCodes = itemMasterforReport.Where(i => i.GenericCodeId == ac.GenericCodeId).Select(s => s.No).ToList();
                    itemNosRpt.ForEach(rpt =>
                    {
                        var distId = dismapeditems.Where(d => d.NavItemId == rpt.ItemId).Select(s => s.NavItemCustomerItemId);
                        geericRptList.Add(new GenericCodeReport
                        {
                            ItemNo = rpt.No,
                            Description = rpt.Description,
                            Description2 = rpt.Description2,
                            ItemCategory = rpt.CategoryId.GetValueOrDefault(0) > 0 ? categoryList[int.Parse(rpt.CategoryId.ToString()) - 1] : string.Empty,
                            InternalRefNo = rpt.InternalRef,
                            //MethodCode = rpt.NavMethodCodeLines.Count > 0 ? rpt.NavMethodCodeLines.First().MethodCode.MethodName : "No MethodCode",
                            //DistItem = rpt.NavItemCitemList.Count > 0 ? rpt.NavItemCitemList.FirstOrDefault().NavItemCustomerItem.ItemDesc : string.Empty,
                            //StockBalance = acItemBalListResult.Where(a => distId.Contains(a.DistItemId)).Sum(s => s.QtyOnHand)
                        });

                    });
                    bool itemMapped = false;
                    decimal SgTenderStockBalance = 0;
                    genericId.Add(ac.GenericCodeId);
                    decimal? inventoryQty = 0;//itemMasterforReport.Where(i => i.GenericCodeId == ac.GenericCodeId).Sum(s => s.Inventory);
                    var navItemIds = itemMasterforReport.Where(i => i.GenericCodeId == ac.GenericCodeId).Select(s => s.ItemId).ToList();
                    inventoryQty = (navStkbalance.Where(n => navItemIds.Contains(n.ItemId.Value)).Sum(s => s.Quantity));
                    var wipQty = (navStkbalance.Where(n => navItemIds.Contains(n.ItemId.Value)).Sum(s => s.Wipqty));
                    decimal? notStartInvQty = (navStkbalance.Where(n => navItemIds.Contains(n.ItemId.Value)).Sum(s => s.NotStartInvQty));
                    var reWorkQty = (navStkbalance.Where(n => navItemIds.Contains(n.ItemId.Value)).Sum(s => s.ReworkQty));
                    var globalQty = (navStkbalance.Where(n => navItemIds.Contains(n.ItemId.Value)).Sum(s => s.GlobalQty));
                    decimal? interCompanyTransitQty = 0;

                    if (ac.No == "FP-PP-TAB-302")
                    {

                    }

                    if (endDate.CompanyId == 1)
                    {
                        var linkItemIds = itemRelations.Where(f => navItemIds.Contains(f.MyItemId.Value)).Select(s => s.SgItemId).ToList();
                        if (linkItemIds.Count > 0)
                        {
                            itemMapped = true;
                            SgTenderStockBalance = navStkbalance.Where(n => linkItemIds.Contains(n.ItemId.Value)).Sum(s => s.Quantity.GetValueOrDefault(0));

                            var itemNosIntertrans = intercompanyItems.Where(t => linkItemIds.Contains(t.ItemId) && t.CompanyId == 2).Select(n => n.No).ToList();
                            interCompanyTransitQty = doNotReceivedList.Where(d => itemNosIntertrans.Contains(d.ItemNo) && d.IsRecived == false && d.CompanyId == 2).Sum(s => s.DoQty);
                        }


                    }
                    else if (endDate.CompanyId == 2)
                    {
                        var linkItemIds = itemRelations.Where(f => navItemIds.Contains(f.SgItemId.Value)).Select(s => s.MyItemId).ToList();
                        if (linkItemIds.Count > 0)
                        {
                            itemMapped = true;
                            SgTenderStockBalance = navStkbalance.Where(n => linkItemIds.Contains(n.ItemId.Value)).Sum(s => s.Quantity.GetValueOrDefault(0));

                            var itemNosIntertrans = intercompanyItems.Where(t => linkItemIds.Contains(t.ItemId) && t.CompanyId == 1).Select(n => n.No).ToList();
                            interCompanyTransitQty = doNotReceivedList.Where(d => itemNosIntertrans.Contains(d.ItemNo) && d.IsRecived == false && d.CompanyId == 1).Sum(s => s.DoQty);
                        }
                    }
                    else if (endDate.CompanyId == 3)
                    {
                        var linkItemIds = itemRelations.Where(f => navItemIds.Contains(f.MyItemId.Value)).Select(s => s.SgItemId).ToList();
                        if (linkItemIds.Count > 0)
                        {
                            itemMapped = true;
                            inventoryQty = (navStkbalance.Where(n => linkItemIds.Contains(n.ItemId.Value)).Sum(s => s.Quantity.GetValueOrDefault(0)));
                        }

                        var sglinkItemIds = itemRelations.Where(f => navItemIds.Contains(f.SgItemId.Value)).Select(s => s.MyItemId).ToList();
                        if (linkItemIds.Count > 0) SgTenderStockBalance = navStkbalance.Where(n => sglinkItemIds.Contains(n.ItemId.Value)).Sum(s => s.Quantity.GetValueOrDefault(0));
                    }
                    var custmerItem = ac.NavItemCitemList.Count > 0 ? ac.NavItemCitemList.FirstOrDefault() : new NavItemCitemList();
                    if (custmerItem.DistAcid > 0)
                    {
                        var distIds = dismapeditems.Where(di => navItemIds.Contains(di.NavItemId.Value)).Select(s => s.NavItemCustomerItemId).Distinct().ToList(); //ac.NavItemCitemList.Select(s => s.NavItemCustomerItemId).ToList();
                        var distStkBal = acItemBalListResult.Where(d => distIds.Contains(d.DistItemId) && d.CompanyId == endDate.CompanyId).Sum(s => s.QtyOnHand);
                        var val = ac.PackSize.HasValue ? ac.PackSize.Value : 900;
                        packSize = val;

                        var prodRecipe = "";
                        var currentBatch = "";
                        var methodeCodeID = ac.NavMethodCodeLines.Count() > 0 ? ac.NavMethodCodeLines.First().MethodCodeId : -1;
                        var itemreceips = recipeList.Where(r => r.ItemRecipeId == methodeCodeID).ToList();
                        if (itemreceips.Count > 0)
                        {
                            var batchSizedes = itemreceips.FirstOrDefault().DefaultBatch;
                            currentBatch = batchSizedes;
                            //string numberOnly = Regex.Replace(batchSizedes, "[^0-9.]", "");
                            //if (!string.IsNullOrEmpty(numberOnly))
                            packSize2 = itemreceips.FirstOrDefault().UnitQTY;
                            prodRecipe = string.Join(",", itemreceips.Select(r => r.RecipeNo).ToList());//string.Format("{0}", itemreceips.FirstOrDefault().DefaultBatch + " | " + batchSizedes);

                        }
                        var Orderitemreceips = _orderRecipeList.Where(r => r.ItemNo == ac.No).ToList();
                        //if (Orderitemreceips.Count > 0)
                        //{
                        //    //var batchSizedes = itemreceips.OrderByDescending(o => o.BatchSize).FirstOrDefault().BatchSize;
                        //    //string numberOnly = Regex.Replace(batchSizedes, "[^0-9.]", "");
                        //    //if (!string.IsNullOrEmpty(numberOnly))
                        //    //    packSize2 = decimal.Parse(numberOnly) * 1000;
                        //    //prodRecipe = string.Format("{0}", itemreceips.OrderByDescending(o => o.BatchSize).FirstOrDefault().RecipeNo + " | " + batchSizedes);

                        //}
                        var threeMonth = distStkBal * packSize * 3;
                        var BatchSize = 900000 / packSize / 50;
                        var StockBalance = distStkBal + inventoryQty;
                        var NoofTickets = (threeMonth / packSize * 1000);
                        //var custId = custmerItem.DistName == "Apex" ? 21 : 1;
                        var acitem = acModel.Where(f => navItemIds.Contains(f.SWItemId.Value)).ToList();
                        var distItemBal = acItemBalListResult.Where(f => distIds.Contains(f.DistAcid) && f.CompanyId == endDate.CompanyId).ToList();
                        var apexQty = acitem.FirstOrDefault(f => f.CustomerId == 62) != null ? acitem.Where(f => f.CustomerId == 62).Sum(s => s.ACQty) : 0;
                        var antahQty = acitem.FirstOrDefault(f => f.CustomerId == 1) != null ? acitem.Where(f => f.CustomerId == 1).Sum(s => s.ACQty) : 0;
                        var sgtQty = acitem.FirstOrDefault(f => f.CustomerId == 60) != null ? acitem.Where(f => f.CustomerId == 60).Sum(s => s.ACQty) : 0;
                        var missQty = acitem.FirstOrDefault(f => f.CustomerId == 39) != null ? acitem.Where(f => f.CustomerId == 39).Sum(s => s.ACQty) : 0;
                        var pxQty = acitem.FirstOrDefault(f => f.CustomerId == 51) != null ? acitem.Where(f => f.CustomerId == 51).Sum(s => s.ACQty) : 0;
                        //sgtQty
                        var symlQty = packSize * (apexQty + antahQty + 0 + missQty + pxQty);
                        threeMonth = symlQty * 3;
                        var distTotal = (apexQty + antahQty + 0 + missQty + pxQty);
                        var AntahStockBalance = distItemBal.FirstOrDefault(f => f.CustomerId == 1) != null ? distItemBal.Where(f => f.CustomerId == 1).Sum(s => s.QtyOnHand) : 0;
                        var ApexStockBalance = distItemBal.FirstOrDefault(f => f.CustomerId == 62) != null ? distItemBal.Where(f => f.CustomerId == 62).Sum(s => s.QtyOnHand) : 0;
                        if (!itemMapped) SgTenderStockBalance = distItemBal.FirstOrDefault(f => f.DistName == "SG Tender") != null ? distItemBal.Where(f => f.DistName == "SG Tender").Sum(s => s.QtyOnHand) : 0;
                        var MsbStockBalance = distItemBal.FirstOrDefault(f => f.CustomerId == 39) != null ? distItemBal.Where(f => f.CustomerId == 39).Sum(s => s.QtyOnHand) : 0;
                        var PsbStockBalance = distItemBal.FirstOrDefault(f => f.CustomerId == 51) != null ? distItemBal.Where(f => f.CustomerId == 51).Sum(s => s.QtyOnHand) : 0;
                        decimal stockHoldingBalance = 0;
                        // decimal myStockBalance = 0;

                        var inTransitQty = doNotReceivedList.Where(d => itemCodes.Contains(d.ItemNo) && d.IsRecived == false && d.CompanyId == endDate.CompanyId).Sum(s => s.DoQty);
                        if (distTotal > 0)
                        {
                            stockHoldingBalance = (interCompanyTransitQty.GetValueOrDefault(0) + notStartInvQty.GetValueOrDefault(0) + wipQty.GetValueOrDefault(0) + 0 + globalQty.GetValueOrDefault(0) + AntahStockBalance + ApexStockBalance + MsbStockBalance + PsbStockBalance + SgTenderStockBalance + inventoryQty.GetValueOrDefault(0) + inTransitQty.GetValueOrDefault(0)) / distTotal;
                        }

                        var isTenderExist = blanletOrders.Any(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)));
                        tenderExist1 = blanletOrders.Any(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month1 && t.ShipmentDate.Value.Year == year1);
                        tenderExist2 = blanletOrders.Any(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month2 && t.ShipmentDate.Value.Year == year2);
                        tenderExist3 = blanletOrders.Any(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month3 && t.ShipmentDate.Value.Year == year3);
                        tenderExist4 = blanletOrders.Any(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month4 && t.ShipmentDate.Value.Year == year4);
                        tenderExist5 = blanletOrders.Any(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month5 && t.ShipmentDate.Value.Year == year5);
                        tenderExist6 = blanletOrders.Any(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month6 && t.ShipmentDate.Value.Year == year6);
                        tenderExist7 = blanletOrders.Any(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month7 && t.ShipmentDate.Value.Year == year7);
                        tenderExist8 = blanletOrders.Any(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month8 && t.ShipmentDate.Value.Year == year8);
                        tenderExist9 = blanletOrders.Any(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month9 && t.ShipmentDate.Value.Year == year9);
                        tenderExist10 = blanletOrders.Any(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month10 && t.ShipmentDate.Value.Year == year10);
                        tenderExist11 = blanletOrders.Any(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month11 && t.ShipmentDate.Value.Year == year11);
                        tenderExist12 = blanletOrders.Any(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month12 && t.ShipmentDate.Value.Year == year12);
                        var groupItemNo = itemMasterforReport.Where(i => i.GenericCodeId == ac.GenericCodeId).Select(s => s.No).ToList();
                        itemdict.Add(ac.No, symlQty);
                        //var tenderSum = blanletOrders.Where(t => itemNos.Contains(t.ItemId.Value)).Sum(s => s.OutstandingQty.Value);
                        MethodCodeList.Add(new INPCalendarPivotModel
                        {
                            GenericCodeReport = geericRptList,
                            GrouoItemNo = ac.No,
                            ItemId = ac.ItemId,
                            ItemNo = ac.GenericCodeDescription2,
                            RecipeLists = itemreceips.Select(s => string.Format("{0}", s.RecipeNo + " | " + s.BatchSize)).ToList(),
                            ItemRecipeLists = itemreceips,
                            OrderRecipeLists = Orderitemreceips,
                            IsSteroid = ac.Steroid.GetValueOrDefault(false),
                            SalesCategoryId = ac.CategoryId,
                            SalesCategory = salesCatLists.Where(w => w.Id == ac.CategoryId).FirstOrDefault()?.Text,
                            LocationID = categoryItems != null && categoryItems.FirstOrDefault(a => a.SaleCategoryId == ac.CategoryId) != null ? categoryItems.FirstOrDefault(a => a.SaleCategoryId == ac.CategoryId).LocationId : null,
                            // SalesCategory = categoryItems != null && categoryItems.FirstOrDefault(a => a.SaleCategoryId == ac.CategoryId) != null ? categoryItems.FirstOrDefault(a => a.SaleCategoryId == ac.CategoryId).Code : "",
                            MethodCodeId = ac.NavMethodCodeLines.Count > 0 ? ac.NavMethodCodeLines.First().MethodCodeId : -1,
                            GenericCodeID = ac.GenericCodeId,
                            Customer = custmerItem.DistName,
                            Description = ac.Description + " " + ac.Description2,
                            PackSize = packSize,
                            PackSize2 = packSize2,
                            Quantity = distStkBal,
                            ApexQty = apexQty,
                            AntahQty = antahQty,
                            SgtQty = sgtQty,
                            PxQty = pxQty,
                            MissQty = missQty,
                            SymlQty = symlQty,
                            ACQty = distStkBal,
                            AcSum = distTotal,
                            UnitQty = distStkBal * packSize,
                            ThreeMonthACQty = threeMonth,
                            ProdRecipe = !string.IsNullOrEmpty(prodRecipe) ? prodRecipe : "",
                            BatchSize = "",
                            NoofTickets = "",
                            NoOfDays = "Hours " + (3 * NoofTickets) + "& Day" + (3 * NoofTickets / 8).ToString(),
                            DistStockBalance = (AntahStockBalance + ApexStockBalance + MsbStockBalance + PsbStockBalance + SgTenderStockBalance + inventoryQty.GetValueOrDefault(0)),
                            AntahStockBalance = AntahStockBalance,
                            ApexStockBalance = ApexStockBalance,
                            SgTenderStockBalance = endDate.CompanyId == 2 ? inventoryQty.GetValueOrDefault(0) : SgTenderStockBalance,
                            MsbStockBalance = MsbStockBalance,
                            PsbStockBalance = PsbStockBalance,
                            NAVStockBalance = inventoryQty.GetValueOrDefault(0),
                            WipQty = wipQty.GetValueOrDefault(0),
                            NotStartInvQty = notStartInvQty.GetValueOrDefault(0),
                            Rework = reWorkQty.GetValueOrDefault(0),
                            InterCompanyTransitQty = interCompanyTransitQty.GetValueOrDefault(0),
                            OtherStoreQty = globalQty.GetValueOrDefault(0),
                            StockBalance = (interCompanyTransitQty.GetValueOrDefault(0) + notStartInvQty.GetValueOrDefault(0) + wipQty.GetValueOrDefault(0) + 0 + globalQty.GetValueOrDefault(0) + AntahStockBalance + ApexStockBalance + MsbStockBalance + PsbStockBalance + SgTenderStockBalance + inventoryQty.GetValueOrDefault(0) + inTransitQty.Value),
                            StockHoldingPackSize = (interCompanyTransitQty.GetValueOrDefault(0) + notStartInvQty.GetValueOrDefault(0) + wipQty.GetValueOrDefault(0) + 0 + globalQty.GetValueOrDefault(0) + AntahStockBalance + ApexStockBalance + MsbStockBalance + PsbStockBalance + SgTenderStockBalance + inventoryQty.GetValueOrDefault(0) + inTransitQty.Value) * packSize,
                            StockHoldingBalance = stockHoldingBalance,
                            MyStockBalance = endDate.CompanyId == 1 || endDate.CompanyId == 3 ? inventoryQty.GetValueOrDefault(0) : SgTenderStockBalance,
                            SgStockBalance = endDate.CompanyId == 2 ? inventoryQty.GetValueOrDefault(0) : SgTenderStockBalance,
                            MethodCode = ac.NavMethodCodeLines.Count > 0 ? ac.NavMethodCodeLines.First().MethodName : "No MethodCode",
                            Month = endDate.StockMonth.ToString("MMMM"),
                            Remarks = "AC",
                            BatchSize90 = currentBatch,
                            Roundup1 = packSize2 > 0 ? threeMonth / packSize2 : 0,
                            Roundup2 = 0,
                            ReportMonth = endDate.StockMonth,
                            UOM = ac.BaseUnitofMeasure,
                            Packuom = ac.PackUom,
                            Replenishment = ac.VendorNo,
                            StatusCodeId = ac.StatusCodeId,
                            isTenderExist = isTenderExist,
                            Itemgrouping = distTotal > 0 ? "Item with AC" : "Item without AC",
                            //TenderSum = tenderSum,
                            //Month1 = stockHoldingBalance > 1 ? stockHoldingBalance - 1 : 0,
                            //Month2 = stockHoldingBalance > 2 ? stockHoldingBalance - 2 : 0,
                            //Month3 = stockHoldingBalance > 3 ? stockHoldingBalance - 3 : 0,
                            //Month4 = stockHoldingBalance > 4 ? stockHoldingBalance - 4 : 0,
                            //Month5 = stockHoldingBalance > 5 ? stockHoldingBalance - 5 : 0,
                            //Month6 = stockHoldingBalance > 6 ? stockHoldingBalance - 6 : 0,
                            //Month7 = stockHoldingBalance > 7 ? stockHoldingBalance - 7 : 0,
                            //Month8 = stockHoldingBalance > 8 ? stockHoldingBalance - 8 : 0,
                            //Month9 = stockHoldingBalance > 9 ? stockHoldingBalance - 9 : 0,
                            //Month10 = stockHoldingBalance > 10 ? stockHoldingBalance - 10 : 0,
                            //Month11 = stockHoldingBalance > 11 ? stockHoldingBalance - 11 : 0,
                            //Month12 = stockHoldingBalance > 12 ? stockHoldingBalance - 12 : 0,

                            Month1 = stockHoldingBalance - 1,
                            Month2 = stockHoldingBalance - 2,
                            Month3 = stockHoldingBalance - 3,
                            Month4 = stockHoldingBalance - 4,
                            Month5 = stockHoldingBalance - 5,
                            Month6 = stockHoldingBalance - 6,
                            Month7 = stockHoldingBalance - 7,
                            Month8 = stockHoldingBalance - 8,
                            Month9 = stockHoldingBalance - 9,
                            Month10 = stockHoldingBalance - 10,
                            Month11 = stockHoldingBalance - 11,
                            Month12 = stockHoldingBalance - 12,
                            DeliverynotReceived = inTransitQty,//doNotReceivedList.Where(d => d.ItemNo == ac.No && d.IsRecived == false).Sum(s => s.DoQty),


                            GroupItemTicket1 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month1 && t.StartDate.Value.Year == year1).Select(s => s.Quantity).ToList()),
                            GroupItemTicket2 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month2 && t.StartDate.Value.Year == year2).Select(s => s.Quantity).ToList()),
                            GroupItemTicket3 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month3 && t.StartDate.Value.Year == year3).Select(s => s.Quantity).ToList()),
                            GroupItemTicket4 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month4 && t.StartDate.Value.Year == year4).Select(s => s.Quantity).ToList()),
                            GroupItemTicket5 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month5 && t.StartDate.Value.Year == year5).Select(s => s.Quantity).ToList()),
                            GroupItemTicket6 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month6 && t.StartDate.Value.Year == year6).Select(s => s.Quantity).ToList()),
                            GroupItemTicket7 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month7 && t.StartDate.Value.Year == year7).Select(s => s.Quantity).ToList()),
                            GroupItemTicket8 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month8 && t.StartDate.Value.Year == year8).Select(s => s.Quantity).ToList()),
                            GroupItemTicket9 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month9 && t.StartDate.Value.Year == year9).Select(s => s.Quantity).ToList()),
                            GroupItemTicket10 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month10 && t.StartDate.Value.Year == year10).Select(s => s.Quantity).ToList()),
                            GroupItemTicket11 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month11 && t.StartDate.Value.Year == year11).Select(s => s.Quantity).ToList()),
                            GroupItemTicket12 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month12 && t.StartDate.Value.Year == year12).Select(s => s.Quantity).ToList()),

                            NoOfTicket1 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month1 && t.StartDate.Value.Year == year1).Select(s => s.NoOfTicket).ToList()),
                            NoOfTicket2 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month2 && t.StartDate.Value.Year == year2).Select(s => s.NoOfTicket).ToList()),
                            NoOfTicket3 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month3 && t.StartDate.Value.Year == year3).Select(s => s.NoOfTicket).ToList()),
                            NoOfTicket4 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month4 && t.StartDate.Value.Year == year4).Select(s => s.NoOfTicket).ToList()),
                            NoOfTicket5 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month5 && t.StartDate.Value.Year == year5).Select(s => s.NoOfTicket).ToList()),
                            NoOfTicket6 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month6 && t.StartDate.Value.Year == year6).Select(s => s.NoOfTicket).ToList()),
                            NoOfTicket7 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month7 && t.StartDate.Value.Year == year7).Select(s => s.NoOfTicket).ToList()),
                            NoOfTicket8 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month8 && t.StartDate.Value.Year == year8).Select(s => s.NoOfTicket).ToList()),
                            NoOfTicket9 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month9 && t.StartDate.Value.Year == year9).Select(s => s.NoOfTicket).ToList()),
                            NoOfTicket10 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month10 && t.StartDate.Value.Year == year10).Select(s => s.NoOfTicket).ToList()),
                            NoOfTicket11 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month11 && t.StartDate.Value.Year == year11).Select(s => s.NoOfTicket).ToList()),
                            NoOfTicket12 = string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month12 && t.StartDate.Value.Year == year12).Select(s => s.NoOfTicket).ToList()),



                            Ticket1 = string.Format("{0}/{1}", string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month1 && t.StartDate.Value.Year == year1).Select(s => s.Quantity).ToList()), string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month1 && t.StartDate.Value.Year == year1).Select(s => s.NoOfTicket).ToList())),
                            Ticket2 = string.Format("{0}/{1}", string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month2 && t.StartDate.Value.Year == year2).Select(s => s.Quantity).ToList()), string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month2 && t.StartDate.Value.Year == year2).Select(s => s.NoOfTicket).ToList())),
                            Ticket3 = string.Format("{0}/{1}", string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month3 && t.StartDate.Value.Year == year3).Select(s => s.Quantity).ToList()), string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month3 && t.StartDate.Value.Year == year3).Select(s => s.NoOfTicket).ToList())),
                            Ticket4 = string.Format("{0}/{1}", string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month4 && t.StartDate.Value.Year == year4).Select(s => s.Quantity).ToList()), string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month4 && t.StartDate.Value.Year == year4).Select(s => s.NoOfTicket).ToList())),
                            Ticket5 = string.Format("{0}/{1}", string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month5 && t.StartDate.Value.Year == year5).Select(s => s.Quantity).ToList()), string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month5 && t.StartDate.Value.Year == year5).Select(s => s.NoOfTicket).ToList())),
                            Ticket6 = string.Format("{0}/{1}", string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month6 && t.StartDate.Value.Year == year6).Select(s => s.Quantity).ToList()), string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month6 && t.StartDate.Value.Year == year6).Select(s => s.NoOfTicket).ToList())),
                            Ticket7 = string.Format("{0}/{1}", string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month7 && t.StartDate.Value.Year == year7).Select(s => s.Quantity).ToList()), string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month7 && t.StartDate.Value.Year == year7).Select(s => s.NoOfTicket).ToList())),
                            Ticket8 = string.Format("{0}/{1}", string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month8 && t.StartDate.Value.Year == year8).Select(s => s.Quantity).ToList()), string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month8 && t.StartDate.Value.Year == year8).Select(s => s.NoOfTicket).ToList())),
                            Ticket9 = string.Format("{0}/{1}", string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month9 && t.StartDate.Value.Year == year9).Select(s => s.Quantity).ToList()), string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month9 && t.StartDate.Value.Year == year9).Select(s => s.NoOfTicket).ToList())),
                            Ticket10 = string.Format("{0}/{1}", string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month10 && t.StartDate.Value.Year == year10).Select(s => s.Quantity).ToList()), string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month10 && t.StartDate.Value.Year == year10).Select(s => s.NoOfTicket).ToList())),
                            Ticket11 = string.Format("{0}/{1}", string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month11 && t.StartDate.Value.Year == year11).Select(s => s.Quantity).ToList()), string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month11 && t.StartDate.Value.Year == year11).Select(s => s.NoOfTicket).ToList())),
                            Ticket12 = string.Format("{0}/{1}", string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month12 && t.StartDate.Value.Year == year12).Select(s => s.Quantity).ToList()), string.Join(",", grouptickets.Where(t => groupItemNo.Contains(t.ItemNo) && t.StartDate.Value.Month == month12 && t.StartDate.Value.Year == year12).Select(s => s.NoOfTicket).ToList())),

                            OutputTicket1 = string.Format("{0}/{1}", string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month1).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month1).Select(s => s.ProdOrderNo).ToList())),
                            OutputTicket2 = string.Format("{0}/{1}", string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month2).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month2).Select(s => s.ProdOrderNo).ToList())),
                            OutputTicket3 = string.Format("{0}/{1}", string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month3).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month3).Select(s => s.ProdOrderNo).ToList())),
                            OutputTicket4 = string.Format("{0}/{1}", string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month4).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month4).Select(s => s.ProdOrderNo).ToList())),
                            OutputTicket5 = string.Format("{0}/{1}", string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month5).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month5).Select(s => s.ProdOrderNo).ToList())),
                            OutputTicket6 = string.Format("{0}/{1}", string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month6).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month6).Select(s => s.ProdOrderNo).ToList())),
                            OutputTicket7 = string.Format("{0}/{1}", string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month7).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month7).Select(s => s.ProdOrderNo).ToList())),
                            OutputTicket8 = string.Format("{0}/{1}", string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month8).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month8).Select(s => s.ProdOrderNo).ToList())),
                            OutputTicket9 = string.Format("{0}/{1}", string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month9).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month9).Select(s => s.ProdOrderNo).ToList())),
                            OutputTicket10 = string.Format("{0}/{1}", string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month10).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month10).Select(s => s.ProdOrderNo).ToList())),
                            OutputTicket11 = string.Format("{0}/{1}", string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month11).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month11).Select(s => s.ProdOrderNo).ToList())),
                            OutputTicket12 = string.Format("{0}/{1}", string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month12).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month12).Select(s => s.ProdOrderNo).ToList())),

                            TicketHoldingStock1 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month1).Sum(s => s.NoOfTicket.GetValueOrDefault(0)),
                            TicketHoldingStock2 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month2).Sum(s => s.NoOfTicket.GetValueOrDefault(0)),
                            TicketHoldingStock3 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month3).Sum(s => s.NoOfTicket.GetValueOrDefault(0)),
                            TicketHoldingStock4 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month4).Sum(s => s.NoOfTicket.GetValueOrDefault(0)),
                            TicketHoldingStock5 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month5).Sum(s => s.NoOfTicket.GetValueOrDefault(0)),
                            TicketHoldingStock6 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month6).Sum(s => s.NoOfTicket.GetValueOrDefault(0)),
                            TicketHoldingStock7 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month7).Sum(s => s.NoOfTicket.GetValueOrDefault(0)),
                            TicketHoldingStock8 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month8).Sum(s => s.NoOfTicket.GetValueOrDefault(0)),
                            TicketHoldingStock9 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month9).Sum(s => s.NoOfTicket.GetValueOrDefault(0)),
                            TicketHoldingStock10 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month10).Sum(s => s.NoOfTicket.GetValueOrDefault(0)),
                            TicketHoldingStock11 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month11).Sum(s => s.NoOfTicket.GetValueOrDefault(0)),
                            TicketHoldingStock12 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month12).Sum(s => s.NoOfTicket.GetValueOrDefault(0)),

                            ProductionTicket1 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month1).Sum(s => s.ProductQty.GetValueOrDefault(0)),
                            ProductionTicket2 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month2).Sum(s => s.ProductQty.GetValueOrDefault(0)),
                            ProductionTicket3 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month3).Sum(s => s.ProductQty.GetValueOrDefault(0)),
                            ProductionTicket4 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month4).Sum(s => s.ProductQty.GetValueOrDefault(0)),
                            ProductionTicket5 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month5).Sum(s => s.ProductQty.GetValueOrDefault(0)),
                            ProductionTicket6 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month6).Sum(s => s.ProductQty.GetValueOrDefault(0)),
                            ProductionTicket7 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month7).Sum(s => s.ProductQty.GetValueOrDefault(0)),
                            ProductionTicket8 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month8).Sum(s => s.ProductQty.GetValueOrDefault(0)),
                            ProductionTicket9 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month9).Sum(s => s.ProductQty.GetValueOrDefault(0)),
                            ProductionTicket10 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month10).Sum(s => s.ProductQty.GetValueOrDefault(0)),
                            ProductionTicket11 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month11).Sum(s => s.ProductQty.GetValueOrDefault(0)),
                            ProductionTicket12 = orederRequ.Where(t => itemNos.Contains(t.ProductId.GetValueOrDefault(-1)) && t.ExpectedStartDate.Value.Month == month12).Sum(s => s.ProductQty.GetValueOrDefault(0)),



                            IsTenderExist1 = tenderExist1,//blanletOrders.Any(t => itemNos.Contains(t.ItemId.Value) && t.ShipmentDate.Value.Month == month1),
                            IsTenderExist2 = tenderExist2,// blanletOrders.Any(t => itemNos.Contains(t.ItemId.Value) && t.ShipmentDate.Value.Month == month2),
                            IsTenderExist3 = tenderExist3,// blanletOrders.Any(t => itemNos.Contains(t.ItemId.Value) && t.ShipmentDate.Value.Month == month3),
                            IsTenderExist4 = tenderExist4,//blanletOrders.Any(t => itemNos.Contains(t.ItemId.Value) && t.ShipmentDate.Value.Month == month4),
                            IsTenderExist5 = tenderExist5,//blanletOrders.Any(t => itemNos.Contains(t.ItemId.Value) && t.ShipmentDate.Value.Month == month5),
                            IsTenderExist6 = tenderExist6,//blanletOrders.Any(t => itemNos.Contains(t.ItemId.Value) && t.ShipmentDate.Value.Month == month6),
                            IsTenderExist7 = tenderExist7,//blanletOrders.Any(t => itemNos.Contains(t.ItemId.Value) && t.ShipmentDate.Value.Month == month7),
                            IsTenderExist8 = tenderExist8,//blanletOrders.Any(t => itemNos.Contains(t.ItemId.Value) && t.ShipmentDate.Value.Month == month8),
                            IsTenderExist9 = tenderExist9,//blanletOrders.Any(t => itemNos.Contains(t.ItemId.Value) && t.ShipmentDate.Value.Month == month9),
                            IsTenderExist10 = tenderExist10,//blanletOrders.Any(t => itemNos.Contains(t.ItemId.Value) && t.ShipmentDate.Value.Month == month10),
                            IsTenderExist11 = tenderExist11,//blanletOrders.Any(t => itemNos.Contains(t.ItemId.Value) && t.ShipmentDate.Value.Month == month11),
                            IsTenderExist12 = tenderExist12,//blanletOrders.Any(t => itemNos.Contains(t.ItemId.Value) && t.ShipmentDate.Value.Month == month12),


                            ProjectedHoldingStock1 = distTotal > 0 ? prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month1).Sum(s => s.Quantity) / (distTotal) : prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month1).Sum(s => s.Quantity),
                            ProjectedHoldingStock2 = distTotal > 0 ? prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month2).Sum(s => s.Quantity) / (distTotal) : prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month2).Sum(s => s.Quantity),
                            ProjectedHoldingStock3 = distTotal > 0 ? prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month3).Sum(s => s.Quantity) / (distTotal) : prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month3).Sum(s => s.Quantity),
                            ProjectedHoldingStock4 = distTotal > 0 ? prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month4).Sum(s => s.Quantity) / (distTotal) : prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month4).Sum(s => s.Quantity),
                            ProjectedHoldingStock5 = distTotal > 0 ? prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month5).Sum(s => s.Quantity) / (distTotal) : prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month5).Sum(s => s.Quantity),
                            ProjectedHoldingStock6 = distTotal > 0 ? prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month6).Sum(s => s.Quantity) / (distTotal) : prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month6).Sum(s => s.Quantity),
                            ProjectedHoldingStock7 = distTotal > 0 ? prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month7).Sum(s => s.Quantity) / (distTotal) : prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month7).Sum(s => s.Quantity),
                            ProjectedHoldingStock8 = distTotal > 0 ? prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month8).Sum(s => s.Quantity) / (distTotal) : prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month8).Sum(s => s.Quantity),
                            ProjectedHoldingStock9 = distTotal > 0 ? prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month9).Sum(s => s.Quantity) / (distTotal) : prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month9).Sum(s => s.Quantity),
                            ProjectedHoldingStock10 = distTotal > 0 ? prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month10).Sum(s => s.Quantity) / (distTotal) : prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month10).Sum(s => s.Quantity),
                            ProjectedHoldingStock11 = distTotal > 0 ? prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month11).Sum(s => s.Quantity) / (distTotal) : prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month11).Sum(s => s.Quantity),
                            ProjectedHoldingStock12 = distTotal > 0 ? prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month12).Sum(s => s.Quantity) / (distTotal) : prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month12).Sum(s => s.Quantity),

                            OutputProjectedHoldingStock1 = distTotal > 0 ? prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month1).Sum(s => s.Quantity) / (distTotal) : prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month1).Sum(s => s.Quantity),
                            OutputProjectedHoldingStock2 = distTotal > 0 ? prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month2).Sum(s => s.Quantity) / (distTotal) : prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month2).Sum(s => s.Quantity),
                            OutputProjectedHoldingStock3 = distTotal > 0 ? prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month3).Sum(s => s.Quantity) / (distTotal) : prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month3).Sum(s => s.Quantity),
                            OutputProjectedHoldingStock4 = distTotal > 0 ? prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month4).Sum(s => s.Quantity) / (distTotal) : prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month4).Sum(s => s.Quantity),
                            OutputProjectedHoldingStock5 = distTotal > 0 ? prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month5).Sum(s => s.Quantity) / (distTotal) : prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month5).Sum(s => s.Quantity),
                            OutputProjectedHoldingStock6 = distTotal > 0 ? prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month6).Sum(s => s.Quantity) / (distTotal) : prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month6).Sum(s => s.Quantity),
                            OutputProjectedHoldingStock7 = distTotal > 0 ? prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month7).Sum(s => s.Quantity) / (distTotal) : prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month7).Sum(s => s.Quantity),
                            OutputProjectedHoldingStock8 = distTotal > 0 ? prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month8).Sum(s => s.Quantity) / (distTotal) : prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month8).Sum(s => s.Quantity),
                            OutputProjectedHoldingStock9 = distTotal > 0 ? prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month9).Sum(s => s.Quantity) / (distTotal) : prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month9).Sum(s => s.Quantity),
                            OutputProjectedHoldingStock10 = distTotal > 0 ? prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month10).Sum(s => s.Quantity) / (distTotal) : prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month10).Sum(s => s.Quantity),
                            OutputProjectedHoldingStock11 = distTotal > 0 ? prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month11).Sum(s => s.Quantity) / (distTotal) : prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month11).Sum(s => s.Quantity),
                            OutputProjectedHoldingStock12 = distTotal > 0 ? prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month12).Sum(s => s.Quantity) / (distTotal) : prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month12).Sum(s => s.Quantity),


                            BlanketAddhoc1 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month1 && t.ShipmentDate.Value.Year == year1).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                            BlanketAddhoc2 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month2 && t.ShipmentDate.Value.Year == year2).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                            BlanketAddhoc3 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month3 && t.ShipmentDate.Value.Year == year3).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                            BlanketAddhoc4 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month4 && t.ShipmentDate.Value.Year == year4).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                            BlanketAddhoc5 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month5 && t.ShipmentDate.Value.Year == year5).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                            BlanketAddhoc6 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month6 && t.ShipmentDate.Value.Year == year6).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                            BlanketAddhoc7 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month7 && t.ShipmentDate.Value.Year == year7).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                            BlanketAddhoc8 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month8 && t.ShipmentDate.Value.Year == year8).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                            BlanketAddhoc9 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month9 && t.ShipmentDate.Value.Year == year9).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                            BlanketAddhoc10 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month10 && t.ShipmentDate.Value.Year == year10).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                            BlanketAddhoc11 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month11 && t.ShipmentDate.Value.Year == year11).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                            BlanketAddhoc12 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.ShipmentDate.Value.Month == month12 && t.ShipmentDate.Value.Year == year12).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),


                        });
                    }
                }
            });
            MethodCodeList = MethodCodeList.Where(f => f.StatusCodeId == 1).ToList();

            MethodCodeList.ForEach(f =>
            {

                if (f.ItemNo.Contains("Folic Acid Tablet"))
                {

                }
                var itemNos = itemMasterforReport.Where(i => i.GenericCodeId == f.GenericCodeID).Select(s => s.ItemId).ToList();

                var item_Nos = itemMasterforReport.Where(i => i.GenericCodeId == f.GenericCodeID).Select(s => s.No).ToList();


                f.GroupTicket1 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month1).Select(s => s.Quantity).ToList());
                f.GroupTicket2 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month2).Select(s => s.Quantity).ToList());
                f.GroupTicket3 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month3).Select(s => s.Quantity).ToList());
                f.GroupTicket4 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month4).Select(s => s.Quantity).ToList());
                f.GroupTicket5 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month5).Select(s => s.Quantity).ToList());
                f.GroupTicket6 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month6).Select(s => s.Quantity).ToList());
                f.GroupTicket7 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month7).Select(s => s.Quantity).ToList());
                f.GroupTicket8 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month8).Select(s => s.Quantity).ToList());
                f.GroupTicket9 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month9).Select(s => s.Quantity).ToList());
                f.GroupTicket10 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month10).Select(s => s.Quantity).ToList());
                f.GroupTicket11 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month11).Select(s => s.Quantity).ToList());
                f.GroupTicket12 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month12).Select(s => s.Quantity).ToList());

                f.ProdOrderNo1 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month1).Select(s => s.ProdOrderNo).ToList());
                f.ProdOrderNo2 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month2).Select(s => s.ProdOrderNo).ToList());
                f.ProdOrderNo3 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month3).Select(s => s.ProdOrderNo).ToList());
                f.ProdOrderNo4 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month4).Select(s => s.ProdOrderNo).ToList());
                f.ProdOrderNo5 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month5).Select(s => s.ProdOrderNo).ToList());
                f.ProdOrderNo6 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month6).Select(s => s.ProdOrderNo).ToList());
                f.ProdOrderNo7 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month7).Select(s => s.ProdOrderNo).ToList());
                f.ProdOrderNo8 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month8).Select(s => s.ProdOrderNo).ToList());
                f.ProdOrderNo9 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month9).Select(s => s.ProdOrderNo).ToList());
                f.ProdOrderNo10 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month10).Select(s => s.ProdOrderNo).ToList());
                f.ProdOrderNo11 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month11).Select(s => s.ProdOrderNo).ToList());
                f.ProdOrderNo12 = string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month12).Select(s => s.ProdOrderNo).ToList());

                f.ProTicket1 = string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month1).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month1).Select(s => s.ProdOrderNo).ToList()));
                f.ProTicket2 = string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month2).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month2).Select(s => s.ProdOrderNo).ToList()));
                f.ProTicket3 = string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month3).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month3).Select(s => s.ProdOrderNo).ToList()));
                f.ProTicket4 = string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month4).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month4).Select(s => s.ProdOrderNo).ToList()));
                f.ProTicket5 = string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month5).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month5).Select(s => s.ProdOrderNo).ToList()));
                f.ProTicket6 = string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month6).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month6).Select(s => s.ProdOrderNo).ToList()));
                f.ProTicket7 = string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month7).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month7).Select(s => s.ProdOrderNo).ToList()));
                f.ProTicket8 = string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month8).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month8).Select(s => s.ProdOrderNo).ToList()));
                f.ProTicket9 = string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month9).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month9).Select(s => s.ProdOrderNo).ToList()));
                f.ProTicket10 = string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month10).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month10).Select(s => s.ProdOrderNo).ToList()));
                f.ProTicket11 = string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month11).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month11).Select(s => s.ProdOrderNo).ToList()));
                f.ProTicket12 = string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month12).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month12).Select(s => s.ProdOrderNo).ToList()));


                f.Ticket1 = f.Ticket1 + f.ProTicket1;
                f.Ticket2 = f.Ticket2 + f.ProTicket2;
                f.Ticket3 = f.Ticket3 + f.ProTicket3;
                f.Ticket4 = f.Ticket4 + f.ProTicket4;
                f.Ticket5 = f.Ticket5 + f.ProTicket5;
                f.Ticket6 = f.Ticket6 + f.ProTicket6;
                f.Ticket7 = f.Ticket7 + f.ProTicket7;
                f.Ticket8 = f.Ticket8 + f.ProTicket8;
                f.Ticket9 = f.Ticket9 + f.ProTicket9;
                f.Ticket10 = f.Ticket10 + f.ProTicket10;
                f.Ticket11 = f.Ticket11 + f.ProTicket11;
                f.Ticket12 = f.Ticket12 + f.ProTicket12;

                /* f.Ticket1 = f.Ticket1 + string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month1).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month1).Select(s => s.ProdOrderNo).ToList()));
                 f.Ticket2 = f.Ticket2 + string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month2).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month2).Select(s => s.ProdOrderNo).ToList()));
                 f.Ticket3 = f.Ticket3 + string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month3).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month3).Select(s => s.ProdOrderNo).ToList()));
                 f.Ticket4 = f.Ticket4 + string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month4).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month4).Select(s => s.ProdOrderNo).ToList()));
                 f.Ticket5 = f.Ticket5 + string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month5).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month5).Select(s => s.ProdOrderNo).ToList()));
                 f.Ticket6 = f.Ticket6 + string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month6).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month6).Select(s => s.ProdOrderNo).ToList()));
                 f.Ticket7 = f.Ticket7 + string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month7).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month7).Select(s => s.ProdOrderNo).ToList()));
                 f.Ticket8 = f.Ticket8 + string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month8).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month8).Select(s => s.ProdOrderNo).ToList()));
                 f.Ticket9 = f.Ticket9 + string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month9).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month9).Select(s => s.ProdOrderNo).ToList()));
                 f.Ticket10 = f.Ticket10 + string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month10).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month10).Select(s => s.ProdOrderNo).ToList()));
                 f.Ticket11 = f.Ticket11 + string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month11).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month11).Select(s => s.ProdOrderNo).ToList()));
                 f.Ticket12 = f.Ticket12 + string.Format("{0}/{1}", string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month12).Select(s => s.Quantity).ToList()), string.Join(",", prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month12).Select(s => s.ProdOrderNo).ToList()));
 */

                f.ProjectedHoldingStock1 += f.AcSum > 0 ? grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month1 && t.StartDate.Value.Year == year1).Sum(s => s.TotalQuantity.Value) / (f.AcSum) : grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month1 && t.StartDate.Value.Year == year1).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStock2 += f.AcSum > 0 ? grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month2 && t.StartDate.Value.Year == year2).Sum(s => s.TotalQuantity.Value) / (f.AcSum) : grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month2 && t.StartDate.Value.Year == year2).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStock3 += f.AcSum > 0 ? grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month3 && t.StartDate.Value.Year == year3).Sum(s => s.TotalQuantity.Value) / (f.AcSum) : grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month3 && t.StartDate.Value.Year == year3).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStock4 += f.AcSum > 0 ? grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month4 && t.StartDate.Value.Year == year4).Sum(s => s.TotalQuantity.Value) / (f.AcSum) : grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month4 && t.StartDate.Value.Year == year4).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStock5 += f.AcSum > 0 ? grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month5 && t.StartDate.Value.Year == year5).Sum(s => s.TotalQuantity.Value) / (f.AcSum) : grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month5 && t.StartDate.Value.Year == year5).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStock6 += f.AcSum > 0 ? grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month6 && t.StartDate.Value.Year == year6).Sum(s => s.TotalQuantity.Value) / (f.AcSum) : grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month6 && t.StartDate.Value.Year == year6).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStock7 += f.AcSum > 0 ? grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month7 && t.StartDate.Value.Year == year7).Sum(s => s.TotalQuantity.Value) / (f.AcSum) : grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month7 && t.StartDate.Value.Year == year7).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStock8 += f.AcSum > 0 ? grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month8 && t.StartDate.Value.Year == year8).Sum(s => s.TotalQuantity.Value) / (f.AcSum) : grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month8 && t.StartDate.Value.Year == year8).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStock9 += f.AcSum > 0 ? grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month9 && t.StartDate.Value.Year == year9).Sum(s => s.TotalQuantity.Value) / (f.AcSum) : grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month9 && t.StartDate.Value.Year == year9).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStock10 += f.AcSum > 0 ? grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month10 && t.StartDate.Value.Year == year10).Sum(s => s.TotalQuantity.Value) / (f.AcSum) : grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month10 && t.StartDate.Value.Year == year10).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStock11 += f.AcSum > 0 ? grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month11 && t.StartDate.Value.Year == year11).Sum(s => s.TotalQuantity.Value) / (f.AcSum) : grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month11 && t.StartDate.Value.Year == year11).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStock12 += f.AcSum > 0 ? grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month12 && t.StartDate.Value.Year == year12).Sum(s => s.TotalQuantity.Value) / (f.AcSum) : grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month12 && t.StartDate.Value.Year == year12).Sum(s => s.TotalQuantity.Value);




                f.ProjectedHoldingStockQty1 = prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month1).Sum(s => s.Quantity);
                f.ProjectedHoldingStockQty2 = prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month2).Sum(s => s.Quantity);
                f.ProjectedHoldingStockQty3 = prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month3).Sum(s => s.Quantity);
                f.ProjectedHoldingStockQty4 = prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month4).Sum(s => s.Quantity);
                f.ProjectedHoldingStockQty5 = prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month5).Sum(s => s.Quantity);
                f.ProjectedHoldingStockQty6 = prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month6).Sum(s => s.Quantity);
                f.ProjectedHoldingStockQty7 = prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month7).Sum(s => s.Quantity);
                f.ProjectedHoldingStockQty8 = prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month8).Sum(s => s.Quantity);
                f.ProjectedHoldingStockQty9 = prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month9).Sum(s => s.Quantity);
                f.ProjectedHoldingStockQty10 = prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month10).Sum(s => s.Quantity);
                f.ProjectedHoldingStockQty11 = prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month11).Sum(s => s.Quantity);
                f.ProjectedHoldingStockQty12 = prodyctionoutTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == false && t.ProcessDate.Value.Month == month12).Sum(s => s.Quantity);

                f.ProjectedHoldingStockQty1 += grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month1 && t.StartDate.Value.Year == year1).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStockQty2 += grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month2 && t.StartDate.Value.Year == year2).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStockQty3 += grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month3 && t.StartDate.Value.Year == year3).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStockQty4 += grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month4 && t.StartDate.Value.Year == year4).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStockQty5 += grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month5 && t.StartDate.Value.Year == year5).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStockQty6 += grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month6 && t.StartDate.Value.Year == year6).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStockQty7 += grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month7 && t.StartDate.Value.Year == year7).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStockQty8 += grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month8 && t.StartDate.Value.Year == year8).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStockQty9 += grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month9 && t.StartDate.Value.Year == year9).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStockQty10 += grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month10 && t.StartDate.Value.Year == year10).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStockQty11 += grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month11 && t.StartDate.Value.Year == year11).Sum(s => s.TotalQuantity.Value);
                f.ProjectedHoldingStockQty12 += grouptickets.Where(t => item_Nos.Contains(t.ItemNo) && t.StartDate.Value.Month == month12 && t.StartDate.Value.Year == year12).Sum(s => s.TotalQuantity.Value);

                f.OutputProjectedHoldingStockQty1 = prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month1).Sum(s => s.Quantity);
                f.OutputProjectedHoldingStockQty2 = prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month2).Sum(s => s.Quantity);
                f.OutputProjectedHoldingStockQty3 = prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month3).Sum(s => s.Quantity);
                f.OutputProjectedHoldingStockQty4 = prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month4).Sum(s => s.Quantity);
                f.OutputProjectedHoldingStockQty5 = prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month5).Sum(s => s.Quantity);
                f.OutputProjectedHoldingStockQty6 = prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month6).Sum(s => s.Quantity);
                f.OutputProjectedHoldingStockQty7 = prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month7).Sum(s => s.Quantity);
                f.OutputProjectedHoldingStockQty8 = prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month8).Sum(s => s.Quantity);
                f.OutputProjectedHoldingStockQty9 = prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month9).Sum(s => s.Quantity);
                f.OutputProjectedHoldingStockQty10 = prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month10).Sum(s => s.Quantity);
                f.OutputProjectedHoldingStockQty11 = prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month11).Sum(s => s.Quantity);
                f.OutputProjectedHoldingStockQty12 = prodyctionTickets.Where(t => itemNos.Contains(t.ItemId) && t.IsOutput == true && t.StartingDate.Month == month12).Sum(s => s.Quantity);



                f.TicketHoldingStock1 = f.ProductionTicket1 > 0 ? f.AcSum == 0 ? (f.ProductionTicket1) / 1 : (f.ProductionTicket1) / f.AcSum : 0;
                f.TicketHoldingStock2 = f.ProductionTicket2 > 0 ? f.AcSum == 0 ? (f.ProductionTicket2) / 1 : (f.ProductionTicket2) / f.AcSum : 0;
                f.TicketHoldingStock3 = f.ProductionTicket3 > 0 ? f.AcSum == 0 ? (f.ProductionTicket3) / 1 : (f.ProductionTicket3) / f.AcSum : 0;
                f.TicketHoldingStock4 = f.ProductionTicket4 > 0 ? f.AcSum == 0 ? (f.ProductionTicket4) / 1 : (f.ProductionTicket4) / f.AcSum : 0;
                f.TicketHoldingStock5 = f.ProductionTicket5 > 0 ? f.AcSum == 0 ? (f.ProductionTicket5) / 1 : (f.ProductionTicket5) / f.AcSum : 0;
                f.TicketHoldingStock6 = f.ProductionTicket6 > 0 ? f.AcSum == 0 ? (f.ProductionTicket6) / 1 : (f.ProductionTicket6) / f.AcSum : 0;
                f.TicketHoldingStock7 = f.ProductionTicket7 > 0 ? f.AcSum == 0 ? (f.ProductionTicket7) / 1 : (f.ProductionTicket7) / f.AcSum : 0;
                f.TicketHoldingStock8 = f.ProductionTicket8 > 0 ? f.AcSum == 0 ? (f.ProductionTicket8) / 1 : (f.ProductionTicket8) / f.AcSum : 0;
                f.TicketHoldingStock9 = f.ProductionTicket9 > 0 ? f.AcSum == 0 ? (f.ProductionTicket9) / 1 : (f.ProductionTicket9) / f.AcSum : 0;
                f.TicketHoldingStock10 = f.ProductionTicket10 > 0 ? f.AcSum == 0 ? (f.ProductionTicket10) / 1 : (f.ProductionTicket10) / f.AcSum : 0;
                f.TicketHoldingStock11 = f.ProductionTicket11 > 0 ? f.AcSum == 0 ? (f.ProductionTicket11) / 1 : (f.ProductionTicket11) / f.AcSum : 0;
                f.TicketHoldingStock12 = f.ProductionTicket12 > 0 ? f.AcSum == 0 ? (f.ProductionTicket12) / 1 : (f.ProductionTicket12) / f.AcSum : 0;



                //if (f.isTenderExist)
                //{
                //    f.StockHoldingBalance = 0;
                //}

                //f.QtyMonth1 = (f.StockBalance - f.AcSum) > 0 ? f.StockBalance - f.AcSum : 0;
                //f.QtyProductionProjected1 = (f.QtyMonth1 + f.ProjectedHoldingStockQty1 + f.OutputProjectedHoldingStockQty1) - f.BlanketAddhoc1;

                //f.QtyMonth2 = (f.QtyProductionProjected1 - f.AcSum) > 0 ? f.QtyProductionProjected1 - f.AcSum : 0;
                //f.QtyProductionProjected2 = (f.QtyMonth2 + f.ProjectedHoldingStockQty2 + f.OutputProjectedHoldingStockQty2) - f.BlanketAddhoc2;

                //f.QtyMonth3 = (f.QtyProductionProjected2 - f.AcSum) > 0 ? f.QtyProductionProjected2 - f.AcSum : 0;
                //f.QtyProductionProjected3 = (f.QtyMonth3 + f.ProjectedHoldingStockQty3 + f.OutputProjectedHoldingStockQty3) - f.BlanketAddhoc3;

                //f.QtyMonth4 = (f.QtyProductionProjected3 - f.AcSum) > 0 ? f.QtyProductionProjected3 - f.AcSum : 0;
                //f.QtyProductionProjected4 = (f.QtyMonth4 + f.ProjectedHoldingStockQty4 + f.OutputProjectedHoldingStockQty4) - f.BlanketAddhoc4;

                //f.QtyMonth5 = (f.QtyProductionProjected4 - f.AcSum) > 0 ? f.QtyProductionProjected4 - f.AcSum : 0;
                //f.QtyProductionProjected5 = (f.QtyMonth5 + f.ProjectedHoldingStockQty5 + f.OutputProjectedHoldingStockQty5) - f.BlanketAddhoc5;

                //f.QtyMonth6 = (f.QtyProductionProjected5 - f.AcSum) > 0 ? f.QtyProductionProjected5 - f.AcSum : 0;
                //f.QtyProductionProjected6 = (f.QtyMonth6 + f.ProjectedHoldingStockQty6 + f.OutputProjectedHoldingStockQty6) - f.BlanketAddhoc6;

                //f.QtyMonth7 = (f.QtyProductionProjected6 - f.AcSum) > 0 ? f.QtyProductionProjected6 - f.AcSum : 0;
                //f.QtyProductionProjected7 = (f.QtyMonth7 + f.ProjectedHoldingStockQty7 + f.OutputProjectedHoldingStockQty7) - f.BlanketAddhoc7;

                //f.QtyMonth8 = (f.QtyProductionProjected7 - f.AcSum) > 0 ? f.QtyProductionProjected7 - f.AcSum : 0;
                //f.QtyProductionProjected8 = (f.QtyMonth8 + f.ProjectedHoldingStockQty8 + f.OutputProjectedHoldingStockQty8) - f.BlanketAddhoc8;

                //f.QtyMonth9 = (f.QtyProductionProjected8 - f.AcSum) > 0 ? f.QtyProductionProjected8 - f.AcSum : 0;
                //f.QtyProductionProjected9 = (f.QtyMonth9 + f.ProjectedHoldingStockQty9 + f.OutputProjectedHoldingStockQty9) - f.BlanketAddhoc9;

                //f.QtyMonth10 = (f.QtyProductionProjected9 - f.AcSum) > 0 ? f.QtyProductionProjected9 - f.AcSum : 0;
                //f.QtyProductionProjected10 = (f.QtyMonth10 + f.ProjectedHoldingStockQty10 + f.OutputProjectedHoldingStockQty10) - f.BlanketAddhoc10;

                //f.QtyMonth11 = (f.QtyProductionProjected10 - f.AcSum) > 0 ? f.QtyProductionProjected10 - f.AcSum : 0;
                //f.QtyProductionProjected11 = (f.QtyMonth11 + f.ProjectedHoldingStockQty11 + f.OutputProjectedHoldingStockQty11) - f.BlanketAddhoc11;

                //f.QtyMonth12 = (f.QtyProductionProjected11 - f.AcSum) > 0 ? f.QtyProductionProjected11 - f.AcSum : 0;
                //f.QtyProductionProjected12 = (f.QtyMonth12 + f.ProjectedHoldingStockQty12 + f.OutputProjectedHoldingStockQty12) - f.BlanketAddhoc12;


                f.QtyMonth1 = f.StockBalance - f.AcSum;
                f.QtyProductionProjected1 = (f.QtyMonth1 + f.ProjectedHoldingStockQty1 + f.OutputProjectedHoldingStockQty1) - f.BlanketAddhoc1;

                f.QtyMonth2 = f.QtyProductionProjected1 - f.AcSum;
                f.QtyProductionProjected2 = (f.QtyMonth2 + f.ProjectedHoldingStockQty2 + f.OutputProjectedHoldingStockQty2) - f.BlanketAddhoc2;

                f.QtyMonth3 = f.QtyProductionProjected2 - f.AcSum;
                f.QtyProductionProjected3 = (f.QtyMonth3 + f.ProjectedHoldingStockQty3 + f.OutputProjectedHoldingStockQty3) - f.BlanketAddhoc3;

                f.QtyMonth4 = f.QtyProductionProjected3 - f.AcSum;
                f.QtyProductionProjected4 = (f.QtyMonth4 + f.ProjectedHoldingStockQty4 + f.OutputProjectedHoldingStockQty4) - f.BlanketAddhoc4;

                f.QtyMonth5 = f.QtyProductionProjected4 - f.AcSum;
                f.QtyProductionProjected5 = (f.QtyMonth5 + f.ProjectedHoldingStockQty5 + f.OutputProjectedHoldingStockQty5) - f.BlanketAddhoc5;

                f.QtyMonth6 = f.QtyProductionProjected5 - f.AcSum;
                f.QtyProductionProjected6 = (f.QtyMonth6 + f.ProjectedHoldingStockQty6 + f.OutputProjectedHoldingStockQty6) - f.BlanketAddhoc6;

                f.QtyMonth7 = f.QtyProductionProjected6 - f.AcSum;
                f.QtyProductionProjected7 = (f.QtyMonth7 + f.ProjectedHoldingStockQty7 + f.OutputProjectedHoldingStockQty7) - f.BlanketAddhoc7;

                f.QtyMonth8 = f.QtyProductionProjected7 - f.AcSum;
                f.QtyProductionProjected8 = (f.QtyMonth8 + f.ProjectedHoldingStockQty8 + f.OutputProjectedHoldingStockQty8) - f.BlanketAddhoc8;

                f.QtyMonth9 = f.QtyProductionProjected8 - f.AcSum;
                f.QtyProductionProjected9 = (f.QtyMonth9 + f.ProjectedHoldingStockQty9 + f.OutputProjectedHoldingStockQty9) - f.BlanketAddhoc9;

                f.QtyMonth10 = f.QtyProductionProjected9 - f.AcSum;
                f.QtyProductionProjected10 = (f.QtyMonth10 + f.ProjectedHoldingStockQty10 + f.OutputProjectedHoldingStockQty10) - f.BlanketAddhoc10;

                f.QtyMonth11 = f.QtyProductionProjected10 - f.AcSum;
                f.QtyProductionProjected11 = (f.QtyMonth11 + f.ProjectedHoldingStockQty11 + f.OutputProjectedHoldingStockQty11) - f.BlanketAddhoc11;

                f.QtyMonth12 = f.QtyProductionProjected11 - f.AcSum;
                f.QtyProductionProjected12 = (f.QtyMonth12 + f.ProjectedHoldingStockQty12 + f.OutputProjectedHoldingStockQty12) - f.BlanketAddhoc12;

                //f.ProductionProjected1 = f.Month1 + f.ProjectedHoldingStock1 + f.TicketHoldingStock1 + f.OutputProjectedHoldingStock1;
                //f.Month2 = f.AcSum > 0 ? f.ProductionProjected1 - 1 > 0 ? f.ProductionProjected1 - 1 : 0 : f.ProductionProjected1;
                //f.ProductionProjected2 = f.Month2 + f.ProjectedHoldingStock2 + f.TicketHoldingStock2 + f.OutputProjectedHoldingStock2;
                //f.Month3 = f.AcSum > 0 ? f.ProductionProjected2 - 1 > 0 ? f.ProductionProjected2 - 1 : 0 : f.ProductionProjected2;
                //f.ProductionProjected3 = f.Month3 + f.ProjectedHoldingStock3 + f.TicketHoldingStock3 + f.OutputProjectedHoldingStock3;
                //f.Month4 = f.AcSum > 0 ? f.ProductionProjected3 - 1 > 0 ? f.ProductionProjected3 - 1 : 0 : f.ProductionProjected3;
                //f.ProductionProjected4 = f.Month4 + f.ProjectedHoldingStock4 + f.TicketHoldingStock4 + f.OutputProjectedHoldingStock4;
                //f.Month5 = f.AcSum > 0 ? f.ProductionProjected4 - 1 > 0 ? f.ProductionProjected4 - 1 : 0 : f.ProductionProjected4;
                //f.ProductionProjected5 = f.Month5 + f.ProjectedHoldingStock5 + f.TicketHoldingStock5 + f.OutputProjectedHoldingStock5;
                //f.Month6 = f.AcSum > 0 ? f.ProductionProjected5 - 1 > 0 ? f.ProductionProjected5 - 1 : 0 : f.ProductionProjected5;
                //f.ProductionProjected6 = f.Month6 + f.ProjectedHoldingStock6 + f.TicketHoldingStock6 + f.OutputProjectedHoldingStock6;
                //f.Month7 = f.AcSum > 0 ? f.ProductionProjected6 - 1 > 0 ? f.ProductionProjected6 - 1 : 0 : f.ProductionProjected6;
                //f.ProductionProjected7 = f.Month7 + f.ProjectedHoldingStock7 + f.TicketHoldingStock7 + f.OutputProjectedHoldingStock7;
                //f.Month8 = f.AcSum > 0 ? f.ProductionProjected7 - 1 > 0 ? f.ProductionProjected7 - 1 : 0 : f.ProductionProjected7;
                //f.ProductionProjected8 = f.Month8 + f.ProjectedHoldingStock8 + f.TicketHoldingStock8 + f.OutputProjectedHoldingStock8;
                //f.Month9 = f.AcSum > 0 ? f.ProductionProjected8 - 1 > 0 ? f.ProductionProjected8 - 1 : 0 : f.ProductionProjected8;
                //f.ProductionProjected9 = f.Month9 + f.ProjectedHoldingStock9 + f.TicketHoldingStock9 + f.OutputProjectedHoldingStock9;
                //f.Month10 = f.AcSum > 0 ? f.ProductionProjected9 - 1 > 0 ? f.ProductionProjected9 - 1 : 0 : f.ProductionProjected9;
                //f.ProductionProjected10 = f.Month10 + f.ProjectedHoldingStock10 + f.TicketHoldingStock10 + f.OutputProjectedHoldingStock10;
                //f.Month11 = f.AcSum > 0 ? f.ProductionProjected10 - 1 > 0 ? f.ProductionProjected10 - 1 : 0 : f.ProductionProjected10;
                //f.ProductionProjected11 = f.Month11 + f.ProjectedHoldingStock11 + f.TicketHoldingStock11 + f.OutputProjectedHoldingStock11;
                //f.Month12 = f.AcSum > 0 ? f.ProductionProjected11 - 1 > 0 ? f.ProductionProjected11 - 1 : 0 : f.ProductionProjected11;
                //f.ProductionProjected12 = f.Month12 + f.ProjectedHoldingStock12 + f.TicketHoldingStock12 + f.OutputProjectedHoldingStock12;

                f.ProductionProjected1 = f.Month1 + f.ProjectedHoldingStock1 + 0 + f.OutputProjectedHoldingStock1;
                f.Month2 = f.AcSum > 0 ? f.ProductionProjected1 - 1 > 0 ? f.ProductionProjected1 - 1 : 0 : f.ProductionProjected1;
                f.ProductionProjected2 = f.Month2 + f.ProjectedHoldingStock2 + 0 + f.OutputProjectedHoldingStock2;
                f.Month3 = f.AcSum > 0 ? f.ProductionProjected2 - 1 > 0 ? f.ProductionProjected2 - 1 : 0 : f.ProductionProjected2;
                f.ProductionProjected3 = f.Month3 + f.ProjectedHoldingStock3 + 0 + f.OutputProjectedHoldingStock3;
                f.Month4 = f.AcSum > 0 ? f.ProductionProjected3 - 1 > 0 ? f.ProductionProjected3 - 1 : 0 : f.ProductionProjected3;
                f.ProductionProjected4 = f.Month4 + f.ProjectedHoldingStock4 + 0 + f.OutputProjectedHoldingStock4;
                f.Month5 = f.AcSum > 0 ? f.ProductionProjected4 - 1 > 0 ? f.ProductionProjected4 - 1 : 0 : f.ProductionProjected4;
                f.ProductionProjected5 = f.Month5 + f.ProjectedHoldingStock5 + 0 + f.OutputProjectedHoldingStock5;
                f.Month6 = f.AcSum > 0 ? f.ProductionProjected5 - 1 > 0 ? f.ProductionProjected5 - 1 : 0 : f.ProductionProjected5;
                f.ProductionProjected6 = f.Month6 + f.ProjectedHoldingStock6 + 0 + f.OutputProjectedHoldingStock6;
                f.Month7 = f.AcSum > 0 ? f.ProductionProjected6 - 1 > 0 ? f.ProductionProjected6 - 1 : 0 : f.ProductionProjected6;
                f.ProductionProjected7 = f.Month7 + f.ProjectedHoldingStock7 + 0 + f.OutputProjectedHoldingStock7;
                f.Month8 = f.AcSum > 0 ? f.ProductionProjected7 - 1 > 0 ? f.ProductionProjected7 - 1 : 0 : f.ProductionProjected7;
                f.ProductionProjected8 = f.Month8 + f.ProjectedHoldingStock8 + 0 + f.OutputProjectedHoldingStock8;
                f.Month9 = f.AcSum > 0 ? f.ProductionProjected8 - 1 > 0 ? f.ProductionProjected8 - 1 : 0 : f.ProductionProjected8;
                f.ProductionProjected9 = f.Month9 + f.ProjectedHoldingStock9 + 0 + f.OutputProjectedHoldingStock9;
                f.Month10 = f.AcSum > 0 ? f.ProductionProjected9 - 1 > 0 ? f.ProductionProjected9 - 1 : 0 : f.ProductionProjected9;
                f.ProductionProjected10 = f.Month10 + f.ProjectedHoldingStock10 + 0 + f.OutputProjectedHoldingStock10;
                f.Month11 = f.AcSum > 0 ? f.ProductionProjected10 - 1 > 0 ? f.ProductionProjected10 - 1 : 0 : f.ProductionProjected10;
                f.ProductionProjected11 = f.Month11 + f.ProjectedHoldingStock11 + 0 + f.OutputProjectedHoldingStock11;
                f.Month12 = f.AcSum > 0 ? f.ProductionProjected11 - 1 > 0 ? f.ProductionProjected11 - 1 : 0 : f.ProductionProjected11;
                f.ProductionProjected12 = f.Month12 + f.ProjectedHoldingStock12 + 0 + f.OutputProjectedHoldingStock12;




                if (f.IsTenderExist1 || f.AcSum <= 0)
                {
                    f.Month1 = f.QtyMonth1;
                    f.ProductionProjected1 = f.QtyProductionProjected1;
                    f.ProjectedHoldingStock1 = f.ProjectedHoldingStockQty1;
                    f.OutputProjectedHoldingStock1 = f.OutputProjectedHoldingStockQty1;
                }
                else
                {
                    if (f.isTenderExist)
                    {
                        f.Month1 = f.QtyMonth1 / f.AcSum;
                        f.ProductionProjected1 = f.QtyProductionProjected1 / f.AcSum;
                    }
                }
                if (f.IsTenderExist2 || f.AcSum <= 0)
                {
                    f.Month2 = f.QtyMonth2;
                    f.ProductionProjected2 = f.QtyProductionProjected2;
                    f.ProjectedHoldingStock2 = f.ProjectedHoldingStockQty2;
                    f.OutputProjectedHoldingStock2 = f.OutputProjectedHoldingStockQty2;
                }
                else
                {
                    if (f.isTenderExist)
                    {
                        f.Month2 = f.QtyMonth2 / f.AcSum;
                        f.ProductionProjected2 = f.QtyProductionProjected2 / f.AcSum;
                    }
                }
                if (f.IsTenderExist3 || f.AcSum <= 0)
                {
                    f.Month3 = f.QtyMonth3;
                    f.ProductionProjected3 = f.QtyProductionProjected3;
                    f.ProjectedHoldingStock3 = f.ProjectedHoldingStockQty3;
                    f.OutputProjectedHoldingStock3 = f.OutputProjectedHoldingStockQty3;
                }
                else
                {
                    if (f.isTenderExist)
                    {
                        f.Month3 = f.QtyMonth3 / f.AcSum;
                        f.ProductionProjected3 = f.QtyProductionProjected3 / f.AcSum;
                    }
                }
                if (f.IsTenderExist4 || f.AcSum <= 0)
                {
                    f.Month4 = f.QtyMonth4;
                    f.ProductionProjected4 = f.QtyProductionProjected4;
                    f.ProjectedHoldingStock4 = f.ProjectedHoldingStockQty4;
                    f.OutputProjectedHoldingStock4 = f.OutputProjectedHoldingStockQty4;
                }
                else
                {
                    if (f.isTenderExist)
                    {
                        f.Month4 = f.QtyMonth4 / f.AcSum;
                        f.ProductionProjected4 = f.QtyProductionProjected4 / f.AcSum;
                    }
                }
                if (f.IsTenderExist5 || f.AcSum <= 0)
                {
                    f.Month5 = f.QtyMonth5;
                    f.ProductionProjected5 = f.QtyProductionProjected5;
                    f.ProjectedHoldingStock5 = f.ProjectedHoldingStockQty5;
                    f.OutputProjectedHoldingStock5 = f.OutputProjectedHoldingStockQty5;
                }
                else
                {
                    if (f.isTenderExist)
                    {
                        f.Month5 = f.QtyMonth5 / f.AcSum;
                        f.ProductionProjected5 = f.QtyProductionProjected5 / f.AcSum;
                    }
                }
                if (f.IsTenderExist6 || f.AcSum <= 0)
                {
                    f.Month6 = f.QtyMonth6;
                    f.ProductionProjected6 = f.QtyProductionProjected6;
                    f.ProjectedHoldingStock6 = f.ProjectedHoldingStockQty6;
                    f.OutputProjectedHoldingStock6 = f.OutputProjectedHoldingStockQty6;
                }
                else
                {
                    if (f.isTenderExist)
                    {
                        f.Month6 = f.QtyMonth6 / f.AcSum;
                        f.ProductionProjected6 = f.QtyProductionProjected6 / f.AcSum;
                    }
                }
                if (f.IsTenderExist7 || f.AcSum <= 0)
                {
                    f.Month7 = f.QtyMonth7;
                    f.ProductionProjected7 = f.QtyProductionProjected7;
                    f.ProjectedHoldingStock7 = f.ProjectedHoldingStockQty7;
                    f.OutputProjectedHoldingStock7 = f.OutputProjectedHoldingStockQty7;
                }
                else
                {
                    if (f.isTenderExist)
                    {
                        f.Month7 = f.QtyMonth7 / f.AcSum;
                        f.ProductionProjected7 = f.QtyProductionProjected7 / f.AcSum;
                    }
                }
                if (f.IsTenderExist8 || f.AcSum <= 0)
                {
                    f.Month8 = f.QtyMonth8;
                    f.ProductionProjected8 = f.QtyProductionProjected8;
                    f.ProjectedHoldingStock8 = f.ProjectedHoldingStockQty8;
                    f.OutputProjectedHoldingStock8 = f.OutputProjectedHoldingStockQty8;
                }
                else
                {
                    if (f.isTenderExist)
                    {
                        f.Month8 = f.QtyMonth8 / f.AcSum;
                        f.ProductionProjected8 = f.QtyProductionProjected8 / f.AcSum;
                    }
                }
                if (f.IsTenderExist9 || f.AcSum <= 0)
                {
                    f.Month9 = f.QtyMonth9;
                    f.ProductionProjected9 = f.QtyProductionProjected9;
                    f.ProjectedHoldingStock9 = f.ProjectedHoldingStockQty9;
                    f.OutputProjectedHoldingStock9 = f.OutputProjectedHoldingStockQty9;
                }
                else
                {
                    if (f.isTenderExist)
                    {
                        f.Month9 = f.QtyMonth9 / f.AcSum;
                        f.ProductionProjected9 = f.QtyProductionProjected9 / f.AcSum;
                    }
                }
                if (f.IsTenderExist10 || f.AcSum <= 0)
                {
                    f.Month10 = f.QtyMonth10;
                    f.ProductionProjected10 = f.QtyProductionProjected10;
                    f.ProjectedHoldingStock10 = f.ProjectedHoldingStockQty10;
                    f.OutputProjectedHoldingStock10 = f.OutputProjectedHoldingStockQty10;
                }
                else
                {
                    if (f.isTenderExist)
                    {
                        f.Month10 = f.QtyMonth10 / f.AcSum;
                        f.ProductionProjected10 = f.QtyProductionProjected10 / f.AcSum;
                    }
                }
                if (f.IsTenderExist11 || f.AcSum <= 0)
                {
                    f.Month11 = f.QtyMonth11;
                    f.ProductionProjected11 = f.QtyProductionProjected11;
                    f.ProjectedHoldingStock11 = f.ProjectedHoldingStockQty11;
                }
                else
                {
                    if (f.isTenderExist)
                    {
                        f.Month11 = f.QtyMonth11 / f.AcSum;
                        f.ProductionProjected11 = f.QtyProductionProjected11 / f.AcSum;
                    }
                }
                if (f.IsTenderExist12 || f.AcSum <= 0)
                {
                    f.Month12 = f.QtyMonth12;
                    f.ProductionProjected12 = f.QtyProductionProjected12;
                    f.ProjectedHoldingStock12 = f.ProjectedHoldingStockQty12;
                    f.OutputProjectedHoldingStock12 = f.OutputProjectedHoldingStockQty12;
                }
                else
                {
                    if (f.isTenderExist)
                    {
                        f.Month12 = f.QtyMonth12 / f.AcSum;
                        f.ProductionProjected12 = f.QtyProductionProjected12 / f.AcSum;
                    }
                }

                //f.Month1 = f.Month1 < 0 ? 0 : f.Month1;
                //f.Month2 = f.Month2 < 0 ? 0 : f.Month2;
                //f.Month3 = f.Month3 < 0 ? 0 : f.Month3;
                //f.Month4 = f.Month4 < 0 ? 0 : f.Month4;
                //f.Month5 = f.Month5 < 0 ? 0 : f.Month5;
                //f.Month6 = f.Month6 < 0 ? 0 : f.Month6;
                //f.Month7 = f.Month7 < 0 ? 0 : f.Month7;
                //f.Month8 = f.Month8 < 0 ? 0 : f.Month8;
                //f.Month9 = f.Month9 < 0 ? 0 : f.Month9;
                //f.Month10 = f.Month10 < 0 ? 0 : f.Month10;
                //f.Month11 = f.Month11 < 0 ? 0 : f.Month11;
                //f.Month12 = f.Month12 < 0 ? 0 : f.Month12;

                //f.ProductionProjected1 = f.ProductionProjected1 < 0 ? 0 : f.ProductionProjected1;
                //f.ProductionProjected2 = f.ProductionProjected2 < 0 ? 0 : f.ProductionProjected2;
                //f.ProductionProjected3 = f.ProductionProjected3 < 0 ? 0 : f.ProductionProjected3;
                //f.ProductionProjected4 = f.ProductionProjected4 < 0 ? 0 : f.ProductionProjected4;
                //f.ProductionProjected5 = f.ProductionProjected5 < 0 ? 0 : f.ProductionProjected5;
                //f.ProductionProjected6 = f.ProductionProjected6 < 0 ? 0 : f.ProductionProjected6;
                //f.ProductionProjected7 = f.ProductionProjected7 < 0 ? 0 : f.ProductionProjected7;
                //f.ProductionProjected8 = f.ProductionProjected8 < 0 ? 0 : f.ProductionProjected8;
                //f.ProductionProjected9 = f.ProductionProjected9 < 0 ? 0 : f.ProductionProjected9;
                //f.ProductionProjected10 = f.ProductionProjected10 < 0 ? 0 : f.ProductionProjected10;
                //f.ProductionProjected11 = f.ProductionProjected11 < 0 ? 0 : f.ProductionProjected11;
                //f.ProductionProjected12 = f.ProductionProjected12 < 0 ? 0 : f.ProductionProjected12;

            });

            genericId = new List<long?>();
            //var customer = new List<string>();
            itemMasterforReport.ForEach(ac =>
            {
                var customer = new List<string>();

                var symlQty = itemdict.FirstOrDefault(f => f.Key == ac.No).Value;

                if (!genericId.Exists(g => g == ac.GenericCodeId) && ac.GenericCodeId != null)
                {
                    genericId.Add(ac.GenericCodeId);
                    var itemNos = itemMasterforReport.Where(i => i.GenericCodeId == ac.GenericCodeId).Select(s => s.ItemId).ToList();
                    // var acQty = MethodCodeList.Where(f => f.ItemId == ac.ItemId).Sum(s=>s.AcSum);
                    blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1))).ToList().ForEach(adhoc =>
                    {

                        if (!customer.Exists(c => c == adhoc.Categories) && !string.IsNullOrEmpty(adhoc.Categories))
                        {
                            customer.Add(adhoc.Categories);
                            MethodCodeList.Add(new INPCalendarPivotModel
                            {
                                ItemId = ac.ItemId,
                                ItemNo = ac.GenericCodeDescription2,
                                IsSteroid = ac.Steroid.GetValueOrDefault(false),
                                SalesCategoryId = ac.CategoryId,
                                SalesCategory = salesCatLists.Where(w => w.Id == ac.CategoryId).FirstOrDefault()?.Text,
                                LocationID = categoryItems != null && categoryItems.FirstOrDefault(a => a.SaleCategoryId == ac.CategoryId) != null ? categoryItems.FirstOrDefault(a => a.SaleCategoryId == ac.CategoryId).LocationId : null,
                                // SalesCategory = categoryItems != null && categoryItems.FirstOrDefault(a => a.SaleCategoryId == ac.CategoryId) != null ? categoryItems.FirstOrDefault(a => a.SaleCategoryId == ac.CategoryId).Code : "",
                                MethodCodeId = ac.NavMethodCodeLines.Count > 0 ? ac.NavMethodCodeLines.First().MethodCodeId : -1,
                                GenericCodeID = ac.GenericCodeId,
                                AddhocCust = adhoc.Categories,
                                Description = ac.Description + " " + ac.Description2,
                                MethodCode = ac.NavMethodCodeLines.Count > 0 ? ac.NavMethodCodeLines.First().MethodName : "No MethodCode",
                                Month = endDate.StockMonth.ToString("MMMM"),
                                Remarks = "Tender",
                                PackSize = ac.PackSize.HasValue ? ac.PackSize.Value : 900,
                                PackSize2 = packSize2,
                                BlanketAddhoc1 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.Categories == adhoc.Categories && t.ShipmentDate.Value.Month == month1 && t.ShipmentDate.Value.Year == year1).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                                BlanketAddhoc2 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.Categories == adhoc.Categories && t.ShipmentDate.Value.Month == month2 && t.ShipmentDate.Value.Year == year2).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                                BlanketAddhoc3 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.Categories == adhoc.Categories && t.ShipmentDate.Value.Month == month3 && t.ShipmentDate.Value.Year == year3).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                                BlanketAddhoc4 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.Categories == adhoc.Categories && t.ShipmentDate.Value.Month == month4 && t.ShipmentDate.Value.Year == year4).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                                BlanketAddhoc5 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.Categories == adhoc.Categories && t.ShipmentDate.Value.Month == month5 && t.ShipmentDate.Value.Year == year5).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                                BlanketAddhoc6 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.Categories == adhoc.Categories && t.ShipmentDate.Value.Month == month6 && t.ShipmentDate.Value.Year == year6).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                                BlanketAddhoc7 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.Categories == adhoc.Categories && t.ShipmentDate.Value.Month == month7 && t.ShipmentDate.Value.Year == year7).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                                BlanketAddhoc8 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.Categories == adhoc.Categories && t.ShipmentDate.Value.Month == month8 && t.ShipmentDate.Value.Year == year8).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                                BlanketAddhoc9 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.Categories == adhoc.Categories && t.ShipmentDate.Value.Month == month9 && t.ShipmentDate.Value.Year == year9).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                                BlanketAddhoc10 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.Categories == adhoc.Categories && t.ShipmentDate.Value.Month == month10 && t.ShipmentDate.Value.Year == year10).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                                BlanketAddhoc11 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.Categories == adhoc.Categories && t.ShipmentDate.Value.Month == month11 && t.ShipmentDate.Value.Year == year11).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                                BlanketAddhoc12 = blanletOrders.Where(t => itemNos.Contains(t.ItemId.GetValueOrDefault(-1)) && t.Categories == adhoc.Categories && t.ShipmentDate.Value.Month == month12 && t.ShipmentDate.Value.Year == year12).Sum(s => s.OutstandingQty.GetValueOrDefault(0)),
                                isTenderExist = true,
                                SymlQty = symlQty,
                                Itemgrouping = symlQty > 0 ? "Adhoc orders - Item with AC" : "Stand alone"
                            });
                        }
                    });
                }

            });

            var groupedResult = MethodCodeList.GroupBy(g => g.MethodCode).ToList();
            var resultData = new List<INPCalendarPivotModel>();
            groupedResult.ForEach(f =>
            {
                if (f.Key.Contains("SW Vitamin C Tablet"))
                {

                }
                f.ToList().ForEach(g =>
                {
                    if (g.SymlQty > 0 && g.Remarks == "AC")
                    {
                        resultData.Add(g);
                    }
                    else
                    {
                        if (g.Remarks == "AC")
                        {
                            var tenderExist = f.Any(t => t.Remarks == "Tender" && t.ItemNo == g.ItemNo && (t.BlanketAddhoc1 > 0 || t.BlanketAddhoc2 > 0 || t.BlanketAddhoc3 > 0 || t.BlanketAddhoc4 > 0 || t.BlanketAddhoc5 > 0
                            || t.BlanketAddhoc6 > 0 || t.BlanketAddhoc7 > 0 || t.BlanketAddhoc8 > 0 || t.BlanketAddhoc9 > 0 || t.BlanketAddhoc10 > 0 || t.BlanketAddhoc11 > 0 || t.BlanketAddhoc12 > 0));
                            if (tenderExist)
                            {
                                resultData.Add(g);
                            }
                            else
                            {
                                if (g.ProjectedHoldingStock1 > 0 || g.ProjectedHoldingStock2 > 0 || g.ProjectedHoldingStock3 > 0 || g.ProjectedHoldingStock4 > 0 || g.ProjectedHoldingStock5 > 0
                      || g.ProjectedHoldingStock6 > 0 || g.ProjectedHoldingStock7 > 0 || g.ProjectedHoldingStock8 > 0 || g.ProjectedHoldingStock9 > 0 || g.ProjectedHoldingStock10 > 0 || g.ProjectedHoldingStock11 > 0 || g.ProjectedHoldingStock12 > 0)
                                {
                                    resultData.Add(g);
                                }
                            }
                        }
                        else
                        {
                            if (g.BlanketAddhoc1 > 0 || g.BlanketAddhoc2 > 0 || g.BlanketAddhoc3 > 0 || g.BlanketAddhoc4 > 0 || g.BlanketAddhoc5 > 0
                            || g.BlanketAddhoc6 > 0 || g.BlanketAddhoc7 > 0 || g.BlanketAddhoc8 > 0 || g.BlanketAddhoc9 > 0 || g.BlanketAddhoc10 > 0 || g.BlanketAddhoc11 > 0 || g.BlanketAddhoc12 > 0)
                            {
                                resultData.Add(g);
                            }
                        }
                    }
                });

            });

            return resultData.ToList();
        }
    }
}
