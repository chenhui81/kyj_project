using kyj_project.Common;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace kyj_project
{
    public partial class timer_test : Form
    {
        private System.Threading.Timer _timer;

        public timer_test()
        {
            // 创建一个间隔为1秒的Timer
            _timer = new System.Threading.Timer(
                async _ => await DoAsyncWork(),
                null,
                0,
                1000);


        }

        private async System.Threading.Tasks.Task DoAsyncWork()
        {
            // 异步工作，不会阻塞UI线程
            await Task.Run(() =>
            {
                // 长时间运行的任务
                if (DateTime.Now.Second == 0)
                {
                    MySqlHelper.ExecuteSql("insert into test_tb values (0,'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','标识1')");
                }

                if (DateTime.Now.Second == 10)
                {
                    MySqlHelper.ExecuteSql("insert into test_tb values (0,'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','标识2')");
                }

                if (DateTime.Now.Second == 20)
                {
                    MySqlHelper.ExecuteSql("insert into test_tb values (0,'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','标识3')");
                }

                if (DateTime.Now.Second == 30)
                {
                    MySqlHelper.ExecuteSql("insert into test_tb values (0,'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','标识4')");
                }
            });
        }

    }
}
