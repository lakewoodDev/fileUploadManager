using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uploading.Data
{
    public class UserManager
    {
        private readonly string _connectionString;

        public UserManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddUser(string username, string password)
        {
            string salt = PasswordHelper.GenerateRandomSalt();
            string hash = PasswordHelper.HashPassword(password, salt);
            User user = new User
            {
                Username = username,
                PasswordHash = hash,
                PasswordSalt = salt
            };
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText =
                    "INSERT INTO Users (Username, PasswordHash, PasswordSalt) VALUES (@username, @hash, @salt)";
                command.Parameters.AddWithValue("@username", user.Username);
                command.Parameters.AddWithValue("@hash", user.PasswordHash);
                command.Parameters.AddWithValue("@salt", user.PasswordSalt);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public string CheckUserAvailability(string username)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText =
                    "SELECT Username FROM Users WHERE Username = @username";
                command.Parameters.AddWithValue("@username", username);
                connection.Open();
                string user = null;
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    user = (string) reader["Username"];
                }
                return user;
            }
        }

        public User Login(string username, string password)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Users WHERE Username = @username";
                command.Parameters.AddWithValue("@username", username);
                connection.Open();
                var reader = command.ExecuteReader();
                if (!reader.Read())
                {
                    return null;
                }

                User user = new User();
                user.Id = (int)reader["Id"];
                user.PasswordHash = (string)reader["PasswordHash"];
                user.PasswordSalt = (string)reader["PasswordSalt"];
                user.Username = (string)reader["Username"];

                if (!PasswordHelper.PasswordMatch(password, user.PasswordSalt, user.PasswordHash))
                {
                    return null;
                }

                return user;
            }
        }
    }
}
