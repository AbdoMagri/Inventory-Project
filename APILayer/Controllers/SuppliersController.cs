using InventoryDB_Business;
using InventoryDB_DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryAPI.Controllers
{
    [Route("api/Suppliers")]
    [ApiController]
    public class SuppliersController : ControllerBase
    {
        [HttpGet("All", Name = "GetAllSuppliers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<clsSupplierData.SupplierDTO>> GetAllSuppliers(int AccountID)
        {
            return Ok(clsSupplier.GetAllSuppliers(AccountID));
        }

        [HttpGet("{id:int}/{AccountID}", Name = "GetSupplierByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<clsSupplierData.SupplierDTO> GetSupplierById(int id, int AccountID)
        {
            if (id < 1)
                return BadRequest($"Not accepted ID {id}");

            clsSupplier Supplier = clsSupplier.Find(id);
            if (Supplier == null || Supplier.AccountID != AccountID)
                return NotFound($"Supplier with ID {id} not found.");

            return Ok(Supplier.DTO);
        }

        [HttpPost(Name = "AddSupplier")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<clsSupplierData.SupplierDTO> AddSupplier(clsSupplierData.SupplierDTO newSupplierDTO, int AccountID)
        {
            if (newSupplierDTO == null || string.IsNullOrWhiteSpace(newSupplierDTO.Name))
                return BadRequest("Invalid Supplier Data");

            clsSupplier Supplier = new clsSupplier(newSupplierDTO);
            if (!Supplier.Save())
                return BadRequest("Failed to save the supplier.");

            newSupplierDTO.SupplierID = Supplier.SupplierID;
            return CreatedAtRoute("GetSupplierByID", new { id = newSupplierDTO.SupplierID }, newSupplierDTO);
        }

        [HttpPut("{id:int}", Name = "UpdateSupplier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<clsSupplierData.SupplierDTO> UpdateSupplier(int id, clsSupplierData.SupplierDTO updatedSupplier)
        {
            if (id < 1 || updatedSupplier == null || string.IsNullOrWhiteSpace(updatedSupplier.Name))
                return BadRequest("Invalid Supplier Data.");

            clsSupplier Supplier = clsSupplier.Find(id);
            if (Supplier == null)
                return NotFound("Supplier Not Found");

            Supplier.Name = updatedSupplier.Name;
            Supplier.Phone = updatedSupplier.Phone;

            if (!Supplier.Save())
                return BadRequest("Failed to update the supplier.");

            return Ok(Supplier.DTO);
        }

        [HttpDelete("{id:int}", Name = "DeleteSupplier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteSupplier(int id)
        {
            if (id < 1)
                return BadRequest("Invalid Supplier Data");

            if (clsSupplier.DeleteSupplier(id))
                return Ok("Supplier has been deleted");
            else
                return NotFound("Supplier Not found");
        }
    }
}