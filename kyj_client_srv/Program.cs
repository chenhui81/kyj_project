using System;
using System.Threading;
using System.Windows.Forms;

namespace kyj_client_srv
{
    static class Program
    {

        static Mutex mutex;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool createdNew;
            // 创建一个命名的互斥锁，"SingleInstanceAppMutex" 是这个互斥锁的名称，可根据需要修改
            mutex = new Mutex(true, "SingleInstanceAppMutex", out createdNew);
            if (!createdNew)
            {
                // 如果没有创建新的互斥锁，说明程序已经在运行
                //  MessageBox.Show("程序已经在运行，请不要重复打开。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            // 释放互斥锁资源
            mutex.ReleaseMutex();
        }
    }
}
