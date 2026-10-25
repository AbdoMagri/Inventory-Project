using InventoryAPI.Models;
using InventoryDB_Business;
using InventoryDB_DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryAPI.Controllers
{
    [Route("api/Sales")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        [HttpGet("All", Name = "GetAllSales")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<clsSaleData.SaleDTO>> GetAllSales(int AccountID)
        {
            return Ok(clsSale.GetAllSales(AccountID));
        }

        [HttpGet("{id:int}", Name = "GetSaleByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<clsSaleData.SaleDTO> GetSaleById(int id)
        {
            if (id < 1)
                return BadRequest($"Not accepted ID {id}");

            clsSale Sale = clsSale.Find(id);
            if (Sale == null)
                return NotFound($"Sale with ID {id} not found.");

            return Ok(Sale.DTO);
        }

        [HttpPost(Name = "MakeSale")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult MakeSale(MakeSaleRequest request)
        {
            if (request == null || request.Items == null || request.Items.Count == 0)
                return BadRequest("Sale has no items.");

            List<clsSaleItem> Items = request.Items.Select(i => new clsSaleItem
            {
                ProductID = i.ProductID,
                Qty = i.Qty,
                UnitPrice = i.UnitPrice
            }).ToList();

            string ErrorMessage = "";
            int SaleID = clsSale.MakeSale(request.AccountID, request.UserID, request.CustomerID, Items, ref ErrorMessage);

            if (SaleID == -1)
                return BadRequest(ErrorMessage);   // e.g. "Insufficient stock for one or more products."

            return CreatedAtRoute("GetSaleByID", new { id = SaleID }, new { SaleID });
        }
    }
}