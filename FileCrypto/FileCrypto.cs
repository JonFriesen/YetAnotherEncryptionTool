using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using Microsoft.VisualBasic;
using System.Diagnostics;

namespace YetAnotherEncryptionTool
{
    public delegate void WorkEventHandler(object sender, CryptoEventArgs e);
    public delegate void WorkCompleteHandler(object sender, CryptoCompleteEventArgs e);

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
            Debug.WriteLine(DateTime.Now.ToString() + " --> Starting Encryption");
            var e = new CryptoEventArgs();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            await Task.Factory.StartNew(() =>
                {
                    try
                    {
                        using (FileStream fsCrypt = new FileStream(outputFile, FileMode.Create))
                        using (ICryptoTransform encryptor = CreateAESKey(skey).CreateEncryptor())
                        using (CryptoStream cs = new CryptoStream(fsCrypt, encryptor, CryptoStreamMode.Write))
                        using (FileStream fsIn = new FileStream(inputFile, FileMode.Open))
                        {
                            int data;
                            int counter = 1;
                            while ((data = fsIn.ReadByte()) != -1)
                            {
                                cs.WriteByte((byte)data);
                                e.CompletionPercentage = (int)((double)fsIn.Position / (double)fsIn.Length * 100);
                                //Through event on no more then a percentage
                                if (e.CompletionPercentage > counter)
                                {
                                    Debug.WriteLine("% " + e.CompletionPercentage + "\tPosition: " + fsIn.Position + "\tLength: " + fsIn.Length);
                                    WorkHandler(this, e);
                                    counter++;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Encryption Failed", ex);
                    }
                });
            sw.Stop();
            var c = new CryptoCompleteEventArgs();
            c.ElapsedTime = sw.Elapsed;
            CompleteHandler(this, c);
            Debug.WriteLine(DateTime.Now.ToString() + " <-- Finishing Encryption after " + sw.ElapsedMilliseconds);
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
            var e = new CryptoEventArgs();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            await Task.Factory.StartNew(() =>
                {
                    try
                    {
                        using (FileStream fsCrypt = new FileStream(inputFile, FileMode.Open))
                        using (FileStream fsOut = new FileStream(outputFile, FileMode.Create))
                        using (ICryptoTransform decryptor = CreateAESKey(skey).CreateDecryptor())
                        using (CryptoStream cs = new CryptoStream(fsCrypt, decryptor, CryptoStreamMode.Read))
                        {
                            int data;
                            int counter = 1;
                            while ((data = cs.ReadByte()) != -1)
                            {
                                fsOut.WriteByte((byte)data);
                                e.CompletionPercentage = (int)((double)fsOut.Position / (double)fsOut.Length * 100);
                                //Through event on no more then a percentage
                                if (e.CompletionPercentage > counter)
                                {
                                    Debug.WriteLine("% " + e.CompletionPercentage + "\tPosition: " + fsOut.Position + "\tLength: " + fsOut.Length);
                                    WorkHandler(this, e);
                                    counter++;
                                }
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Decryption Failed", ex);
                    }
                });
            sw.Stop();
            var c = new CryptoCompleteEventArgs();
            c.ElapsedTime = sw.Elapsed;
            CompleteHandler(this, c);
            Debug.WriteLine(DateTime.Now.ToString() + " <-- Finishing Decryption after " + sw.ElapsedMilliseconds);
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
        public int CompletionPercentage { get; set; }
    }
    /// <summary>
    /// Passes back the elapsed time of the crypto function
    /// </summary>
    public class CryptoCompleteEventArgs : EventArgs
    {
        public TimeSpan ElapsedTime { get; set; }
    }
}
