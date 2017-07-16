namespace KindnessWall.Models
{
    public class Category : BaseEntity
    {
        public string Title { get; set; }

        public string ImageUrl { get; set; }

        public int ViewOrder { get; set; }
    }
}