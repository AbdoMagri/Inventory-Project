using InventoryDB_Business;
using InventoryDB_DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryAPI.Controllers
{
    [Route("api/Units")]
    [ApiController]
    public class UnitsController : ControllerBase
    {
        [HttpGet("All", Name = "GetAllUnits")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<clsUnitData.UnitDTO>> GetAllUnits()
        {
            return Ok(clsUnit.GetAllUnits());
        }

        [HttpGet("{id:int}", Name = "GetUnitByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<clsUnitData.UnitDTO> GetUnitById(int id)
        {
            if (id < 1)
                return BadRequest($"Not accepted ID {id}");

            clsUnit Unit = clsUnit.Find(id);
            if (Unit == null)
                return NotFound($"Unit with ID {id} not found.");

            return Ok(Unit.DTO);
        }

        [HttpPost(Name = "AddUnit")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<clsUnitData.UnitDTO> AddUnit(clsUnitData.UnitDTO newUnitDTO)
        {
            if (newUnitDTO == null || string.IsNullOrWhiteSpace(newUnitDTO.Name))
                return BadRequest("Invalid Unit Data");
            if (clsUnit.isNameUnitExist(newUnitDTO.Name))
            {
                return BadRequest("Unit Name is already exists");
            }
            clsUnit Unit = new clsUnit(newUnitDTO);
            if (!Unit.Save())
                return BadRequest("Failed to save the unit.");

            newUnitDTO.UnitID = Unit.UnitID;
            return CreatedAtRoute("GetUnitByID", new { id = newUnitDTO.UnitID }, newUnitDTO);
        }

        [HttpPut("{id:int}", Name = "UpdateUnit")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<clsUnitData.UnitDTO> UpdateUnit(int id, clsUnitData.UnitDTO updatedUnit)
        {
            if (id < 1 || updatedUnit == null || string.IsNullOrWhiteSpace(updatedUnit.Name))
                return BadRequest("Invalid Unit Data.");

            clsUnit Unit = clsUnit.Find(id);
            if (Unit == null)
                return NotFound("Unit Not Found");

            Unit.Name = updatedUnit.Name;

            if (!Unit.Save())
                return BadRequest("Failed to update the unit.");

            return Ok(Unit.DTO);
        }

        //[HttpDelete("{id:int}", Name = "DeleteUnit")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //public ActionResult DeleteUnit(int id)
        //{
        //    if (id < 1)
        //        return BadRequest("Invalid Unit Data");

        //    if (clsUnit.DeleteUnit(id))
        //        return Ok("Unit has been deleted");
        //    else
        //        return NotFound("Unit Not found");
        //}
    }
}