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
        public CheckoutPage(OrderModel currentOrder)
        {
            InitializeComponent();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void ViewOrdersButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void SaveOrderButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void BrowseProductButton_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}