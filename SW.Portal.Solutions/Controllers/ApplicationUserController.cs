using AutoMapper;
using Core.Entities;
using Core.Repositories.Query;
using DevExpress.XtraRichEdit.Model;
using Infrastructure.Repository.Query;
using Microsoft.AspNetCore.Mvc;
using SW.Portal.Solutions.Models;
using SW.Portal.Solutions.Services;

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
        public async Task<ActionResult<ResponseModel<IEnumerable<ApplicationUser>>>> Login(string loginId, string password)
        {
            var response = new ResponseModel<ApplicationUser>();

            try
            {
                response.ResponseCode = ResponseCode.Success;

                var lst = await _applicationUserQueryRepository.LoginAuth(loginId, password);
                if(lst != null)
                {
                    if(lst.Locked)
                    {
                        response.ResponseCode = ResponseCode.Failure;
                        response.ErrorMessages.Add("Invalid Credentials");
                    }
                    else
                    {
                        if(lst.InvalidAttempts != 0)
                        {
                            response.ResponseCode = ResponseCode.Failure;
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
                    response.ErrorMessages.Add("Invalid User");
                }
                                 
            }
            catch (Exception ex)
            {
                response.ResponseCode = ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);


            //var response = await _applicationUserQueryRepository.LoginAuth(loginId, password);
            //if (response != null)
            //    return Ok(response);
            //else
            //    return NotFound("Invalid User");
        }

        [HttpPost]
        [Route("UpdateMobileDeviceId")]
        public async Task<ActionResult<ResponseModel<IEnumerable<ReplyConversation>>>> UpdateMobileDeviceId(string LoginId, string DeviceID)
        {
            var response = new ResponseModel<ReplyConversation>();

            try
            {
                response.ResponseCode = ResponseCode.Success;

                var lst = await _applicationUserQueryRepository.UpdateDeviceId(LoginId, DeviceID);
                if (lst != "-1")
                {
                    var emailconversations = new ReplyConversation
                    {                        
                        Message = "Updated Successfully"
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


            //var response = await _applicationUserQueryRepository.LoginAuth(loginId, password);
            //if (response != null)
            //    return Ok(response);
            //else
            //    return NotFound("Invalid User");
        }


        [HttpGet]
        public string Get()
        {
            return "Test";
        }
    }
}
