using kyj_project.DAL;
using S7.Net;
using System;
using System.Windows.Forms;

namespace kyj_project
{
    public partial class frm_plc_test : Form
    {

        Plc _plc;

        public frm_plc_test()
        {
            InitializeComponent();
            this.Text = biz_cls.xiangmu_mingcheng;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (s7_cls.s7_check_constr(this.textBox1.Text) != "")
                {
                    this.lb_zt.Text = "PLC连接字符串格式不正确";
                    return;
                }

                _plc = s7_cls.get_plc(this.textBox1.Text);
                if (_plc != null)
                {
                    _plc.Open();
                    this.lb_zt.Text = "PLC已连接";
                }
                else
                {
                    this.lb_zt.Text = "PLC连接失败，请检查连接字符串";
                }

            }
            catch (Exception ex)
            {
                this.lb_zt.Text = ex.Message;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                _plc.Close();
                this.lb_zt.Text = "PLC已断开";
            }
            catch (Exception ex)
            {
                this.lb_zt.Text = ex.Message;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (s7_cls.s7_check_dizhi(this.textBox2.Text) != "")
                {
                    this.lb_zt.Text = "模块地址格式不正确";
                    return;
                }

                string s = s7_cls.get_plc_value(_plc, this.textBox2.Text);
                this.lb_zt.Text = "读取值：" + s;
            }
            catch (Exception ex)
            {
                this.lb_zt.Text = ex.Message;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                s7_cls.set_plc_value(_plc, this.textBox2.Text, this.textBox4.Text);
                this.lb_zt.Text = "PLC写入完成";
            }
            catch (Exception ex)
            {
                this.lb_zt.Text = ex.Message;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
