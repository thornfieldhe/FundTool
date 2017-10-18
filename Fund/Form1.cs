using System;
using System.Linq;
using System.Windows.Forms;

namespace Fund
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FundEntity.Load();
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
                FundEntity.Instance.Pool = list1.Intersect(list2).Intersect(list3).Intersect(list4).Intersect(list5)
                    .Intersect(list6).ToList();
                FundEntity.SaveConfig();
            }

            MessageBox.Show(FundEntity.Instance.Pool.Count.ToString());
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
            FundEntity.Instance.Pool = new List<string>();
        }
    }
}
