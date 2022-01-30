namespace car_wash_api.Models
{
    public class User
    {
        public string email { get; set; }
        public bool? subscription { get; set; }

        public string? password { get; set; }

        public string? firstname { get; set; }
        public string? lastname { get; set; }

        public bool? administrator { get; set; }
    }

    public class UserView
    {
        public string email { get; set; }
        public bool subscription { get; set; }
        public string? firstname { get; set; }
        public string? lastname { get; set; }
        public string? administrator { get; set; }
    }
}