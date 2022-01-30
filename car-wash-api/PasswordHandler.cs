using car_wash_api.Models;
using FireSharp;
using FireSharp.Interfaces;
using FireSharp.Response;
using System.Security.Cryptography;
using System.Text;

namespace car_wash_api
{
    public class PasswordHandler
    {
        static string salt = "0DguIusieUnqx2caYGAM";
        public static string Encrypt(string password)
        {
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                UTF8Encoding utf8 = new UTF8Encoding();
                byte[] bytes = md5.ComputeHash(utf8.GetBytes(password + salt));
                return Convert.ToBase64String(bytes);
            }
        }

        public async static Task<bool> CheckPassword(string password, string user_id, bool? admin_required = false)
        {
            IFirebaseClient client = new FirebaseClient(Firebase.FirebaseConfig);

            FirebaseResponse response = await client.GetAsync("Users/" + user_id);
            Dictionary<string, string> user = response.ResultAs<Dictionary<string, string>>();

            if (!user.ContainsKey("administrator"))
            {
                user["administrator"] = "false";
            }
            if (admin_required==true && user["administrator"] != "true")
            {
                return false;
            }

            return Encrypt(password) == user["password"];
        }
    }
}
