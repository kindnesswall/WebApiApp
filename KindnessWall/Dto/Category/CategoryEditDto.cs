using System.ComponentModel.DataAnnotations;

namespace KindnessWall.Dto.Category
{
    public class CategoryEditDto
    {
        [Required]
        public string Title { get; set; }
    }
}