using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KindnessWall.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            CreateDateTime = DateTime.Now;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ImageSrc { get; set; }
        public string Mobile { get; set; }
        public bool MobileNumberConfirmed { get; set; }
        public DateTime? BirthDate { get; set; }
        public string VerificationCode { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreateDateTime { get; set; }
        public bool IsLogin { get; set; }
        public bool SmsSent { get; set; }
        public DateTime? VerificationDateTime { get; set; }
        public string NotificationKey { get; set; }
        public ICollection<Request> RequestFromUsers { get; set; }
        public ICollection<Request> RequestToUsers { get; set; }
        public ICollection<Gift> ReceivedGifts { get; set; }
        public ICollection<Gift> DonatedGifts { get; set; }
        public ICollection<Bookmark> Bookmarks { get; set; }
        public ICollection<Feedback> Feedbacks { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }
}