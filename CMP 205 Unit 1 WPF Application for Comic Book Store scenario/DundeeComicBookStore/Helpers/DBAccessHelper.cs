﻿using DundeeComicBookStore.Interfaces;
using DundeeComicBookStore.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

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
                Console.WriteLine("Database connection established");

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
            catch (Exception e)
            {
                string output = $@"Database interaction failed.\nException:\n{e.Message}";
                Console.WriteLine(output);
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
                Permissions = permissions
            };
        }

        #endregion Getting Users

        #region Setting users

        public static IUser SetUser(string firstName, string lastName, string email, string password, string phone, string address/*, string profilPicture*/)
        {
            string hashedPassword = HashPassword(password, email);

            using SqlConnection conn = new SqlConnection(ConnectionHelper.ConnVal("mssql1900040"));
            try
            {
                conn.Open();
                Console.WriteLine("Database connection established");

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
            catch (Exception e)
            {
                string output = $@"Database interaction failed.\nException:\n{e.Message}";
                Console.WriteLine(output);
                return null;
            }
        }

        #endregion Setting users

        #region User Functions

        public static bool IsEmailNotInUse(string email)
        {
            using SqlConnection conn = new SqlConnection(ConnectionHelper.ConnVal("mssql1900040"));
            try
            {
                conn.Open();
                Console.WriteLine("Database connection established");

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
            catch (Exception e)
            {
                string output = $@"Database interaction failed.\nException:\n{e.Message}";
                Console.WriteLine(output);
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

        #region Get products

        public static List<IProduct> GetAllProducts()
        {
            using SqlConnection conn = new SqlConnection(ConnectionHelper.ConnVal("mssql1900040"));
            try
            {
                conn.Open();
                Console.WriteLine("Database connection established");

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
                        UnitsInStock = (uint)(int)row["stockCount"]
                    };
                    productList.Add(product);
                }
                return productList;
            }
            catch (Exception e)
            {
                string output = $@"Database interaction failed.\nException:\n{e.Message}";
                Console.WriteLine(output);
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
                Console.WriteLine("Database connection established");

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
                        UnitsInStock = (uint)(int)row["stockCount"]
                    };
                    productList.Add(product);
                }
                return productList;
            }
            catch (Exception e)
            {
                string output = $"Database interaction failed.\nException:\n{e.Message}";
                output = $"{output}\n{e.InnerException}";
                System.Windows.MessageBox.Show(output);
                return new List<IProduct>();
            }
        }

        #endregion Get products

        #endregion Products

        #region Orders

        #region Save an order

        public static bool SaveOrder(BasketModel basket)
        {
            using SqlConnection conn = new SqlConnection(ConnectionHelper.ConnVal("mssql1900040"));
            try
            {
                conn.Open();
                Console.WriteLine("Database connection established");

                string insertInto = "INSERT INTO";
                string table = "Orders";
                string columns = "(userId,address)";
                string values = $"VALUES (@userId,@address)";
                string query = $"{insertInto} {table} {columns} {values}";

                SqlCommand command = new SqlCommand(query, conn);

                command.Parameters.AddWithValue("userId", basket.User.ID);
                command.Parameters.AddWithValue("address", basket.User.Address);

                int affectedRows = command.ExecuteNonQuery();

                if (affectedRows != 1)
                    return false;

                return true;
            }
            catch (Exception e)
            {
                string output = $@"Database interaction failed.\nException:\n{e.Message}";
                Console.WriteLine(output);
                return false;
            }
        }

        #endregion Save an order

        #endregion Orders

        #region Misc

        public static SqlConnection GetConnectionString(string _string)
        {
            return new SqlConnection(ConnectionHelper.ConnVal(_string));
        }

        #endregion Misc
    }
}