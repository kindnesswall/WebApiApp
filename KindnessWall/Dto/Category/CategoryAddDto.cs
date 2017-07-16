using System.ComponentModel.DataAnnotations;

namespace KindnessWall.Dto.Category
{
    public class CategoryAddDto
    {
        [Required]
        public string Title { get; set; }
    }
}