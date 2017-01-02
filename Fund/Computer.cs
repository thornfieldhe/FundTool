// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Computer.cs" company="" author="何翔华">
//   
// </copyright>
// <summary>
//   Computer
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Fund
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// 计算器
    /// </summary>
    public static class Computer
    {
        private const string HisValueTable = "table.json";

        private const string Correlation = "correlatio.json";

        public static List<Correlation> SelectCorrelation
        {
            get; set;
        }

        public static List<Tuple<string, double>> PriceLimit
        {
            get; set;
        }

        /// <summary>
        /// 计算相关性
        /// </summary>
        /// <param name="fromTxt">
        /// The from Txt.
        /// </param>
        /// <param name="toTxt">
        /// The to Txt.
        /// </param>
        /// <param name="takeTxt"></param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<Correlation> ComputingCorrelation(string fromTxt, string toTxt, string takeTxt)
        {
            var from = -1.0;
            var to = 0.5;
            var take = 150;
            double.TryParse(fromTxt, out from);
            double.TryParse(toTxt, out to);
            int.TryParse(takeTxt, out take);

            var names = HisValue.List.Select(r => r.symbol).Distinct().ToList();
            var result = new List<Tuple<string, string, double>>();
            foreach (var t in names)
            {
                foreach (var n in names)
                {
                    var x =
                        HisValue.List.Where(r => r.symbol == t).OrderByDescending(r => r.fbrq).Take(take);
                    var y =
                        HisValue.List.Where(r => r.symbol == n).OrderByDescending(r => r.fbrq).Take(take);
                    var item = ComputingCorrelation(
                        x.OrderByDescending(r => r.fbrq).Select(r => r.jjjz).ToArray(),
                        y.OrderByDescending(r => r.fbrq).Select(r => r.jjjz).ToArray(),
                        t,
                        n);
                    result.Add(item);
                }
            }

            var list = result.Select(r => new Correlation { symbolX = r.Item1, symbolY = r.Item2, value = r.Item3 }).ToList();
            JsonHelper<List<Correlation>>.Serialize(list, Correlation);




            var symbols = list.Select(r => r.symbolX).ToList();
            symbols.AddRange(list.Select(r => r.symbolY).ToList());
            var p = symbols.GroupBy(r => r).Select(r => new
            {
                Key = r,
                Count = r.Count()
            }).Select(r => r.Key.Key).ToList();

            list = list.Where(r => p.Contains(r.symbolX) || p.Contains(r.symbolY)).ToList();
            SelectCorrelation = list.Where(r => r.value > from && r.value < to).ToList();
            return SelectCorrelation;
        }




        public static DataTable BindCorrelationStatistics(List<Correlation> list)
        {
            var table = new DataTable();
            var header = list.Select(r => r.symbolX).ToList();
            header.AddRange(list.Select(r => r.symbolY).ToList());
            header = header.Distinct().OrderBy(r => r).ToList();
            table.Columns.Add("列");
            foreach (var col in header)
            {
                table.Columns.Add(col);
            }
            var row1 = table.NewRow();
            row1[0] = string.Empty;

            for (var i = 0; i < header.Count; i++)
            {
                row1[i + 1] = header[i];
            }

            table.Rows.Add(row1);

            for (var i = 0; i < header.Count; i++)
            {
                var row = table.NewRow();
                row[0] = header[i];
                for (var j = 0; j < header.Count; j++)
                {
                    var value = list.FirstOrDefault(r => r.symbolX == header[i] && r.symbolY == header[j]);
                    row[j + 1] = value?.value.ToString("0.00") ?? string.Empty;
                    if (header[i] == header[j])
                    {
                        row[j + 1] = "1";
                    }
                }

                table.Rows.Add(row);
            }

            return table;
        }

        /// <summary>
        /// 计算周期内涨跌幅度
        /// </summary>
        /// <param name="cycleTxt">
        /// </param>
        /// <param name="codeTxt">
        /// 周期天数
        /// </param>
        public static DataTable ComputingPriceLimit(string cycleTxt, string codeTxt)
        {
            var cycle = 7;
            int.TryParse(cycleTxt, out cycle);
            var table = new DataTable();
            var codes = codeTxt.Trim().Split(',');
            var hisValues = new List<HisValue>();
            table.Columns.Add("日期");
            if (codes.Length == 1 && codes[0] == string.Empty)
            {
                codes = HisValue.List.Select(r => r.symbol).Distinct().OrderBy(r => r).ToArray();
                hisValues = HisValue.List.OrderByDescending(r => r.fbrq).ToList();
            }
            else
            {
                hisValues = HisValue.List.Where(r => codes.Contains(r.symbol)).OrderByDescending(r => r.fbrq).ToList();
            }

            foreach (var code in codes)
            {
                table.Columns.Add(code);
            }

            var dates = hisValues.Where(r => r.symbol == codes[0]).Select(r => r.fbrq).OrderBy(r => r).ToList();
            var firstDate = dates.First();
            dates = dates.Where(r => ((TimeSpan)(r - firstDate)).Days % cycle == 0).ToList();
            var list = new List<Tuple<string, DateTime, double>>();
            foreach (var date in dates.Skip(1))
            {
                for (int i = 0; i < codes.Length; i++)
                {
                    var value = hisValues.SingleOrDefault(r => r.symbol == codes[i] && r.fbrq == date);
                    if (value != null)
                    {
                        var frontValue = AddItemToPriceLimit(codes[i], cycle, date, hisValues);

                        if (frontValue != null)
                        {
                            var limit = (value.ljjz - frontValue.ljjz) / frontValue.ljjz * 100;
                            list.Add(new Tuple<string, DateTime, double>(codes[i], date, limit));
                        }
                    }
                }
            }

            foreach (var date in dates.Skip(1))
            {
                var row = table.NewRow();
                row[0] = date.ToString("yyyy-MM-dd");
                for (int i = 0; i < codes.Length; i++)
                {
                    var value = list.SingleOrDefault(r => r.Item1 == codes[i] && r.Item2 == date);
                    if (value != null)
                    {
                        row[i + 1] = value.Item3.ToString("0.00");
                    }
                    else
                    {
                        row[i + 1] = 0;
                    }
                }
                table.Rows.Add(row);
            }

            return table;
        }

        private static List<Tuple<string, double>> GetStandardDeviation()
        {
            var result = new List<Tuple<string, double>>();
            return result;
        }

        private static HisValue AddItemToPriceLimit(string code, int cycle, DateTime date, List<HisValue> hisValues)
        {
            if (cycle >= 1000)
            {
                return null;
            }
            var frontValue = hisValues.SingleOrDefault(r => r.symbol == code && r.fbrq == date.AddDays(-cycle));
            if (frontValue != null)
            {
                return frontValue;
            }
            else
            {
                return AddItemToPriceLimit(code, cycle + 1, date, hisValues);
            }
        }

        /// <summary>
        /// 组装公式
        /// </summary>
        /// <param name="priceLimit"></param>
        /// <param name="minTxt"></param>
        /// <returns></returns>
        public static string PackagingFormula(DataTable priceLimit, string minTxt)
        {
            var str = new StringBuilder();
            if (string.IsNullOrWhiteSpace(minTxt))
            {
                str.Append("MAX=M;\r\n");
                for (int i = 0; i < priceLimit.Rows.Count; i++)
                {
                    var rowStr = new StringBuilder();
                    for (int j = 1; j < priceLimit.Columns.Count; j++)
                    {
                        rowStr.Append(priceLimit.Rows[i][j] + "*x" + j + "+");
                    }

                    str.Append(rowStr.ToString().Trim('+') + ">=M;\r\n");
                }
                str.Append("M>=0;\r\n");
                for (int j = 1; j < priceLimit.Columns.Count; j++)
                {
                    str.Append("x" + j + ">=0;\r\n");
                }

                var str2 = new StringBuilder();
                for (int j = 1; j < priceLimit.Columns.Count; j++)
                {
                    str2.Append("x" + j + "+");
                }
                str.Append(str2.ToString().Trim('+') + "=1;");
            }
            else
            {
                var min = 0D;
                double.TryParse(minTxt, out min);
                var total = new StringBuilder();
                for (int j = 1; j < priceLimit.Columns.Count; j++)
                {
                    total.Append(1.0 / (priceLimit.Columns.Count - 1) + "*x" + j + "+");
                }
                str.Append("MAX=" + total.ToString().Trim('+') + ";\r\n");
                for (int i = 0; i < priceLimit.Rows.Count; i++)
                {
                    var rowStr = new StringBuilder();
                    for (int j = 1; j < priceLimit.Columns.Count; j++)
                    {
                        rowStr.Append(priceLimit.Rows[i][j] + "*x" + j + "+");
                    }

                    str.Append(rowStr.ToString().Trim('+') + ">=" + min + ";\r\n");
                }
                for (int j = 1; j < priceLimit.Columns.Count; j++)
                {
                    str.Append("x" + j + ">=0;\r\n");
                }

                var str2 = new StringBuilder();
                for (int j = 1; j < priceLimit.Columns.Count; j++)
                {
                    str2.Append("x" + j + "+");
                }
                str.Append(str2.ToString().Trim('+') + "=1;");

            }

            return str.ToString();
        }

        /// <summary>
        /// 基金根据相关度分组
        /// 同一分组内5支基金相关度都低于0.5
        /// </summary>
        /// <param name="code1">
        /// </param>
        /// <param name="code2">
        /// </param>
        /// <param name="code3">
        /// </param>
        /// <param name="code4">
        /// </param>
        /// <param name="fromTxt">
        /// </param>
        /// <param name="toTxt">
        /// </param>
        /// <returns>
        /// </returns>
        public static List<Tuple<string, string>> BindCorrelationGroups(
            string code1,
            string code2,
            string code3,
            string code4,
            string fromTxt,
            string toTxt)
        {
            var list = new List<Tuple<string, List<string>>>();
            var newList = new List<Tuple<string, string>>();
            var from = -1.0;
            var to = 0.5;
            double.TryParse(fromTxt, out from);
            double.TryParse(toTxt, out to);
            var values = JsonHelper<List<Correlation>>
                    .DeSerialize(Correlation)
                    .Where(r => r != null && ((r.value >= from && r.value <= to) || r.value == 1));
            var names = values.Select(r => r.symbolX).Intersect(values.Select(r => r.symbolY)).Distinct();
            Parallel.ForEach(
                    names,
                    name =>
                        {
                            var x = name;
                            var y = values.Where(r => r.symbolX == x).Select(r => r.symbolY).Distinct().ToList();
                            y.Add(x);
                            y.Sort();
                            list.Add(new Tuple<string, List<string>>(x, y));
                        });
            if (!string.IsNullOrWhiteSpace(code1))
            {
                list = list.Where(r => r.Item2.Contains(code1.Trim())).ToList();
            }

            if (!string.IsNullOrWhiteSpace(code2))
            {
                list = list.Where(r => r.Item2.Contains(code2.Trim())).ToList();
            }

            if (!string.IsNullOrWhiteSpace(code3))
            {
                list = list.Where(r => r.Item2.Contains(code3.Trim())).ToList();
            }

            if (!string.IsNullOrWhiteSpace(code4))
            {
                list = list.Where(r => r.Item2.Contains(code4.Trim())).ToList();
            }

            newList = list.Select(r => new Tuple<string, string>(r.Item1, string.Join(",", r.Item2))).ToList();

            return newList;
        }

        public static DataTable LoadHisValueTable()
        {
            return JsonHelper<DataTable>.DeSerialize(HisValueTable);
        }

        public static DataTable CreateHisValueTable()
        {
            var date = HisValue.List.Select(r => r.fbrq).Distinct();
            var codes = HisValue.List.Select(r => r.symbol).Distinct().ToList();
            codes.Sort();

            var table = new DataTable();
            table.Columns.Add("日期");
            foreach (var code in codes)
            {
                table.Columns.Add(code);
            }
            foreach (var dateTime in date)
            {
                var row = table.NewRow();
                row[0] = dateTime.ToShortDateString();
                for (int i = 0; i < codes.Count; i++)
                {
                    var value = HisValue.List.FirstOrDefault(r => r.symbol == codes[i] && r.fbrq == dateTime);
                    row[i + 1] = value?.jjjz ?? 0;
                }
                table.Rows.Add(row);
            }
            JsonHelper<DataTable>.Serialize(table, HisValueTable);
            return table;
        }

        private static Tuple<string, string, double> ComputingCorrelation(IReadOnlyList<double> x, IReadOnlyList<double> y, string xName, string yName)
        {
            var n = x.Count();
            var sumX = x.Sum();
            var sumY = y.Sum();
            var sumXY = 0.0;
            for (var i = 0; i < x.Count(); i++)
            {
                sumXY += x[i] * y[i];
            }

            var powX = x.Sum(r => Math.Pow(r, 2));
            var powY = y.Sum(r => Math.Pow(r, 2));
            var result = (n * sumXY - sumX * sumY)
                     / (Math.Sqrt(n * powX - Math.Pow(sumX, 2))
                        * Math.Sqrt(n * powY - Math.Pow(sumY, 2)));

            return new Tuple<string, string, double>(xName, yName, result);
        }

        public class TupleComparer : IEqualityComparer<Tuple<string, string>>
        {
            public bool Equals(Tuple<string, string> x, Tuple<string, string> y)
            {
                if (x.Item1 == y.Item1 && x.Item2 == y.Item2)
                {
                    return true;
                }
                return false;
            }

            public int GetHashCode(Tuple<string, string> obj)
            {
                return obj.Item1.GetHashCode() + obj.Item2.GetHashCode();
            }
        }
    }

}