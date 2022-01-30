using car_wash_api.Models;
using FireSharp;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.AspNetCore.Mvc;

namespace car_wash_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {

        [HttpGet("")]
        public async Task<IActionResult> GetAsync(string email, string password)
        {
            if (!await PasswordHandler.CheckPassword(password, email))
            {
               return Unauthorized();
            }
            IFirebaseClient client = new FirebaseClient(Firebase.FirebaseConfig);

            FirebaseResponse response = await client.GetAsync("Users/" + email);
            UserView user = response.ResultAs<UserView>();

            user.firstname = await EncryptionHandler.Decrypt(user.firstname);
            user.lastname = await EncryptionHandler.Decrypt(user.lastname);

            return Ok(user);
        }

        [HttpPost("")]
        public async Task<IActionResult> PostAsync([FromBody] User user)
        {
            IFirebaseClient client = new FirebaseClient(Firebase.FirebaseConfig);

            // Check if users exists
            FirebaseResponse response = await client.GetAsync("Users/" + user.email);
            UserView user_exist = response.ResultAs<UserView>();
            if (user_exist != null){
                return BadRequest("user already exists");
            }

            user.subscription = false;
            user.administrator = false;
            user.password = PasswordHandler.Encrypt(user.password);


            await client.SetAsync("Users/" + user.email, user);

            return Ok();
        }

        [HttpPut("")]
        public async Task<IActionResult> PutAsync([FromBody] User user, string password)
        {
            bool check_admin_privalege = user.administrator == true;

            if (!await PasswordHandler.CheckPassword(password, user.email, check_admin_privalege))
            {
                return Unauthorized();
            }

            if (String.IsNullOrEmpty(user.password))
            {
                user.password = PasswordHandler.Encrypt(password);
            }
            else
            {
                user.password = PasswordHandler.Encrypt(user.password);
            }

            user.firstname = await EncryptionHandler.Encrypt(user.firstname);
            user.lastname = await EncryptionHandler.Encrypt(user.lastname);

            IFirebaseClient client = new FirebaseClient(Firebase.FirebaseConfig);

            await client.SetAsync("Users/" + user.email, user);

            return Ok();
        }
    }
}