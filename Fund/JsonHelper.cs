// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonHelper.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the JsonHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Fund
{
    using System.IO;
    using System.Text;

    using Newtonsoft.Json;

    public static class JsonHelper<T> where T : class, new()
    {
        private static readonly string Path = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

        public static void Serialize(T list, string fileName)
        {
            using (
                var stream = File.Open(Path + fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                var json = JsonConvert.SerializeObject(list);
                var bytes = Encoding.UTF8.GetBytes(json);
                stream.Write(bytes, 0, bytes.Length);
                stream.Close();
            }
        }

        public static T DeSerialize(string fileName)
        {
            if (File.Exists(Path + fileName))
            {
                using (var stream = File.Open(Path + fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    var bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, bytes.Length);
                    var json = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                    var t = JsonConvert.DeserializeObject<T>(json);
                    stream.Close();
                    return t;
                }
            }

            return null;
        }
    }
}