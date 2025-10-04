using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bas_DATSYS_IT505
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            lockoutTime = DateTime.Now;
        }
        string connectionString = Database.ConnectionString;

        private DateTime lockoutTime;

        private int loginAttempts = 0;
        private const int MAX_ATTEMPTS = 3;

        private static string HashPassword(string plainPassword)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(plainPassword);
                byte[] hash = sha256.ComputeHash(bytes);
                StringBuilder builder = new StringBuilder();
                foreach (byte b in hash)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (DateTime.Now < lockoutTime)
            {
                TimeSpan remainingTime = lockoutTime - DateTime.Now;
                MessageBox.Show($"Maximum login attempts exceeded. Please try again after {remainingTime.Minutes} minutes and {remainingTime.Seconds} seconds.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            bool isValid = true;
            string username = txtUsername.Text.Trim();
            string plainPassword = txtPassword.Text.Trim();

            string plainpassword = txtPassword.Text;
            string hashpassword = (HashPassword(plainpassword));

            if (string.IsNullOrWhiteSpace(username))
            {
                errorProvider1.SetError(txtUsername, "Username is required.");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(plainPassword))
            {
                errorProvider2.SetError(txtPassword, "Password is required.");
                isValid = false;
            }

            if (isValid)
            {

                string PlainPassword = txtPassword.Text;
                string hashedPassword = HashPassword(plainPassword);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("Login_SP", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@username", txtUsername.Text);
                    cmd.Parameters.AddWithValue("@password", hashedPassword);

                    conn.Open();

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        loginAttempts = 0;


                        int userId = Convert.ToInt32(reader["UserID"]);
                        string Username = reader["Username"].ToString();
                        string status = reader["Status"].ToString();
                        int roleId = Convert.ToInt32(reader["RoleID"]);
                        string roleName = reader["RoleName"].ToString();
                        string firstName = reader["FirstName"].ToString();
                        string lastName = reader["LastName"].ToString();

                        if (status == "Pending")
                        {
                            MessageBox.Show("Your account is pending approval. Please wait for the admin to approve your account.", "Login", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.Show();
                            return;
                        }

                        if (status == "Inactive")
                        {
                            MessageBox.Show("Your account is inactive. Please wait for the admin to approve your account.", "Login", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.Show();
                            return;
                        }


                        MessageBox.Show($"Login Successful! Welcome, {roleName} {firstName} {lastName}.", "Login", MessageBoxButtons.OK, MessageBoxIcon.Information);


                        if (roleId == 1)
                        {
                            this.Hide();
                            Dashboard adminDashboard = new Dashboard();
                            adminDashboard.Show();
                        }
                        else if (roleId == 2)
                        {
                            this.Hide();
                            TeachDashboard teacherDashboard = new TeachDashboard();
                            teacherDashboard.Show();
                        }
                        else if (roleId == 3)
                        {
                            this.Hide();
                            StudDashboard studentDashboard = new StudDashboard();
                            studentDashboard.Show();
                        }
                    }
                    else
                    {
                        loginAttempts++;

                        if (loginAttempts >= MAX_ATTEMPTS)
                        {
                            lockoutTime = DateTime.Now.AddMinutes(3);
                            MessageBox.Show($"Maximum login attempts exceeded. You are locked out for 3 minutes.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            MessageBox.Show($"Invalid username or password. You have {MAX_ATTEMPTS - loginAttempts} attempts remaining.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }


        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            
            Register register = new Register();
            register.Show();
            this.Hide();
        }

        private void pbVisible_Click(object sender, EventArgs e)
        {
            pbVisible.Hide();
            pbNotVisible.Show();
            txtPassword.UseSystemPasswordChar = true;
        }

        private void pbNotVisible_Click(object sender, EventArgs e)
        {
            pbNotVisible.Hide();
            pbVisible.Show();
            txtPassword.UseSystemPasswordChar = false;
        }

        private void btnForgotPass_Click(object sender, EventArgs e)
        {
            EmailConfirmation confirmation = new EmailConfirmation();
            confirmation.Show();
            this.Hide();
        }
    }
}
