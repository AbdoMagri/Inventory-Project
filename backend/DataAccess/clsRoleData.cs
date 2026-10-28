using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace InventoryDB_DataAccess
{
    public class clsRoleData
    {
        public class RoleDTO
        {
            public RoleDTO(int roleID, string name) { RoleID = roleID; Name = name; }
            public int RoleID { get; set; }
            public string Name { get; set; }
        }
        public static bool GetRoleInfoByID(int RoleID, ref string Name)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT * FROM Roles WHERE RoleID = @RoleID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@RoleID", RoleID);
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
        public static bool GetRoleInfoByName(string Name, ref int RoleID)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT * FROM Roles WHERE Name = @Name";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Name", Name);
            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    isFound = true;
                    RoleID = (int)reader["RoleID"];
                }
                reader.Close();
            }
            catch (Exception){ isFound = false; }
            finally { connection.Close(); }
            return isFound;
        }

        public static int AddNewRole(string Name)
        {
            int RoleID = -1;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"INSERT INTO Roles (Name) VALUES (@Name);
                             SELECT SCOPE_IDENTITY();";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Name", Name);
            try
            {
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int insertedID))
                    RoleID = insertedID;
            }
            catch (Exception){ throw;}
            finally { connection.Close(); }
            return RoleID;
        }

        //public static bool UpdateRole(int RoleID, string Name)
        //{
        //    int rowsAffected = 0;
        //    SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
        //    string query = "UPDATE Roles SET Name = @Name WHERE RoleID = @RoleID";
        //    SqlCommand command = new SqlCommand(query, connection);
        //    command.Parameters.AddWithValue("@RoleID", RoleID);
        //    command.Parameters.AddWithValue("@Name", Name);
        //    try
        //    {
        //        connection.Open();
        //        rowsAffected = command.ExecuteNonQuery();
        //    }
        //    catch (Exception){ return false; }
        //    finally { connection.Close(); }
        //    return (rowsAffected > 0);
        //}

        public static List<RoleDTO> GetAllRoles()
        {
            List<RoleDTO> roles = new List<RoleDTO>();

            SqlConnection connection = new SqlConnection(
                clsDataAccessSettings.ConnectionString);

            string query = "SELECT RoleID, Name FROM Roles";

            SqlCommand command = new SqlCommand(query, connection);

            try
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    roles.Add(new RoleDTO(
                        (int)reader["RoleID"],
                        reader["Name"].ToString()
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

            return roles;
        }

        //public static bool DeleteRole(int RoleID)
        //{
        //    int rowsAffected = 0;
        //    SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
        //    string query = "DELETE Roles WHERE RoleID = @RoleID";
        //    SqlCommand command = new SqlCommand(query, connection);
        //    command.Parameters.AddWithValue("@RoleID", RoleID);
        //    try
        //    {
        //        connection.Open();
        //        rowsAffected = command.ExecuteNonQuery();
        //    }
        //    catch (Exception) { throw; }
        //    finally { connection.Close(); }
        //    return (rowsAffected > 0);
        //}

        public static bool IsRoleExist(int RoleID)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT Found=1 FROM Roles WHERE RoleID = @RoleID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@RoleID", RoleID);
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