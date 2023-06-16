using Application.Common.Mapper;
using Application.Queries;
using Application.Response;
using Core.Entities;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using MediatR;
using System;
using System.Linq;

namespace CMS.Application.Handlers.QueryHandlers
{
   
    public class GetUserEmailTopicListHandler : IRequestHandler<GetUserEmailTopics, List<EmailTopics>>
    {

        private readonly IEmailTopicsQueryRepository _forumTopicsQueryRepository;
        public GetUserEmailTopicListHandler(IEmailTopicsQueryRepository forumTopicsQueryRepository)
        {
           _forumTopicsQueryRepository = forumTopicsQueryRepository;
        }
        public async Task<List<EmailTopics>> Handle(GetUserEmailTopics request, CancellationToken cancellationToken)
        {
            return await _forumTopicsQueryRepository.GetUserTopicList(request.UserId);
           
        }
    }
    public class GetEmailTopicToHandler : IRequestHandler<GetEmailTopicTo, List<EmailTopics>>
    {

        private readonly IEmailTopicsQueryRepository _forumTopicsQueryRepository;
        public GetEmailTopicToHandler(IEmailTopicsQueryRepository forumTopicsQueryRepository)
        {
            _forumTopicsQueryRepository = forumTopicsQueryRepository;
        }
        public async Task<List<EmailTopics>> Handle(GetEmailTopicTo request, CancellationToken cancellationToken)
        {
            return await _forumTopicsQueryRepository.GetTopicToList(request.UserId);

        }
    }
    public class GetEmailTopicCCHandler : IRequestHandler<GetEmailTopicCC, List<EmailTopics>>
    {

        private readonly IEmailTopicsQueryRepository _forumTopicsQueryRepository;
        public GetEmailTopicCCHandler(IEmailTopicsQueryRepository forumTopicsQueryRepository)
        {
            _forumTopicsQueryRepository = forumTopicsQueryRepository;
        }
        public async Task<List<EmailTopics>> Handle(GetEmailTopicCC request, CancellationToken cancellationToken)
        {
            return await _forumTopicsQueryRepository.GetTopicCCList(request.UserId);

        }
    }
    public class GetSentTopicHandler : IRequestHandler<GetSentTopic, List<EmailTopics>>
    {

        private readonly IEmailTopicsQueryRepository _forumTopicsQueryRepository;
        public GetSentTopicHandler(IEmailTopicsQueryRepository forumTopicsQueryRepository)
        {
            _forumTopicsQueryRepository = forumTopicsQueryRepository;
        }
        public async Task<List<EmailTopics>> Handle(GetSentTopic request, CancellationToken cancellationToken)
        {
            return await _forumTopicsQueryRepository.GetTopicSentList(request.UserId);

        }
    }

    public class GetEmailParticipantListHandler : IRequestHandler<GetEmailParticipantsList, List<TopicParticipant>>
    {

        private readonly IEmailTopicsQueryRepository _forumTopicsQueryRepository;
        public GetEmailParticipantListHandler(IEmailTopicsQueryRepository forumTopicsQueryRepository)
        {
            _forumTopicsQueryRepository = forumTopicsQueryRepository;
        }
        public async Task<List<TopicParticipant>> Handle(GetEmailParticipantsList request, CancellationToken cancellationToken)
        {
            return await _forumTopicsQueryRepository.GetParticipantList(request.TopicId);
        }
    }

    public class GetByIdEmailTopicListHandler : IRequestHandler<GetByIdEmailTopics, List<EmailTopics>>
    {

        private readonly IEmailTopicsQueryRepository _forumTopicsQueryRepository;
        public GetByIdEmailTopicListHandler(IEmailTopicsQueryRepository forumTopicsQueryRepository)
        {
            _forumTopicsQueryRepository = forumTopicsQueryRepository;
        }
        public async Task<List<EmailTopics>> Handle(GetByIdEmailTopics request, CancellationToken cancellationToken)
        {
            return await _forumTopicsQueryRepository.GetByIdAsync(request.ID);

        }
    }
    
    public class CreateEmailTopicsHandler : IRequestHandler<CreateEmailTopics, long>
    {
        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;

