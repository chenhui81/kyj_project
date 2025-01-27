using kyj_project.Common;
using kyj_project.DAL;
using System;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace kyj_project
{
    public partial class frm_shebei_shishi : Form
    {

        public int shebei_id = 0; //设备ID
        public string shebei_leixing_id = "";//设备类型ID
        public string shebei_mingcheng = "";
        public string shebei_leixing = "";
        public string zhandian_mingcheng = "";
        public frm_shebei_shishi()
        {
            InitializeComponent();
            this.Text = biz_cls.xiangmu_mingcheng;

        }

        private void load_data()
        {
            StringBuilder sb_zhu = new StringBuilder();
            shebei_leixing_id = Utility.ToObjectString(MySqlHelper.Get_sigle("select shebei_leixing_id from base_shebei where shebei_id=" + shebei_id));
            DataSet ds = new DataSet();
            ds = MySqlHelper.Get_DataSet("select canshu_field_name,canshu_mingcheng,canshu_danwei from base_shebei_leixing_canshu where shebei_leixing_id='" + shebei_leixing_id + "'");
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    string zhucanshu = Utility.ToObjectString(dr["canshu_field_name"]);
                    string canshu_mingcheng = Utility.ToObjectString(dr["canshu_mingcheng"]);
                    string canshu_danwei = Utility.ToObjectString(dr["canshu_danwei"]);
                    DataSet ds1 = new DataSet();
                    ds1 = MySqlHelper.Get_DataSet("select shuju,shijian from uv_caiji_base where shebei_id=" + shebei_id + " and zhucanshu='" + zhucanshu + "' order by shijian desc limit 1");
                    if (ds1.Tables[0].Rows.Count > 0)
                    {
                        DataRow dr1 = ds1.Tables[0].Rows[0];
                        string shuju = Utility.ToDecimal(dr1["shuju"]).ToString("G29");
                        string shijian = Utility.ToObjectString(dr1["shijian"]);
                        sb_zhu.Append(canshu_mingcheng + ": " + shuju + canshu_danwei + "    " + shijian + "\r\n");
                    }
                }

                this.richTextBox1.Text = sb_zhu.ToString();
            }

            string sj = Utility.ToObjectString(MySqlHelper.Get_sigle("select max(shijian) from uv_caiji_base where shebei_id=" + shebei_id + " and fucanshu<>''"));
            StringBuilder sb_fu = new StringBuilder();
            string str = Utility.ToObjectString(MySqlHelper.Get_sigle("select caiji_canshu_value from base_shebei where shebei_id=" + shebei_id));
            if (str != "")
            {
                string[] s = str.Split('|');
                foreach (string s1 in s)
                {
                    if (s1 != "")
                    {
                        sb_fu.Append(s1.Trim() + "  " + sj + "\r\n");
                    }
                }

                this.richTextBox2.Text = sb_fu.ToString();
            }

        }

        private void frm_shebei_shishi_Load(object sender, EventArgs e)
        {
            this.lb_title.Text = "设备；【" + shebei_mingcheng + "】  类型：【" + shebei_leixing + "】  站点：【" + zhandian_mingcheng + "】";

            this.load_data();

            this.timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.load_data();
        }
    }
}
