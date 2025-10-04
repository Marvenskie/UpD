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
    public partial class AdminAddStud : Form
    {
        public AdminAddStud()
        {
            InitializeComponent();
        }

        string connectionString = Database.ConnectionString;

        string cmb;

        string mailPattern = @"^[\w\.-]+@gmail\.com$";
        string phonePattern = @"^(?:\+63|0)?9\d{9}$";
        string agePattern = @"^(1[0-9]{2}|[1-9]?[0-9])$";

        public static bool IsValid(string input, string pattern)
        {
            return Regex.IsMatch(input, pattern);
        }

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

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear();
            errorProvider2.Clear();
            errorProvider3.Clear();
            errorProvider4.Clear();
            errorProvider5.Clear();
            errorProvider6.Clear();
            errorProvider7.Clear();


            string age = txtAge.Text;
            string phone = txtphoneNum.Text;
            string email = txtemailAddress.Text;
            DateTime enrollDate;
            enrollDate = dateTimePicker1.Value;
            string action = "Add Student";
            string description = "Added a new student";


            bool requiredFieldsMissing = false;

            if (string.IsNullOrWhiteSpace(txtfirstName.Text)) { errorProvider1.SetError(txtfirstName, "First name is required."); requiredFieldsMissing = true; }
            if (string.IsNullOrWhiteSpace(txtlastName.Text)) { errorProvider2.SetError(txtlastName, "Last name is required."); requiredFieldsMissing = true; }
            if (string.IsNullOrWhiteSpace(cmbGender.Text)) { errorProvider3.SetError(cmbGender, "Gender is required."); requiredFieldsMissing = true; }
            if (string.IsNullOrWhiteSpace(txtAge.Text)) { errorProvider4.SetError(txtAge, "Age is required."); requiredFieldsMissing = true; }
            if (string.IsNullOrWhiteSpace(txtphoneNum.Text)) { errorProvider5.SetError(txtphoneNum, "Phone number is required."); requiredFieldsMissing = true; }
            if (string.IsNullOrWhiteSpace(txtAddress.Text)) { errorProvider6.SetError(txtAddress, "Address is required."); requiredFieldsMissing = true; }
            if (string.IsNullOrWhiteSpace(txtemailAddress.Text)) { errorProvider7.SetError(txtemailAddress, "Email is required."); requiredFieldsMissing = true; }

            if (requiredFieldsMissing)
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


            try
            {

                bool allValid = true;

                if (!IsValid(email, mailPattern))
                {
                    errorProvider7.SetError(txtemailAddress, "Please enter a valid Email.");
                    allValid = false;
                }

                if (!IsValid(phone, phonePattern))
                {
                    errorProvider5.SetError(txtphoneNum, "Please enter a valid Phone number.");
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


                using (SqlConnection conn = new SqlConnection(connectionString))
                {

                    conn.Open();

                    SqlCommand Checkcmd = new SqlCommand("SELECT COUNT(*) FROM Profiles WHERE Email = @email", conn);
                    Checkcmd.Parameters.AddWithValue("@email", txtemailAddress.Text);

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

                    SqlCommand cmd = new SqlCommand("AddStudent_SP", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@firstname", txtfirstName.Text);
                    cmd.Parameters.AddWithValue("@lastname", txtlastName.Text);
                    cmd.Parameters.AddWithValue("@age", txtAge.Text);
                    cmd.Parameters.AddWithValue("@gender", cmb);
                    cmd.Parameters.AddWithValue("@phone", txtphoneNum.Text);
                    cmd.Parameters.AddWithValue("@address", txtAddress.Text);
                    cmd.Parameters.AddWithValue("@email", txtemailAddress.Text);
                    cmd.Parameters.AddWithValue("@Username", generatedUserID);
                    cmd.Parameters.AddWithValue("@HashedPassword", hashedPassword);
                    cmd.Parameters.AddWithValue("@EnrollmentDate", enrollDate);
                    cmd.Parameters.AddWithValue("@Action", action);
                    cmd.Parameters.AddWithValue("@Description", description);


                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Add Student Successful!" + "\n Username: " + generatedUserID +
                                    "\n Password: " + generatedPassword +
                                    "\n The account is pending.",
                                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);




                    Student adminStudents = new Student();
                    adminStudents.Show();
                    this.Hide();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Student adminStudents = new Student();
            adminStudents.Show();
        }
    }
}