        public CreateEmailTopicsHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }
        public async Task<long> Handle(CreateEmailTopics request, CancellationToken cancellationToken)
        {
            var customerEntity = RoleMapper.Mapper.Map<EmailTopics>(request);

            if (customerEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            var newTopics = _emailTopicsQueryRepository.Insert(customerEntity);
            var customerResponse = RoleMapper.Mapper.Map<long>(newTopics);
            return customerResponse;
        }
    }
    public class UpdateEmailTopicDueDateHandler : IRequestHandler<UpdateEmailTopicDueDate, long>
    {
        private readonly IEmailTopicsQueryRepository _forumTopicsQueryRepository;

        public UpdateEmailTopicDueDateHandler(IEmailTopicsQueryRepository forumTopicsQueryRepository)
        {
            _forumTopicsQueryRepository = forumTopicsQueryRepository;

        }

        public async Task<long> Handle(UpdateEmailTopicDueDate request, CancellationToken cancellationToken)
        {
            var req = await _forumTopicsQueryRepository.UpdateDueDate(request);
            return req;
        }
    }
    public class UpdateEmailTopicClosedHandler : IRequestHandler<UpdateEmailTopicClosed, long>
    {
        private readonly IEmailTopicsQueryRepository _forumTopicsQueryRepository;

        public UpdateEmailTopicClosedHandler(IEmailTopicsQueryRepository forumTopicsQueryRepository)
        {
            _forumTopicsQueryRepository = forumTopicsQueryRepository;

        }

        public async Task<long> Handle(UpdateEmailTopicClosed request, CancellationToken cancellationToken)
        {
            var req = await _forumTopicsQueryRepository.UpdateTopicClose(request);
            return req;
        }
    }

    public class CreateEmailParticipantHandler : IRequestHandler<CreateEmailTopicParticipant, long>
    {
        private readonly IEmailTopicsQueryRepository _forumTopicsQueryRepository;

        public CreateEmailParticipantHandler(IEmailTopicsQueryRepository forumTopicsQueryRepository)
        {
            _forumTopicsQueryRepository = forumTopicsQueryRepository;
        }
        public async Task<long> Handle(CreateEmailTopicParticipant request, CancellationToken cancellationToken)
        {
            var newTopics = await _forumTopicsQueryRepository.InsertParticipant(request);
            //var customerResponse = RoleMapper.Mapper.Map<long>(newTopics);
            return newTopics;
        }
    }
    public class GetAllEmailTopicsHandler : IRequestHandler<GetAllEmailTopics, List<EmailTopics>>
    {

        private readonly IQueryRepository<EmailTopics> _queryRepository;
        public GetAllEmailTopicsHandler(IQueryRepository<EmailTopics> queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<EmailTopics>> Handle(GetAllEmailTopics request, CancellationToken cancellationToken)
        {
            return (List<EmailTopics>)await _queryRepository.GetListAsync();
            //return (List<EmailTypes>)await _roleQueryRepository.GetAllAsync();
        }
    }
    //public class GetEmailTopicListHandler : IRequestHandler<GetEmailTopicList, string type>
    //{
    //    private readonly IEmailTopicsQueryRepository _forumTopicsQueryRepository;

    //    public GetEmailTopicListHandler(IEmailTopicsQueryRepository forumTopicsQueryRepository)
    //    {
    //        _forumTopicsQueryRepository = forumTopicsQueryRepository;
    //    }
    //    public async Task<EmailTopicsResponse> Handle(GetEmailTopicList request, CancellationToken cancellationToken)
    //    {
    //        //var topicList = RoleMapper.Mapper.Map<EmailTopics>(request);

    //        //if (topicList is null)
    //        //{
    //        //    throw new ApplicationException("There is a problem in mapper");
    //        //}


    //        var newTopics = await _forumTopicsQueryRepository.GetTopicListAsync(request.type);
    //        return newTopics;
    //        //var newTopics = await _forumTopicsQueryRepository.Insert(topicList);
    //        //var customerTopicResponse = RoleMapper.Mapper.Map<EmailTopicsResponse>(newTopics);
    //        //return customerTopicResponse;
    //    }
    //}

}
