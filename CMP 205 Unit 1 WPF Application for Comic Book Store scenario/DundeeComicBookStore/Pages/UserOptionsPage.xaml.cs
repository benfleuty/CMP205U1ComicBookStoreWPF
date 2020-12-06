using DundeeComicBookStore.Interfaces;
using System;
using System.Collections.Generic;
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
    public partial class UserOptionsPage : BasePage
    {
        private decimal highestCost = 0.00m;
        private IUser _user;

        public IUser User
        { get { return _user; } set { _user = value; } }

        public UserOptionsPage(IUser loggedInUser)
        {
            InitializeComponent();
            User = loggedInUser;

            DisplayUserInfo();
            List<IProduct> catalog = GetCatalog();

            if (!CheckCatalog(catalog)) return;

            ShowCatalog(catalog);
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

        private bool CheckCatalog(List<IProduct> catalog)
        {
            // no products
            if (catalog.Count == 0)
            {
                TextBlock noProducts = new TextBlock()
                {
                    Text = "There are no products to display at this time!"
                };
                resultsViewer.Children.Add(noProducts);
                return false;
            }
            return true;
        }

        private void ShowCatalog(List<IProduct> catalog)
        {
            // products
            foreach (IProduct product in catalog)
            {
                string name = product.Name;
                int descLength = (product.Description.Length > 64) ? 64 : product.Description.Length;
                string desciption = $"{product.Description.Substring(0, descLength)}...";
                string price = product.UnitPrice.ToString("C");
                string text = $"{name}\n{desciption}\n{price}";
                var content = new TextBlock()
                {
                    Text = text,
                    TextWrapping = TextWrapping.Wrap
                };
                var button = new Button()
                {
                    Content = content
                };
                resultsViewer.Children.Add(button);
                // Set the highest cost product for price range
                if (highestCost < product.UnitPrice)
                    highestCost = product.UnitPrice;
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePageTo(new LoginPage());
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            searchTextbox.Text = string.Empty;
        }

        private void filterResultsButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void priceRangeCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            priceRangeContainer.IsEnabled = (bool)priceRangeCheckBox.IsChecked;
        }
    }
}