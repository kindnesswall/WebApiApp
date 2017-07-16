#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using AutoMapper;
using KindnessWall.Dto.Gift;
using KindnessWall.Enums;
using KindnessWall.Models;
using Microsoft.AspNet.Identity;
using KindnessWall.Dto.Feedback;

#endregion

namespace KindnessWall.Controllers.v01
{
    public class GiftController : BaseApiController
    {
        [HttpGet]
        [Route("api/v01/Gift/{cityId}/{regionId}/{categoryId}/{startIndex}/{lastIndex}")]
        public IHttpActionResult GetGifts(int cityId, int regionId, int categoryId, int startIndex, int lastIndex, string searchText = null)
        {
            var isSerachNull = string.IsNullOrWhiteSpace(searchText);
            var giftList = Context.Gifts
                .Where(x => (categoryId == 0 || x.CategoryId == categoryId)
                            && (cityId == 0 || x.CityId == cityId)
                            && (regionId == 0 || x.RegionId == regionId)
                            && (isSerachNull || x.Title.ToLower().Contains(searchText.ToLower()))
                            && x.Status == GiftStatus.WaitingToBeDonated
                )
                .OrderByDescending(x => x.CreateDateTime)
                .Skip(startIndex).Take(lastIndex - startIndex)
                .ToList()
                .Select(Mapper.Map<Gift, GiftPublicListItemDto>);

            return Ok(giftList);
        }

        [Authorize]
        [HttpGet]
        [Route("api/v01/MyGift/{startIndex}/{lastIndex}")]
        public IHttpActionResult GetMyGifts(int startIndex, int lastIndex)
        {
            var currentUser = User.Identity.GetUserId();
            var giftList = Context.Gifts
                .Where(x => x.UserId == currentUser)
                .OrderByDescending(x => x.CreateDateTime)
                .Skip(startIndex).Take(lastIndex - startIndex)
                .ToList().Select(Mapper.Map<Gift, GiftPrivateListItemDto>);

            return Ok(giftList);
        }

        [HttpGet]
        [Route("api/v01/RegisteredGifts/{userId}/{startIndex}/{lastIndex}")]
        public IHttpActionResult RegisteredGifts(string userId, int startIndex, int lastIndex)
        {
            var giftList = Context.Gifts
                .Where(x => x.UserId == userId
                            && x.Status == GiftStatus.WaitingToBeDonated)
                .OrderByDescending(x => x.CreateDateTime)
                .Skip(startIndex).Take(lastIndex - startIndex)
                .ToList();

            if (User.Identity.IsAuthenticated)
            {
                var currentUserId = User.Identity.GetUserId();
                if (currentUserId == userId)
                {
                    return Ok(giftList.Select(Mapper.Map<Gift, GiftPrivateListItemDto>));
                }
            }

            return Ok(giftList.Select(Mapper.Map<Gift, GiftPublicListItemDto>));


        }

        [HttpGet]
        [Route("api/v01/DonatedGifts/{userId}/{startIndex}/{lastIndex}")]
        public IHttpActionResult DonatedGifts(string userId, int startIndex, int lastIndex)
        {
            var giftList = Context.Gifts
                .Where(x => x.UserId == userId
                            && x.Status == GiftStatus.Donated)
                .OrderByDescending(x => x.CreateDateTime)
                .Skip(startIndex).Take(lastIndex - startIndex)
                .ToList();

            if (User.Identity.IsAuthenticated)
            {
                var currentUserId = User.Identity.GetUserId();
                if (currentUserId == userId)
                {
                    return Ok(giftList.Select(Mapper.Map<Gift, GiftPrivateListItemDto>));
                }
            }

            return Ok(giftList.Select(Mapper.Map<Gift, GiftPublicListItemDto>));
        }

