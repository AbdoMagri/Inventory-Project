using InventoryDB_Business;
using InventoryDB_DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryAPI.Controllers
{
    [Route("api/Customers")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        [HttpGet("All", Name = "GetAllCustomers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<clsCustomerData.CustomerDTO>> GetAllCustomers(int AccountID)
        {
            return Ok(clsCustomer.GetAllCustomers(AccountID));
        }

        [HttpGet("{id:int}", Name = "GetCustomerByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<clsCustomerData.CustomerDTO> GetCustomerById(int id)
        {
            if (id < 1)
                return BadRequest($"Not accepted ID {id}");

            clsCustomer Customer = clsCustomer.Find(id);
            if (Customer == null)
                return NotFound($"Customer with ID {id} not found.");

            return Ok(Customer.DTO);
        }

        [HttpPost(Name = "AddCustomer")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<clsCustomerData.CustomerDTO> AddCustomer(clsCustomerData.CustomerDTO newCustomerDTO, int AccountID)
        {
            if (newCustomerDTO == null || string.IsNullOrWhiteSpace(newCustomerDTO.Name))
                return BadRequest("Invalid Customer Data");

            clsCustomer Customer = new clsCustomer(newCustomerDTO);
            if (!Customer.Save())
                return BadRequest("Failed to save the customer.");

            newCustomerDTO.CustomerID = Customer.CustomerID;
            return CreatedAtRoute("GetCustomerByID", new { id = newCustomerDTO.CustomerID }, newCustomerDTO);
        }

        [HttpPut("{id:int}", Name = "UpdateCustomer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<clsCustomerData.CustomerDTO> UpdateCustomer(int id, clsCustomerData.CustomerDTO updatedCustomer)
        {
            if (id < 1 || updatedCustomer == null || string.IsNullOrWhiteSpace(updatedCustomer.Name))
                return BadRequest("Invalid Customer Data.");

            clsCustomer Customer = clsCustomer.Find(id);
            if (Customer == null)
                return NotFound("Customer Not Found");

            Customer.Name = updatedCustomer.Name;
            Customer.Phone = updatedCustomer.Phone;

            if (!Customer.Save())
                return BadRequest("Failed to update the customer.");

            return Ok(Customer.DTO);
        }

        //[HttpDelete("{id:int}", Name = "DeleteCustomer")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //public ActionResult DeleteCustomer(int id)
        //{
        //    if (id < 1)
        //        return BadRequest("Invalid Customer Data");

        //    if (clsCustomer.DeleteCustomer(id))
        //        return Ok("Customer has been deleted");
        //    else
        //        return NotFound("Customer Not found");
        //}
    }
}