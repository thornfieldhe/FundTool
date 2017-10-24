using System;
using System.Windows.Forms;

namespace Fund
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    public partial class Form1 : Form
    {
        private bool asc = true;

        public Form1()
        {
            InitializeComponent();
            FundEntity.Load();
            HisValue.Load();
            this.Load1();
            this.Load2();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Load1();
        }

        private void Load1()
        {
            if (FundEntity.Instance.Total == 0)
            {
                GetTotal();
                var list1 = new List<string>();
                var list2 = new List<string>();
                var list3 = new List<string>();
                var list4 = new List<string>();
                var list5 = new List<string>();
                var list6 = new List<string>();

                // list1 = this.ChooseStep1();
                // list2 = this.ChooseStep2();
                // list3 = this.ChooseStep3();
                // list4 = this.ChooseStep4();
                // list5 = this.ChooseStep5();
                // list6 = this.ChooseStep6();
                var t1 = new Task(() => list1 = this.ChooseStep1());
                t1.Start();
                var t2 = new Task(() => list2 = this.ChooseStep2());
                t2.Start();
                var t3 = new Task(() => list3 = this.ChooseStep3());
                t3.Start();
                var t4 = new Task(() => list4 = this.ChooseStep4());
                t4.Start();
                var t5 = new Task(() => list5 = this.ChooseStep5());
                t5.Start();
                var t6 = new Task(() => list6 = this.ChooseStep6());
                t6.Start();
                Task.WaitAll(t1, t2, t3, t4, t5, t6);
                var list = list1.Intersect(list2).Intersect(list3).Intersect(list4).Intersect(list5)
                    .Intersect(list6).ToList();
                FundEntity.Instance.Pool = GetSimpleList(list);
                FundEntity.SaveConfig();
            }
            if (!string.IsNullOrWhiteSpace(this.textBox2.Text))
            {
                var rate = decimal.Parse(this.textBox2.Text);
                FundEntity.Instance.Pool =
                    FundEntity.Instance.Pool.Where(r => r.J6gy > rate && r.J1n > rate && r.J2n > rate).ToList();
            }


            this.bindingSource1.DataSource = FundEntity.Instance.Pool;
        }

        /// <summary>
        /// 获取基金总数
        /// </summary>
        /// <returns></returns>
        private int GetTotal()
        {
            if (FundEntity.Instance.Total == 0)
            {
                var data = FundDataHelper.GetDataFromUrl(
                    "http://fund.eastmoney.com/data/rankhandler.aspx?op=ph&dt=kf&ft=all&rs=&gs=0&sc=zzf&st=descpi=1&pn=1&dx=1");

                FundEntity.Instance.Total = int.Parse(FundDataHelper.RegexMatch(data, @"allNum:(\d+)"));
            }

            return FundEntity.Instance.Total;
        }

        /// <summary>
        /// 满足近一年收益在同类基金中的排名必须在前1/4
        /// </summary>
        private List<string> ChooseStep1()
        {
            var data = FundDataHelper.GetDataFromUrl(
                $"http://fund.eastmoney.com/data/rankhandler.aspx?op=ph&dt=kf&ft=all&rs=&gs=0&sc=1nzf&st=desc&sd={DateTime.Today.AddYears(-1).ToString("yyyy-MM-dd")}&ed={DateTime.Today.ToString("yyyy-MM-dd")}&qdii=&tabSubtype=,,,,,&pi=1&pn={FundEntity.Instance.Total / int.Parse(this.textBox1.Text)}&dx=1");
            return FundDataHelper.RegexMatches(data, "\"" + @"(\d+)\,");
        }

        /// <summary>
        /// 基金2年以来基金绩效在同类基金中的排名必须在前1/4
        /// </summary>
        private List<string> ChooseStep2()
        {
            var data = FundDataHelper.GetDataFromUrl(
                $"http://fund.eastmoney.com/data/rankhandler.aspx?op=ph&dt=kf&ft=all&rs=&gs=0&sc=2nzf&st=desc&sd={DateTime.Today.AddYears(-1).ToString("yyyy-MM-dd")}&ed={DateTime.Today.ToString("yyyy-MM-dd")}&qdii=&tabSubtype=,,,,,&pi=1&pn={FundEntity.Instance.Total / int.Parse(this.textBox8.Text)}&dx=1&v=0.4092219410412177");
            return FundDataHelper.RegexMatches(data, "\"" + @"(\d+)\,");
        }

        /// <summary>
        /// 基金3年以来基金绩效在同类基金中的排名必须在前1/4
        /// </summary>
        private List<string> ChooseStep3()
        {
            var data = FundDataHelper.GetDataFromUrl(
                $"http://fund.eastmoney.com/data/rankhandler.aspx?op=ph&dt=kf&ft=all&rs=&gs=0&sc=3nzf&st=desc&sd={DateTime.Today.AddYears(-1).ToString("yyyy-MM-dd")}&ed={DateTime.Today.ToString("yyyy-MM-dd")}&qdii=&tabSubtype=,,,,,&pi=1&pn={FundEntity.Instance.Total / int.Parse(this.textBox7.Text)}&dx=1&v=0.41588528205085806");
            return FundDataHelper.RegexMatches(data, "\"" + @"(\d+)\,");
        }

        /// <summary>
        /// 基金今年以来基金绩效在同类基金中的排名必须在前1/4
        /// </summary>
        private List<string> ChooseStep4()
        {
            var data = FundDataHelper.GetDataFromUrl(
                $"http://fund.eastmoney.com/data/rankhandler.aspx?op=ph&dt=kf&ft=all&rs=&gs=0&sc=jnzf&st=desc&sd={DateTime.Today.AddYears(-1).ToString("yyyy-MM-dd")}&ed={DateTime.Today.ToString("yyyy-MM-dd")}&qdii=&tabSubtype=,,,,,&pi=1&pn={FundEntity.Instance.Total / int.Parse(this.textBox6.Text)}&dx=1&v=0.8129854273117316");
            return FundDataHelper.RegexMatches(data, "\"" + @"(\d+)\,");
        }

        /// <summary>
        /// 基金近6个月收益在同类基金中的排名必须在前1/3
        /// </summary>
        private List<string> ChooseStep5()
        {
            var data = FundDataHelper.GetDataFromUrl(
                $"http://fund.eastmoney.com/data/rankhandler.aspx?op=ph&dt=kf&ft=all&rs=&gs=0&sc=6yzf&st=desc&sd={DateTime.Today.AddYears(-1).ToString("yyyy-MM-dd")}&ed={DateTime.Today.ToString("yyyy-MM-dd")}&qdii=&tabSubtype=,,,,,&pi=1&pn={FundEntity.Instance.Total / int.Parse(this.textBox5.Text)}&dx=1&v=0.7777132235075257");
            return FundDataHelper.RegexMatches(data, "\"" + @"(\d+)\,");
        }

        /// <summary>
        /// 基金近3个月收益在同类基金中的排名必须在前1/3
        /// </summary>
        private List<string> ChooseStep6()
        {
            var data = FundDataHelper.GetDataFromUrl(
                $"http://fund.eastmoney.com/data/rankhandler.aspx?op=ph&dt=kf&ft=all&rs=&gs=0&sc=3yzf&st=desc&sd={DateTime.Today.AddYears(-1).ToString("yyyy-MM-dd")}&ed={DateTime.Today.ToString("yyyy-MM-dd")}&qdii=&tabSubtype=,,,,,&pi=1&pn={FundEntity.Instance.Total / int.Parse(this.textBox4.Text)}&dx=1&v=0.2586164657790102");
            return FundDataHelper.RegexMatches(data, "\"" + @"(\d+)\,");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FundEntity.Instance.Total = 0;
            FundEntity.Instance.Pool = new List<FundSimpleListItem>();
        }


        private List<FundSimpleListItem> GetSimpleList(List<string> list)
        {
            var result = new List<FundSimpleListItem>();
            var url =
                $"http://fund.eastmoney.com/data/rankhandler.aspx?op=ph&dt=kf&ft=all&rs=&gs=0&sc=1nzf&st=desc&sd={DateTime.Today.AddYears(-1).ToString("yyyy-MM-dd")}&ed={DateTime.Today.ToString("yyyy-MM-dd")}&qdii=&tabSubtype=,,,,,&pi=1&pn=1000&dx=1";
            var regex = new Regex("\"" + @"(.*?)\,(.*?)\,(.*?)\,(.*?)\,(.*?)\,(.*?)\,(.*?)\,(.*?)\,(.*?)\,(.*?)\,(.*?)\,(.*?)\,(.*?)\,(.*?)\,(.*?)\,(.*?)\,(.*?)" + "\"");
            var data = FundDataHelper.GetDataFromUrl(url);
            foreach (Match match in regex.Matches(data))
            {

                var code = match.Groups[1].Value;
                if (list.Contains(code))
                {
                    var item = new FundSimpleListItem()
                    {
                        Code = match.Groups[1].Value,
                        Jjmc = match.Groups[2].Value,
                        Date = match.Groups[4].Value,
                        Dwjz = decimal.Parse(match.Groups[5].Value),
                        Zf = decimal.Parse(match.Groups[7].Value)
                    };
                    var r = 0M;
                    decimal.TryParse(match.Groups[8].Value, out r);
                    item.J1z = r;
                    r = 0M;
                    decimal.TryParse(match.Groups[9].Value, out r);
                    item.J1gy = r;
                    r = 0M;
                    decimal.TryParse(match.Groups[10].Value, out r);
                    item.J3gy = r;
                    r = 0M;
                    decimal.TryParse(match.Groups[11].Value, out r);
                    item.J6gy = r;
                    r = 0M;
                    decimal.TryParse(match.Groups[12].Value, out r);
                    item.J1n = r;
                    r = 0M;
                    decimal.TryParse(match.Groups[13].Value, out r);
                    item.J2n = r;
                    r = 0M;
                    decimal.TryParse(match.Groups[16].Value, out r);
                    item.Clyl = r;


                    result.Add(item);
                }

            }
            return result;
        }



        private void dataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            using (SolidBrush b = new SolidBrush(this.dataGridView1.RowHeadersDefaultCellStyle.ForeColor))
            {
                e.Graphics.DrawString(Convert.ToString(e.RowIndex + 1, CultureInfo.CurrentUICulture),
                    e.InheritedRowStyle.Font, b, e.RowBounds.Location.X + 10, e.RowBounds.Location.Y + 4);
            }
        }



        private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            int index = e.ColumnIndex;
            if (asc)
            {
                switch (index)
                {
                    case 0:
                        FundEntity.Instance.Pool = FundEntity.Instance.Pool.OrderByDescending(w => w.Code).ToList();
                        break;
                    case 1:
                        FundEntity.Instance.Pool = FundEntity.Instance.Pool.OrderByDescending(w => w.Jjmc).ToList();
                        break;
                    case 5:
                        FundEntity.Instance.Pool = FundEntity.Instance.Pool.OrderByDescending(w => w.J1z).ToList();
                        break;
                    case 6:
                        FundEntity.Instance.Pool = FundEntity.Instance.Pool.OrderByDescending(w => w.J1gy).ToList();
                        break;
                    case 7:
                        FundEntity.Instance.Pool = FundEntity.Instance.Pool.OrderByDescending(w => w.J3gy).ToList();
                        break;
                    case 8:
                        FundEntity.Instance.Pool = FundEntity.Instance.Pool.OrderByDescending(w => w.J6gy).ToList();
                        break;
                    case 9:
                        FundEntity.Instance.Pool = FundEntity.Instance.Pool.OrderByDescending(w => w.J1n).ToList();
                        break;
                    case 10:
                        FundEntity.Instance.Pool = FundEntity.Instance.Pool.OrderByDescending(w => w.J2n).ToList();
                        break;
                    case 11:
                        FundEntity.Instance.Pool = FundEntity.Instance.Pool.OrderByDescending(w => w.Clyl).ToList();
                        break;
                }
                asc = false;
            }
            else
            {
                switch (index)
                {
                    case 0:
                        FundEntity.Instance.Pool = FundEntity.Instance.Pool.OrderBy(w => w.Code).ToList();
                        break;
                    case 1:
                        FundEntity.Instance.Pool = FundEntity.Instance.Pool.OrderBy(w => w.Jjmc).ToList();
                        break;
                    case 5:
                        FundEntity.Instance.Pool = FundEntity.Instance.Pool.OrderBy(w => w.J1z).ToList();
                        break;
                    case 6:
                        FundEntity.Instance.Pool = FundEntity.Instance.Pool.OrderBy(w => w.J1gy).ToList();
                        break;
                    case 7:
                        FundEntity.Instance.Pool = FundEntity.Instance.Pool.OrderBy(w => w.J3gy).ToList();
                        break;
                    case 8:
                        FundEntity.Instance.Pool = FundEntity.Instance.Pool.OrderBy(w => w.J6gy).ToList();
                        break;
                    case 9:
                        FundEntity.Instance.Pool = FundEntity.Instance.Pool.OrderBy(w => w.J1n).ToList();
                        break;
                    case 10:
                        FundEntity.Instance.Pool = FundEntity.Instance.Pool.OrderBy(w => w.J2n).ToList();
                        break;
                    case 11:
                        FundEntity.Instance.Pool = FundEntity.Instance.Pool.OrderBy(w => w.Clyl).ToList();
                        break;
                }
                asc = true;
            }

            dataGridView1.DataSource = FundEntity.Instance.Pool;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Load2();
        }

        private void Load2()
        {
            if (HisValue.List == null || HisValue.List.Count == 0)
            {
                HisValue.ReLoad();
            }

            this.dataGridView2.DataSource = HisValue.List.OrderBy(r => r.symbol).ThenByDescending(r => r.fbrq).ToList();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            HisValue.List = new List<HisValue>();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Computer.ComputingCorrelation(this.textBox3.Text, this.textBox9.Text);
        }

        private List<string> l1;
        private List<string> l2;
        private List<string> l3;
        private List<string> l4;

        private void button6_Click(object sender, EventArgs e)
        {
            l1 = Computer.SelectCorrelation.Where(r => r.symbolX == this.textBox11.Text.Trim()).Select(r => r.symbolY).ToList();
            this.textBox10.Text = string.Join(",", l1);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            this.l2 = Computer.SelectCorrelation.Where(r => r.symbolX == this.textBox12.Text.Trim() && l1.Contains(r.symbolY)).Select(r => r.symbolY).ToList();
            this.textBox13.Text = string.Join(",", l2);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            this.l3 = Computer.SelectCorrelation.Where(r => r.symbolX == this.textBox14.Text.Trim() && l2.Contains(r.symbolY)).Select(r => r.symbolY).ToList();
            this.textBox15.Text = string.Join(",", l3);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            this.l4 = Computer.SelectCorrelation.Where(r => r.symbolX == this.textBox12.Text.Trim() && l3.Contains(r.symbolY)).Select(r => r.symbolY).ToList();
            this.textBox17.Text = string.Join(",", l4);
        }

        private void button10_Click(object sender, EventArgs e)
        {

        }

        private void button11_Click(object sender, EventArgs e)
        {
            this.dataGridView1.DataSource =
                FundEntity.Instance.Pool.Where(r => this.textBox20.Text.Contains(r.Code)).ToList();
        }
    }
}
