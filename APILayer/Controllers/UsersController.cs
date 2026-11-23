using InventoryDB_Business;
using InventoryDB_DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using System.Security.Claims;

namespace InventoryAPI.Controllers
{
    [Authorize]
    [Route("api/Users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        [Authorize(Roles = "Admin")]
        [HttpGet("All", Name ="GetAllUsers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<clsUserData.UserDTO>> GetAllUsers(int AccountID)
        {
            if (AccountID < 1)
                return BadRequest($"Invalid Account ID {AccountID}");
            if (!clsAccount.isAccountExist(AccountID))
                return NotFound("Account Not Found");
            List<clsUserData.UserDTO> UsersList = clsUser.GetAllUsers(AccountID);
            if (UsersList.Count == 0)
            {
                return NotFound("No Users Found!");
            }
            return Ok(UsersList);
        }

        [HttpGet("{id}", Name = "GetUserByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<clsUserData.UserDTO>> GetUserById( int id, [FromServices] IAuthorizationService authorizationService)
        {
            if (id < 1)
            {
                return BadRequest($"Not accepted ID {id}");
            }
            clsUser UserD = clsUser.FindByUserID(id);

            if (UserD == null ) {
                return NotFound($"User with ID {id} not found.");
            }

            var authResult = await authorizationService.AuthorizeAsync(
     User,
     new OwnershipResource
     {
         Id = id,
         AdminCheack = true,
         Type = OwnershipType.User
     },
     "UserOrAccountOwner"
 );

            if (!authResult.Succeeded)
                return Forbid();

            clsUserData.UserDTO UDTO = UserD.UDTO;

            return Ok(UDTO);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost(Name ="AddUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<clsUserData.UserDTO> AddUser(clsUserData.UserDTO newUserDTO,int AccountID, string Password)
        {
            if (newUserDTO == null || string.IsNullOrEmpty(newUserDTO.UserName) || string.IsNullOrEmpty(newUserDTO.RoleName)) {
                return BadRequest("Invalid User Data");
            }
            if (clsUser.isUserExist(newUserDTO.UserName))
                return BadRequest("Username already exists");

            clsUser User = new clsUser(new clsUserData.UserDTO(newUserDTO.Id, newUserDTO.UserName, newUserDTO.RoleName, newUserDTO.IsActive), AccountID, Password);
            User.Save();

            newUserDTO.Id = User.UserID;

            return CreatedAtRoute("GetUserById", new {AccountID ,id = newUserDTO.Id}, newUserDTO);
        }

        
        [HttpPut("{id}",Name ="UpdateUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<clsUserData.UserDTO>> UpdateUser(int id, clsUserData.UserDTO updatedUser, [FromServices] IAuthorizationService authorizationService)
        {
            if (id < 1 || updatedUser == null || string.IsNullOrEmpty(updatedUser.UserName))
            {
                return BadRequest("Invalid User Data.");
            }
            clsUser UserD = clsUser.FindByUserID(id);

            if (UserD == null) {
                return NotFound($"User Not Found");
            }
            var authResult = await authorizationService.AuthorizeAsync(
    User,
    new OwnershipResource
    {
        Id = id,
        AdminCheack = true,
        Type = OwnershipType.User
    },
    "UserOrAccountOwner"
);

            if (!authResult.Succeeded)
                return Forbid();
            UserD.UserName = updatedUser.UserName;
            UserD.RoleInfo = clsRole.FindByName(updatedUser.RoleName);
            UserD.RoleID = UserD.RoleInfo.RoleID;
            UserD.IsActive = updatedUser.IsActive;
            UserD.Save();

            return Ok(UserD.UDTO);
        }

        
        [HttpPatch("{id:int}", Name = "ChangePassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangeUserPassword(
            int id,string OldPassword, [FromBody] string NewPassword, [FromServices] IAuthorizationService authorizationService)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid user ID.");
            }

            if (string.IsNullOrWhiteSpace(NewPassword))
            {
                return BadRequest("Password cannot be empty.");
            }
            var authResult = await authorizationService.AuthorizeAsync(
    User,
    new OwnershipResource
    {
        Id = id,
        AdminCheack = true,
        Type = OwnershipType.User
    },
    "UserOrAccountOwner"
);

            if (!authResult.Succeeded)
                return Forbid();

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

            if (!user.VerifyPassword(OldPassword))
            {
                return BadRequest("password incorrect");
            }

            if (!user.ChangePassword(NewPassword))
            {
                return BadRequest("Failed to change the password.");
            }

            return Ok("Password changed successfully.");
        }
        [Authorize(Roles = "Admin")]
        [HttpPatch("{AccountID:int}/{id:int}/Deactivate", Name = "DeactivateUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public ActionResult DeactivateUser(int AccountID, int id, [FromQuery] int actingUserID)
        {
            
            if (id < 1 || AccountID < 1 || actingUserID < 1)
                return BadRequest("Invalid user data.");

            try
            {
                clsUser.DeactivateUser(AccountID, id, actingUserID);
                return Ok("User has been deactivated.");
            }
            catch (SqlException ex) when (ex.Number == 50051)
            {
                return NotFound(ex.Message);
            }
            catch (SqlException ex) when (ex.Number >= 50052 && ex.Number <= 50054)
            {
                return Conflict(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Could not deactivate user.");
            }
        }


        //[HttpDelete("{id:int}", Name = "DeleteUser")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]

        //public ActionResult DeleteUser(int id)
        //{
        //    if (id < 0) {
        //        return BadRequest("Invalid User Data");
        //    }

        //    if (clsUser.DeleteUser(id))
        //        return Ok($"User has been deleted");
        //    else
        //        return NotFound("User Not found");

        //}
    }
}
