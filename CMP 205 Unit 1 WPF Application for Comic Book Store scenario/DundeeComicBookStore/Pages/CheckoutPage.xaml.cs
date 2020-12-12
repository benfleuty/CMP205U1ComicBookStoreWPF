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
            SetupPage();
        }

        private void SetupPage()
        {
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
            ChangePageTo(new BasketPage(Order));
        }
    }
}