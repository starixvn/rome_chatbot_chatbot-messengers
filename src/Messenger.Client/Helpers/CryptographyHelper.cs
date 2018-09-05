namespace Messenger.Client.Helpers
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    public interface ICryptographyHelper
    {
        string Encrypt(string message, string key, string salt);

        string Decrypt(string encryptedMessage, string key, string salt);

        string Hash(string message);
    }

    public class CryptographyHelper : ICryptographyHelper
    {
        public string Decrypt(string encryptedMessage, string key, string salt)
        {
            // Check arguments.
            if (encryptedMessage == null || encryptedMessage.Length <= 0)
            {
                throw new ArgumentNullException(nameof(encryptedMessage));
            }

            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (salt == null || salt.Length <= 0)
            {
                throw new ArgumentNullException(nameof(salt));
            }

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an RijndaelManaged object
            // with the specified key and IV.
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = ConvertHexStringToByte(key);
                rijAlg.IV = ComputeSalt(salt);

                // Create a decrytor to perform the stream transform.
                var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(encryptedMessage)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }

        public string Encrypt(string message, string key, string salt)
        {
            // Check arguments.
            if (message == null || message.Length <= 0)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (salt == null || salt.Length <= 0)
            {
                throw new ArgumentNullException(nameof(salt));
            }

            byte[] encrypted;
            // Create an RijndaelManaged object
            // with the specified key and IV.
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = ConvertHexStringToByte(key);
                rijAlg.IV = ComputeSalt(salt);

                // Create a decryptor to perform the stream transform.
                var encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(message);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return Convert.ToBase64String(encrypted);
        }

        public string Hash(string message)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);
            var sha = new SHA1CryptoServiceProvider();
            var result = sha.ComputeHash(messageBytes);

            return ConvertHexByteToString(result);
        }

        private static string ConvertHexByteToString(byte[] data)
            => BitConverter.ToString(data).Replace("-", "");

        private static byte[] ConvertHexStringToByte(string hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        public static byte[] ComputeSalt(string salt)
        {
            MD5 md5 = MD5.Create();
            byte[] source = Encoding.Default.GetBytes(salt);
            return md5.ComputeHash(source);
        }
    }
}