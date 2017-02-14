// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Rsi.cs" company="" author="何翔华">
//   
// </copyright>
// <summary>
//   Rsi
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Fund
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// 相对强弱指标
    /// </summary>
    public class Rsi
    {
        public static readonly string FileName = "rsi.json";

        public static List<Rsi> List
        {
            get; set;
        }

        public string Code
        {
            get; set;
        }

        public DateTime Date
        {
            get; set;
        }

        public double Value
        {
            get; set;
        }

        public static List<Rsi> Load()
        {
            List = JsonHelper<List<Rsi>>.DeSerialize(FileName);
            return List;
        }

        public static List<Rsi> Comput(string downTxt, string nTxt)
        {
            var down = 25.0;
            var n = 9;
            double.TryParse(downTxt, out down);
            int.TryParse(nTxt, out n);
            List = JsonHelper<List<Rsi>>.DeSerialize(FileName);
            if (List == null || List.Count == 0)
            {
                List = new List<Rsi>();
                var stocks = Stock.Stocks.Where(r => r.Date.AddDays(20) > DateTime.Today).ToList();
                var his = Stock.Stocks.Where(r => r.Date.AddDays(40) > DateTime.Today).ToList();
                foreach (var stock in stocks)
                {
                    var list = Stock.GetCycleList(his.Where(r => r.Code == stock.Code && r.Date <= stock.Date).ToList(), n + 1);
                    if (list.Count > 0)
                    {
                        var high = 0.0;
                        var low = 0.0;
                        for (int i = 1; i < list.Count; i++)
                        {
                            if (list[i].Close > list[i - 1].Close)
                            {
                                high += list[i].Close - list[i - 1].Close;
                            }

                            if (list[i].Close < list[i - 1].Close)
                            {
                                low += list[i - 1].Close - list[i].Close;
                            }
                        }

                        var value = 100 - 100 / ((high / n) / (low / n) + 1);
                        if (value < down)
                        {
                            List.Add(new Rsi() { Date = stock.Date, Code = stock.Code, Value = value });
                        }
                    }
                }

                List =
                    List.OrderBy(r => r.Code)
                        .ThenByDescending(r => r.Date)
                        .GroupBy(r => r.Code)
                        .Select(r => r.First())
                        .ToList();

                JsonHelper<List<Rsi>>.Serialize(List, FileName);
            }

            return List;
        }
    }
}