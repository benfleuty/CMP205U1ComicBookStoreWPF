using DundeeComicBookStore.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
    /// Interaction logic for SalesViewerPage.xaml
    /// </summary>
    public partial class SalesViewerPage : BasePage
    {
        private StaffModel _staff;

        public StaffModel Staff
        {
            get { return _staff; }
            set { _staff = value; }
        }

        public SalesViewerPage(StaffModel staff)
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

                #region Get number of items ordered

                var sql = new StringBuilder();
                sql.Append("SELECT SUM(quantity) as quantity ");
                sql.Append("FROM OrderItems ");

                string query = sql.ToString();

                SqlCommand command = new SqlCommand(query, conn);

                SqlDataReader reader = command.ExecuteReader();
                DataTable dataTable = new DataTable();
                dataTable.Load(reader);

                int totalItems = (int)dataTable.Rows[0]["quantity"];

                #endregion Get number of items ordered

                #region Get number of orders placed

                sql = new StringBuilder();
                sql.Append("SELECT COUNT(id) as id ");
                sql.Append("FROM Orders ");

                query = sql.ToString();

                command = new SqlCommand(query, conn);

                reader = command.ExecuteReader();
                dataTable = new DataTable();
                dataTable.Load(reader);

                int totalOrders = (int)dataTable.Rows[0]["id"];

                #endregion Get number of orders placed

                #region Get gross revenue

                sql = new StringBuilder();
                sql.Append("SELECT SUM(amount) as amount ");
                sql.Append("FROM Payments ");

                query = sql.ToString();

                command = new SqlCommand(query, conn);

                reader = command.ExecuteReader();
                dataTable = new DataTable();
                dataTable.Load(reader);

                decimal grossRevenue = (decimal)dataTable.Rows[0]["amount"];

                #endregion Get gross revenue

                #region Get expenses

                sql = new StringBuilder();
                sql.Append("SELECT productId,quantity ");
                sql.Append("FROM OrderItems,Payments ");
                sql.Append("WHERE OrderItems.orderId = Payments.orderId ");

                query = sql.ToString();

                command = new SqlCommand(query, conn);

                reader = command.ExecuteReader();
                dataTable = new DataTable();
                dataTable.Load(reader);

                var productsSold = new Dictionary<int, int>();

                foreach (DataRow row in dataTable.Rows)
                {
                    int productId = (int)row["productId"];
                    int quantity = (int)row["quantity"];
                    if (productsSold.ContainsKey(productId))
                        productsSold[productId] += quantity;
                    else productsSold[productId] = quantity;
                }

                decimal totalExpense = 0.0m;

                foreach (var product in productsSold)
                {
                    sql = new StringBuilder();
                    sql.Append("SELECT unitCost ");
                    sql.Append("FROM Products ");
                    sql.Append($"WHERE id = {product.Key}");

                    query = sql.ToString();

                    command = new SqlCommand(query, conn);

                    reader = command.ExecuteReader();
                    dataTable = new DataTable();
                    dataTable.Load(reader);

                    totalExpense += decimal.Parse(dataTable.Rows[0]["unitCost"].ToString()) * product.Value;
                }

                #endregion Get expenses

                #region Net revenue

                decimal netRevenue = grossRevenue - totalExpense;

                #endregion Net revenue

                #region Output

                numOfItemsSold.Text = totalItems.ToString();

                numOfOrders.Text = totalOrders.ToString();

                grossRevenueTb.Text = grossRevenue.ToString("C");

                expenses.Text = totalExpense.ToString("C");

                netRevenueTb.Text = netRevenue.ToString("C");

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