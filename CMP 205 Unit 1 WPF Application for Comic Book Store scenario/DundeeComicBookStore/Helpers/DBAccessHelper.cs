using DundeeComicBookStore.Interfaces;
using DundeeComicBookStore.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static DundeeComicBookStore.Models.StaffModel;

namespace DundeeComicBookStore
{
    public class DBAccessHelper
    {
        #region Users

        #region Getting Users

        public static IUser GetUser(string email, string password)
        {
            string hashedPassword = HashPassword(password, email);

            using SqlConnection conn = new SqlConnection(ConnectionHelper.ConnVal("mssql1900040"));
            try
            {
                conn.Open();

                string select = "SELECT *";
                string from = "FROM Users";
                string where = "WHERE email = @email AND password = @password";
                string query = $"{select} {from} {where}";

                SqlCommand command = new SqlCommand(query, conn);

                command.Parameters.AddWithValue("email", email);
                command.Parameters.AddWithValue("password", hashedPassword);

                SqlDataReader reader = command.ExecuteReader();
                DataTable dataTable = new DataTable();
                dataTable.Load(reader);
                // there must be 1 user returned
                if (dataTable.Rows.Count != 1)
                    return null;

                DataRow data = dataTable.Rows[0];
                // check if user
                byte permissions;
                if (data["permissions"] == DBNull.Value)
                    permissions = 0;
                else permissions = ((byte[])data["permissions"])[0];

                bool isCustomer = permissions == 0;

                if (isCustomer) return GenerateUser(data);
                else return GenerateStaff(data, (byte)permissions);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static IUser GetUser(int userId)
        {
            using SqlConnection conn = new SqlConnection(ConnectionHelper.ConnVal("mssql1900040"));
            try
            {
                conn.Open();

                string select = "SELECT *";
                string from = "FROM Users";
                string where = "WHERE id = @userId";
                string query = $"{select} {from} {where}";

                SqlCommand command = new SqlCommand(query, conn);

                command.Parameters.AddWithValue("userId", userId);

                SqlDataReader reader = command.ExecuteReader();
                DataTable dataTable = new DataTable();
                dataTable.Load(reader);
                // there must be 1 user returned
                if (dataTable.Rows.Count != 1)
                    return null;

                DataRow data = dataTable.Rows[0];
                // check if user
                byte permissions;
                if (data["permissions"] == DBNull.Value)
                    permissions = 0;
                else permissions = ((byte[])data["permissions"])[0];

                bool isCustomer = permissions == 0;

                if (isCustomer) return GenerateUser(data);
                else return GenerateStaff(data, permissions);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static DataTable GetUsers(bool canAccessEmployees)
        {
            using SqlConnection conn = new SqlConnection(ConnectionHelper.ConnVal("mssql1900040"));
            try
            {
                conn.Open();

                string select = "SELECT *";
                string from = "FROM Users";
                string where = "";
                if (!canAccessEmployees)
                    where = $"WHERE permissions IS NULL";
                string query = $"{select} {from} {where}";

                SqlCommand command = new SqlCommand(query, conn);

                SqlDataReader reader = command.ExecuteReader();
                DataTable dataTable = new DataTable();
                dataTable.Load(reader);

                dataTable.Columns.Remove("permissions");
                dataTable.Columns.Remove("password");
                dataTable.Columns.Remove("profilepicture");

                return dataTable;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static DataTable GetStaff(bool canAccessEmployees)
        {
            if (!canAccessEmployees) return null;
            using SqlConnection conn = new SqlConnection(ConnectionHelper.ConnVal("mssql1900040"));
            try
            {
                conn.Open();

                string select = "SELECT *";
                string from = "FROM Users";
                string where = $"WHERE permissions IS NOT NULL";
                string query = $"{select} {from} {where}";

                SqlCommand command = new SqlCommand(query, conn);

                SqlDataReader reader = command.ExecuteReader();
                DataTable dataTable = new DataTable();
                dataTable.Load(reader);

                dataTable.Columns.Remove("password");
                dataTable.Columns.Remove("profilepicture");

                return dataTable;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static IUser GenerateUser(DataRow data)
        {
            return new CustomerModel()
            {
                ID = (int)data["id"],
                FirstName = (string)data["firstName"],
                LastName = (string)data["lastName"],
                EmailAddress = (string)data["email"],
                Address = (string)data["address"],
                PhoneNumber = (string)data["phone"],
                //ProfilePictureSource = (string)row["profilePicture"],
                RewardPoints = (uint)(int)data["rewardPoints"]
            };
        }

        private static IUser GenerateStaff(DataRow data, byte permissions)
        {
            return new StaffModel()
            {
                ID = (int)data["id"],
                FirstName = (string)data["firstName"],
                LastName = (string)data["lastName"],
                EmailAddress = (string)data["email"],
                //Address = new AddressModel((string)row["address"])),
                PhoneNumber = (string)data["phone"],
                //ProfilePictureSource = (string)row["profilePicture"],
                RewardPoints = (uint)(int)data["rewardPoints"],
                Permissions = CalculatePermissions(permissions)
            };
        }

        public static BitArray CalculatePermissions(byte permsToCheck)
        {
            BitArray perms = new BitArray(8);

            // check for all setter
            if (permsToCheck == 127)
            {
                for (int i = 0; i < 8; i++)
                    perms[i] = true;
                return perms;
            }

            // check for no perms
            if (permsToCheck == 0)
            {
                for (int i = 0; i < 8; i++)
                    perms[i] = false;
                return perms;
            }

            // otherwise individual permissions
            perms[(byte)Permission.ReadCustomerData] = ((byte)StaffPermissions.ReadCustomerData & permsToCheck) > 0;
            perms[(byte)Permission.WriteCustomerData] = ((byte)StaffPermissions.WriteCustomerData & permsToCheck) > 0;
            perms[(byte)Permission.DeleteCustomerData] = ((byte)StaffPermissions.DeleteCustomerData & permsToCheck) > 0;
            perms[(byte)Permission.ReadStockData] = ((byte)StaffPermissions.ReadStockData & permsToCheck) > 0;
            perms[(byte)Permission.WriteStockData] = ((byte)StaffPermissions.WriteStockData & permsToCheck) > 0;
            perms[(byte)Permission.DeleteStockData] = ((byte)StaffPermissions.DeleteStockData & permsToCheck) > 0;
            perms[(byte)Permission.AccessEmployeeData] = ((byte)StaffPermissions.AccessEmployeeData & permsToCheck) > 0;
            return perms;
        }

        #endregion Getting Users

        #region Setting Users

        public static IUser SetUser(string firstName, string lastName, string email, string password, string phone, string address/*, string profilPicture*/)
        {
            string hashedPassword = HashPassword(password, email);

            using SqlConnection conn = new SqlConnection(ConnectionHelper.ConnVal("mssql1900040"));
            try
            {
                conn.Open();

                string insertInto = "INSERT INTO";
                string table = "Users";
                string columns = "(firstName,lastName,phone,email,address,password)";
                //string columns = "('firstName','lastName','phone','email','address','password','profilePicture')";
                string values = $"VALUES (@firstName,@lastName,@phone,@email,@address,@password)";
                string query = $"{insertInto} {table} {columns} {values}";

                SqlCommand command = new SqlCommand(query, conn);

                command.Parameters.AddWithValue("firstName", firstName);
                command.Parameters.AddWithValue("lastName", lastName);
                command.Parameters.AddWithValue("phone", phone);
                command.Parameters.AddWithValue("email", email);
                command.Parameters.AddWithValue("address", address);
                command.Parameters.AddWithValue("password", hashedPassword);

                int affectedRows = command.ExecuteNonQuery();

                if (affectedRows != 1)
                {
                    System.Windows.MessageBox.Show("Something went wrong inserting data");
                    return null;
                }

                return GetUser(email, password);
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion Setting Users

        #region Deleting Users

        public static bool DeleteUser(int id)
        {
            using SqlConnection conn = new SqlConnection(ConnectionHelper.ConnVal("mssql1900040"));
            try
            {
                conn.Open();

                StringBuilder sql = new StringBuilder();

                sql.Append("DELETE FROM ");
                sql.Append("Users ");
                sql.Append("WHERE id = @id");

                SqlCommand command = new SqlCommand(sql.ToString(), conn);

                command.Parameters.AddWithValue("id", id);

                int affected = command.ExecuteNonQuery();
                if (affected == 1)
                    return true;
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion Deleting Users

        #region Updating Users

        public static bool AlterUser(CustomerModel changedModel)
        {
            using SqlConnection conn = new SqlConnection(ConnectionHelper.ConnVal("mssql1900040"));
            try
            {
                conn.Open();

                StringBuilder sql = new StringBuilder();

                sql.Append("UPDATE ");
                sql.Append("Users ");
                sql.Append("SET firstName = @firstName, lastName = @lastName, ");
                sql.Append("phone = @phone, email = @email, address = @address ");
                sql.Append("WHERE id = @id");

                SqlCommand command = new SqlCommand(sql.ToString(), conn);

                command.Parameters.AddWithValue("firstName", changedModel.FirstName);
                command.Parameters.AddWithValue("lastName", changedModel.LastName);
                command.Parameters.AddWithValue("phone", changedModel.PhoneNumber);
                command.Parameters.AddWithValue("email", changedModel.EmailAddress);
                command.Parameters.AddWithValue("address", changedModel.Address);
                command.Parameters.AddWithValue("id", changedModel.ID);

                int affected = command.ExecuteNonQuery();
                if (affected == 1)
                    return true;
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion Updating Users

        #region User Functions

        public static bool IsEmailNotInUse(string email)
        {
            using SqlConnection conn = new SqlConnection(ConnectionHelper.ConnVal("mssql1900040"));
            try
            {
                conn.Open();

                string select = "SELECT email";
                string from = "FROM Users";
                string where = "WHERE email = @email";
                string query = $"{select} {from} {where}";

                SqlCommand command = new SqlCommand(query, conn);

                command.Parameters.AddWithValue("email", email);

                SqlDataReader reader = command.ExecuteReader();
                DataTable dataTable = new DataTable();
                dataTable.Load(reader);
                // there must be 1 user returned
                if (dataTable.Rows.Count != 0)
                    return false;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static string HashPassword(string password, string email)
        {
            // get salt
            string[] salt = GetPasswordSalt(email);
            // apply salt to password
            return SaltPassword(password, salt);
        }

        private static string[] GetPasswordSalt(string email)
        {
            string username = email.Split('@')[0];
            string[] output = new string[2];

            // even length
            if (username.Length % 2 == 0)
            {
                // get first half of username
                output[0] = username.Substring(0, username.Length / 2);
                // get second half of username
                output[1] = username.Substring(username.Length / 2);
                return output;
            }

            // odd length
            // get first half of username
            output[0] = username.Substring(0, (username.Length - 1) / 2);
            // get second half of username
            output[1] = username.Substring(username.Length / 2);
            return output;
        }

        private static string SaltPassword(string password, string[] salt)
        {
            string value = $"{salt[0]} {password} {salt[1]}";
            // hash the password value
            byte[] passwordBytes = Encoding.UTF8.GetBytes(value);
            var sha = new SHA256Managed();
            byte[] hash = sha.ComputeHash(passwordBytes);

            value = string.Empty;

            foreach (byte b in hash)
            {
                value += b.ToString("x2");
            }

            return value;
        }

        #endregion User Functions

        #endregion Users

        #region Products

        #region Get Products

        public static List<IProduct> GetAllProducts()
        {
            using SqlConnection conn = new SqlConnection(ConnectionHelper.ConnVal("mssql1900040"));
            try
            {
                conn.Open();

                string select = "SELECT id,name,description,unitPrice,stockCount";
                string from = "FROM Products";
                string query = $"{select} {from}";

                SqlCommand command = new SqlCommand(query, conn);

                SqlDataReader reader = command.ExecuteReader();
                DataTable dataTable = new DataTable();
                dataTable.Load(reader);

                var productList = new List<IProduct>();

                // see if the list is empty

                if (dataTable.Rows.Count < 1)
                    return productList;

                foreach (DataRow row in dataTable.Rows)
                {
                    var product = new ProductModel()
                    {
                        ID = (int)row["id"],
                        Name = (string)row["name"],
                        Description = (string)row["description"],
                        UnitPrice = (decimal)row["unitPrice"],
                        UnitsInStock = (int)row["stockCount"]
                    };
                    productList.Add(product);
                }
                return productList;
            }
            catch (Exception)
            {
                return new List<IProduct>();
            }
        }

        public static List<IProduct> GetProducts(string query, Dictionary<string, object> parameters)
        {
            using SqlConnection conn = new SqlConnection(ConnectionHelper.ConnVal("mssql1900040"));
            try
            {
                var command = new SqlCommand(query, conn);
                foreach (var item in parameters)
                    command.Parameters.AddWithValue(item.Key, item.Value);

                conn.Open();

                SqlDataReader reader = command.ExecuteReader();
                DataTable dataTable = new DataTable();
                dataTable.Load(reader);

                var productList = new List<IProduct>();

                // see if the list is empty

                if (dataTable.Rows.Count < 1)
                    return productList;

                foreach (DataRow row in dataTable.Rows)
                {
                    var product = new ProductModel()
                    {
                        ID = (int)row["id"],
                        Name = (string)row["name"],
                        Description = (string)row["description"],
                        UnitPrice = (decimal)row["unitPrice"],
                        UnitsInStock = (int)row["stockCount"]
                    };
                    productList.Add(product);
                }
                return productList;
            }
            catch (Exception)
            {
                return new List<IProduct>();
            }
        }

        public static DataTable GetProductsReturnDataTable(string query)
        {
            using SqlConnection conn = new SqlConnection(ConnectionHelper.ConnVal("mssql1900040"));
            try
            {
                var command = new SqlCommand(query, conn);

                conn.Open();

                SqlDataReader reader = command.ExecuteReader();
                DataTable dataTable = new DataTable();
                dataTable.Load(reader);

                return dataTable;
            }
            catch (Exception)
            {
                return new DataTable();
            }
        }

        public static DataTable GetProductsReturnDataTable(string query, Dictionary<string, object> parameters)
        {
            using SqlConnection conn = new SqlConnection(ConnectionHelper.ConnVal("mssql1900040"));
            try
            {
                var command = new SqlCommand(query, conn);
                foreach (var item in parameters)
                    command.Parameters.AddWithValue(item.Key, item.Value);

                conn.Open();

                SqlDataReader reader = command.ExecuteReader();
                DataTable dataTable = new DataTable();
                dataTable.Load(reader);

                return dataTable;
            }
            catch (Exception)
            {
                return new DataTable();
            }
        }

        public static IProduct GetProductById(int productId)
        {
            using SqlConnection conn = new SqlConnection(ConnectionHelper.ConnVal("mssql1900040"));
            try
            {
                conn.Open();

                string select = "SELECT id,name,description,unitPrice,stockCount,unitCost";
                string from = "FROM Products";
                string where = "WHERE id = @productId";
                string query = $"{select} {from} {where}";

                SqlCommand command = new SqlCommand(query, conn);

                command.Parameters.AddWithValue("productId", productId);

                SqlDataReader reader = command.ExecuteReader();
                DataTable dataTable = new DataTable();
                dataTable.Load(reader);

                if (dataTable.Rows.Count != 1)
                    return null;

                DataRow data = dataTable.Rows[0];

                return new ProductModel()
                {
                    ID = (int)data["id"],
                    Name = (string)data["name"],
                    Description = (string)data["description"],
                    UnitPrice = (decimal)data["unitPrice"],
                    UnitsInStock = (int)data["stockCount"],
                    UnitCost = (decimal)data["unitCost"]
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion Get Products

        #region Deleting Products

        public static bool DeleteProduct(int id)
        {
            using SqlConnection conn = new SqlConnection(ConnectionHelper.ConnVal("mssql1900040"));
            try
            {
                conn.Open();

                StringBuilder sql = new StringBuilder();

                sql.Append("DELETE FROM ");
                sql.Append("Products ");
                sql.Append("WHERE id = @id");

                SqlCommand command = new SqlCommand(sql.ToString(), conn);

                command.Parameters.AddWithValue("id", id);

                int affected = command.ExecuteNonQuery();
                if (affected == 1)
                    return true;
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion Deleting Products

        #region Add Products

        public static bool AddProduct(ProductModel newProduct)
        {
            using SqlConnection conn = new SqlConnection(ConnectionHelper.ConnVal("mssql1900040"));
            try
            {
                conn.Open();

                StringBuilder sql = new StringBuilder();

                sql.Append("INSERT INTO ");
                sql.Append("Products ");
                sql.Append("(name,description,unitPrice,stockCount,unitCost) ");
                sql.Append("VALUES ");
                sql.Append("(@name,@description,@unitPrice,@stockCount,@unitCost)");

                SqlCommand command = new SqlCommand(sql.ToString(), conn);

                command.Parameters.AddWithValue("name", newProduct.Name);
                command.Parameters.AddWithValue("description", newProduct.Description);
                command.Parameters.AddWithValue("unitPrice", newProduct.UnitPrice);
                command.Parameters.AddWithValue("stockCount", newProduct.UnitsInStock);
                command.Parameters.AddWithValue("unitCost", newProduct.UnitCost);

                int affected = command.ExecuteNonQuery();
                if (affected == 1)
                    return true;
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion Add Products

        #endregion Products

        #region Orders

        #region Save an order

        public static bool SaveOrder(OrderModel order)
        {
            using SqlConnection conn = new SqlConnection(ConnectionHelper.ConnVal("mssql1900040"));
            try
            {
                conn.Open();

                // if the order is not being edited
                if (!order.BeingEdited)
                    SaveNewOrder(conn, order);
                else SaveExistingOrder(conn, order);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static void SaveExistingOrder(SqlConnection conn, OrderModel order)
        {
            // get the order currently in the database
            string select = "SELECT";
            string columns = "productId,quantity";
            string from = "FROM";
            string table = "OrderItems";
            string where = $"WHERE";
            string conditions = $"orderId = @orderId";
            string query = $"{select} {columns} {from} {table} {where} {conditions}";

            var command = new SqlCommand(query, conn);

            command.Parameters.AddWithValue($"orderId", order.ID);

            SqlDataReader reader = command.ExecuteReader();

            DataTable dataTable = new DataTable();
            dataTable.Load(reader);

            var dbItems = new Dictionary<int, int>();

            foreach (DataRow row in dataTable.Rows)
            {
                int product = (int)row["productId"];
                int quantity = (int)row["quantity"];
                dbItems.Add(product, quantity);
            }

            var existingItems = new BasketModel();
            var existingItemsToUpdate = new BasketModel();
            var newItems = new BasketModel();

            // iterate rows to identify products

            foreach (var product in order.Basket.Items)
            {
                //if existing
                if (dbItems.ContainsKey(product.Key.ID))
                {
                    if (product.Value == dbItems[product.Key.ID])
                        existingItems.Add(product.Key, product.Value);
                    else
                        existingItemsToUpdate.Add(product.Key, product.Value);
                }
                else
                    newItems.Add(product.Key, product.Value);
            }

            // update quantities for items already in basket
            if (existingItemsToUpdate.Items.Count > 0)
            {
                foreach (var product in existingItemsToUpdate.Items)
                {
                    int productId = product.Key.ID;
                    int quantity = product.Value;

                    string update = "UPDATE";
                    table = "OrderItems";
                    string set = "SET";
                    columns = "quantity = @quantity";
                    where = $"WHERE";
                    conditions = $"orderId = @orderId AND productId = @productId";
                    query = $"{update} {table} {set} {columns} {where} {conditions}";

                    command = new SqlCommand(query, conn);

                    command.Parameters.AddWithValue($"quantity", quantity);
                    command.Parameters.AddWithValue($"orderId", order.ID);
                    command.Parameters.AddWithValue($"productId", productId);

                    command.ExecuteNonQuery();
                }
            }

            // check if any items remain
            // if not return
            if (newItems.Items.Count < 1) return;

            // there are new items
            foreach (var product in newItems.Items)
            {
                int productId = product.Key.ID;
                int quantity = product.Value;

                string insertInto = "INSERT INTO";
                table = "OrderItems";
                columns = "(orderId,productId,quantity)";
                string values = $"VALUES (@orderId,@productId,@quantity)";
                query = $"{insertInto} {table} {columns} {values}";

                command = new SqlCommand(query, conn);

                command.Parameters.AddWithValue($"orderId", order.ID);
                command.Parameters.AddWithValue($"productId", productId);
                command.Parameters.AddWithValue($"quantity", quantity);

                command.ExecuteNonQuery();
            }
        }

        private static void SaveNewOrder(SqlConnection conn, OrderModel order)
        {
            string insertInto = "INSERT INTO";
            string table = "Orders";
            string columns = "(userId,address)";
            string output = "OUTPUT inserted.id";
            string values = $"VALUES (@userId,@address)";
            string query = $"{insertInto} {table} {columns} {output} {values}";

            SqlCommand command = new SqlCommand(query, conn);

            BasketModel basket = order.Basket;

            command.Parameters.AddWithValue("userId", order.User.ID);
            command.Parameters.AddWithValue("address", order.User.Address);

            SqlDataReader reader = command.ExecuteReader();

            DataTable dataTable = new DataTable();
            dataTable.Load(reader);

            DataRow data = dataTable.Rows[0];

            Dictionary<IProduct, int> items = basket.Items;

            int lastOrderId = (int)data["id"];

            insertInto = "INSERT INTO";
            table = "OrderItems";
            columns = "(orderId,productId,quantity)";
            values = $"VALUES ";
            int count = 1;
            foreach (var item in items)
            {
                if (count > 1) values += ",";
                values += $"({lastOrderId},@productId{count},@quantity{count})";
                count++;
            }

            query = $"{insertInto} {table} {columns} {values}";

            command = new SqlCommand(query, conn);

            count = 1;

            foreach (var item in items)
            {
                command.Parameters.AddWithValue($"productId{count}", item.Key.ID);
                command.Parameters.AddWithValue($"quantity{count}", item.Value);
                count++;
            }

            command.ExecuteNonQuery();
        }

        #endregion Save an order

        #region Get orders

        public static List<OrderModel> GetOrders(int userId)
        {
            using SqlConnection conn = new SqlConnection(ConnectionHelper.ConnVal("mssql1900040"));
            try
            {
                conn.Open();

                // Get orders

                string select = "SELECT Orders.id AS order_id, Orders.address,Orders.orderDate";
                string from = "FROM Orders";
                string where = "WHERE Orders.userId = @userId";
                string query = $"{select} {from} {where}";

                SqlCommand command = new SqlCommand(query, conn);

                command.Parameters.AddWithValue("userId", userId);

                SqlDataReader reader = command.ExecuteReader();
                DataTable dataTable = new DataTable();
                dataTable.Load(reader);

                if (dataTable.Rows.Count < 1)
                    return null;

                var orders = new List<OrderModel>();

                foreach (DataRow row in dataTable.Rows)
                {
                    var order = new OrderModel()
                    {
                        ID = (int)row["order_id"],
                        Address = (string)row["address"],
                        PlacedAt = (DateTime)row["orderDate"]
                    };
                    orders.Add(order);
                }

                // get items ordered by user

                foreach (var order in orders)
                {
                    select = "SELECT OrderItems.productId, OrderItems.quantity";
                    from = "FROM OrderItems";
                    where = $"WHERE OrderItems.orderId = {order.ID}";
                    query = $"{select} {from} {where}";

                    command = new SqlCommand(query, conn);

                    command.Parameters.AddWithValue("userId", userId);

                    reader = command.ExecuteReader();
                    dataTable = new DataTable();
                    dataTable.Load(reader);

                    if (dataTable.Rows.Count < 1)
                        return null;

                    var orderBasket = new BasketModel();

                    foreach (DataRow row in dataTable.Rows)
                    {
                        int productId = (int)row["productId"];
                        int quantity = (int)row["quantity"];
                        IProduct product = GetProductById(productId);
                        orderBasket.Items.Add(product, quantity);
                    }

                    // check for made payments

                    select = "SELECT id";
                    from = "FROM Payments";
                    where = $"WHERE orderId = {order.ID}";
                    query = $"{select} {from} {where}";

                    command = new SqlCommand(query, conn);

                    reader = command.ExecuteReader();
                    dataTable = new DataTable();
                    dataTable.Load(reader);

                    order.Complete = dataTable.Rows.Count > 0;
                    order.User = GetUser(userId);
                    order.Basket = orderBasket;
                }

                return orders;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static DataTable GetOrders(string query)
        {
            using SqlConnection conn = new SqlConnection(ConnectionHelper.ConnVal("mssql1900040"));
            try
            {
                // Get orders
                SqlCommand command = new SqlCommand(query, conn);
                conn.Open();

                SqlDataReader reader = command.ExecuteReader();
                DataTable dataTable = new DataTable();
                dataTable.Load(reader);
                return dataTable;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static DataTable GetOrders(string query, Dictionary<string, object> parameters)
        {
            using SqlConnection conn = new SqlConnection(ConnectionHelper.ConnVal("mssql1900040"));
            try
            {
                // Get orders

                SqlCommand command = new SqlCommand(query, conn);
                foreach (var item in parameters)
                    command.Parameters.AddWithValue(item.Key, item.Value);

                conn.Open();

                SqlDataReader reader = command.ExecuteReader();
                DataTable dataTable = new DataTable();
                dataTable.Load(reader);
                return dataTable;
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion Get orders

        #region Delete an order

        public static bool DeleteOrder(OrderModel order)
        {
            using SqlConnection conn = new SqlConnection(ConnectionHelper.ConnVal("mssql1900040"));
            try
            {
                conn.Open();

                string deleteFROM = "DELETE FROM";
                string table = "Orders";
                string where = "WHERE id = @orderId";
                string query = $"{deleteFROM} {table} {where}";

                SqlCommand command = new SqlCommand(query, conn);

                command.Parameters.AddWithValue("orderId", order.ID);

                int affected = command.ExecuteNonQuery();
                if (affected == 1) return true;
                else return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion Delete an order

        #region Process order

        public static bool ProcessOrder(OrderModel order)
        {
            using SqlConnection conn = new SqlConnection(ConnectionHelper.ConnVal("mssql1900040"));
            if (order.ID == 0)
                return ProcessNewOrder(conn, order);
            else return ProcessExistingOrder(conn, order);
        }

        private static bool ProcessNewOrder(SqlConnection conn, OrderModel order)
        {
            try
            {
                conn.Open();

                #region insert the order into Orders, OrderItems

                string insertInto = "INSERT INTO";
                string table = "Orders";
                string columns = "(userId,address)";
                string output = "OUTPUT inserted.id";
                string values = $"VALUES (@userId,@address)";
                string query = $"{insertInto} {table} {columns} {output} {values}";

                SqlCommand command = new SqlCommand(query, conn);

                BasketModel basket = order.Basket;

                command.Parameters.AddWithValue("userId", order.User.ID);
                command.Parameters.AddWithValue("address", order.User.Address);

                SqlDataReader reader = command.ExecuteReader();

                DataTable dataTable = new DataTable();
                dataTable.Load(reader);

                DataRow data = dataTable.Rows[0];

                Dictionary<IProduct, int> items = basket.Items;

                int lastOrderId = (int)data["id"];

                insertInto = "INSERT INTO";
                table = "OrderItems";
                columns = "(orderId,productId,quantity)";
                values = $"VALUES ";
                int count = 1;
                foreach (var item in items)
                {
                    if (count > 1) values += ",";
                    values += $"({lastOrderId},@productId{count},@quantity{count})";
                    count++;
                }

                query = $"{insertInto} {table} {columns} {values}";

                command = new SqlCommand(query, conn);

                count = 1;

                foreach (var item in items)
                {
                    command.Parameters.AddWithValue($"productId{count}", item.Key.ID);
                    command.Parameters.AddWithValue($"quantity{count}", item.Value);
                    count++;
                }

                command.ExecuteNonQuery();

                #endregion insert the order into Orders, OrderItems

                #region insert the payment information

                insertInto = "INSERT INTO";
                table = "Payments";
                columns = "(orderId,type,amount)";
                values = "VALUES (@orderId,@type,@amount)";
                query = $"{insertInto} {table} {columns} {values}";

                command = new SqlCommand(query, conn);

                command.Parameters.AddWithValue("orderId", lastOrderId);

                switch (order.PaymentType)
                {
                    case 0:
                        command.Parameters.AddWithValue("type", "card");
                        break;

                    case 1:
                        command.Parameters.AddWithValue("type", "cash");
                        break;
                }

                command.Parameters.AddWithValue("amount", order.Basket.Total);

                int affected = command.ExecuteNonQuery();
                if (affected == 1) return true;
                else return false;

                #endregion insert the payment information
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool ProcessExistingOrder(SqlConnection conn, OrderModel order)
        {
            try
            {
                conn.Open();

                #region insert the payment information

                string insertInto = "INSERT INTO";
                string table = "Payments";
                string columns = "(orderId,type,amount)";
                string values = "VALUES (@orderId,@type,@amount)";
                string query = $"{insertInto} {table} {columns} {values}";

                SqlCommand command = new SqlCommand(query, conn);

                command.Parameters.AddWithValue("orderId", order.ID);

                switch (order.PaymentType)
                {
                    case 0:
                        command.Parameters.AddWithValue("type", "card");
                        break;

                    case 1:
                        command.Parameters.AddWithValue("type", "cash");
                        break;
                }

                command.Parameters.AddWithValue("amount", order.Basket.Total);

                int affected = command.ExecuteNonQuery();
                if (affected == 1) return true;
                else return false;

                #endregion insert the payment information
            }
            catch (Exception)
            {
                return false;
            }
        }

        /* process existing
        private static bool ProcessNewOrder(SqlConnection conn, OrderModel order)
        {
            try
            {
                conn.Open();

                string insertInto = "INSERT INTO";
                string table = "Payments";
                string columns = "orderId,type,amount";
                string values = "VALUES (@orderId,@type,@amount)";
                string query = $"{insertInto} {table} {columns} {values}";

                SqlCommand command = new SqlCommand(query, conn);

                command.Parameters.AddWithValue("orderId", order.ID);

                switch (order.PaymentType)
                {
                    case 0:
                        command.Parameters.AddWithValue("type", "card");
                        break;

                    case 1:
                        command.Parameters.AddWithValue("type", "cash");
                        break;
                }

                command.Parameters.AddWithValue("amount", order.Basket.Total);

                int affected = command.ExecuteNonQuery();
                if (affected == 1) return true;
                else return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        */

        #endregion Process order

        #endregion Orders

        #region Misc

        public static SqlConnection GetConnectionString(string _string)
        {
            return new SqlConnection(ConnectionHelper.ConnVal(_string));
        }

        #endregion Misc
    }
}