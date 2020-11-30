using DundeeComicBookStore.Helpers;
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

namespace DundeeComicBookStore.Pages
{
    /// <summary>
    /// Interaction logic for RegisterPage.xaml
    /// </summary>
    public partial class RegisterPage : BasePage
    {
        //private List<Image> ppList = new List<Image>();

        public RegisterPage()
        {
            InitializeComponent();
        }

        private void FirstNameTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            InputValidationHelper.ValidateInput((TextBox)sender, FirstNameErrorMessage, ErrorHelper.UIError.RequiredField);
        }

        private void LastNameTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            InputValidationHelper.ValidateInput((TextBox)sender, LastNameErrorMessage, ErrorHelper.UIError.RequiredField);
        }

        private void EmailTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Check it is a valid email
            InputValidationHelper.ValidateInput((TextBox)sender, EmailErrorMessage, ErrorHelper.UIError.InvalidEmail);
            // Check it is not in use
            InputValidationHelper.ValidateInput((TextBox)sender, EmailErrorMessage, ErrorHelper.UIError.UsedEmail);
        }

        private void PasswordPBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            InputValidationHelper.ValidateInput((PasswordBox)sender, PasswordErrorMessage, ErrorHelper.UIError.PasswordComplexity);
        }

        private void PasswordConfirmPBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            InputValidationHelper.ValidateInput((PasswordBox)sender, PasswordPBox, PasswordConfirmErrorMessage, ErrorHelper.UIError.PasswordMismatch);
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePageTo(new LoginPage());
        }
    }
}