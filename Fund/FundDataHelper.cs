// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpHelper.cs" company="" author="何翔华">
//   
// </copyright>
// <summary>
//   基金请求帮助类
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Fund
{
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// 基金请求帮助类
    /// </summary>
    public static class FundDataHelper
    {
        public static string GetDataFromUrl(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }

        public static string RegexMatch(string source, string matchText)
        {
            var regex = new Regex(matchText);
            return regex.Match(source).Groups[1].Value;
        }

        public static List<string> RegexMatches(string source, string matchText)
        {
            var regex = new Regex(matchText);
            var list = new List<string>();
            foreach (Match item in regex.Matches(source))
            {
                list.Add(item.Groups[1].Value);
            }
            return list;
        }
    }
}