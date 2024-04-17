using Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SW.Portal.Solutions.Services;
using Core.EntityModels;
using SW.Portal.Solutions.Models;
using Core.Entities.Views;
using Core.Entities;
using Core.Repositories.Query;
using AC.SD.Core.Data;
using AC.SD.Core.Pages.Masters;
using Google.Api.Gax.ResourceNames;
using DevExpress.Web;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using Newtonsoft.Json;

namespace SW.Portal.Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductionRoutineController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IPlantQueryRepository _PlantQueryRepository;
        private readonly IProductionActivityAppQueryRepository _ProductionActivityAppQueryRepository;
        private readonly IRoutineQueryRepository _RoutineQueryRepository;
      
        public ProductionRoutineController(IMediator mediator, IPlantQueryRepository PlantQueryRepository, IProductionActivityAppQueryRepository productionActivityAppQueryRepository, IRoutineQueryRepository routineQueryRepository)
        {
            _mediator = mediator;
            _PlantQueryRepository = PlantQueryRepository;
            _ProductionActivityAppQueryRepository = productionActivityAppQueryRepository;
            _RoutineQueryRepository = routineQueryRepository;
            
        }
        [HttpGet("GetCompanyList")]
        public async Task<ActionResult<Services.ResponseModel<List<ViewPlants>>>> GetCompanyList()
        {
            var response = new Services.ResponseModel<ViewPlants>();
            var result = await _PlantQueryRepository.GetAllAsync();
            try
            {
                response.ResponseCode = Services.ResponseCode.Success;
                response.Results = (List<ViewPlants>)(result.Count > 0 ? result : new List<ViewPlants> { new ViewPlants() });
                
            }
            catch (Exception ex)
            {
                response.ResponseCode = Services.ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }

        [HttpGet("GetLocation")]
        public async Task<ActionResult<Services.ResponseModel<List<ProductionActivityAppModel>>>> GetLocation(long? CompanyID)
        {

            var response = new Services.ResponseModel<ProductionActivityAppModel>();

            var result = await _ProductionActivityAppQueryRepository.GetAllAsync(CompanyID);
            var displayResult = result?.Select(topic => new ProductionActivityAppModel
            {
               ProductionActivityAppID =topic.ProductionActivityAppID,
                CompanyID =topic.CompanyID,
                LocationID = topic.LocationID,
                ProdOrderNo = topic.ProdOrderNo,
                Comment = topic.Comment,
                ICTMasterID =topic.ICTMasterID,
                TopicID =topic.TopicID,
                DeropdownName = topic.DeropdownName,
            }).ToList();

            try
            {
                response.ResponseCode = Services.ResponseCode.Success;
                // response.Results = displayResult;
                response.Results = displayResult.Count > 0 ? displayResult : new List<ProductionActivityAppModel> { new ProductionActivityAppModel() };
            }
            catch (Exception ex)
            {
                response.ResponseCode = Services.ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }
        [HttpGet("GetRoutineList")]
        public async Task<ActionResult<Services.ResponseModel<List<ApplicationMasterChildModel>>>> GetRoutineList()
        {

            var response = new Services.ResponseModel<ApplicationMasterChildModel>();

            var result = await _mediator.Send(new GetAllApplicationMasterChildListQuery("108"));
            try
            {
                response.ResponseCode = Services. ResponseCode.Success;
                response.Results = result.Count > 0 ? result : new List<ApplicationMasterChildModel> { new ApplicationMasterChildModel() };
            }
            catch (Exception ex)
            {
                response.ResponseCode = Services. ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }


        [HttpGet("GetRoutineCategoryList")]
        public async Task<ActionResult<Services.ResponseModel<List<ApplicationMasterChildModel>>>> GetRoutineCategoryList(long ManufacturingProcessChildId)
        {

            var response = new Services.ResponseModel<ApplicationMasterChildModel>();

            var result = await _mediator.Send(new GetAllApplicationMasterChildByIdQuery(ManufacturingProcessChildId));
          
           
            try
            {
                response.ResponseCode = Services.ResponseCode.Success;
               
                response.Results = result.Count > 0 ? result : new List<ApplicationMasterChildModel> { new ApplicationMasterChildModel() };

            }
            catch (Exception ex)
            {
                response.ResponseCode = Services.ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }


        [HttpGet("GetActionList")]
        public async Task<ActionResult<Services.ResponseModel<List<ApplicationMasterChildModel>>>> GetActionList(long ProdActivityCategoryChildId)
        {

            var response = new Services.ResponseModel<ApplicationMasterChildModel>();

            var result = await _mediator.Send(new GetAllApplicationMasterChildByIdQuery(ProdActivityCategoryChildId));
            try
            {
                response.ResponseCode = Services.ResponseCode.Success;
                response.Results = result.Count > 0 ? result : new List<ApplicationMasterChildModel> { new ApplicationMasterChildModel() };
            }
            catch (Exception ex)
            {
                response.ResponseCode = Services.ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }


        [HttpGet("GetRoutineInfo")]
        public async Task<ActionResult<Services.ResponseModel<List<View_ApplicationMasterDetail>>>> GetRoutineInfo()
        {

            var response = new Services.ResponseModel<View_ApplicationMasterDetail>();

            var result = await _mediator.Send(new GetAllApplicationMasterDetailQuery(331));
            try
            {
                response.ResponseCode = Services.ResponseCode.Success;
                response.Results = result.Count > 0 ? result : new List<View_ApplicationMasterDetail> { new View_ApplicationMasterDetail() };
            }
            catch (Exception ex)
            {
                response.ResponseCode = Services.ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }
        [HttpGet("GetTemplateList")]
        public async Task<ActionResult<Services.ResponseModel<List<ProductActivityCaseLineModel>>>> GetTemplateList(long ManufacturingProcessChildId, long ProdActivityCategoryChildId,long ProdActivityActionChildD)
        {

            var response = new Services.ResponseModel<ProductActivityCaseLineModel>();

            if (ManufacturingProcessChildId > 0 && ProdActivityCategoryChildId > 0)
            {
                var result = await _mediator.Send(new GetProductActivityCaseLineTemplateItems(ManufacturingProcessChildId, ProdActivityCategoryChildId, ProdActivityActionChildD));


                try
                {
                    response.ResponseCode = Services.ResponseCode.Success;
                    response.Results = result.Count > 0 ? result : new List<ProductActivityCaseLineModel> { new ProductActivityCaseLineModel() };
                }
                catch (Exception ex)
                {
                    response.ResponseCode = Services.ResponseCode.Failure;
                    response.ErrorMessages.Add(ex.Message);
                }
            }
            return Ok(response);
        }
        [HttpPost("InsertRoutineMaster")]
        public async Task<ActionResult<Services.ResponseModel<IEnumerable<ProductionRoutine>>>> InsertRoutineMaster(ProductionRoutine value)
        {
            var response = new Services.ResponseModel<ProductionRoutine>();
            var request = new CreateProductionActivityRoutineAppCommand

            {
                ProductionActivityRoutineAppLineId = value.ProductionActivityRoutineAppLineId,
                CompanyId = value.CompanyID,
                ProdOrderNo = value.ProdOrderNo,
                LocationId = value.LocationID,
                AddedDate = DateTime.Now,
                SessionId = Guid.NewGuid(),
                LineSessionId = Guid.NewGuid(),
                StatusCodeID = 1,
                AddedByUserID = value.AddedByUserID,
                ManufacturingProcessChildId = value.ManufacturingProcessChildId,
                ProdActivityCategoryChildId = value.ProdActivityCategoryChildId,
                ProdActivityActionChildD = value.ProdActivityActionChildD,
                ProdActivityResultId = value.ProdActivityResultId,
                RoutineStatusId = value.RoutineStatusId,
                LineComment = value.LineComment,
                NavprodOrderLineId = value.NavprodOrderLineId > 0 ? value.NavprodOrderLineId : null,
                ModifiedByUserID = value.AddedByUserID,
                ModifiedDate = DateTime.Now,
                IsOthersOptions = value.OthersOptions == "Yes" ? true : false,
                TimeSheetAction = true,
               IsTemplateUpload = value.IsTemplateUpload,
               IsTemplateUploadFlag = value.IsTemplateUpload == true ? "Yes" : "No",
               ProductActivityCaseLineId = value.ProductActivityCaseLineId > 0 ? value.ProductActivityCaseLineId : null,
               RoutineInfoIds = value.RoutineInfoIds,
               LotNo =value.LotNo,
               ItemName = value.ItemName
           };

            var result = await _mediator.Send(request);
            var emailconversations = new ProductionRoutine
            {
                ProductionActivityRoutineAppId = (int)result,

            };
            try
            {
                response.ResponseCode = Services.ResponseCode.Success;
                response.Result = emailconversations;
            }
            catch (Exception ex)
            {
                response.ResponseCode = Services.ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }
        [HttpGet("GetLocationScan")]
        public async Task<ActionResult<Services.ResponseModel<List<ProductionActivityApp>>>> GetLocationScan(string LocationName)
        {

            var response = new Services.ResponseModel<RoutineScanModel>();

            if (LocationName != null)
            {
                var result = await _mediator.Send(new GetAllProductionActivityLocationAppQuery(LocationName));
                var display = new RoutineScanModel();
                if(result != null)
                 {
                    display = new RoutineScanModel
                    {
                        IctMasterID = result.ICTMasterID,
                        CompantId = result.CompanyID,

                    };
                }
                else
                {
                     display = new RoutineScanModel
                    {
                       Message = "Location Not Found"

                    };
                }
                try
                {
                    response.ResponseCode = Services.ResponseCode.Success;
                   
                    response.Result = display;
                }
                catch (Exception ex)
                {
                    response.ResponseCode = Services.ResponseCode.Failure;
                    response.ErrorMessages.Add(ex.Message);
                }
            }
            return Ok(response);
        }
        [HttpPost("GetRoutineDetailResult")]
        public async Task<ActionResult<Services.ResponseModel<List<ProductionRoutineDetailModel>>>> GetRoutineDetailResult(ProductionRoutineDetailModel RoutineDetailModel)
        {

            var response = new Services.ResponseModel<ProductionRoutineDetailModel>();
            ProductionActivityRoutineAppModel FilterData = new ProductionActivityRoutineAppModel();
            if (RoutineDetailModel.CompanyId> 0)
            {
                FilterData.CompanyId = RoutineDetailModel.CompanyId;
                FilterData.LotNo = RoutineDetailModel.LotNo;
                FilterData.ItemName = RoutineDetailModel.ItemName;
                FilterData.AddedByUserID = RoutineDetailModel.AddedByUserID;
                FilterData.GetTypes = "User";
                FilterData.LocationId = RoutineDetailModel.LocationId;
                FilterData.TimeSheetAction = true;
                var result = await _mediator.Send(new GetAllProductionActivityRoutineAppLineQuery(FilterData));


                var displayResult = result?.Select(topic => new ProductionRoutineDetailModel
                {

                  Type =topic.Type,
                    ActivityProfileNo =topic.ActivityProfileNo,
                    AddedDate =topic.AddedDate,
                    ManufacturingProcessChild = topic.ManufacturingProcessChild,
                    ProdActivityCategoryChild = topic.ProdActivityCategoryChild,
                    ProdActivityActionChild = topic.ProdActivityActionChild,
                    LineComment = topic.LineComment,
                    ModifiedByUser = topic.ModifiedByUser,
                    ModifiedDate = topic.ModifiedDate,
                    ProdActivityResult = topic.ProdActivityResult,
                    MasterProductionFileProfileTypeId =topic.MasterProductionFileProfileTypeId,
                    ProductionActivityRoutineAppLineId = topic.ProductionActivityRoutineAppLineId,
                    ProductionActivityRoutineAppId = topic.ProductionActivityRoutineAppId,
                    ManufacturingProcessChildId = topic.ManufacturingProcessChildId,
                    ProdActivityCategoryChildId = topic.ProdActivityCategoryChildId,
                    ProdActivityActionChildD = topic.ProdActivityActionChildD,
                    IsTemplateUpload = topic.IsTemplateUpload,
                    ProductActivityCaseLineId = topic.ProductActivityCaseLineId,
                    LineSessionId = topic.LineSessionId.ToString(),
                    IsOthersOptions = topic.IsOthersOptions,
                    ProdActivityResultId = topic.ProdActivityResultId,
                    RoutineStatusId = topic.RoutineStatusId,
                    LocationName = topic.LocationName,
                    RoutineInfoStatus =topic.RoutineInfoStatus,
                    IsTemplateUploadFlag = topic.IsTemplateUploadFlag,
                    NameOfTemplate = topic.NameOfTemplate,
                    OthersOptions = topic.OthersOptions,
                    RoutineInfoIds =topic.RoutineInfoIds

                }).ToList();
                try
                {
                    response.ResponseCode = Services.ResponseCode.Success;
                    response.Results = displayResult;
                }
                catch (Exception ex)
                {
                    response.ResponseCode = Services.ResponseCode.Failure;
                    response.ErrorMessages.Add(ex.Message);
                }
            }
            return Ok(response);
        }

        [HttpPost("DeleteRoutineMaster")]
        public async Task<ActionResult<Services.ResponseModel<IEnumerable<ProductionRoutineModel>>>> DeleteRoutineMaster(ProductionRoutineModel value)
        {
            var response = new Services.ResponseModel<ProductionRoutineModel>();
            ProductionActivityRoutineAppModel Data = new ProductionActivityRoutineAppModel();
            Data.ProductionActivityRoutineAppLineId = value.ProductionActivityRoutineAppLineId;
            Data.LineSessionId = value.LineSessionId;
            var result = await _RoutineQueryRepository.DeleteproductActivityRoutineAppLine(Data);


            try
            {
                response.ResponseCode = Services.ResponseCode.Success;

                var emailconversations = new ProductionRoutineModel
                {
                   
                    Message = "Delete Successfully"
                };

                response.Result = emailconversations;
               
            }
            catch (Exception ex)
            {
                response.ResponseCode = Services.ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }
        [HttpGet("GetDivisionList")]
        public async Task<ActionResult<Services.ResponseModel<List<ViewDivision>>>> GetDivisionList(long CompanyID)
        {

            var response = new Services.ResponseModel<ViewDivision>();

            var result = await _mediator.Send(new GetDivisionByCompany(CompanyID));
            try
            {
                response.ResponseCode = Services.ResponseCode.Success;
                response.Results = result.Count > 0 ? result : new List<ViewDivision> { new ViewDivision() };
            }
            catch (Exception ex)
            {
                response.ResponseCode = Services.ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }
        [HttpGet("GetDepartmentList")]
        public async Task<ActionResult<Services.ResponseModel<List<ViewDepartment>>>> GetDepartmentList(long divisionId)
        {

            var response = new Services.ResponseModel<ViewDepartment>();

            var result = await _mediator.Send(new GetDepartmentByDivision(divisionId));
            try
            {
                response.ResponseCode = Services.ResponseCode.Success;
                response.Results = result.Count > 0 ? result : new List<ViewDepartment> { new ViewDepartment() };
            }
            catch (Exception ex)
            {
                response.ResponseCode = Services.ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }
        [HttpGet("GetSectionList")]
        public async Task<ActionResult<Services.ResponseModel<List<ViewSection>>>> GetSectionList(long departmentId)
        {

            var response = new Services.ResponseModel<ViewSection>();

            var result = await _mediator.Send(new GetSectionByDepartment(departmentId));
            try
            {
                response.ResponseCode = Services.ResponseCode.Success;
                response.Results = result.Count > 0 ? result : new List<ViewSection> { new ViewSection() };
            }
            catch (Exception ex)
            {
                response.ResponseCode = Services.ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }
        [HttpGet("GetSubSectionList")]
        public async Task<ActionResult<Services.ResponseModel<List<ViewSubSection>>>> GetSubSectionList(long sectionId)
        {

            var response = new Services.ResponseModel<ViewSubSection>();

            var result = await _mediator.Send(new GetSubSectionBySection(sectionId));
            try
            {
                response.ResponseCode = Services.ResponseCode.Success;
                response.Results = result.Count > 0 ? result : new List<ViewSubSection> { new ViewSubSection() };
            }
            catch (Exception ex)
            {
                response.ResponseCode = Services.ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }
        [HttpGet("GetFileProfileTypeList")]
        public async Task<ActionResult<Services.ResponseModel<List<DocumentsModel>>>> GetFileProfileTypeList(long FileProfileTypeID)
        {

            var response = new Services.ResponseModel<DocumentfileType>();
           
            var result = await _mediator.Send(new GetFileProfileTypeList(FileProfileTypeID));

            DocumentfileType FilterData = new DocumentfileType();
            FilterData.Name = result.Name;
            FilterData.Description = result.Description;
            FilterData.SessionId = (Guid)result.SessionId;
            FilterData.FileProfileTypeID = (long)result.FileProfileTypeId;
            FilterData.ProfileID = (long)result.ProfileID;
            try
            {
                response.ResponseCode = Services.ResponseCode.Success;
                response.Result = FilterData;
            }
            catch (Exception ex)
            {
                response.ResponseCode = Services.ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }
        [HttpGet("GetFilleProfileTypeTree")]
        public async Task<ActionResult<Services.ResponseModel<List<FileProfileDropDown>>>> GetFilleProfileTypeTree(long ProfileID)
        {
            
            var response = new Services.ResponseModel<FileProfileDropDown>();
            FileProfileDropDown Data = new FileProfileDropDown();

         var documentProfileNoSeriesData = await _mediator.Send(new GetDocumentProfileNoSeriesById(ProfileID));
            if(documentProfileNoSeriesData != null)
            {
                dynamic abbreviation = JsonConvert.DeserializeObject(documentProfileNoSeriesData.Abbreviation1);
                if (abbreviation != null)
                {
                    foreach (var item in abbreviation)
                    {
                        var itemsId = item.Id;
                        if (itemsId == 1)
                        {
                            Data.isPlant = true;

                        }
                        if (itemsId == 2)
                        {
                            Data.isDepartment = true;

                        }
                        if (itemsId == 3)
                        {
                            Data.isSection = true;

                        }
                        if (itemsId == 4)
                        {
                            Data.isSubSection = true;

                        }
                    }
                }
            }
           
           
            try
            {
                response.ResponseCode = Services.ResponseCode.Success;
                response.Result = Data;
            }
            catch (Exception ex)
            {
                response.ResponseCode = Services.ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }
    }
}
