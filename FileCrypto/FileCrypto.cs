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
    public delegate void WorkEventHandler(object sender, CryptoEventArgs e);
    public delegate void WorkCompleteHandler(object sender, EventArgs e);

    public class FileCrypto
    {
        public event WorkEventHandler WorkHandler;
        public event WorkCompleteHandler CompleteHandler;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="outputFile"></param>
        /// <param name="skey"></param>
        public async void EncryptFile(string inputFile, string outputFile, string skey)
        {
            System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + " --> Starting Encryption");
            await Task.Factory.StartNew(() =>
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
                                            var e = new CryptoEventArgs();
                                            e.TotalWork = (int)fsIn.Length;
                                            e.WorkDone = (int)fsIn.Position;
                                            WorkHandler(this, e);
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
                });
            CompleteHandler(this, EventArgs.Empty);
            System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + " <-- Finishing Encryption");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="outputFile"></param>
        /// <param name="skey"></param>
        public async void DecryptFile(string inputFile, string outputFile, string skey)
        {
            System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + " --> Starting Decryption");
            await Task.Factory.StartNew(() =>
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
                                            var e = new CryptoEventArgs();
                                            e.TotalWork = (int)fsOut.Length;
                                            e.WorkDone = data;
                                            WorkHandler(this, e);
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
                });
            CompleteHandler(this, EventArgs.Empty);
            System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + " <-- Finishing Encryption");
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
    /// <summary>
    ///  EventArgs for getting the UI work down and work to go
    ///  to update the progress bar
    /// </summary>
    public class CryptoEventArgs : EventArgs
    {
        public int TotalWork { get; set; }
        public int WorkDone { get; set; }
    }
}
