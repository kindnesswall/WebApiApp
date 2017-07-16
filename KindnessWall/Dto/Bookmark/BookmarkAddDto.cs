using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using KindnessWall.Dto.Gift;

namespace KindnessWall.Dto.Bookmark
{
    public class BookmarkAddDto
    {
        [Required]
        public int GiftId { get; set; }
    }
}