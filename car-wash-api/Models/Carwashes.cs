namespace car_wash_api.Models
{
    public enum State
    {
        ON,
        OFF,
        SHUTDOWN
    }
    public class Carwash
    {
        public string id { get; set; }
        public string name { get; set; }
        public State state { get; set; }

    }
}
