using Microsoft.Data.SqlClient;
using System;
using System.Data;
using static InventoryDB_DataAccess.clsUserData;

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
            catch (Exception){ isFound = false; }
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
            catch (Exception){ throw;}
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
            catch (Exception){ return false; }
            finally { connection.Close(); }
            return (rowsAffected > 0);
        }

        public static List<UnitDTO> GetAllUnits()
        {
            var UnitsList = new List<UnitDTO>();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT * FROM Units";
            SqlCommand command = new SqlCommand(query, connection);
            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    UnitsList.Add(new UnitDTO(
                        (int)reader["UnitID"],
                        (string)reader["Name"]
                        ));
                }
                reader.Close();
            }
            catch (Exception){ throw;}
            finally { connection.Close(); }
            return UnitsList;
        }

        //public static bool DeleteUnit(int UnitID)
        //{
        //    int rowsAffected = 0;
        //    SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
        //    string query = "DELETE Units WHERE UnitID = @UnitID";
        //    SqlCommand command = new SqlCommand(query, connection);
        //    command.Parameters.AddWithValue("@UnitID", UnitID);
        //    try
        //    {
        //        connection.Open();
        //        rowsAffected = command.ExecuteNonQuery();
        //    }
        //    catch (Exception){ throw;}
        //    finally { connection.Close(); }
        //    return (rowsAffected > 0);
        //}

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
            catch (Exception){ isFound = false; }
            finally { connection.Close(); }
            return isFound;
        }
        public static bool isNameUnitExist(string Name)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT Found=1 FROM Units WHERE Name = @Name";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Name", Name);
            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                isFound = reader.HasRows;
                reader.Close();
            }
            catch (Exception) { isFound = false; }
            finally { connection.Close(); }
            return isFound;
        }
    }
}