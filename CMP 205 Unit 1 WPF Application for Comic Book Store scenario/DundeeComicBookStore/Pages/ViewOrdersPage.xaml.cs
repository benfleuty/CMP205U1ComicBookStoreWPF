using DundeeComicBookStore.Interfaces;
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
    /// Interaction logic for ViewOrdersPage.xaml
    /// </summary>
    public partial class ViewOrdersPage : BasePage
    {
        private List<OrderModel> _orders;

        public List<OrderModel> Orders
        {
            get { return _orders; }
            set { _orders = value; }
        }

        private OrderModel _currentOrder;

        public OrderModel CurrentOrder
        {
            get { return _currentOrder; }
            set { _currentOrder = value; }
        }

        public ViewOrdersPage(OrderModel order)
        {
            InitializeComponent();
            CurrentOrder = order;
            ShowOrders();
        }

        private void ShowOrders()
        {
            ordersViewer.Children.Clear();
            Orders = DBAccessHelper.GetOrders(CurrentOrder.User.ID);
            if (Orders == null)
            {
                TextBlock tb = new TextBlock()
                {
                    Text = "There are no orders to display!"
                };

                ordersViewer.Children.Add(tb);
                return;
            }
            // iterate orders
            foreach (var order in Orders)
            {
                //order
                string output = "";
                output += $"Order ID: {order.ID}\n";
                output += $"Order Address: {order.Address}\n";
                output += $"Order Date:\n{order.PlacedAt}\n";
                string yesNo = (order.Complete) ? "Yes" : "No";
                output += $"Complete: {yesNo} \n";
                if (order.Complete)
                    output += $"{order.Basket.Count} items ordered";
                else
                    output += $"{order.Basket.Count} items in this basket";

                TextBlock tb = new TextBlock()
                {
                    Text = output,
                    TextWrapping = TextWrapping.Wrap
                };

                var orderButton = new Button()
                {
                    Content = tb,
                    Tag = order.ID
                };

                orderButton.Click += OrderButton_Click;

                ordersViewer.Children.Add(orderButton);
            }
        }

        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            var sent = sender as Button;
            OrderModel result = Orders.Find(order => order.ID == (int)sent.Tag);
            (new OrderViewerWindow(this, result)).Show();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePageTo(new LoginPage());
        }

        private void BrowseProductButton_Click(object sender, RoutedEventArgs e)
        {
            browseProductButton.IsEnabled = false;
            ChangePageTo(new SearchProductsPage(CurrentOrder));
        }

        internal void OrderViewerClosingGoToBasket() => ChangePageTo(new BasketPage(CurrentOrder));

        internal void OrderViewerClosingUpdateOrderDisplay() => ShowOrders();

        private void CheckoutButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePageTo(new CheckoutPage(CurrentOrder));
        }

        private void BasketButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePageTo(new BasketPage(CurrentOrder));
        }

        private void SaveOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentOrder.Basket.Count == 0)
            {
                MessageBox.Show("You can't save an empty order!");
                return;
            }
            if (!DBAccessHelper.SaveOrder(CurrentOrder)) MessageBox.Show("Your order could not be saved!");
            else MessageBox.Show("Your order was saved!");
        }
    }
}