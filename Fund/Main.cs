namespace Fund
{
    using System;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;

    using AutoMapper;

    /// <summary>
    ///     The main.
    /// </summary>
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            Mapper.CreateMap<FundBaseInfoView, FundBaseInfo>()
                .ForMember(
                    m => m.form_start,
                    n => n.MapFrom(r => Regex.IsMatch(r.form_start, @"\d") ? double.Parse(r.form_start) : 0))
                .ForMember(
                    m => m.form_year,
                    n => n.MapFrom(r => Regex.IsMatch(r.form_year, @"\d") ? double.Parse(r.form_year) : 0))
                .ForMember(
                    m => m.one_year,
                    n => n.MapFrom(r => Regex.IsMatch(r.one_year, @"\d") ? double.Parse(r.one_year) : 0))
                .ForMember(
                    m => m.six_month,
                    n => n.MapFrom(r => Regex.IsMatch(r.six_month, @"\d") ? double.Parse(r.six_month) : 0))
                .ForMember(
                    m => m.three_month,
                    n => n.MapFrom(r => Regex.IsMatch(r.three_month, @"\d") ? double.Parse(r.three_month) : 0));

            this.dataGridView1.DataSource = FundBaseInfo.Load();
            this.dataGridView2.DataSource = HisValue.Load();
            this.dataGridView3.DataSource = Computer.LoadHisValueTable();
            this.dataGridView4.DataSource = Computer.ComputingCorrelation(
                this.textBox4.Text,
                this.textBox5.Text,
                this.textBox16.Text);
            this.dataGridView9.DataSource = Stock.Load();
            this.dataGridView10.DataSource = Stock.LoadHis();
            //            this.dataGridView11.DataSource = Rsi.Comput(this.textBox30.Text, this.textBox27.Text);

            if (!(DateTime.Today.DayOfWeek == DayOfWeek.Saturday || DateTime.Today.DayOfWeek == DayOfWeek.Sunday)
                && DateTime.Now.Hour >= 9 && DateTime.Now.Hour <= 15)
            {
                this.timer1.Interval = int.Parse(WatcherConfig.Load().TimeSpan) * 60000;
                this.timer2.Interval = 60000;
                this.timer1.Start();
                this.timer2.Start();
            }

        }

        /// <summary>
        ///     请求总体数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            ShowLogin(
                () =>
                {
                    this.dataGridView1.DataSource = FundBaseInfo.ReLoad(
                         this.textBox2.Text,
                         this.textBox8.Text,
                         this.textBox9.Text,
                         this.textBox10.Text,
                         this.textBox11.Text);
                });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ShowLogin(
                () =>
                {
                    if (string.IsNullOrWhiteSpace(this.textBox7.Text))
                    {
                        this.dataGridView2.DataSource = HisValue.ReLoad(
                            this.textBox1.Text,
                            this.textBox16.Text,
                            this.textBox17.Text);
                    }

                    this.dataGridView2.DataSource = HisValue.ReLoad(
                        this.textBox1.Text,
                        this.textBox16.Text,
                        this.textBox17.Text,
                        this.textBox7.Text.Trim().Split(',').Where(r => r != string.Empty).ToList());
                });
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.dataGridView2.DataSource = HisValue.Filter(this.textBox3.Text);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.dataGridView3.DataSource = Computer.CreateHisValueTable();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ShowLogin(
                () =>
                {
                    var correlations = Computer.ComputingCorrelation(
                        this.textBox4.Text,
                        this.textBox5.Text,
                        this.textBox17.Text);
                    this.dataGridView4.DataSource = correlations;
                    this.dataGridView5.DataSource = Computer.BindCorrelationStatistics(correlations);
                });
        }

        private void dataGridView4_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            var rectangle = new Rectangle(
                e.RowBounds.Location.X,
                e.RowBounds.Location.Y,
                dataGridView1.RowHeadersWidth - 4,
                e.RowBounds.Height);

            TextRenderer.DrawText(
                e.Graphics,
                (e.RowIndex + 1).ToString(),
                dataGridView1.RowHeadersDefaultCellStyle.Font,
                rectangle,
                dataGridView1.RowHeadersDefaultCellStyle.ForeColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.dataGridView6.DataSource = Computer.BindCorrelationGroups(
                this.textBox6.Text,
                this.textBox20.Text,
                this.textBox21.Text,
                this.textBox22.Text,
                this.textBox4.Text,
                this.textBox5.Text);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            ShowLogin(
                () =>
                    {
                        var table = Computer.ComputingPriceLimit(this.textBox13.Text, this.textBox12.Text);
                        this.dataGridView7.DataSource = table;
                        this.textBox14.Text = Computer.PackagingFormula(table, this.textBox15.Text);
                    });
        }

        private void 显示ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
            WindowState = FormWindowState.Normal;
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("是否确认退出FundWatcher？", "退出", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
               == DialogResult.OK)
            {
                // 关闭所有的线程
                this.Dispose();
                this.Close();
            }
        }

        private void Main_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
            }
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("是否确认退出FundWatcher？", "退出", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
               == DialogResult.OK)
            {
                // 关闭所有的线程
                this.Dispose();
                this.Close();
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void 监视配置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var exist = false;
            foreach (Form form in Application.OpenForms)
            {
                if (form.Name == "Form1")
                {
                    exist = true;
                    form.Show();
                }
            }
            if (!exist)
            {
                var form1 = new Form1();
                form1.Show();
            }

        }

        private void button8_Click(object sender, EventArgs e)
        {
            this.dataGridView8.DataSource = SharpeRatio.GetFromPriceLimit(
                this.dataGridView7.DataSource as DataTable,
                this.textBox18.Text);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var info = string.Join("\r\n", FundWatcher.Watch());
            if (info.Length > 0)
            {
                MessageBox.Show(info);
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (FundWatcher.Watchers != null && FundWatcher.Watchers.Count > 0 && e.Button == MouseButtons.Left)
            {
                var info = string.Join(
                    "\r\n",
                    FundWatcher
                    .Watchers
                    .Select(
                        r =>
                        string.Format(
                            "[{0}]:{1}    {2}{3}",
                            r.Symbol,
                            r.Value,
                            ((r.Value - r.YesterdayValue) * 100 / r.YesterdayValue).ToString("0.00"),
                            r.Value - r.YesterdayValue > 0 ? "↑" : "↓")).ToList());
                this.notifyIcon1.BalloonTipText = info;
                this.notifyIcon1.ShowBalloonTip(1000, "实时涨跌", info, ToolTipIcon.Info);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(this.textBox19.Text))
            {
                var names = this.textBox19.Text.Trim().Split(',');
                var data = FundBaseInfo.Load().Where(r => names.Contains(r.symbol)).ToList();
                this.dataGridView1.DataSource = data;
            }
            else
            {
                this.dataGridView1.DataSource = FundBaseInfo.Load();
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (DateTime.Now.Hour == 14 && DateTime.Now.Minute == 45)
            {
                MessageBox.Show("检查投资更新！");
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            ShowLogin(() => this.dataGridView9.DataSource = Stock.GetList(this.textBox24.Text, this.textBox23.Text, this.textBox26.Text));
        }

        private void button11_Click(object sender, EventArgs e)
        {
            ShowLogin(() => this.dataGridView10.DataSource = Stock.GetHis(this.textBox25.Text));
        }

        private void ShowLogin(Action action)
        {
            var form2 = new Form2();
            form2.Show();
            Application.DoEvents();
            action();
            form2.Close();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(this.textBox29.Text))
            {
                this.dataGridView10.DataSource = Stock.Stocks.Where(r => r.Code == this.textBox29.Text.Trim()).ToList();
            }
            else
            {
                this.dataGridView10.DataSource = Stock.Stocks;
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            this.dataGridView11.DataSource = Rsi.Comput(this.textBox30.Text, this.textBox27.Text);
        }
    }
}