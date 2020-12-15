using DundeeComicBookStore.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows;

namespace DundeeComicBookStore.Pages
{
    /// <summary>
    /// Interaction logic for StockLevelPage.xaml
    /// </summary>
    public partial class StockLevelPage : BasePage
    {
        private StaffModel _staff;

        public StaffModel Staff
        {
            get { return _staff; }
            set { _staff = value; }
        }

        public StockLevelPage(StaffModel staff)
        {
            InitializeComponent();
            Staff = staff;
            GetData();
        }

        private void GetData()
        {
            using SqlConnection conn = new SqlConnection(ConnectionHelper.ConnVal("mssql1900040"));
            try
            {
                conn.Open();

                #region Get number of products in db

                var sql = new StringBuilder();
                sql.Append("SELECT COUNT(id) as quantity ");
                sql.Append("FROM Products ");

                string query = sql.ToString();

                SqlCommand command = new SqlCommand(query, conn);

                SqlDataReader reader = command.ExecuteReader();
                DataTable dataTable = new DataTable();
                dataTable.Load(reader);

                int totalProducts = (int)dataTable.Rows[0]["quantity"];

                #endregion Get number of products in db

                #region Get number of items with good stock (6 or more)

                sql = new StringBuilder();
                sql.Append("SELECT COUNT(id) as quantity ");
                sql.Append("FROM Products ");
                sql.Append("WHERE stockCount > 5");

                query = sql.ToString();

                command = new SqlCommand(query, conn);

                reader = command.ExecuteReader();
                dataTable = new DataTable();
                dataTable.Load(reader);

                int goodStockCount = (int)dataTable.Rows[0]["quantity"];

                #endregion Get number of items with good stock (6 or more)

                #region Get number of items with bad stock (5 or less, more than zero)

                sql = new StringBuilder();
                sql.Append("SELECT COUNT(id) as quantity ");
                sql.Append("FROM Products ");
                sql.Append("WHERE stockCount < 6 AND stockCount > 0");

                query = sql.ToString();

                command = new SqlCommand(query, conn);

                reader = command.ExecuteReader();
                dataTable = new DataTable();
                dataTable.Load(reader);

                int badStockCount = (int)dataTable.Rows[0]["quantity"];

                #endregion Get number of items with bad stock (5 or less, more than zero)

                #region Get number of out of stock items

                sql = new StringBuilder();
                sql.Append("SELECT COUNT(id) as quantity ");
                sql.Append("FROM Products ");
                sql.Append("WHERE stockCount < 1");

                query = sql.ToString();

                command = new SqlCommand(query, conn);

                reader = command.ExecuteReader();
                dataTable = new DataTable();
                dataTable.Load(reader);

                int noStockCount = (int)dataTable.Rows[0]["quantity"];

                #endregion Get number of out of stock items

                #region Get low and out of stock items

                sql = new StringBuilder();
                sql.Append("SELECT * ");
                sql.Append("FROM Products ");
                sql.Append("WHERE stockCount < 6 ");
                sql.Append("ORDER BY stockCount ASC ");

                query = sql.ToString();

                command = new SqlCommand(query, conn);

                reader = command.ExecuteReader();
                dataTable = new DataTable();
                dataTable.Load(reader);

                #endregion Get low and out of stock items

                #region Output

                numOfItemsInDBTb.Text = totalProducts.ToString();

                numOfItemsGoodStockTb.Text = goodStockCount.ToString();

                numOfItemsLowStock.Text = badStockCount.ToString();

                numOfItemsOutOfStock.Text = noStockCount.ToString();

                dgLowNoStockItems.ItemsSource = dataTable.AsDataView();

                #endregion Output
            }
            catch (Exception)
            {
                return;
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
    }
}