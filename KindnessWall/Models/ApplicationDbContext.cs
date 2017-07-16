using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace KindnessWall.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Bookmark> Bookmarks { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Gift> Gifts { get; set; }
        public DbSet<GiftImage> GiftImages { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<AppVersion> AppVersions { get; set; }
        public DbSet<AppVersionChange> AppVersionChanges { get; set; }

        public ApplicationDbContext() : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            #region Gift

            modelBuilder.Entity<Gift>()
                        .HasRequired(m => m.User)
                        .WithMany(t => t.DonatedGifts)
                        .HasForeignKey(m => m.UserId)
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<Gift>()
                        .HasOptional(m => m.ReceivedUser)
                        .WithMany(t => t.ReceivedGifts)
                        .HasForeignKey(m => m.ReceivedUserId)
                        .WillCascadeOnDelete(false);

            #endregion

            #region Request

            modelBuilder.Entity<Request>()
                        .HasRequired(m => m.FromUser)
                        .WithMany(t => t.RequestFromUsers)
                        .HasForeignKey(m => m.FromUserId)
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<Request>()
                        .HasRequired(m => m.ToUser)
                        .WithMany(t => t.RequestToUsers)
                        .HasForeignKey(m => m.ToUserId)
                        .WillCascadeOnDelete(false);

            #endregion
            
            base.OnModelCreating(modelBuilder);
        }
    }
}