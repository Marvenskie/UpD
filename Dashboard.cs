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
using System.Windows.Forms.DataVisualization.Charting;

namespace Bas_DATSYS_IT505
{
    public partial class Dashboard: Form
    {
        public Dashboard()
        {
            InitializeComponent();
            LoadStudentChart();
            LoadTeacherChart();
            LoadCount();
        }
        string connectionstring = Database.ConnectionString;

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            this.Show();
        }

        private void btnStudents_Click(object sender, EventArgs e)
        {
            Student  adminStudents = new Student();
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
           
        }

        private void btnLogs_Click(object sender, EventArgs e)
        {
            Logs adminLogs = new Logs();
            adminLogs.Show();
            this.Hide();
        }
        private void LoadStudentChart()
        {
            string sqlQuery = "SELECT Status, COUNT(*) AS TotalCount " +
                      "FROM Profiles " +
                      "WHERE ProfileID IN (SELECT ProfileID FROM Users WHERE RoleID = (SELECT RoleID FROM Roles WHERE RoleName = 'Student')) " +
                      "GROUP BY Status";

            using (SqlConnection conn = new SqlConnection(connectionstring))
            {
                try
                {
                    conn.Open();
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlQuery, conn);
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    chart1.Series.Clear();
                    chart1.ChartAreas.Clear();

                    chart1.ChartAreas.Add(new ChartArea("MainChart"));

                    Series series = new Series("StudentStatus");
                    series.ChartType = SeriesChartType.Column;
                    series.IsValueShownAsLabel = true;

                    foreach (DataRow row in dataTable.Rows)
                    {
                        string status = row["Status"].ToString();
                        int count = Convert.ToInt32(row["TotalCount"]);
                        series.Points.AddXY(status, count);
                    }

                    chart1.Series.Add(series);

                    chart1.Titles.Clear();
                    chart1.Titles.Add("Student");
                    chart1.ChartAreas["MainChart"].AxisX.Title = "Status";
                    chart1.ChartAreas["MainChart"].AxisY.Title = "Number of Students";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while loading the chart: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LoadTeacherChart()
        {
            string sqlQuery = "SELECT Status, COUNT(*) AS TotalCount " +
                      "FROM Profiles " +
                      "WHERE ProfileID IN (SELECT ProfileID FROM Users WHERE RoleID = (SELECT RoleID FROM Roles WHERE RoleName = 'Instructor')) " +
                      "GROUP BY Status";

            using (SqlConnection conn = new SqlConnection(connectionstring))
            {
                try
                {
                    conn.Open();
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlQuery, conn);
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    chart2.Series.Clear();
                    chart2.ChartAreas.Clear();

                    chart2.ChartAreas.Add(new ChartArea("MainChart"));

                    Series series = new Series("TeacherStatus");
                    series.ChartType = SeriesChartType.Column;
                    series.IsValueShownAsLabel = true;

                    foreach (DataRow row in dataTable.Rows)
                    {
                        string status = row["Status"].ToString();
                        int count = Convert.ToInt32(row["TotalCount"]);
                        series.Points.AddXY(status, count);
                    }

                    chart2.Series.Add(series);

                    chart2.Titles.Clear();
                    chart2.Titles.Add("Teacher");
                    chart2.ChartAreas["MainChart"].AxisX.Title = "Status";
                    chart2.ChartAreas["MainChart"].AxisY.Title = "Number of Teacher";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while loading the chart: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LoadCount()
        {

            string sqlQuery_TotalStudentCount = "SELECT COUNT(p.ProfileID) " +
                                          "FROM Profiles AS p " +
                                          "INNER JOIN Users AS u ON p.ProfileID = u.ProfileID " +
                                          "INNER JOIN Roles AS r ON u.RoleID = r.RoleID " +
                                          "WHERE r.RoleName = 'Student'";

            string sqlQuery_TotalTeacherCount = "SELECT COUNT(p.ProfileID) " +
                                          "FROM Profiles AS p " +
                                          "INNER JOIN Users AS u ON p.ProfileID = u.ProfileID " +
                                          "INNER JOIN Roles AS r ON u.RoleID = r.RoleID " +
                                          "WHERE r.RoleName = 'Instructor'";

            using (SqlConnection conn = new SqlConnection(connectionstring))
            {

                conn.Open();

                SqlCommand countCmd = new SqlCommand(sqlQuery_TotalStudentCount, conn);
                int StudentCount = (int)countCmd.ExecuteScalar();
                lblSTudCount.Text = StudentCount.ToString();

                SqlCommand countCMD = new SqlCommand(sqlQuery_TotalTeacherCount, conn);
                int TeacherCount = (int)countCMD.ExecuteScalar();
                lblTeacHCount.Text = TeacherCount.ToString();
            }
        }

        private void btnlogOUT_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to log out?","BAs",MessageBoxButtons.YesNo, MessageBoxIcon.Question)==DialogResult.Yes)
            {
                Form1 form1 = new Form1();
                form1.Show();
                this.Close();
            }
        }

        private void btnApproval_Click(object sender, EventArgs e)
        {
            this.Hide();
            Approval approval = new Approval();
            approval.Show();
        }
    }
}
