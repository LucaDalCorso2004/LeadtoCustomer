using LeadtoCustomer.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LeadtoCustomer.Controller
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class LeadController : ControllerBase
    {
        [Authorize(Roles = "Administrators")]
        [HttpDelete("{id}")]

        public IActionResult Delete(int id)
        {
            CustomersModel.DeleteCustomer(id);
            return NoContent(); 
        }




        [Authorize(Roles = "Administrators,Editor")]
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] LeadModel lead)
        {
            lead.Id = id;

            var existinglead = LeadsModel.GetLeadById(id);
            if (existinglead == null)
            {
                return NotFound("Lead not found");
            }

            foreach (var prop in typeof(LeadModel).GetProperties())
            {
                var newValue = prop.GetValue(lead);
                var oldValue = prop.GetValue(existinglead);

                if (newValue != null && newValue.ToString().ToLower() == "string")
                {
                
                    prop.SetValue(lead, oldValue);
                }
                else if (newValue != null)
                {
                  
                    prop.SetValue(existinglead, newValue);
                }
            }

            LeadsModel.Update(existinglead);
            return Ok(existinglead);
        }




        [Authorize(Roles = "Administrators,Editor,Viewer")]
        [HttpGet]

        public IEnumerable<LeadModel> Get()
        {
            var allLeads = LeadsModel.GetAllLeads();
            return allLeads;
        }


        [Authorize(Roles = "Administrators")]
        [HttpPost]
        public IActionResult Post([FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] LeadModel lead)
        {
            if (lead == null)
            {
                return BadRequest("Lead must not be null");
            }

            var leadModel = new LeadModel()
            {
                Name =  lead.Name,
                Gender = lead.Gender,
                Address = lead.Address,
                LeadSource = lead.LeadSource
            };

            bool result = LeadsModel.CreateLeads(leadModel);

            if (result)
            {
                return Ok("Lead was successfully created");
            }
            else
            {
                return StatusCode(500, "Lead could not be created");
            }
        }



    }
}
