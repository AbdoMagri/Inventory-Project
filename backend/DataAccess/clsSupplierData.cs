using Microsoft.Data.SqlClient;
using System;
using System.Data;
using static InventoryDB_DataAccess.clsUserData;

namespace InventoryDB_DataAccess
{
    public class clsSupplierData
    {
        public class SupplierDTO
        {
            public SupplierDTO(int supplierID, int accountID, string name, string phone)
            {
                SupplierID = supplierID; AccountID = accountID; Name = name; Phone = phone;
            }
            public int SupplierID { get; set; }
            public int AccountID { get; set; }
            public string Name { get; set; }
            public string Phone { get; set; }
        }
        public static bool GetSupplierInfoByID(int SupplierID, ref int AccountID, ref string Name, ref string Phone)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT * FROM Suppliers WHERE SupplierID = @SupplierID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@SupplierID", SupplierID);
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
            catch (Exception){ isFound = false; }
            finally { connection.Close(); }
            return isFound;
        }

        public static int AddNewSupplier(int AccountID, string Name, string Phone)
        {
            int SupplierID = -1;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"INSERT INTO Suppliers (AccountID, Name, Phone) VALUES (@AccountID, @Name, @Phone);
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
                    SupplierID = insertedID;
            }
            catch (Exception){ throw;}
            finally { connection.Close(); }
            return SupplierID;
        }

        public static bool UpdateSupplier(int SupplierID, string Name, string Phone)
        {
            int rowsAffected = 0;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "UPDATE Suppliers SET Name = @Name, Phone = @Phone WHERE SupplierID = @SupplierID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@SupplierID", SupplierID);
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
            catch (Exception){ return false; }
            finally { connection.Close(); }
            return (rowsAffected > 0);
        }

        public static List<SupplierDTO> GetAllSuppliers(int AccountID)
        {
            var SuppliersList = new List<SupplierDTO>();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT SupplierID, Name, Phone FROM Suppliers WHERE AccountID = @AccountID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@AccountID", AccountID);
            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    SuppliersList.Add(new SupplierDTO(
                        (int)reader["SupplierID"],
                        AccountID,
                        (string)reader["Name"],
                        (string)reader["Phone"]
                        ));
                }
                reader.Close();
            }
            catch (Exception){ throw;}
            finally { connection.Close(); }
            return SuppliersList;
        }

        public static bool DeleteSupplier(int AccountID, int SupplierID)
        {
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            SqlCommand command = new SqlCommand("sp_DeleteSupplier", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@AccountID", AccountID);
            command.Parameters.AddWithValue("@SupplierID", SupplierID);

            try
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception) { throw; }
            finally { connection.Close(); }

            return true;   // reaching here means the procedure didn't throw
        }

        public static bool IsSupplierExist(int SupplierID)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT Found=1 FROM Suppliers WHERE SupplierID = @SupplierID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@SupplierID", SupplierID);
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