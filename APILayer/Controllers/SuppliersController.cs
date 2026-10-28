using InventoryDB_Business;
using InventoryDB_DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

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
            if (!clsAccount.isAccountExist(AccountID))
            {
                return NotFound("Account not found");
            }
            if (newSupplierDTO == null || string.IsNullOrWhiteSpace(newSupplierDTO.Name))
                return BadRequest("Invalid Supplier Data");

            clsSupplier Supplier = new clsSupplier(newSupplierDTO);
            Supplier.AccountID = AccountID;
            if (!Supplier.Save())
                return BadRequest("Failed to save the supplier.");

            newSupplierDTO.SupplierID = Supplier.SupplierID;
            return CreatedAtRoute("GetSupplierByID", new { id = newSupplierDTO.SupplierID, newSupplierDTO.AccountID }, newSupplierDTO);
        }

        [HttpPut("{id:int}/{AccountID:int}", Name = "UpdateSupplier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<clsSupplierData.SupplierDTO> UpdateSupplier(int id, int AccountID, clsSupplierData.SupplierDTO updatedSupplier)
        {
            if (id < 1 || updatedSupplier == null || string.IsNullOrWhiteSpace(updatedSupplier.Name))
                return BadRequest("Invalid Supplier Data.");

            clsSupplier Supplier = clsSupplier.Find(id);
            if (Supplier == null || Supplier.AccountID != AccountID)
                return NotFound("Supplier Not Found");

            Supplier.Name = updatedSupplier.Name;
            Supplier.Phone = updatedSupplier.Phone;
            
                if (!Supplier.Save())
                    return BadRequest("Failed to update the supplier.");
            
            

            return Ok(Supplier.DTO);
        }

        [HttpDelete("{AccountID:int}/{id:int}", Name = "DeleteSupplier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteSupplier(int AccountID, int id)
        {
            if (id < 1)
                return BadRequest("Invalid Supplier Data");

            try
            {
                if (clsSupplier.DeleteSupplier(AccountID, id))
                    return Ok("Supplier has been deleted");
            }
            catch (SqlException ex) when (ex.Number == 50041)
            {
                return NotFound(ex.Message);
            }
            catch (SqlException ex) when (ex.Number == 50042)
            {
                return Conflict(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Could not delete supplier");
            }

            return BadRequest("Could not delete supplier");
        }
    }
}