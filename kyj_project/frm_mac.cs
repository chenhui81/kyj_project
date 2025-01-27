using kyj_project.DAL;
using System;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows.Forms;

namespace kyj_project
{
    public partial class frm_mac : Form
    {
        public frm_mac()
        {
            InitializeComponent();
            this.Text = biz_cls.xiangmu_mingcheng;


        }

        private void frm_mac_Load(object sender, EventArgs e)
        {


            StringBuilder sb = new StringBuilder();
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up)
                {
                    string macAddress = nic.GetPhysicalAddress().ToString();
                    sb.Append(macAddress + "\r\n");
                }
            }
            this.textBox1.Text = sb.ToString();
        }
    }
}
