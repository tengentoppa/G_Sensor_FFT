using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

//[20180221]Create by Simon
namespace MP_Moudule
{


    /// <summary>
    /// 處理字串的AES128加解密，請自備IV與Key，並注意資料長度
    /// </summary>
    public static class AES128inString
    {
        public static byte[] AES128Encrypt(string data, byte[] key, byte[] iv)
        {
            byte[] encrypted;
            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(data);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        public static string AES128Decrypt(List<byte> data, byte[] key, byte[] iv)
        {
            return AES128Decrypt(data.ToArray(), key, iv);
        }
        public static string AES128Decrypt(byte[] data, byte[] key, byte[] iv)
        {
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(data))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting 
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return plaintext;
        }
    }

    /// <summary>
    /// 處理AES128加解密，請自備IV與Key，並注意資料長度
    /// </summary>
    public static class AES128inByte
    {
        public static byte[] AES128Encrypt(List<byte> data, byte[] key, byte[] iv)
        {
            return AES128Encrypt(data.ToArray(), key, iv);
        }
        public static byte[] AES128Encrypt(byte[] data, byte[] key, byte[] iv)
        {
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                // Create a encrytor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(data, 0, data.Length);

                        byte[] b = new byte[data.Length];
                        Array.Copy(msEncrypt.ToArray(), b, data.Length);
                        WriteLog.Console("    Data    ", data);
                        return b;
                    }
                }
            }
        }

        public static byte[] AES128Decrypt(List<byte> data, byte[] key, byte[] iv)
        {
            return AES128Decrypt(data.ToArray(), key, iv);
        }
        public static byte[] AES128Decrypt(byte[] data, byte[] key, byte[] iv)
        {
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream())
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Write))
                    {
                        csDecrypt.Write(data, 0, data.Length);
                        csDecrypt.Close();
                    }
                    byte[] b = new byte[data.Length];
                    Array.Copy(msDecrypt.ToArray(), b, data.Length);

                    return b;
                }
            }
        }
    }
}
