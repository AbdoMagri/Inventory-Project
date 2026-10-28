using InventoryAPI.Models;
using InventoryDB_Business;
using InventoryDB_DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryAPI.Controllers
{
    [Route("api/Purchases")]
    [ApiController]
    public class PurchasesController : ControllerBase
    {
        [HttpGet("All", Name = "GetAllPurchases")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<IEnumerable<clsPurchaseData.PurchaseDTO>> GetAllPurchases(
    int AccountID)
        {
            if (AccountID < 1)
                return BadRequest($"Invalid Account ID {AccountID}");

            return Ok(clsPurchase.GetAllPurchases(AccountID));
        }

        [HttpGet("{AccountId:int}/{id:int}", Name = "GetPurchaseByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<clsPurchaseData.PurchaseDTO> GetPurchaseById(int AccountId, int id)
        {
            if (id < 1)
                return BadRequest($"Not accepted ID {id}");

            clsPurchase Purchase = clsPurchase.Find(id);
            if (Purchase == null || Purchase.AccountID != AccountId)
                return NotFound($"Purchase with ID {id} not found.");

            return Ok(Purchase.DTO);
        }

        [HttpPost(Name = "MakePurchase")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult MakePurchase(MakePurchaseRequest request)
        {
            if (request == null || request.Items == null || request.Items.Count == 0)
                return BadRequest("Purchase has no items.");

            List<clsPurchaseItem> Items = request.Items.Select(i => new clsPurchaseItem
            {
                ProductID = i.ProductID,
                Qty = i.Qty,
                UnitCost = i.UnitCost
            }).ToList();

            string ErrorMessage = "";
            int PurchaseID = clsPurchase.MakePurchase(request.AccountID, request.UserID, request.SupplierID, Items, ref ErrorMessage);

            if (PurchaseID == -1)
                return BadRequest(ErrorMessage);

            return CreatedAtRoute(
    "GetPurchaseByID",
    new
    {
        AccountId = request.AccountID,
        id = PurchaseID
    },
    new { PurchaseID });
        }
    }
}