        [HttpGet]
        [Route("api/v01/ReceivedGifts/{userId}/{startIndex}/{lastIndex}")]
        public IHttpActionResult ReceivedGifts(string userId, int startIndex, int lastIndex)
        {
            var giftList = Context.Gifts
                .Where(x => x.ReceivedUserId == userId
                            && x.Status == GiftStatus.Donated)
                .OrderByDescending(x => x.CreateDateTime)
                .Skip(startIndex).Take(lastIndex - startIndex)
                .ToList();

            if (User.Identity.IsAuthenticated)
            {
                var currentUserId = User.Identity.GetUserId();
                if (currentUserId == userId)
                {
                    return Ok(giftList.Select(Mapper.Map<Gift, GiftPrivateListItemDto>));
                }
            }

            return Ok(giftList.Select(Mapper.Map<Gift, GiftPublicListItemDto>));

        }

        [Authorize]
        [HttpGet]
        [Route("api/v01/GiftsRequested/{startIndex}/{lastIndex}")]
        public IHttpActionResult GiftsRequested(int startIndex, int lastIndex)
        {
            var currentUserId = User.Identity.GetUserId();

            var requests = Context.Requests.Where(x => x.ToUserId == currentUserId && x.ToStatus == RequestToStatus.Pending);
            if (!requests.Any()) return Ok(requests);

            var giftsRequested = requests.Select(x => x.GiftId).ToList();

            var giftList = Context.Gifts.AsQueryable()
                .Where(x => x.UserId == currentUserId && giftsRequested.Contains(x.Id) && x.Status == GiftStatus.WaitingToBeDonated)
                .OrderByDescending(x => x.CreateDateTime)
                .Skip(startIndex).Take(lastIndex - startIndex)
                .ToList().Select(Mapper.Map<Gift, GiftRequestedListItemDto>).ToList();

            foreach (var item in giftList)
            {
                var giftId = int.Parse(item.Id);
                item.RequestCount = requests.Count(x => x.GiftId == giftId).ToString();
            }

            return Ok(giftList);
        }

        [HttpGet]
        [Route("api/v01/Gift/{id}")]
        public IHttpActionResult GetGift(int id)
        {
            var gift = Context.Gifts.FirstOrDefault(x => x.Id == id);
            if (gift == null) return NotFound();

            if (!User.Identity.IsAuthenticated)
            {
                var result = Mapper.Map<Gift, GiftPublicItemDto>(gift);
                return Ok(result);
            }

            var currentUserId = User.Identity.GetUserId();

            if (gift.UserId == currentUserId)
            {
                var result = Mapper.Map<Gift, GiftPrivateItemDto>(gift);
                result.Bookmark = Context.Bookmarks.Any(x => x.UserId == currentUserId && x.GiftId == gift.Id);
                return Ok(result);
            }

            var request = Context.Requests.FirstOrDefault(x => x.GiftId == gift.Id && x.FromUserId == currentUserId);
            if (request == null)
            {
                var result = Mapper.Map<Gift, GiftPublicItemDto>(gift);
                result.Bookmark = Context.Bookmarks.Any(x => x.UserId == currentUserId && x.GiftId == gift.Id);
                return Ok(result);
            }

            if (gift.Status == GiftStatus.Donated)
            {
                if (request.ToStatus == RequestToStatus.DonatedToAnother)
                {
                    var result = Mapper.Map<Gift, GiftPublicItemDto>(gift);
                    result.Bookmark = Context.Bookmarks.Any(x => x.UserId == currentUserId && x.GiftId == gift.Id);
                    return Ok(result);
                }
                if (request.ToStatus == RequestToStatus.Accepted)
                {
                    var result = Mapper.Map<Gift, GiftPrivateItemDto>(gift);
                    result.Bookmark = Context.Bookmarks.Any(x => x.UserId == currentUserId && x.GiftId == gift.Id);
                    result.Status = GiftStatus.RequestAccepted.ToString();
                    return Ok(result);
                }
            }
            if (gift.Status == GiftStatus.WaitingToBeDonated)
            {
                if (request.ToStatus == RequestToStatus.Pending)
                {
                    var result = Mapper.Map<Gift, GiftPublicItemDto>(gift);
                    result.Bookmark = Context.Bookmarks.Any(x => x.UserId == currentUserId && x.GiftId == gift.Id);
                    result.Status = GiftStatus.RequestPending.ToString();
                    return Ok(result);
                }
                if (request.ToStatus == RequestToStatus.Rejected)
                {
                    var result = Mapper.Map<Gift, GiftPublicItemDto>(gift);
                    result.Bookmark =
                        Context.Bookmarks.Any(x => x.UserId == currentUserId && x.GiftId == gift.Id);
                    result.Status = GiftStatus.RequestRejected.ToString();
                    return Ok(result);
                }
            }

            return null;
        }

