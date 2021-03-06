﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KindnessWall.Models
{
    public class Bookmark : BaseEntity
    {
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public int GiftId { get; set; }
        public virtual Gift Gift { get; set; }

    }
}