using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bas_DATSYS_IT505
{
    public partial class Report : Form
    {
        public Report()
        {
            InitializeComponent();
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

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            Dashboard adminDashboard = new Dashboard();
            adminDashboard.Show();
            this.Hide();
        }

        private void btnStudents_Click(object sender, EventArgs e)
        {

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
            this.Show();
        }

        private void btnLogs_Click(object sender, EventArgs e)
        {
            Logs adminLogs = new Logs();
            adminLogs.Show();
            this.Hide();
        }

        private void btnApproval_Click(object sender, EventArgs e)
        {
            this.Hide();
            Approval approval = new Approval();
            approval.Show();
        }
    }
}
