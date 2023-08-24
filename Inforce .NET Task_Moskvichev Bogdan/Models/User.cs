using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace Inforce_.NET_Task_Moskvichev_Bogdan.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string ? Email { get; set; }
        [Required]
        public string ? PasswordHash { get; set; } = string.Empty;
        [Required]
        public string ? Salt { get; set; }= string.Empty;
        public string ? Role { get; set; }=string.Empty;
        public void SetPassword(string password)
        {
            var saltBytes = GenerateSalt();
            PasswordHash = HashPassword(password, saltBytes);
            Salt = ConvertToBase64String(saltBytes);
        }

        public bool VerifyPassword(string ?password)
        {
            var saltBytes = ConvertFromBase64String(Salt);
            var hashedPassword = HashPassword(password, saltBytes);
            return string.Equals(hashedPassword, PasswordHash);
        }

        private byte[] GenerateSalt()
        {
            var saltBytes = new byte[32];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }
            return saltBytes;
        }

        private string HashPassword(string password, byte[] saltBytes)
        {
            using (var sha256 = SHA256.Create())
            {
                var passwordBytes = Encoding.UTF8.GetBytes(password);
                var combinedBytes = new byte[passwordBytes.Length + saltBytes.Length];
                Buffer.BlockCopy(passwordBytes, 0, combinedBytes, 0, passwordBytes.Length);
                Buffer.BlockCopy(saltBytes, 0, combinedBytes, passwordBytes.Length, saltBytes.Length);
                var hashedBytes = sha256.ComputeHash(combinedBytes);
                return ConvertToBase64String(hashedBytes);
            }
        }

        private string ConvertToBase64String(byte[] bytes)
        {
            return Convert.ToBase64String(bytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .TrimEnd('=');
        }

        private byte[] ConvertFromBase64String(string base64String)
        {
            base64String = base64String.Replace('-', '+').Replace('_', '/');
            switch (base64String.Length % 4)
            {
                case 2: base64String += "=="; break;
                case 3: base64String += "="; break;
            }
            return Convert.FromBase64String(base64String);
        }

    }
}
