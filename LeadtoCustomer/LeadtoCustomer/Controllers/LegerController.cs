using LeadtoCustomer.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LeadtoCustomer.Controller
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class LegerController : ControllerBase
    {
       
     
        
        
        [HttpGet]
    
        public IEnumerable<LegerModel> Get()
        {
            var allLedgers = LedgersModel.GetAllLedgers();
            return allLedgers;
        }



        [HttpPost]
        public IActionResult Post([FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] LegerModel ledger)
        {
            if (ledger == null)
            {
                return BadRequest("Ledger darf nicht null sein");
            }

            var legerModel = new LegerModel()
            {
                Name = ledger.Name,
                Gender = ledger.Gender,
                Adress = ledger.Adress,
                Leadsource = ledger.Leadsource
            };

            bool result = LedgersModel.CreateLedgers(legerModel);

            if (result)
            {
                return Ok("Ledger wurde erfolgreich erstellt");
            }
            else
            {
                return StatusCode(500, "Ledger konnte nicht erstellt werden");
            }
        }



    }
}

