using car_wash_api.Models;
using FireSharp;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.AspNetCore.Mvc;

namespace car_wash_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CarwashController : ControllerBase
    {

        [HttpGet("")]
        public async Task<IActionResult> GetAsync(string id, string email, string password)
        {
            if (!await PasswordHandler.CheckPassword(password, email, true))
            {
               return Unauthorized();
            }
            IFirebaseClient client = new FirebaseClient(Firebase.FirebaseConfig);

            FirebaseResponse response = await client.GetAsync("Carwashes/" + id);
            Carwash carwash = response.ResultAs<Carwash>();

            return Ok(carwash);

        }

        [HttpGet("list")]
        public async Task<IActionResult> ListAsync(string email, string password)
        {
            if (!await PasswordHandler.CheckPassword(password, email))
            {
                return Unauthorized();
            }
            IFirebaseClient client = new FirebaseClient(Firebase.FirebaseConfig);

            FirebaseResponse response = await client.GetAsync("Carwashes/");
            Dictionary<string, Carwash> washes = response.ResultAs<Dictionary<string, Carwash>>();
            return Ok(washes.Values.ToList());
        }

        [HttpPost("")]
        public async Task<IActionResult> PostAsync([FromBody] Carwash carwash, string email, string password)
        {
            if (!await PasswordHandler.CheckPassword(password, email, true))
            {
                return Unauthorized();
            }

            IFirebaseClient client = new FirebaseClient(Firebase.FirebaseConfig);

            FirebaseResponse response = await client.GetAsync("Carwashes/" + carwash.id);
            Carwash carwash_exist = response.ResultAs<Carwash>();
            if (carwash_exist != null){
                return BadRequest("carwash already exists");
            }

            await client.SetAsync("Carwashes/" + carwash.id, carwash);

            return Ok();
        }

        [HttpPut("")]
        public async Task<IActionResult> PutAsync([FromBody] Carwash carwash, string email, string password)
        {

            if (!await PasswordHandler.CheckPassword(password, email, true))
            {
                return Unauthorized();
            }

            IFirebaseClient client = new FirebaseClient(Firebase.FirebaseConfig);

            await client.SetAsync("Carwashes/" + carwash.id, carwash);

            return Ok();
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartAsync(string id, string email, string password, bool payment)
        {
            if (!await PasswordHandler.CheckPassword(password, email))
            {
                return Unauthorized();
            }

            IFirebaseClient client = new FirebaseClient(Firebase.FirebaseConfig);

            FirebaseResponse carwash_response = await client.GetAsync("Carwashes/" + id);
            Carwash carwash = carwash_response.ResultAs<Carwash>();
            if (carwash.state != State.OFF)
            {
                return BadRequest($"carwash cannot start because it is in state: {carwash.state}");
            }

            FirebaseResponse response = await client.GetAsync("Users/" + email);
            User user = response.ResultAs<User>();
            if (! (payment || user.subscription == true))
            {
                return BadRequest("missing payment");
            }
            Task.Run(() => CarwashProcessor.StartCarwash(carwash));
            return Ok();
        }

        [HttpPost("stop")]
        public async Task<IActionResult> StopAsync(string id, string email, string password)
        {
            if (!await PasswordHandler.CheckPassword(password, email, true))
            {
                return Unauthorized();
            }

            CarwashProcessor.StopCarwash(id);

            IFirebaseClient client = new FirebaseClient(Firebase.FirebaseConfig);
            FirebaseResponse response = await client.GetAsync("Carwashes/" + id);
            Carwash carwash = response.ResultAs<Carwash>();
            carwash.state = State.OFF;

            await client.SetAsync("Carwashes/" + id, carwash);
            
            return Ok();
        }

        [HttpPost("emergency/stop")]
        public async Task<IActionResult> EmergencyStopAsync(string id, string email, string password)
        {
            if (!await PasswordHandler.CheckPassword(password, email, true))
            {
                return Unauthorized();
            }

            CarwashProcessor.StopCarwash(id);

            IFirebaseClient client = new FirebaseClient(Firebase.FirebaseConfig);
            FirebaseResponse response = await client.GetAsync("Carwashes/" + id);
            Carwash carwash = response.ResultAs<Carwash>();
            carwash.state = State.SHUTDOWN;

            await client.SetAsync("Carwashes/" + id, carwash);

            return Ok();
        }
    }
}