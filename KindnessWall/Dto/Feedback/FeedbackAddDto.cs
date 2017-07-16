using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KindnessWall.Dto.Feedback
{
    public class FeedbackAddDto
    {
        [Required]
        public int GiftId { get; set; }

        [Required]
        public string Message { get; set; }
    }
}