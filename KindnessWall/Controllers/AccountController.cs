#region using

using KindnessWall.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

#endregion

namespace KindnessWall.Controllers
{
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private ApplicationUserManager _userManager;
        private readonly ApplicationDbContext _dbContext = new ApplicationDbContext();

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }

        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { _userManager = value; }
        }

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; }

        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register/{mobile}")]
        public async Task<IHttpActionResult> Register(string mobile)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var random = new Random();
            var verificationCode = random.Next(100000, 999999).ToString();

            var user = UserManager.FindByName(mobile);

            if (user == null)
            {
                user = new ApplicationUser()
                {
                    UserName = mobile,
                    Mobile = mobile,
                    VerificationDateTime = DateTime.Now,
                    SmsSent = true,

                };

                var result = await UserManager.CreateAsync(user, verificationCode);
                if (!result.Succeeded) return GetErrorResult(result);
            }
            else
            {
                var dif = DateTime.Now - user.VerificationDateTime.Value;
                if (dif.TotalMinutes < 1 && user.SmsSent)
                {
                    if (dif.TotalSeconds != 0)
                    {
                        var span = new TimeSpan(0, 0, (int)(Math.Abs(dif.TotalSeconds - 60)));
                        var remainigSeconds = Convert.ToInt32(span.ToString(@"ss"));
                        return Ok(new { remainingSeconds = remainigSeconds.ToString() });
                    }
                }

                user.PasswordHash = UserManager.PasswordHasher.HashPassword(verificationCode);
                user.IsLogin = false;
                user.SmsSent = true;
                user.VerificationDateTime = DateTime.Now;

                var result = await UserManager.UpdateAsync(user);
                if (!result.Succeeded) return GetErrorResult(result);
            }

            //send verification sms here
           

            return Ok(new { });
        }

        // POST api/Account/Logout
        [Authorize]
        [HttpPost]
        [Route("Logout")]
        public IHttpActionResult Logout(LogoutModel logoutModel)
        {
            var userId = User.Identity.GetUserId();

            var deviceItem = _dbContext.Devices.FirstOrDefault(x => x.UserId == userId && x.RegisterationId == logoutModel.RegisterationId);
            if (deviceItem != null)
            {
                deviceItem.UserId = null;
                _dbContext.SaveChanges();
            }
            

            var user = UserManager.FindById(userId);
            user.SmsSent = false;
            user.IsLogin = false;

            UserManager.Update(user);

            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("SetDevice")]
        public IHttpActionResult SetDevice(SetDeviceModel setDeviceModel)
        {
            try
            {
                var deviceItem = _dbContext.Devices.FirstOrDefault(x => x.DeviceId == setDeviceModel.DeviceId);

                if (deviceItem == null) //Create 
                {
                    _dbContext.Devices.Add(new Device()
                    {
                        DeviceId = setDeviceModel.DeviceId,
                        RegisterationId = setDeviceModel.RegisterationId
                    });
                    _dbContext.SaveChanges();
                    return Ok(new { status = 1 });
                }
                else //update
                {
                    deviceItem.RegisterationId = setDeviceModel.RegisterationId;
                    _dbContext.SaveChanges();
                    return Ok(new { status = 2 });
                }


            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (_userManager != null))
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        #region Helpers

        private IAuthenticationManager Authentication => Request.GetOwinContext().Authentication;

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
                return InternalServerError();

            if (result.Succeeded) return null;

            if (result.Errors != null)
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error);

            if (ModelState.IsValid)
                return BadRequest();

            return BadRequest(ModelState);
        }

        #endregion


    }

    public class SetDeviceModel
    {
        public string DeviceId { get; set; }
        public string RegisterationId { get; set; }
    }

    public class LogoutModel
    {
        public string RegisterationId { get; set; }
    }
}