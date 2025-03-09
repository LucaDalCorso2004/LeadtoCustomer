using LeadtoCustomer.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LeadtoCustomer.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {


        [Authorize(Roles = "Administrators,Editor,Viewer")]
        [HttpGet]
            public IEnumerable<CustomerModel> Get()
        {
            var allCustomers = CustomersModel.GetAllCustomers();
            return allCustomers;
        }

        [Authorize(Roles = "Administrators")]
        [HttpPost("{id}")]
        public IActionResult Post(int id)
        {
           

   
            bool result = CustomersModel.TransferLeadToCustomer(id);

            if (!result)
            {
                return StatusCode(500, "Lead could not be converted into Customer");
            }

         
           
             LeadsModel.DeleteLead(id);

         


            return Ok("Lead was successfully converted to Customer and deleted from Leads");
        }

        [Authorize(Roles = "Administrators")]
        [HttpDelete("{id}")]

        public void Delete(int id)
        {


       CustomersModel.DeleteCustomer(id);

        }



        [Authorize(Roles = "Administrators,Editor")]
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] CustomerModel customer)
        {
            customer.Id = id;

            var existingcustomer = CustomersModel.GetCustomerById(id);
            if (existingcustomer == null)
            {
                return NotFound("Lead not found");
            }

            foreach (var prop in typeof(CustomerModel).GetProperties())
            {
                var newValue = prop.GetValue(customer);
                var oldValue = prop.GetValue(existingcustomer);

                if (newValue != null && newValue.ToString().ToLower() == "string")
                {

                    prop.SetValue(customer, oldValue);
                }
                else if (newValue != null)
                {

                    prop.SetValue(existingcustomer, newValue);
                }
            }

            CustomersModel.Update(existingcustomer);
            return Ok(existingcustomer);
        }






    }
}
