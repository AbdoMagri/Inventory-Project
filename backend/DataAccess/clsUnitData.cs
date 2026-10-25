using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace InventoryDB_DataAccess
{
    public class clsUnitData
    {
        public class UnitDTO
        {
            public UnitDTO(int unitID, string name) { UnitID = unitID; Name = name; }
            public int UnitID { get; set; }
            public string Name { get; set; }
        }
        public static bool GetUnitInfoByID(int UnitID, ref string Name)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT * FROM Units WHERE UnitID = @UnitID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UnitID", UnitID);
            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    isFound = true;
                    Name = (string)reader["Name"];
                }
                reader.Close();
            }
            catch (Exception ex) { isFound = false; }
            finally { connection.Close(); }
            return isFound;
        }

        public static int AddNewUnit(string Name)
        {
            int UnitID = -1;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"INSERT INTO Units (Name) VALUES (@Name);
                             SELECT SCOPE_IDENTITY();";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Name", Name);
            try
            {
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int insertedID))
                    UnitID = insertedID;
            }
            catch (Exception ex) { }
            finally { connection.Close(); }
            return UnitID;
        }

        public static bool UpdateUnit(int UnitID, string Name)
        {
            int rowsAffected = 0;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "UPDATE Units SET Name = @Name WHERE UnitID = @UnitID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UnitID", UnitID);
            command.Parameters.AddWithValue("@Name", Name);
            try
            {
                connection.Open();
                rowsAffected = command.ExecuteNonQuery();
            }
            catch (Exception ex) { return false; }
            finally { connection.Close(); }
            return (rowsAffected > 0);
        }

        public static DataTable GetAllUnits()
        {
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT * FROM Units";
            SqlCommand command = new SqlCommand(query, connection);
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

        public static bool DeleteUnit(int UnitID)
        {
            int rowsAffected = 0;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "DELETE Units WHERE UnitID = @UnitID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UnitID", UnitID);
            try
            {
                connection.Open();
                rowsAffected = command.ExecuteNonQuery();
            }
            catch (Exception ex) { }
            finally { connection.Close(); }
            return (rowsAffected > 0);
        }

        public static bool IsUnitExist(int UnitID)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT Found=1 FROM Units WHERE UnitID = @UnitID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UnitID", UnitID);
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