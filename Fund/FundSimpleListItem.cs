// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FundSimpleListItem.cs" company="" author="何翔华">
//   
// </copyright>
// <summary>
//   基金列表对象
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Fund
{
    /// <summary>
    /// 基金列表对象
    /// </summary>
    public class FundSimpleListItem
    {
        /// <summary>
        /// 基金代码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 基金简称
        /// </summary>
        public string Jjmc { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// 单位净值
        /// </summary>
        public decimal Dwjz { get; set; }

        /// <summary>
        /// 日增长率
        /// </summary>
        public decimal Zf { get; set; }

        /// <summary>
        /// 近1周
        /// </summary>
        public decimal J1z { get; set; }

        /// <summary>
        /// 近1月
        /// </summary>
        public decimal J1gy { get; set; }

        /// <summary>
        /// 近3月
        /// </summary>
        public decimal J3gy { get; set; }

        /// <summary>
        /// 近6月
        /// </summary>
        public decimal J6gy { get; set; }

        /// <summary>
        /// 近1年
        /// </summary>
        public decimal J1n { get; set; }

        /// <summary>
        /// 近2年
        /// </summary>
        public decimal J2n { get; set; }

        /// <summary>
        /// 成立以来
        /// </summary>
        public decimal Clyl { get; set; }
    }
}