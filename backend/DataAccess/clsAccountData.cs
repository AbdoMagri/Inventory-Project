using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace InventoryDB_DataAccess
{
    public class clsAccountData
    {

        public class AccountDTO
        {
            public AccountDTO(int accountID, string name, DateTime createdAt, string imagePath)
            {
                AccountID = accountID; Name = name; CreatedAt = createdAt; ImagePath = imagePath;
            }
            public int AccountID { get; set; }
            public string Name { get; set; }
            public DateTime CreatedAt { get; set; }
            public string ImagePath { get; set; }
        }
        public class AccountDTOForUpdate
        {
            public AccountDTOForUpdate(string name, string imagePath)
            {
                Name = name;  ImagePath = imagePath;
            }
            public string Name { get; set; }
            public string ImagePath { get; set; }
        }
        public static bool GetAccountInfoByID(int AccountID, ref string Name, ref DateTime CreatedAt, ref string ImagePath)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT * FROM Accounts WHERE AccountID = @AccountID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@AccountID", AccountID);
            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    isFound = true;
                    Name = (string)reader["Name"];
                    CreatedAt = (DateTime)reader["CreatedAt"];
                    ImagePath = reader["ImagePath"] != DBNull.Value ? (string)reader["ImagePath"] : "";
                }
                reader.Close();
            }
            catch (Exception){ isFound = false;  }
            finally { connection.Close(); }
            return isFound;
        }

        public static int AddNewAccount(string Name, string ImagePath)
        {
            int AccountID = -1;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"INSERT INTO Accounts (Name, ImagePath) VALUES (@Name, @ImagePath);
                 SELECT SCOPE_IDENTITY();";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Name", Name);
            command.Parameters.AddWithValue("@ImagePath",
    string.IsNullOrEmpty(ImagePath) ? (object)DBNull.Value : ImagePath);
            try
            {
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int insertedID))
                    AccountID = insertedID;
            }
            catch (Exception){ throw; }
            finally { connection.Close(); }
            return AccountID;
        }

        public static bool UpdateAccount(int AccountID, string Name, string ImagePath)
        {
            int rowsAffected = 0;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"UPDATE Accounts
                 SET Name = @Name,
                     ImagePath = @ImagePath
                 WHERE AccountID = @AccountID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@AccountID", AccountID);
            command.Parameters.AddWithValue("@Name", Name);
            command.Parameters.AddWithValue("@ImagePath",
    string.IsNullOrEmpty(ImagePath) ? (object)DBNull.Value : ImagePath);
            try
            {
                connection.Open();
                rowsAffected = command.ExecuteNonQuery();
            }
            catch (Exception){ return false; }
            finally { connection.Close(); }
            return (rowsAffected > 0);
        }

        public static List<AccountDTO> GetAllAccounts()
        {
            List<AccountDTO> accounts = new List<AccountDTO>();

            SqlConnection connection = new SqlConnection(
                clsDataAccessSettings.ConnectionString);

            string query = @"SELECT AccountID, Name, CreatedAt, ImagePath
                     FROM Accounts";

            SqlCommand command = new SqlCommand(query, connection);

            try
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    accounts.Add(new AccountDTO(
                        (int)reader["AccountID"],
                        reader["Name"].ToString(),
                        (DateTime)reader["CreatedAt"],
                        reader["ImagePath"] == DBNull.Value
                            ? null
                            : reader["ImagePath"].ToString()
                    ));
                }

                reader.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                connection.Close();
            }

            return accounts;
        }
        //public static bool DeleteAccount(int AccountID)
        //{
        //    int rowsAffected = 0;
        //    SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
        //    string query = "DELETE Accounts WHERE AccountID = @AccountID";
        //    SqlCommand command = new SqlCommand(query, connection);
        //    command.Parameters.AddWithValue("@AccountID", AccountID);
        //    try
        //    {
        //        connection.Open();
        //        rowsAffected = command.ExecuteNonQuery();
        //    }
        //    catch (Exception){ throw; }
        //    finally { connection.Close(); }
        //    return (rowsAffected > 0);
        //}

        public static bool IsAccountExist(int AccountID)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT Found=1 FROM Accounts WHERE AccountID = @AccountID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@AccountID", AccountID);
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