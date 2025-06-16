using Application.Common.Mapper;
using Application.Queries;
using Core.Entities;
using Core.Entities.Views;
using Core.EntityModels;
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
    
    public class GetEmailPrintAllDiscussionListHandler : IRequestHandler<GetEmailPrintAllDiscussionList, List<EmailConversations>>
    {
        private readonly IEmailConversationsQueryRepository _emailConversationsQueryRepository;

        public GetEmailPrintAllDiscussionListHandler(IEmailConversationsQueryRepository emailConversationsQueryRepository)
        {

            _emailConversationsQueryRepository = emailConversationsQueryRepository;
        }
        public async Task<List<EmailConversations>> Handle(GetEmailPrintAllDiscussionList request, CancellationToken cancellationToken)
        {
            return (List<EmailConversations>)await _emailConversationsQueryRepository.GetEmailPrintAllDiscussionListAsync(request.TopicId, request.UserId, request.Option);
        }
    }
    public class GetNotificationCountHandler : IRequestHandler<GetEmailNotificationCount, long>
    {
        private readonly IEmailConversationsQueryRepository _emailConversationsQueryRepository;

        public GetNotificationCountHandler(IEmailConversationsQueryRepository emailConversationsQueryRepository)
        {

            _emailConversationsQueryRepository = emailConversationsQueryRepository;
        }
        public async Task<long> Handle(GetEmailNotificationCount request, CancellationToken cancellationToken)
        {
            return (long)await _emailConversationsQueryRepository.GetTotalNotificationCountAsync(request.UserId);
        }
    }

    public class GetEmailUnReadNotificationHandler : IRequestHandler<GetEmailUnReadNotification, List<EmailConversations>>
    {
        private readonly IEmailConversationsQueryRepository _emailConversationsQueryRepository;

        public GetEmailUnReadNotificationHandler(IEmailConversationsQueryRepository emailConversationsQueryRepository)
        {

            _emailConversationsQueryRepository = emailConversationsQueryRepository;
        }
        public async Task<List<EmailConversations>> Handle(GetEmailUnReadNotification request, CancellationToken cancellationToken)
        {
            return await _emailConversationsQueryRepository.GetUnReadNotificationAsync(request.UserId);
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
            return (List<EmailConversations>)await _emailConversationsQueryRepository.GetDiscussionListAsync(request.TopicId,request.UserId,request.Option);           
        }
    }
    public class GetOnReplyDiscussionListHandler : IRequestHandler<GetOnReplyDiscussionList, List<EmailConversations>>
    {
        private readonly IEmailConversationsQueryRepository _emailConversationsQueryRepository;

        public GetOnReplyDiscussionListHandler(IEmailConversationsQueryRepository emailConversationsQueryRepository)
        {

            _emailConversationsQueryRepository = emailConversationsQueryRepository;
        }
        public async Task<List<EmailConversations>> Handle(GetOnReplyDiscussionList request, CancellationToken cancellationToken)
        {
            return (List<EmailConversations>)await _emailConversationsQueryRepository.GetOnDiscussionListAsync(request.ReplyId, request.UserId);
        }
    }
    public class GetReplyListPagedHandler : IRequestHandler<GetReplyListPaged, List<EmailConversations>>
    {
        private readonly IEmailConversationsQueryRepository _emailConversationsQueryRepository;

        public GetReplyListPagedHandler(IEmailConversationsQueryRepository emailConversationsQueryRepository)
        {

            _emailConversationsQueryRepository = emailConversationsQueryRepository;
        }
        public async Task<List<EmailConversations>> Handle(GetReplyListPaged request, CancellationToken cancellationToken)
        {
            return (List<EmailConversations>)await _emailConversationsQueryRepository.GetReplyListPagedAsync(request.ReplyId, request.UserId,request.currentPage,request.pageSize);
        }
    }
    

    public class GetFileDataHandler : IRequestHandler<GetFileData, byte[]>
    {
        private readonly IEmailConversationsQueryRepository _emailConversationsQueryRepository;

        public GetFileDataHandler(IEmailConversationsQueryRepository emailConversationsQueryRepository)
        {

            _emailConversationsQueryRepository = emailConversationsQueryRepository;
        }
        public async Task<byte[]> Handle(GetFileData request, CancellationToken cancellationToken)
        {
            return (byte[])await _emailConversationsQueryRepository.GetFileDataAsync(request.ID);
        }
    }


    public class GetReplyDiscussionListHandler : IRequestHandler<GetReplyDiscussionList, List<EmailConversations>>
    {
        private readonly IEmailConversationsQueryRepository _emailConversationsQueryRepository;

        public GetReplyDiscussionListHandler(IEmailConversationsQueryRepository emailConversationsQueryRepository)
        {

            _emailConversationsQueryRepository = emailConversationsQueryRepository;
        }
        public async Task<List<EmailConversations>> Handle(GetReplyDiscussionList request, CancellationToken cancellationToken)
        {
            return (List<EmailConversations>)await _emailConversationsQueryRepository.GetByReplyDiscussionList(request.ReplyId);
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
    public class OnReplyConversationHandler : IRequestHandler<OnReplyConversationTopic, OnReplyEmailTopic>
    {
        private readonly IEmailConversationsQueryRepository _emailConversationsQueryRepository;
        private readonly IEmployeeQueryRepository _employeeQueryRepository;
        private readonly IFileprofileQueryRepository _fileprofileQueryRepository;
        public OnReplyConversationHandler(IEmailConversationsQueryRepository emailConversationsQueryRepository, IEmployeeQueryRepository employeeQueryRepository, IFileprofileQueryRepository fileprofileQueryRepository)
        {

            _emailConversationsQueryRepository = emailConversationsQueryRepository;
            _employeeQueryRepository = employeeQueryRepository;
            _fileprofileQueryRepository = fileprofileQueryRepository;
        }
        public async Task<OnReplyEmailTopic> Handle(OnReplyConversationTopic request, CancellationToken cancellationToken)
        {
           
            var ToIds = new List<long>();
            var CcIds = new List<long>();            
            //var _Toparticipant = new List<string>();
            //var _CCparticipant = new List<string>();
            var _allparticipant = new List<long> ();
            var _allparticipants = new List<ViewEmployee>();
            var t3 = new List<long>();

            var _participantUserGroup = new List<UserGroup> ();

            var _replyComment = await _emailConversationsQueryRepository.GetReplyDiscussionListAsync(request.ID, request.UserId);

            var isUserType = _replyComment[0].UserType;

            if (string.IsNullOrEmpty(isUserType) || isUserType == "Users")
            {
                var plist = await _employeeQueryRepository.GetAllUserAsync();
                var allplist = plist.Where(c => c.UserID != request.UserId).ToList();
                var gettids = await _emailConversationsQueryRepository.GetConversationListAsync(request.ID);

                var getplllst = await _emailConversationsQueryRepository.GetAllConvTopicPListAsync(request.ID, gettids[0].TopicID);
                var parttcc = getplllst.Select(p => p.UserID).Where(userId => userId.HasValue).Select(userId => userId.Value).ToList();
                _allparticipants = allplist.Where(c => c.UserID != request.UserId).ToList();


                var convlistTo = await _emailConversationsQueryRepository.GetConversationAssignToList(_replyComment[0].ID);
                var conto = convlistTo.Where(c => c.UserId != request.UserId).ToList();
                //ToIds = conto.Select(s => s.UserId).ToList();

                List<long> updatedList = conto.Select(s => s.UserId).ToList();
                long fromUserId = _replyComment[0].UserId.Value;

                if (fromUserId != request.UserId)
                {
                    updatedList.Add(fromUserId);
                }

                ToIds = new List<long>();

                //cclist
                var convlistCC = await _emailConversationsQueryRepository.GetConversationAssignCCList(_replyComment[0].ID);
                var concc = convlistCC.Where(c => c.UserId != request.UserId).ToList();
                //CcIds = concc.Select(s => s.UserId).ToList();

                var t11 = parttcc.Where(userId => updatedList.Any(s => s == userId)).ToList();
                var t21 = parttcc.Where(userId => concc.Any(s => s.UserId == userId)).ToList();

                //vk
                //CcIds = t11.Concat(t21).ToList();
                CcIds = parttcc;

                var t1 = ToIds;
                var t2 = CcIds;
                t3 = t1.Concat(t2).ToList();
                //vk
               // t3.Add(request.UserId);
            }
            else
            {
                _participantUserGroup = await _fileprofileQueryRepository.GetAllUserGroups();


                var listTo = await _emailConversationsQueryRepository.GetAssignCCUserGroupList(_replyComment[0].ID);
                ToIds = listTo.Select(s => s.GroupId).ToList();


                //var filteredToList = _participantUserGroup.Where(c => ToIds.Contains(c.UserGroupId)).ToList();
                //IEnumerable<string> Totags = filteredToList.Select(s => s.Name).ToList();
                //_Toparticipant = Totags.ToList();


                //var filteredCCList = _participantUserGroup.Where(c => CcIds.Contains(c.UserGroupId)).ToList();
                //IEnumerable<string> CCtags = filteredCCList.Select(s => s.Name).ToList();
                //_CCparticipant = CCtags.ToList();
                
                t3 = ToIds;
            }




            var conversation = new OnReplyEmailTopic();
                conversation.ID = (int)request.ID;
                conversation.ToIds = ToIds;
                conversation.CcIds = CcIds;             
                conversation.allparticipant = t3;                
                conversation._allparticipants = _allparticipants.Select(employee => new ViewEmployeeModel { 
                                                UserID = employee.UserID,
                                                Name = $"{employee.FirstName} {employee.LastName} {employee.NickName}",
                                                EmployeeID = employee.EmployeeID,
                                                FirstName = employee.FirstName,                                               
                                                LoginID = employee.LoginID
                                                }).ToList();
            return conversation;           
        }
    }
    public class OndropdownHandler : IRequestHandler<OnReplyDropDown, OnReplyEmailTopic>
    {
        private readonly IEmailConversationsQueryRepository _emailConversationsQueryRepository;
        private readonly IEmployeeQueryRepository _employeeQueryRepository;
        private readonly IFileprofileQueryRepository _fileprofileQueryRepository;
        public OndropdownHandler(IEmailConversationsQueryRepository emailConversationsQueryRepository, IEmployeeQueryRepository employeeQueryRepository, IFileprofileQueryRepository fileprofileQueryRepository)
        {

            _emailConversationsQueryRepository = emailConversationsQueryRepository;
            _employeeQueryRepository = employeeQueryRepository;
            _fileprofileQueryRepository = fileprofileQueryRepository;
        }
        public async Task<OnReplyEmailTopic> Handle(OnReplyDropDown request, CancellationToken cancellationToken)
        {
            List<long> ToIds = request.ToIds.Split(',')
                                .Select(long.Parse)
                                .ToList();
           
            var CcIds = request.CcIds.Split(',')
                                .Select(long.Parse)
                                .ToList();
            var _ccallparticipants = new List<ViewEmployee>();
            var _toallparticipants = new List<ViewEmployee>();



            var Plist = await _employeeQueryRepository.GetAllUserAsync();
                var cclistDropdown = Plist.Where(c => CcIds.Contains(c.UserID.Value)).ToList();
               // var CCresult = Plist.Where(c => c.UserID != cclistDropdown[0].UserID).ToList();
            var CCresult = Plist.Except(cclistDropdown).ToList();




           
            var tolistdropdown = Plist.Where(c => ToIds.Contains(c.UserID.Value)).ToList();
           // var Toresult = Plist.Where(c => c.UserID != tolistdropdown[0].UserID).ToList();
            var Toresult = Plist.Except(tolistdropdown).ToList();
            _toallparticipants = Toresult.ToList();
            _ccallparticipants = CCresult.ToList();



            var conversation = new OnReplyEmailTopic();
            // var CCconversation = new OnReplyEmailTopic();
            //  conversation.ToIds = ToIds;
            // conversation.CcIds = CcIds;

            conversation._Toallparticipants = _toallparticipants.Select(toemployee => new ViewEmployeeModel
            {
                UserID = toemployee.UserID,
                Name = $"{toemployee.FirstName} {toemployee.LastName} {toemployee.NickName}",
                EmployeeID = toemployee.EmployeeID,
                FirstName = toemployee.FirstName
            }).ToList();

            conversation._CCallparticipants = _ccallparticipants.Select(ccemployee => new ViewEmployeeModel
            {
                UserID = ccemployee.UserID,
                Name = $"{ccemployee.FirstName} {ccemployee.LastName} {ccemployee.NickName}",
                EmployeeID = ccemployee.EmployeeID,
                FirstName = ccemployee.FirstName               
               
            }).ToList();
            return conversation;
           
        }
    }
    public class OnReplyConversationHandler_old : IRequestHandler<OnReplyConversation, OnReplyEmail>
    {
        private readonly IEmailConversationsQueryRepository _emailConversationsQueryRepository;
        private readonly IEmployeeQueryRepository _employeeQueryRepository;
        public OnReplyConversationHandler_old(IEmailConversationsQueryRepository emailConversationsQueryRepository, IEmployeeQueryRepository employeeQueryRepository)
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
    public class GetEmailPrintReplyDiscussionListHandler : IRequestHandler<GetEmailPrintReplyDiscussionList, List<EmailConversations>>
    {
        private readonly IEmailConversationsQueryRepository _emailConversationsQueryRepository;

        public GetEmailPrintReplyDiscussionListHandler(IEmailConversationsQueryRepository emailConversationsQueryRepository)
        {

            _emailConversationsQueryRepository = emailConversationsQueryRepository;
        }
        public async Task<List<EmailConversations>> Handle(GetEmailPrintReplyDiscussionList request, CancellationToken cancellationToken)
        {
            return (List<EmailConversations>)await _emailConversationsQueryRepository.GetPrintReplyDiscussionListAsync(request.TopicId, request.UserId);
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
            return (List<Documents>)await _conversationQueryRepository.GetTopicDocListAsync(request.TopicId,request.UserId,request.Option);
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
           // var DeleteCopyEmail = await _conversationQueryRepository.DeleteCopyEmail(req);
             EmailTopics data = new EmailTopics();
            data.ID = req;
            data.CopyEmailIds = request.CopyEmailIds;
            data.SessionId = request.SessionId;
            data.AddedByUserID = request.AddedByUserID;
            data.AddedDate = request.AddedDate;
            var insertcopyemail = await _conversationQueryRepository.InsertCopyEmail(data);
            if (request.ReplyId == 0)
            {
                var updatereq = await _conversationQueryRepository.LastUserIDUpdate(req, request.AddedByUserID.Value);
            }
            else
            {
                var updatereq = await _conversationQueryRepository.LastUserIDUpdate(request.ReplyId, request.AddedByUserID.Value);
            }

            if(request.IsDueDate == 1)
            {
                var updateDueDatereq = await _conversationQueryRepository.UpdateDueDateReqested(request.ReplyId, request.AddedByUserID.Value,1);
            }


            var ETUpdateDate = await _conversationQueryRepository.LastUpdateDateEmailTopic(request.TopicID);

            if(request.EmailFormSectionSessionID != null)
            {
                var updateDynFrmDataSessionid = await _conversationQueryRepository.InsertEmailDynamicFormDateUploadSession(request.DynamicFormID.Value,request.EmailFormDataSessionID.Value, request.EmailFormSectionSessionID.Value,request.SessionId.Value);
                var docInsert = await _conversationQueryRepository.DocInsertDynamicFormDateUpload(request.EmailFormDataSessionID.Value, request.EmailFormSectionSessionID.Value,request.SessionId.Value,request.AddedByUserID.Value);
            }


            if (string.IsNullOrEmpty(request.UserType) || request.UserType == "Users")
            {
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

                var plistData = request.AllParticipantIds.ToList();
                if (plistData.Count > 0)
                {
                    request.AllParticipantIds.ToList().ForEach(async a =>
                    {
                        var forumNotifications = new EmailNotifications();
                        forumNotifications.ConversationId = req;
                        forumNotifications.TopicId = request.TopicID;
                        forumNotifications.UserId = a;
                        forumNotifications.AddedByUserID = request.AddedByUserID;
                        forumNotifications.AddedDate = request.AddedDate;
                        forumNotifications.IsRead = request.AddedByUserID == a ? true : false;
                        await _conversationQueryRepository.InsertEmailNotifications(forumNotifications);
                    });
                }

            }
            else
            {
                var AssignUserGroup = new EmailConversationAssignToUserGroup();
                AssignUserGroup.ConversationId = req;
                AssignUserGroup.ReplyId = request.ReplyId;
                AssignUserGroup.AllowPlistids = request.AllowPlistids;
                AssignUserGroup.PlistIdss = request.ParticipantsUserGroup;                
                AssignUserGroup.TopicId = request.TopicID;                
                AssignUserGroup.AddedByUserID = request.AddedByUserID;                
                AssignUserGroup.AddedDate = request.AddedDate;
                AssignUserGroup.AssigntoIds = request.ToUserGroup;
                AssignUserGroup.AssignccIds = request.CCUserGroup;
                AssignUserGroup.ConIds = request.ConIds;
                var reqq = await _conversationQueryRepository.InsertAssignToUserGroup_sp(AssignUserGroup);

                var GroupUserIdsList = await _conversationQueryRepository.GetGroupByUserIdList(request.ParticipantsUserGroup, request.TopicID);

                if (GroupUserIdsList.Count > 0)
                {
                    GroupUserIdsList.ForEach(async a =>
                    {
                        var forumNotifications = new EmailNotifications();
                        forumNotifications.ConversationId = req;
                        forumNotifications.TopicId = request.TopicID;
                        forumNotifications.UserId = a;
                        forumNotifications.AddedByUserID = request.AddedByUserID;
                        forumNotifications.AddedDate = request.AddedDate;
                        forumNotifications.IsRead = request.AddedByUserID == a ? true : false;
                        await _conversationQueryRepository.InsertEmailNotifications(forumNotifications);
                    });
                }

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
    public class GetListByConversationSessionHandler : IRequestHandler<GetListByConversationSession, List<EmailConversations>>
    {

        private readonly IEmailConversationsQueryRepository _emailConversationsQueryRepository;
        public GetListByConversationSessionHandler(IEmailConversationsQueryRepository emailConversationsQueryRepository)
        {
            _emailConversationsQueryRepository = emailConversationsQueryRepository;
        }
        public async Task<List<EmailConversations>> Handle(GetListByConversationSession request, CancellationToken cancellationToken)
        {
            return await _emailConversationsQueryRepository.GetBySessionConversationList(request.SessionId);

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
    public class GetConvasationplistUserGroupHandler : IRequestHandler<GetByConvasationPUserGroupList, List<long>>
    {
        private readonly IEmailConversationsQueryRepository _conversationQueryRepository;
        public GetConvasationplistUserGroupHandler(IEmailConversationsQueryRepository conversationQueryRepository)
        {
            _conversationQueryRepository = conversationQueryRepository;
        }
        public async Task<List<long>> Handle(GetByConvasationPUserGroupList request, CancellationToken cancellationToken)
        {
            return (List<long>)await _conversationQueryRepository.GetConvPListUserGroupAsync(request.ConvasationId);
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
    public class GetByIdConversationListHandler : IRequestHandler<GetByIdConversation, EmailConversations>
    {
        private readonly IEmailConversationsQueryRepository _conversationQueryRepository;
        public GetByIdConversationListHandler(IEmailConversationsQueryRepository conversationQueryRepository)
        {
            _conversationQueryRepository = conversationQueryRepository;
        }
        public async Task<EmailConversations> Handle(GetByIdConversation request, CancellationToken cancellationToken)
        {
            return (EmailConversations)await _conversationQueryRepository.GetByIdAsync(request.Id);
        }
    }
    public class GetUserTokenListQueryHandler : IRequestHandler<GetUserTokenListQuery, List<UserNotification>>
    {
        private readonly IEmailConversationsQueryRepository _conversationQueryRepository;
        public GetUserTokenListQueryHandler(IEmailConversationsQueryRepository conversationQueryRepository)
        {
            _conversationQueryRepository = conversationQueryRepository;
        }
        public async Task<List<UserNotification>> Handle(GetUserTokenListQuery request, CancellationToken cancellationToken)
        {
            return (List<UserNotification>)await _conversationQueryRepository.GetUserTokenListAsync(request.UserId);
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
    public class GetAssignCCUserGroupHandler : IRequestHandler<GetAssignCCUserGroup, List<EmailConversationAssignToUserGroup>>
    {
        private readonly IEmailConversationsQueryRepository _conversationQueryRepository;
        public GetAssignCCUserGroupHandler(IEmailConversationsQueryRepository conversationQueryRepository)
        {
            _conversationQueryRepository = conversationQueryRepository;
        }
        public async Task<List<EmailConversationAssignToUserGroup>> Handle(GetAssignCCUserGroup request, CancellationToken cancellationToken)
        {
            return (List<EmailConversationAssignToUserGroup>)await _conversationQueryRepository.GetAssignCCUserGroupList(request.ConversationId);
        }
    }
    public class GetEmailParticipantHandler : IRequestHandler<GetEmailParticipantList, List<EmailTopics>>
    {
        private readonly IEmailConversationsQueryRepository _conversationQueryRepository;
        public GetEmailParticipantHandler(IEmailConversationsQueryRepository conversationQueryRepository)
        {
            _conversationQueryRepository = conversationQueryRepository;
        }
        public async Task<List<EmailTopics>> Handle(GetEmailParticipantList request, CancellationToken cancellationToken)
        {
            return (List<EmailTopics>)await _conversationQueryRepository.GetEmailParticipantListAsync(request.ConversationID,request.Userid);
        }
    }
    public class GetEmailCloseHandler : IRequestHandler<GetEmailClosedQuery, List<EmailTopics>>
    {
        private readonly IEmailConversationsQueryRepository _conversationQueryRepository;
        public GetEmailCloseHandler(IEmailConversationsQueryRepository conversationQueryRepository)
        {
            _conversationQueryRepository = conversationQueryRepository;
        }
        public async Task<List<EmailTopics>> Handle(GetEmailClosedQuery request, CancellationToken cancellationToken)
        {
            return (List<EmailTopics>)await _conversationQueryRepository.UpdateEmailCloseAsync(request.ConversationID, request.Userid,request.Isclose);
        }
    }
}