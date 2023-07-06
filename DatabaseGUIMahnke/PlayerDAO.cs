using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DatabaseGUIMahnke
{
    internal class PlayerDAO
    {
        private string sortedColumn = "";
        private string sortedOrder = "ASC";
        public List<Player> getAllPlayers()
        {
            List<Player> players = new List<Player>();

            using (SqlConnection connection = DatabaseManager.getConnection())
            {
                string query = "SELECT * FROM Player";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();

                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            Player player = new Player();
                            player.IDNum = reader.GetInt32(reader.GetOrdinal("IDNum"));
                            player.teamName = reader.GetString(reader.GetOrdinal("teamName"));
                            player.lastName = reader.GetString(reader.GetOrdinal("lastName"));
                            player.firstName = reader.GetString(reader.GetOrdinal("firstName"));
                            player.phoneNum = reader.IsDBNull(reader.GetOrdinal("phoneNum")) ? string.Empty : reader.GetString(reader.GetOrdinal("phoneNum"));

                            players.Add(player);
                        }

                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }
                }
            }

            return players;
        }

        public void AddPlayer(int idNum, string teamName, string lastName, string firstName, string phoneNum)
        {
            using (SqlConnection connection = DatabaseManager.getConnection())
            {
                string query = "INSERT INTO Player (IDNum, teamName, lastName, firstName, phoneNum) " +
                               "VALUES (@IDNum, @TeamName, @LastName, @FirstName, @PhoneNum)";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@IDNum", idNum);
                command.Parameters.AddWithValue("@TeamName", teamName);
                command.Parameters.AddWithValue("@LastName", lastName);
                command.Parameters.AddWithValue("@FirstName", firstName);
                command.Parameters.AddWithValue("@PhoneNum", phoneNum);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void DeletePlayer(string IDNum)
        {
            using (SqlConnection connection = DatabaseManager.getConnection())
            {
                connection.Open();
                try
                {
                    string query = "DELETE FROM Player WHERE IDNum = @IDNum";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@IDNum", IDNum);
                        command.ExecuteNonQuery();
                    }
                } catch (Exception ex)
                {
                    MessageBox.Show("Error adding entry: " + ex.Message + "\n" + ex.ToString());
                }
            }
        }

        public List<Player> sortBy(string columnName)
        {

            if (columnName.Equals(sortedColumn))
            {
                sortedOrder = sortedOrder.Equals("ASC") ? "DESC" : "ASC";
            }
            else
            {
                sortedColumn = columnName;
                sortedOrder = "ASC";
            }

            string sql = $"SELECT * FROM Player ORDER BY {columnName} {sortedOrder}";

            List<Player> sortedResult = new List<Player>();

            // Execute the SQL statement and retrieve the sorted result
            using (SqlConnection connection = DatabaseManager.getConnection())
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Player player = new Player();
                            player.IDNum = reader.GetInt32(reader.GetOrdinal("IDNum"));
                            player.teamName = reader.GetString(reader.GetOrdinal("teamName"));
                            player.lastName = reader.GetString(reader.GetOrdinal("lastName"));
                            player.firstName = reader.GetString(reader.GetOrdinal("firstName"));
                            player.phoneNum = reader.IsDBNull(reader.GetOrdinal("phoneNum")) ? string.Empty : reader.GetString(reader.GetOrdinal("phoneNum"));

                            sortedResult.Add(player);
                        }
                    }
                }
            }

            return sortedResult;
        }
        public string getSortedOrder() { return sortedOrder; }
    }
}
