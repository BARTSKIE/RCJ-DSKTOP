using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RCJ_ADMIN
{
    public partial class TransactionsPage : UserControl
    {
        public TransactionsPage()
        {
            InitializeComponent();
            SetupGrid(); // ← Initialize datagrid when loaded
            _ = RefreshGridFromFirebase();

        }

        // Local list to store transactions (you can switch this to Firebase later)
        List<Transaction> transactions = AppData.Transactions;



        // Class to hold each transaction
        public class Transaction
        {
            public string Id { get; set; }
            public string CustomerName { get; set; }
            public string ContactNumber { get; set; }
            public string Email { get; set; }
            public string StaffName { get; set; }
            public decimal AmountPaid { get; set; }
            public string Remarks { get; set; } // <- Custom note
            public DateTime Date { get; set; }
            public string PictureUrl { get; set; } // Optional image
        }


        // Setup DataGridView columns
        private void SetupGrid()
        {
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.Columns.Clear(); // Clear old columns

            // Customer Name
            var customerCol = new DataGridViewTextBoxColumn();
            customerCol.Name = "CustomerName";
            customerCol.HeaderText = "Customer Name";
            customerCol.DataPropertyName = "CustomerName";
            customerCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns.Add(customerCol);

            // Staff Name
            var staffCol = new DataGridViewTextBoxColumn();
            staffCol.Name = "StaffName";
            staffCol.HeaderText = "Staff Name";
            staffCol.DataPropertyName = "StaffName";
            staffCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns.Add(staffCol);

            // Contact #
            var contactCol = new DataGridViewTextBoxColumn();
            contactCol.Name = "ContactNumber";
            contactCol.HeaderText = "Contact #";
            contactCol.DataPropertyName = "ContactNumber";
            contactCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns.Add(contactCol);

            // Email
            var emailCol = new DataGridViewTextBoxColumn();
            emailCol.Name = "Email";
            emailCol.HeaderText = "Email";
            emailCol.DataPropertyName = "Email";
            emailCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns.Add(emailCol);

            // Glass Model
            var modelCol = new DataGridViewTextBoxColumn();
            modelCol.Name = "GlassModel";
            modelCol.HeaderText = "Glass Model";
            modelCol.DataPropertyName = "GlassModel";
            modelCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns.Add(modelCol);

            // Price
            var priceCol = new DataGridViewTextBoxColumn();
            priceCol.Name = "Price";
            priceCol.HeaderText = "Price";
            priceCol.DataPropertyName = "Price";
            priceCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns.Add(priceCol);

            // Date
            var dateCol = new DataGridViewTextBoxColumn();
            dateCol.Name = "Date";
            dateCol.HeaderText = "Date";
            dateCol.DataPropertyName = "Date";
            dateCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns.Add(dateCol);

            // Picture column
            var imageCol = new DataGridViewImageColumn
            {
                Name = "Picture",
                HeaderText = "Image",
                ImageLayout = DataGridViewImageCellLayout.Zoom
            };
            dataGridView1.Columns.Add(imageCol);


        }

        // Refresh grid manually
        private async Task RefreshGridFromFirebase()
        {
            try
            {
                dataGridView1.Rows.Clear();

                // Optional: show a temporary loading message
                dataGridView1.Rows.Add("Loading...", "", "", "", "", 0, "", null);


                var transactions = await FirebaseHelper.GetAllTransactions();
                AppData.Transactions = transactions;

                dataGridView1.Rows.Clear(); // Clear loading row

                foreach (var t in transactions)
                {
                    Image img = null;
                    if (!string.IsNullOrEmpty(t.PictureUrl))
                    {
                        try
                        {
                            string[] urls = t.PictureUrl.Split('|');
                            using (var client = new HttpClient())
                            {
                                var stream = await client.GetStreamAsync(urls[0]); // just first image
                                img = Image.FromStream(stream);
                            }
                        }
                        catch
                        {
                            // fallback: skip image
                        }
                    }

                    dataGridView1.Rows.Add(
                        t.CustomerName, t.StaffName, t.ContactNumber, t.Email,
                        t.GlassModel, t.Price, t.Date.ToString("g"), img
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load data:\n" + ex.Message);
            }
        }





        //// ADD Transaction button
        //private async void button1_Click(object sender, EventArgs e)
        //{
        //    using (var form = new AddTransactionForm())
        //    {
        //        if (form.ShowDialog() == DialogResult.OK)
        //        {
        //            var transaction = form.NewTransaction;

        //            // Add to shared list
        //            AppData.Transactions.Add(transaction);

        //            // Save to Firebase
        //            await FirebaseHelper.AddTransaction(transaction);

        //            await RefreshGridFromFirebase();

        //        }
        //    }
        //}



        // DELETE Selected Transaction
        private async void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                int index = dataGridView1.CurrentRow.Index;
                if (index >= 0 && index < AppData.Transactions.Count)
                {
                    var transaction = AppData.Transactions[index];

                    var confirm = MessageBox.Show(
                        $"Are you sure you want to delete the transaction for {transaction.CustomerName}?",
                        "Confirm Deletion",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning
                    );

                    if (confirm == DialogResult.Yes)
                    {
                        try
                        {
                            await FirebaseHelper.DeleteTransaction(transaction.Id);
                            AppData.Transactions.RemoveAt(index);
                            await RefreshGridFromFirebase();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Failed to delete transaction:\n" + ex.Message);
                        }
                    }
                }
            }
        }



        private async void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Ignore header row or invalid index
            if (e.RowIndex < 0 || e.ColumnIndex != dataGridView1.Columns["Picture"].Index)
                return;

            // Get the corresponding transaction
            var transaction = AppData.Transactions[e.RowIndex];

            if (string.IsNullOrWhiteSpace(transaction.PictureUrl))
                return;

            string[] urls = transaction.PictureUrl.Split('|');

            // Create a new form to show the images
            Form imageViewer = new Form
            {
                Text = $"Images for {transaction.CustomerName}",
                Width = 800,
                Height = 600,
                StartPosition = FormStartPosition.CenterParent
            };

            FlowLayoutPanel imagePanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true
            };

            imageViewer.Controls.Add(imagePanel);

            foreach (string url in urls)
            {
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        var stream = await client.GetStreamAsync(url);
                        var image = Image.FromStream(stream);

                        PictureBox pb = new PictureBox
                        {
                            Width = 200,
                            Height = 200,
                            SizeMode = PictureBoxSizeMode.Zoom,
                            Image = image,
                            Margin = new Padding(10)
                        };

                        imagePanel.Controls.Add(pb);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to load image: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            imageViewer.ShowDialog();
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            await RefreshGridFromFirebase();
        }

    }
}

