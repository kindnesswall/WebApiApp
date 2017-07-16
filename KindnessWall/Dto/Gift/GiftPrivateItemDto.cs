using System.Collections.Generic;

namespace KindnessWall.Dto.Gift
{
    public class GiftPrivateItemDto : BaseDto
    {
        public string Title { get; set; }

        public string Address { get; set; }

        public string Description { get; set; }

        public string Price { get; set; }

        public string Status { get; set; }

        public string UserId { get; set; }
        public string User { get; set; }

        public string ReceivedUserId { get; set; }
        public string ReceivedUser { get; set; }

        public string CategoryId { get; set; }
        public string Category { get; set; }

        public string CityId { get; set; }

        public string RegionId { get; set; }

        public bool Bookmark { get; set; }

        public IEnumerable<string> GiftImages { get; set; }
    }
}