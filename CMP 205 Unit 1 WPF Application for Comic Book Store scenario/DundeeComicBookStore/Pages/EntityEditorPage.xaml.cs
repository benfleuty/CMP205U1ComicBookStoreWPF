﻿using DundeeComicBookStore.Models;
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

        #region Initial page setup

        public EntityEditorPage(StaffModel staff, EntityType entityType)
        {
            InitializeComponent();
            Staff = staff;
            Entity = entityType;
            SetupPage();
        }

        private void SetupPage()
        {
            if (Entity == EntityType.CustomerRecord)
                CustomerSetup();
            else if (Entity == EntityType.ProductRecord)
                ProductSetup();
            else if (Entity == EntityType.OrderRecord)
                OrderSetup();
            else
                StaffSetup();
        }

        private void CustomerSetup()
        {
            pageName.Text = "Entity Editor - Customer Records";
            bool canAccessEmployees = Staff.Can(StaffModel.Permission.AccessEmployeeData);
            dataSource = DBAccessHelper.GetUsers(canAccessEmployees);
            if (dataSource == null) dataSource = new DataTable()
            {
                Columns = {
                    {"error" },
                    {"no values found" }
                }
            };
            resultDg.ItemsSource = dataSource.AsDataView();
            customerSearchBar.Visibility = Visibility.Visible;
            formCustomerData.Visibility = Visibility.Visible;
            resultDg.Visibility = Visibility.Visible;
        }

        private void ProductSetup()
        {
            pageName.Text = "Entity Editor - Product Records";
            string query = $"select id,name,description,unitPrice,stockCount,unitCost from products";
            dataSource = DBAccessHelper.GetProductsReturnDataTable(query);
            if (dataSource == null) dataSource = new DataTable()
            {
                Columns = {
                    {"error" },
                    {"no values found" }
                }
            };
            resultDg.ItemsSource = dataSource.AsDataView();
            productSearchBar.Visibility = resultDg.Visibility = form.Visibility =
                formProductData.Visibility = Visibility.Visible;
        }

        private void OrderSetup()
        {
            pageName.Text = "Entity Editor - Order Records";
            string query = @"SELECT
OrderItems.orderId,
Orders.userId,
Orders.address,
FORMAT (Orders.orderDate, 'dd-MM-yyy hh:mm:ss') as date,
OrderItems.productId,
OrderItems.quantity,
Payments.amount,
Payments.type
FROM Orders
LEFT JOIN OrderItems ON OrderItems.orderId = Orders.id
LEFT JOIN Payments ON Orders.id = Payments.orderId
ORDER BY Orders.id DESC";
            dataSource = DBAccessHelper.GetOrders(query);
            if (dataSource == null) dataSource = new DataTable()
            {
                Columns = {
                    {"error" },
                    {"no values found" }
                }
            };
            resultDg.ItemsSource = dataSource.AsDataView();
            saveFormChanges.Visibility = Visibility.Collapsed;
            orderSearchBar.Visibility = resultDg.Visibility = form.Visibility =
                formOrderData.Visibility = Visibility.Visible;
        }

        private void StaffSetup()
        {
            pageName.Text = "Entity Editor - Staff Records";
            bool canAccessEmployees = Staff.Can(StaffModel.Permission.AccessEmployeeData);
            // need get staff function
            dataSource = DBAccessHelper.GetStaff(canAccessEmployees);
            if (dataSource == null) dataSource = new DataTable()
            {
                Columns = {
                    {"error" },
                    {"no values found" }
                }
            };
            resultDg.ItemsSource = dataSource.AsDataView();
            employeeSearchBar.Visibility = Visibility.Visible;
            formEmployeeData.Visibility = Visibility.Visible;
            resultDg.Visibility = Visibility.Visible;
        }

        #endregion Initial page setup

        #region Top bar events

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

        #endregion Top bar events

        #region DataGrid

        private void ParseTableToVars(DataRow row)
        {
            if (Entity == EntityType.CustomerRecord)
                ParseTableToCustomerForm(row);
            else if (Entity == EntityType.ProductRecord)
                ParseTableToProductForm(row);
            else if (Entity == EntityType.OrderRecord)
                ParseTableToOrderForm(row);
            else if (Entity == EntityType.StaffRecord)
                ParseTableToEmployeeForm(row);
        }

        private void ParseTableToCustomerForm(DataRow row)
        {
            formCustomerFirstNameTextbox.Text = (string)row["firstName"];
            formCustomerLastNameTextbox.Text = (string)row["lastName"];
            formCustomerPhoneNumberTextbox.Text = (string)row["phone"];
            formCustomerEmailAddressTextbox.Text = (string)row["email"];
            string content = (string)row["address"];
            string[] address = content.Split('|');
            formCustomerHouseNumberNameTextbox.Text = address[0];
            formCustomerPostCodeTextbox.Text = address[1];
        }

        private void ParseTableToProductForm(DataRow row)
        {
            formCustomerFirstNameTextbox.Text = (string)row["firstName"];
            formCustomerLastNameTextbox.Text = (string)row["lastName"];
            formCustomerPhoneNumberTextbox.Text = (string)row["phone"];
            formCustomerEmailAddressTextbox.Text = (string)row["email"];
            string content = (string)row["address"];
            string[] address = content.Split('|');
            formCustomerHouseNumberNameTextbox.Text = address[0];
            formCustomerPostCodeTextbox.Text = address[1];
        }

        private void ParseTableToOrderForm(DataRow row)
        {
            formCustomerFirstNameTextbox.Text = (string)row["firstName"];
            formCustomerLastNameTextbox.Text = (string)row["lastName"];
            formCustomerPhoneNumberTextbox.Text = (string)row["phone"];
            formCustomerEmailAddressTextbox.Text = (string)row["email"];
            string content = (string)row["address"];
            string[] address = content.Split('|');
            formCustomerHouseNumberNameTextbox.Text = address[0];
            formCustomerPostCodeTextbox.Text = address[1];
        }

        private void ParseTableToEmployeeForm(DataRow row)
        {
            formEmployeeFirstNameTextbox.Text = (string)row["firstName"];
            formEmployeeLastNameTextbox.Text = (string)row["lastName"];
            formEmployeePhoneNumberTextbox.Text = (string)row["phone"];
            formEmployeeEmailAddressTextbox.Text = (string)row["email"];
            string content = (string)row["address"];
            string[] address = content.Split('|');
            formEmployeeHouseNumberNameTextbox.Text = address[0];
            formEmployeePostCodeTextbox.Text = address[1];

            byte permissions = ((byte[])row["permissions"])[0];

            StaffModel selected = new StaffModel()
            {
                Permissions = DBAccessHelper.CalculatePermissions(permissions)
            };

            if (selected.Can(StaffModel.Permission.ReadCustomerData))
                employeeCB_RCD.IsChecked = true;

            if (selected.Can(StaffModel.Permission.WriteCustomerData))
                employeeCB_WCD.IsChecked = true;

            if (selected.Can(StaffModel.Permission.DeleteCustomerData))
                employeeCB_DCD.IsChecked = true;

            if (selected.Can(StaffModel.Permission.ReadStockData))
                employeeCB_RSD.IsChecked = true;

            if (selected.Can(StaffModel.Permission.WriteStockData))
                employeeCB_WSD.IsChecked = true;

            if (selected.Can(StaffModel.Permission.DeleteStockData))
                employeeCB_DSD.IsChecked = true;

            if (selected.Can(StaffModel.Permission.AccessEmployeeData))
                employeeCB_AED.IsChecked = true;
        }

        #region Events

        private void ResultDg_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var sent = sender as DataGrid;
            DataRowView drv = (DataRowView)sent.SelectedItem;
            DataRow row = drv.Row;
            ParseTableToVars(row);
        }

        #endregion Events

        #endregion DataGrid

        #region Form

        #region Form events

        private void EmployeeBtn_applyAll_Click(object sender, RoutedEventArgs e)
        {
            employeeCB_RCD.IsChecked = employeeCB_WCD.IsChecked = employeeCB_DCD.IsChecked =
                employeeCB_RSD.IsChecked = employeeCB_WSD.IsChecked = employeeCB_DSD.IsChecked =
                employeeCB_AED.IsChecked = true;
        }

        private void EmployeeBtn_removeAll_Click(object sender, RoutedEventArgs e)
        {
            employeeCB_RCD.IsChecked = employeeCB_WCD.IsChecked = employeeCB_DCD.IsChecked =
                employeeCB_RSD.IsChecked = employeeCB_WSD.IsChecked = employeeCB_DSD.IsChecked =
                employeeCB_AED.IsChecked = false;
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion Form events

        #endregion Form
    }
}