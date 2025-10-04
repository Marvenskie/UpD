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
    public partial class EmailConfirmation : Form
    {
        public EmailConfirmation()
        {
            InitializeComponent();
        }

        string connectionString = Database.ConnectionString;
        string mailPattern = @"^[\w\.-]+@gmail\.com$";

        public static bool IsValidGmail(string email, string pattern)
        {
            return Regex.IsMatch(email, pattern);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Form1 login = new Form1();
            login.Show();
            this.Hide();
        }

        private void btnCongirmPass_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text;

            if (string.IsNullOrEmpty(email))
            {
                errorProvider1.SetError(txtEmail, "Please enter your email address.");
                return;
            }

            if (!IsValidGmail(email, mailPattern))
            {
                MessageBox.Show("Please enter a valid email format", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = "SELECT ProfileID FROM Profiles WHERE Email = @Email";
            int profileId = -1;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Email", email);

                try
                {
                    connection.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        profileId = Convert.ToInt32(result);
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Database error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (profileId != -1)
            {
                MessageBox.Show("Successfully Confirmed!", "Email Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ForgotPass forgotConfirmForm = new ForgotPass(email);
                this.Hide();
                forgotConfirmForm.Show();
            }
            else
            {
                errorProvider1.SetError(txtEmail, "Email not found. Please check your email and try again.");

            }
        }
    }
}
