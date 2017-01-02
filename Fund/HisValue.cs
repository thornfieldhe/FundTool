// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HisValue.cs" company="" author="何翔华">
//   
// </copyright>
// <summary>
//   HisValue
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Fund
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using Newtonsoft.Json;

    /// <summary>
    /// 基金历史净值
    /// </summary>
    public class HisValue
    {
        public static readonly string FileName = "his.json";
        /// <summary>
        /// 已缓存列表
        /// </summary>
        public static List<HisValue> List
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

        public DateTime fbrq
        {
            get; set;
        }

        /// <summary>
        /// 基金净值
        /// </summary>
        public double jjjz
        {
            get; set;
        }

        /// <summary>
        /// 累计净值
        /// </summary>
        public double ljjz
        {
            get; set;
        }

        public static List<HisValue> Load()
        {
            List = JsonHelper<List<HisValue>>.DeSerialize(FileName);
            return List.OrderBy(r => r.symbol).ToList();
        }

        public static List<HisValue> ReLoad(string url, string daysTxt, string maxCountTxt, List<string> codes = null)
        {
            var days = 300;
            int.TryParse(daysTxt, out days);

            var maxCount = 200;
            int.TryParse(maxCountTxt, out maxCount);

            List = new List<HisValue>();
            var symbols = codes == null || codes.Count == 0 ? FundBaseInfo.List.Select(r => r.symbol).ToList() : codes;

            Parallel.ForEach(
               symbols,
                item =>
                    {
                        var newUrl = string.Format(
           url,
           item,
           DateTime.Today.AddDays(-days).ToString("yyyy-MM-dd"),
           DateTime.Today.ToString("yyyy-MM-dd"));
                        var wc = new WebClient();
                        var bHtml = wc.DownloadData(newUrl);
                        var strHtml = Encoding.GetEncoding("GBK").GetString(bHtml);
                        strHtml = Regex.Replace(strHtml, @"(.*?)\n(.*?)data""\:\[(.*?)\](.*?)$", @"[${3}]");
                        var temp = JsonConvert.DeserializeObject<List<HisValue>>(strHtml);
                        if (temp.Count > maxCount)
                        {
                            temp = temp.OrderByDescending(r => r.fbrq).Take(maxCount).ToList();
                            lock (List)
                            {
                                Parallel.ForEach(temp, s => s.symbol = item);
                                List.AddRange(temp);
                            }
                        }


                    });

            List = List.OrderByDescending(r => r.fbrq).ToList();
            JsonHelper<List<HisValue>>.Serialize(List, FileName);
            return List.OrderBy(r => r.symbol).ToList();
        }


        public static List<HisValue> Filter(string name)
        {
            return List.Where(r => r.symbol.Contains(name.Trim())).OrderBy(r => r.symbol).ToList();
        }
    }
}