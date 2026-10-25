using InventoryDB_Business;
using InventoryDB_DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryAPI.Controllers
{
    [Route("api/Accounts")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        [HttpGet("All", Name = "GetAllAccounts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<clsAccountData.AccountDTO>> GetAllAccounts()
        {
            return Ok(clsAccount.GetAllAccounts());
        }

        [HttpGet("{id:int}", Name = "GetAccountByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<clsAccountData.AccountDTO> GetAccountById(int id)
        {
            if (id < 1)
                return BadRequest($"Not accepted ID {id}");

            clsAccount Account = clsAccount.Find(id);
            if (Account == null)
                return NotFound($"Account with ID {id} not found.");

            return Ok(Account.DTO);
        }

        [HttpPost(Name = "AddAccount")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<clsAccountData.AccountDTO> AddAccount(clsAccountData.AccountDTO newAccountDTO)
        {
            if (newAccountDTO == null || string.IsNullOrWhiteSpace(newAccountDTO.Name))
                return BadRequest("Invalid Account Data");

            clsAccount Account = new clsAccount(newAccountDTO);
            if (!Account.Save())
                return BadRequest("Failed to save the account.");

            newAccountDTO.AccountID = Account.AccountID;
            return CreatedAtRoute("GetAccountByID", new { id = newAccountDTO.AccountID }, newAccountDTO);
        }

        [HttpPut("{id:int}", Name = "UpdateAccount")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<clsAccountData.AccountDTO> UpdateAccount(int id, clsAccountData.AccountDTO updatedAccount)
        {
            if (id < 1 || updatedAccount == null || string.IsNullOrWhiteSpace(updatedAccount.Name))
                return BadRequest("Invalid Account Data.");

            clsAccount Account = clsAccount.Find(id);
            if (Account == null)
                return NotFound("Account Not Found");

            Account.Name = updatedAccount.Name;
            Account.ImagePath = updatedAccount.ImagePath;

            if (!Account.Save())
                return BadRequest("Failed to update the account.");

            return Ok(Account.DTO);
        }

        [HttpDelete("{id:int}", Name = "DeleteAccount")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteAccount(int id)
        {
            if (id < 1)
                return BadRequest("Invalid Account Data");

            if (clsAccount.DeleteAccount(id))
                return Ok("Account has been deleted");
            else
                return NotFound("Account Not found");
        }
    }
}