// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FundEntity.cs" company="" author="何翔华">
//   
// </copyright>
// <summary>
//   基金池信息对象
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Fund
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    using Newtonsoft.Json;

    using TAF.Utility;

    /// <summary>
    /// 基金池信息对象
    /// </summary>
    public class FundEntity : SingletonBase<FundEntity>
    {
        private static readonly string fileName = "fundEntity.json";

        /// <summary>
        /// 基金总条数
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 基金池
        /// </summary>
        public List<FundSimpleListItem> Pool { get; set; }

        public static void SaveConfig()
        {
            using (FileStream stream = File.Open(fileName, System.IO.FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                string json = JsonConvert.SerializeObject(Instance);
                byte[] bytes = UTF8Encoding.UTF8.GetBytes(json);
                stream.Write(bytes, 0, bytes.Length);
                stream.Close();
            }
        }

        public static void Load()
        {
            if (File.Exists(fileName))
            {
                using (FileStream stream = File.Open(fileName, System.IO.FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    byte[] bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, bytes.Length);
                    string json = UTF8Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                    var result = JsonConvert.DeserializeObject<FundEntity>(json);
                    stream.Close();
                    Instance.Total = result.Total;
                    Instance.Pool = result.Pool;
                }
            }
        }
    }
}