// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Stock.cs" company="" author="何翔华">
// </copyright>
// <summary>
//   Stock
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Fund
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    /// <summary>
    /// </summary>
    public class Stock
    {
        public static readonly string FileName = "stock_list.json";
        public static readonly string FileName2 = "stock_Hislist.json";

        /// <summary>
        ///     代码列表
        /// </summary>
        public static List<string> Codes
        {
            get; set;
        }

        /// <summary>
        ///     历史价格列表
        /// </summary>
        public static List<Stock> Stocks
        {
            get; set;
        }

        /// <summary>
        ///     代码
        /// </summary>
        public string Code
        {
            get; set;
        }

        /// <summary>
        ///     日期
        /// </summary>
        public DateTime Date
        {
            get; set;
        }

        /// <summary>
        ///     开盘价
        /// </summary>
        public double Open
        {
            get; set;
        }

        /// <summary>
        ///     收盘价
        /// </summary>
        public double Close
        {
            get; set;
        }

        /// <summary>
        ///     最高价
        /// </summary>
        public double High
        {
            get; set;
        }

        /// <summary>
        ///     最低价
        /// </summary>
        public double Low
        {
            get; set;
        }

        /// <summary>
        ///     成交量
        /// </summary>
        public double Volume
        {
            get; set;
        }

        public static DataTable Load()
        {
            Codes = new List<string>();
            var table = JsonHelper<DataTable>.DeSerialize(FileName);
            foreach (DataRow row in table.Rows)
            {
                Codes.Add(row[0] as string);
            }

            return table;
        }

        public static List<Stock> LoadHis()
        {

            Stocks = JsonHelper<List<Stock>>.DeSerialize(FileName2);
            return Stocks;
        }

        public static DataTable GetList(string url, string maxTradeTxt)
        {
            var maxTrade = 20.0;
            var table = new DataTable();
            Codes = new List<string>();
            table.Columns.Add("代码");
            table.Columns.Add("名称");
            table.Columns.Add("现价");
            double.TryParse(maxTradeTxt, out maxTrade);
            var wc = new WebClient();
            var bHtml = wc.DownloadData(url);
            var strHtml = Encoding.GetEncoding("utf-8").GetString(bHtml);
            var reg = new Regex(@"""\d+\,(\d+),(.*?)\,(.*?)\,(.*?)""\,");
            var matches = reg.Matches(strHtml);
            foreach (Match match in matches)
            {
                var value = 0.0;
                double.TryParse(match.Groups[3].Value, out value);
                if (value > 0 && value < maxTrade)
                {
                    var row = table.NewRow();
                    row[0] = match.Groups[1].Value;
                    row[1] = match.Groups[2].Value;
                    row[2] = value;
                    table.Rows.Add(row);
                    Codes.Add(match.Groups[1].Value);
                }
            }

            JsonHelper<DataTable>.Serialize(table, FileName);
            return table;
        }

        /// <summary>
        /// 获取历史数据
        /// </summary>
        /// <param name="url">
        /// </param>
        /// <returns>
        /// </returns>
        public static List<Stock> GetHis(string[] url)
        {
            Stocks = new List<Stock>();
            var codes1 = Codes.Take(Codes.Count / 4).ToList();
            var codes2 = Codes.Skip(Codes.Count / 4).Take(Codes.Count / 4).ToList();
            var codes3 = Codes.Skip(Codes.Count / 4 * 2).Take(Codes.Count / 4).ToList();
            var codes4 = Codes.Skip(Codes.Count / 4 * 3).Take(Codes.Count / 4).ToList();
            var task1 = new Task(() =>
                                     {
                                         lock (Stocks)
                                         {
                                             Stocks.AddRange(
                                                 GetHis(
                                                     url[0],
                                                     codes1,
                                                     @"(\d{8})\,(.*?)\,(.*?)\,(.*?)\,(.*?)\,(.*?)\,(.*?)\,(.*?);",
                                                     GetHisFromTHSAPI));
                                         }
                                     });
            var task2 = new Task(() =>
                                     {
                                         lock (Stocks)
                                         {
                                             Stocks.AddRange(
                                                 GetHis(
                                                     url[1],
                                                     codes2,
                                                     @"\[""(\d{4}-\d{2}-\d{2})""\,""(.*?)""\,""(.*?)""\,""(.*?)""\,""(.*?)""\,""(.*?)""",
                                                     GetHisFromTXAPI));
                                         }
                                     });
            var task3 = new Task(() =>
            {
                lock (Stocks)
                {
                    Stocks.AddRange(
                        GetHis(
                            url[2],
                            codes3,
                            @"""date"":(.*?),(.*?)""open"":(.*?)\,""high"":(.*?)\,""low"":(.*?)\,""close"":(.*?)\,""volume"":(.*?)\,",
                            GetHisFromBDAPI));
                }
            });
            var task4 = new Task(() =>
            {
                lock (Stocks)
                {
                    Stocks.AddRange(
                        GetHis(
                            url[3],
                            codes4,
                            @"\[(\d{8}),(.*?),(.*?),(.*?),(.*?),(.*?),(.*?)\]",
                            GetHisFromQJAPI));
                }
            });
            task1.Start();
            task2.Start();
            task3.Start();
            task4.Start();
            Task.WaitAll(task1, task2, task3, task4);
            JsonHelper<List<Stock>>.Serialize(Stocks, FileName2);
            return Stocks;
        }

        /// <summary>
        ///     同花顺获取历史数据
        /// </summary>
        /// <param name="match"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        private static Stock GetHisFromTHSAPI(Match match, string code)
        {
            return new Stock
            {
                Code = code,
                Date =
                               new DateTime(
                               int.Parse(match.Groups[1].Value.Substring(0, 4)),
                               int.Parse(match.Groups[1].Value.Substring(4, 2)),
                               int.Parse(match.Groups[1].Value.Substring(6, 2))),
                Open = double.Parse(match.Groups[2].Value),
                High = double.Parse(match.Groups[3].Value),
                Low = double.Parse(match.Groups[4].Value),
                Close = double.Parse(match.Groups[5].Value),
                Volume = double.Parse(match.Groups[6].Value)
            };
        }

        /// <summary>
        ///     腾讯获取历史数据
        /// </summary>
        /// <param name="match"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        private static Stock GetHisFromTXAPI(Match match, string code)
        {
            return new Stock
            {
                Code = code,
                Date =
                               new DateTime(
                               int.Parse(match.Groups[1].Value.Substring(0, 4)),
                               int.Parse(match.Groups[1].Value.Substring(5, 2)),
                               int.Parse(match.Groups[1].Value.Substring(8, 2))),
                Open = double.Parse(match.Groups[2].Value),
                High = double.Parse(match.Groups[3].Value),
                Low = double.Parse(match.Groups[4].Value),
                Close = double.Parse(match.Groups[5].Value),
                Volume = double.Parse(match.Groups[6].Value)
            };
        }

        /// <summary>
        ///     百度获取历史数据
        /// </summary>
        /// <param name="match"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        private static Stock GetHisFromBDAPI(Match match, string code)
        {
            return new Stock
            {
                Code = code,
                Date =
                               new DateTime(
                               int.Parse(match.Groups[1].Value.Substring(0, 4)),
                               int.Parse(match.Groups[1].Value.Substring(4, 2)),
                               int.Parse(match.Groups[1].Value.Substring(6, 2))),
                Open = double.Parse(match.Groups[3].Value),
                High = double.Parse(match.Groups[4].Value),
                Low = double.Parse(match.Groups[5].Value),
                Close = double.Parse(match.Groups[6].Value),
                Volume = double.Parse(match.Groups[7].Value)
            };
        }

        /// <summary>
        ///     全景获取历史数据
        /// </summary>
        /// <param name="match"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        private static Stock GetHisFromQJAPI(Match match, string code)
        {
            return new Stock
            {
                Code = code,
                Date =
                               new DateTime(
                               int.Parse(match.Groups[1].Value.Substring(0, 4)),
                               int.Parse(match.Groups[1].Value.Substring(4, 2)),
                               int.Parse(match.Groups[1].Value.Substring(6, 2))),
                Open = double.Parse(match.Groups[2].Value),
                High = double.Parse(match.Groups[3].Value),
                Low = double.Parse(match.Groups[4].Value),
                Close = double.Parse(match.Groups[5].Value),
                Volume = double.Parse(match.Groups[6].Value)
            };
        }

        private static List<Stock> CreateList(IEnumerable<string> codes, string url, string regTxt, Func<Match, string, Stock> func)
        {
            List<Stock> list = new List<Stock>();
            Parallel.ForEach(
                codes,
                code =>
                    {
                        var newurl = string.Format(url, code);
                        var wc = new WebClient();
                        var bHtml = wc.DownloadData(newurl);
                        var strHtml = Encoding.GetEncoding("utf-8").GetString(bHtml);
                        var reg = new Regex(regTxt);
                        var matches = reg.Matches(strHtml);
                        foreach (Match match in matches)
                        {
                            lock (list)
                            {
                                list.Add(func(match, code));
                            }
                        }
                    });
            return list;
        }

        private static List<Stock> GetHis(string url, List<string> codes, string regTxt, Func<Match, string, Stock> func)
        {
            var list = new List<Stock>();
            var list1 = codes.Take(codes.Count / 10);
            var list2 = codes.Skip(codes.Count / 10).Take(codes.Count / 10);
            var list3 = codes.Skip(codes.Count / 10 * 2).Take(codes.Count / 10);
            var list4 = codes.Skip(codes.Count / 10 * 3).Take(codes.Count / 10);
            var list5 = codes.Skip(codes.Count / 10 * 4).Take(codes.Count / 10);
            var list6 = codes.Skip(codes.Count / 10 * 5).Take(codes.Count / 10);
            var list7 = codes.Skip(codes.Count / 10 * 6).Take(codes.Count / 10);
            var list8 = codes.Skip(codes.Count / 10 * 7).Take(codes.Count / 10);
            var list9 = codes.Skip(codes.Count / 10 * 8).Take(codes.Count / 10);
            var list10 = codes.Skip(codes.Count / 10 * 9).Take(codes.Count / 10);

            var task1 = new Task(() => list.AddRange(CreateList(list1, url, regTxt, func)));
            var task2 = new Task(() => list.AddRange(CreateList(list2, url, regTxt, func)));
            var task3 = new Task(() => list.AddRange(CreateList(list3, url, regTxt, func)));
            var task4 = new Task(() => list.AddRange(CreateList(list4, url, regTxt, func)));
            var task5 = new Task(() => list.AddRange(CreateList(list5, url, regTxt, func)));
            var task6 = new Task(() => list.AddRange(CreateList(list6, url, regTxt, func)));
            var task7 = new Task(() => list.AddRange(CreateList(list7, url, regTxt, func)));
            var task8 = new Task(() => list.AddRange(CreateList(list8, url, regTxt, func)));
            var task9 = new Task(() => list.AddRange(CreateList(list9, url, regTxt, func)));
            var task10 = new Task(() => list.AddRange(CreateList(list10, url, regTxt, func)));
            task1.Start();
            task2.Start();
            task3.Start();
            task4.Start();
            task5.Start();
            task6.Start();
            task7.Start();
            task8.Start();
            task9.Start();
            task10.Start();
            Task.WaitAll(task1, task2, task3, task4, task5, task6, task7, task8, task9, task10);
            return list;
        }
    }
}