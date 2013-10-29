using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace YetAnotherEncryptionTool
{
    class Program
    {
        static void Main(string[] args)
        {
            //#region Parameter Parsing
            //if (args.Length < 1) args = new String[] { @"\?" }; ;

            //if (args[0] == @"\?")
            //{
            //    Console.WriteLine("Name:\tYet Another Encryption Tool!");
            //    Console.WriteLine("Author:\tJon Friesen (www.jonfriesen.ca)");
            //    Console.WriteLine("Date:\tOct 28, 2013");
            //    Console.WriteLine("-----------------------------");
            //    Console.WriteLine();
            //    Console.WriteLine("Usage: -[D|E] [Path] [PassKey]");
            //    Console.WriteLine("\nOptions:");
            //    Console.WriteLine("-E\tEncrypt");
            //    Console.WriteLine("-D\tDecrypt");
            //    Console.WriteLine();
            //}
            //else
            //{
            //    if (File.Exists(args[1]))
            //    {
            //        if (args[2].Length < 1) throw new ArgumentNullException("No password set!");
            //        //if (ASCIIEncoding.UTF8.GetBytes(args[2]).Length < 16) throw new ArgumentNullException("Password is too short!");
            //        if (args[0] == "-E")
            //        {
            //            Console.WriteLine("Attempting to encrypt: " + args[1]);
            //            EncryptFile(args[1], args[1] + ".enc", args[2]);
            //        }
            //        if (args[0] == "-D")
            //        {
            //            Console.WriteLine("Attempting to decrypt: " + args[1]);
            //            DecryptFile(args[1], args[1], args[2]);
            //        }
            //    }
            //    else
            //    {
            //        throw new FileNotFoundException("File not found at: " + args[1]);
            //    }
            //}
            //#endregion
            string file = @"C:\Users\Jon\Desktop\Test.txt";
            string pass = "hello";
            EncryptFile(file, file+".enc", pass);
            DecryptFile(file+".enc", file + ".dec", pass);
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
        private static void EncryptFile(string inputFile, string outputFile, string skey)
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
                throw new Exception("Oh snap! Encryption Failed!!!", ex);
            }
        }
        private static void DecryptFile(string inputFile, string outputFile, string skey)
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
                throw new Exception("Oh snap! Decryption Failed!!!", ex);
            }
        }
        /// <summary>
        /// Creates the AES key needed for encryption/decryption
        /// </summary>
        /// <param name="secret">The 'password' used for encryption</param>
        /// <returns>A SymmetricAlgorithm used to generate the encryption or decryption key</returns>
        private static SymmetricAlgorithm CreateAESKey(string secret)
        {
            DeriveBytes keys = new Rfc2898DeriveBytes(secret, Encoding.Unicode.GetBytes("This should not be...this..."));
            SymmetricAlgorithm aes = new RijndaelManaged();
            aes.BlockSize = 128;
            aes.KeySize = 256;
            aes.Key = keys.GetBytes(aes.KeySize >> 3);
            aes.IV = keys.GetBytes(aes.BlockSize >> 3); // this shouldn't be the same as the key... >.>
            aes.Mode = CipherMode.CBC;
            return aes;
        }
    }
}
