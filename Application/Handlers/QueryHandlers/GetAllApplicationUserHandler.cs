using Application.Queries;
using Core.Entities;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using MediatR;
using System;
using System.Linq;

namespace CMS.Application.Handlers.QueryHandlers
{
    public class GetAllApplicationUserHandler : IRequestHandler<GetAllApplicationUserQuery, List<ApplicationUser>>
    {
        private readonly IApplicationUserQueryRepository _applicationUserQueryRepository;
        private readonly IQueryRepository<ApplicationUser> _queryRepository;
        public GetAllApplicationUserHandler(IApplicationUserQueryRepository applicationUserQueryRepository, IQueryRepository<ApplicationUser> queryRepository)
        {
            _applicationUserQueryRepository = applicationUserQueryRepository;
            _queryRepository= queryRepository;
        }
        public async Task<List<ApplicationUser>> Handle(GetAllApplicationUserQuery request, CancellationToken cancellationToken)
        {
            return (List<ApplicationUser>)await _queryRepository.GetListAsync();
            //return (List<ApplicationUser>)await _applicationUserQueryRepository.GetAllAsync();
        }
    }
    public class GetAllApplicationUseByLoiginIDrHandler : IRequestHandler<GetAllApplicationUserByLoginIDQuery, ApplicationUser>
    {
        private readonly IApplicationUserQueryRepository _applicationUserQueryRepository;
        private readonly IQueryRepository<ApplicationUser> _queryRepository;
        public GetAllApplicationUseByLoiginIDrHandler(IApplicationUserQueryRepository applicationUserQueryRepository, IQueryRepository<ApplicationUser> queryRepository)
        {
            _applicationUserQueryRepository = applicationUserQueryRepository;
            _queryRepository = queryRepository;
        }
        public async Task<ApplicationUser> Handle(GetAllApplicationUserByLoginIDQuery request, CancellationToken cancellationToken)
        {
            return await _applicationUserQueryRepository.GetByUsers(request.Name);
        }
    }
    public class AddPushTokenIDHandler : IRequestHandler<AddPushTokenID, string>
    {
        private readonly IApplicationUserQueryRepository _applicationUserQueryRepository;
        private readonly IQueryRepository<ApplicationUser> _queryRepository;
        public AddPushTokenIDHandler(IApplicationUserQueryRepository applicationUserQueryRepository, IQueryRepository<ApplicationUser> queryRepository)
        {
            _applicationUserQueryRepository = applicationUserQueryRepository;
            _queryRepository = queryRepository;
        }
        public async Task<String> Handle(AddPushTokenID request, CancellationToken cancellationToken)
        {
            return await _applicationUserQueryRepository.UpdateDeviceId(request.LoginId,request.DeviceType,request.TokenID);
        }
    }
    public class DeleteTokenIDHandler : IRequestHandler<DeletePushTokenID, long>
    {
        private readonly IApplicationUserQueryRepository _applicationUserQueryRepository;
        private readonly IQueryRepository<ApplicationUser> _queryRepository;
        public DeleteTokenIDHandler(IApplicationUserQueryRepository applicationUserQueryRepository, IQueryRepository<ApplicationUser> queryRepository)
        {
            _applicationUserQueryRepository = applicationUserQueryRepository;
            _queryRepository = queryRepository;
        }
        public async Task<long> Handle(DeletePushTokenID request, CancellationToken cancellationToken)
        {
            return await _applicationUserQueryRepository.DeleteDeviceId(request.TokenID);
        }
    }
}
