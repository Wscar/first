using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 毕业设计2
{
 public static  class AppConfig
    {
        public static Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        /// <summary>
        /// 获得当前的下载目录
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetFolderValue(string key)
        {
            string result = string.Empty;
            if (config.AppSettings.Settings[key] != null)
            {
                result = config.AppSettings.Settings[key].Value;
            }
            return result;
        }
        public static void SetValue(string key, string value)
        {
            if (config.AppSettings.Settings[key] != null)
            {
                config.AppSettings.Settings[key].Value = value;
            }
        }
    }
}
