using Application.Common.Mapper;
using Application.Queries;
using Core.Entities;
using Core.Entities.Views;
using Core.Repositories.Command.Base;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.QueryHandlers
{
    public class GetAllEmailConversationHandler : IRequestHandler<GetAllEmailConversation, List<EmailConversations>>
    {

        private readonly IQueryRepository<EmailConversations> _queryRepository;
        public GetAllEmailConversationHandler(IQueryRepository<EmailConversations> queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<EmailConversations>> Handle(GetAllEmailConversation request, CancellationToken cancellationToken)
        {
            return (List<EmailConversations>)await _queryRepository.GetListAsync();
            //return (List<EmailTypes>)await _roleQueryRepository.GetAllAsync();
        }
    }
    //Get discussion list
    public class GetEmailDiscussionListHandler : IRequestHandler<GetEmailDiscussionList, List<EmailConversations>>
    {
        private readonly IEmailConversationsQueryRepository _emailConversationsQueryRepository;
        
        public GetEmailDiscussionListHandler(IEmailConversationsQueryRepository emailConversationsQueryRepository)
        {

            _emailConversationsQueryRepository = emailConversationsQueryRepository;
        }
        public async Task<List<EmailConversations>> Handle(GetEmailDiscussionList request, CancellationToken cancellationToken)
        {
            return (List<EmailConversations>)await _emailConversationsQueryRepository.GetDiscussionListAsync(request.TopicId,request.UserId);           
        }
    }
    public class GetDemoEmailFileDataListHandler : IRequestHandler<GetDemoEmailFileDataList, List<EmailConversations>>
    {
        private readonly IEmailConversationsQueryRepository _emailConversationsQueryRepository;

        public GetDemoEmailFileDataListHandler(IEmailConversationsQueryRepository emailConversationsQueryRepository)
        {

            _emailConversationsQueryRepository = emailConversationsQueryRepository;
        }
        public async Task<List<EmailConversations>> Handle(GetDemoEmailFileDataList request, CancellationToken cancellationToken)
        {
            return (List<EmailConversations>)await _emailConversationsQueryRepository.GetDemoEmailFileDataListAsync();
        }
    }
    public class GetDemoUpdateEmailFileDataListHandler : IRequestHandler<GetDemoUpdateEmailFileDataList, long>
    {
        private readonly IEmailConversationsQueryRepository _emailConversationsQueryRepository;

        public GetDemoUpdateEmailFileDataListHandler(IEmailConversationsQueryRepository emailConversationsQueryRepository)
        {
            _emailConversationsQueryRepository = emailConversationsQueryRepository;
        }
        public async Task<long> Handle(GetDemoUpdateEmailFileDataList request, CancellationToken cancellationToken)
        {
            return (long)await _emailConversationsQueryRepository.GetDemoUpdateEmailFileDataListAsync(request.id,request.fileData);
        }
    }

    public class GetEmailValidUserListHandler : IRequestHandler<GetEmailValidUserList, List<EmailConversations>>
    {
        private readonly IEmailConversationsQueryRepository _emailConversationsQueryRepository;

        public GetEmailValidUserListHandler(IEmailConversationsQueryRepository emailConversationsQueryRepository)
        {

            _emailConversationsQueryRepository = emailConversationsQueryRepository;
        }
        public async Task<List<EmailConversations>> Handle(GetEmailValidUserList request, CancellationToken cancellationToken)
        {
            return (List<EmailConversations>)await _emailConversationsQueryRepository.GetValidUserListAsync(request.TopicId, request.UserId);
        }
    }
    //Get Conversation list
    public class GetEmailConversationListHandler : IRequestHandler<GetEmailConversationList, List<EmailConversations>>
    {
        private readonly IEmailConversationsQueryRepository _emailConversationsQueryRepository;

        public GetEmailConversationListHandler(IEmailConversationsQueryRepository emailConversationsQueryRepository)
        {

            _emailConversationsQueryRepository = emailConversationsQueryRepository;
        }
        public async Task<List<EmailConversations>> Handle(GetEmailConversationList request, CancellationToken cancellationToken)
        {
            return (List<EmailConversations>)await _emailConversationsQueryRepository.GetConversationListAsync(request.ID);
        }
    }
    public class OnReplyConversationHandler : IRequestHandler<OnReplyConversation, OnReplyEmail>
    {
        private readonly IEmailConversationsQueryRepository _emailConversationsQueryRepository;
        private readonly IEmployeeQueryRepository _employeeQueryRepository;
        public OnReplyConversationHandler(IEmailConversationsQueryRepository emailConversationsQueryRepository, IEmployeeQueryRepository employeeQueryRepository)
        {

            _emailConversationsQueryRepository = emailConversationsQueryRepository;
            _employeeQueryRepository = employeeQueryRepository;
        }
        public async Task<OnReplyEmail> Handle(OnReplyConversation request, CancellationToken cancellationToken)
        {
            var _allparticipant = new List<ViewEmployee>(); 
            var AssignccIds = new List<long>(); 
            var SelectedCCIds = new List<string>(); 
            var _Toparticipant = new List<ViewEmployee>(); 
            var _CCparticipant = new List<ViewEmployee>(); 
            var _allparticipants = new List<ViewEmployee>();


            var _replyComment = await _emailConversationsQueryRepository.GetReplyDiscussionListAsync(request.ID, request.UserId);
            if (_replyComment[0].IsAllowParticipants.GetValueOrDefault(false))
            {
                var plist = await _employeeQueryRepository.GetAllUserAsync();
                var allplist = plist.Where(c => c.UserID != request.UserId).ToList();

                var gettids = await _emailConversationsQueryRepository.GetConversationListAsync(request.ID);
                var getplllst = await _emailConversationsQueryRepository.GetAllConvTopicPListAsync(request.ID, gettids[0].TopicID);                
                var parttcc = getplllst.Select(p => p.UserID).Where(userId => userId.HasValue).Select(userId => userId.Value).ToList();

                #region AssignTo
                /*Assignto*/
                var convlistTo = await _emailConversationsQueryRepository.GetConversationAssignToList(_replyComment[0].ID);               
                var conto = convlistTo.Where(c => c.UserId != request.UserId).ToList();                
                List<long> updatedList = conto.Select(s => s.UserId).ToList();
                long fromUserId = _replyComment[0].UserId.Value;

                if (fromUserId != request.UserId)
                {
                    updatedList.Add(fromUserId);
                   _allparticipant = allplist.Where(c => c.UserID != request.UserId).ToList();
                   _Toparticipant = _allparticipant.ToList();
                   _CCparticipant = _allparticipant.ToList();
                   _allparticipants = _allparticipant.ToList();                    
                }
                else
                {
                   _allparticipant = allplist.Where(c => c.UserID != _replyComment[0].UserId.Value).ToList();
                   _Toparticipant = _allparticipant.ToList();
                   _CCparticipant = _allparticipant.ToList();
                   _allparticipants = _allparticipant.ToList();
                }                
                //Data.AssigntoIds = new List<long>();
                #endregion

                #region AssignCC
                /*AssignCC*/
                var convlistCC = await _emailConversationsQueryRepository.GetConversationAssignCCList(_replyComment[0].ID);                 
                var concc = convlistCC.Where(c => c.UserId != request.UserId).ToList();
                var t1 = parttcc.Where(userId => updatedList.Any(s => s == userId)).ToList();
                var t2 = parttcc.Where(userId => concc.Any(s => s.UserId == userId)).ToList();
                AssignccIds = t1.Concat(t2).ToList();

                var filteredCCList = _allparticipant.Where(c => AssignccIds.Contains(c.UserID.Value)).ToList();
                IEnumerable<string> CCtags = filteredCCList.Select(s => s.Name + "-" + s.NickName).ToList();
                SelectedCCIds = CCtags.ToList();


                _Toparticipant = _Toparticipant.Where(c => !AssignccIds.Select(id => (long?)id).Contains(c.UserID)).ToList();
                #endregion


                var conversation = new OnReplyEmail();
                conversation.ID = (int)request.ID;
                conversation.ToList = _Toparticipant;
                conversation.CCList = _CCparticipant;
                conversation.Allparticipant = _allparticipant;
                conversation.Allparticipants = _allparticipants;
                conversation.SelectedCCIds = SelectedCCIds;

                return conversation;

            }
            else
            {
                var gettid = await _emailConversationsQueryRepository.GetAllConvTPListAsync(_replyComment[0].TopicID);                
                var allplist = gettid;
                var gettids = await _emailConversationsQueryRepository.GetAllConvTPListAsync(_replyComment[0].TopicID);
                var getpatlist = gettids.Select(p => p.UserID).Where(userId => userId.HasValue).Select(userId => userId.Value).ToList();


                #region AssignTo
                /*Assignto*/
                var convlistTo = await _emailConversationsQueryRepository.GetConversationAssignToList(_replyComment[0].ID);                
                var conto = convlistTo.Where(c => c.UserId != request.UserId).ToList();
                List<long> updatedList = conto.Select(s => s.UserId).ToList();
                long fromUserId = _replyComment[0].AddedByUserID.Value;

                if (fromUserId != request.UserId)
                {
                    updatedList.Add(fromUserId);
                   _allparticipant = allplist.Where(c => c.UserID != request.UserId).ToList();
                   _Toparticipant = _allparticipant.ToList();
                   _CCparticipant = _allparticipant.ToList();
                   _allparticipants = _allparticipant.ToList();                   
                }
                else
                {
                   _allparticipant = allplist.Where(c => c.UserID != _replyComment[0].AddedByUserID.Value).ToList();
                   _Toparticipant = _allparticipant.ToList();
                   _CCparticipant = _allparticipant.ToList();
                   _allparticipants = _allparticipant.ToList();                   
                }                
                //Data.AssigntoIds = new List<long>();

                #endregion


                #region AssignCC
                var convlistCC = await _emailConversationsQueryRepository.GetConversationAssignCCList(_replyComment[0].ID);                
                var concc = convlistCC.Where(c => c.UserId != request.UserId).ToList();
                
                var t1 = getpatlist.Where(userId => updatedList.Any(s => s == userId)).ToList();
                var t2 = getpatlist.Where(userId => concc.Any(s => s.UserId == userId)).ToList();

                AssignccIds = t1.Concat(t2).ToList();

                var filteredCCList = _allparticipant.Where(c => AssignccIds.Contains(c.UserID.Value)).ToList();
                IEnumerable<string> CCtags = filteredCCList.Select(s => s.Name + "-" + s.NickName).ToList();
                SelectedCCIds = CCtags.ToList();

                _Toparticipant = _Toparticipant.Where(c => !AssignccIds.Select(id => (long?)id).Contains(c.UserID)).ToList();

                #endregion


                var conversation = new OnReplyEmail();
                conversation.ID = (int)request.ID;
                conversation.ToList = _Toparticipant;
                conversation.CCList = _CCparticipant;
                conversation.Allparticipant = _allparticipant;
                conversation.Allparticipants = _allparticipants;
                conversation.SelectedCCIds = SelectedCCIds;

                return conversation;
            }
        }
    }
    public class GetTopConversationListHandler : IRequestHandler<GetTopConversationList, List<EmailConversations>>
    {
        private readonly IEmailConversationsQueryRepository _emailConversationsQueryRepository;

        public GetTopConversationListHandler(IEmailConversationsQueryRepository emailConversationsQueryRepository)
        {

            _emailConversationsQueryRepository = emailConversationsQueryRepository;
        }
        public async Task<List<EmailConversations>> Handle(GetTopConversationList request, CancellationToken cancellationToken)
        {
            return (List<EmailConversations>)await _emailConversationsQueryRepository.GetTopConversationListAsync(request.TopicId);
        }
    }
    //Get Reply discussion list
    public class GetEmailReplyDiscussionListHandler : IRequestHandler<GetEmailReplyDiscussionList, List<EmailConversations>>
    {
        private readonly IEmailConversationsQueryRepository _emailConversationsQueryRepository;

        public GetEmailReplyDiscussionListHandler(IEmailConversationsQueryRepository emailConversationsQueryRepository)
        {

            _emailConversationsQueryRepository = emailConversationsQueryRepository;
        }
        public async Task<List<EmailConversations>> Handle(GetEmailReplyDiscussionList request, CancellationToken cancellationToken)
        {
            return (List<EmailConversations>)await _emailConversationsQueryRepository.GetReplyDiscussionListAsync(request.TopicId,request.UserId);
        }
    }
    public class GetEmailFullDiscussionListHandler : IRequestHandler<GetEmailFullDiscussionList, List<EmailConversations>>
    {
        private readonly IEmailConversationsQueryRepository _emailConversationsQueryRepository;

        public GetEmailFullDiscussionListHandler(IEmailConversationsQueryRepository emailConversationsQueryRepository)
        {

            _emailConversationsQueryRepository = emailConversationsQueryRepository;
        }
        public async Task<List<EmailConversations>> Handle(GetEmailFullDiscussionList request, CancellationToken cancellationToken)
        {
            return (List<EmailConversations>)await _emailConversationsQueryRepository.GetFullDiscussionListAsync(request.TopicId);
        }
    }
    
    public class GetEmailTopicDocListHandler : IRequestHandler<GetEmailTopicDocList, List<Documents>>
    {
        private readonly IEmailConversationsQueryRepository _conversationQueryRepository;

        public GetEmailTopicDocListHandler(IEmailConversationsQueryRepository conversationQueryRepository)
        {

            _conversationQueryRepository = conversationQueryRepository;
        }
        public async Task<List<Documents>> Handle(GetEmailTopicDocList request, CancellationToken cancellationToken)
        {
            return (List<Documents>)await _conversationQueryRepository.GetTopicDocListAsync(request.TopicId);
        }
    }
    public class GetSubEmailTopicDocListHandler : IRequestHandler<GetSubEmailTopicDocList, List<Documents>>
    {
        private readonly IEmailConversationsQueryRepository _conversationQueryRepository;

        public GetSubEmailTopicDocListHandler(IEmailConversationsQueryRepository conversationQueryRepository)
        {

            _conversationQueryRepository = conversationQueryRepository;
        }
        public async Task<List<Documents>> Handle(GetSubEmailTopicDocList request, CancellationToken cancellationToken)
        {
            return (List<Documents>)await _conversationQueryRepository.GetSubTopicDocListAsync(request.ConversationId);
        }
    }


    public class CreateEmailConversationHandler : IRequestHandler<CreateEmailCoversation, long>
    {
        private readonly IEmailConversationsQueryRepository _conversationQueryRepository;

        public CreateEmailConversationHandler(IEmailConversationsQueryRepository conversationQueryRepository)
        {
            _conversationQueryRepository = conversationQueryRepository;

        }

        public async Task<long> Handle(CreateEmailCoversation request, CancellationToken cancellationToken)
        {
            var req = await _conversationQueryRepository.Insert(request);
            var updatereq = await _conversationQueryRepository.LastUserIDUpdate(request.ReplyId,request.AddedByUserID.Value);
            
            var conversationAssignTo = new EmailConversationAssignTo();
            conversationAssignTo.ConversationId = req;
            conversationAssignTo.ReplyId = request.ReplyId;
            conversationAssignTo.PlistIdss = request.PlistIdss;
            conversationAssignTo.AllowPlistids = request.AllowPlistids;            
            conversationAssignTo.TopicId = request.TopicID;            
            conversationAssignTo.StatusCodeID = request.StatusCodeID;
            conversationAssignTo.AddedByUserID = request.AddedByUserID;
            conversationAssignTo.SessionId = request.SessionId;
            conversationAssignTo.AddedDate = request.AddedDate;
            conversationAssignTo.AssigntoIds = request.AssigntoIdss;
            conversationAssignTo.AssignccIds = request.AssignccIdss;
            conversationAssignTo.ConIds = request.ConIds;            
            var reqq = await _conversationQueryRepository.InsertAssignTo_sp(conversationAssignTo);

            //var listData = request.AssigntoIds.ToList();
            //if (listData.Count > 0)
            //{
            //    request.AssigntoIds.ToList().ForEach(a =>
            //    {
            //        var conversationAssignTo = new EmailConversationAssignTo();
            //        conversationAssignTo.ConversationId = req;
            //        conversationAssignTo.TopicId = request.TopicID;
            //        conversationAssignTo.UserId = a;
            //        conversationAssignTo.StatusCodeID = request.StatusCodeID;
            //        conversationAssignTo.AddedByUserID = request.AddedByUserID;
            //        conversationAssignTo.SessionId = request.SessionId;
            //        conversationAssignTo.AddedDate = request.AddedDate;
            //        _conversationQueryRepository.InsertAssignTo(conversationAssignTo);
            //    });
            //}

            var plistData = request.AllParticipantIds.ToList();
            if (plistData.Count > 0)
            {
                request.AllParticipantIds.ToList().ForEach(a =>
                {
                    var forumNotifications = new EmailNotifications();
                    forumNotifications.ConversationId = req;
                    forumNotifications.TopicId = request.TopicID;
                    forumNotifications.UserId = a;
                    forumNotifications.AddedByUserID = request.AddedByUserID;
                    forumNotifications.AddedDate = request.AddedDate;
                    forumNotifications.IsRead = request.AddedByUserID == a ? true:false;
                    _conversationQueryRepository.InsertEmailNotifications(forumNotifications);
                });
            }


            return req;
        }
    }
    public class EditEmailConversationHandler : IRequestHandler<EditEmailConversation, long>
    {
        private readonly IEmailConversationsQueryRepository _conversationQueryRepository;

        public EditEmailConversationHandler(IEmailConversationsQueryRepository conversationQueryRepository)
        {
            _conversationQueryRepository = conversationQueryRepository;

        }

        public async Task<long> Handle(EditEmailConversation request, CancellationToken cancellationToken)
        {
            var req = await _conversationQueryRepository.Update(request);
            return req;
        }
    }
    public class DeleteEmailConversationHandler : IRequestHandler<DeleteEmailConversation, long>
    {
        private readonly IEmailConversationsQueryRepository _conversationQueryRepository;

        public DeleteEmailConversationHandler(IEmailConversationsQueryRepository conversationQueryRepository)
        {
            _conversationQueryRepository = conversationQueryRepository;

        }

        public async Task<long> Handle(DeleteEmailConversation request, CancellationToken cancellationToken)
        {
            var req = await _conversationQueryRepository.Delete(request);
            return req;
        }
    }

    public class DeleteEmailParticipantHandler : IRequestHandler<DeleteEmailParticipant, long>
    {
        private readonly IEmailConversationsQueryRepository _conversationQueryRepository;

        public DeleteEmailParticipantHandler(IEmailConversationsQueryRepository conversationQueryRepository)
        {
            _conversationQueryRepository = conversationQueryRepository;

        }

        public async Task<long> Handle(DeleteEmailParticipant request, CancellationToken cancellationToken)
        {
            //var req = await _conversationQueryRepository.DeleteParticipant(request);
            //return req;

            var newTopics = _conversationQueryRepository.DeleteParticipant(request);
            var customerResponse = RoleMapper.Mapper.Map<long>(newTopics);
            return customerResponse;
        }
    }

    public class GetAllEmailParticipantListHandler : IRequestHandler<GetAllEmailParticipantListQuery, List<ViewEmployee>>
    {
        private readonly IEmailConversationsQueryRepository _conversationQueryRepository;
        public GetAllEmailParticipantListHandler(IEmailConversationsQueryRepository conversationQueryRepository)
        {
            _conversationQueryRepository = conversationQueryRepository;
        }
        public async Task<List<ViewEmployee>> Handle(GetAllEmailParticipantListQuery request, CancellationToken cancellationToken)
        {
            return (List<ViewEmployee>)await _conversationQueryRepository.GetAllParticipantAsync(request.TopicId);
        }
    }
    public class GetAddConversationParticipantListHandler : IRequestHandler<GetAddConversationPListQuery, List<ViewEmployee>>
    {
        private readonly IEmailConversationsQueryRepository _conversationQueryRepository;
        public GetAddConversationParticipantListHandler(IEmailConversationsQueryRepository conversationQueryRepository)
        {
            _conversationQueryRepository = conversationQueryRepository;
        }
        public async Task<List<ViewEmployee>> Handle(GetAddConversationPListQuery request, CancellationToken cancellationToken)
        {
            return (List<ViewEmployee>)await _conversationQueryRepository.GetAddConversationPListAsync(request.ConversationId);
        }
    }
    
    public class GetEmailAssignToListHandler : IRequestHandler<GetEmailAssignToList, List<EmailAssignToList>>
    {
        private readonly IEmailConversationsQueryRepository _conversationQueryRepository;
        public GetEmailAssignToListHandler(IEmailConversationsQueryRepository conversationQueryRepository)
        {
            _conversationQueryRepository = conversationQueryRepository;
        }
        public async Task<List<EmailAssignToList>> Handle(GetEmailAssignToList request, CancellationToken cancellationToken)
        {
            return (List<EmailAssignToList>)await _conversationQueryRepository.GetAllAssignToListAsync(request.TopicId);
        }
    }
    public class GetByEmailTopicPListHandler : IRequestHandler<GetByEmailTopicIDPList, List<ViewEmployee>>
    {
        private readonly IEmailConversationsQueryRepository _conversationQueryRepository;
        public GetByEmailTopicPListHandler(IEmailConversationsQueryRepository conversationQueryRepository)
        {
            _conversationQueryRepository = conversationQueryRepository;
        }
        public async Task<List<ViewEmployee>> Handle(GetByEmailTopicIDPList request, CancellationToken cancellationToken)
        {
            return (List<ViewEmployee>)await _conversationQueryRepository.GetAllPListAsync(request.TopicId);
        }
    }
    public class GetConvasationTopicIDHandler : IRequestHandler<GetByConvasationTopicIDPList, List<ViewEmployee>>
    {
        private readonly IEmailConversationsQueryRepository _conversationQueryRepository;
        public GetConvasationTopicIDHandler(IEmailConversationsQueryRepository conversationQueryRepository)
        {
            _conversationQueryRepository = conversationQueryRepository;
        }
        public async Task<List<ViewEmployee>> Handle(GetByConvasationTopicIDPList request, CancellationToken cancellationToken)
        {
            return (List<ViewEmployee>)await _conversationQueryRepository.GetAllConvTopicPListAsync(request.ConvasationId, request.TopicId);
        }
    }
    public class GetConvasationTIDHandler : IRequestHandler<GetByConvasationTIDPList, List<ViewEmployee>>
    {
        private readonly IEmailConversationsQueryRepository _conversationQueryRepository;
        public GetConvasationTIDHandler(IEmailConversationsQueryRepository conversationQueryRepository)
        {
            _conversationQueryRepository = conversationQueryRepository;
        }
        public async Task<List<ViewEmployee>> Handle(GetByConvasationTIDPList request, CancellationToken cancellationToken)
        {
            return (List<ViewEmployee>)await _conversationQueryRepository.GetAllConvTPListAsync(request.TopicId);
        }
    }

    public class GetConvasationplistHandler : IRequestHandler<GetByConvasationPList, List<ViewEmployee>>
    {
        private readonly IEmailConversationsQueryRepository _conversationQueryRepository;
        public GetConvasationplistHandler(IEmailConversationsQueryRepository conversationQueryRepository)
        {
            _conversationQueryRepository = conversationQueryRepository;
        }
        public async Task<List<ViewEmployee>> Handle(GetByConvasationPList request, CancellationToken cancellationToken)
        {
            return (List<ViewEmployee>)await _conversationQueryRepository.GetConvPListAsync(request.ConvasationId);
        }
    }

    public class GetAllConvAssToListHandler : IRequestHandler<GetAllConvAssToListQuery, List<ViewEmployee>>
    {
        private readonly IEmailConversationsQueryRepository _conversationQueryRepository;
        public GetAllConvAssToListHandler(IEmailConversationsQueryRepository conversationQueryRepository)
        {
            _conversationQueryRepository = conversationQueryRepository;
        }
        public async Task<List<ViewEmployee>> Handle(GetAllConvAssToListQuery request, CancellationToken cancellationToken)
        {
            return (List<ViewEmployee>)await _conversationQueryRepository.GetAllConvAssignToListAsync(request.EmployeeID);
        }
    }
    public class GetEmailTopicToListHandler : IRequestHandler<GetEmailTopicToList, List<EmailTopicTo>>
    {
        private readonly IEmailConversationsQueryRepository _conversationQueryRepository;
        public GetEmailTopicToListHandler(IEmailConversationsQueryRepository conversationQueryRepository)
        {
            _conversationQueryRepository = conversationQueryRepository;
        }
        public async Task<List<EmailTopicTo>> Handle(GetEmailTopicToList request, CancellationToken cancellationToken)
        {
            return (List<EmailTopicTo>)await _conversationQueryRepository.GetTopicToListAsync(request.TopicId);
        }
    }
    public class GetEmailConversationAssignToHandler : IRequestHandler<GetEmailConversationAssignTo, List<EmailConversationAssignTo>>
    {
        private readonly IEmailConversationsQueryRepository _conversationQueryRepository;
        public GetEmailConversationAssignToHandler(IEmailConversationsQueryRepository conversationQueryRepository)
        {
            _conversationQueryRepository = conversationQueryRepository;
        }
        public async Task<List<EmailConversationAssignTo>> Handle(GetEmailConversationAssignTo request, CancellationToken cancellationToken)
        {
            return (List<EmailConversationAssignTo>)await _conversationQueryRepository.GetConversationAssignToList(request.ConversationId);
        }
    }

    public class GetEmailConversationTIdHandler : IRequestHandler<GetEmailConversationTId, List<EmailConversations>>
    {
        private readonly IEmailConversationsQueryRepository _conversationQueryRepository;
        public GetEmailConversationTIdHandler(IEmailConversationsQueryRepository conversationQueryRepository)
        {
            _conversationQueryRepository = conversationQueryRepository;
        }
        public async Task<List<EmailConversations>> Handle(GetEmailConversationTId request, CancellationToken cancellationToken)
        {
            return (List<EmailConversations>)await _conversationQueryRepository.GetConversationTopicIdList(request.TopicId);
        }
    }
    public class GetEmailConversationAssignCCHandler : IRequestHandler<GetEmailConversationAssignCC, List<EmailConversationAssignTo>>
	{
		private readonly IEmailConversationsQueryRepository _conversationQueryRepository;
		public GetEmailConversationAssignCCHandler(IEmailConversationsQueryRepository conversationQueryRepository)
		{
			_conversationQueryRepository = conversationQueryRepository;
		}
		public async Task<List<EmailConversationAssignTo>> Handle(GetEmailConversationAssignCC request, CancellationToken cancellationToken)
		{
			return (List<EmailConversationAssignTo>)await _conversationQueryRepository.GetConversationAssignCCList(request.ConversationId);
		}
	}
}