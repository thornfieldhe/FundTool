namespace Fund
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;

    using AutoMapper;

    using Newtonsoft.Json;

    public class FundBaseInfo
    {
        public static readonly string FileName = "total.json";

        /// <summary>
        /// 已缓存列表
        /// </summary>
        public static List<FundBaseInfo> List
        {
            get; set;
        }

        /// <summary>
        /// 代码
        /// </summary>
        public string symbol
        {
            get; set;
        }

        public string sname
        {
            get; set;
        }

        public string per_nav
        {
            get; set;
        }

        public string total_nav
        {
            get; set;
        }

        /// <summary>
        /// 近3个月
        /// </summary>
        public double three_month
        {
            get; set;
        }

        /// <summary>
        /// 近6个月
        /// </summary>
        public double six_month
        {
            get; set;
        }

        /// <summary>
        /// 近1年
        /// </summary>
        public double one_year
        {
            get; set;
        }

        public double form_year
        {
            get; set;
        }

        /// <summary>
        /// 成立以来
        /// </summary>
        public double form_start
        {
            get; set;
        }

        public string name
        {
            get; set;
        }

        public string zmjgm
        {
            get; set;
        }

        public string clrq
        {
            get; set;
        }

        public string jjjl
        {
            get; set;
        }

        public string dwjz
        {
            get; set;
        }

        public string ljjz
        {
            get; set;
        }

        public string jzrq
        {
            get; set;
        }

        public string zjzfe
        {
            get; set;
        }

        public string jjglr_code
        {
            get; set;
        }

        public static List<FundBaseInfo> Load()
        {
            List = JsonHelper<List<FundBaseInfo>
            >.DeSerialize(FileName);
            return List.OrderBy(r => r.symbol).ToList();
        }

        public static List<FundBaseInfo> ReLoad(string url, string l1, string l2, string l3, string l4)
        {
            var ll1 = 4;
            var ll2 = 4;
            var ll3 = 4;
            var ll4 = 4;
            int.TryParse(l1, out ll1);
            int.TryParse(l2, out ll2);
            int.TryParse(l3, out ll3);
            int.TryParse(l4, out ll4);
            var wc = new WebClient();
            var bHtml = wc.DownloadData(url);
            var strHtml = Encoding.GetEncoding("GBK").GetString(bHtml);
            strHtml = Regex.Replace(strHtml, @"(.*?)\n(.*?)data\:(.*?)\,exec_time(.*?)$", @"${3}");
            var list = Mapper.Map<List<FundBaseInfo>>(JsonConvert.DeserializeObject<List<FundBaseInfoView>>(strHtml));
            return Filter(list, ll1, ll2, ll3, ll4);
        }

        private static List<FundBaseInfo> Filter(List<FundBaseInfo> list, int l1, int l2, int l3, int l4)
        {
            list = list.Where(r => r.form_start > 0 && r.one_year >= 0 && r.six_month >= 0 && r.three_month >= 0).ToList();
            var list1 = list.OrderByDescending(r => r.one_year).Take(list.Count / l1).Select(r => r.symbol).ToList();
            var list2 = list.OrderByDescending(r => r.form_start).Take(list.Count / l2).Select(r => r.symbol).ToList();
            var list3 = list.OrderByDescending(r => r.six_month).Take(list.Count / l3).Select(r => r.symbol).ToList();
            var list4 = list.OrderByDescending(r => r.three_month).Take(list.Count / l4).Select(r => r.symbol).ToList();
            var list5 = list1.Intersect(list2).Intersect(list3).Intersect(list4);
            List = list.Where(r => list5.Contains(r.symbol)).OrderByDescending(r => r.three_month).ToList();
            JsonHelper<List<FundBaseInfo>>.Serialize(List, FileName);

            return List;
        }
    }
}