using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bas_DATSYS_IT505
{
    public partial class Student : Form
    {
        public Student()
        {
            InitializeComponent();
            dgvStudents.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            LoadData();

        }

        string connectionString = Database.ConnectionString;
        private string selectedStudentId;

        private void LoadData()
        {
            string sqlQuery_TotalCount = "SELECT COUNT(p.ProfileID) " +
                                   "FROM Profiles AS p " +
                                   "INNER JOIN Users AS u ON p.ProfileID = u.ProfileID " +
                                   "INNER JOIN Roles AS r ON u.RoleID = r.RoleID " +
                                   "WHERE r.RoleName = 'Student' AND p.Status = 'Active'";

            string sqlQuery_LoadData = "SELECT s.StudentID, p.FirstName, p.LastName, p.Age, p.Gender, p.Phone, p.Address, p.Email, ISNULL(p.Status, 'Unknown') AS Status " +
                                       "FROM Profiles AS p " +
                                       "INNER JOIN Users AS u ON p.ProfileID = u.ProfileID " +
                                       "INNER JOIN Roles AS r ON u.RoleID = r.RoleID " +
                                       "INNER JOIN Students AS s ON p.ProfileID = s.ProfileID " +
                                       "WHERE r.RoleName IN ('Student') AND p.Status = 'Active' " +
                                       "ORDER BY s.StudentID DESC";


            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    SqlCommand countCmd = new SqlCommand(sqlQuery_TotalCount, conn);
                    int activeTeacherCount = (int)countCmd.ExecuteScalar();
                    lblNumOfStud.Text = activeTeacherCount.ToString();

                    SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlQuery_LoadData, conn);
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    dgvStudents.AutoGenerateColumns = false;
                    dgvStudents.Columns.Clear();
                    dgvStudents.ReadOnly = true;

                    dgvStudents.Columns.Add("StudentID", "Student ID");
                    dgvStudents.Columns.Add("FirstName", "First Name");
                    dgvStudents.Columns.Add("LastName", "Last Name");
                    dgvStudents.Columns.Add("Age", "Age");
                    dgvStudents.Columns.Add("Gender", "Gender");
                    dgvStudents.Columns.Add("Phone", "Phone");
                    dgvStudents.Columns.Add("Address", "Address");
                    dgvStudents.Columns.Add("Email", "Email");
                    dgvStudents.Columns.Add("Status", "Status");

                    dgvStudents.Columns["Status"].Visible = false;




                    foreach (DataGridViewColumn col in dgvStudents.Columns)
                    {
                        if (dataTable.Columns.Contains(col.Name))
                        {
                            col.DataPropertyName = col.Name;
                        }
                    }

                    dgvStudents.DataSource = dataTable;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            Dashboard adminDashboard = new Dashboard();
            adminDashboard.Show();
            this.Hide();
        }

        private void btnStudents_Click(object sender, EventArgs e)
        {
            this.Show();
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

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearch.Text.Trim();

            if (string.IsNullOrEmpty(searchTerm))
            {
                LoadData();
                return;
            }

            bool isNumeric = int.TryParse(searchTerm, out int numericSearchTerm);
            bool isGender = searchTerm.Equals("Male", StringComparison.OrdinalIgnoreCase) || searchTerm.Equals("Female", StringComparison.OrdinalIgnoreCase);


            string sqlQuery = "SELECT s.StudentID, p.FirstName, p.LastName, p.Age, p.Gender, p.Phone, p.Address, p.Email, ISNULL(p.Status, 'Unknown') AS Status " +
                                       "FROM Profiles AS p " +
                                       "INNER JOIN Users AS u ON p.ProfileID = u.ProfileID " +
                                       "INNER JOIN Roles AS r ON u.RoleID = r.RoleID " +
                                       "INNER JOIN Students AS s ON p.ProfileID = s.ProfileID " +
                                       "WHERE r.RoleName IN ('Student') AND p.Status = 'Active' AND ";


            if (isNumeric)
            {
                sqlQuery += " (p.ProfileID = @searchVal OR p.Age = @searchVal)";
            }
            else if (isGender)
            {
                sqlQuery += " p.Gender = @searchVal";
            }
            else
            {
                sqlQuery += " (p.FirstName LIKE @searchVal OR p.LastName LIKE @searchVal OR p.Phone LIKE @searchVal OR p.Address LIKE @searchVal OR p.Email LIKE @searchVal OR p.Status LIKE @searchVal)";
            }

            sqlQuery += " ORDER BY s.StudentID DESC";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlQuery, conn);

                    if (isNumeric)
                    {
                        dataAdapter.SelectCommand.Parameters.AddWithValue("@searchVal", numericSearchTerm);
                    }
                    else if (isGender)
                    {
                        dataAdapter.SelectCommand.Parameters.AddWithValue("@searchVal", searchTerm);
                    }
                    else
                    {
                        dataAdapter.SelectCommand.Parameters.AddWithValue("@searchVal", "%" + searchTerm + "%");
                    }

                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);
                    dgvStudents.DataSource = dataTable;

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

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AdminAddStud adminAddStud = new AdminAddStud();
            adminAddStud.Show();
            this.Hide();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            pnlUpdStud.Show();
        }


       

        private void btnDelete_Click(object sender, EventArgs e)
        {
            string getProfileIDQuery = "SELECT ProfileID FROM Students WHERE StudentID = @studentID_int";
            int profileID = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(getProfileIDQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@studentID_int", selectedStudentId);
                        conn.Open();
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            profileID = Convert.ToInt32(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not find the Profile ID: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                if (dgvStudents.SelectedRows.Count > 0)
                {
                    DataGridViewRow selectedRow = dgvStudents.SelectedRows[0];


                    string currentStatus = string.Empty;

                    if (selectedRow.Cells["Status"].Value != null)
                    {
                        currentStatus = selectedRow.Cells["Status"].Value.ToString();
                    }


                    DialogResult confirmResult = MessageBox.Show($"Are you sure you want to deactivate this student?", "Confirm Deactivation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (confirmResult == DialogResult.Yes)
                    {
                        string LogName = txtFirstnamE.Text + " " + txtLastnamE.Text;
                        string logDescription = $"Deleted a student.";
                        AddLogEntry(LogName, "Delete Student", logDescription);

                        string newStatus = "Inactive";
                        UpdateUserStatus(profileID, newStatus);

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
        private void UpdateUserStatus(int profileId, string newStatus)
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
                            MessageBox.Show($"Student has been set to '{newStatus}'.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

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

       
        string mailPattern = @"^[\w\.-]+@gmail\.com$";
        string phonePattern = @"^(?:\+63|0)?9\d{9}$";
        string agePattern = @"^(1[0-9]{2}|[1-9]?[0-9])$";

        private string selectedProfileId;

        public static bool IsValid(string input, string pattern)
        {
            return Regex.IsMatch(input, pattern);
        }

        private void btnFUpdate_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear();
            errorProvider2.Clear();
            errorProvider3.Clear();
            errorProvider4.Clear();
            errorProvider5.Clear();
            errorProvider6.Clear();
            errorProvider7.Clear();


            if (string.IsNullOrEmpty(selectedStudentId))
            {
                MessageBox.Show("Please select a student to update.", "No Student Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool requiredFieldsMissing = false;

            if (string.IsNullOrWhiteSpace(txtFirstnamE.Text)) { errorProvider1.SetError(txtFirstnamE, "First name is required."); requiredFieldsMissing = true; }
            if (string.IsNullOrWhiteSpace(txtLastnamE.Text)) { errorProvider2.SetError(txtLastnamE, "Last name is required."); requiredFieldsMissing = true; }
            if (string.IsNullOrWhiteSpace(cmbGendEr.Text)) { errorProvider3.SetError(cmbGendEr, "Gender is required."); requiredFieldsMissing = true; }
            if (string.IsNullOrWhiteSpace(txtAgE.Text)) { errorProvider4.SetError(txtAgE, "Age is required."); requiredFieldsMissing = true; }
            if (string.IsNullOrWhiteSpace(txtPhoneNUM.Text)) { errorProvider5.SetError(txtPhoneNUM, "Phone number is required."); requiredFieldsMissing = true; }
            if (string.IsNullOrWhiteSpace(txtAddrEss.Text)) { errorProvider6.SetError(txtAddrEss, "Address is required."); requiredFieldsMissing = true; }
            if (string.IsNullOrWhiteSpace(txtEAddress.Text)) { errorProvider7.SetError(txtEAddress, "Email is required."); requiredFieldsMissing = true; }

            if (requiredFieldsMissing)
            {
                return;
            }

            string getProfileIDQuery = "SELECT ProfileID FROM Students WHERE StudentID = @studentID_int";
            int profileID = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(getProfileIDQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@studentID_int", selectedStudentId);
                        conn.Open();
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            profileID = Convert.ToInt32(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not find the Profile ID: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                string firstName = txtFirstnamE.Text;
                string lastName = txtLastnamE.Text;
                string gender = cmbGendEr.Text;
                string address = txtAddrEss.Text;
                string newEmail = txtEAddress.Text;
                string age = txtAgE.Text;
                string phone = txtPhoneNUM.Text;


                bool allValid = true;

                if (!IsValid(newEmail, mailPattern))
                {
                    errorProvider1.SetError(txtEAddress, "Please enter a valid Email.");
                    allValid = false;
                }

                if (!IsValid(phone, phonePattern))
                {
                    errorProvider2.SetError(txtPhoneNUM, "Please enter a valid Phone number.");
                    allValid = false;
                }

                if (!IsValid(age, agePattern))
                {
                    errorProvider3.SetError(txtAgE, "Age is in invalid format.");
                    allValid = false;
                }

                if (!allValid)
                {
                    return;

                }

                string originalEmail = dgvStudents.SelectedRows[0].Cells["Email"].Value.ToString();


                if (newEmail != originalEmail)
                {
                    if (IsEmailTaken(newEmail, profileID))
                    {
                        MessageBox.Show("This email address is already in use by another user.", "Email Invalid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }



                string simpleUpdateQuery =
                                    "UPDATE Profiles SET " +
                                    "FirstName = @firstName, " +
                                    "LastName = @lastName, " +
                                    "Age = @age, " +
                                    "Gender = @gender, " +
                                    "Phone = @phone, " +
                                    "Address = @address, " +
                                    "Email = @email " +
                                    "WHERE ProfileID = @profileID";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(simpleUpdateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@firstName", firstName);
                        cmd.Parameters.AddWithValue("@lastName", lastName);
                        cmd.Parameters.AddWithValue("@age", age);
                        cmd.Parameters.AddWithValue("@gender", gender);
                        cmd.Parameters.AddWithValue("@phone", phone);
                        cmd.Parameters.AddWithValue("@address", address);
                        cmd.Parameters.AddWithValue("@email", newEmail);
                        cmd.Parameters.AddWithValue("@profileID", profileID);

                        conn.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {


                            string LogName = txtFirstnamE.Text + " " + txtLastnamE.Text;
                            string logDescription = $"Updated a student.";
                            AddLogEntry(LogName, "Update Student", logDescription);

                            MessageBox.Show("Student profile updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadData();
                            pnlUpdStud.Visible = false;



                        }
                        else
                        {
                            MessageBox.Show("No records were updated. Profile not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred during the update: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool IsEmailTaken(string email, int currentProfileId)
        {

            string sqlQuery = "SELECT COUNT(*) FROM Profiles WHERE Email = @email AND ProfileID != @currentProfileId";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@currentProfileId", currentProfileId);
                    conn.Open();
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        private void dgvStudents_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvStudents.Rows[e.RowIndex];


                selectedStudentId = row.Cells["StudentID"].Value.ToString();

                string firstName = row.Cells["FirstName"].Value.ToString();
                string lastName = row.Cells["LastName"].Value.ToString();
                string age = row.Cells["Age"].Value.ToString();
                string gender = row.Cells["Gender"].Value.ToString();
                string phone = row.Cells["Phone"].Value.ToString();
                string address = row.Cells["Address"].Value.ToString();
                string email = row.Cells["Email"].Value.ToString();

                txtFirstnamE.Text = firstName;
                txtLastnamE.Text = lastName;
                txtAgE.Text = age;
                txtPhoneNUM.Text = phone;
                txtAddrEss.Text = address;
                txtEAddress.Text = email;

                cmbGendEr.Text = gender;
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want log out?", "Pizsity", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Form1 login = new Form1();
                login.Show();
                this.Close();
            }
        }

        private void btnApproval_Click(object sender, EventArgs e)
        {
            this.Hide();
            Approval approval = new Approval();
            approval.Show();
        }


        private void AddLogEntry(string Name, string action, string description)
        {

            string sqlQuery = "INSERT INTO Logs (Name, Action, Description) VALUES (@Name, @action, @description)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", Name);
                    cmd.Parameters.AddWithValue("@action", action);
                    cmd.Parameters.AddWithValue("@description", description);

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error logging action: " + ex.Message);
                    }
                }
            }
        }
    }
}
