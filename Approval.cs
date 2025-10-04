﻿using System;
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
    public partial class Approval : Form
    {
        public Approval()
        {
            InitializeComponent();
            dgvPending.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            LoadData();
        }

        string connectionString = Database.ConnectionString;

        private void LoadData()
        {
            string sqlQuery_TotalCount = "SELECT COUNT(p.ProfileID) " +
                                 "FROM Profiles AS p " +
                                 "INNER JOIN Users AS u ON p.ProfileID = u.ProfileID " +
                                 "INNER JOIN Roles AS r ON u.RoleID = r.RoleID " +
                                 "WHERE r.RoleName = 'Student' AND p.Status = 'Pending'";

            string sqlQuery_LoadData = "SELECT p.ProfileID, p.FirstName, p.LastName, p.Age, p.Gender, p.Phone, p.Address, p.Email, ISNULL(p.Status, 'Unknown') AS Status " +
                                       "FROM Profiles AS p " +
                                       "INNER JOIN Users AS u ON p.ProfileID = u.ProfileID " +
                                       "INNER JOIN Roles AS r ON u.RoleID = r.RoleID " +
                                       "WHERE r.RoleName IN ('Student', 'Instructor') AND p.Status = 'Pending' " +
                                       "ORDER BY p.ProfileID DESC";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    SqlCommand countCmd = new SqlCommand(sqlQuery_TotalCount, conn);
                    int pendingStudentCount = (int)countCmd.ExecuteScalar();
                    lblNumOfPending.Text = pendingStudentCount.ToString();

                    SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlQuery_LoadData, conn);
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    dgvPending.AutoGenerateColumns = false;
                    dgvPending.Columns.Clear();
                    dgvPending.ReadOnly = true;

                    dgvPending.Columns.Add("ProfileID", "Profile ID");
                    dgvPending.Columns.Add("FirstName", "First Name");
                    dgvPending.Columns.Add("LastName", "Last Name");
                    dgvPending.Columns.Add("Age", "Age");
                    dgvPending.Columns.Add("Gender", "Gender");
                    dgvPending.Columns.Add("Phone", "Phone");
                    dgvPending.Columns.Add("Address", "Address");
                    dgvPending.Columns.Add("Email", "Email");
                    dgvPending.Columns.Add("Status", "Status");

                    DataGridViewButtonColumn btnColumn = new DataGridViewButtonColumn();
                    btnColumn.Name = "StatusActionButton";
                    btnColumn.HeaderText = "Change Status";
                    btnColumn.Text = "Approve";
                    btnColumn.UseColumnTextForButtonValue = true;
                    dgvPending.Columns.Insert(0, btnColumn);

                    foreach (DataGridViewColumn col in dgvPending.Columns)
                    {
                        if (dataTable.Columns.Contains(col.Name))
                        {
                            col.DataPropertyName = col.Name;
                        }
                    }
                    dgvPending.DataSource = dataTable;
                    dgvPending.Columns["Status"].Visible = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            this.Hide();
            Dashboard dashboard = new Dashboard();
            dashboard.Show();
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
            Report adminReports = new Report();
            adminReports.Show();
            this.Hide();
        }

        private void btnLogs_Click(object sender, EventArgs e)
        {
            Logs adminLogs = new Logs();
            adminLogs.Show();
            this.Hide();
        }

        private void btnApproval_Click(object sender, EventArgs e)
        {
            this.Show();
        }

        private void dgvPending_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvPending.Columns[e.ColumnIndex].Name == "StatusActionButton")
            {
                DataGridViewRow row = dgvPending.Rows[e.RowIndex];
                string profileId = row.Cells["ProfileID"].Value.ToString();
                string currentStatus = row.Cells["Status"].Value.ToString();
                string newStatus = string.Empty;

                if (currentStatus.Equals("Pending", StringComparison.OrdinalIgnoreCase))
                {
                    DialogResult result = MessageBox.Show($"Do you want to activate this student?", "Approve Student", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        newStatus = "Active";
                    }
                    else
                    {
                        return;
                    }
                }

                if (!string.IsNullOrEmpty(newStatus))
                {

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        try
                        {
                            conn.Open();
                            string updateQuery = "UPDATE Profiles SET Status = @newStatus WHERE ProfileID = @profileId";
                            using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                            {
                                cmd.Parameters.AddWithValue("@newStatus", newStatus);
                                cmd.Parameters.AddWithValue("@profileId", profileId);

                                int rowsAffected = cmd.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    MessageBox.Show($"Successfully updated status to '{newStatus}'.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                    LoadData();
                                }
                                else
                                {
                                    MessageBox.Show("No rows were affected. The update may have failed.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("An error occurred while updating the database: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearch.Text.Trim();

            if (string.IsNullOrEmpty(searchTerm))
            {
                LoadData();
                return;
            }

            // Base SQL query (for students)
            string sqlQuery = "SELECT p.ProfileID, p.FirstName, p.LastName, p.Age, p.Gender, p.Phone, p.Address, p.Email, p.Status " +
                              "FROM Profiles AS p " +
                              "INNER JOIN Users AS u ON p.ProfileID = u.ProfileID " +
                              "INNER JOIN Roles AS r ON u.RoleID = r.RoleID " +
                              "WHERE r.RoleName = 'Student' AND p.Status= 'Pending' AND ";

            // Building the dynamic WHERE clause
            if (int.TryParse(searchTerm, out int numericSearchTerm))
            {
                sqlQuery += "(p.ProfileID = @searchVal OR p.Age = @searchVal)";
            }
            else if (searchTerm.Equals("Male", StringComparison.OrdinalIgnoreCase) || searchTerm.Equals("Female", StringComparison.OrdinalIgnoreCase))
            {
                sqlQuery += "p.Gender = @searchVal";
            }
            else
            {
                sqlQuery += "(p.FirstName LIKE @searchVal OR p.LastName LIKE @searchVal OR p.Phone LIKE @searchVal OR p.Address LIKE @searchVal OR p.Email LIKE @searchVal OR p.Status LIKE @searchVal)";
            }

            // *** FIX: Replaced invalid teacher ORDER BY with a simple, valid one ***
            sqlQuery += " ORDER BY p.ProfileID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlQuery, conn);

                    // Add parameters based on the search type
                    // Using a single parameter name (@searchVal) for consistency
                    if (int.TryParse(searchTerm, out int numericSearchTerm2))
                    {
                        dataAdapter.SelectCommand.Parameters.AddWithValue("@searchVal", numericSearchTerm2);
                    }
                    else if (searchTerm.Equals("Male", StringComparison.OrdinalIgnoreCase) || searchTerm.Equals("Female", StringComparison.OrdinalIgnoreCase))
                    {
                        dataAdapter.SelectCommand.Parameters.AddWithValue("@searchVal", searchTerm);
                    }
                    else
                    {
                        dataAdapter.SelectCommand.Parameters.AddWithValue("@searchVal", "%" + searchTerm + "%");
                    }

                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    dgvPending.DataSource = dataTable; // Assuming ApprovalData is the student DataGridView

                    if (dataTable.Rows.Count == 0)
                    {
                        MessageBox.Show("No users found matching your search criteria.", "No Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred during search: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvPending.SelectedRows.Count > 0)
                {
                    DataGridViewRow selectedRow = dgvPending.SelectedRows[0];

                    string profileId = selectedRow.Cells["ProfileID"].Value.ToString();

                    string currentStatus = string.Empty;
                    if (selectedRow.Cells["Status"].Value != null)
                    {
                        currentStatus = selectedRow.Cells["Status"].Value.ToString();
                    }


                    DialogResult confirmResult = MessageBox.Show($"Are you sure you want to delete this Student {profileId}?", "Confirm Deactivation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (confirmResult == DialogResult.Yes)
                    {
                        string newStatus = "Inactive";
                        UpdateUserStatus(profileId, newStatus);

                    }
                }
                else
                {
                    MessageBox.Show("Please select a student to deactivate.", "No Student Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateUserStatus(string profileId, string newStatus)
        {


            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string updateQuery = "UPDATE Profiles SET Status = @newStatus WHERE ProfileID = @profileId";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@newStatus", newStatus);
                        cmd.Parameters.AddWithValue("@profileId", profileId);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show($"Student {profileId} has been set to '{newStatus}'.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            LoadData();
                        }
                        else
                        {
                            MessageBox.Show("The status could not be updated.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while updating the database: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
