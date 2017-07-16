using System.Collections.Generic;

namespace KindnessWall.Models
{
    public class Gift : BaseEntity
    {
        public string Title { get; set; }

        public string Address { get; set; }

        public string Description { get; set; }

        public int Price { get; set; }

        public int Status { get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public string ReceivedUserId { get; set; }
        public virtual ApplicationUser ReceivedUser { get; set; }

        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

        public int CityId { get; set; }

        public int RegionId { get; set; }

        public virtual ICollection<GiftImage> GiftImages { get; set; }
        public ICollection<Request> Requests { get; set; }
        public ICollection<Bookmark> Bookmarks { get; set; }

    }
}