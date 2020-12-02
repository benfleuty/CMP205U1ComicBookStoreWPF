using System.Windows;
using System.Windows.Controls;

namespace DundeeComicBookStore.Helpers
{
    public class InputValidationHelper
    {
        private static bool ValidPhoneNumber(string phone)
        {
            int digitCount = 0;
            foreach (char c in phone)
            {
                if (char.IsDigit(c))
                    ++digitCount;
                else if (char.IsWhiteSpace(c)) continue;
                else return false;
            }

            if (digitCount != 11) return false;
            return true;
        }

        private static void SanitisePhoneNumber(ref string number)
        {
            string result = string.Empty;
            foreach (char c in number)
            {
                // if number
                if (char.IsDigit(c)) result += c;
                // if space after a number
                else if (char.IsWhiteSpace(c) && char.IsDigit(result[^1]))
                    result += c;
            }
            number = result;
        }

        private static bool ValidEmail(string email)
        {
            var emailChecker = new System.ComponentModel.DataAnnotations.EmailAddressAttribute();
            return emailChecker.IsValid(email);
        }

        private static bool ValidNewPassword(string password)
        {
            return password.Length > 0;
        }

        private static bool ValidPasswords(string password1, string password2)
        {
            return password1 == password2;
        }

        private static bool ValidText(string text)
        {
            return text.Length > 0;
        }

        public static bool ValidInput(object sender, TextBlock errorMsgTextBlock, ErrorHelper.UIError uiErrorReason)
        {
            switch (uiErrorReason)
            {
                case ErrorHelper.UIError.RequiredField:
                    string text = ((TextBox)sender).Text;
                    // If the text is valid
                    if (ValidText(text))
                    {
                        ErrorHelper.ClearInputError(errorMsgTextBlock);
                        return true;
                    }
                    // Text is not valid
                    break;

                case ErrorHelper.UIError.InvalidPhoneNumber:
                    string number = ((TextBox)sender).Text;
                    if (ValidPhoneNumber(number))
                    {
                        SanitisePhoneNumber(ref number);
                        ((TextBox)sender).Text = number;
                        ErrorHelper.ClearInputError(errorMsgTextBlock);
                        return true;
                    }
                    break;

                case ErrorHelper.UIError.InvalidEmail:
                    {
                        // Check if email is valid
                        string email = ((TextBox)sender).Text;
                        if (ValidEmail(email))
                        {
                            ErrorHelper.ClearInputError(errorMsgTextBlock);
                            return true;
                        }
                        break;
                    }

                case ErrorHelper.UIError.EmailInUse:
                    {
                        string email = ((TextBox)sender).Text;
                        if (DBAccessHelper.IsEmailNotInUse(email))
                        {
                            ErrorHelper.ClearInputError(errorMsgTextBlock);
                            return true;
                        }
                        break;
                    }

                case ErrorHelper.UIError.PasswordComplexity:
                    string password = ((PasswordBox)sender).Password.ToString();
                    if (ValidNewPassword(password))
                    {
                        ErrorHelper.ClearInputError(errorMsgTextBlock);
                        return true;
                    }
                    break;

                default:
                    MessageBox.Show("The given error message is not handled here!", "Error RegisterPage.xaml.cs:ValidateInput(3)", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
            }
            ErrorHelper.ShowInputError(uiErrorReason, errorMsgTextBlock);
            return false;
        }

        public static bool ValidInput(object sender, object compare, TextBlock errorMsg, ErrorHelper.UIError uiError)
        {
            switch (uiError)
            {
                case ErrorHelper.UIError.PasswordMismatch:
                    string password1 = ((PasswordBox)sender).Password;
                    string password2 = ((PasswordBox)compare).Password;
                    if (InputValidationHelper.ValidPasswords(password1, password2))
                    {
                        ErrorHelper.ClearInputError(errorMsg);
                        return true;
                    }
                    else ErrorHelper.ShowInputError(uiError, errorMsg);
                    break;
            }

            return false;
        }
    }
}