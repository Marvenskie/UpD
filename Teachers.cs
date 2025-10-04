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
    public partial class Teachers : Form
    {
        public Teachers()
        {
            InitializeComponent();
            dgvTeachers.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            LoadData();
        }
        string connectionString = Database.ConnectionString;
        private string selectedInstructorId;

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearch.Text.Trim();
            string parameterName = "";
            object parameterValue = null;

            if (string.IsNullOrEmpty(searchTerm))
            {
                LoadData();
                return;
            }

            string sqlQuery = "SELECT i.InstructorID, p.FirstName, p.LastName, p.Age, p.Gender, p.Phone, p.Address, p.Email, p.Status, d.DepartmentName " +
                              "FROM Profiles AS p " +
                              "INNER JOIN Users AS u ON p.ProfileID = u.ProfileID " +
                              "INNER JOIN Roles AS r ON u.RoleID = r.RoleID " +
                              "LEFT JOIN Instructors AS i ON p.ProfileID = i.ProfileID " +
                              "LEFT JOIN Departments AS d ON i.DepartmentID = d.DepartmentID " +
                              "WHERE r.RoleName = 'Instructor' AND p.Status = 'Active' AND ";

            if (int.TryParse(searchTerm, out int numericSearchTerm))
            {
                sqlQuery += "(p.ProfileID = @NumericTerm OR p.Age = @NumericTerm)";
                parameterName = "@NumericTerm";
                parameterValue = numericSearchTerm;
            }
            else if (searchTerm.Equals("Male", StringComparison.OrdinalIgnoreCase) || searchTerm.Equals("Female", StringComparison.OrdinalIgnoreCase))
            {
                sqlQuery += "p.Gender = @ExactTerm";
                parameterName = "@ExactTerm";
                parameterValue = searchTerm;
            }
            else
            {
                sqlQuery += "(p.FirstName LIKE @WildcardTerm OR p.LastName LIKE @WildcardTerm OR p.Phone LIKE @WildcardTerm OR p.Address LIKE @WildcardTerm OR p.Email LIKE @WildcardTerm OR p.Status LIKE @WildcardTerm OR d.DepartmentName LIKE @WildcardTerm)";
                parameterName = "@WildcardTerm";
                parameterValue = "%" + searchTerm + "%";
            }

            sqlQuery += " ORDER BY i.InstructorID DESC";


            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlQuery, conn);
                    if (parameterName != "")
                    {
                        dataAdapter.SelectCommand.Parameters.AddWithValue(parameterName, parameterValue);
                    }

                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    dgvTeachers.DataSource = dataTable;

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

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AdminAddTeach addTeacher = new AdminAddTeach();
            addTeacher.Show();
            this.Hide();
        }

        private void LoadData()
        {
            string sqlQuery_TotalCount = "SELECT COUNT(p.ProfileID) " +
                                   "FROM Profiles AS p " +
                                   "INNER JOIN Users AS u ON p.ProfileID = u.ProfileID " +
                                   "INNER JOIN Roles AS r ON u.RoleID = r.RoleID " +
                                   "WHERE r.RoleName = 'Instructor' AND p.Status = 'Active'";

            string sqlQuery_LoadData = "SELECT i.InstructorID, p.FirstName, p.LastName, p.Age, p.Gender, p.Phone, p.Address, p.Email, ISNULL(p.Status, 'Unknown') AS Status, d.DepartmentName " +
                                       "FROM Profiles AS p " +
                                       "INNER JOIN Users AS u ON p.ProfileID = u.ProfileID " +
                                       "INNER JOIN Roles AS r ON u.RoleID = r.RoleID " +
                                       "INNER JOIN Instructors AS i ON p.ProfileID = i.ProfileID " +
                                       "INNER JOIN Departments AS d ON i.DepartmentID = d.DepartmentID " +
                                       "WHERE r.RoleName IN ('Instructor') AND p.Status = 'Active' " +
                                       "ORDER BY " +
                                       "p.ProfileID DESC";


            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    SqlCommand countCmd = new SqlCommand(sqlQuery_TotalCount, conn);
                    int activeTeacherCount = (int)countCmd.ExecuteScalar();
                    lblNumOfTeachers.Text = activeTeacherCount.ToString();

                    SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlQuery_LoadData, conn);
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    dgvTeachers.AutoGenerateColumns = false;
                    dgvTeachers.Columns.Clear();
                    dgvTeachers.ReadOnly = true;

                    dgvTeachers.Columns.Add("InstructorID", "Instructor ID");
                    dgvTeachers.Columns.Add("FirstName", "First Name");
                    dgvTeachers.Columns.Add("LastName", "Last Name");
                    dgvTeachers.Columns.Add("Age", "Age");
                    dgvTeachers.Columns.Add("Gender", "Gender");
                    dgvTeachers.Columns.Add("Phone", "Phone");
                    dgvTeachers.Columns.Add("Address", "Address");
                    dgvTeachers.Columns.Add("Email", "Email");
                    dgvTeachers.Columns.Add("DepartmentName", "Department Name");
                    dgvTeachers.Columns.Add("Status", "Status");

                    dgvTeachers.Columns["Status"].Visible = false;




                    foreach (DataGridViewColumn col in dgvTeachers.Columns)
                    {
                        if (dataTable.Columns.Contains(col.Name))
                        {
                            col.DataPropertyName = col.Name;
                        }
                    }

                    dgvTeachers.DataSource = dataTable;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }



        private void btnUpdate_Click(object sender, EventArgs e)
        {
            pnlUpdTeach.Visible = true;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            string getProfileIDQuery = "SELECT ProfileID FROM Instructors WHERE InstructorID = @instructorID_int";
            int profileID = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(getProfileIDQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@instructorID_int", selectedInstructorId);
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
                if (dgvTeachers.SelectedRows.Count > 0)
                {
                    DataGridViewRow selectedRow = dgvTeachers.SelectedRows[0];


                    string currentStatus = string.Empty;
                    if (selectedRow.Cells["Status"].Value != null)
                    {
                        currentStatus = selectedRow.Cells["Status"].Value.ToString();
                    }


                    DialogResult confirmResult = MessageBox.Show($"Are you sure you want to deactivate this teacher?", "Confirm Deactivation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (confirmResult == DialogResult.Yes)
                    {
                        string newStatus = "Inactive";
                        UpdateUserStatus(profileID, newStatus);

                        //string logDescription = $"Deactivated a teacher";
                        //AddLogEntry(Convert.ToInt32(profileId), "Delete Teacher", logDescription);
                    }
                }
                else
                {
                    MessageBox.Show("Please select a teacher to deactivate.", "No Teacher Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                            MessageBox.Show($"Teacher has been set to '{newStatus}'.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

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
            errorProvider8.Clear();


            if (string.IsNullOrEmpty(selectedInstructorId))
            {
                MessageBox.Show("Please select a teacher to update.", "No Student Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            if (string.IsNullOrWhiteSpace(cmbDepartment.Text)) { errorProvider8.SetError(cmbDepartment, "Department is required."); requiredFieldsMissing = true; }

            if (requiredFieldsMissing)
            {
                return;
            }

            string getProfileIDQuery = "SELECT ProfileID FROM Instructors WHERE InstructorID = @instructorID_int";
            int profileID = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(getProfileIDQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@instructorID_int", selectedInstructorId);
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

                string originalEmail = dgvTeachers.SelectedRows[0].Cells["Email"].Value.ToString();


                if (newEmail != originalEmail)
                {
                    if (IsEmailTaken(newEmail, profileID))
                    {
                        MessageBox.Show("This email address is already in use by another user.", "Email Invalid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                string selectedDepartmentName = cmbDepartment.SelectedItem.ToString();

                int departmentID = GetDepartmentID(selectedDepartmentName);

                if (departmentID == -1)
                {
                    MessageBox.Show("Selected department not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }


                string sqlQuery = "UPDATE Profiles SET " +
                                  "FirstName = @FirstName, " +
                                  "LastName = @LastName, " +
                                  "Age = @Age, " +
                                  "Gender = @Gender, " +
                                  "Phone = @Phone, " +
                                  "Address = @Address, " +
                                  "Email = @Email " +
                                  "WHERE ProfileID = @profileId; " +
                                  "UPDATE Instructors SET DepartmentID = @DepartmentID WHERE ProfileID = @profileId;";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand(sqlQuery, conn);

                        cmd.Parameters.AddWithValue("@FirstName", txtFirstnamE.Text);
                        cmd.Parameters.AddWithValue("@LastName", txtLastnamE.Text);
                        cmd.Parameters.AddWithValue("@Age", txtAgE.Text);
                        cmd.Parameters.AddWithValue("@Gender", cmbGendEr.SelectedItem.ToString());
                        cmd.Parameters.AddWithValue("@Phone", txtPhoneNUM.Text);
                        cmd.Parameters.AddWithValue("@Address", txtAddrEss.Text);
                        cmd.Parameters.AddWithValue("@Email", txtEAddress.Text);
                        cmd.Parameters.AddWithValue("@profileId", profileID);
                        cmd.Parameters.AddWithValue("@DepartmentID", departmentID);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            // Create log description
                            //string logDescription = $"Updated a teacher.";
                            //AddLogEntry(selectedProfileID, "Update Teacher", logDescription);

                            MessageBox.Show("Teacher updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadData();
                        }
                        else
                        {
                            MessageBox.Show("No records were updated. Profile not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred during the update: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private int GetDepartmentID(string departmentName)
        {
            int departmentID = -1;
            string sqlQuery = "SELECT DepartmentID FROM Departments WHERE DepartmentName = @DepartmentName";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sqlQuery, conn);
                    cmd.Parameters.AddWithValue("@DepartmentName", departmentName);
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        departmentID = Convert.ToInt32(result);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while getting DepartmentID: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return departmentID;
        }

        private void dgvTeachers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvTeachers.Rows[e.RowIndex];

                selectedInstructorId = row.Cells["InstructorID"].Value.ToString();

                string firstName = row.Cells["FirstName"].Value.ToString();
                string lastName = row.Cells["LastName"].Value.ToString();
                string age = row.Cells["Age"].Value.ToString();
                string gender = row.Cells["Gender"].Value.ToString();
                string phone = row.Cells["Phone"].Value.ToString();
                string address = row.Cells["Address"].Value.ToString();
                string email = row.Cells["Email"].Value.ToString();
                string department = row.Cells["DepartmentName"].Value.ToString();

                txtFirstnamE.Text = firstName;
                txtLastnamE.Text = lastName;
                txtAgE.Text = age;
                txtPhoneNUM.Text = phone;
                txtAddrEss.Text = address;
                txtEAddress.Text = email;
                cmbGendEr.Text = gender;
                cmbDepartment.Text = department;
            }
        }

        

        private void btnApproval_Click(object sender, EventArgs e)
        {
            this.Hide();
            Approval approval = new Approval();
            approval.Show();
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
    }
}
