using kyj_project.Common;
using kyj_project.DAL;
using kyj_project.Models;
using System;
using System.Windows.Forms;

namespace kyj_project
{
    public partial class frm_zhandian_edit : Form
    {
        public string zhandian_id = "";//站点ID
        private en_zhandian en;
        public frm_zhandian_edit()
        {
            InitializeComponent();
            this.Text = biz_cls.xiangmu_mingcheng;
        }

        private void load_data()
        {
            if (zhandian_id == "")
            {
                //新增状态
                this.lb_title.Text = "站点新增";
                en = new en_zhandian();
            }
            else
            {
                //编辑状态
                this.lb_title.Text = "站点编辑";
                en = new en_zhandian();
                en = biz_cls.get_zhandian_byid(zhandian_id);
                this.txt_zhandian_id.Text = en.zhandian_id;
                this.txt_zhandian_mingcheng.Text = en.zhandian_mingcheng;
                this.txt_mubiaoyali.Text = Utility.ToObjectString(en.mubiaoyali);
                this.txt_url.Text = en.zutaitu_url;
                if (en.liandong_flag == 1)
                {
                    this.cb_liandong.Checked = true;
                }
                else
                {
                    this.cb_liandong.Checked = false;
                }
            }
        }

        private void frm_zhandian_edit_Load(object sender, EventArgs e)
        {
            this.load_data();
        }

        private bool check()
        {
            if (this.txt_zhandian_id.Text == "")
            {
                MessageBox.Show("站点ID不能位空");
                this.txt_zhandian_id.Focus();
                return false;
            }

            if (this.txt_zhandian_mingcheng.Text == "")
            {
                MessageBox.Show("站点名称不能位空");
                this.txt_zhandian_mingcheng.Focus();
                return false;
            }

            if (this.txt_mubiaoyali.Text == "")
            {
                MessageBox.Show("目标压力不能位空");
                this.txt_mubiaoyali.Focus();
                return false;
            }

            if (Utility.IsFloat(this.txt_mubiaoyali.Text) == false)
            {
                MessageBox.Show("目标压力数据类型不正确");
                this.txt_mubiaoyali.Focus();
                return false;
            }

            if (Utility.ToDecimal(this.txt_mubiaoyali.Text) <= 0)
            {
                MessageBox.Show("目标压力应大于0");
                this.txt_mubiaoyali.Focus();
                return false;
            }

            if (this.txt_url.Text == "")
            {
                MessageBox.Show("组态图URL地址不能位空");
                this.txt_url.Focus();
                return false;
            }

            return true;
        }

        private string add()
        {
            en.zhandian_id = this.txt_zhandian_id.Text;
            en.zhandian_mingcheng = this.txt_zhandian_mingcheng.Text;
            en.mubiaoyali = Utility.ToDecimal(this.txt_mubiaoyali.Text);
            en.zutaitu_url = this.txt_url.Text;
            if (this.cb_liandong.Checked == true)
            {
                en.liandong_flag = 1;
            }
            else
            {
                en.liandong_flag = 0;
            }

            return biz_cls.zhandian_add(en);
        }

        private string upd()
        {
            en.zhandian_id = this.txt_zhandian_id.Text;
            en.zhandian_mingcheng = this.txt_zhandian_mingcheng.Text;
            en.mubiaoyali = Utility.ToDecimal(this.txt_mubiaoyali.Text);
            en.zutaitu_url = this.txt_url.Text;
            if (this.cb_liandong.Checked == true)
            {
                en.liandong_flag = 1;
            }
            else
            {
                en.liandong_flag = 0;
            }

            return biz_cls.zhandian_upd(en);
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.check() == false) { return; }
                string err_str = "";

                if (zhandian_id == "")
                {
                    err_str = this.add();
                }
                else
                {
                    err_str = this.upd();
                }

                if (err_str != "")
                {
                    MessageBox.Show(err_str);
                }
                else
                {
                    frm_zhandian f = (frm_zhandian)this.Owner;
                    f.Load_data();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
