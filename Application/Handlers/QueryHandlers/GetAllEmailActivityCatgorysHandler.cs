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
            var newlist = await _emailactyqueryRepository.DeleteAsync(request.ID);
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
            var newlist = await _emailactyqueryRepository.UpdateOtherAsync(request.otherTag,request.Name);
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

}
