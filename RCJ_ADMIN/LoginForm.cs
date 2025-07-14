using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace RCJ_ADMIN
{
    public partial class LoginForm : Form
    {
        private Timer slideUpTimer;
        private int slideSpeed = 20; // pixels per tick

        private const string FirebaseApiKey = "AIzaSyBFntuphB0i42f-AcHNLiojCkCXGYE7AI4";
        private const string AdminEmail = "rcjdesk.panel@gmail.com";

        public LoginForm()
        {
            InitializeComponent();
            SetupLockScreen();
        }

        private void SetupLockScreen()
        {
            // Position and visibility setup...
            lblError.Visible = false;
            txtPassword.Visible = false;
            btnLogin.Visible = false;

            lockPanel.Top = 0;
            lockPanel.BringToFront();
            lockPanel.Visible = true;

            // Show time/date immediately
            UpdateTimeAndDate();

            // Update every second
            Timer clockTimer = new Timer();
            clockTimer.Interval = 1000; // every 1 second
            clockTimer.Tick += (s, e) => UpdateTimeAndDate();
            clockTimer.Start();

            lockPanel.Click += (s, e) => ShowLoginFields();
            this.KeyPreview = true;
            this.KeyDown += (s, e) => ShowLoginFields();
        }

        private void UpdateTimeAndDate()
        {
            lblTime.Text = DateTime.Now.ToString("hh:mm tt").ToUpper();
            // e.g., 01:01 PM
            lblDate.Text = DateTime.Now.ToString("dd/MM/yyyy");     // e.g., 14/07/2025
        }



        private void ShowLoginFields()
        {
            slideUpTimer = new Timer();
            slideUpTimer.Interval = 10; // faster = smoother
            slideUpTimer.Tick += SlideLockPanelUp;
            slideUpTimer.Start();
        }

        private void SlideLockPanelUp(object sender, EventArgs e)
        {
            if (lockPanel.Top > -lockPanel.Height)
            {
                lockPanel.Top -= slideSpeed;
            }
            else
            {
                slideUpTimer.Stop();
                slideUpTimer.Dispose();

                lockPanel.Visible = false;

                // Reset position for future use
                lockPanel.Top = 0;

                // Now show login controls
                txtPassword.Visible = true;
                btnLogin.Visible = true;
                txtPassword.Focus();
            }
        }





        private async void btnLogin_Click_1(object sender, EventArgs e)
        {
            string password = txtPassword.Text;

            try
            {
                var response = await SignInWithFirebase(AdminEmail, password);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch
            {
                lblError.Text = "Invalid password!";
                lblError.Visible = true;

                // Optional: hide error after a few seconds
                var timer = new Timer { Interval = 3000 };
                timer.Tick += (s, args) =>
                {
                    lblError.Visible = false;
                    timer.Stop();
                };
                timer.Start();
            }
        }

        private async Task<FirebaseLoginResponse> SignInWithFirebase(string email, string password)
        {
            var payload = new
            {
                email = email,
                password = password,
                returnSecureToken = true
            };

            var json = JsonConvert.SerializeObject(payload);
            using (var client = new HttpClient())
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var result = await client.PostAsync(
                    $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={FirebaseApiKey}",
                    content
                );

                if (!result.IsSuccessStatusCode)
                    throw new Exception("Login failed");

                string responseString = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<FirebaseLoginResponse>(responseString);
            }
        }

        private class FirebaseLoginResponse
        {
            public string idToken { get; set; }
            public string email { get; set; }
            public string refreshToken { get; set; }
            public string expiresIn { get; set; }
            public string localId { get; set; }
        }
    }
}
