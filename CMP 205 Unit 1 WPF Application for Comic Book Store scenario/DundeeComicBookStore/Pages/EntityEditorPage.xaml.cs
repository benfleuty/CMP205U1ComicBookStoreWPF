using DundeeComicBookStore.Models;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for EntityEditorPage.xaml
    /// </summary>
    public partial class EntityEditorPage : BasePage
    {
        private DataTable dataSource;

        private StaffModel _staff;

        public StaffModel Staff
        {
            get { return _staff; }
            set { _staff = value; }
        }

        private EntityType Entity;

        public enum EntityType
        {
            CustomerRecord,
            StaffRecord,
            ProductRecord,
            OrderRecord
        }

        #region No entity type given

        public EntityEditorPage(StaffModel staff)
        {
            InitializeComponent();
            Staff = staff;
            GetEntityType();
        }

        private void GetEntityType()
        {
            if (Staff.Can(StaffModel.Permission.ReadCustomerData))
            {
                customerEntityButton.IsEnabled = true;
                customerEntityButton.Visibility = Visibility.Visible;
                orderEntityButton.IsEnabled = true;
                orderEntityButton.Visibility = Visibility.Visible;
            }
            if (Staff.Can(StaffModel.Permission.ReadStockData))
            {
                productEntityButton.IsEnabled = true;
                productEntityButton.Visibility = Visibility.Visible;
            }
            if (Staff.Can(StaffModel.Permission.AccessEmployeeData))
            {
                staffEntityButton.IsEnabled = true;
                staffEntityButton.Visibility = Visibility.Visible;
            }
        }

        private void CustomerEntityButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePageTo(new EntityEditorPage(Staff, EntityType.CustomerRecord));
        }

        private void OrderEntityButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePageTo(new EntityEditorPage(Staff, EntityType.OrderRecord));
        }

        private void StaffEntityButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePageTo(new EntityEditorPage(Staff, EntityType.StaffRecord));
        }

        private void ProductEntityButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePageTo(new EntityEditorPage(Staff, EntityType.ProductRecord));
        }

        #endregion No entity type given

        public EntityEditorPage(StaffModel staff, EntityType entityType)
        {
            InitializeComponent();
            Staff = staff;
            Entity = entityType;
            GetData();
        }

        private void GetData()
        {
            if (Entity == EntityType.CustomerRecord)
            {
                pageName.Text = "Entity Editor - Customer Records";
                bool canAccessEmployees = Staff.Can(StaffModel.Permission.AccessEmployeeData);
                dataSource = DBAccessHelper.GetUsers(canAccessEmployees);
                customerSearchBar.Visibility = Visibility.Visible;
                resultDg.Visibility = Visibility.Visible;
                resultDg.ItemsSource = dataSource.AsDataView();
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePageTo(new LoginPage());
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePageTo(new StaffLandingPage(Staff));
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ProductPriceRangeCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OrderPriceRangeCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void CustomerPriceRangeCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void EmployeePriceRangeCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}