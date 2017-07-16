using System.ComponentModel.DataAnnotations;

namespace KindnessWall.Dto.Request
{
    public class RequestAddDto
    {
        [Required]
        public int GiftId { get; set; }
    }
}