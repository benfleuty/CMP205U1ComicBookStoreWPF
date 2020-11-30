using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Dapper;
using DundeeComicBookStore.Interfaces;

namespace DundeeComicBookStore
{
    public class DBAccessHelper
    {
        public static bool IsValidUser(string email, string password)
        {
            HashPassword(ref password, email);
            using IDbConnection connection = new System.Data.SqlClient.SqlConnection(ConnectionHelper.ConnVal("mssql1900040"));
            List<IUser> returned = connection.Query<IUser>($"dbo.Users_GetByLogin @email @password", new { email = email, password = password }).ToList();
            // If there is only one account returned
            // then the user is valid
            // 0 means no user
            // more than 1 means something has gone WRONG
            return returned.Count == 1;
        }

        private static void HashPassword(ref string password, string email)
        {
            // get salt
            string[] salt = GetPasswordSalt(email);
            // apply salt to password
            SaltPassword(ref password, salt);
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
                output[1] = username.Substring(username.Length + 1, username.Length);
                return output;
            }

            // odd length
            // get first half of username
            output[0] = username.Substring(0, (username.Length - 1) / 2);
            // get second half of username
            output[1] = username.Substring((username.Length + 1) / 2, username.Length);
            return output;
        }

        private static void SaltPassword(ref string password, string[] salt)
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

            password = value;
        }
    }
}