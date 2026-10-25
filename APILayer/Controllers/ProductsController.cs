using InventoryDB_Business;
using InventoryDB_DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryAPI.Controllers
{
    [Route("api/Products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        [HttpGet("All", Name = "GetAllProducts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<clsProductData.ProductDTO>> GetAllProducts(int AccountID)
        {
            return Ok(clsProduct.GetAllProducts(AccountID));
        }

        [HttpGet("LowStock", Name = "GetLowStockProducts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult GetLowStockProducts(int AccountID)
        {
            return Ok(clsProduct.GetLowStockProducts(AccountID));
        }

        [HttpGet("{id:int}", Name = "GetProductByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<clsProductData.ProductDTO> GetProductById(int id)
        {
            if (id < 1)
                return BadRequest($"Not accepted ID {id}");

            clsProduct Product = clsProduct.Find(id);
            if (Product == null)
                return NotFound($"Product with ID {id} not found.");

            return Ok(Product.DTO);
        }

        [HttpGet("BySKU", Name = "GetProductBySKU")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<clsProductData.ProductDTO> GetProductBySKU(int AccountID, string SKU)
        {
            if (string.IsNullOrWhiteSpace(SKU))
                return BadRequest("SKU is required.");

            clsProduct Product = clsProduct.FindBySKU(AccountID, SKU);
            if (Product == null)
                return NotFound($"Product with SKU {SKU} not found.");

            return Ok(Product.DTO);
        }

        [HttpPost(Name = "AddProduct")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public ActionResult<clsProductData.ProductDTO> AddProduct(clsProductData.ProductDTO newProductDTO, int AccountID)
        {
            if (newProductDTO == null || string.IsNullOrWhiteSpace(newProductDTO.Name)
                || string.IsNullOrWhiteSpace(newProductDTO.SKU))
                return BadRequest("Invalid Product Data");

            if (clsProduct.isSKUExist(AccountID, newProductDTO.SKU))
                return Conflict("SKU already exists.");

            clsProduct Product = new clsProduct(newProductDTO);
            if (!Product.Save())
                return BadRequest("Failed to save the product.");

            newProductDTO.ProductID = Product.ProductID;
            return CreatedAtRoute("GetProductByID", new { id = newProductDTO.ProductID }, newProductDTO);
        }

        // Quantity is not updated here, it only changes through sales, purchases and stock adjustments
        [HttpPut("{id:int}", Name = "UpdateProduct")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public ActionResult<clsProductData.ProductDTO> UpdateProduct(int id, clsProductData.ProductDTO updatedProduct)
        {
            if (id < 1 || updatedProduct == null || string.IsNullOrWhiteSpace(updatedProduct.Name)
                || string.IsNullOrWhiteSpace(updatedProduct.SKU))
                return BadRequest("Invalid Product Data.");

            clsProduct Product = clsProduct.Find(id);
            if (Product == null)
                return NotFound("Product Not Found");

            if (updatedProduct.SKU != Product.SKU && clsProduct.isSKUExist(Product.AccountID, updatedProduct.SKU))
                return Conflict("SKU already exists.");

            Product.CategoryID = updatedProduct.CategoryID;
            Product.UnitID = updatedProduct.UnitID;
            Product.Name = updatedProduct.Name;
            Product.SKU = updatedProduct.SKU;
            Product.CostPrice = updatedProduct.CostPrice;
            Product.SellPrice = updatedProduct.SellPrice;
            Product.MinStock = updatedProduct.MinStock;
            Product.IsActive = updatedProduct.IsActive;
            Product.ImagePath = updatedProduct.ImagePath;

            if (!Product.Save())
                return BadRequest("Failed to update the product.");

            return Ok(Product.DTO);
        }

        [HttpDelete("{id:int}", Name = "DeleteProduct")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteProduct(int id)
        {
            if (id < 1)
                return BadRequest("Invalid Product Data");

            if (clsProduct.DeleteProduct(id))
                return Ok("Product has been deleted");
            else
                return NotFound("Product Not found");
        }
    }
}