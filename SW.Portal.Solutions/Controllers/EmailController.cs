using Application.Queries;
using Core.Repositories.Query;
using Infrastructure.Repository.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;

namespace SW.Portal.Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : Controller
    {
        private readonly IApplicationUserQueryRepository _applicationUserQueryRepository;
        private readonly IMediator _mediator;

        public EmailController(IMediator mediator, IApplicationUserQueryRepository applicationUserQueryRepository)
        {
            _mediator = mediator;
            _applicationUserQueryRepository = applicationUserQueryRepository;
        }


        [HttpGet("GetEmailList")]
        public async Task<IActionResult> GetList(string mode, long UserId)
        {
            List<Core.Entities.EmailTopics> result = null;

            if (mode == "To")
            {
                result = await _mediator.Send(new GetEmailTopicTo(UserId));
            }
            else if (mode == "CC")
            {
                result = await _mediator.Send(new GetEmailTopicCC(UserId));
            }
            else if (mode == "Sent")
            {
                result = await _mediator.Send(new GetSentTopic(UserId));
            }
            else if (mode == "All")
            {
                result = await _mediator.Send(new GetEmailTopicAll(UserId));
            }
            else
            {
                result = new List<Core.Entities.EmailTopics>();
            }

            return Ok(result);
        }
        [HttpGet("GetSubEmailList")]
        public async Task<IActionResult> GetSubList(string mode, long Id, long UserId)
        {
            List<Core.Entities.EmailTopics> subResult = null;

            if (mode == "To")
            {
                subResult = await _mediator.Send(new GetSubEmailTopicTo(Id, UserId));
            }
            else if (mode == "CC")
            {
                subResult = await _mediator.Send(new GetSubEmailTopicCC(Id, UserId));
            }
            else if (mode == "Sent")
            {
                subResult = await _mediator.Send(new GetSubEmailTopicSent(Id, UserId));
            }
            else if (mode == "All")
            {
                subResult = await _mediator.Send(new GetSubEmailTopicAll(Id, UserId));
            }
            else
            {
                subResult = new List<Core.Entities.EmailTopics>();
            }

            return Ok(subResult);
        }
        [HttpGet("GetDiscussionList")]
        public async Task<IActionResult> GetEmailDiscussionList(long Id, long UserId)
        {
            var result = await _mediator.Send(new GetEmailReplyDiscussionList(Id, UserId));
            return Ok(result);
        }
    }
}
