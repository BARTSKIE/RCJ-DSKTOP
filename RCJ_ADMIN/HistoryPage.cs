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
    public partial class HistoryPage : UserControl
    {
        private List<TransactionsPage.Transaction> transactions;
        private List<Appointment> appointments;

        public HistoryPage()
        {
            InitializeComponent();
            SetupUI();
            LoadData();
        }

        private async void LoadData()
        {
            transactions = await FirebaseHelper.GetAllTransactions();
            appointments = await FirebaseHelper.GetAllAppointments(); // You create this

            cmbFilter.SelectedIndex = 0; // default to Transactions
            LoadTransactionsToGrid();
        }

        private void SetupUI()
        {
            cmbFilter.Items.AddRange(new string[] { "Transactions", "Appointments" });
            cmbFilter.SelectedIndexChanged += (s, e) =>
            {
                if (cmbFilter.SelectedIndex == 0)
                    LoadTransactionsToGrid();
                else
                    LoadAppointmentsToGrid();
            };

            dgvHistory.CellClick += dgvHistory_CellClick;
        }

        private void LoadTransactionsToGrid()
        {
            dgvHistory.Rows.Clear();
            foreach (var t in transactions)
            {
                dgvHistory.Rows.Add(t.CustomerName, t.StaffName, t.Date.ToString("g"), "View");
            }
        }

        private void LoadAppointmentsToGrid()
        {
            dgvHistory.Rows.Clear();
            foreach (var a in appointments)
            {
                dgvHistory.Rows.Add(a.Name, a.Date.ToString("g"), a.Status, "View");
            }
        }

        private void dgvHistory_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != dgvHistory.Columns["Action"].Index)
                return;

            if (cmbFilter.SelectedIndex == 0)
            {
                var t = transactions[e.RowIndex];
                new TransactionDetailForm(t).ShowDialog();
            }
            else
            {
                var a = appointments[e.RowIndex];
                new AppointmentDetailForm(a).ShowDialog();
            }
        }
    }

