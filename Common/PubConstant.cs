using System.Configuration;


namespace kyj_project.Common
{

    public class PubConstant
    {
        /// <summary>
        /// ��ȡ�����ַ���
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
        ///// �õ�web.config������������ݿ������ַ�����
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
