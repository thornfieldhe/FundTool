// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SharpeRatio.cs" company="" author="何翔华">
//   
// </copyright>
// <summary>
//   SharpeRatio
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Fund
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    /// <summary>
    /// 夏普系数
    /// </summary>
    public class SharpeRatio
    {
        /// <summary>
        /// 代码
        /// </summary>
        public string symbol
        {
            get; set;
        }


        /// <summary>
        /// 平均值
        /// </summary>
        public string Average
        {
            get; set;
        }

        /// <summary>
        /// 标准差
        /// </summary>
        public string Stdevpa
        {
            get; set;
        }


        /// <summary>
        /// 标准差上限
        /// </summary>
        public string UpRate
        {
            get; set;
        }


        /// <summary>
        /// 标准差下限
        /// </summary>
        public string DownRate
        {
            get; set;
        }

        /// <summary>
        /// 夏普系数
        /// </summary>
        public string Ratio
        {
            get; set;
        }

        public static List<SharpeRatio> GetFromPriceLimit(DataTable dt, string rfTxt)
        {
            double rf = 0.0;
            double.TryParse(rfTxt, out rf);
            var result = new List<SharpeRatio>();
            if (dt == null || dt.Rows.Count == 0)
            {
                return result;
            }

            for (int i = 1; i < dt.Columns.Count; i++)
            {
                var sharp = new SharpeRatio() { symbol = dt.Columns[i].ToString() };
                var values = new List<double>();
                for (int l = 0; l < dt.Rows.Count; l++)
                {
                    values.Add(double.Parse(dt.Rows[l][i].ToString()));
                    var average = values.Average();
                    var stdevpa = GetStdevpa(values);
                    sharp.Average = average.ToString("0.00");
                    sharp.Stdevpa = stdevpa.ToString("0.00");
                    sharp.UpRate = (average + stdevpa).ToString("0.00");
                    sharp.DownRate = (average - stdevpa).ToString("0.00");
                    sharp.Ratio = ((average - rf) / stdevpa).ToString("0.00");
                }

                result.Add(sharp);
            }

            return result.OrderByDescending(r => r.Ratio).ToList();
        }

        public static double GetStdevpa(List<double> values)
        {
            var average = values.Average();

            return Math.Sqrt(values.Sum(r => Math.Pow(r - average, 2)) / (values.Count - 1));
        }

    }
}