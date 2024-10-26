using Application.Queries;
using Core.Entities;
using Core.Entities.Views;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.QueryHandlers
{
    public class GetAllEmailActivityCatgorysHandler : IRequestHandler<GetAllEmailActivityCatgorys, List<EmailActivityCatgorys>>
    {
        private readonly IEmailActivityCatgorysQueryRepository _emailactyqueryRepository;        
        public GetAllEmailActivityCatgorysHandler(IEmailActivityCatgorysQueryRepository emailactyqueryRepository)
        {
            _emailactyqueryRepository = emailactyqueryRepository;
        }
        public async Task<List<EmailActivityCatgorys>> Handle(GetAllEmailActivityCatgorys request, CancellationToken cancellationToken)
        {   
            return (List<EmailActivityCatgorys>)await _emailactyqueryRepository.GetAllAsync();
        }
    }
    public class GetAllTopicCategoryHandler : IRequestHandler<GetAllTopicCategory, List<EmailActivityCatgorys>>
    {

        private readonly IEmailActivityCatgorysQueryRepository _emailactyqueryRepository;
        public GetAllTopicCategoryHandler(IEmailActivityCatgorysQueryRepository emailactyqueryRepository)
        {
            _emailactyqueryRepository = emailactyqueryRepository;
        }
        public async Task<List<EmailActivityCatgorys>> Handle(GetAllTopicCategory request, CancellationToken cancellationToken)
        {
            return (List<EmailActivityCatgorys>)await _emailactyqueryRepository.GetAllTopicCategoryAsync(request.TopicId);
            
        }
    }
    public class GetGetActionTagMultipleHandler : IRequestHandler<GetActionTagMultiple, List<long>>
    {

        private readonly IEmailActivityCatgorysQueryRepository _emailactyqueryRepository;
        public GetGetActionTagMultipleHandler(IEmailActivityCatgorysQueryRepository emailactyqueryRepository)
        {
            _emailactyqueryRepository = emailactyqueryRepository;
        }
        public async Task<List<long>> Handle(GetActionTagMultiple request, CancellationToken cancellationToken)
        {
            return (List<long>)await _emailactyqueryRepository.GetActionTagMultipleAsync(request.TopicId);

        }
    }
    
    public class GetTagLockInfoHandler : IRequestHandler<GetTagLockInfo, bool>
    {

        private readonly IEmailActivityCatgorysQueryRepository _emailactyqueryRepository;
        public GetTagLockInfoHandler(IEmailActivityCatgorysQueryRepository emailactyqueryRepository)
        {
            _emailactyqueryRepository = emailactyqueryRepository;
        }
        public async Task<bool> Handle(GetTagLockInfo request, CancellationToken cancellationToken)
        {
            return await _emailactyqueryRepository.GetTagLockInfoAsync(request.TopicId);

        }
    }

    public class CreateEmailActivityCatgorysHandler : IRequestHandler<CreateEmailActivityCatgorysQuery, long>
    {
        private readonly IEmailActivityCatgorysQueryRepository _emailactyqueryRepository;
        public CreateEmailActivityCatgorysHandler(IEmailActivityCatgorysQueryRepository emailactyqueryRepository, IQueryRepository<EmailActivityCatgorys> queryRepository)
        {
            _emailactyqueryRepository = emailactyqueryRepository;
        }

        public async Task<long> Handle(CreateEmailActivityCatgorysQuery request, CancellationToken cancellationToken)
        {
            var newlist = await _emailactyqueryRepository.Insert(request);
            return newlist;
        }
    }

    public class EditTopicCategoryQeryHandler : IRequestHandler<EditTopicCategoryQery, long>
    {
        private readonly IEmailActivityCatgorysQueryRepository _emailactyqueryRepository;
        public EditTopicCategoryQeryHandler(IEmailActivityCatgorysQueryRepository emailactyqueryRepository, IQueryRepository<EmailActivityCatgorys> queryRepository)
        {
            _emailactyqueryRepository = emailactyqueryRepository;
        }

        public async Task<long> Handle(EditTopicCategoryQery request, CancellationToken cancellationToken)
        {
            var newlist = await _emailactyqueryRepository.UpdateAsync(request);
            return newlist;
        }
    }

    public class DeleteTopicCategoryQeryHandler : IRequestHandler<DeleteTopicCategoryQery, long>
    {
        private readonly IEmailActivityCatgorysQueryRepository _emailactyqueryRepository;
        public DeleteTopicCategoryQeryHandler(IEmailActivityCatgorysQueryRepository emailactyqueryRepository, IQueryRepository<EmailActivityCatgorys> queryRepository)
        {
            _emailactyqueryRepository = emailactyqueryRepository;
        }

        public async Task<long> Handle(DeleteTopicCategoryQery request, CancellationToken cancellationToken)
        {
            var newlist = await _emailactyqueryRepository.DeleteAsync(request.ID,request.TopicID);
            return newlist;
        }
    }

    public class GetAllemailCategoryHandler : IRequestHandler<GetAllemailCategory, List<EmailActivityCatgorys>>
    {

        private readonly IEmailActivityCatgorysQueryRepository _emailactyqueryRepository;
        public GetAllemailCategoryHandler(IEmailActivityCatgorysQueryRepository emailactyqueryRepository)
        {
            _emailactyqueryRepository = emailactyqueryRepository;
        }
        public async Task<List<EmailActivityCatgorys>> Handle(GetAllemailCategory request, CancellationToken cancellationToken)
        {
            return (List<EmailActivityCatgorys>)await _emailactyqueryRepository.GetAllemailCategoryAsync(request.TopicId);

        }
    }



    public class GetAllUserActivityCatgorysHandler : IRequestHandler<GetAllUserActivityCatgorys, List<EmailActivityCatgorys>>
    {
        private readonly IEmailActivityCatgorysQueryRepository _emailactyqueryRepository;
        public GetAllUserActivityCatgorysHandler(IEmailActivityCatgorysQueryRepository emailactyqueryRepository)
        {
            _emailactyqueryRepository = emailactyqueryRepository;
        }
        public async Task<List<EmailActivityCatgorys>> Handle(GetAllUserActivityCatgorys request, CancellationToken cancellationToken)
        {
            return (List<EmailActivityCatgorys>)await _emailactyqueryRepository.GetAllUserTagAsync(request.UserID);
        }
    }
    public class GetByUserTageHandler : IRequestHandler<GetByUserTage, List<EmailActivityCatgorys>>
    {
        private readonly IEmailActivityCatgorysQueryRepository _emailactyqueryRepository;
        public GetByUserTageHandler(IEmailActivityCatgorysQueryRepository emailactyqueryRepository)
        {
            _emailactyqueryRepository = emailactyqueryRepository;
        }
        public async Task<List<EmailActivityCatgorys>> Handle(GetByUserTage request, CancellationToken cancellationToken)
        {
            return (List<EmailActivityCatgorys>)await _emailactyqueryRepository.GetByUserTagAsync(request.TopicID,request.UserID);
        }
    }
    public class EditOtherQeryHandler : IRequestHandler<EditOtherTagQery, string>
    {
        private readonly IEmailActivityCatgorysQueryRepository _emailactyqueryRepository;
        public EditOtherQeryHandler(IEmailActivityCatgorysQueryRepository emailactyqueryRepository, IQueryRepository<EmailActivityCatgorys> queryRepository)
        {
            _emailactyqueryRepository = emailactyqueryRepository;
        }

        public async Task<string> Handle(EditOtherTagQery request, CancellationToken cancellationToken)
        {
            var newlist = await _emailactyqueryRepository.UpdateOtherAsync(request.otherTag,request.Name,request.modifiedByUserID);
            return newlist;
        }
    }
    public class EditOtherTagQeryHandler : IRequestHandler<EditOtherQery, string>
    {
        private readonly IEmailActivityCatgorysQueryRepository _emailactyqueryRepository;
        public EditOtherTagQeryHandler(IEmailActivityCatgorysQueryRepository emailactyqueryRepository, IQueryRepository<EmailActivityCatgorys> queryRepository)
        {
            _emailactyqueryRepository = emailactyqueryRepository;
        }

        public async Task<string> Handle(EditOtherQery request, CancellationToken cancellationToken)
        {
            var newlist = await _emailactyqueryRepository.UpdateOtherTagAsync(request.id, request.Name, request.modifiedByUserID);
            return newlist;
        }
    }
    public class EditUsertagQeryHandler : IRequestHandler<EditUserTagQery, string>
    {
        private readonly IEmailActivityCatgorysQueryRepository _emailactyqueryRepository;
        public EditUsertagQeryHandler(IEmailActivityCatgorysQueryRepository emailactyqueryRepository, IQueryRepository<EmailActivityCatgorys> queryRepository)
        {
            _emailactyqueryRepository = emailactyqueryRepository;
        }

        public async Task<string> Handle(EditUserTagQery request, CancellationToken cancellationToken)
        {
            var newlist = await _emailactyqueryRepository.UpdateuserAsync(request.userTag, request.Name);
            return newlist;
        }
    }
    public class DeleteUserTagHandler : IRequestHandler<DeleteUserTagQery, long>
    {
        private readonly IEmailActivityCatgorysQueryRepository _emailactyqueryRepository;
        public DeleteUserTagHandler(IEmailActivityCatgorysQueryRepository emailactyqueryRepository, IQueryRepository<EmailActivityCatgorys> queryRepository)
        {
            _emailactyqueryRepository = emailactyqueryRepository;
        }

        public async Task<long> Handle(DeleteUserTagQery request, CancellationToken cancellationToken)
        {
            var newlist = await _emailactyqueryRepository.DeleteUserTagAsync(request.ID);
            return newlist;
        }
    }
    public class GetAllOthersHandler : IRequestHandler<GetAllOthersQuery, List<EmailActivityCatgorys>>
    {

        private readonly IEmailActivityCatgorysQueryRepository _emailactyqueryRepository;
        public GetAllOthersHandler(IEmailActivityCatgorysQueryRepository emailactyqueryRepository)
        {
            _emailactyqueryRepository = emailactyqueryRepository;
        }
        public async Task<List<EmailActivityCatgorys>> Handle(GetAllOthersQuery request, CancellationToken cancellationToken)
        {
            return (List<EmailActivityCatgorys>)await _emailactyqueryRepository.GetAllOthersAsync(request.OtherTag);

        }
    }
}
