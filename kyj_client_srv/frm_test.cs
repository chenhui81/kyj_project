using kyj_project.Common;
using System;
using System.Windows.Forms;

namespace kyj_client_srv
{
    public partial class frm_test : Form
    {
        public frm_test()
        {
            InitializeComponent();
        }

        private void frm_test_Load(object sender, EventArgs e)
        {
            string s1 = "123456.789";
            string s2 = "3.1425926E+8";

            decimal d1 = Utility.ToDecimal(s1);
            decimal d2 = Utility.ToDecimal(s2);
            decimal d3 = Utility.ToDecimal(Utility.ToDouble(s1));
            decimal d4 = Utility.ToDecimal(Utility.ToDouble(s2));

        }
        /// <summary>
        /// 获取一个随机数
        /// </summary>
        /// <returns>返回一个1到100之间的随机整数</returns>
        private static readonly Random _random = new Random();
        private static readonly object _lockObj = new object();

        private int GetRdm()
        {
            // 确保线程安全地生成随机数
            lock (_lockObj)
            {
                // 生成一个1到100之间的随机整数
                return _random.Next(1, 101);
            }
        }


    }
}
