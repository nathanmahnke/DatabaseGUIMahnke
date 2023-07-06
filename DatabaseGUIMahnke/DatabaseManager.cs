using System.Data.SqlClient;

namespace DatabaseGUIMahnke
{
    internal class DatabaseManager
    {
        // private static string connectionString = "Server=localhost;Database=mahnke_nathan_db;Integrated Security=True";
        private static string connectionString = "";
        public static void setConnectionString(string newConnectionString)
        {
            connectionString = newConnectionString;
        }
        public static SqlConnection getConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
