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
using static Bas_DATSYS_IT505.AdminAddSub;

namespace Bas_DATSYS_IT505
{
    public partial class Subjects : Form
    {
        public Subjects()
        {
            InitializeComponent();
            dgvSubjects.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            LoadCourses();
            dgvSubjects.DefaultCellStyle.ForeColor = Color.Black;
            DataTable departmentsData = DatabaseManager.GetDepartments();
            cmbDepartment.DataSource = departmentsData;
            cmbDepartment.DisplayMember = "DepartmentName";
            cmbDepartment.ValueMember = "DepartmentID";
        }

        string connectionString = Database.ConnectionString;

        private void LoadCourses()
        {
            string sqlQuery_TotalCount = "SELECT COUNT(*)FROM Courses " +
                "WHERE Status = 'Active'";

            string sqlQuery = "SELECT c.CourseID, c.CourseName, c.CourseCode, c.Description, c.Credits, " +
                              "p.FirstName, p.LastName, d.DepartmentName, c.Status " +
                              "FROM Courses AS c " +
                              "INNER JOIN Instructors AS i ON c.InstructorID = i.InstructorID " +
                              "INNER JOIN Profiles AS p ON i.ProfileID = p.ProfileID " +
                              "INNER JOIN Departments AS d ON c.DepartmentID = d.DepartmentID " +
                              "WHERE c.Status = 'Active' " +
                              "ORDER BY c.CourseName DESC";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {

                    conn.Open();

                    SqlCommand countCmd = new SqlCommand(sqlQuery_TotalCount, conn);
                    int activeTeacherCount = (int)countCmd.ExecuteScalar();
                    lblNumOfSubjects.Text = activeTeacherCount.ToString();

                    SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlQuery, conn);
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                     dgvSubjects.DataSource = dataTable;
                    SetupCoursesDataGridView();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while loading courses: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }



