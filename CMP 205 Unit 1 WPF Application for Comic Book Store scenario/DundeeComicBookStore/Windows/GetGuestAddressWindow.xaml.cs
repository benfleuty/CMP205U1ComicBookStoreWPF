using DundeeComicBookStore.Pages;
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
using System.Windows.Shapes;

namespace DundeeComicBookStore.Windows
{
    /// <summary>
    /// Interaction logic for GetGuestAddressWindow.xaml
    /// </summary>
    public partial class GetGuestAddressWindow : Window
    {
        private object caller;

        public GetGuestAddressWindow(object caller)
        {
            this.caller = caller;
            InitializeComponent();
        }

        private void ConfirmAddress_Click(object sender, RoutedEventArgs e)
        {
            guestHouseInfo.Text = guestHouseInfo.Text.Trim();
            guestPostCode.Text = guestPostCode.Text.Trim();
            if (guestHouseInfo.Text.Length == 0 || guestPostCode.Text.Length == 0)
            {
                var errorMessage = new StringBuilder();
                if (guestHouseInfo.Text.Length == 0)
                    errorMessage.Append("No house number/name provided!\n");
                if (guestPostCode.Text.Length == 0)
                    errorMessage.Append("No post code provided!\n");

                MessageBox.Show(errorMessage.ToString(), "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string address = $"{guestHouseInfo.Text}|{guestPostCode.Text}";
            ((CheckoutPage)caller).SetGuestAddress(address);
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}