using LeadtoCustomer.Model;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;


[Route("api/v1/[controller]")]
[ApiController]
public class LoginController : ControllerBase
{
    [HttpPost]

    public async Task<IActionResult> Post([FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] LoginModel login)
    {
        return await Task.Run(() =>
        {
            IActionResult response = Unauthorized();

            UserModel user = UsersModel.Authenticate(login);

            if (user != null)
                response = Ok(new { token = LoginModel.CreateJWT(user) });

            return response;
        });
    }
}