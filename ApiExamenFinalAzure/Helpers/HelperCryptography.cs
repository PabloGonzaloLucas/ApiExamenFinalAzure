using ApiExamenFinalAzure.Models;
using Azure.Security.KeyVault.Secrets;
using System.Security.Cryptography;
using System.Text;

namespace ApiExamenFinalAzure.Helpers
{
    public static class HelperCryptography
    {
        private static string KeyCifrado;
        private static string ApiUrl;
        private static KeyVaultAccesorModel keyVaultSecrets;

        public static byte[] EncryptPassword(string password, string salt)
        {
            string contenido = password + salt;
            SHA512 managed = SHA512.Create();
            byte[] salida = Encoding.UTF8.GetBytes(contenido);
            for (int i = 1; i <= 15; i++)
            {
                salida = managed.ComputeHash(salida);
            }
            managed.Clear();
            return salida;
        }

        public static void Initialize(IConfiguration configuration, KeyVaultAccesorModel keyVaultSecret)
        {
            //KeyCifrado = configuration.GetValue<string>("Cypher:Key")
            keyVaultSecrets = keyVaultSecret;
            KeyCifrado = keyVaultSecret.CypherKey;
               
        }

        public static string CifrarString(string data)
        {
            //CONVERTIMOS A BYTES LA KEY
            byte[] keyData = Encoding.UTF8.GetBytes(KeyCifrado);
            string res = EncryptString(keyData, data);
            return res;
        }

        public static string DescifrarString(string data)
        {
            //CONVERTIMOS A BYTES LA KEY
            byte[] keyData = Encoding.UTF8.GetBytes(KeyCifrado);
            string res = DecryptString(keyData, data);
            return res;
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
