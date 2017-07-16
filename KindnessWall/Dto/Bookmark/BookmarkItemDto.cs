using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KindnessWall.Dto.Gift;

namespace KindnessWall.Dto.Bookmark
{
    public class BookmarkItemDto
    {
        public string GiftId { get; set; }
        public GiftPublicItemDto Gift { get; set; }

        public string UserId { get; set; }
        public string User { get; set; }
    }
}