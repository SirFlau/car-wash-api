using System.Security.Cryptography;
using System.Text;

namespace car_wash_api
{
    public class EncryptionHandler
    {
        private static string key = "FAB4640C65zeP7wfXZpKlWEA0eIZoMXI";
        private static string iv = "LKR2tf1xr1TM8OJ4";
        public static async Task<string?> Encrypt(string? raw)
        {
            if (raw is null)
            {
                return null;
            }
            byte[] data = UTF8Encoding.UTF8.GetBytes(raw);
            Aes cipher = Aes.Create();

            cipher.Key = Encoding.ASCII.GetBytes(key);
            cipher.IV = Encoding.ASCII.GetBytes(iv);

            ICryptoTransform encryptor = cipher.CreateEncryptor();
            byte[] encrypted_bytes = encryptor.TransformFinalBlock(data, 0, data.Length);
            return Convert.ToBase64String(encrypted_bytes);
        }

        public static async Task<string?> Decrypt(string? encrypted_data)
        {
            if (encrypted_data is null)
            {
                return null;
            }
            byte[] data = Convert.FromBase64String(encrypted_data);
            Aes cipher = Aes.Create();
            cipher.Key = Encoding.ASCII.GetBytes(key);
            cipher.IV = Encoding.ASCII.GetBytes(iv);

            ICryptoTransform encryptor = cipher.CreateDecryptor();
            byte[] decrypted_bytes = encryptor.TransformFinalBlock(data, 0, data.Length);
            return Encoding.UTF8.GetString(decrypted_bytes);

        }
    }
}
