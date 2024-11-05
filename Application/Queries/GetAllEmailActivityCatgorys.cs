using Application.Queries.Base;
using Application.Response;
using Core.Entities;
using Core.Entities.Views;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetAllEmailActivityCatgorys : PagedRequest, IRequest<List<EmailActivityCatgorys>>
    {
    }
    public class GetAllTopicCategory : PagedRequest, IRequest<List<EmailActivityCatgorys>>
    {
        public long TopicId { get; set; }
        public GetAllTopicCategory(long TopicId)
        {
            this.TopicId = TopicId;
        }
    }
    public class GetActionTagMultiple : PagedRequest, IRequest<List<long>>
    {
        public long TopicId { get; set; }
        public GetActionTagMultiple(long TopicId)
        {
            this.TopicId = TopicId;
        }
    }
    public class GetTagLockInfo : IRequest<bool>
    {
        public long TopicId { get; set; }
        public GetTagLockInfo(long TopicId)
        {
            this.TopicId = TopicId;
        }
    }
    

    public class CreateEmailActivityCatgorysQuery : EmailActivityCatgorys, IRequest<long>
    {
    }
    public class EditTopicCategoryQery : EmailActivityCatgorys, IRequest<long>
    {
    }
    public class EditOtherTagQery : PagedRequest, IRequest<string>
    {
        public long modifiedByUserID { get; set; }
        public string otherTag { get; set; }
        public string Name { get; set; }
        public EditOtherTagQery(string otherTag, string Name, long modifiedByUserID)
        {
            this.otherTag = otherTag;
            this.Name = Name;
            this.modifiedByUserID = modifiedByUserID;
        }
    }
    public class EditOtherQery : PagedRequest, IRequest<string>
    {
        public long modifiedByUserID { get; set; }
        public long id { get; set; }
        public string Name { get; set; }
        public EditOtherQery(long id, string Name, long modifiedByUserID)
        {
            this.id = id;
            this.Name = Name;
            this.modifiedByUserID = modifiedByUserID;
        }
    }
    public class EditUserTagQery : PagedRequest, IRequest<string>
    {
        public string userTag { get; set; }
        public string Name { get; set; }
        public EditUserTagQery(string userTag, string name)
        {
            this.userTag = userTag;
            this.Name = name;
        }
    }
    public class DeleteTopicCategoryQery : PagedRequest, IRequest<long>
    {
        public long ID { get; set; }
        public long TopicID { get; set; }
        public DeleteTopicCategoryQery(long ID,long TopicId)
        {
            this.ID = ID;
            this.TopicID = TopicId;
        }
    }
    public class GetAllemailCategory : PagedRequest, IRequest<List<EmailActivityCatgorys>>
    {
        public long TopicId { get; set; }
        public GetAllemailCategory(long TopicId)
        {
            this.TopicId = TopicId;
        }
    }

    public class GetAllUserActivityCatgorys : PagedRequest, IRequest<List<EmailActivityCatgorys>>
    {
        public long UserID { get; set; }
        public GetAllUserActivityCatgorys(long UserID)
        {
            this.UserID = UserID;
        }
    }
    public class GetByUserTage : PagedRequest, IRequest<List<EmailActivityCatgorys>>
    {
        public long TopicID { get; set; }
        public long UserID { get; set; }
        public GetByUserTage(long topicId,long UserID)
        {
            this.TopicID = topicId;
            this.UserID = UserID;
        }
    }
    
    public class DeleteUserTagByNameQery : PagedRequest, IRequest<long>
    {
       
        public long UserID { get; set; }
        public string UserTag { get; set; }

        public DeleteUserTagByNameQery(long UserID, string UserTag)
        {            
            this.UserID = UserID;
            this.UserTag = UserTag;

        }
    }
    public class DeleteUserTagQery : PagedRequest, IRequest<long>
    {
        public long TopicID { get; set; }
        public long UserID { get; set; }
        public long UserTagID { get; set; }

        public DeleteUserTagQery(long TopicID,long UserID,long UserTag)
        {
            this.TopicID = TopicID;
            this.UserID = UserID;
            this.UserTagID = UserTag;
           
        }
    }
    public class DeleteuserallTagQery : PagedRequest, IRequest<long>
    {
        public long ID { get; set; }
        public long UserID { get; set; }
        public long Tagid { get; set; }


        public DeleteuserallTagQery(long ID, long UserID,long TagID)
        {
            this.ID = ID;
            this.UserID = UserID;
            this.Tagid = TagID;

        }
    }
    public class GetAllOthersQuery : PagedRequest, IRequest<List<EmailActivityCatgorys>>
    {
        public string OtherTag { get; set; }
        public GetAllOthersQuery(string OtherTag)
        {
            this.OtherTag = OtherTag;
        }
    }
    public class GetAllUserlistQuery : PagedRequest, IRequest<List<EmailActivityCatgorys>>
    {
        public long Usertagid { get; set; }
        public GetAllUserlistQuery(long usertagid)
        {
            this.Usertagid = usertagid;
        }
    }
    public class GetAllUserListQuery : PagedRequest, IRequest<List<EmailActivityCatgorys>>
    {
        public string UserTag { get; set; }
        public long UserID { get; set; }
        public GetAllUserListQuery(string UserTag ,long UserID)
        {
            this.UserTag = UserTag;
            this.UserID = UserID;
        }
    }
}
