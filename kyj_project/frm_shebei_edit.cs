using kyj_project.Common;
using kyj_project.DAL;
using kyj_project.Models;
using System;
using System.Data;
using System.Windows.Forms;

namespace kyj_project
{
    public partial class frm_shebei_edit : Form
    {

        public int shebei_id = 0;//设备ID
        private en_shebei en;
        public frm_shebei_edit()
        {
            InitializeComponent();
            this.Text = biz_cls.xiangmu_mingcheng;
        }

        #region  加载数据

        private void bind_shebei_leixing()
        {
            DataSet ds = new DataSet();
            ds = MySqlHelper.Get_DataSet("select shebei_leixing_id,shebei_leixing_mingcheng from base_shebei_leixing  order by paixuhao");
            DataRow dr = ds.Tables[0].NewRow();
            dr["shebei_leixing_id"] = "0";
            dr["shebei_leixing_mingcheng"] = "";
            ds.Tables[0].Rows.InsertAt(dr, 0);

            this.comb_shebei_leixing.DisplayMember = "shebei_leixing_mingcheng";
            this.comb_shebei_leixing.ValueMember = "shebei_leixing_id";
            this.comb_shebei_leixing.DataSource = ds.Tables[0];
        }

        private void bind_zhandian()
        {
            DataSet ds = new DataSet();
            ds = MySqlHelper.Get_DataSet("select zhandian_id,zhandian_mingcheng from base_zhandian  order by zhandian_id");
            DataRow dr = ds.Tables[0].NewRow();
            dr["zhandian_id"] = "0";
            dr["zhandian_mingcheng"] = "";
            ds.Tables[0].Rows.InsertAt(dr, 0);

            this.comb_zhandian.DisplayMember = "zhandian_mingcheng";
            this.comb_zhandian.ValueMember = "zhandian_id";
            this.comb_zhandian.DataSource = ds.Tables[0];
        }

        #endregion

        private void frm_shebei_edit_Load(object sender, EventArgs e)
        {


            this.bind_shebei_leixing();
            this.bind_zhandian();
            this.load_data();


        }

        private void load_data()
        {
            if (this.shebei_id == 0)
            {
                this.lb_title.Text = "设备新增";
                en = new en_shebei();
            }
            else
            {
                this.lb_title.Text = "设备编辑";
                en = new en_shebei();
                en = biz_cls.get_shebei_byid(this.shebei_id);
                this.txt_shebei_name.Text = en.shebei_mingcheng;
                if (en.f_shebei_id > 0)
                {
                    this.txt_fshebei_id.Text = en.f_shebei_id.ToString();
                }
                else
                {
                    this.txt_fshebei_id.Text = "";
                }
                this.comb_shebei_leixing.SelectedValue = en.shebei_leixing_id.ToString();
                this.comb_shebei_leixing_SelectedIndexChanged(null, null);
                this.comb_zhandian.SelectedValue = en.zhandian_id.ToString();
                this.txt_caijicanshu.Text = en.caiji_canshu_str;
                if (Utility.ToInt(this.txt_paixu.Text) > 0)
                {
                    this.txt_paixu.Text = en.paixu_num.ToString();
                }
                else
                {
                    this.txt_paixu.Text = "0";
                }
                this.txt_shebei_miaoshu.Text = en.shebei_miaoshu;
                if (Utility.ToInt(this.txt_yali_min.Text) > 0)
                {
                    this.txt_yali_min.Text = en.yali_min.ToString();
                }
                else
                {
                    this.txt_yali_min.Text = "0";
                }
                if (Utility.ToInt(this.txt_yali_max.Text) > 0)
                {
                    this.txt_yali_max.Text = en.yali_max.ToString();
                }
                else
                {
                    this.txt_yali_max.Text = "0";
                }
                if (en.if_nenghaojisuan == 1) { this.cb_nenghao.Checked = true; } else { this.cb_nenghao.Checked = false; }
                if (en.if_zongguanliuliang == 1) { this.cb_liuliang.Checked = true; } else { this.cb_liuliang.Checked = false; }
                if (en.if_zongguanyali == 1) { this.cb_yali.Checked = true; } else { this.cb_yali.Checked = false; }
                if (en.qiyong_flag == 1) { this.cb_qiyong.Checked = true; } else { this.cb_qiyong.Checked = false; }

            }
        }

