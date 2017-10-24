// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Computer.cs" company="" author="何翔华">
//   
// </copyright>
// <summary>
//   计算器
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Fund
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    using Newtonsoft.Json;

    /// <summary>
    /// 计算器
    /// </summary>
    public class Computer
    {
        private const string Correlation = "correlatio.json";

        public static List<Correlation> SelectCorrelation
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
        /// <returns>
        /// The <see cref="List{T}"/>.
        /// </returns>
        public static List<Correlation> ComputingCorrelation(string fromTxt, string toTxt)
        {
            var from = -1.0;
            var to = 0.5;
            var take = 150;
            double.TryParse(fromTxt, out from);
            double.TryParse(toTxt, out to);

            var names = new List<string>();
            if (HisValue.List != null)
            {

                names = HisValue.List.Select(r => r.symbol).Distinct().ToList();

            }
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
            using (FileStream stream = File.Open(Correlation, System.IO.FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                string json = JsonConvert.SerializeObject(SelectCorrelation);
                byte[] bytes = UTF8Encoding.UTF8.GetBytes(json);
                stream.Write(bytes, 0, bytes.Length);
                stream.Close();
            }




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

    }
}