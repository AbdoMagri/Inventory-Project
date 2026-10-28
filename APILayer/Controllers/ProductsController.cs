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

        [HttpGet("{CurrentAccountID:int},{id:int}", Name = "GetProductByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<clsProductData.ProductDTO> GetProductById(int id, int CurrentAccountID)
        {
            if (id < 1)
                return BadRequest($"Not accepted ID {id}");

            clsProduct Product = clsProduct.Find(id);

            if (Product == null || Product.AccountID != CurrentAccountID)
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


        [HttpPost("{AccountID}", Name = "AddProduct")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public ActionResult<clsProductData.ProductDTO> AddProduct(
             clsProductData.ProductDTO newProductDTO, int AccountID)
        {
            if (newProductDTO == null ||
                string.IsNullOrWhiteSpace(newProductDTO.Name) ||
                string.IsNullOrWhiteSpace(newProductDTO.SKU))
            {
                return BadRequest("Invalid Product Data.");
            }

            if (AccountID <= 0)
                return BadRequest("Invalid AccountID.");

            if (clsProduct.isSKUExist(AccountID, newProductDTO.SKU))
                return Conflict("SKU already exists.");

            // Build the business-layer DTO.
            var productDTO = new clsProductData.ProductDTO(
    0, // ProductID: new product
    AccountID,
    newProductDTO.CategoryID,
    newProductDTO.UnitID,
    newProductDTO.Name,
    newProductDTO.SKU,
    newProductDTO.CostPrice,
    newProductDTO.SellPrice,
    newProductDTO.MinStock,
    newProductDTO.Quantity,
    newProductDTO.IsActive,
    newProductDTO.ImagePath ?? ""
);

            var product = new clsProduct(productDTO);

            if (!product.Save())
                return BadRequest("Failed to save the product.");

            // Return the saved product with its generated ID.
            productDTO.ProductID = product.ProductID;

            return StatusCode(201, productDTO);
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
        public ActionResult DeleteProduct(int AccountID, int id)
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