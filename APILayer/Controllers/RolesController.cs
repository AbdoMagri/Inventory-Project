using InventoryDB_Business;
using InventoryDB_DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryAPI.Controllers
{
    [Route("api/Roles")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        [HttpGet("All", Name = "GetAllRoles")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<clsRoleData.RoleDTO>> GetAllRoles()
        {
            return Ok(clsRole.GetAllRoles());
        }

        [HttpGet("{id:int}", Name = "GetRoleByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<clsRoleData.RoleDTO> GetRoleById(int id)
        {
            if (id < 1)
                return BadRequest($"Not accepted ID {id}");

            clsRole Role = clsRole.Find(id);
            if (Role == null)
                return NotFound($"Role with ID {id} not found.");

            return Ok(Role.DTO);
        }

        // names of the permissions this role has, for showing/hiding pages in React
        [HttpGet("{id:int}/permissions", Name = "GetRolePermissions")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<string>> GetRolePermissions(int id)
        {
            if (!clsRole.isRoleExist(id))
                return NotFound("Role Not found");

            return Ok(Enum.GetValues<clsPermissions.enPermission>()
                .Where(p => clsPermissions.HasPermission(id, p))
                .Select(p => p.ToString()));
        }

        [HttpPost(Name = "AddRole")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<clsRoleData.RoleDTO> AddRole(clsRoleData.RoleDTO newRoleDTO)
        {
            if (newRoleDTO == null || string.IsNullOrWhiteSpace(newRoleDTO.Name))
                return BadRequest("Invalid Role Data");

            clsRole Role = new clsRole(newRoleDTO);
            if (!Role.Save())
                return BadRequest("Failed to save the role.");

            newRoleDTO.RoleID = Role.RoleID;
            return CreatedAtRoute("GetRoleByID", new { id = newRoleDTO.RoleID }, newRoleDTO);
        }

        [HttpPut("{id:int}", Name = "UpdateRole")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<clsRoleData.RoleDTO> UpdateRole(int id, clsRoleData.RoleDTO updatedRole)
        {
            if (id < 1 || updatedRole == null || string.IsNullOrWhiteSpace(updatedRole.Name))
                return BadRequest("Invalid Role Data.");

            clsRole Role = clsRole.Find(id);
            if (Role == null)
                return NotFound("Role Not Found");

            Role.Name = updatedRole.Name;

            if (!Role.Save())
                return BadRequest("Failed to update the role.");

            return Ok(Role.DTO);
        }

        [HttpDelete("{id:int}", Name = "DeleteRole")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteRole(int id)
        {
            if (id < 1)
                return BadRequest("Invalid Role Data");

            if (clsRole.DeleteRole(id))
                return Ok("Role has been deleted");
            else
                return NotFound("Role Not found");
        }
    }
}