using DevExpress.Utils.Serializing.Helpers;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using DocumentViewer.Models;
using Microsoft.AspNetCore.Mvc;
namespace DocumentViewer.Controllers
{
    public class SchedulerDataController : Controller
    {
        private readonly AppDbContext _context;
        public SchedulerDataController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public object Get(DataSourceLoadOptions loadOptions)
        {
            var query = from oal in _context.ProductionPlanningScheduler
                        join oau in _context.PlanningForProductionProcessByMachineRelated on oal.PlanningForProductionProcessByMachineRelatedId equals oau.PlanningForProductionProcessByMachineRelatedId
                        join oau1 in _context.PlanningForProductionProcessByMachine on oau.PlanningForProductionProcessByMachineId equals oau1.PlanningForProductionProcessByMachineId
                        select new ProductionPlanningScheduler
                        {
                            ProductionPlanningSchedulerId = oal.ProductionPlanningSchedulerId,
                            PlanningForProductionProcessByMachineRelatedId = oal.PlanningForProductionProcessByMachineRelatedId,
                            ProductionPlanningProcess = oau1.ProductionPlanningProcessId,
                            PlanningForProductionProcessByMachine = oau.FixAssetMachineNameRequipmentId,
                            Description = oal.Description,
                            StartDate = oal.StartDate,
                            EndDate = oal.EndDate,
                            ReplanRefNo = oal.ReplanRefNo,
                            ProdOrderNo = oal.ProdOrderNo,
                            RecipeNo = oal.RecipeNo,
                            ProductionBOMNo = oal.ProductionBOMNo,
                            BatchNo = oal.BatchNo,
                            ItemNo = oal.ItemNo,
                            Description2 = oal.Description2,
                            InternalRef = oal.InternalRef,
                            LocationCode = oal.LocationCode,
                            Quantity = oal.Quantity,
                            MachineCenterName = oal.MachineCenterName,
                            Remarks = oal.Remarks,
                            SubStatus = oal.SubStatus,
                            OutputDate = oal.OutputDate,
                            NotToContinue = oal.NotToContinue,
                            CompleteOrder = oal.CompleteOrder,
                            UnitofMeasureCode = oal.UnitofMeasureCode,
                            RemainingQuantity = oal.RemainingQuantity,
                            FinishedQuantity = oal.FinishedQuantity,
                            Text = oal.ReplanRefNo + "/" + oal.BatchNo + "/" + oal.UnitofMeasureCode + "/" + oal.RecipeNo + "/" + oal.Description + "/" + oal.Description2
                        };
            var res = query.ToList();
            return DataSourceLoader.Load(res, loadOptions);
        }

        [HttpPost("InsertAppointment")]
        public IActionResult InsertAppointment(string values)
        {
            var newAppointment = new ProductionPlanningScheduler();
            JsonPopulateObjectExtensions.PopulateObject(values, newAppointment);
            _context.Add(newAppointment);
            _context.SaveChanges();

            return Ok();
        }

        [HttpPut("UpdateAppointment")]
        public IActionResult UpdateAppointment(int key, string values)
        {
            var newAppointment = new ProductionPlanningScheduler();
            var appointment = _context.ProductionPlanningScheduler.FirstOrDefault(a => a.ProductionPlanningSchedulerId == key);
            JsonPopulateObjectExtensions.PopulateObject(values, newAppointment);
            if (appointment != null)
            {
                appointment.StartDate = newAppointment.StartDate;
                appointment.EndDate = newAppointment.EndDate;
                appointment.Description = newAppointment.Description;
                appointment.Text = newAppointment.Text;
                appointment.ProductionPlanningProcess = newAppointment.ProductionPlanningProcess;
                appointment.PlanningForProductionProcessByMachine = newAppointment.PlanningForProductionProcessByMachine;
                appointment.Text = newAppointment.Text;
                _context.SaveChanges();
            }

            return Ok();
        }

        [HttpDelete("DeleteAppointment")]
        public IActionResult DeleteAppointment(int key)
        {
            var appointment = _context.ProductionPlanningScheduler.FirstOrDefault(a => a.ProductionPlanningSchedulerId == key);
            if (appointment != null)
            {
                _context.Remove(appointment);
                _context.SaveChanges();
            }
            return Ok();
        }
    }
}
