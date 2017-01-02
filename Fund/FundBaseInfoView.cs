// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FundBaseInfo.cs" company="" author="何翔华">
// </copyright>
// <summary>
//   FundBaseInfo
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Fund
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;

    using AutoMapper;

    using Newtonsoft.Json;

    /// <summary>
    /// 基础信息列表
    /// </summary>
    public class FundBaseInfoView
    {
        /// <summary>
        /// 代码
        /// </summary>
        public string symbol
        {
            get; set;
        }

        public string sname
        {
            get; set;
        }

        public string per_nav
        {
            get; set;
        }

        public string total_nav
        {
            get; set;
        }

        /// <summary>
        /// 近3个月
        /// </summary>
        public string three_month
        {
            get; set;
        }

        /// <summary>
        /// 近6个月
        /// </summary>
        public string six_month
        {
            get; set;
        }

        /// <summary>
        /// 近1年
        /// </summary>
        public string one_year
        {
            get; set;
        }

        public string form_year
        {
            get; set;
        }

        /// <summary>
        /// 成立以来
        /// </summary>
        public string form_start
        {
            get; set;
        }

        public string name
        {
            get; set;
        }

        public string zmjgm
        {
            get; set;
        }

        public string clrq
        {
            get; set;
        }

        public string jjjl
        {
            get; set;
        }

        public string dwjz
        {
            get; set;
        }

        public string ljjz
        {
            get; set;
        }

        public string jzrq
        {
            get; set;
        }

        public string zjzfe
        {
            get; set;
        }

        public string jjglr_code
        {
            get; set;
        }
    }
}