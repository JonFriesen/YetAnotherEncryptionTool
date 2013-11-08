using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YetAnotherEncryptionTool;

namespace YetAnotherEncryptionToolGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private OpenFileDialog FileSelector;
        private SaveFileDialog FileSave;
        private enum CryptoAction { Encrypting, Decrypting };
        private CryptoAction taskType;
        private FileCrypto crypto;
        public MainWindow()
        {
            FileSelector = new OpenFileDialog();
            FileSave = new SaveFileDialog();
            crypto = new FileCrypto();

            InitializeComponent();
        }

        void crypto_WorkHandler(object sender, CryptoEventArgs e)
        {
            this.Dispatcher.Invoke((Action)(() =>
                {
                    System.Diagnostics.Debug.WriteLine("Updating Progress Bar: " + e.WorkDone + " of " + e.TotalWork + " = " + ((double)e.WorkDone / (double)e.TotalWork)*100 + "%");
                    ProgressBar.Value = ((double)e.WorkDone / (double)e.TotalWork) * 100;
                }));
        }

        void crypto_CompleteHandler(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                System.Diagnostics.Debug.WriteLine("Encryption Complete");
                DisableUI(false);
            }));
        }

        private void FileSelectButton_Click(object sender, RoutedEventArgs e)
        {
            bool? result = FileSelector.ShowDialog();
            if (result == true)
            {
                FilePathLabel.Content = (FileSelector.FileName.Split('\\'))[FileSelector.FileName.Split('\\').Length - 1];
                if (FileSelector.FileName.EndsWith(".enc"))
                {
                    EncryptDecryptButton.Content = "Decrypt";
                    taskType = CryptoAction.Decrypting;
                }
                else
                {
                    EncryptDecryptButton.Content = "Encrypt";
                    taskType = CryptoAction.Encrypting;
                }
            }
        }

        private void EncryptDecryptButton_Click(object sender, RoutedEventArgs e)
        {
            crypto.WorkHandler += crypto_WorkHandler;
            crypto.CompleteHandler += crypto_CompleteHandler;
            if (taskType == CryptoAction.Encrypting)
            {
                FileSave.Title = "Save encrypted file";
                FileSave.DefaultExt = "enc";
                FileSave.ValidateNames = true;
                FileSave.FileName = FileSelector.FileName;
                bool? result = FileSave.ShowDialog();
                if (result == true)
                {
                    DisableUI(true);
                    ProgressBar.Visibility = Visibility.Visible;
                    crypto.EncryptFile(FileSelector.FileName, FileSave.FileName, PasswordField.Password);
                    return;
                }
            }
            else
            {
                string newFile = (FileSelector.FileName.Split('.'))[FileSelector.FileName.Split('.').Length - 1];
                FileSave.Title = "Save decrypted file";
                FileSave.DefaultExt = "enc";
                FileSave.ValidateNames = true;
                FileSave.FileName = newFile;
                bool? result = FileSave.ShowDialog();
                if (result == true)
                {
                    DisableUI(true);
                    ProgressBar.Visibility = Visibility.Visible;
                    crypto.DecryptFile(FileSelector.FileName, FileSave.FileName, PasswordField.Password);
                    return;
                }
            }
        }

        private void PasswordField_KeyUp(object sender, KeyEventArgs e)
        {
            if (!ValidatePassword(PasswordField.Password))
            {
                PasswordField.BorderBrush = Brushes.Red;
            }
            else
            {
                PasswordField.BorderBrush = Brushes.Green;
                EncryptDecryptButton.Visibility = Visibility.Visible;

            }
        }

        private bool ValidatePassword(string password)
        {
            string MatchPassPattern = "^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{8,32}$";
            if (password != null) return Regex.IsMatch(password, MatchPassPattern);
            else return false;
        }

        private void DisableUI(bool disableUI)
        {
            PasswordField.IsEnabled = !disableUI;
            SelectFileButton.IsEnabled = !disableUI;
            EncryptDecryptButton.IsEnabled = !disableUI;
        }
    }
}
