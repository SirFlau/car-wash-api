using car_wash_api.Models;
using FireSharp;
using FireSharp.Interfaces;
using FireSharp.Response;

namespace car_wash_api
{
    public class CarwashProcessor
    {
        public static Dictionary<string, Task?> tasks = new Dictionary<string, Task?>();
        public static async Task StopCarwash(string id)
        {
            if (tasks.ContainsKey(id) && tasks[id] != null)
            {
                tasks[id].Dispose();
                tasks[id] = null;
            }
        }
        public static async Task StartCarwash(Carwash carwash)
        {
            tasks[carwash.id] = ProcessCarwash(carwash);
            tasks[carwash.id].Wait();
            StopCarwash(carwash.id);
        }
        private static async Task ProcessCarwash(Carwash carwash)
        {
            IFirebaseClient client = new FirebaseClient(Firebase.FirebaseConfig);

            carwash.state = State.ON;
            await client.SetAsync("Carwashes/" + carwash.id, carwash);

            Thread.Sleep(60000);

            FirebaseResponse carwash_response = await client.GetAsync("Carwashes/" + carwash.id);
            carwash = carwash_response.ResultAs<Carwash>();
            if (carwash.state != State.ON)
            {
                return;
            }
            carwash.state = State.OFF;
            await client.SetAsync("Carwashes/" + carwash.id, carwash);
        }
    }
}
