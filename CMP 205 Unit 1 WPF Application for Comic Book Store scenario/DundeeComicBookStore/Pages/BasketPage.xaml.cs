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
    /// Interaction logic for BasketPage.xaml
    /// </summary>
    public partial class BasketPage : BasePage
    {
        private BasketModel _basket;

        public BasketModel Basket
        {
            get { return _basket; }
            set { _basket = value; }
        }

        public BasketPage(BasketModel basket)
        {
            Basket = basket;
            InitializeComponent();
            OutputBasketItems();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePageTo(new LoginPage());
        }

        private void OutputBasketItems()
        {
            foreach (var item in Basket.Items)
            {
                IProduct product = item.Key;
                int quantity = item.Value;
                string name = product.Name;
                bool cutOffDescription = product.Description.Length > 64;
                int descCutOffPoint = (product.Description.Length >= 64) ? 61 : product.Description.Length;
                string desciption =
                    (cutOffDescription)
                    ? $"{product.Description.Substring(0, descCutOffPoint)}..."
                    : product.Description;
                string price = product.UnitPrice.ToString("C");
                string total = (product.UnitPrice * quantity).ToString();
                string text = $"{name}\n\n{desciption}\n\n{quantity}@{price}\n\nTotal: {total}";
                var content = new TextBlock()
                {
                    Text = text,
                    TextWrapping = TextWrapping.Wrap
                };
                var button = new Button()
                {
                    Content = content,
                    Tag = product.ID,
                };
                button.Click += BasketItem_Clicked;
                basketItemsViewer.Children.Add(button);
            }
        }

        private void BasketItem_Clicked(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BrowseProductButton_Click(object sender, RoutedEventArgs e)
        {
            browseProductButton.IsEnabled = false;
            ChangePageTo(new SearchProductsPage(new BasketModel(Basket.User)));
        }
    }
}