        private string add()
        {
            try
            {
                en.shebei_mingcheng = this.txt_shebei_name.Text;
                if (Utility.ToInt(this.txt_fshebei_id.Text) <= 0) { en.f_shebei_id = 0; } else { en.f_shebei_id = Utility.ToInt(this.txt_fshebei_id.Text); }
                en.shebei_leixing_id = Utility.ToObjectString(this.comb_shebei_leixing.SelectedValue);
                en.zhandian_id = Utility.ToObjectString(this.comb_zhandian.SelectedValue);
                en.shebei_miaoshu = this.txt_shebei_miaoshu.Text;
                en.caiji_canshu_str = this.txt_caijicanshu.Text.Trim();
                if (this.cb_yali.Checked) { en.if_zongguanyali = 1; } else { en.if_zongguanyali = 0; }
                if (this.cb_liuliang.Checked) { en.if_zongguanliuliang = 1; } else { en.if_zongguanliuliang = 0; }
                if (this.cb_nenghao.Checked) { en.if_nenghaojisuan = 1; } else { en.if_nenghaojisuan = 0; }
                if (this.cb_qiyong.Checked) { en.qiyong_flag = 1; } else { en.qiyong_flag = 0; }
                en.zhucanshu_num = 0;
                en.fucanshu_num = 0;
                en.zhucanshu_zong = biz_cls.get_shebei_zhucanshu_zong(en.shebei_leixing_id);
                if (en.caiji_canshu_str != "")
                {
                    string[] s = en.caiji_canshu_str.Split('|');
                    en.fucanshu_zong = s.Length;
                }
                else
                {
                    en.fucanshu_zong = 0;
                }

                return biz_cls.shebei_add(en);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string upd()
        {
            try
            {
                en.shebei_mingcheng = this.txt_shebei_name.Text;
                if (Utility.ToInt(this.txt_fshebei_id.Text) <= 0) { en.f_shebei_id = 0; } else { en.f_shebei_id = Utility.ToInt(this.txt_fshebei_id.Text); }
                en.shebei_leixing_id = Utility.ToObjectString(this.comb_shebei_leixing.SelectedValue);
                en.zhandian_id = Utility.ToObjectString(this.comb_zhandian.SelectedValue);
                en.shebei_miaoshu = this.txt_shebei_miaoshu.Text;
                en.caiji_canshu_str = this.txt_caijicanshu.Text.Trim();
                if (this.cb_yali.Checked) { en.if_zongguanyali = 1; } else { en.if_zongguanyali = 0; }
                if (this.cb_liuliang.Checked) { en.if_zongguanliuliang = 1; } else { en.if_zongguanliuliang = 0; }
                if (this.cb_nenghao.Checked) { en.if_nenghaojisuan = 1; } else { en.if_nenghaojisuan = 0; }
                if (this.cb_qiyong.Checked) { en.qiyong_flag = 1; } else { en.qiyong_flag = 0; }
                en.zhucanshu_num = Utility.ToInt(MySqlHelper.Get_sigle("select count(*) from uv_tx_mokuai_field where shebei_id=" + this.shebei_id + " and zhucanshu<>''"));
                en.fucanshu_num = Utility.ToInt(MySqlHelper.Get_sigle("select count(*) from uv_tx_mokuai_field where shebei_id=" + this.shebei_id + " and fucanshu<>''"));

                en.zhucanshu_zong = biz_cls.get_shebei_zhucanshu_zong(en.shebei_leixing_id);
                if (en.caiji_canshu_str != "")
                {
                    string[] s = en.caiji_canshu_str.Split('|');
                    en.fucanshu_zong = s.Length;
                }
                else
                {
                    en.fucanshu_zong = 0;
                }

                return biz_cls.shebei_upd(en);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool check()
        {
            if (Utility.ToObjectString(this.comb_shebei_leixing.SelectedValue) == "0")
            {
                MessageBox.Show("请选择设备类型");
                this.comb_shebei_leixing.Focus();
                return false;
            }

            if (Utility.ToObjectString(this.comb_zhandian.SelectedValue) == "0")
            {
                MessageBox.Show("请选择站点");
                this.comb_zhandian.Focus();
                return false;
            }

            if (this.txt_shebei_name.Text == "")
            {
                MessageBox.Show("设备名称不能为空");
                this.txt_shebei_name.Focus();
                return false;
            }

            #region 设备名称不能重复

            if (this.shebei_id <= 0)
            {
                //新增状态
                if (Utility.ToInt(MySqlHelper.Get_sigle("select count(*) from base_shebei where shebei_mingcheng='" + this.txt_shebei_name.Text + "'")) > 0)
                {
                    MessageBox.Show("该设备名称已存在");
                    this.txt_shebei_name.Focus();
                    return false;
                }
            }
            else
            {
                //修改状态
                if (Utility.ToInt(MySqlHelper.Get_sigle("select count(*) from base_shebei where shebei_mingcheng='" + this.txt_shebei_name.Text + "' and shebei_id<>" + shebei_id)) > 0)
                {
                    MessageBox.Show("该设备名称已存在");
                    this.txt_shebei_name.Focus();
                    return false;
                }
            }

            #endregion

            #region 父设备ID是否存在

            if (this.txt_fshebei_id.Text != "" && this.txt_fshebei_id.Text != "0")
            {
                if (Utility.ToInt(MySqlHelper.Get_sigle("select count(*) from base_shebei where shebei_id=" + Utility.ToInt(this.txt_fshebei_id.Text))) <= 0)
                {
                    MessageBox.Show("父设备ID不存在");
                    this.txt_fshebei_id.Focus();
                    return false;
                }
            }

            #endregion

            if (this.txt_yali_min.Text != "" && this.txt_yali_min.Text != "0")
            {
                if (Utility.IsFloat(this.txt_yali_min.Text) == false)
                {
                    MessageBox.Show("最小压力应为数字类型");
                    this.txt_yali_min.Focus();
                    return false;
                }

                if (Utility.ToDecimal(this.txt_yali_min.Text) < 0)
                {
                    MessageBox.Show("最小压力数据输入错误");
                    this.txt_yali_min.Focus();
                    return false;
                }
            }

            if (this.txt_yali_max.Text != "" && this.txt_yali_max.Text != "0")
            {
                if (Utility.IsFloat(this.txt_yali_max.Text) == false)
                {
                    MessageBox.Show("最大压力应为数字类型");
                    this.txt_yali_max.Focus();
                    return false;
                }

                if (Utility.ToDecimal(this.txt_yali_max.Text) < 0 || Utility.ToDecimal(this.txt_yali_max.Text) < Utility.ToDecimal(this.txt_yali_min.Text))
                {
                    MessageBox.Show("最大压力数据输入错误");
                    this.txt_yali_max.Focus();
                    return false;
                }
            }

            if (this.txt_paixu.Text != "" && this.txt_paixu.Text != "0")
            {
                if (Utility.IsInt(this.txt_paixu.Text) == false)
                {
                    MessageBox.Show("排序号应为正整数");
                    this.txt_paixu.Focus();
                    return false;
                }

                if (Utility.ToInt(this.txt_paixu.Text) < 0)
                {
                    MessageBox.Show("排序号应为正整数");
                    this.txt_paixu.Focus();
                    return false;
                }
            }

            if (this.txt_caijicanshu.Text != "")
            {
                string[] s = this.txt_caijicanshu.Text.Split('|');
                foreach (string s1 in s)
                {
                    if (s1.Contains("{") == false || s1.Contains("#") == false || s1.Contains("}") == false)
                    {
                        MessageBox.Show("【实时采集参数】格式不正确");
                        this.txt_caijicanshu.Focus();
                        return false;
                    }
                }
            }

            return true;
        }

        private void comb_shebei_leixing_SelectedIndexChanged(object sender, EventArgs e)
        {
            string svalue = Utility.ToObjectString(this.comb_shebei_leixing.SelectedValue);
            switch (svalue)
            {
                case "lixinji":
                    this.cb_liuliang.Visible = false;
                    this.cb_nenghao.Visible = false;
                    this.cb_yali.Visible = false;

                    this.lb_canshu.Visible = true;
                    this.txt_caijicanshu.Visible = true;
                    this.lb_beizhu.Visible = true;

                    this.lb_yali_min.Visible = false;
                    this.lb_yali_max.Visible = false;
                    this.txt_yali_max.Visible = false;
                    this.txt_yali_min.Visible = false;

                    this.lb_paixuhao.Visible = false;
                    this.txt_paixu.Visible = false;
                    break;
                case "luoganji":
                    this.cb_liuliang.Visible = false;
                    this.cb_nenghao.Visible = false;
                    this.cb_yali.Visible = false;

                    this.lb_canshu.Visible = true;
                    this.txt_caijicanshu.Visible = true;
                    this.lb_beizhu.Visible = true;

                    this.lb_yali_min.Visible = true;
                    this.lb_yali_max.Visible = true;
                    this.txt_yali_max.Visible = true;
                    this.txt_yali_min.Visible = true;

                    this.lb_paixuhao.Visible = true;
                    this.txt_paixu.Visible = true;
                    break;
                case "dianbiao":
                    this.cb_liuliang.Visible = false;
                    this.cb_nenghao.Visible = true;
                    this.cb_yali.Visible = false;

                    this.lb_canshu.Visible = true;
                    this.txt_caijicanshu.Visible = true;
                    this.lb_beizhu.Visible = true;

                    this.lb_yali_min.Visible = false;
                    this.lb_yali_max.Visible = false;
                    this.txt_yali_max.Visible = false;
                    this.txt_yali_min.Visible = false;

                    this.lb_paixuhao.Visible = false;
                    this.txt_paixu.Visible = false;
                    break;
                case "liuliangji":
                    this.cb_liuliang.Visible = true;
                    this.cb_nenghao.Visible = true;
                    this.cb_yali.Visible = false;

                    this.lb_canshu.Visible = false;
                    this.txt_caijicanshu.Visible = false;
                    this.lb_beizhu.Visible = false;

                    this.lb_yali_min.Visible = false;
                    this.lb_yali_max.Visible = false;
                    this.txt_yali_max.Visible = false;
                    this.txt_yali_min.Visible = false;

                    this.lb_paixuhao.Visible = false;
                    this.txt_paixu.Visible = false;
                    break;
                case "wenduji":
                    this.cb_liuliang.Visible = false;
                    this.cb_nenghao.Visible = false;
                    this.cb_yali.Visible = false;

                    this.lb_canshu.Visible = false;
                    this.txt_caijicanshu.Visible = false;
                    this.lb_beizhu.Visible = false;

                    this.lb_yali_min.Visible = false;
                    this.lb_yali_max.Visible = false;
                    this.txt_yali_max.Visible = false;
                    this.txt_yali_min.Visible = false;

                    this.lb_paixuhao.Visible = false;
                    this.txt_paixu.Visible = false;
                    break;
                case "ludianyi":
                    this.cb_liuliang.Visible = false;
                    this.cb_nenghao.Visible = false;
                    this.cb_yali.Visible = false;

                    this.lb_canshu.Visible = false;
                    this.txt_caijicanshu.Visible = false;
                    this.lb_beizhu.Visible = false;

                    this.lb_yali_min.Visible = false;
                    this.lb_yali_max.Visible = false;
                    this.txt_yali_max.Visible = false;
                    this.txt_yali_min.Visible = false;

                    this.lb_paixuhao.Visible = false;
                    this.txt_paixu.Visible = false;
                    break;
                case "yalichuanganqi":
                    this.cb_liuliang.Visible = false;
                    this.cb_nenghao.Visible = false;
                    this.cb_yali.Visible = true;

                    this.lb_canshu.Visible = false;
                    this.txt_caijicanshu.Visible = false;
                    this.lb_beizhu.Visible = false;

                    this.lb_yali_min.Visible = false;
                    this.lb_yali_max.Visible = false;
                    this.txt_yali_max.Visible = false;
                    this.txt_yali_min.Visible = false;

                    this.lb_paixuhao.Visible = false;
                    this.txt_paixu.Visible = false;
                    break;
                case "lengganji":
                    this.cb_liuliang.Visible = false;
                    this.cb_nenghao.Visible = false;
                    this.cb_yali.Visible = false;

                    this.lb_canshu.Visible = true;
                    this.txt_caijicanshu.Visible = true;
                    this.lb_beizhu.Visible = true;

                    this.lb_yali_min.Visible = false;
                    this.lb_yali_max.Visible = false;
                    this.txt_yali_max.Visible = false;
                    this.txt_yali_min.Visible = false;

                    this.lb_paixuhao.Visible = false;
                    this.txt_paixu.Visible = false;
                    break;
                case "xiganji":
                    this.cb_liuliang.Visible = false;
                    this.cb_nenghao.Visible = false;
                    this.cb_yali.Visible = false;

                    this.lb_canshu.Visible = true;
                    this.txt_caijicanshu.Visible = true;
                    this.lb_beizhu.Visible = true;

                    this.lb_yali_min.Visible = false;
                    this.lb_yali_max.Visible = false;
                    this.txt_yali_max.Visible = false;
                    this.txt_yali_min.Visible = false;

                    this.lb_paixuhao.Visible = false;
                    this.txt_paixu.Visible = false;
                    break;
                case "lengqueji":
                    this.cb_liuliang.Visible = false;
                    this.cb_nenghao.Visible = false;
                    this.cb_yali.Visible = true;

                    this.lb_canshu.Visible = true;
                    this.txt_caijicanshu.Visible = true;
                    this.lb_beizhu.Visible = true;

                    this.lb_yali_min.Visible = false;
                    this.lb_yali_max.Visible = false;
                    this.txt_yali_max.Visible = false;
                    this.txt_yali_min.Visible = false;

                    this.lb_paixuhao.Visible = false;
                    this.txt_paixu.Visible = false;
                    break;
                default:
                    break;
            }
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.check() == false) { return; }
                string err_str = "";

                if (shebei_id == 0)
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
                    frm_shebei f = (frm_shebei)this.Owner;
                    f.Load_data();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void lb_lb_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void txt_shebei_name_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void cb_liuliang_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
