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

namespace YetAnotherEncryptionToolGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private OpenFileDialog FileSelector;
        private SaveFileDialog FileSave;
        private enum Action { Encrypting, Decrypting };
        private Action taskType;
        public MainWindow()
        {
            FileSelector = new OpenFileDialog();
            FileSave = new SaveFileDialog();
            InitializeComponent();
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
                    taskType = Action.Decrypting;
                }
                else
                {
                    EncryptDecryptButton.Content = "Encrypt";
                    taskType = Action.Encrypting;
                }
            }
        }

        private void EncryptDecryptButton_Click(object sender, RoutedEventArgs e)
        {
            ProgressBar.Visibility = Visibility.Visible;
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
                if (taskType == Action.Encrypting)
                {
                    FileSave.Title = "Save encrypted file";
                    FileSave.DefaultExt = "enc";
                    FileSave.ValidateNames = true;
                    FileSave.FileName = FileSelector.FileName;
                    bool? result = FileSave.ShowDialog();
                }
                else
                {
                    string newFile = (FileSelector.FileName.Split('.'))[FileSelector.FileName.Split('.').Length - 1];
                    FileSave.Title = "Save decrypted file";
                    FileSave.DefaultExt = "";
                    FileSave.ValidateNames = true;
                    FileSave.FileName = newFile;
                    bool? result = FileSave.ShowDialog();
                }
            }
        }

        private bool ValidatePassword(string password)
        {
            string MatchPassPattern = "^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{8,32}$";
            if (password != null) return Regex.IsMatch(password, MatchPassPattern);
            else return false;
        }
    }
}
