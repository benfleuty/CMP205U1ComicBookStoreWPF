using System;
using System.Collections.Generic;
using System.Text;
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
using DundeeComicBookStore.Helpers;
using DundeeComicBookStore.Interfaces;

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

        private void EmailTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoginButton.IsEnabled = VerifyLoginLengths();
        }

        private void PasswordPBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            LoginButton.IsEnabled = VerifyLoginLengths();
        }

        private bool VerifyLoginLengths()
        {
            return true; //(EmailTextBox.Text.Length >= 3 && PasswordPBox.Password.Length >= 6);
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // check email
            string email = EmailTextBox.Text;
            // invalid email
            if (!InputValidationHelper.ValidInput(EmailTextBox, EmailErrorMessage, ErrorHelper.UIError.InvalidEmail))
                return;
            // otherwise valid

            // check credentials
            string password = PasswordPBox.Password;

            IUser userLoggingIn = DBAccessHelper.GetUser(email, password);

            // credentials are invalid
            if (userLoggingIn == null)
            {
                ErrorHelper.ShowInputError(ErrorHelper.UIError.EmailPasswordComboNotRecognised, EmailErrorMessage);
                ErrorHelper.ShowInputError(ErrorHelper.UIError.EmailPasswordComboNotRecognised, PasswordErrorMessage);
                return;
            }
            // credentials are valid

            UserOptionsPage uop = new UserOptionsPage(userLoggingIn);
            ChangePageTo(uop);
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePageTo(new RegisterPage());
        }
    }
}