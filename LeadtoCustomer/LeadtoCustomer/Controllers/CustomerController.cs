using LeadtoCustomer.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LeadtoCustomer.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
    
       

        [HttpGet]
            public IEnumerable<CustomerModel> Get()
        {
            var allCustomers = CustomersModel.GetAllCustomers();
            return allCustomers;
        }

        [HttpPost("{id}")]
        public IActionResult Post(int id)
        {
           

       

            // Erstelle Customer und überprüfe den Erfolg
            bool result = CustomersModel.TransferLeadToCustomer(id);

            if (!result)
            {
                return StatusCode(500, "Lead could not be converted into Customer");
            }

            // Wenn Customer erstellt wurde, versuche das Lead zu löschen
           
             LeadsModel.DeleteLead(id);

         

            // Erfolgreich, gebe eine Bestätigung zurück
            return Ok("Lead was successfully converted to Customer and deleted from Leads");
        }
        [HttpDelete("{id}")]

        public void Delete(int id)
        {


       CustomersModel.DeleteCustomer(id);

        }




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
