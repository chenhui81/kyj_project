using kyj_project.DAL;
using Modbus.Device;
using System;
using System.Windows.Forms;

namespace kyj_project
{
    public partial class frm_mbus_tcp_test : Form
    {

        ModbusMaster _mm;
        public frm_mbus_tcp_test()
        {
            InitializeComponent();
            this.Text = biz_cls.xiangmu_mingcheng;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                _mm = mtcp_cls.get_mtcp(this.textBox1.Text);
                this.lb_zt.Text = "MODBUS TCP已连接";
                _mm.Dispose();

            }
            catch (Exception ex)
            {
                this.lb_zt.Text = ex.Message;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //连接
            _mm = mtcp_cls.get_mtcp(this.textBox1.Text);

            //读取
            string s = mtcp_cls.get_mtcp_value(_mm, this.textBox2.Text);
            this.lb_zt.Text = "读取值：" + s;

            //断开
            _mm.Dispose();
        }

        private void frm_mbus_tcp_test_Load(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            //连接
            _mm = mtcp_cls.get_mtcp(this.textBox1.Text);

            //写入
            string s = mtcp_cls.set_mtcp_value(_mm, this.textBox2.Text, this.textBox4.Text);
            if (s == "")
            {
                this.lb_zt.Text = "写入成功";
            }
            else
            {
                this.lb_zt.Text = s;
            }

            //断开
            _mm.Dispose();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
