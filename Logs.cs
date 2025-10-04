using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bas_DATSYS_IT505
{
    public partial class Logs : Form
    {
        public Logs()
        {
            InitializeComponent();
            dgvLogs.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            LoadLogs();
        }
        private void LoadLogs()
        {

            string sqlQuery = "SELECT LogID, Name, Action, Description, Date, " +
                               "CONVERT(VARCHAR(8), Time, 100) AS Time " +
                               "FROM Logs " +
                               "ORDER BY " +
                               "LogID DESC";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlQuery, conn);
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    dgvLogs.DataSource = dataTable;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while loading logs: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {

            string searchTerm = txtSearch.Text.Trim();

            if (string.IsNullOrEmpty(searchTerm))
            {
                LoadLogs();
                return;
            }

            string sqlQuery = "SELECT LogID, Name, Action, Description, Date, " +
                              "CONVERT(VARCHAR(8), Time, 100) AS Time " +
                              "FROM Logs ";

            if (int.TryParse(searchTerm, out int numericSearchTerm))
            {
                sqlQuery += "WHERE LogID = @searchTerm";
            }
            else if (DateTime.TryParse(searchTerm, out DateTime dateValue))
            {
                sqlQuery += "WHERE Date = @searchTerm";
            }
            else
            {
                sqlQuery += "WHERE Name LIKE @searchTerm OR Action LIKE @searchTerm OR Description LIKE @searchTerm ";
            }

            sqlQuery += " ORDER BY LogID DESC";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlQuery, conn);

                    if (int.TryParse(searchTerm, out int numericValue))
                    {
                        dataAdapter.SelectCommand.Parameters.AddWithValue("@searchTerm", numericValue);
                    }
                    else if (DateTime.TryParse(searchTerm, out DateTime date))
                    {
                        dataAdapter.SelectCommand.Parameters.AddWithValue("@searchTerm", date.Date);
                    }
                    else
                    {
                        dataAdapter.SelectCommand.Parameters.AddWithValue("@searchTerm", "%" + searchTerm + "%");
                    }

                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);
                    dgvLogs.DataSource = dataTable;

                    if (dataTable.Rows.Count == 0)
                    {
                        MessageBox.Show("No logs found matching your search criteria.", "No Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred during search: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        string connectionString = Database.ConnectionString;

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            Dashboard adminDashboard = new Dashboard();
            adminDashboard.Show();
            this.Hide();
        }

        private void btnStudents_Click(object sender, EventArgs e)
        {
            Student adminStudents = new Student();
            adminStudents.Show();
            this.Hide();
        }

        private void btnTeachers_Click(object sender, EventArgs e)
        {
            Teachers adminTeachers = new Teachers();
            adminTeachers.Show();
            this.Hide();
        }

        private void btnSubjects_Click(object sender, EventArgs e)
        {
            Subjects adminSubjects = new Subjects();
            adminSubjects.Show();
            this.Hide();
        }

        private void btnReports_Click(object sender, EventArgs e)
        {
            Logs adminLogs = new Logs();
            adminLogs.Show();
            this.Hide();
        }

        private void btnLogs_Click(object sender, EventArgs e)
        {
            this.Show();
        }

        private void btnApproval_Click(object sender, EventArgs e)
        {
            Approval approval = new Approval();
            approval.Show();
            this.Hide();
        }

        private void btnSearch_Click_1(object sender, EventArgs e)
        {
            string searchTerm = txtSearch.Text.Trim();

            if (string.IsNullOrEmpty(searchTerm))
            {
                LoadLogs();
                return;
            }

            string sqlQuery = "SELECT LogID, Name, Action, Description, Date, " +
                              "CONVERT(VARCHAR(8), Time, 100) AS Time " +
                              "FROM Logs ";

            if (int.TryParse(searchTerm, out int numericSearchTerm))
            {
                sqlQuery += "WHERE LogID = @searchTerm";
            }
            else if (DateTime.TryParse(searchTerm, out DateTime dateValue))
            {
                sqlQuery += "WHERE Date = @searchTerm";
            }
            else
            {
                sqlQuery += "WHERE Name LIKE @searchTerm OR Action LIKE @searchTerm OR Description LIKE @searchTerm ";
            }

            sqlQuery += " ORDER BY LogID DESC";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlQuery, conn);

                    if (int.TryParse(searchTerm, out int numericValue))
                    {
                        dataAdapter.SelectCommand.Parameters.AddWithValue("@searchTerm", numericValue);
                    }
                    else if (DateTime.TryParse(searchTerm, out DateTime date))
                    {
                        dataAdapter.SelectCommand.Parameters.AddWithValue("@searchTerm", date.Date);
                    }
                    else
                    {
                        dataAdapter.SelectCommand.Parameters.AddWithValue("@searchTerm", "%" + searchTerm + "%");
                    }

                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);
                    dgvLogs.DataSource = dataTable;

                    if (dataTable.Rows.Count == 0)
                    {
                        MessageBox.Show("No logs found matching your search criteria.", "No Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred during search: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Hide();
        }
    }
}
