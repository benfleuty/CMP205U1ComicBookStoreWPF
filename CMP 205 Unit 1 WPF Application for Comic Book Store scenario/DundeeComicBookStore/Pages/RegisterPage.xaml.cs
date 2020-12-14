using DundeeComicBookStore.Helpers;
using DundeeComicBookStore.Interfaces;
using DundeeComicBookStore.Models;
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
            InputValidationHelper.ValidInput((TextBox)sender, FirstNameErrorMessage, ErrorHelper.UIError.RequiredField);
            TryEnableRegisterButton();
        }

        private void LastNameTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            InputValidationHelper.ValidInput((TextBox)sender, LastNameErrorMessage, ErrorHelper.UIError.RequiredField);
            TryEnableRegisterButton();
        }

        private void PhoneTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            InputValidationHelper.ValidInput((TextBox)sender, PhoneErrorMessage, ErrorHelper.UIError.InvalidPhoneNumber);
            TryEnableRegisterButton();
        }

        private void EmailTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Check it is a valid email
            if (InputValidationHelper.ValidInput((TextBox)sender, EmailErrorMessage, ErrorHelper.UIError.InvalidEmail))
                // Check it is not in use
                InputValidationHelper.ValidInput((TextBox)sender, EmailErrorMessage, ErrorHelper.UIError.EmailInUse);
            TryEnableRegisterButton();
        }

        private void PasswordPBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            InputValidationHelper.ValidInput((PasswordBox)sender, PasswordErrorMessage, ErrorHelper.UIError.PasswordComplexity);
            TryEnableRegisterButton();
        }

        private void PasswordConfirmPBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            InputValidationHelper.ValidInput((PasswordBox)sender, PasswordPBox, PasswordConfirmErrorMessage, ErrorHelper.UIError.PasswordMismatch);
            TryEnableRegisterButton();
        }

        private bool TryEnableRegisterButton()
        {
            if (FirstNameTextbox.Text.Trim().Length > 0 && FirstNameErrorMessage.Visibility == Visibility.Collapsed) { /* valid input */ }
            else { RegisterButton.IsEnabled = false; return false; }
            if (LastNameTextbox.Text.Trim().Length > 0 && LastNameErrorMessage.Visibility == Visibility.Collapsed) { /* valid input */ }
            else { RegisterButton.IsEnabled = false; return false; }
            if (HouseNumTextbox.Text.Trim().Length > 0 && PostcodeTextbox.Text.Trim().Length > 0 && AddressErrorMessage.Visibility == Visibility.Collapsed) { /* valid input */ }
            else { RegisterButton.IsEnabled = false; return false; }
            if (EmailTextbox.Text.Trim().Length > 0 && EmailErrorMessage.Visibility == Visibility.Collapsed) { /* valid input */ }
            else { RegisterButton.IsEnabled = false; return false; }
            if (PasswordPBox.Password.Trim().Length > 0 && PasswordErrorMessage.Visibility == Visibility.Collapsed) { /* valid input */ }
            else { RegisterButton.IsEnabled = false; return false; }
            if (PasswordConfirmPBox.Password.Trim().Length > 0 && PasswordConfirmErrorMessage.Visibility == Visibility.Collapsed) { /* valid input */ }
            else { RegisterButton.IsEnabled = false; return false; }

            RegisterButton.IsEnabled = true;
            return true;
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            TryEnableRegisterButton();
            string firstName = FirstNameTextbox.Text.Trim();
            string lastName = LastNameTextbox.Text.Trim();
            string address = $"{HouseNumTextbox.Text.Trim()}|{PostcodeTextbox.Text.Trim()}";
            string email = EmailTextbox.Text.Trim();
            string password = PasswordPBox.Password;
            string phone = PhoneTextbox.Text.Trim();

            IUser returned = DBAccessHelper.SetUser(firstName, lastName, email, password, phone, address);
            if (returned == null)
            {
                MessageBox.Show("error registering");
                return;
            }

            OrderModel order = new OrderModel()
            {
                User = returned
            };
            var searchpage = new SearchProductsPage(order);
            ChangePageTo(searchpage);
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePageTo(new LoginPage());
        }
    }
}