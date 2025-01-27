using kyj_project.Common;
using kyj_project.DAL;
using System;
using System.Data;
using System.Windows.Forms;

namespace kyj_project
{
    public partial class frm_login : Form
    {
        public frm_login()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void btn_login_Click(object sender, EventArgs e)
        {
            this.login();
        }

        private void login()
        {
            try
            {
                if (this.txt_tel.Text == "")
                {
                    MessageBox.Show("登陆账号不能为空！");
                    return;
                }

                if (this.txt_pwd.Text == "")
                {
                    MessageBox.Show("登陆密码不能为空！");
                    return;
                }

                if (this.txt_pwd.Text.Length < 6)
                {
                    MessageBox.Show("密码错误！");
                    return;
                }



                DataSet ds = new DataSet();
                ds = MySqlHelper.Get_DataSet("select * from uv_zh_yonghu where (yonghu_dianhua='" + this.txt_tel.Text + "' or yonghu_xingming='" + this.txt_tel.Text + "') and login_pwd='" + this.txt_pwd.Text + "'");
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];

                    biz_cls.yonghu_name = Utility.ToObjectString(dr["yonghu_xingming"]);
                    biz_cls.juese_id = Utility.ToObjectString(dr["juese_id"]);
                    biz_cls.juese_name = Utility.ToObjectString(dr["juese_name"]);

                    frm_main f = new frm_main
                    {
                        Text = this.lb_mingcheng.Text
                    };
                    f.lb_title.Text = this.lb_mingcheng.Text;
                    f.lb_yonghu.Text = "登陆用户： " + Utility.ToObjectString(dr["yonghu_xingming"]);
                    f.lb_shijian.Text = "登陆时间： " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    f.Show();
                    // Application.Run(new frm_index());
                    this.Hide();
                    //  this.Close();
                }
                else
                {
                    MessageBox.Show("账号或密码错误！");
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("mysql"))
                {
                    MessageBox.Show("登陆失败，请确认网络是否连接！");
                }
                else
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void txt_pwd_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                this.login();
            }
        }

        private void txt_tel_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                this.txt_pwd.Focus();
            }
        }

        private void frm_login_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Environment.Exit(0);
            Application.Exit();
        }

        private void frm_login_Load(object sender, EventArgs e)
        {
            biz_cls.xiangmu_mingcheng = Utility.ToObjectString(MySqlHelper.Get_sigle("select xiangmu_mingcheng from base_xiangmu limit 1"));
            this.lb_mingcheng.Text = biz_cls.xiangmu_mingcheng;
            this.Text = biz_cls.xiangmu_mingcheng;
            this.txt_tel.Focus();
        }
    }
}
