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


        [HttpPut("{id}",Name ="UpdateUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<clsUserData.UserDTO> UpdateUser(int id, clsUserData.UserDTO updatedUser)
        {
            if (id < 1 || updatedUser == null || string.IsNullOrEmpty(updatedUser.UserName))
            {
                return BadRequest("Invalid User Data.");
            }
            clsUser User = clsUser.FindByUserID(id);

            if (User == null) {
                return NotFound($"User Not Found");
            }
            User.UserName = updatedUser.UserName;
            User.RoleInfo = clsRole.FindByName(updatedUser.RoleName);
            User.RoleID = User.RoleInfo.RoleID;
            User.IsActive = updatedUser.IsActive;
            User.Save();

            return Ok(User.UDTO);
        }


        [HttpPatch("{id:int}", Name = "ChangePassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult ChangeUserPassword(
            int id, [FromBody] string NewPassword)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid user ID.");
            }

            if (string.IsNullOrWhiteSpace(NewPassword))
            {
                return BadRequest("Password cannot be empty.");
            }

            NewPassword = NewPassword.Trim();

            if (NewPassword.Length < 8)
            {
                return BadRequest("Password must be at least 8 characters long.");
            }

            if (NewPassword.Length > 20)
            {
                return BadRequest("Password cannot exceed 20 characters.");
            }

            clsUser user = clsUser.FindByUserID(id);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            if (!user.ChangePassword(NewPassword))
            {
                return BadRequest("Failed to change the password.");
            }

            return Ok("Password changed successfully.");
        }


        [HttpDelete("{id:int}", Name = "DeleteUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult DeleteUser(int id)
        {
            if (id < 0) {
                return BadRequest("Invalid User Data");
            }

            if (clsUser.DeleteUser(id))
                return Ok($"User has been deleted");
            else
                return NotFound("User Not found");
            
        }
    }
}
