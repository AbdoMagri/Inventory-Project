using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace InventoryDB_DataAccess
{
    public class clsCustomerData
    {
        public static bool GetCustomerInfoByID(int CustomerID, ref int AccountID, ref string Name, ref string Phone)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT * FROM Customers WHERE CustomerID = @CustomerID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@CustomerID", CustomerID);
            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    isFound = true;
                    AccountID = (int)reader["AccountID"];
                    Name = (string)reader["Name"];
                    Phone = reader["Phone"] != DBNull.Value ? (string)reader["Phone"] : "";
                }
                reader.Close();
            }
            catch (Exception ex) { isFound = false; }
            finally { connection.Close(); }
            return isFound;
        }

        public static int AddNewCustomer(int AccountID, string Name, string Phone)
        {
            int CustomerID = -1;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"INSERT INTO Customers (AccountID, Name, Phone) VALUES (@AccountID, @Name, @Phone);
                             SELECT SCOPE_IDENTITY();";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@AccountID", AccountID);
            command.Parameters.AddWithValue("@Name", Name);
            if (!string.IsNullOrEmpty(Phone))
                command.Parameters.AddWithValue("@Phone", Phone);
            else
                command.Parameters.AddWithValue("@Phone", DBNull.Value);
            try
            {
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int insertedID))
                    CustomerID = insertedID;
            }
            catch (Exception ex) { }
            finally { connection.Close(); }
            return CustomerID;
        }

        public static bool UpdateCustomer(int CustomerID, string Name, string Phone)
        {
            int rowsAffected = 0;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "UPDATE Customers SET Name = @Name, Phone = @Phone WHERE CustomerID = @CustomerID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@CustomerID", CustomerID);
            command.Parameters.AddWithValue("@Name", Name);
            if (!string.IsNullOrEmpty(Phone))
                command.Parameters.AddWithValue("@Phone", Phone);
            else
                command.Parameters.AddWithValue("@Phone", DBNull.Value);
            try
            {
                connection.Open();
                rowsAffected = command.ExecuteNonQuery();
            }
            catch (Exception ex) { return false; }
            finally { connection.Close(); }
            return (rowsAffected > 0);
        }

        public static DataTable GetAllCustomers(int AccountID)
        {
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT CustomerID, Name, Phone FROM Customers WHERE AccountID = @AccountID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@AccountID", AccountID);
            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows) { dt.Load(reader); }
                reader.Close();
            }
            catch (Exception ex) { }
            finally { connection.Close(); }
            return dt;
        }

        public static bool DeleteCustomer(int CustomerID)
        {
            int rowsAffected = 0;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "DELETE Customers WHERE CustomerID = @CustomerID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@CustomerID", CustomerID);
            try
            {
                connection.Open();
                rowsAffected = command.ExecuteNonQuery();
            }
            catch (Exception ex) { }
            finally { connection.Close(); }
            return (rowsAffected > 0);
        }

        public static bool IsCustomerExist(int CustomerID)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT Found=1 FROM Customers WHERE CustomerID = @CustomerID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@CustomerID", CustomerID);
            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                isFound = reader.HasRows;
                reader.Close();
            }
            catch (Exception ex) { isFound = false; }
            finally { connection.Close(); }
            return isFound;
        }
    }
}