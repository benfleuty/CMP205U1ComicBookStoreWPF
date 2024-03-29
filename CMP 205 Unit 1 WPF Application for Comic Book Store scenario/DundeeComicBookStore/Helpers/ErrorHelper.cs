﻿using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace DundeeComicBookStore.Helpers
{
    public class ErrorHelper
    {
        public static readonly Dictionary<UIError, string> UIErrorMessages = new Dictionary<UIError, string>()
        {
            { UIError.RequiredField, "This field is required" },
            {UIError.InvalidPhoneNumber, "Your phone number must be 11 digits" },
            {UIError.InvalidEmail, "This is not a valid email address" },
            {UIError.EmailInUse,"This email is already in use" },
            {UIError.PasswordComplexity, "Your password must contain a lower & uppercase letter, a number, a symbol, and be at least 6 characters long" },
            {UIError.PasswordMismatch,"Your passwords do not match" },
            {UIError.EmailPasswordComboNotRecognised,"This email/password combo is not recognised"}
        };

        public enum UIError
        {
            RequiredField,
            InvalidPhoneNumber,
            InvalidEmail,
            EmailInUse,
            PasswordComplexity,
            PasswordMismatch,
            EmailPasswordComboNotRecognised
        }

        public static void ShowInputError(UIError message, TextBlock control)
        {
            control.Text = UIErrorMessages[message];
            control.Visibility = Visibility.Visible;
        }

        public static void ClearInputError(TextBlock control)
        {
            control.Visibility = Visibility.Collapsed;
            control.Text = string.Empty;
        }
    }
}