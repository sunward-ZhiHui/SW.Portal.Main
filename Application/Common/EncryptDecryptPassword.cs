using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common
{
    public static class EncryptDecryptPassword
    {
        public static string Encrypt(string clearText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);

            using (Rijndael encryptor = RijndaelManaged.Create())
            {
                HashAlgorithm hashAlgo = new MD5CryptoServiceProvider();
                byte[] hash = hashAlgo.ComputeHash(Encoding.UTF8.GetBytes(EncryptionKey));
                encryptor.BlockSize = hash.Length * 8;
                encryptor.Key = hash;
                encryptor.IV = hash;

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        public static string Decrypt(string cipherText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] cipherBytes = Convert.FromBase64String(cipherText);

            using (Rijndael encryptor = RijndaelManaged.Create())
            {
                HashAlgorithm hashAlgo = new MD5CryptoServiceProvider();
                byte[] hash = hashAlgo.ComputeHash(Encoding.UTF8.GetBytes(EncryptionKey));
                encryptor.BlockSize = hash.Length * 8;
                encryptor.Key = hash;
                encryptor.IV = hash;

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray(), 0, ms.ToArray().Length);
                }
            }
            return cipherText;
        }
    }
}
