using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Windows.Forms;

namespace DatabaseGUIMahnke
{
    public partial class Form1 : Form
    {
        BindingSource dataBindingSource = new BindingSource();
        TeamDAO teamDAO = new TeamDAO();
        PlayerDAO playerDAO = new PlayerDAO();
        private bool isTeamsButtonClicked = false;
        private bool isPlayersButtonClicked = false;
        private Dictionary<string, int> tableFieldCounts = new Dictionary<string, int>()
        {
            { "Teams", 7 },
            { "Players", 5 }
        };
        private Dictionary<string, string> fieldValues = new Dictionary<string, string>();


        public Form1()
        {
            string connectionString = PromptForConnectionString();
            DatabaseManager.setConnectionString(connectionString);
            InitializeComponent();
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.ColumnHeaderMouseClick += dataGridView1_ColumnHeaderMouseClick;
        }

        private string PromptForConnectionString()
        {
            string connectionString = string.Empty;
            DialogResult result = MessageBox.Show("Please enter the database connection string (You must be using SQL Server): \nEx: Server=localhost;Database=mahnke_nathan_db;Integrated Security=True", "Connection String", MessageBoxButtons.OKCancel);

            if (result == DialogResult.OK)
            {
                using (InputDialog dialog = new InputDialog())
                {
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        connectionString = dialog.ConnectionString;
                    }
                }
            }

            return connectionString;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TeamDAO teamDAO = new TeamDAO();

            try
            {
                List<Team> teams = teamDAO.getAllTeams();
                dataBindingSource.DataSource = teams;
                dataGridView1.DataSource = dataBindingSource;
                dataGridView1.Refresh();
                isTeamsButtonClicked = true;
                isPlayersButtonClicked = false;
                ShowEntryFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

            try
            {
                List<Player> players = playerDAO.getAllPlayers();
                dataBindingSource.DataSource = players;
                dataGridView1.DataSource = dataBindingSource;
                dataGridView1.Refresh();
                isTeamsButtonClicked = false;
                isPlayersButtonClicked = true;
                ShowEntryFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void ShowEntryFields()
        {
            int fieldCount = isTeamsButtonClicked ? tableFieldCounts["Teams"] : tableFieldCounts["Players"];

            // Clear the TableLayoutPanel
            tableLayoutPanel1.Controls.Clear();
            tableLayoutPanel1.RowStyles.Clear();
            tableLayoutPanel1.ColumnStyles.Clear();

            // Set the number of columns and rows based on the field count
            int columnCount = fieldCount;
            int rowCount = 2;
            tableLayoutPanel1.ColumnCount = columnCount;
            tableLayoutPanel1.RowCount = rowCount;

            // Adjust the column styles to distribute the width evenly
            for (int i = 0; i < columnCount; i++)
            {
                tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / columnCount));
            }

            // Dynamically create TextBox controls and labels
            for (int i = 0; i < fieldCount; i++)
            {
                TextBox textBox = new TextBox();
                textBox.ReadOnly = false;
                textBox.TextChanged += textBox_TextChanged;

                Label label = new Label();
                label.AutoSize = true;
                label.TextAlign = ContentAlignment.MiddleLeft;
                label.Text = GetFieldName(i);

                // Add the TextBox and label to the TableLayoutPanel
                tableLayoutPanel1.Controls.Add(label, i, 0);
                tableLayoutPanel1.Controls.Add(textBox, i, 1);
            }

            groupBox1.Visible = true;
            button3.Enabled = false; // Disable the add entry button
            button3.Visible = true;
            if (button3.Visible == false)
            {
                button3.Visible = true;
            }
        }

        private string GetFieldName(int index)
        {
            if (isTeamsButtonClicked)
            {
                Type teamType = typeof(Team);
                var properties = teamType.GetProperties();

                if (index >= 0 && index < properties.Length)
                {
                    var property = properties[index];
                    return property.Name;
                }
            }
            else if (isPlayersButtonClicked)
            {
                Type playerType = typeof(Player);
                var properties = playerType.GetProperties();

                if (index >= 0 && index < properties.Length)
                {
                    var property = properties[index];
                    return property.Name;
                }
            }

            return string.Empty;
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            int col = tableLayoutPanel1.GetColumn(textBox);
            Label label = (Label)tableLayoutPanel1.GetControlFromPosition(col, 0);
            string fieldName = label.Text;
            string value = textBox.Text;

            if (fieldValues.ContainsKey(fieldName))
            {
                fieldValues[fieldName] = value;
            }
            else
            {
                fieldValues.Add(fieldName, value);
            }
            ValidateFieldsAndEnableButton();
        }

