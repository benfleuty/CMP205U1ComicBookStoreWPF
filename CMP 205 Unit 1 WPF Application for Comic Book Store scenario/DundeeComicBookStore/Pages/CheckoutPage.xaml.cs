using DundeeComicBookStore.Models;
using DundeeComicBookStore.Windows;
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
    /// Interaction logic for CheckoutPage.xaml
    /// </summary>
    public partial class CheckoutPage : BasePage
    {
        private OrderModel _order;

        public OrderModel Order
        {
            get { return _order; }
            set { _order = value; }
        }

        public CheckoutPage(OrderModel currentOrder)
        {
            InitializeComponent();
            Order = currentOrder;

            if (Order.User.ID == 0)
            {
                viewOrdersButton.IsEnabled = false;
                viewOrdersButton.Visibility = Visibility.Collapsed;
            }

            SetupPage();
        }

        private void SetupPage()
        {
            if (Order.Basket.Count == 0)
            {
                orderViewer.Visibility = Visibility.Collapsed;
                tbNoItems.Visibility = Visibility.Visible;
                return;
            }

            // 25% discount for 100 or more reward points
            tbSubtotal.Text = $"Subtotal: {Order.Subtotal:C}";
            if (Order.Discounted)
                tbSubtotal.Text += $"\nDiscount: 25% - Reward Points";

            rbHomeDelivery.IsChecked = true;
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePageTo(new LoginPage());
        }

        private void ViewOrdersButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePageTo(new ViewOrdersPage(Order));
        }

        private void BasketButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePageTo(new BasketPage(Order));
        }

        private void BrowseProductButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePageTo(new SearchProductsPage(Order));
        }

        private void DeliveryChoice_Changed(object sender, RoutedEventArgs e)
        {
            decimal basketTotal = Order.Basket.Total;
            if (rbHomeDelivery.IsChecked == true)
            {
                Order.HomeDelivery = true;
                tbTotal.Text = $"Total: {Order.Total:C}";
            }
            else
            {
                Order.HomeDelivery = false;
                tbTotal.Text = $"Total: {Order.Total:C}";
            }
        }

        private void ConfirmPaymentButton_Click(object sender, RoutedEventArgs e)
        {
            bool items = Order.Basket.Count != 0;
            bool deliverySet = (rbCollection.IsChecked ?? false) || (rbHomeDelivery.IsChecked ?? false);
            bool correctInput = items & deliverySet;
            if (!correctInput)
            {
                string output = string.Empty;
                if (!items)
                    output += "You need to add items to your basket to checkout!\n";
                if (!deliverySet)
                    output += "Please select a delivery option";
                MessageBox.Show(output);
                return;
            }
            // if the user is a customer, get confirmation of payment from a staff member
            if (!Order.User.IsStaff)
            {
                Window window = new StaffConfirmAction();
                bool result = window.ShowDialog() ?? false;
                // if the action is cancelled
                if (!result) return;
            }
            // enter order into the db
            Order.PaymentMethod = (selectedPaymentMethod.SelectedIndex == 0) ? OrderModel.PaymentType.Card : OrderModel.PaymentType.Cash;

            if (DBAccessHelper.ProcessOrder(Order)) MessageBox.Show("Payment saved!");
            else MessageBox.Show("Payment could not be saved!");
        }
    }
}