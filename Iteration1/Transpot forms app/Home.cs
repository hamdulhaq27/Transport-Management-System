using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Home : Form
    {
        private bool sidebarExpanded = false;
        private string text = " Next bus will leave at 12:25am! ";

        public Home()
        {
            InitializeComponent();
            panel1.Width = 0;
            button1.Left = panel1.Right + 10;
            timer1.Interval = 120; 
            timer1.Start();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            int targetWidth = sidebarExpanded ? 0 : 200; 
            int step = sidebarExpanded ? -20 : 20; 

            while ((step > 0 && panel1.Width < targetWidth) || (step < 0 && panel1.Width > targetWidth))
            {
                panel1.Width += step;
                await Task.Delay(10);
            }

            panel1.Width = targetWidth; 
            sidebarExpanded = !sidebarExpanded;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            text = text.Substring(1) + text[0];  
            textBox2.Text = text;
        }

        private void Home_Load(object sender, EventArgs e)
        {

        }
    }
}
