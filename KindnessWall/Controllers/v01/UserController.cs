#region using

using AutoMapper;
using KindnessWall.Dto.User;
using KindnessWall.Models;
using System.Linq;
using System.Web.Http;
using KindnessWall.Enums;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;

#endregion

namespace KindnessWall.Controllers.v01
{
    public class UserController : BaseApiController
    {
        [Route("api/v01/User/{id}")]
        public IHttpActionResult GetUser(string id)
        {
            var user = Context.Users.FirstOrDefault(x => x.Id == id);
            if (user == null) return NotFound();


            if (!User.Identity.IsAuthenticated)
            {
                var result = Mapper.Map<ApplicationUser, UserPublicItemDto>(user);
                return Ok(result);
            }
            else
            {
                var currentUserId = User.Identity.GetUserId();
                var request = Context.Requests.FirstOrDefault(x => x.FromUserId == id && x.ToUserId == currentUserId);
                if (request == null)
                {
                    var result = Mapper.Map<ApplicationUser, UserPublicItemDto>(user);
                    return Ok(result);
                }
                else
                {
                    var result = Mapper.Map<ApplicationUser, UserPrivateItemDto>(user);
                    return Ok(result);
                }
            }
        }

        [Route("api/v01/GetStatistics")]
        public IHttpActionResult GetStatistics()
        {
            var devices = Context.Devices.DistinctBy(x => x.DeviceId).Count();
            var totalGifts = Context.Gifts.Count();
            var donatedGifts = Context.Gifts.Count(x => x.Status == GiftStatus.Donated);

            var result = new Dictionary<string, long>
            {
                {"تعداد نصب", devices},
                {"تعداد هدیه های ثبت شده", totalGifts},
                {"تعداد هدیه های اهداء شده", donatedGifts}
            };


            return Ok(new { statistics = result });

        }
    }
}