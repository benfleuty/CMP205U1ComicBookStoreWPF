﻿using DundeeComicBookStore.Models;
using DundeeComicBookStore.Pages;
using System;
using System.Windows;
using System.Windows.Controls;

namespace DundeeComicBookStore.Windows
{
    /// <summary>
    /// Interaction logic for OrderViewerWindow.xaml
    /// </summary>
    public partial class OrderViewerWindow : Window
    {
        private ViewOrdersPage caller;
        private OrderModel _order;

        public OrderModel Order
        {
            get { return _order; }
            set { _order = value; }
        }

        public OrderViewerWindow(ViewOrdersPage caller, OrderModel order)
        {
            InitializeComponent();
            Order = order;
            this.caller = caller;
            SetupWindow();
        }

        private void SetupWindow()
        {
            orderNumberTitleTextbox.Text = $"Order #{Order.ID}";
            tbOrderDate.Text = $"Order created: {Order.PlacedAt}";
            if (Order.Complete) openOrderViewerButton.Content = "Buy again";

            foreach (var product in Order.Basket.Items)
            {
                var temp = new StackPanel()
                {
                    Orientation = Orientation.Vertical,
                    Margin = new Thickness(0, 0, 0, 15)
                };

                var productName = new TextBlock()
                {
                    Text = $"Product: {product.Key.Name} - {product.Key.ID}"
                };
                var productDescription = new TextBlock()
                {
                    Text = $"\tDescription: {product.Key.Description}",
                    FontSize = 22
                };
                var productQuantity = new TextBlock()
                {
                    Text = $"\tQuantity: {product.Value}",
                    FontSize = 22
                };
                var productUnitCost = new TextBlock()
                {
                    Text = $"\tCost each: {product.Key.UnitCost:C}",
                    FontSize = 22
                };
                var productTotalCost = new TextBlock()
                {
                    Text = $"\tTotal cost: {product.Key.UnitCost * product.Value:C}",
                    FontSize = 22
                };

                temp.Children.Add(productName);
                temp.Children.Add(productDescription);
                temp.Children.Add(productQuantity);
                temp.Children.Add(productUnitCost);
                temp.Children.Add(productTotalCost);
                productOutput.Children.Add(temp);
            }
        }

        private void CloseOrderViewerButton_Click(object sender, RoutedEventArgs e) => Close();

        private void OpenOrderViewerButton_Click(object sender, RoutedEventArgs e)
        {
            // if there is a basket that is currently in use
            if (caller.CurrentOrder.Basket.Items.Count > 0)
            {
                MessageBoxResult result =
                    MessageBox.Show("You currently have items in a basket? Would you like to save that order?", "Existing order!", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                    DBAccessHelper.SaveOrder(caller.CurrentOrder);
            }
            if (Order.Complete)
            {
                OrderModel newOrder = new OrderModel()
                {
                    ID = 0,
                    User = Order.User,
                    PlacedAt = DateTime.Now,
                    Basket = Order.Basket,
                    Address = Order.Address,
                    BeingEdited = false,
                    Complete = false
                };
                caller.CurrentOrder = newOrder;
            }
            else
            {
                Order.BeingEdited = true;
                caller.CurrentOrder = Order;
            }
            caller.OrderViewerClosingGoToBasket();

            Close();
        }

        private void DeleteOrderViewerButton_Click(object sender, RoutedEventArgs e)
        {
            if (DBAccessHelper.DeleteOrder(Order))
            {
                MessageBox.Show("Your order was deleted!"); ;
                caller.OrderViewerClosingUpdateOrderDisplay();
                Close();
            }
        }
    }
}