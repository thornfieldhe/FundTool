// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Watcher.cs" company="" author="何翔华">
//   
// </copyright>
// <summary>
//   Watcher
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

    /// <summary>
    /// 
    /// </summary>
    public class FundWatcher
    {
        public static List<FundWatcher> Watchers
        {
            get; set;
        }

        /// <summary>
        /// 当前估值
        /// </summary>
        public double Value
        {
            get; set;
        }

        /// <summary>
        /// 昨日净值
        /// </summary>
        public double YesterdayValue
        {
            get; set;
        }

        /// <summary>
        /// 编码
        /// </summary>
        public string Symbol
        {
            get; set;
        }

        /// <summary>
        /// 报警次数
        /// </summary>
        public int AlertTimes
        {
            get; set;
        }



        public static List<string> Watch()
        {
            var result = new List<string>();
            var config = WatcherConfig.Load();
            var list = config.Codes.Split(',');
            if (Watchers == null)
            {
                Watchers = new List<FundWatcher>();
            }

            try
            {
                foreach (var item in list)
                {
                    var wc = new WebClient();
                    var bHtml = wc.DownloadData(string.Format(config.Url, item));
                    var strHtml = Encoding.GetEncoding("GBK").GetString(bHtml);
                    var yesValue = Regex.Match(strHtml, "\"pre\":\"(.*?)\"").Groups[1].Value;
                    var value = Regex.Match(strHtml, "\"value\":\"(.*?)\"").Groups[1].Value;
                    if (yesValue == "" && value == "")
                    {
                        continue;
                    }
                    var watcher = Watchers.FirstOrDefault(r => r.Symbol == item);
                    if (watcher == null)
                    {
                        watcher = new FundWatcher() { Value = double.Parse(value), Symbol = item, AlertTimes = 0, YesterdayValue = double.Parse(yesValue) };
                        Watchers.Add(watcher);
                    }
                    else
                    {
                        watcher.Value = double.Parse(value);
                        watcher.YesterdayValue = double.Parse(yesValue);
                    }

                    var rate = (watcher.Value - watcher.YesterdayValue) * 100 / watcher.YesterdayValue;
                    if (rate >= double.Parse(config.UpRate) || rate <= double.Parse(config.DownRate))
                    {
                        watcher.AlertTimes += 1;
                    }

                    if (watcher.AlertTimes >= int.Parse(config.AlertTime))
                    {
                        result.Add($"{item}:{rate}{(rate >= double.Parse(config.UpRate) ? "↑" : "↓")}");
                    }
                }
            }
            catch (Exception e)
            {

            }

            return result;
        }
    }
}