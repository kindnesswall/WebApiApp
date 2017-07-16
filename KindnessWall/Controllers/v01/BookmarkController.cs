#region using

using System.Linq;
using System.Web.Http;
using AutoMapper;
using KindnessWall.Dto.Bookmark;
using KindnessWall.Dto.Gift;
using KindnessWall.Dto.Request;
using KindnessWall.Enums;
using KindnessWall.Models;
using Microsoft.AspNet.Identity;

#endregion

namespace KindnessWall.Controllers.v01
{
    public class BookmarkController : BaseApiController
    {
        [Authorize]
        [Route("api/v01/BookmarkList/{startIndex}/{lastIndex}")]
        public IHttpActionResult GetBookmarkList(int startIndex, int lastIndex)
        {
            var currentUserId = User.Identity.GetUserId();

            var giftIds = Context.Bookmarks.Where(x => x.UserId == currentUserId).Select(x => x.GiftId).ToList();
            if (!giftIds.Any()) return Ok(giftIds);

            var giftList = Context.Gifts.AsQueryable()
                .Where(x => giftIds.Contains(x.Id))
                .OrderByDescending(x => x.CreateDateTime)
                .Skip(startIndex).Take(lastIndex - startIndex)
                .ToList().Select(Mapper.Map<Gift, GiftPublicListItemDto>).ToList();

            return Ok(giftList);
        }

        [Authorize]
        [HttpPost]
        [Route("api/v01/BookmarkGift")]
        public IHttpActionResult BookmarkGift(BookmarkAddDto bookmarkAddDto)
        {
            if (!ModelState.IsValid || bookmarkAddDto == null) return BadRequest();

            var currentUser = User.Identity.GetUserId();

            var gift = Context.Gifts.FirstOrDefault(x => x.Id == bookmarkAddDto.GiftId);
            if (gift == null) return BadRequest("Invalid giftId");

            var bookmarkInDb =
                Context.Bookmarks.FirstOrDefault(x => x.GiftId == gift.Id && x.UserId == currentUser);
            if (bookmarkInDb != null)
            {
                Context.Bookmarks.Remove(bookmarkInDb);
            }
            else
            {
                var bookmark = new Bookmark
                {
                    UserId = currentUser,
                    GiftId = gift.Id
                };
                Context.Bookmarks.Add(bookmark);
            }
            Context.SaveChanges();

            return Ok();
        }


    }
}