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





    }
}
