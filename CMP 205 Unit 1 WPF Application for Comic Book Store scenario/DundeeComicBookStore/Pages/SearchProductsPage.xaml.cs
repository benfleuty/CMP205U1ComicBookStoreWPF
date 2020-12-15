using DundeeComicBookStore.Interfaces;
using DundeeComicBookStore.Models;
using DundeeComicBookStore.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DundeeComicBookStore.Pages
{
    /// <summary>
    /// Interaction logic for UserOptionsPage.xaml
    /// </summary>
    public partial class SearchProductsPage : BasePage
    {
        private decimal highestCost = 0.00m;
        private List<IProduct> currentSearchResults;
        private OrderModel _currentOrder;

        public OrderModel CurrentOrder
        {
            get { return _currentOrder; }
            set { _currentOrder = value; }
        }

        public SearchProductsPage(OrderModel order)
        {
            InitializeComponent();
            CurrentOrder = order;
            UpdateBasket();

            // if guest
            if (order.User.ID == 0)
                DisplayGuestInfo();
            else
                DisplayUserInfo();

            OutputSearchResults(GetCatalog());
        }

        private void DisplayUserInfo()
        {
            string message = $"Welcome, {CurrentOrder.User.FullName} ({CurrentOrder.User.EmailAddress})";
            usernameTextblock.Text = message;
        }

        private void DisplayGuestInfo()
        {
            usernameTextblock.Text = $"Welcome, Guest";

            saveOrderButton.IsEnabled = false;
            saveOrderButton.Visibility = Visibility.Collapsed;
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
            int count = CurrentOrder.Basket.Count;
            if (count == 0)
                basketButton.Content = "Basket";
            else
                basketButton.Content = $"Basket ({count})";
        }

        private void BasketButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePageTo(new BasketPage(CurrentOrder));
        }

        private void CheckoutButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePageTo(new CheckoutPage(CurrentOrder));
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