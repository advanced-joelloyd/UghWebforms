using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace IRIS.Law.WebApp.Security
{
    public class AesDecryptor
    {
        private readonly string _sharedSecret;

        public AesDecryptor(string sharedSecret)
        {
            _sharedSecret = sharedSecret;
        }

        public string Decrypt(string ciphertext, string salt)
        {
            var key = new Rfc2898DeriveBytes(_sharedSecret, Convert.FromBase64String(salt));

            using (var aes = new AesManaged())
            {
                aes.Key = key.GetBytes(aes.KeySize / 8);
                aes.IV = key.GetBytes(aes.BlockSize / 8);

                var bytes = Convert.FromBase64String(ciphertext);
                using (var memoryStream = new MemoryStream())
                using (var cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(bytes, 0, bytes.Length);
                    cryptoStream.Close();
                    return Encoding.UTF8.GetString(memoryStream.ToArray());
                }
            }
        }
    }
}