// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WatcherConfig.cs" company="" author="何翔华">
//   
// </copyright>
// <summary>
//   WatcherConfig
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Fund
{
    /// <summary>
    /// 基金监视器配置文件
    /// </summary>
    public class WatcherConfig
    {
        public static readonly string FileName = "watcher.json";

        public string Url
        {
            get; set;
        }

        public string TimeSpan
        {
            get; set;
        }

        public string UpRate
        {
            get; set;
        }

        public string DownRate
        {
            get; set;
        }

        public string AlertTime
        {
            get; set;
        }

        public string Codes
        {
            get; set;
        }


        public static WatcherConfig Load()
        {
            return JsonHelper<WatcherConfig>.DeSerialize(FileName);
        }

        public static void Save(string list, string url, string rateUp, string rateDown, string timespan, string alarm)
        {
            var config = new WatcherConfig
            {
                DownRate = rateDown,
                UpRate = rateUp,
                TimeSpan = timespan,
                Url = url,
                AlertTime = alarm,
                Codes = list
            };
            JsonHelper<WatcherConfig>.Serialize(config, FileName);
        }
    }
}