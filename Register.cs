using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bas_DATSYS_IT505
{
    public partial class Register : Form
    {
        public Register()
        {
            InitializeComponent();
           
        }
        string connectionString = @"Data Source=LAB4-PC48\LAB2PC45; Initial Catalog=BAS_DB; Integrated Security=true";
        string cmb;
        string mailPattern = @"^[\w\.-]+@gmail\.com$";
        string phonePattern = @"^(?:\+63|0)?9\d{9}$";

        private string HashPassword(string plainPassword)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(plainPassword);
                byte[] hash = sha256.ComputeHash(bytes);
                StringBuilder builder = new StringBuilder();
                foreach (byte b in hash)
                    builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }

        public static bool IsValidPhoneNumber(string phone, string pattern)
        {
            return Regex.IsMatch(phone, pattern);
        }

        public static bool IsValidGmail(string email, string pattern)
        {
            return Regex.IsMatch(email, pattern);
        }



        private void btnSubmit2_Click(object sender, EventArgs e)
        {
            int age;
            string phone = txtPhoneNo.Text;
            string email = txtEmail.Text;
            DateTime dateTime;
            dateTime = dateTimePicker1.Value;


            if (string.IsNullOrWhiteSpace(txtFirstname.Text) || string.IsNullOrWhiteSpace(txtLastname.Text) ||
               string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Please fill in all required fields.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!IsValidGmail(email, mailPattern))
            {
                MessageBox.Show("Please enter a valid email format", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            if (!int.TryParse(txtAge.Text, out age))
            {
                MessageBox.Show("Please enter a valid age.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            if (!IsValidPhoneNumber(phone, phonePattern))
            {
                MessageBox.Show("Please enter a vaild phone number.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }



            if (cmbGender.SelectedIndex == 0)
            {
                cmb += cmbGender.Text;
            }
            if (cmbGender.SelectedIndex == 1)
            {
                cmb += cmbGender.Text;
            }
            if (cmbGender.SelectedIndex == 2)
            {
                cmb += cmbGender.Text;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {

                conn.Open();

                SqlCommand Checkcmd = new SqlCommand("SELECT COUNT(*) FROM Profiles WHERE Email = @email", conn);
                Checkcmd.Parameters.AddWithValue("@email", txtEmail.Text);

                int userCount = (int)Checkcmd.ExecuteScalar();

                if (userCount > 0)
                {
                    MessageBox.Show("This email is already registered. Please use a different one.", "Registration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Random rnd = new Random();
                string generatedUserID = "ST" + rnd.Next(100000, 999999).ToString();
                string generatedPassword = generatedUserID;

                string hashedPassword = HashPassword(generatedPassword);

                SqlCommand cmd = new SqlCommand("RegisterStudent_SP", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@firstname", txtFirstname.Text);
                cmd.Parameters.AddWithValue("@lastname", txtLastname.Text);
                cmd.Parameters.AddWithValue("@age", age);
                cmd.Parameters.AddWithValue("@gender", cmb);
                cmd.Parameters.AddWithValue("@phone", txtPhoneNo.Text);
                cmd.Parameters.AddWithValue("@address", txtAddress.Text);
                cmd.Parameters.AddWithValue("@email", txtEmail.Text);
                cmd.Parameters.AddWithValue("@Username", generatedUserID);
                cmd.Parameters.AddWithValue("@EnrollmentDate", dateTime);
                cmd.Parameters.AddWithValue("@HashedPassword", hashedPassword);


                cmd.ExecuteNonQuery();
                MessageBox.Show("Registration Successful!" + "\n Username: " + generatedUserID +
                                "\n Password: " + generatedPassword +
                                "\n Please wait for the admin's approval.",
                                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }


        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Hide();
        }

        private void Register_Load(object sender, EventArgs e)
        {
            dateTimePicker1.Value = DateTime.Now;
        }
    }
}

