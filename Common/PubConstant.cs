using System.Configuration;


namespace kyj_project.Common
{

    public class PubConstant
    {
        /// <summary>
        /// 获取连接字符串
        /// </summary>
        public static string ConnectionString
        {
            get
            {
                if (ConfigurationManager.AppSettings["ConnectionString"] != null)
                {
                    string _connectionString = ConfigurationManager.AppSettings["ConnectionString"];
                    string ConStringEncrypt = ConfigurationManager.AppSettings["ConStringEncrypt"];
                    if (ConStringEncrypt == "true")
                    {
                        _connectionString = DESEncrypt.Decode(_connectionString);
                    }
                    return _connectionString;
                }
                else
                {
                    string _connectionString = ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString(); ;
                    string ConStringEncrypt = ConfigurationManager.ConnectionStrings["ConStringEncrypt"].ToString(); ;
                    if (ConStringEncrypt == "true")
                    {
                        _connectionString = DESEncrypt.Decode(_connectionString);
                    }
                    return _connectionString;
                }

            }
        }




        ///// <summary>
        ///// 得到web.config里配置项的数据库连接字符串。
        ///// </summary>
        ///// <param name="configName"></param>
        ///// <returns></returns>
        //public static string GetConnectionString(string configName)
        //{
        //    string connectionString = ConfigurationManager.AppSettings[configName];
        //    string ConStringEncrypt = ConfigurationManager.AppSettings["ConStringEncrypt"];
        //    if (ConStringEncrypt == "true")
        //    {
        //        connectionString = DESEncrypt.Decrypt(connectionString);
        //    }
        //    return connectionString;
        //}


    }
}
