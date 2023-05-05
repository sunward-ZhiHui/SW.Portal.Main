using Application.Common.Mapper;
using Application.Queries;
using Application.Response;
using Core.Entities;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using MediatR;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CMS.Application.Handlers.QueryHandlers
{
    public class LoginHandler: IRequestHandler<LoginRequest, ApplicationUser>
    {
        private readonly IApplicationUserQueryRepository _applicationUserQueryRepository;
        private ILocalStorageService<ApplicationUser> _localStorageService;
        private string _userKey = "user";

       // public ApplicationUser User { get; private set; }

        public LoginHandler(IApplicationUserQueryRepository applicationUserQueryRepository, ILocalStorageService<ApplicationUser> localStorageService)
        {
            _applicationUserQueryRepository = applicationUserQueryRepository;
            _localStorageService = localStorageService;
        }

        public async Task<ApplicationUser> Handle(LoginRequest request, CancellationToken cancellationToken)
        {
            //var loginEntity = RoleMapper.Mapper.Map<ApplicationUser>(request);

            //if (loginEntity is null)
            //{
            //    throw new ApplicationException("There is a problem in mapper");
            //}

            //var newEntity = await _applicationUserQueryRepository.Auth(loginEntity);
            //var loginResponse = RoleMapper.Mapper.Map<ApplicationUser>(newEntity);
            //return loginResponse;

           // User = await _localStorageService.GetItem<ApplicationUser>(_userKey);

            var newEntity = await _applicationUserQueryRepository.Auth(request.LoginID,request.Password);          
            await _localStorageService.SetItem(_userKey, newEntity);
            return newEntity;
                            
            
        }


    }
    public class UpdateUserHandler : IRequestHandler<UpdateUserPasswordRequest, ApplicationUser>
    {
        private readonly IApplicationUserQueryRepository _applicationUserQueryRepository;
        private ILocalStorageService<ApplicationUser> _localStorageService;
        private string _userKey = "user";

        public UpdateUserHandler(IApplicationUserQueryRepository applicationUserQueryRepository, ILocalStorageService<ApplicationUser> localStorageService)
        {
            _applicationUserQueryRepository = applicationUserQueryRepository;
            _localStorageService = localStorageService;
        }

        public async Task<ApplicationUser> Handle(UpdateUserPasswordRequest request, CancellationToken cancellationToken)
        {
            var newEntity = await _applicationUserQueryRepository.UpdatePasswordUser(request.LoginID, request.Password);
            return newEntity;
        }


    }

}
