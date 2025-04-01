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
using Org.BouncyCastle.Crypto.Generators;


namespace WindowsFormsApp1
{

    public partial class SignUp : Form
    {
        public SignUp()
        {
            InitializeComponent();
        }

        private string connectionString = "server=localhost;database=user_database;user=root;password=im4usomeone;";
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            textBox2.UseSystemPasswordChar = !checkBox1.Checked;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Login loginForm = new Login();
            loginForm.Show();
            this.Hide();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            string password = textBox2.Text;
            string name = textBox3.Text;
            string roll = textBox1.Text;

            if (password.Length < 4 || password.Length > 8)
            {
                MessageBox.Show("Password length must be between 4 and 8 characters!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!name.All(c => char.IsLetter(c) || c == ' '))
            {
                MessageBox.Show("Name should not contain symbols!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (name.Length < 2)
            {
                MessageBox.Show("Write your full name!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (roll.Length != 7 || roll[0] != 'i')
            {
                MessageBox.Show("Roll number must be exactly 7 characters long and start with a lowercase 'i'!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Hash the password before storing
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Check if roll number already exists
                    string checkQuery = "SELECT COUNT(*) FROM users WHERE roll=@roll";
                    using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@roll", roll);
                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                        if (count > 0)
                        {
                            MessageBox.Show("Roll number already exists!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    // Insert new user
                    string query = "INSERT INTO users (roll, name, password) VALUES (@roll, @name, @password)";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@roll", roll);
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@password", hashedPassword); // Store hashed password

                        int result = cmd.ExecuteNonQuery();
                        if (result > 0)
                        {
                            MessageBox.Show("Registration successful!", "Registered", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Login loginForm = new Login();
                            loginForm.Show();
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("Registration failed!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                textBox2.UseSystemPasswordChar = false;
            }
            else
            {
                textBox2.UseSystemPasswordChar = true;
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }
    }
}
