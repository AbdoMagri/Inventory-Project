using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace InventoryDB_DataAccess
{
    public class clsUserData
    {
        public class UserDTO
        {
            public UserDTO(int id, string username,string roleName, bool isActive) { 
                this.Id = id;
                this.UserName = username;
                this.RoleName = roleName;
                this.IsActive = isActive;
               
            }
            public int Id { get; set; }
            public string UserName { get; set; }
            public string RoleName { get; set; }
            public bool IsActive { get; set; }
        }
        public static bool GetUserInfoByUserID(int UserID, ref int AccountID, ref int RoleID,
            ref string UserName, ref string PasswordHash, ref bool IsActive)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT * FROM Users WHERE UserID = @UserID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserID", UserID);
            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    isFound = true;
                    AccountID = (int)reader["AccountID"];
                    RoleID = (int)reader["RoleID"];
                    UserName = (string)reader["UserName"];
                    PasswordHash = (string)reader["PasswordHash"];
                    IsActive = (bool)reader["IsActive"];
                }
                reader.Close();
            }
            catch (Exception){ isFound = false; }
            finally { connection.Close(); }
            return isFound;
        }

        // Used for login: get the hash by username, then verify it in the BLL
        public static bool GetUserInfoByUserName(string UserName, ref int UserID, ref int AccountID,
            ref int RoleID, ref string PasswordHash, ref bool IsActive)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT * FROM Users WHERE UserName = @UserName";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserName", UserName);
            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    isFound = true;
                    UserID = (int)reader["UserID"];
                    AccountID = (int)reader["AccountID"];
                    RoleID = (int)reader["RoleID"];
                    PasswordHash = (string)reader["PasswordHash"];
                    IsActive = (bool)reader["IsActive"];
                }
                reader.Close();
            }
            catch (Exception){ isFound = false; }
            finally { connection.Close(); }
            return isFound;
        }

        public static int AddNewUser(int AccountID, int RoleID, string UserName,
            string PasswordHash, bool IsActive)
        {
            int UserID = -1;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"INSERT INTO Users (AccountID, RoleID, UserName, PasswordHash, IsActive)
                             VALUES (@AccountID, @RoleID, @UserName, @PasswordHash, @IsActive);
                             SELECT SCOPE_IDENTITY();";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@AccountID", AccountID);
            command.Parameters.AddWithValue("@RoleID", RoleID);
            command.Parameters.AddWithValue("@UserName", UserName);
            command.Parameters.AddWithValue("@PasswordHash", PasswordHash);
            command.Parameters.AddWithValue("@IsActive", IsActive);
            try
            {
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int insertedID))
                    UserID = insertedID;
            }
            catch (Exception){ throw;}
            finally { connection.Close(); }
            return UserID;
        }

        // Password is changed separately with ChangePassword
        public static bool UpdateUser(int UserID, int RoleID, string UserName, bool IsActive)
        {
            int rowsAffected = 0;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"UPDATE Users
                             SET RoleID = @RoleID,
                                 UserName = @UserName,
                                 IsActive = @IsActive
                             WHERE UserID = @UserID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserID", UserID);
            command.Parameters.AddWithValue("@RoleID", RoleID);
            command.Parameters.AddWithValue("@UserName", UserName);
            command.Parameters.AddWithValue("@IsActive", IsActive);
            try
            {
                connection.Open();
                rowsAffected = command.ExecuteNonQuery();
            }
            catch (Exception){ return false; }
            finally { connection.Close(); }
            return (rowsAffected > 0);
        }

        public static bool ChangePassword(int UserID, string NewPasswordHash)
        {
            int rowsAffected = 0;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "UPDATE Users SET PasswordHash = @PasswordHash WHERE UserID = @UserID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserID", UserID);
            command.Parameters.AddWithValue("@PasswordHash", NewPasswordHash);
            try
            {
                connection.Open();
                rowsAffected = command.ExecuteNonQuery();
            }
            catch (Exception){ return false; }
            finally { connection.Close(); }
            return (rowsAffected > 0);
        }

        public static List<UserDTO> GetAllUsers(int AccountID)
        {
            var UsersList = new List<UserDTO>();
            
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"SELECT Users.UserID, Users.UserName, RoleName = Roles.Name, Users.IsActive
                             FROM Users INNER JOIN Roles ON Users.RoleID = Roles.RoleID
                             WHERE Users.AccountID = @AccountID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@AccountID", AccountID);
            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read()) {
                    UsersList.Add(new UserDTO(
                        (int)reader["UserID"],
                        (string)reader["UserName"],
                        (string)reader["RoleName"],
                        (bool)reader["IsActive"]
                        ));
                }
                reader.Close();
            }
            catch (Exception){ throw; }
            finally { connection.Close(); }
            return UsersList;
        }

        public static bool DeleteUser(int UserID)
        {
            int rowsAffected = 0;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "DELETE Users WHERE UserID = @UserID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserID", UserID);
            try
            {
                connection.Open();
                rowsAffected = command.ExecuteNonQuery();
            }
            catch (Exception){ throw;}
            finally { connection.Close(); }
            return (rowsAffected > 0);
        }

        public static bool IsUserExist(int UserID)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT Found=1 FROM Users WHERE UserID = @UserID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserID", UserID);
            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                isFound = reader.HasRows;
                reader.Close();
            }
            catch (Exception){ isFound = false; }
            finally { connection.Close(); }
            return isFound;
        }

        public static bool IsUserExist(string UserName)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT Found=1 FROM Users WHERE UserName = @UserName";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserName", UserName);
            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                isFound = reader.HasRows;
                reader.Close();
            }
            catch (Exception){ isFound = false; }
            finally { connection.Close(); }
            return isFound;
        }
    }
}