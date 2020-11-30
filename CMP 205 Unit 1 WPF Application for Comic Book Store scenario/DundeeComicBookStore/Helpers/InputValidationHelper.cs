using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace DundeeComicBookStore.Helpers
{
    public class InputValidationHelper
    {
        public static bool ValidateEmail(string email)
        {
            var emailChecker = new System.ComponentModel.DataAnnotations.EmailAddressAttribute();
            return emailChecker.IsValid(email);
        }

        public static bool ValidateNewPassword(string password)
        {
            return password.Length > 0;
        }

        public static bool ValidatePasswords(string password1, string password2)
        {
            return password1 == password2;
        }

        public static bool ValidateText(string text)
        {
            return text.Length > 0;
        }

        public static void ValidateInput(object sender, object compare, TextBlock errorMsg, ErrorHelper.UIError uiError)
        {
            switch (uiError)
            {
                case ErrorHelper.UIError.PasswordMismatch:
                    string password1 = ((PasswordBox)sender).Password;
                    string password2 = ((PasswordBox)compare).Password;
                    if (InputValidationHelper.ValidatePasswords(password1, password2))
                        ErrorHelper.ClearInputError(errorMsg);
                    else ErrorHelper.ShowInputError(uiError, errorMsg);
                    break;
            }
        }

        public static void ValidateInput(object sender, TextBlock errorMsgTextBlock, ErrorHelper.UIError uiErrorReason)
        {
            switch (uiErrorReason)
            {
                case ErrorHelper.UIError.RequiredField:
                    string text = ((TextBox)sender).Text;
                    // If the text is valid
                    if (InputValidationHelper.ValidateText(text))
                        ErrorHelper.ClearInputError(errorMsgTextBlock);
                    // Text is not valid
                    else ErrorHelper.ShowInputError(uiErrorReason, errorMsgTextBlock);
                    break;

                case ErrorHelper.UIError.InvalidEmail:
                    // Check if email is valid
                    string email = ((TextBox)sender).Text;
                    if (InputValidationHelper.ValidateEmail(email))
                        ErrorHelper.ClearInputError(errorMsgTextBlock);
                    else ErrorHelper.ShowInputError(uiErrorReason, errorMsgTextBlock);
                    break;

                case ErrorHelper.UIError.UsedEmail:
                    throw new NotImplementedException();
                //break;

                case ErrorHelper.UIError.PasswordComplexity:
                    string password = ((PasswordBox)sender).ToString();
                    if (InputValidationHelper.ValidateNewPassword(password))
                        ErrorHelper.ClearInputError(errorMsgTextBlock);
                    else ErrorHelper.ShowInputError(uiErrorReason, errorMsgTextBlock);
                    break;

                default:
                    MessageBox.Show("The given error message is not handled here!", "Error RegisterPage.xaml.cs:ValidateInput(3)", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
            }
        }
    }
}