using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KindnessWall.Models
{
    public class Request : BaseEntity
    {
        public int GiftId { get; set; }
        public virtual Gift Gift { get; set; }

        public string FromUserId { get; set; }
        public virtual ApplicationUser FromUser { get; set; }

        public string ToUserId { get; set; }
        public virtual ApplicationUser ToUser { get; set; }

        //public byte FromStatus { get; set; }
        public byte ToStatus { get; set; }


    }
}