﻿using DundeeComicBookStore.Helpers;
using DundeeComicBookStore.Interfaces;
using DundeeComicBookStore.Models;
using System.Windows;
using System.Windows.Controls;

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

            // customer logging in
            if (!userLoggingIn.IsStaff)
            {
                // credentials are valid
                OrderModel order = new OrderModel()
                {
                    User = userLoggingIn
                };

                ChangePageTo(new SearchProductsPage(order));
                return;
            }

            // staff logging in
            var staffMember = (StaffModel)userLoggingIn;
            ChangePageTo(new StaffLandingPage(staffMember));
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePageTo(new RegisterPage());
        }

        private void GuestButton_Click(object sender, RoutedEventArgs e)
        {
            CustomerModel guest = (CustomerModel)DBAccessHelper.GetUser("guest@guest", "guest");
            OrderModel order = new OrderModel()
            {
                ID = 0,
                User = guest
            };

            ChangePageTo(new SearchProductsPage(order));
        }
    }
}