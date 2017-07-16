using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KindnessWall.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using KindnessWall.Helper;
using System.Linq;

namespace KindnessWall.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;
        private readonly ApplicationDbContext _dbContext = new ApplicationDbContext();

        public ApplicationOAuthProvider(string publicClientId)
        {
            if (publicClientId == null)
                throw new ArgumentNullException("publicClientId");

            _publicClientId = publicClientId;
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();
            var user = await userManager.FindAsync(context.UserName, context.Password);

            if (user == null)
            {
                context.SetError("invalid_grant", "incorrect_user_pass");
                return;
            }

            if (!user.SmsSent)
            {
                context.SetError("invalid_grant", "please register first.");
                return;
            }

            user.IsLogin = true;
            var isLoginUpdated = await userManager.UpdateAsync(user);
            if (!isLoginUpdated.Succeeded)
            {
                context.SetError("internal_error", "An error has occurred.");
                return;
            }
            try
            {
                var data = await context.Request.ReadFormAsync();
                if (data["deviceId"] != null)
                {
                    var deviceId = data["deviceId"].ToString();
                    var registerationId = data["registerationId"].ToString();

                    if (string.IsNullOrEmpty(user.NotificationKey))
                    {
                        var notificationKey = Firebase.CreateFirebaseGroup("U-" + user.Id, registerationId);

                        user.NotificationKey = notificationKey;
                        var notificationKeyUpdated = await userManager.UpdateAsync(user);
                        if (!notificationKeyUpdated.Succeeded)
                        {
                            context.SetError("internal_error", "An error has occurred.");
                            return;
                        }

                        

                    }
                    else
                    {
                        Firebase.AddToFirebaseGroup("U-" + user.Id, user.NotificationKey, registerationId);
                    }


                    var oldRegisterId = SetLoginRegisterationId(user.Id, deviceId, registerationId);

                    if (!string.IsNullOrEmpty(oldRegisterId) && oldRegisterId != registerationId)
                    {
                        Firebase.RemoveFromFireBase("U-" + user.Id, user.NotificationKey, new string[] { oldRegisterId });
                    }

                }
                else
                {
                    context.SetError("internal_error", "call setDevice before login.");
                    return;
                }
            }
            catch (Exception ex)
            {
                context.SetError("firebase_error", ex.Message);
            }

            

            var oAuthIdentity = await user.GenerateUserIdentityAsync(userManager,
                OAuthDefaults.AuthenticationType);
            var cookiesIdentity = await user.GenerateUserIdentityAsync(userManager,
                CookieAuthenticationDefaults.AuthenticationType);
            var properties = CreateProperties(user.Id);
            var ticket = new AuthenticationTicket(oAuthIdentity, properties);
            context.Validated(ticket);
            context.Request.Context.Authentication.SignIn(cookiesIdentity);
        }

        private string SetLoginRegisterationId(string userId, string deviceId, string registerationId)
        {
            var deviceItem = _dbContext.Devices.FirstOrDefault(x => x.DeviceId == deviceId);
            var oldRegisterationId = "";
            if (deviceItem == null) return "";

            oldRegisterationId = deviceItem.RegisterationId;
            deviceItem.UserId = userId;
            deviceItem.RegisterationId = registerationId;

            _dbContext.SaveChanges();

            return oldRegisterationId;
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (var property in context.Properties.Dictionary)
                context.AdditionalResponseParameters.Add(property.Key, property.Value);

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
                context.Validated();

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                var expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                    context.Validated();
            }

            return Task.FromResult<object>(null);
        }

        public static AuthenticationProperties CreateProperties(string userId)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                {"userId", userId}
            };
            return new AuthenticationProperties(data);
        }
    }
}