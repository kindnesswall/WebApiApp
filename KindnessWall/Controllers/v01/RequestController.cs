#region using

using System.Linq;
using System.Web.Http;
using AutoMapper;
using KindnessWall.Dto.Request;
using KindnessWall.Enums;
using KindnessWall.Helper;
using KindnessWall.Models;
using Microsoft.AspNet.Identity;

#endregion

namespace KindnessWall.Controllers.v01
{
    public class RequestController : BaseApiController
    {
        [Authorize]
        [Route("api/v01/SentRequestList/{startIndex}/{lastIndex}")]
        public IHttpActionResult GetSentRequestList(int startIndex, int lastIndex)
        {
            var currentUserId = User.Identity.GetUserId();
            var requestList = Context.Requests
                .Where(x => x.FromUserId == currentUserId)
                .OrderByDescending(x => x.CreateDateTime)
                .Skip(startIndex).Take(lastIndex - startIndex)
                .ToList().Select(Mapper.Map<Request, RequestItemDto>).ToList();

            foreach (var item in requestList)
                if (item.ToStatus != "1") item.ToUser = "";

            return Ok(requestList);
        }

        [Authorize]
        [Route("api/v01/RecievedRequestList/{giftId}/{startIndex}/{lastIndex}")]
        public IHttpActionResult GetRecievedRequestList(int giftId, int startIndex, int lastIndex)
        {
            var currentUserId = User.Identity.GetUserId();
            var requestList = Context.Requests
                .Where(x => x.GiftId == giftId && x.ToUserId == currentUserId && x.ToStatus == RequestToStatus.Pending)
                .OrderByDescending(x => x.CreateDateTime)
                .Skip(startIndex).Take(lastIndex - startIndex)
                .ToList().Select(Mapper.Map<Request, RequestItemDto>);

            return Ok(requestList);
        }

        [Authorize]
        [HttpPost]
        [Route("api/v01/RequestGift")]
        public IHttpActionResult CreateRequest(RequestAddDto requestAddDto)
        {
            if (!ModelState.IsValid || requestAddDto == null) return BadRequest();

            var currentUser = User.Identity.GetUserId();

            var gift = Context.Gifts.FirstOrDefault(x => x.Id == requestAddDto.GiftId);
            if (gift == null) return BadRequest("Invalid giftId");

            if (gift.UserId == currentUser) return BadRequest("Invalid request");

            var requestInDb =
                Context.Requests.FirstOrDefault(x => x.GiftId == requestAddDto.GiftId && x.FromUserId == currentUser);
            if (requestInDb != null) return BadRequest("Request exists");

            var request = Mapper.Map<RequestAddDto, Request>(requestAddDto);


            request.FromUserId = currentUser;
            request.ToUserId = gift.UserId;

            request.ToStatus = RequestToStatus.Pending;

            Context.Requests.Add(request);
            Context.SaveChanges();

            var toUser = Context.Users.FirstOrDefault(x => x.Id == request.ToUserId);
            if (toUser != null)
            {
                Firebase.SendeMessageToGroup(toUser.NotificationKey, new MessageFireBase()
                {
                    data = new
                    {
                        message = "یک درخواست برای هدیه شما به ثبت رسید.",
                        imageUrl = "",
                        title = "درخواست جدید"
                    },
                    userId = toUser.Id,
                    method = ""
                });
            }
            

            return Ok();
        }

        [Authorize]
        [HttpPut]
        [Route("api/v01/AcceptRequest/{giftId}/{fromUserId}")]
        public IHttpActionResult AcceptRequest(int giftId, string fromUserId)
        {
            if (!ModelState.IsValid || giftId == 0) return BadRequest();

            var currentUser = User.Identity.GetUserId();
            
            var request =
                Context.Requests.FirstOrDefault(
                    x =>
                        x.GiftId == giftId && x.ToUserId == currentUser && x.FromUserId == fromUserId && x.ToStatus == RequestToStatus.Pending);
            if (request == null) return BadRequest("Invalid request");

            request.ToStatus = RequestToStatus.Accepted;

            var gift = Context.Gifts.FirstOrDefault(x => x.Id == giftId && x.UserId == currentUser);
            if (gift == null) return BadRequest("Invalid giftId");

            gift.Status = GiftStatus.Donated;
            gift.ReceivedUserId = request.FromUserId;

            var requestList =
                Context.Requests.Where(
                    x =>
                        x.GiftId == giftId && x.ToUserId == currentUser && x.FromUserId != fromUserId && x.ToStatus == RequestToStatus.Pending);

            foreach (var item in requestList)
                item.ToStatus = RequestToStatus.DonatedToAnother;

            Context.SaveChanges();

            var toUser = Context.Users.FirstOrDefault(x => x.Id == request.FromUserId);
            if (toUser != null)
            {
                Firebase.SendeMessageToGroup(toUser.NotificationKey, new MessageFireBase()
                {
                    data = new
                    {
                        message = "با درخواست شما موافقت شد.",
                        imageUrl = "",
                        title = "هدیه جدید"
                    },
                    userId = toUser.Id,
                    method = ""
                });
            }

            return Ok();
        }

        [Authorize]
        [HttpPut]
        [Route("api/v01/DenyRequest/{giftId}/{fromUserId}")]
        public IHttpActionResult DenyRequest(int giftId, string fromUserId)
        {
            if (!ModelState.IsValid || giftId == 0) return BadRequest();

            var currentUser = User.Identity.GetUserId();

            var request =
                Context.Requests.FirstOrDefault(
                    x =>
                        x.GiftId == giftId && x.ToUserId == currentUser && x.FromUserId == fromUserId && x.ToStatus == RequestToStatus.Pending);
            if (request == null) return BadRequest("Invalid request");

            request.ToStatus = RequestToStatus.Rejected;
            Context.SaveChanges();

            var toUser = Context.Users.FirstOrDefault(x => x.Id == request.FromUserId);
            if (toUser != null)
            {
                Firebase.SendeMessageToGroup(toUser.NotificationKey, new MessageFireBase()
                {
                    data = new
                    {
                        message = "متاسفانه با درخواست شما موافقت نشد.",
                        imageUrl = "",
                        title = "رد درخواست"
                    },
                    userId = toUser.Id,
                    method = ""
                });
            }

            return Ok();
        }


        [Authorize]
        [HttpDelete]
        [Route("api/v01/DeleteMyRequest/{giftId}")]
        public IHttpActionResult DeleteMyRequest(int giftId)
        {
            if (!ModelState.IsValid || giftId == 0) return BadRequest();

            var currentUser = User.Identity.GetUserId();

            var request =
                Context.Requests.FirstOrDefault(
                    x => x.GiftId == giftId && x.FromUserId == currentUser);
            if (request == null) return BadRequest("Invalid request");

            Context.Requests.Remove(request);
            Context.SaveChanges();

            return Ok();
        }
    }
}