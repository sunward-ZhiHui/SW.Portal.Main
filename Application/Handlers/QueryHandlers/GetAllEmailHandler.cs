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

        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;
        public GetUserEmailTopicListHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
           _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }
        public async Task<List<EmailTopics>> Handle(GetUserEmailTopics request, CancellationToken cancellationToken)
        {
            return await _emailTopicsQueryRepository.GetUserTopicList(request.UserId);
           
        }
    }
    public class GetEmailTopicToHandler : IRequestHandler<GetEmailTopicTo, List<EmailTopics>>
    {

        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;
        public GetEmailTopicToHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }
        public async Task<List<EmailTopics>> Handle(GetEmailTopicTo request, CancellationToken cancellationToken)
        {
            return await _emailTopicsQueryRepository.GetTopicToList(request.UserId);

        }
    }
    public class GetSubEmailTopicToHandler : IRequestHandler<GetSubEmailTopicTo, List<EmailTopics>>
    {

        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;
        public GetSubEmailTopicToHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }
        public async Task<List<EmailTopics>> Handle(GetSubEmailTopicTo request, CancellationToken cancellationToken)
        {
            return await _emailTopicsQueryRepository.GetSubTopicToList(request.TopicId,request.UserId);

        }
    }
    public class GetSubEmailTopicCCHandler : IRequestHandler<GetSubEmailTopicCC, List<EmailTopics>>
    {

        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;
        public GetSubEmailTopicCCHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }
        public async Task<List<EmailTopics>> Handle(GetSubEmailTopicCC request, CancellationToken cancellationToken)
        {
            return await _emailTopicsQueryRepository.GetSubTopicCCList(request.TopicId, request.UserId);

        }
    }
    public class GetEmailTopicCCHandler : IRequestHandler<GetEmailTopicCC, List<EmailTopics>>
    {

        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;
        public GetEmailTopicCCHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }
        public async Task<List<EmailTopics>> Handle(GetEmailTopicCC request, CancellationToken cancellationToken)
        {
            return await _emailTopicsQueryRepository.GetTopicCCList(request.UserId);

        }
    }
    public class GetSentTopicHandler : IRequestHandler<GetSentTopic, List<EmailTopics>>
    {

        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;
        public GetSentTopicHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }
        public async Task<List<EmailTopics>> Handle(GetSentTopic request, CancellationToken cancellationToken)
        {
            return await _emailTopicsQueryRepository.GetTopicSentList(request.UserId);

        }
    }

    public class GetEmailParticipantListHandler : IRequestHandler<GetEmailParticipantsList, List<EmailParticipant>>
    {

        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;
        public GetEmailParticipantListHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }
        public async Task<List<EmailParticipant>> Handle(GetEmailParticipantsList request, CancellationToken cancellationToken)
        {
            return await _emailTopicsQueryRepository.GetParticipantList(request.TopicId);
        }
    }

    public class GetByIdEmailTopicListHandler : IRequestHandler<GetByIdEmailTopics, List<EmailTopics>>
    {

        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;
        public GetByIdEmailTopicListHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }
        public async Task<List<EmailTopics>> Handle(GetByIdEmailTopics request, CancellationToken cancellationToken)
        {
            return await _emailTopicsQueryRepository.GetByIdAsync(request.ID);

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
        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;

        public UpdateEmailTopicDueDateHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;

        }

        public async Task<long> Handle(UpdateEmailTopicDueDate request, CancellationToken cancellationToken)
        {
            var req = await _emailTopicsQueryRepository.UpdateDueDate(request);
            return req;
        }
    }
    public class UpdateEmailTopicClosedHandler : IRequestHandler<UpdateEmailTopicClosed, long>
    {
        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;

        public UpdateEmailTopicClosedHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;

        }

        public async Task<long> Handle(UpdateEmailTopicClosed request, CancellationToken cancellationToken)
        {
            var req = await _emailTopicsQueryRepository.UpdateTopicClose(request);
            return req;
        }
    }

    public class CreateEmailParticipantHandler : IRequestHandler<CreateEmailTopicParticipant, long>
    {
        private readonly IEmailTopicsQueryRepository _emailTopicsQueryRepository;

        public CreateEmailParticipantHandler(IEmailTopicsQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }
        public async Task<long> Handle(CreateEmailTopicParticipant request, CancellationToken cancellationToken)
        {
            //var newTopics = await _emailTopicsQueryRepository.InsertParticipant(request);            
            //return newTopics;

            var newTopics = _emailTopicsQueryRepository.Insert_sp_Participant(request);
            var customerResponse = RoleMapper.Mapper.Map<long>(newTopics);
            return customerResponse;
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
        }
    }
}
