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

        private readonly Dictionary<UIError, string> UIErrorMessages = new Dictionary<UIError, string>()
        {
            { UIError.RequiredField, "This field is required" },
            {UIError.InvalidEmail, "This is not a valid email address" },
            {UIError.UsedEmail,"This email is already in use" },
            {UIError.PasswordComplexity, "Your password must contain a lower & uppercase letter, a number, a symbol, and be at least 6 characters long" },
            {UIError.PasswordMismatch,"Your passwords do not match" }
        };

        private enum UIError
        {
            RequiredField,
            InvalidEmail,
            UsedEmail,
            PasswordComplexity,
            PasswordMismatch
        }

        public RegisterPage()
        {
            InitializeComponent();
        }

        private void FirstNameTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateInput((TextBox)sender, FirstNameErrorMessage, UIError.RequiredField);
        }

        private void LastNameTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateInput((TextBox)sender, LastNameErrorMessage, UIError.RequiredField);
        }

        private void EmailTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Check it is a valid email
            ValidateInput((TextBox)sender, EmailErrorMessage, UIError.InvalidEmail);
            // Check it is not in use
            ValidateInput((TextBox)sender, EmailErrorMessage, UIError.UsedEmail);
        }

        private void PasswordPBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ValidateInput((PasswordBox)sender, PasswordErrorMessage, UIError.PasswordComplexity);
        }

        private void PasswordConfirmPBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ValidateInput((PasswordBox)sender, PasswordPBox, PasswordConfirmErrorMessage, UIError.PasswordMismatch);
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void ValidateInput(object sender, TextBlock errorMsg, UIError uiError)
        {
            switch (uiError)
            {
                case UIError.RequiredField:
                    // If the text is valid
                    if (ValidateText((TextBox)sender)) ClearInputError(errorMsg);
                    // Text is not valid
                    else ShowInputError(uiError, errorMsg);
                    break;

                case UIError.InvalidEmail:
                    // Check if email is valid
                    if (ValidateEmail((TextBox)sender)) ClearInputError(errorMsg);
                    else ShowInputError(uiError, errorMsg);
                    break;

                case UIError.UsedEmail:
                    throw new NotImplementedException();
                //break;

                case UIError.PasswordComplexity:
                    if (ValidatePassword((PasswordBox)sender)) ClearInputError(errorMsg);
                    else ShowInputError(uiError, errorMsg);
                    break;

                default:
                    MessageBox.Show("The given error message is not handled here!", "Error RegisterPage.xaml.cs:ValidateInput(3)", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
            }
        }

        private void ValidateInput(object sender, object compare, TextBlock errorMsg, UIError uiError)
        {
            switch (uiError)
            {
                case UIError.PasswordMismatch:
                    if (ValidatePasswords((PasswordBox)sender, (PasswordBox)compare)) ClearInputError(errorMsg);
                    else ShowInputError(uiError, errorMsg);
                    break;
            }
        }

        private bool ValidateText(TextBox control)
        {
            return control.Text.Length > 0;
        }

        private bool ValidateEmail(TextBox control)
        {
            var emailChecker = new System.ComponentModel.DataAnnotations.EmailAddressAttribute();
            string email = control.Text;
            return emailChecker.IsValid(email);
        }

        private bool ValidatePassword(PasswordBox control)
        {
            return control.Password.Length > 0;
        }

        private bool ValidatePasswords(PasswordBox control1, PasswordBox control2)
        {
            return control1.Password == control2.Password;
        }

        private void ShowInputError(UIError message, TextBlock control)
        {
            control.Text = UIErrorMessages[message];
            control.Visibility = Visibility.Visible;
        }

        private void ClearInputError(TextBlock control)
        {
            control.Visibility = Visibility.Collapsed;
            control.Text = string.Empty;
        }

        private bool NotEmpty(TextBox value)
        {
            return value.Text.Length > 0;
        }

        private bool NotEmpty(PasswordBox value)
        {
            return value.Password.Length > 0;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePageTo(new LoginPage());
        }
    }
}