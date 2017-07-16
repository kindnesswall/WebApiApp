#region using

using AutoMapper;
using KindnessWall.Dto.Category;
using KindnessWall.Models;
using System;
using System.Linq;
using System.Web.Http;

#endregion

namespace KindnessWall.Controllers.v01
{
    public class CategoryController : BaseApiController
    {
        [Route("api/v01/Category/{startIndex}/{lastIndex}/{densityId}")]
        public IHttpActionResult GetCategories(int startIndex, int lastIndex, int densityId)
        {
            var categoryList = Context.Categories
                .OrderBy(x => x.ViewOrder)
                .Skip(startIndex).Take(lastIndex - startIndex)
                .ToList()
                .Select(Mapper.Map<Category, CategoryItemDto>);

            return Ok(categoryList);
        }

        [Route("api/v01/Category/{id}")]
        public IHttpActionResult GetCategory(int id)
        {
            var category = Context.Categories.FirstOrDefault(x => x.Id == id);

            if (category == null) return NotFound();

            return Ok(Mapper.Map<Category, CategoryItemDto>(category));
        }
    }
}