using AutoMapper;
using Core.Entities;
using Core.Repositories.Query;
using DevExpress.XtraRichEdit.Model;
using Duende.IdentityServer.Models;
using Infrastructure.Repository.Query;
using Microsoft.AspNetCore.Mvc;
using SW.Portal.Solutions.Models;
using SW.Portal.Solutions.Services;
using LoginModel = SW.Portal.Solutions.Models.LoginModel;

namespace SW.Portal.Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationUserController : Controller
    {
        private readonly IApplicationUserQueryRepository _applicationUserQueryRepository;
        private readonly IMapper _mapper;

        public ApplicationUserController(IApplicationUserQueryRepository applicationUserQueryRepository, IMapper mapper)
        {
            _applicationUserQueryRepository = applicationUserQueryRepository;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult<ResponseModel<IEnumerable<ApplicationUser>>>> Login([FromBody] LoginModel loginModel)
        {
            var response = new ResponseModel<ApplicationUser>();

            try
            {
                response.ResponseCode = ResponseCode.Success;

                var lst = await _applicationUserQueryRepository.LoginAuth(loginModel.loginId, loginModel.password);
                if (lst != null)
                {
                    if (lst.Locked)
                    {
                        response.ResponseCode = ResponseCode.Failure;
                        response.Result = new ApplicationUser();
                        response.ErrorMessages.Add("Your Account Locked");
                    }
                    else
                    {
                        if (lst.InvalidAttempts != 0)
                        {
                            response.ResponseCode = ResponseCode.Failure;
                            response.Result = new ApplicationUser();
                            response.ErrorMessages.Add("Invalid Password, " + (3 - lst.InvalidAttempts) + " attempt(s) left.");
                        }
                        else
                        {
                            response.Result = _mapper.Map<ApplicationUser>(lst);
                        }
                    }

                }

                else
                {
                    response.ResponseCode = ResponseCode.Failure;
                    response.Result = new ApplicationUser();
                    response.ErrorMessages.Add("Invalid User");
                }

            }
            catch (Exception ex)
            {
                response.ResponseCode = ResponseCode.Failure;
                response.Result = new ApplicationUser();
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);

        }

        [HttpPost]
        [Route("UpdateDeviceinfo")]
        public async Task<ActionResult<ResponseModel<IEnumerable<ReplyConversation>>>> UpdateDeviceinfo([FromBody] DeviceTkenModel devicemodel)
        {
            var response = new ResponseModel<ReplyConversation>();

            try
            {
                response.ResponseCode = ResponseCode.Success;
                var lst = await _applicationUserQueryRepository.UpdateDeviceId(devicemodel.LoginId, devicemodel.DeviceType, devicemodel.TokenID);
                if (lst != "-1")
                {
                    var emailconversations = new ReplyConversation
                    {
                        Message = lst.ToString(),
                    };

                    response.Result = emailconversations;
                }
                else
                {
                    response.ResponseCode = ResponseCode.Failure;
                    response.ErrorMessages.Add("Invalid User");
                }

            }
            catch (Exception ex)
            {
                response.ResponseCode = ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }
        [HttpGet("GetTokenList")]
        public async Task<ActionResult<ResponseModel<List<UserNotification>>>> GetTokenList()
        {
            var response = new ResponseModel<UserNotification>();
            try
            {
                response.ResponseCode = ResponseCode.Success;
                var userNotifications = await _applicationUserQueryRepository.GetTokenList();
                response.Results = userNotifications; // Assign the list of results
            }
            catch (Exception ex)
            {
                response.ResponseCode = ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }

        [HttpGet("GetAllMobileTokenList")]
        public async Task<ActionResult<ResponseModel<List<UserNotification>>>> GetAllMobileTokenList(string DeviceType)
        {
            var response = new ResponseModel<UserNotification>();
            List<String>? tokenList = [];
            try
            {

                response.ResponseCode = ResponseCode.Success;
                var userNotifications = await _applicationUserQueryRepository.GetAllTokenList(DeviceType);
                foreach (var token in userNotifications)
                {
                    tokenList.Add(token.TokenID);
                }
                response.ErrorMessages = tokenList; // Assign the list of results
            }
            catch (Exception ex)
            {
                response.ResponseCode = ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }


        [HttpGet]
        public string Get()
        {
            return "Test";
        }
    }
}
