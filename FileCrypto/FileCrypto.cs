using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using Microsoft.VisualBasic;

namespace YetAnotherEncryptionTool
{
    public class FileCrypto
    {
        public static void EncryptFile(string inputFile, string outputFile, string skey)
        {
            try
            {
                using (FileStream fsCrypt = new FileStream(outputFile, FileMode.Create))
                {
                    using (ICryptoTransform encryptor = CreateAESKey(skey).CreateEncryptor())
                    {
                        using (CryptoStream cs = new CryptoStream(fsCrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (FileStream fsIn = new FileStream(inputFile, FileMode.Open))
                            {
                                int data;
                                while ((data = fsIn.ReadByte()) != -1)
                                {
                                    cs.WriteByte((byte)data);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Encryption Failed", ex);
            }
        }
        public static void DecryptFile(string inputFile, string outputFile, string skey)
        {
            try
            {
                using (FileStream fsCrypt = new FileStream(inputFile, FileMode.Open))
                {
                    using (FileStream fsOut = new FileStream(outputFile, FileMode.Create))
                    {
                        using (ICryptoTransform decryptor = CreateAESKey(skey).CreateDecryptor())
                        {
                            using (CryptoStream cs = new CryptoStream(fsCrypt, decryptor, CryptoStreamMode.Read))
                            {
                                int data;
                                while ((data = cs.ReadByte()) != -1)
                                {
                                    fsOut.WriteByte((byte)data);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Decryption Failed", ex);
            }
        }
        /// <summary>
        /// Creates the AES key needed for encryption/decryption
        /// </summary>
        /// <param name="secret">The 'password' used for encryption</param>
        /// <returns>A SymmetricAlgorithm used to generate the encryption or decryption key</returns>
        public static SymmetricAlgorithm CreateAESKey(string secret)
        {
            DeriveBytes keys = new Rfc2898DeriveBytes(secret, Encoding.Unicode.GetBytes("This should not be...this..."));
            SymmetricAlgorithm aes = new RijndaelManaged();
            aes.BlockSize = 128;
            aes.KeySize = 256;
            aes.Key = keys.GetBytes(aes.KeySize >> 3);
            aes.IV = keys.GetBytes(aes.BlockSize >> 3); // From my understanding this shouldn't be the same key
            aes.Mode = CipherMode.CBC;
            return aes;
        }
    }
}
