using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KindnessWall.Models
{
    public class AppVersionChange : BaseEntity
    {
        public int AppVersionId { get; set; }
        public AppVersion AppVersion { get; set; }

        public string Description { get; set; }

        public int ViewOrder { get; set; }
    }
}