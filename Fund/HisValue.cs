// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HisValue.cs" company="" author="何翔华">
//   
// </copyright>
// <summary>
//   基金历史净值
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Fund
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using Newtonsoft.Json;

    using TAF.Utility;

    /// <summary>
    /// 基金历史净值
    /// </summary>
    public class HisValue
    {
        private static readonly string fileName = "his.json";

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
        public string symbol { get; set; }


        public DateTime fbrq { get; set; }


        /// <summary>
        /// 基金净值
        /// </summary>
        public double jjjz { get; set; }

        public static List<HisValue> Load()
        {
            List = new List<HisValue>();

            if (File.Exists(fileName))
            {
                using (FileStream stream = File.Open(
                    fileName,
                    System.IO.FileMode.Open,
                    FileAccess.ReadWrite,
                    FileShare.ReadWrite))
                {
                    byte[] bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, bytes.Length);
                    string json = UTF8Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                    var result = JsonConvert.DeserializeObject<List<HisValue>>(json);
                    stream.Close();
                    List = result.OrderBy(r => r.symbol).ToList();
                }
            }
            return List;
        }

        public static List<HisValue> ReLoad()
        {
            var url =
                    "http://stock.finance.sina.com.cn/fundInfo/api/openapi.php/CaihuiFundInfoService.getNav?callback=jQuery111208704630140164356_1481113132542&symbol={0}&datefrom={1}&dateto={2}&page=1&num=300"
                ;


            List = new List<HisValue>();
            var symbols = FundEntity.Instance.Pool.Select(r => r.Code).ToList();

            Parallel.ForEach(
                symbols,
                item =>
                    {
                        var newUrl = string.Format(
                            url,
                            item,
                            DateTime.Today.AddDays(-300).ToString("yyyy-MM-dd"),
                            DateTime.Today.ToString("yyyy-MM-dd"));
                        var wc = new WebClient();
                        var bHtml = wc.DownloadData(newUrl);
                        var strHtml = Encoding.GetEncoding("GBK").GetString(bHtml);
                        strHtml = Regex.Replace(strHtml, @"(.*?)\n(.*?)data""\:\[(.*?)\](.*?)$", @"[${3}]");
                        var temp = JsonConvert.DeserializeObject<List<HisValue>>(strHtml);

                        temp = temp.OrderByDescending(r => r.fbrq).Take(300).ToList();
                        lock (List)
                        {
                            Parallel.ForEach(temp, s => s.symbol = item);
                            List.AddRange(temp);
                        }



                    });

            List = List.OrderByDescending(r => r.fbrq).ToList();
            using (FileStream stream = File.Open(fileName, System.IO.FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                string json = JsonConvert.SerializeObject(List);
                byte[] bytes = UTF8Encoding.UTF8.GetBytes(json);
                stream.Write(bytes, 0, bytes.Length);
                stream.Close();
            }
            return List.OrderBy(r => r.symbol).ToList();
        }

    }

}