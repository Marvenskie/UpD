﻿using System;
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
        string connectionString = @"Data Source=DESKTOP-154LG8I; Initial Catalog=DB_Bas; Integrated Security=true";
        string cmb;
        string mailPattern = @"^[\w\.-]+@gmail\.com$";
        string phonePattern = @"^(?:\+63|0)?9\d{9}$";
        string agePattern = @"^(1[0-9]{2}|[1-9]?[0-9])$";

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

        public static bool IsValid(string input, string pattern)
        {
            return Regex.IsMatch(input, pattern);
        }

        



        private void btnSubmit2_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear();
            errorProvider2.Clear();
            errorProvider3.Clear();
            errorProvider4.Clear();
            errorProvider5.Clear();
            errorProvider6.Clear();
            errorProvider7.Clear();


            string age = txtAge.Text;
            string phone = txtPhoneNo.Text;
            string email = txtEmail.Text;


            bool requiredFieldsMissing = false;

            if (string.IsNullOrWhiteSpace(txtFirstname.Text)) { errorProvider1.SetError(txtFirstname, "First name is required."); requiredFieldsMissing = true; }
            if (string.IsNullOrWhiteSpace(txtLastname.Text)) { errorProvider2.SetError(txtLastname, "Last name is required."); requiredFieldsMissing = true; }
            if (string.IsNullOrWhiteSpace(cmbGender.Text)) { errorProvider3.SetError(cmbGender, "Gender is required."); requiredFieldsMissing = true; }
            if (string.IsNullOrWhiteSpace(txtAge.Text)) { errorProvider4.SetError(txtAge, "Age is required."); requiredFieldsMissing = true; }
            if (string.IsNullOrWhiteSpace(txtPhoneNo.Text)) { errorProvider5.SetError(txtPhoneNo, "Phone number is required."); requiredFieldsMissing = true; }
            if (string.IsNullOrWhiteSpace(txtAddress.Text)) { errorProvider6.SetError(txtAddress, "Address is required."); requiredFieldsMissing = true; }
            if (string.IsNullOrWhiteSpace(txtEmail.Text)) { errorProvider7.SetError(txtEmail, "Email is required."); requiredFieldsMissing = true; }

            if (requiredFieldsMissing)
            {
                return;
            }



            bool allValid = true;

            if (!IsValid(email, mailPattern))
            {
                errorProvider7.SetError(txtEmail, "Please enter a valid Email.");
                allValid = false;
            }

            if (!IsValid(phone, phonePattern))
            {
                errorProvider5.SetError(txtPhoneNo, "Please enter a valid Phone number.");
                allValid = false;
            }

            if (!IsValid(age, agePattern))
            {
                errorProvider4.SetError(txtAge, "Age is in invalid format.");
                allValid = false;
            }

            if (!allValid)
            {
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
                    MessageBox.Show("This email address is already in use by another user.", "Email Conflict", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                cmd.Parameters.AddWithValue("@age", txtAge.Text);
                cmd.Parameters.AddWithValue("@gender", cmb);
                cmd.Parameters.AddWithValue("@phone", txtPhoneNo.Text);
                cmd.Parameters.AddWithValue("@address", txtAddress.Text);
                cmd.Parameters.AddWithValue("@email", txtEmail.Text);
                cmd.Parameters.AddWithValue("@Username", generatedUserID);
                cmd.Parameters.AddWithValue("@HashedPassword", hashedPassword);


                cmd.ExecuteNonQuery();
                MessageBox.Show("Registration Successful!" + "\n Username: " + generatedUserID +
                                "\n Password: " + generatedPassword +
                                "\n Please wait for the admin's approval.",
                                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                Form1 login = new Form1();
                login.Show();
                this.Hide();
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

