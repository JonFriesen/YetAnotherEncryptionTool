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
    class Program
    {
        static void Main(string[] args)
        {
            #region Parameter Parsing
            if (args.Length < 1) args = new String[] { @"\?" }; ;

            if (args[0] == @"\?")
            {
                Console.WriteLine("Name:\tYet Another Encryption Tool!");
                Console.WriteLine("Author:\tJon Friesen (www.jonfriesen.ca)");
                Console.WriteLine("Date:\tOct 28, 2013");
                Console.WriteLine("Version:\t1.0.0");
                Console.WriteLine("-----------------------------");
                Console.WriteLine();
                Console.WriteLine("Usage: -[D|E] [Path] [PassKey]");
                Console.WriteLine("\nOptions:");
                Console.WriteLine("-E\tEncrypt");
                Console.WriteLine("-D\tDecrypt");
                Console.WriteLine();
            }
            else
            {
                if (File.Exists(args[1]))
                {
                    string password;
                    if (args[2].Length < 1) throw new ArgumentNullException("No password set!");
                    if (args[2] == null)
                    {
                        password = Interaction.InputBox("Enter a password:", "Yet another encryption tool", "", 0, 0);
                    }
                    else
                    {
                        password = args[2];
                    }
                    if (args[0] == "-E")
                    {
                        Console.WriteLine("Attempting to encrypt: " + args[1]);
                        //FileCrypto.EncryptFile(args[1], args[1] + ".enc", password);
                    }
                    if (args[0] == "-D")
                    {
                        Console.WriteLine("Attempting to decrypt: " + args[1]);
                        string DecFileName = "";
                        foreach(string piece in args[1].Split('.'))
                        {
                            if (piece != "enc") DecFileName += piece + '.';
                        }
                        if (File.Exists(DecFileName))
                        {
                            Interaction.InputBox("Enter a new file name", "Yet another encryption tool", "", 0, 0);
                        }
                        //FileCrypto.DecryptFile(args[1], args[1], password);
                    }
                }
                else
                {
                    throw new FileNotFoundException("File not found at: " + args[1]);
                }
            }
            #endregion
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

    }
}
