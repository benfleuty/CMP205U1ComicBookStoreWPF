using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DundeeComicBookStore.Pages;

namespace DundeeComicBookStore.Pages
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : BasePage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void LoginTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoginButton.IsEnabled = VerifyLoginLengths();
        }

        private void PasswordPBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            LoginButton.IsEnabled = VerifyLoginLengths();
        }

        private bool VerifyLoginLengths()
        {
            return (UsernameTextBox.Text.Length >= 3 && PasswordPBox.Password.Length >= 6);
        }

        private void LoginButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string message = $"{UsernameTextBox.Text} tried logging in with the password:\n {PasswordPBox.Password}";
            MessageBox.Show(message);
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePageTo(new RegisterPage());
        }
    }
}