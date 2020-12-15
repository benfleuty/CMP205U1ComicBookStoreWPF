using DundeeComicBookStore.Helpers;
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

        private object selectedRow;

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
            dataSource = DBAccessHelper.GetUsers();
            if (dataSource == null) dataSource = new DataTable()
            {
                Columns = {
                    {"error" },
                    {"no values found" }
                }
            };
            resultDg.ItemsSource = dataSource.AsDataView();
            resultDg.Visibility = form.Visibility =
                formCustomerData.Visibility = deleteSelectedRecord.Visibility =
                addNewRecord.Visibility = saveFormChanges.Visibility = Visibility.Visible;
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
            resultDg.Visibility = form.Visibility =
                formProductData.Visibility = deleteSelectedRecord.Visibility =
                addNewRecord.Visibility = saveFormChanges.Visibility = Visibility.Visible;
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
            resultDg.Visibility = form.Visibility =
                formOrderData.Visibility = Visibility.Visible;
            deleteSelectedRecord.Visibility =
               addNewRecord.Visibility = saveFormChanges.Visibility = Visibility.Collapsed;
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
            resultDg.Visibility = form.Visibility = formEmployeeData.Visibility =
                deleteSelectedRecord.Visibility = saveFormChanges.Visibility = Visibility.Visible;

            addNewRecord.Visibility = Visibility.Collapsed;
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
            else if (Entity == EntityType.StaffRecord)
                ParseTableToEmployeeForm(row);
        }

        private void ParseTableToCustomerForm(DataRow row)
        {
            string firstName = (string)row["firstName"];
            string lastName = (string)row["lastName"];
            string phone = (string)row["phone"];
            string email = (string)row["email"];
            string address = (string)row["address"];
            string roadAddr = address.Split('|')[0];
            string postCode = address.Split('|')[1];
            formCustomerFirstNameTextbox.Text = firstName;
            formCustomerLastNameTextbox.Text = lastName;
            formCustomerPhoneNumberTextbox.Text = phone;
            formCustomerEmailAddressTextbox.Text = email;
            formCustomerHouseNumberNameTextbox.Text = roadAddr;
            formCustomerPostCodeTextbox.Text = postCode;

            selectedRow = new CustomerModel()
            {
                ID = (int)row["id"],
                FirstName = (string)row["firstName"],
                LastName = (string)row["lastName"],
                PhoneNumber = phone,
                EmailAddress = email,
                Address = address
            };
        }

        private void ParseTableToProductForm(DataRow row)
        {
            int id = (int)row["id"];
            string name = (string)row["name"];
            string desc = (string)row["description"];
            decimal unitPrice = (decimal)row["unitPrice"];
            int stockCount = (int)row["stockCount"];
            decimal unitCost = (decimal)row["unitCost"];

            formProductName.Text = name;
            formProductDescription.Text = desc;
            formProductPricePerUnit.Text = $"{unitPrice:C}";
            formProductStockCount.Text = $"{stockCount:C}";
            formProductUnitCost.Text = $"{unitCost:C}";

            selectedRow = new ProductModel()
            {
                ID = id,
                Name = name,
                Description = desc,
                UnitPrice = unitPrice,
                UnitsInStock = stockCount,
                UnitCost = unitCost
            };
        }

        private void ParseTableToEmployeeForm(DataRow row)
        {
            string firstName = (string)row["firstName"];
            string lastName = (string)row["lastName"];
            string phone = (string)row["phone"];
            string email = (string)row["email"];
            string address = (string)row["address"];
            string roadAddr = address.Split('|')[0];
            string postCode = address.Split('|')[1];
            formEmployeeFirstNameTextbox.Text = firstName;
            formEmployeeLastNameTextbox.Text = lastName;
            formEmployeePhoneNumberTextbox.Text = phone;
            formEmployeeEmailAddressTextbox.Text = email;
            formEmployeeHouseNumberNameTextbox.Text = roadAddr;
            formEmployeePostCodeTextbox.Text = postCode;

            byte permissions = ((byte[])row["permissions"])[0];

            StaffModel selected = new StaffModel()
            {
                ID = (int)row["id"],
                FirstName = (string)row["firstName"],
                LastName = (string)row["lastName"],
                PhoneNumber = phone,
                EmailAddress = email,
                Address = address,
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

            selectedRow = selected;
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

        #region Form Actions

        #region Form Check

        private bool CheckFields()
        {
            string error = string.Empty;
            if (Entity == EntityType.CustomerRecord)
                error = CheckCustomerRecord();
            else if (Entity == EntityType.ProductRecord)
                error = CheckProductRecord();
            else if (Entity == EntityType.StaffRecord)
                error = CheckStaffRecord();

            if (error == string.Empty)
                return true;

            MessageBox.Show(error, "Error In Form!", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        private string CheckCustomerRecord()
        {
            var errorMessage = new StringBuilder();

            // First name
            if (formCustomerFirstNameTextbox.Text.Trim().Length == 0)
                errorMessage.Append("You have not entered a first name!\n");

            // Last name
            if (formCustomerLastNameTextbox.Text.Trim().Length == 0)
                errorMessage.Append("You have not entered a last name!\n");

            // Phone number
            if (formCustomerPhoneNumberTextbox.Text.Trim().Length == 0)
                errorMessage.Append("You have not entered a phone number!\n");

            // Email address
            if (formCustomerEmailAddressTextbox.Text.Trim().Length == 0)
                errorMessage.Append("You have not entered an email!\n");
            else if (!InputValidationHelper.ValidInput(formCustomerEmailAddressTextbox, ErrorHelper.UIError.InvalidEmail))
                errorMessage.Append("You have not entered a valid email address!\n");

            // if the email entered is in use and isn't the email for the selected entity
            else if (formCustomerEmailAddressTextbox.Text != ((CustomerModel)selectedRow).EmailAddress
                && !InputValidationHelper.ValidInput(formCustomerEmailAddressTextbox, ErrorHelper.UIError.EmailInUse))
                errorMessage.Append("The email you have entered is already in use by another user!\n");

            // House Name / Number
            if (formCustomerHouseNumberNameTextbox.Text.Trim().Length == 0)
                errorMessage.Append("You have not entered a house name or number!\n");

            // PostCode
            if (formCustomerPostCodeTextbox.Text.Trim().Length == 0)
                errorMessage.Append("You have not entered a post code!");

            return errorMessage.ToString();
        }

        private string CheckProductRecord()
        {
            var errorMessage = new StringBuilder();

            // Product name
            if (formProductName.Text.Trim().Length == 0)
                errorMessage.Append("You have not entered a name for the product!\n");

            // Product description
            if (formProductDescription.Text.Trim().Length == 0)
                errorMessage.Append("You have not entered a description for the product!\n");

            // Price per unit
            if (formProductPricePerUnit.Text.Trim().Length == 0)
                errorMessage.Append("You have not entered the price per unit!\n");

            // Stock count
            if (formProductStockCount.Text.Trim().Length == 0)
                errorMessage.Append("You have not entered how many there are in stock!\n");

            // Unit cost
            if (formProductUnitCost.Text.Trim().Length == 0)
                errorMessage.Append("You have not entered the cost per unit!\n");

            return errorMessage.ToString();
        }

        private string CheckStaffRecord()
        {
            var errorMessage = new StringBuilder();

            // First name
            if (formEmployeeFirstNameTextbox.Text.Trim().Length == 0)
                errorMessage.Append("You have not entered a first name!\n");

            // Last name
            if (formEmployeeLastNameTextbox.Text.Trim().Length == 0)
                errorMessage.Append("You have not entered a last name!\n");

            // Phone number
            if (formEmployeePhoneNumberTextbox.Text.Trim().Length == 0)
                errorMessage.Append("You have not entered a phone number!\n");

            // Email address
            if (formEmployeeEmailAddressTextbox.Text.Trim().Length == 0)
                errorMessage.Append("You have not entered an email!\n");
            else if (!InputValidationHelper.ValidInput(formEmployeeEmailAddressTextbox, ErrorHelper.UIError.InvalidEmail))
                errorMessage.Append("You have not entered a valid email address!\n");

            // if the email entered is in use and isn't the email already set for the selected entity
            else if (formEmployeeEmailAddressTextbox.Text != ((StaffModel)selectedRow).EmailAddress
                && !InputValidationHelper.ValidInput(formEmployeeEmailAddressTextbox, ErrorHelper.UIError.EmailInUse))
                errorMessage.Append("The email you have entered is already in use by another user!\n");

            // House Name / Number
            if (formEmployeeHouseNumberNameTextbox.Text.Trim().Length == 0)
                errorMessage.Append("You have not entered a house name or number!\n");

            // PostCode
            if (formEmployeePostCodeTextbox.Text.Trim().Length == 0)
                errorMessage.Append("You have not entered a post code!");

            return errorMessage.ToString();
        }

        #endregion Form Check

        #region Form Delete

        private void DeleteCustomerRecord()
        {
            if (!CheckFields()) return;

            var customer = ((CustomerModel)selectedRow);

            bool result = DBAccessHelper.DeleteUser(customer.ID);
            // delete failed
            if (!result)
            {
                MessageBox.Show("Record could not be deleted!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // delete was a success
            ClearForm();
        }

        private void DeleteProductRecord()
        {
            if (!CheckFields()) return;

            var product = ((ProductModel)selectedRow);

            bool result = DBAccessHelper.DeleteProduct(product.ID);
            // delete failed
            if (!result)
            {
                MessageBox.Show("Record could not be deleted!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // delete was a success
            ClearForm();
        }

        private void DeleteStaffRecord()
        {
            if (!CheckFields()) return;
            if (!Staff.Can(StaffModel.Permission.AccessEmployeeData))
            {
                MessageBox.Show("You do not have the correct permissions to perform this action!");
                return;
            }
            var employee = ((StaffModel)selectedRow);

            bool result = DBAccessHelper.DeleteUser(employee.ID);
            // delete failed
            if (!result)
            {
                MessageBox.Show("Record could not be deleted!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // delete was a success
            ClearForm();
        }

        #endregion Form Delete

        #region Form Add

        private void AddProductRecord()
        {
            // can only add a new product record
            if (Entity != EntityType.ProductRecord) return;
            if (!CheckFields()) return;

            var newProduct = new ProductModel()
            {
                Name = formProductName.Text,
                Description = formProductDescription.Text,
                UnitPrice = (decimal)formProductPricePerUnit.Value,
                UnitsInStock = (int)formProductStockCount.Value,
                UnitCost = (decimal)formProductUnitCost.Value
            };

            if (DBAccessHelper.AddProduct(newProduct)) MessageBox.Show("Your product was added!", "Product added", MessageBoxButton.OK, MessageBoxImage.Information);
            else MessageBox.Show("Your product was not added!", "Error: Product not added", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        #endregion Form Add

        #region Form Save

        private bool SaveCustomerChanges()
        {
            if (!CheckFields()) return false;

            var changedModel = new CustomerModel()
            {
                ID = ((CustomerModel)selectedRow).ID,
                FirstName = formCustomerFirstNameTextbox.Text.Trim(),
                LastName = formCustomerLastNameTextbox.Text.Trim(),
                PhoneNumber = formCustomerPhoneNumberTextbox.Text.Trim(),
                EmailAddress = formCustomerEmailAddressTextbox.Text.Trim(),
                Address = $"{formCustomerHouseNumberNameTextbox.Text.Trim()}|{formCustomerPostCodeTextbox.Text.Trim()}"
            };

            bool changeEmail = customerPasswordTb.Visibility == Visibility.Visible;

            if (changeEmail)
            {
                if (formCustomerPasswordbox.Password == string.Empty)
                    MessageBox.Show("You need to input the customer's password to change their email.", "No password entered!");
                else return DBAccessHelper.AlterUser(changedModel, ((CustomerModel)selectedRow).EmailAddress, formCustomerPasswordbox.Password);
            }

            return DBAccessHelper.AlterUser(changedModel);
        }

        private bool SaveProductChanges()
        {
            throw new NotImplementedException();
        }

        private bool SaveStaffChanges()
        {
            if (!CheckFields()) return false;

            #region Calculate new permissions

            byte newPerms = 0;
            if (employeeCB_RCD.IsChecked ?? false)
                newPerms += 1;
            if (employeeCB_WCD.IsChecked ?? false)
                newPerms += 2;
            if (employeeCB_DCD.IsChecked ?? false)
                newPerms += 4;
            if (employeeCB_RSD.IsChecked ?? false)
                newPerms += 8;
            if (employeeCB_WSD.IsChecked ?? false)
                newPerms += 16;
            if (employeeCB_DSD.IsChecked ?? false)
                newPerms += 32;
            if (employeeCB_AED.IsChecked ?? false)
                newPerms += 64;

            #endregion Calculate new permissions

            var changedModel = new StaffModel()
            {
                ID = ((StaffModel)selectedRow).ID,
                FirstName = formEmployeeFirstNameTextbox.Text.Trim(),
                LastName = formEmployeeLastNameTextbox.Text.Trim(),
                PhoneNumber = formEmployeePhoneNumberTextbox.Text.Trim(),
                EmailAddress = formEmployeeEmailAddressTextbox.Text.Trim(),
                Address = $"{formEmployeeHouseNumberNameTextbox.Text.Trim()}|{formEmployeePostCodeTextbox.Text.Trim()}"
            };

            bool changeEmail = employeePasswordTb.Visibility == Visibility.Visible;

            if (changeEmail)
            {
                if (formEmployeePasswordbox.Password == string.Empty)
                    MessageBox.Show("You need to input the staff member's password to change their email.", "No password entered!");
                else return DBAccessHelper.AlterStaff(changedModel, ((StaffModel)selectedRow).EmailAddress, formEmployeePasswordbox.Password, newPerms);
            }
            return DBAccessHelper.AlterStaff(changedModel, newPerms);
        }

        #endregion Form Save

        private void ClearForm()
        {
            // Customer
            formCustomerFirstNameTextbox.Text = string.Empty;
            formCustomerLastNameTextbox.Text = string.Empty;
            formCustomerPhoneNumberTextbox.Text = string.Empty;
            formCustomerEmailAddressTextbox.Text = string.Empty;
            formCustomerHouseNumberNameTextbox.Text = string.Empty;
            formCustomerPostCodeTextbox.Text = string.Empty;

            // Product
            formProductName.Text = string.Empty;
            formProductDescription.Text = string.Empty;
            formProductPricePerUnit.Text = string.Empty;
            formProductStockCount.Text = string.Empty;
            formProductUnitCost.Text = string.Empty;

            // Employee
            formEmployeeFirstNameTextbox.Text = string.Empty;
            formEmployeeLastNameTextbox.Text = string.Empty;
            formEmployeePhoneNumberTextbox.Text = string.Empty;
            formEmployeeEmailAddressTextbox.Text = string.Empty;
            formEmployeeHouseNumberNameTextbox.Text = string.Empty;
            formEmployeePostCodeTextbox.Text = string.Empty;

            employeeCB_RCD.IsChecked = employeeCB_WCD.IsChecked =
            employeeCB_DCD.IsChecked = employeeCB_RSD.IsChecked =
            employeeCB_WSD.IsChecked = employeeCB_DSD.IsChecked =
            employeeCB_AED.IsChecked = false;

            selectedRow = new object();
        }

        #endregion Form Actions

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

        private void DeleteSelectedRecord_Click(object sender, RoutedEventArgs e)
        {
            var result = System.Windows.MessageBox.Show("Are you sure you want to delete this record?\nYou cannot undo this aciton!", "Delete Record", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
            // dont delete
            if (result == MessageBoxResult.No) return;
            // delete confirmed

            if (Entity == EntityType.CustomerRecord)
                DeleteCustomerRecord();
            else if (Entity == EntityType.ProductRecord)
                DeleteProductRecord();
            else if (Entity == EntityType.StaffRecord)
                DeleteStaffRecord();
        }

        private void AddNewRecord_Click(object sender, RoutedEventArgs e) => AddProductRecord();

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            bool result = false;
            if (Entity == EntityType.CustomerRecord)
                result = SaveCustomerChanges();
            else if (Entity == EntityType.ProductRecord)
                result = SaveProductChanges();
            else if (Entity == EntityType.StaffRecord)
                result = SaveStaffChanges();

            if (result)
            {
                MessageBox.Show("Changes saved successfully!", "Changes Saved", MessageBoxButton.OK, MessageBoxImage.Information);
                ClearForm();
                return;
            }

            MessageBox.Show("Your changes could not be saved!", "Error: Changes not saved!", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void formCustomerEmailAddressTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (selectedRow == null) return;
            if (formCustomerEmailAddressTextbox.Text.Trim() != ((CustomerModel)selectedRow).EmailAddress)
            {
                formCustomerPasswordbox.Password = string.Empty;
                formCustomerPasswordbox.Visibility = Visibility.Visible;
                customerPasswordTb.Visibility = Visibility.Visible;
                return;
            }

            formCustomerPasswordbox.Password = string.Empty;
            formCustomerPasswordbox.Visibility = Visibility.Collapsed;
            customerPasswordTb.Visibility = Visibility.Collapsed;
        }

        private void formEmployeeEmailAddressTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (selectedRow == null) return;
            if (formEmployeeEmailAddressTextbox.Text.Trim() != ((StaffModel)selectedRow).EmailAddress)
            {
                formEmployeePasswordbox.Password = string.Empty;
                formEmployeePasswordbox.Visibility = Visibility.Visible;
                employeePasswordTb.Visibility = Visibility.Visible;
                return;
            }

            formEmployeePasswordbox.Password = string.Empty;
            formEmployeePasswordbox.Visibility = Visibility.Collapsed;
            employeePasswordTb.Visibility = Visibility.Collapsed;
        }

        #endregion Form events

        #endregion Form
    }
}