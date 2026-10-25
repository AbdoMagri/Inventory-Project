using InventoryDB_Business;
using InventoryDB_DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryAPI.Controllers
{
    [Route("api/Categories")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        [HttpGet("All", Name = "GetAllCategories")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<clsCategoryData.CategoryDTO>> GetAllCategories(int AccountID)
        {
            return Ok(clsCategory.GetAllCategories(AccountID));
        }

        [HttpGet("{id:int}", Name = "GetCategoryByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<clsCategoryData.CategoryDTO> GetCategoryById(int id)
        {
            if (id < 1)
                return BadRequest($"Not accepted ID {id}");

            clsCategory Category = clsCategory.Find(id);
            if (Category == null)
                return NotFound($"Category with ID {id} not found.");

            return Ok(Category.DTO);
        }

        [HttpPost(Name = "AddCategory")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public ActionResult<clsCategoryData.CategoryDTO> AddCategory(clsCategoryData.CategoryDTO newCategoryDTO, int AccountID)
        {
            if (newCategoryDTO == null || string.IsNullOrWhiteSpace(newCategoryDTO.Name))
                return BadRequest("Invalid Category Data");

            if (clsCategory.isCategoryExist(AccountID, newCategoryDTO.Name))
                return Conflict("Category already exists.");

            clsCategory Category = new clsCategory(newCategoryDTO);
            if (!Category.Save())
                return BadRequest("Failed to save the category.");

            newCategoryDTO.CategoryID = Category.CategoryID;
            return CreatedAtRoute("GetCategoryByID", new { id = newCategoryDTO.CategoryID }, newCategoryDTO);
        }

        [HttpPut("{id:int}", Name = "UpdateCategory")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public ActionResult<clsCategoryData.CategoryDTO> UpdateCategory(int id, clsCategoryData.CategoryDTO updatedCategory)
        {
            if (id < 1 || updatedCategory == null || string.IsNullOrWhiteSpace(updatedCategory.Name))
                return BadRequest("Invalid Category Data.");

            clsCategory Category = clsCategory.Find(id);
            if (Category == null)
                return NotFound("Category Not Found");

            if (!string.Equals(Category.Name, updatedCategory.Name, StringComparison.OrdinalIgnoreCase)
                && clsCategory.isCategoryExist(Category.AccountID, updatedCategory.Name))
                return Conflict("Category already exists.");

            Category.Name = updatedCategory.Name;

            if (!Category.Save())
                return BadRequest("Failed to update the category.");

            return Ok(Category.DTO);
        }

        [HttpDelete("{id:int}", Name = "DeleteCategory")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteCategory(int id)
        {
            if (id < 1)
                return BadRequest("Invalid Category Data");

            if (clsCategory.DeleteCategory(id))
                return Ok("Category has been deleted");
            else
                return NotFound("Category Not found");
        }
    }
}