        [Authorize]
        [HttpPost]
        [Route("api/v01/Gift")]
        public IHttpActionResult CreateGift(GiftAddDto giftAddDto)
        {
            if (!ModelState.IsValid || giftAddDto == null) return BadRequest();

            var gift = new Gift
            {
                Title = giftAddDto.Title,
                Address = giftAddDto.Address,
                Description = giftAddDto.Description,
                Price = giftAddDto.Price,
                CategoryId = giftAddDto.CategoryId,
                CityId = giftAddDto.CityId,
                RegionId = giftAddDto.RegionId,
                Status = GiftStatus.WaitingToBeDonated,
                UserId = User.Identity.GetUserId()
            };


            Context.Gifts.Add(gift);
            Context.SaveChanges();

            foreach (var giftImageDto in giftAddDto.GiftImages)
            {
                var giftImage = new GiftImage
                {
                    GiftId = gift.Id,
                    ImageUrl = giftImageDto,
                    UserId = gift.UserId
                };

                Context.GiftImages.Add(giftImage);
            }


            Context.SaveChanges();

            var giftItemDtop = Mapper.Map<Gift, GiftPrivateItemDto>(gift);

            return Created(new Uri(Request.RequestUri + "/" + gift.Id), giftItemDtop);
        }

        [Authorize]
        [HttpPut]
        [Route("api/v01/Gift/{id}")]
        public IHttpActionResult UpdateGift(int id, GiftEditDto giftEditDto)
        {
            if (!ModelState.IsValid || giftEditDto == null) return BadRequest();

            var gift = Context.Gifts.FirstOrDefault(x => x.Id == id);
            if (gift == null || gift.UserId != User.Identity.GetUserId()) return NotFound();

            if (gift.Status == GiftStatus.Donated) return BadRequest("Can't edit after donation.");

            gift.Title = giftEditDto.Title;
            gift.Address = giftEditDto.Address;
            gift.Description = giftEditDto.Description;
            gift.Price = giftEditDto.Price;
            gift.CategoryId = giftEditDto.CategoryId;
            gift.CityId = giftEditDto.CityId;
            gift.RegionId = giftEditDto.RegionId;

            var giftImages = Context.GiftImages.Where(x => x.GiftId == gift.Id).ToList();
            Context.GiftImages.RemoveRange(giftImages);
            Context.SaveChanges();

            foreach (var giftImageDto in giftEditDto.GiftImages)
            {
                var giftImage = new GiftImage
                {
                    GiftId = gift.Id,
                    ImageUrl = giftImageDto,
                    UserId = gift.UserId
                };

                Context.GiftImages.Add(giftImage);
            }

            Context.SaveChanges();

            return Ok();
        }

        [Authorize]
        [HttpDelete]
        [Route("api/v01/Gift/{id}")]
        public IHttpActionResult DeleteGift(int id)
        {
            var gift = Context.Gifts.FirstOrDefault(x => x.Id == id);
            if (gift == null || gift.UserId != User.Identity.GetUserId()) return NotFound();

            if(gift.Status == GiftStatus.Donated) return BadRequest("Can't delete after donation.");

            Context.Gifts.Remove(gift);
            Context.SaveChanges();

            return Ok();
        }

        [Authorize]
        [HttpPost]
        [Route("api/v01/ReportGift")]
        public IHttpActionResult ReportGift(FeedbackAddDto feedbackAddDto)
        {
            if (!ModelState.IsValid || feedbackAddDto == null) return BadRequest();

            var feedback = new Feedback
            {
                GiftId = feedbackAddDto.GiftId,
                UserId = User.Identity.GetUserId(),
                Message = feedbackAddDto.Message
            };

            Context.Feedbacks.Add(feedback);
            Context.SaveChanges();

            return Ok();
        }
    }
}