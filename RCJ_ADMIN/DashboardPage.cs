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
    public partial class DashboardPage : UserControl
    {
        public DashboardPage()
        {
            InitializeComponent();
        }

        private void UpdateSalesSummary()
        {
            var today = DateTime.Today;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek); // Sunday
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            var startOfYear = new DateTime(today.Year, 1, 1);

            lblTodaySales.Text = $"₱ {AppData.Transactions.Where(t => t.Date.Date == today).Sum(t => t.Price):N2}";
            lblWeekSales.Text = $"₱ {AppData.Transactions.Where(t => t.Date >= startOfWeek).Sum(t => t.Price):N2}";
            lblMonthSales.Text = $"₱ {AppData.Transactions.Where(t => t.Date >= startOfMonth).Sum(t => t.Price):N2}";
            lblYearSales.Text = $"₱ {AppData.Transactions.Where(t => t.Date >= startOfYear).Sum(t => t.Price):N2}";
        }


        private void DashboardPage_Load(object sender, EventArgs e)
        {
            UpdateSalesSummary();
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
