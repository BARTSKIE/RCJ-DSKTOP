using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RCJ_ADMIN
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void LoadPage(UserControl page)
        {
            showpanel.Controls.Clear();
            page.Dock = DockStyle.Fill;
            showpanel.Controls.Add(page);
        }

        private void exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        private void minimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void LoadTransactionsPage()
        {
            LoadPage(new TransactionsPage());
        }

        private void trans_Click(object sender, EventArgs e)
        {
            LoadTransactionsPage();
        }

        private void LoadDashboardPage()
        {
            LoadPage(new DashboardPage());
        }

        private void sales_Click(object sender, EventArgs e)
        {
            LoadDashboardPage();
        }
    }
}
