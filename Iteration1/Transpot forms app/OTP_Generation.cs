using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Net;
using System.Net.Mail;


namespace WindowsFormsApp1
{
    public partial class OTP_Generation : Form
    {
        private string rollNumber;
        private string generatedOTP;
        private string senderEmail;
        private DateTime otpExpirationTime;
        private string connectionString = "server=localhost;database=user_database;user=root;password=im4usomeone;";
        public OTP_Generation(string roll)
        {
            InitializeComponent();
            rollNumber = roll;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private string GenerateOTP()
        {
            Random rand = new Random();
            generatedOTP = rand.Next(1000, 9999).ToString(); // Generates a 4-digit OTP
            otpExpirationTime = DateTime.Now.AddMinutes(5); // OTP valid for 5 minutes
            return generatedOTP;
        }

        private bool SendOTPEmail(string recipientEmail, string otp)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("saifshahzad901@gmail.com"); // Replace with your email
                mail.To.Add(recipientEmail);
                mail.Subject = "Your OTP Code";
                mail.Body = $"Your 4-digit OTP for password reset is: {otp}\nValid for 5 minutes";

                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.Credentials = new NetworkCredential("saifshahzad901@gmail.com", "bzfs lkky urpn skln"); // Replace with real credentials
                smtp.EnableSsl = true;
                smtp.Send(mail);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Email error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine("Email error: " + ex.Message);
                return false;
            }
        }


        private void button1_Click(object sender, EventArgs e) // Send OTP button
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT email FROM users WHERE roll=@roll";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@roll", rollNumber);
                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            string email = result.ToString();
                            string otp = GenerateOTP();  // Generate a 4-digit OTP

                            bool emailSent = SendOTPEmail(email, otp);

                            if (emailSent)
                            {
                                MessageBox.Show($"A 4-digit password reset OTP has been sent to {email}!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                button1.Enabled = false; // Disable send button temporarily

                                // Re-enable send button after 1 minute
                                Timer cooldownTimer = new Timer();
                                cooldownTimer.Interval = 60000; // 1 minute
                                cooldownTimer.Tick += (s, args) =>
                                {
                                    button1.Enabled = true;
                                    cooldownTimer.Stop();
                                };
                                cooldownTimer.Start();
                            }
                            else
                            {
                                MessageBox.Show("Failed to send OTP. Please try again later.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Error retrieving email for the roll number!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }

        private void otpTextBox_TextChanged(object sender, EventArgs e)
        {
            TextBox currentBox = sender as TextBox;

            // Move focus to next box when a digit is entered
            if (currentBox != null && currentBox.Text.Length == 1)
            {
                if (currentBox == otp1 && otp2.Enabled)
                    otp2.Focus();
                else if (currentBox == otp2 && otp3.Enabled)
                    otp3.Focus();
                else if (currentBox == otp3 && otp4.Enabled)
                    otp4.Focus();
            }

            // Auto-verify when all 4 digits are entered
            if (!string.IsNullOrEmpty(otp1.Text) &&
                !string.IsNullOrEmpty(otp2.Text) &&
                !string.IsNullOrEmpty(otp3.Text) &&
                !string.IsNullOrEmpty(otp4.Text))
            {
                button2.PerformClick();
            }
        }

        private void OTPTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Login loginForm = new Login();
            loginForm.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Combine all OTP digits
            string enteredOTP = otp1.Text + otp2.Text + otp3.Text + otp4.Text;

            if (enteredOTP.Length < 4)
            {
                MessageBox.Show("Please enter all 4 digits of the OTP.", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (DateTime.Now > otpExpirationTime)
            {
                MessageBox.Show("OTP has expired. Please request a new one.", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                ClearOTPBoxes();
                return;
            }

            if (enteredOTP == generatedOTP)
            {
                // OTP verified, open password reset form
                changePassword resetForm = new changePassword(rollNumber);
                resetForm.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Invalid OTP. Please try again.", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                ClearOTPBoxes();
            }
        }

        private void ClearOTPBoxes()
        {
            otp1.Text = "";
            otp2.Text = "";
            otp3.Text = "";
            otp4.Text = "";
            otp1.Focus();
        }

        // Add these KeyPress handlers for each OTP TextBox
        private void otp1_KeyPress(object sender, KeyPressEventArgs e)
        {
            HandleOTPKeyPress(otp1, null, e);
        }

        private void otp2_KeyPress(object sender, KeyPressEventArgs e)
        {
            HandleOTPKeyPress(otp2, otp1, e);
        }

        private void otp3_KeyPress(object sender, KeyPressEventArgs e)
        {
            HandleOTPKeyPress(otp3, otp2, e);
        }

        private void otp4_KeyPress(object sender, KeyPressEventArgs e)
        {
            HandleOTPKeyPress(otp4, otp3, e);
        }

        private void HandleOTPKeyPress(TextBox current, TextBox previous, KeyPressEventArgs e)
        {
            // Only allow digits and backspace
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
                return;
            }

            // Handle backspace
            if (e.KeyChar == (char)Keys.Back)
            {
                if (string.IsNullOrEmpty(current.Text) && previous != null)
                {
                    previous.Focus();
                }
                return;
            }

            // Auto move to next box
            if (char.IsDigit(e.KeyChar))
            {
                if (current.Text.Length == 0)
                {
                    current.Text = e.KeyChar.ToString();
                    current.SelectionStart = 1;
                    e.Handled = true;

                    // Move to next box
                    if (current == otp1) otp2.Focus();
                    else if (current == otp2) otp3.Focus();
                    else if (current == otp3) otp4.Focus();
                }
                else
                {
                    e.Handled = true;
                }
            }
        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {
            senderEmail = textBox1.Text.Trim();
        }

        private void button1_Click_1(object sender, EventArgs e) // Send OTP button
        {
            if (string.IsNullOrEmpty(senderEmail))
            {
                MessageBox.Show("Please enter your email before sending an OTP.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM users WHERE roll=@roll";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@roll", rollNumber);
                        int userExists = Convert.ToInt32(cmd.ExecuteScalar());

                        if (userExists > 0)
                        {
                            string otp = GenerateOTP();  // Generate a 4-digit OTP

                            bool emailSent = SendOTPEmail(senderEmail, otp); // Send OTP to entered email

                            if (emailSent)
                            {
                                MessageBox.Show($"A 4-digit OTP has been sent to {senderEmail}!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                button1.Enabled = false; // ✅ Correct way to disable button

                                // Re-enable send button after 1 minute
                                Timer cooldownTimer = new Timer();
                                cooldownTimer.Interval = 60000; // 1 minute
                                cooldownTimer.Tick += (s, args) =>
                                {
                                    button1.Enabled = true; // ✅ Correct way to re-enable button
                                    cooldownTimer.Stop();
                                };
                                cooldownTimer.Start();
                            }
                            else
                            {
                                MessageBox.Show("Failed to send OTP. Please try again later.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Roll number not found in the system!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
