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
    /// Interaction logic for ViewOrdersPage.xaml
    /// </summary>
    public partial class ViewOrdersPage : BasePage
    {
        private BasketModel _basket;

        public BasketModel Basket
        {
            get { return _basket; }
            set { _basket = value; }
        }

        public ViewOrdersPage(BasketModel basket)
        {
            InitializeComponent();
            Basket = basket;
            ShowOrders();
        }

        private void ShowOrders()
        {
            List<OrderModel> orders = DBAccessHelper.GetOrders(Basket.User.ID);
            // iterate orders
            foreach (var order in orders)
            {
                //order
                string output = "";
                output += $"Order ID: {order.ID}\n";
                output += $"Order Address: {order.Address}\n";
                output += $"Order PlacedAt: {order.PlacedAt}\n";
                string yesNo = (order.Complete) ? "Yes" : "No";
                output += $"Complete: {yesNo} \n";
                if (order.Complete)
                    output += $"{order.Basket.Count()} items ordered";
                else
                    output += $"{order.Basket.Count()} items in this basket";

                TextBlock tb = new TextBlock()
                {
                    Text = output,
                    TextWrapping = TextWrapping.Wrap
                };

                var orderButton = new Button()
                {
                    Content = tb
                };

                ordersViewer.Children.Add(orderButton);
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePageTo(new LoginPage());
        }

        private void BrowseProductButton_Click(object sender, RoutedEventArgs e)
        {
            browseProductButton.IsEnabled = false;
            ChangePageTo(new SearchProductsPage(Basket));
        }
    }
}