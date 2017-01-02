using System;
using System.Windows.Forms;

namespace Fund
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var config = WatcherConfig.Load();
            if (config != null)
            {
                this.textBox1.Text = config.Codes;
                this.textBox2.Text = config.UpRate;
                this.textBox3.Text = config.DownRate;
                this.textBox4.Text = config.TimeSpan;
                this.textBox5.Text = config.AlertTime;

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            WatcherConfig.Save(
                this.textBox1.Text,
                this.textBox6.Text,
                this.textBox2.Text,
                this.textBox3.Text,
                this.textBox4.Text,
                this.textBox5.Text);
            this.Close();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
