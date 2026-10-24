using InventoryDB_Business;
using InventoryDB_DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryAPI.Controllers
{
    [Route("api/Users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        [HttpGet("All", Name ="GetAllUsers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<clsUserData.UserDTO>> GetAllUsers(int AccountID)
        {
            List<clsUserData.UserDTO> UsersList = clsUser.GetAllUsers(AccountID);
            if (UsersList.Count == 0)
            {
                return NotFound("No Users Found!");
            }
            return Ok(UsersList);
        }

        [HttpGet("one", Name = "GetUserByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<clsUserData.UserDTO> GetUserById(int id)
        {
            if (id < 1)
            {
                return BadRequest($"Not accepted ID {id}");
            }
            clsUser User = clsUser.FindByUserID(id);

            if (User == null) {
                return NotFound($"User with ID {id} not found.");
            }
            clsUserData.UserDTO UDTO = User.UDTO;

            return Ok(UDTO);
        }


        [HttpPost(Name ="AddUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<clsUserData.UserDTO> AddUser(clsUserData.UserDTO newUserDTO,int AccountID, string Password)
        {
            if (newUserDTO == null || string.IsNullOrEmpty(newUserDTO.UserName) || string.IsNullOrEmpty(newUserDTO.RoleName)) {
                return BadRequest("Invalid User Data");
            }

            clsUser User = new clsUser(new clsUserData.UserDTO(newUserDTO.Id, newUserDTO.UserName, newUserDTO.RoleName, newUserDTO.IsActive), AccountID, Password);
            User.Save();

            newUserDTO.Id = User.UserID;

            return CreatedAtRoute("GetUserById", new {id = newUserDTO.Id}, newUserDTO);
        }

    }
}
