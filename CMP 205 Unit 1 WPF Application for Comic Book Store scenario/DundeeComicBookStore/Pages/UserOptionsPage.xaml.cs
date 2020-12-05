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
        private IUser _user;

        public IUser User
        { get { return _user; } set { _user = value; } }

        public UserOptionsPage(IUser loggedInUser)
        {
            InitializeComponent();
            User = loggedInUser;

            DisplayUserInfo();
            List<IProduct> catalog = GetCatalog();

            // no products
            if (catalog.Count == 0)
            {
                TextBlock noProducts = new TextBlock()
                {
                    Text = "There are no products to display at this time!"
                };
                resultsViewer.Children.Add(noProducts);
                return;
            }

            // products
            foreach (IProduct product in catalog)
            {
                string name = product.Name;
                int descLength = (product.Description.Length > 64) ? 64 : product.Description.Length;
                string desciption = product.Description.Substring(0, descLength);
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
            }
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

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePageTo(new LoginPage());
        }
    }
}