﻿using DundeeComicBookStore.Interfaces;
using DundeeComicBookStore.Models;
using DundeeComicBookStore.Windows;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// Interaction logic for UserOptionsPage.xaml
    /// </summary>
    public partial class SearchProductsPage : BasePage
    {
        private decimal highestCost = 0.00m;
        private List<IProduct> currentSearchResults;
        private IUser _user;

        public IUser User
        { get { return _user; } set { _user = value; } }

        private BasketModel _basket;

        public BasketModel Basket
        {
            get { return _basket; }
            set { _basket = value; }
        }

        public SearchProductsPage(BasketModel basket)
        {
            InitializeComponent();
            Basket = basket;
            User = Basket.User;
            UpdateBasket();

            DisplayUserInfo();

            OutputSearchResults(GetCatalog());
        }

        private void DisplayUserInfo()
        {
            string message = $"Welcome, {User.FullName} ({User.EmailAddress})";
            usernameTextblock.Text = message;
        }

        private List<IProduct> GetCatalog()
        {
            List<IProduct> products = DBAccessHelper.GetAllProducts();

            return products;
        }

        private List<IProduct> GetCatalog(string search, bool getAll = false)
        {
            string select = "SELECT id,name,description,unitPrice,stockCount";
            string from = "FROM Products";
            string where = "WHERE";
            string and = "AND ";
            string or = "OR ";
            string orderby = "ORDER BY";

            if (titleCheckBox.IsChecked == true && getAll == false)
            {
                if (descriptionCheckBox.IsChecked == false)
                    where = $"{where} name LIKE @title";
                if (descriptionCheckBox.IsChecked == true)
                    where = $"{where} ( name LIKE @title";
            }

            if (descriptionCheckBox.IsChecked == true && getAll == false)
            {
                if (where != "WHERE")
                    where = $"{where} {or}";

                if (where != "WHERE")
                    where = $"{where} description LIKE @description)";
                else where = $"{where} description LIKE @description";
            }

            if (priceRangeCheckBox.IsChecked == true)
            {
                if (minimumPriceDUD.Value != null && minimumPriceDUD.Value > 0)
                {
                    if (where != "WHERE")
                        where = $"{where} {and}";

                    where = $"{where} unitPrice >= @minPrice";
                }

                if (maximumPriceDUD.Value != null)
                {
                    if (where != "WHERE")
                        where = $"{where} {and}";
                    where = $"{where} unitPrice <= @maxPrice";
                }
            }

            var selectedItem = (ComboBoxItem)sortByComboBox.SelectedItem;
            string tag = selectedItem.Tag.ToString();
            if (tag == "noSort")
                orderby = string.Empty;
            else
                orderby = $"{orderby} {tag}";

            if (where == "WHERE") where = string.Empty;

            string query = $"{select} {from} {where} {orderby}";

            var parameterKVPs = new Dictionary<string, object>();

            if (titleCheckBox.IsChecked == true)
                parameterKVPs.Add("@title", $"%{search}%");

            if (descriptionCheckBox.IsChecked == true)
                parameterKVPs.Add("@description", $"%{search}%");

            if (priceRangeCheckBox.IsChecked == true)
            {
                if (minimumPriceDUD.Value != null && minimumPriceDUD.Value > 0)
                    parameterKVPs.Add("@minPrice", (decimal)minimumPriceDUD.Value);

                if (maximumPriceDUD.Value != null)
                    parameterKVPs.Add("@maxPrice", (decimal)maximumPriceDUD.Value);
            }

            List<IProduct> products = DBAccessHelper.GetProducts(query, parameterKVPs);

            return products;
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePageTo(new LoginPage());
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            searchTextbox.Text = string.Empty;
        }

        private void PriceRangeCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            priceRangeContainer.IsEnabled = (bool)priceRangeCheckBox.IsChecked;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string search = searchTextbox.Text;
            if (search == string.Empty)
                OutputSearchResults(GetCatalog(search, true));
            else
                OutputSearchResults(GetCatalog(search));
        }

        private void OutputSearchResults(List<IProduct> results)
        {
            if (!CheckCatalog(results)) return;

            ShowCatalog(results);
        }

        private bool CheckCatalog(List<IProduct> catalog)
        {
            // no products
            if (catalog.Count == 0)
            {
                TextBlock noProducts = new TextBlock()
                {
                    Text = "There are no products to display at this time!"
                };
                resultsViewer.Children.Clear();
                resultsViewer.Children.Add(noProducts);
                return false;
            }
            return true;
        }

        private void ShowCatalog(List<IProduct> catalog)
        {
            resultsViewer.Children.Clear();

            // products
            currentSearchResults = catalog;
            foreach (IProduct product in catalog)
            {
                string name = product.Name;
                bool cutOffDescription = product.Description.Length > 64;
                int descCutOffPoint = (product.Description.Length >= 64) ? 61 : product.Description.Length;
                string desciption =
                    (cutOffDescription)
                    ? $"{product.Description.Substring(0, descCutOffPoint)}..."
                    : product.Description;
                string price = product.UnitPrice.ToString("C");
                bool anyInStock = product.UnitsInStock > 1;
                bool lowStock = product.UnitsInStock < 10;
                string inStock = string.Empty;
                if (anyInStock)
                {
                    if (lowStock) inStock += "LOW STOCK: ";
                    inStock += $"{product.UnitsInStock} available";
                }
                else inStock = "OUT OF STOCK!";
                string text = $"{name}\n\n{desciption}\n\n{price}\n\n{inStock}";
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
                button.Click += Product_Clicked;
                resultsViewer.Children.Add(button);
                // Set the highest cost product for price range
                if (highestCost < product.UnitPrice)
                    highestCost = product.UnitPrice;
            }
        }

        private void Product_Clicked(object sender, RoutedEventArgs e)
        {
            var sent = sender as Button;
            int id = (int)sent.Tag;
            IProduct p = currentSearchResults.First(item => item.ID == id);
            var viewer = new ProductViewerWindow(this, p);
            viewer.Show();
        }

        public void UpdateBasket()
        {
            int count = Basket.Count();
            if (count == 0)
                basketButton.Content = "Basket";
            else
                basketButton.Content = $"Basket ({count})";
        }

        private void BasketButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePageTo(new BasketPage(Basket));
        }
    }
}