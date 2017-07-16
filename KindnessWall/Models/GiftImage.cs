namespace KindnessWall.Models
{
    public class GiftImage : BaseEntity
    {
        public int GiftId { get; set; }
        public Gift Gift { get; set; }

        public string ImageUrl { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

    }
}