        private void DeleteCourse(int courseID)
        {

            string sqlCommand = "UPDATE Courses SET Status = 'Inactive' WHERE CourseID = @CourseID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sqlCommand, conn);
                    cmd.Parameters.AddWithValue("@CourseID", courseID);

                    int rowsAffected = cmd.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred during deletion: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void SetupCoursesDataGridView()
        {
            dgvSubjects.AutoGenerateColumns = false;
            dgvSubjects.Columns.Clear();
            dgvSubjects.ReadOnly = true;

            dgvSubjects.Columns.Add("CourseID", "Course ID");
            dgvSubjects.Columns.Add("CourseName", "Course Name");
            dgvSubjects.Columns.Add("CourseCode", "Course Code");
            dgvSubjects.Columns.Add("Description", "Description");
            dgvSubjects.Columns.Add("Credits", "Credits");
            dgvSubjects.Columns.Add("InstructorName", "Instructor Name");
            dgvSubjects.Columns.Add("DepartmentName", "Department Name");
            dgvSubjects.Columns.Add("Status", "Status");

            DataTable dataTable = (DataTable)dgvSubjects.DataSource;
            if (dataTable != null && !dataTable.Columns.Contains("InstructorName"))
            {
                dataTable.Columns.Add("InstructorName", typeof(string), "FirstName + ' ' + LastName");
            }

            foreach (DataGridViewColumn col in dgvSubjects.Columns)
            {
                if (dataTable.Columns.Contains(col.Name))
                {
                    col.DataPropertyName = col.Name;
                }
            }

            dgvSubjects.Columns["Status"].Visible = false;



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
           this.Show();
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

        private void btnFUpdate_Click(object sender, EventArgs e)
        {
            if (dgvSubjects.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a course.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int courseID = Convert.ToInt32(dgvSubjects.SelectedRows[0].Cells["CourseID"].Value);

            string selectedDepartmentName = cmbDepartment.Text;
            string selectedInstructorName = cmbTeacherAssigned.Text;

            if (string.IsNullOrEmpty(selectedDepartmentName) || string.IsNullOrEmpty(selectedInstructorName))
            {
                MessageBox.Show("Please select both a Department and an Instructor.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbDepartment.SelectedValue == null || cmbTeacherAssigned.SelectedValue == null)
            {
                MessageBox.Show("Selected Department or Instructor is not a valid item.", "Data Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int departmentID = Convert.ToInt32(cmbDepartment.SelectedValue);
            int instructorID = Convert.ToInt32(cmbTeacherAssigned.SelectedValue);

            string sqlQuery = "UPDATE Courses SET " +
                              "CourseName = @CourseName, " +
                              "CourseCode = @CourseCode, " +
                              "Credits = @Credits, " +
                              "Description = @Description, " +
                              "DepartmentID = @DepartmentID, " +
                              "InstructorID = @InstructorID " +
                              "WHERE CourseID = @CourseID;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sqlQuery, conn);

                    cmd.Parameters.AddWithValue("@CourseID", courseID);
                    cmd.Parameters.AddWithValue("@CourseName", txtCourse.Text);
                    cmd.Parameters.AddWithValue("@CourseCode", txtCourseCode.Text);
                    cmd.Parameters.AddWithValue("@Credits", cmbCredits.Text);
                    cmd.Parameters.AddWithValue("@Description", txtDescription.Text);

                    cmd.Parameters.AddWithValue("@DepartmentID", departmentID);
                    cmd.Parameters.AddWithValue("@InstructorID", instructorID);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Course details updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadCourses();


                    }
                    else
                    {
                        MessageBox.Show("Update failed. Course not found or no changes were made.", "Update Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred during the update: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private string selectedCourseId;
        private void dgvSubjects_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvSubjects.Rows[e.RowIndex];

                selectedCourseId = row.Cells["CourseID"].Value.ToString();

                string courseName = row.Cells["CourseName"].Value.ToString();
                string courseCode = row.Cells["CourseCode"].Value.ToString();
                string credits = row.Cells["Credits"].Value.ToString();
                string description = row.Cells["Description"].Value.ToString();
                string department = row.Cells["DepartmentName"].Value.ToString();
                string instructor = row.Cells["InstructorName"].Value.ToString();

                txtCourse.Text = courseName;
                txtCourseCode.Text = courseCode;
                cmbCredits.Text = credits;
                txtDescription.Text = description;

                cmbDepartment.SelectedIndexChanged -= cmbDepartment_SelectedIndexChanged;
                cmbDepartment.SelectedIndex = cmbDepartment.FindStringExact(department);
                cmbDepartment.SelectedIndexChanged += cmbDepartment_SelectedIndexChanged;

                if (cmbDepartment.SelectedValue != null)
                {
                    int selectedDepartmentID = Convert.ToInt32(cmbDepartment.SelectedValue);

                    DataTable instructorsData = DatabaseManager.GetInstructorsByDepartment(selectedDepartmentID);
                    cmbTeacherAssigned.DataSource = instructorsData;
                    cmbTeacherAssigned.DisplayMember = "FullName";
                    cmbTeacherAssigned.ValueMember = "InstructorID";
                }

                cmbTeacherAssigned.SelectedIndex = cmbTeacherAssigned.FindStringExact(instructor);
            }

        }
        public static class DatabaseManager
        {
            public static DataTable GetDepartments()
            {
                DataTable dataTable = new DataTable();
                string sqlQuery = "SELECT DepartmentID, DepartmentName FROM Departments";
                using (SqlConnection connection = new SqlConnection(Database.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        connection.Open();
                        SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                        dataAdapter.Fill(dataTable);
                    }
                }
                return dataTable;
            }


            public static DataTable GetInstructorsByDepartment(int departmentID)
            {
                DataTable dataTable = new DataTable();
                string sqlQuery = @"
                                  SELECT 
                                  i.InstructorID, 
                                  p.FirstName + ' ' + p.LastName AS FullName
                                  FROM 
                                  Instructors i
                                  INNER JOIN 
                                  Profiles p ON i.ProfileID = p.ProfileID
                                  WHERE 
                                  i.DepartmentID = @DepartmentID
                                  AND
                                  p.Status = 'Active';
                                  ";

                using (SqlConnection connection = new SqlConnection(Database.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@DepartmentID", departmentID);
                        connection.Open();
                        SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                        dataAdapter.Fill(dataTable);
                    }
                }
                return dataTable;
            }


        }

        private void cmbDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbDepartment.SelectedValue != null && cmbDepartment.SelectedValue.ToString() != "")
            {
                try
                {
                    int selectedDepartmentID = Convert.ToInt32(cmbDepartment.SelectedValue);

                    DataTable instructorsData = DatabaseManager.GetInstructorsByDepartment(selectedDepartmentID);

                    cmbTeacherAssigned.DataSource = instructorsData;
                    cmbTeacherAssigned.DisplayMember = "FullName";
                    cmbTeacherAssigned.ValueMember = "InstructorID";
                }
                catch (InvalidCastException ex)
                {
                    Console.WriteLine("InvalidCastException: " + ex.Message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message);
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AdminAddSub subject = new AdminAddSub();
            subject.Show();
            this.Hide();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            pnlUpdSub.Visible = true;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvSubjects.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dgvSubjects.SelectedRows[0];

                int courseID = Convert.ToInt32(selectedRow.Cells["CourseID"].Value);

                DialogResult result = MessageBox.Show("Are you sure you want to delete this course?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    DeleteCourse(courseID);

                    LoadCourses();
                }
            }
            else
            {
                MessageBox.Show("Please select a course to delete.", "No Course Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearch.Text.Trim();

            if (string.IsNullOrEmpty(searchTerm))
            {
                LoadCourses();
                return;
            }

            string sqlQuery = "SELECT c.CourseID, c.CourseName, c.CourseCode, c.Description, c.Credits, " +
                              "p.FirstName, p.LastName, d.DepartmentName, c.Status " +
                              "FROM Courses AS c " +
                              "INNER JOIN Instructors AS i ON c.InstructorID = i.InstructorID " +
                              "INNER JOIN Profiles AS p ON i.ProfileID = p.ProfileID " +
                              "INNER JOIN Departments AS d ON c.DepartmentID = d.DepartmentID " +
                              "WHERE c.Status = 'Active' AND " +
                              "(c.CourseName LIKE @searchTerm OR c.CourseCode LIKE @searchTerm OR p.FirstName LIKE @searchTerm OR p.LastName LIKE @searchTerm OR d.DepartmentName LIKE @searchTerm)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlQuery, conn);
                    dataAdapter.SelectCommand.Parameters.AddWithValue("@searchTerm", "%" + searchTerm + "%");

                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    dgvSubjects.DataSource = dataTable;

                    SetupCoursesDataGridView();

                    if (dataTable.Rows.Count == 0)
                    {
                        MessageBox.Show("No courses found matching your search criteria.", "No Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred during search: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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
