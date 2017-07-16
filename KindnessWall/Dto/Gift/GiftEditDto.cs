using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KindnessWall.Dto.Gift
{
    public class GiftEditDto
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int Price { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int CityId { get; set; }

        [Required]
        public int RegionId { get; set; }

        public IEnumerable<string> GiftImages { get; set; }
    }
}