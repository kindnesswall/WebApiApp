using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KindnessWall.Dto.Request
{
    public class RequestItemDto
    {
        public string GiftId { get; set; }
        public string Gift { get; set; }

        public string FromUserId { get; set; }
        public string FromUser { get; set; }

        public string ToUserId { get; set; }
        public string ToUser { get; set; }

        //public string FromStatus { get; set; }
        public string ToStatus { get; set; }

    }
}