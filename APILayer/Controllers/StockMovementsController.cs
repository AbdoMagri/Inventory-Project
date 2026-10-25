using InventoryAPI.Models;
using InventoryDB_Business;
using InventoryDB_DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryAPI.Controllers
{
    [Route("api/StockMovements")]
    [ApiController]
    public class StockMovementsController : ControllerBase
    {
        [HttpGet("All", Name = "GetAllMovements")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<clsStockMovementData.StockMovementDTO>> GetAllMovements(int AccountID)
        {
            return Ok(clsStockMovement.GetAllMovements(AccountID));
        }

        [HttpGet("Product/{productID:int}", Name = "GetMovementsByProduct")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<IEnumerable<clsStockMovementData.StockMovementDTO>> GetMovementsByProduct(int productID)
        {
            if (productID < 1)
                return BadRequest($"Not accepted ID {productID}");

            return Ok(clsStockMovement.GetMovementsByProductID(productID));
        }

        [HttpGet("{id:int}", Name = "GetMovementByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<clsStockMovementData.StockMovementDTO> GetMovementById(int id)
        {
            if (id < 1)
                return BadRequest($"Not accepted ID {id}");

            clsStockMovement Movement = clsStockMovement.Find(id);
            if (Movement == null)
                return NotFound($"Movement with ID {id} not found.");

            return Ok(Movement.DTO);
        }

        // QtyChange is signed: +5 found extra, -3 damaged/lost
        [HttpPost("Adjust", Name = "AdjustStock")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult AdjustStock(AdjustStockRequest request)
        {
            if (request == null || request.QtyChange == 0)
                return BadRequest("Invalid adjustment data.");

            string ErrorMessage = "";
            int MovementID = clsStockMovement.AdjustStock(request.AccountID, request.UserID,
                request.ProductID, request.QtyChange, request.Note, ref ErrorMessage);

            if (MovementID == -1)
                return BadRequest(ErrorMessage);

            return CreatedAtRoute("GetMovementByID", new { id = MovementID }, new { MovementID });
        }

        // physical count: send what you counted, the difference is calculated for you
        [HttpPost("AdjustToCount", Name = "AdjustStockToCount")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult AdjustStockToCount(AdjustToCountRequest request)
        {
            if (request == null || request.CountedQty < 0)
                return BadRequest("Invalid count data.");

            string ErrorMessage = "";
            int MovementID = clsStockMovement.AdjustStockToCount(request.AccountID, request.UserID,
                request.ProductID, request.CountedQty, request.Note, ref ErrorMessage);

            if (MovementID == -1)
                return BadRequest(ErrorMessage);

            return CreatedAtRoute("GetMovementByID", new { id = MovementID }, new { MovementID });
        }
    }
}