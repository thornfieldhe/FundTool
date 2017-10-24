// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Correlation.cs" company="" author="何翔华">
//   
// </copyright>
// <summary>
//   相关性指标值
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Fund
{
    /// <summary>
    /// 相关性指标值
    /// </summary>
    public class Correlation
    {
        public string symbolX
        {
            get; set;
        }

        public string symbolY
        {
            get; set;
        }

        public double value
        {
            get; set;
        }
    }
}