        private void ValidateFieldsAndEnableButton()
        {
            bool allFieldsFilled = true;
            foreach (Control control in tableLayoutPanel1.Controls)
            {
                if (control is TextBox textBox && string.IsNullOrEmpty(textBox.Text))
                {
                    allFieldsFilled = false;
                    break;
                }
            }

            button3.Enabled = allFieldsFilled; // Enable or disable the add entry button
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (isTeamsButtonClicked)
                {
                    // Retrieve field values from the dictionary
                    string name = fieldValues["name"];
                    string homeFieldName = fieldValues["homeFieldName"];
                    int division = int.Parse(fieldValues["division"]);
                    int winsThisSeason = int.Parse(fieldValues["winsThisSeason"]);
                    int lossesThisSeason = int.Parse(fieldValues["lossesThisSeason"]);
                    int numPlayers = int.Parse(fieldValues["numPlayers"]);
                    int numRosterSpots = int.Parse(fieldValues["numRosterSpots"]);

                    teamDAO.AddTeam(name, homeFieldName, division, winsThisSeason, lossesThisSeason, numPlayers, numRosterSpots);

                    // Reset fieldValues dictionary
                    fieldValues.Clear();
                }
                else if (isPlayersButtonClicked)
                {
                    // Retrieve field values from the dictionary
                    int IDNum = int.Parse(fieldValues["IDNum"]);
                    string teamName = fieldValues["teamName"];
                    string lastName = fieldValues["lastName"];
                    string firstName = fieldValues["firstName"];
                    string phoneNum = fieldValues["phoneNum"];

                    playerDAO.AddPlayer(IDNum, teamName, lastName, firstName, phoneNum);

                    // Reset fieldValues dictionary
                    fieldValues.Clear();
                }

                // Disable the "Add Entry" button
                button3.Enabled = false;
                RefreshDataSource();

                MessageBox.Show("Entry added successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding entry: " + ex.Message + "\n" + ex.ToString());
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DialogResult result = MessageBox.Show("Are you sure you want to delete the selected row? This action will delete all related entries.", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    // Get the selected row
                    DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                    if (isTeamsButtonClicked)
                    {
                        string teamName = selectedRow.Cells["name"].Value.ToString();
                        teamDAO.DeleteTeam(teamName);
                    }
                    else if (isPlayersButtonClicked)
                    {
                        string playerIDNum = selectedRow.Cells["IDNum"].Value.ToString();
                        playerDAO.DeletePlayer(playerIDNum);
                    }

                    // Refresh the data grid view to reflect the changes
                    RefreshDataSource();
                }
            }
        }

        private void RefreshDataSource()
        {
            if (isTeamsButtonClicked)
            {
                List<Team> teams = teamDAO.getAllTeams();
                dataBindingSource.DataSource = teams;
            }
            else if (isPlayersButtonClicked)
            {
                List<Player> players = playerDAO.getAllPlayers();
                dataBindingSource.DataSource = players;
            }

            dataBindingSource.ResetBindings(false);
            dataGridView1.Update();
        }

        private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridViewColumn clickedColumn = dataGridView1.Columns[e.ColumnIndex];

            // Get the column name
            string columnName = clickedColumn.Name;
            string sortedOrder = "";
            if (isPlayersButtonClicked)
            {
                List<Player> sortedPlayers = playerDAO.sortBy(columnName);

                // Update the data source and refresh the DataGridView
                dataGridView1.DataSource = sortedPlayers;
                dataGridView1.Refresh();
                sortedOrder = playerDAO.getSortedOrder();
            }
            else if (isTeamsButtonClicked)
            {
                List<Team> sortedTeams = teamDAO.sortBy(columnName);

                // Update the data source and refresh the DataGridView
                dataGridView1.DataSource = sortedTeams;
                dataGridView1.Refresh();
                sortedOrder = teamDAO.getSortedOrder();
            }
            // Handle adding/changing the carat which indicates the sorting order
            clickedColumn = dataGridView1.Columns[e.ColumnIndex];
            if (sortedOrder.Equals("ASC"))
            {
                clickedColumn.HeaderCell.SortGlyphDirection = System.Windows.Forms.SortOrder.Ascending;
            } else
            {
                clickedColumn.HeaderCell.SortGlyphDirection = System.Windows.Forms.SortOrder.Descending;
            }

        }
    }
}
