using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RCJ_ADMIN
{
    public partial class AddTransactionDialog : Form
    {
        public TransactionsPage.Transaction NewTransaction { get; private set; }
        private string uploadedImageUrl = "";

        public AddTransactionDialog()
        {
            InitializeComponent();
        }

        private async void btnUploadImage_Click(object sender, EventArgs e)
        {
            using OpenFileDialog dlg = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png"
            };

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                pictureBoxPreview.Image = Image.FromFile(dlg.FileName);
                uploadedImageUrl = await UploadToCloudinary(dlg.FileName);
            }
        }

        private async Task<string> UploadToCloudinary(string filePath)
        {
            using var client = new HttpClient();
            using var content = new MultipartFormDataContent();

            content.Add(new StreamContent(File.OpenRead(filePath)), "file", Path.GetFileName(filePath));
            content.Add(new StringContent("QA8HfMfrdnj-DeucYvZpT8Olzug"), "upload_preset");

            var response = await client.PostAsync("https://api.cloudinary.com/v1_1/rcjdesk/image/upload", content);
            string json = await response.Content.ReadAsStringAsync();

            dynamic result = JsonConvert.DeserializeObject(json);
            return result.secure_url;
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            NewTransaction = new TransactionsPage.Transaction
            {
                CustomerName = txtCustomer.Text.Trim(),
                ContactNumber = txtContact.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                StaffName = txtStaff.Text.Trim(),
                AmountPaid = decimal.TryParse(txtAmount.Text, out decimal val) ? val : 0,
                Remarks = txtRemarks.Text.Trim(),
                Date = DateTime.Now,
                PictureUrl = uploadedImageUrl
            };

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
