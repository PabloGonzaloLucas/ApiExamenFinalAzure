using ApiExamenFinalAzure.Models;
using System.Security.Cryptography;
using System.Text;

namespace ApiExamenFinalAzure.Helpers
{
    public static class HelperCryptography
    {
        private static string KeyCifrado = string.Empty;

        public static void Initialize(IConfiguration configuration, KeyVaultAccesorModel keyVaultSecrets)
        {
            // Use the cypher key already resolved from Key Vault at startup
            KeyCifrado = keyVaultSecrets.CypherKey ?? string.Empty;
        }

        public static string CifrarString(string data)
        {
            byte[] keyData = Encoding.UTF8.GetBytes(KeyCifrado);
            return EncryptString(keyData, data);
        }

        public static string DescifrarString(string data)
        {
            byte[] keyData = Encoding.UTF8.GetBytes(KeyCifrado);
            return DecryptString(keyData, data);
        }

        private static string EncryptString(byte[] key, string plainText)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        private static string DecryptString(byte[] key, string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
