using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DatabaseGUIMahnke
{
    internal class TeamDAO
    {
        private string sortedColumn = "";
        private string sortedOrder = "ASC";
        public List<Team> getAllTeams()
        {
            List<Team> teams = new List<Team>();

            using (SqlConnection connection = DatabaseManager.getConnection())
            {
                string query = "SELECT * FROM Team";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();

                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            Team team = new Team();
                            team.name = reader["name"].ToString();
                            team.homeFieldName = reader["homeFieldName"].ToString();
                            team.division = reader.IsDBNull(reader.GetOrdinal("division")) ? 0 : reader.GetInt32(reader.GetOrdinal("division"));
                            team.winsThisSeason = reader.IsDBNull(reader.GetOrdinal("winsThisSeason")) ? 0 : reader.GetInt32(reader.GetOrdinal("winsThisSeason"));
                            team.lossesThisSeason = reader.IsDBNull(reader.GetOrdinal("lossesThisSeason")) ? 0 : reader.GetInt32(reader.GetOrdinal("lossesThisSeason"));
                            team.numPlayers = reader.IsDBNull(reader.GetOrdinal("numPlayers")) ? 0 : reader.GetInt32(reader.GetOrdinal("numPlayers"));
                            team.numRosterSpots = reader.IsDBNull(reader.GetOrdinal("numRosterSpots")) ? 0 : reader.GetInt32(reader.GetOrdinal("numRosterSpots"));

                            teams.Add(team);
                        }

                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }
                }
            }

            return teams;
        }

        public void AddTeam(string name, string homeFieldName, int division, int winsThisSeason, int lossesThisSeason, int numPlayers, int numRosterSpots)
        {
            using (SqlConnection connection = DatabaseManager.getConnection())
            {
                string query = "INSERT INTO Team (name, homeFieldName, division, winsThisSeason, lossesThisSeason, numPlayers, numRosterSpots) " +
                               "VALUES (@Name, @HomeFieldName, @Division, @WinsThisSeason, @LossesThisSeason, @NumPlayers, @NumRosterSpots)";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@HomeFieldName", homeFieldName);
                command.Parameters.AddWithValue("@Division", division);
                command.Parameters.AddWithValue("@WinsThisSeason", winsThisSeason);
                command.Parameters.AddWithValue("@LossesThisSeason", lossesThisSeason);
                command.Parameters.AddWithValue("@NumPlayers", numPlayers);
                command.Parameters.AddWithValue("@NumRosterSpots", numRosterSpots);

                connection.Open();
                try {
                    command.ExecuteNonQuery();
                } catch (Exception ex)
                {
                    MessageBox.Show("Error adding entry: " + ex.Message + "\n" + ex.ToString());
                }
            }
        }

        public void DeleteTeam(string teamName)
        {
            using (SqlConnection connection = DatabaseManager.getConnection())
            {
                try
                {
                    SqlCommand command = new SqlCommand("DeleteTeamWithGames", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@teamName", teamName);

                    connection.Open();
                    command.ExecuteNonQuery();
                    
                } catch (Exception ex)
                {
                    MessageBox.Show("Error adding entry: " + ex.Message + "\n" + ex.ToString());
                }
            }
        }

        public List<Team> sortBy(string columnName)
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

            string sql = $"SELECT * FROM Team ORDER BY {columnName} {sortedOrder}";

            List<Team> sortedResult = new List<Team>();

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
                            Team team = new Team();
                            team.name = reader["name"].ToString();
                            team.homeFieldName = reader["homeFieldName"].ToString();
                            team.division = reader.IsDBNull(reader.GetOrdinal("division")) ? 0 : reader.GetInt32(reader.GetOrdinal("division"));
                            team.winsThisSeason = reader.IsDBNull(reader.GetOrdinal("winsThisSeason")) ? 0 : reader.GetInt32(reader.GetOrdinal("winsThisSeason"));
                            team.lossesThisSeason = reader.IsDBNull(reader.GetOrdinal("lossesThisSeason")) ? 0 : reader.GetInt32(reader.GetOrdinal("lossesThisSeason"));
                            team.numPlayers = reader.IsDBNull(reader.GetOrdinal("numPlayers")) ? 0 : reader.GetInt32(reader.GetOrdinal("numPlayers"));
                            team.numRosterSpots = reader.IsDBNull(reader.GetOrdinal("numRosterSpots")) ? 0 : reader.GetInt32(reader.GetOrdinal("numRosterSpots"));

                            sortedResult.Add(team);
                        }
                    }
                }
            }
            return sortedResult;
        }
        public string getSortedOrder() { return sortedOrder; }
    }
}
