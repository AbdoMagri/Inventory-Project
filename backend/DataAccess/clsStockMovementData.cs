using Microsoft.Data.SqlClient;
using System;
using System.Data;
using static InventoryDB_DataAccess.clsSupplierData;
using static InventoryDB_DataAccess.clsUserData;

namespace InventoryDB_DataAccess
{
    // The stock log is append-only: no Update or Delete on purpose
    public class clsStockMovementData
    {
        public class StockMovementDTO
        {
            public StockMovementDTO(int movementID, int productID, int userID, string movementType,
                int qty, int? referenceID, string note, DateTime movementDate)
            {
                MovementID = movementID; ProductID = productID; UserID = userID; MovementType = movementType;
                Qty = qty; ReferenceID = referenceID; Note = note; MovementDate = movementDate;
            }
            public int MovementID { get; set; }
            public int ProductID { get; set; }
            public int UserID { get; set; }
            public string MovementType { get; set; }
            public int Qty { get; set; }
            public int? ReferenceID { get; set; }
            public string Note { get; set; }
            public DateTime MovementDate { get; set; }
        }

        public class clsStockMovement
        {
            public int MovementID { get; set; }
            public DateTime MovementDate { get; set; }
            public string ProductName { get; set; }
            public string SKU { get; set; }
            public string MovementType { get; set; }
            public int Qty { get; set; }
            public int? ReferenceID { get; set; }
            public string Note { get; set; }
            public string UserName { get; set; }
        }
        public static bool GetMovementInfoByID(int MovementID, ref int ProductID, ref int UserID,
            ref string MovementType, ref int Qty, ref int? ReferenceID, ref string Note, ref DateTime MovementDate)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT * FROM StockMovements WHERE MovementID = @MovementID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@MovementID", MovementID);
            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    isFound = true;
                    ProductID = (int)reader["ProductID"];
                    UserID = (int)reader["UserID"];
                    MovementType = (string)reader["MovementType"];
                    Qty = (int)reader["Qty"];
                    ReferenceID = reader["ReferenceID"] != DBNull.Value ? (int?)reader["ReferenceID"] : null;
                    Note = reader["Note"] != DBNull.Value ? (string)reader["Note"] : "";
                    MovementDate = (DateTime)reader["MovementDate"];
                }
                reader.Close();
            }
            catch (Exception){ isFound = false; }
            finally { connection.Close(); }
            return isFound;
        }

        // Qty is signed: positive = stock in, negative = stock out
        //public static int AddNewMovement(int ProductID, int UserID, string MovementType,
        //    int Qty, int? ReferenceID, string Note)
        //{
        //    int MovementID = -1;
        //    SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
        //    string query = @"INSERT INTO StockMovements (ProductID, UserID, MovementType, Qty, ReferenceID, Note)
        //                     VALUES (@ProductID, @UserID, @MovementType, @Qty, @ReferenceID, @Note);
        //                     SELECT SCOPE_IDENTITY();";
        //    SqlCommand command = new SqlCommand(query, connection);
        //    command.Parameters.AddWithValue("@ProductID", ProductID);
        //    command.Parameters.AddWithValue("@UserID", UserID);
        //    command.Parameters.AddWithValue("@MovementType", MovementType);
        //    command.Parameters.AddWithValue("@Qty", Qty);
        //    command.Parameters.AddWithValue("@ReferenceID", (object)ReferenceID ?? DBNull.Value);
        //    if (!string.IsNullOrEmpty(Note))
        //        command.Parameters.AddWithValue("@Note", Note);
        //    else
        //        command.Parameters.AddWithValue("@Note", DBNull.Value);
        //    try
        //    {
        //        connection.Open();
        //        object result = command.ExecuteScalar();
        //        if (result != null && int.TryParse(result.ToString(), out int insertedID))
        //            MovementID = insertedID;
        //    }
        //    catch (Exception){ throw;}
        //    finally { connection.Close(); }
        //    return MovementID;
        //}

        public static List<StockMovementDTO> GetMovementsByProductID(int ProductID)
        {
            var StockMovementsList = new List<StockMovementDTO>();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"SELECT m.MovementID, m.MovementDate, m.MovementType, m.Qty,
                                    m.ReferenceID, m.Note, UserName = u.UserName , m.UserID
                             FROM StockMovements m
                             INNER JOIN Users u ON m.UserID = u.UserID
                             WHERE m.ProductID = @ProductID
                             ORDER BY m.MovementDate DESC";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ProductID", ProductID);
            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    StockMovementsList.Add(new StockMovementDTO(
                        (int)reader["MovementID"],
                        ProductID, (int)reader["UserID"],
                        (string)reader["MovementType"],
                        (int)reader["Qty"],
                        reader["ReferenceID"] == DBNull.Value ? null :
                        (int)reader["ReferenceID"],
                        reader["Note"] == DBNull.Value
    ? ""
    : reader["Note"].ToString() ?? "",
                        (DateTime)reader["MovementDate"]
                        ));
                }
                reader.Close();
            }
            catch (Exception){ throw;}
            finally { connection.Close(); }
            return StockMovementsList;
        }

        public static List<clsStockMovement> GetAllMovements(int AccountID)
        {
            List<clsStockMovement> movements = new List<clsStockMovement>();

            SqlConnection connection = new SqlConnection(
                clsDataAccessSettings.ConnectionString);

            string query = @"SELECT m.MovementID, m.MovementDate,
                            ProductName = p.Name, p.SKU,
                            m.MovementType, m.Qty, m.ReferenceID,
                            m.Note, UserName = u.UserName
                     FROM StockMovements m
                     INNER JOIN Products p
                        ON m.ProductID = p.ProductID
                     INNER JOIN Users u
                        ON m.UserID = u.UserID
                     WHERE p.AccountID = @AccountID
                     ORDER BY m.MovementDate DESC";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@AccountID", AccountID);

            try
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    movements.Add(new clsStockMovement
                    {
                        MovementID = (int)reader["MovementID"],
                        MovementDate = (DateTime)reader["MovementDate"],
                        ProductName = reader["ProductName"].ToString(),
                        SKU = reader["SKU"].ToString(),
                        MovementType = reader["MovementType"].ToString(),
                        Qty = (int)reader["Qty"],
                        ReferenceID = reader["ReferenceID"] == DBNull.Value
                            ? null
                            : (int?)reader["ReferenceID"],
                        Note = reader["Note"] == DBNull.Value
                            ? null
                            : reader["Note"].ToString(),
                        UserName = reader["UserName"].ToString()
                    });
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                // Handle or log the exception.
                throw;
            }
            finally
            {
                connection.Close();
            }

            return movements;
        }
        public static int AdjustStock(int AccountID, int UserID, int ProductID,
    int QtyChange, string Note, ref string ErrorMessage)
        {
            int MovementID = -1;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            SqlCommand command = new SqlCommand("sp_AdjustStock", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@AccountID", AccountID);
            command.Parameters.AddWithValue("@UserID", UserID);
            command.Parameters.AddWithValue("@ProductID", ProductID);
            command.Parameters.AddWithValue("@QtyChange", QtyChange);
            command.Parameters.AddWithValue("@Note", (object)Note ?? DBNull.Value);

            SqlParameter movementIDParam = new SqlParameter("@MovementID", SqlDbType.Int);
            movementIDParam.Direction = ParameterDirection.Output;
            command.Parameters.Add(movementIDParam);

            try
            {
                connection.Open();
                command.ExecuteNonQuery();
                MovementID = (int)movementIDParam.Value;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            finally { connection.Close(); }

            return MovementID;
        }
    }
}