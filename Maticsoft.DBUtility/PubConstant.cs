using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maticsoft.DBUtility
{
   /// <summary>
   /// 
   /// </summary>
   public class PubConstant
    {
        /// <summary>
        /// 获取指定Setting key的链接字符串
        /// </summary>
        /// <param name="configName"></param>
        /// <returns></returns>
        public static string GetConnectionString(string configName)
        {
            string text = ConfigurationManager.AppSettings[configName];
            string str2 = ConfigurationManager.AppSettings["ConStringEncrypt"];
            if (str2 == "true")
            {
                text = DesEncrypt.Decrypt(text);
            }
            return text;
        }

        /// <summary>
        /// 
        /// </summary>
        public static string ConnectionString
        {
            get
            {
                string text = ConfigurationManager.AppSettings["ConnectionString"];
                string str2 = ConfigurationManager.AppSettings["ConStringEncrypt"];
                if (str2 == "true")
                {
                    text = DesEncrypt.Decrypt(text);
                }
                return text;
            }
        }
    }
}
