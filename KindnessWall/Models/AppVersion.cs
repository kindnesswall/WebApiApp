using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KindnessWall.Models
{
    public class AppVersion : BaseEntity
    {
        public string Version { get; set; }

        public string ApkUrl { get; set; }

        public string Changes { get; set; }

        public string LastUpdateVersion { get; set; }
    }
}