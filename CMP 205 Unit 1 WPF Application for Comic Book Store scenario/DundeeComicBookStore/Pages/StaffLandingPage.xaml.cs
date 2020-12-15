using DundeeComicBookStore.Models;
using System.Windows;
using System.Windows.Controls;

namespace DundeeComicBookStore.Pages
{
    /// <summary>
    /// Interaction logic for StaffLandingPage.xaml
    /// </summary>
    public partial class StaffLandingPage : BasePage
    {
        private StaffModel _staff;

        public StaffModel Staff
        {
            get { return _staff; }
            set { _staff = value; }
        }

        public StaffLandingPage(StaffModel staff)
        {
            InitializeComponent();
            Staff = staff;
            usernameTextblock.Text = $"Welcome, {Staff.FullName} ({Staff.EmailAddress})";

            SetupActionButtons();
        }

        private void SetupActionButtons()
        {
            var button = new Button()
            {
                Content = "Continue as a customer"
            };
            button.Click += ContinueAsCustomer;
            staffActionButtonsPanel.Children.Add(button);

            button = new Button()
            {
                Content = "Entity editor"
            };
            button.Click += OpenEntityEditor;
            staffActionButtonsPanel.Children.Add(button);

            button = new Button()
            {
                Content = "Stock Levels"
            };
            button.Click += OpenStockLevelViewer;
            staffActionButtonsPanel.Children.Add(button);

            button = new Button()
            {
                Content = "Sales Viewer"
            };
            button.Click += SalesViewer;
            staffActionButtonsPanel.Children.Add(button);
        }

        private void ContinueAsCustomer(object sender, RoutedEventArgs e)
        {
            OrderModel order = new OrderModel()
            {
                User = Staff
            };

            ChangePageTo(new SearchProductsPage(order));
        }

        private void OpenEntityEditor(object sender, RoutedEventArgs e)
        {
            ChangePageTo(new EntityEditorPage(Staff));
        }

        private void OpenStockLevelViewer(object sender, RoutedEventArgs e)
        {
            ChangePageTo(new StockLevelPage(Staff));
        }

        private void SalesViewer(object sender, RoutedEventArgs e)
        {
            ChangePageTo(new SalesViewerPage(Staff));
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePageTo(new LoginPage());
        }
    }
}