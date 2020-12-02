using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using DundeeComicBookStore.Interfaces;
using DundeeComicBookStore.Models;

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

                string select = "SELECT id,firstName,lastName,phone,email,rewardPoints,permissions";
                //string select = "SELECT *"; use upon full implementation
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
                //Address = new AddressModel((string)row["address"])),
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

        //public static bool IsValidUser(string email, string password)
        //{
        //    HashPassword(ref password, email);

        //    using SqlConnection conn = new SqlConnection(ConnectionHelper.ConnVal("mssql1900040"));
        //    try
        //    {
        //        conn.Open();
        //        Console.WriteLine("Database connection established");

        //        string select = "SELECT id,firstName,lastName,phone,email,address,rewardPoints,permissions";
        //        string from = "FROM Users";
        //        string where = "WHERE email = @email AND password = @password";
        //        string query = $"{select} {from} {where}";

        //        SqlCommand command = new SqlCommand(query, conn);

        //        command.Parameters.AddWithValue("email", email);
        //        command.Parameters.AddWithValue("password", password);

        //        SqlDataReader reader = command.ExecuteReader();
        //        // no rows = no user
        //        if (!reader.HasRows) return false;

        //        return true;
        //    }
        //    catch (Exception e)
        //    {
        //        string output = $@"Database interaction failed.\nException:\n{e.Message}";
        //        Console.WriteLine(output);
        //        return false;
        //    }
        //}

        #endregion Getting Users

        #region Setting users

        public static IUser SetUser(string firstName, string lastName, string email, string password, string phone /*, string address, string profilPicture*/)
        {
            string hashedPassword = HashPassword(password, email);

            using SqlConnection conn = new SqlConnection(ConnectionHelper.ConnVal("mssql1900040"));
            try
            {
                conn.Open();
                Console.WriteLine("Database connection established");

                string insertInto = "INSERT INTO";
                string table = "Users";
                string columns = "(firstName,lastName,phone,email,password)";
                //string columns = "('firstName','lastName','phone','email','address','password','profilePicture')";
                string values = $"VALUES (@firstName,@lastName,@phone,@email,@password)";
                string query = $"{insertInto} {table} {columns} {values}";

                SqlCommand command = new SqlCommand(query, conn);

                command.Parameters.AddWithValue("firstName", firstName);
                command.Parameters.AddWithValue("lastName", lastName);
                command.Parameters.AddWithValue("phone", phone);
                command.Parameters.AddWithValue("email", email);
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

        #endregion Users

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
    }
}