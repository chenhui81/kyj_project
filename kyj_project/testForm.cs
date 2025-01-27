using kyj_project.Common;
using System;
using System.Data;
using System.Windows.Forms;

namespace kyj_project
{
    public partial class testForm : Form
    {
        public testForm()
        {
            InitializeComponent();
        }

        private void testForm_Load(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            // 创建shijian列
            DataColumn shijian = new DataColumn("shijian", typeof(string));
            dt.Columns.Add(shijian);
            // 创建shuju1列
            DataColumn shuju1 = new DataColumn("shuju1", typeof(int));
            dt.Columns.Add(shuju1);

            // 创建shuju2列
            DataColumn shuju2 = new DataColumn("shuju2", typeof(int));
            dt.Columns.Add(shuju2);

            // 创建shuju3列
            DataColumn shuju3 = new DataColumn("shuju3", typeof(int));
            dt.Columns.Add(shuju3);


            DataSet ds = new DataSet();
            ds = MySqlHelper.Get_DataSet("select shijian,shuju from tongji_m_value where shebei_id=32 and zhucanshu='dianliang' and shijian>='2024-12-22 00:00:00' and shijian<'2024-12-23 00:00:00'");

            DataSet ds1 = new DataSet();
            ds1 = MySqlHelper.Get_DataSet("select shijian,shuju from tongji_m_value where shebei_id=33 and zhucanshu='dianliang' and shijian>='2024-12-22 00:00:00' and shijian<'2024-12-23 00:00:00'");

            DataSet ds2 = new DataSet();
            ds2 = MySqlHelper.Get_DataSet("select shijian,shuju from tongji_m_value where shebei_id=34 and zhucanshu='dianliang' and shijian>='2024-12-22 00:00:00' and shijian<'2024-12-23 00:00:00'");

            foreach (DataRow dr1 in ds.Tables[0].Rows)
            {
                DataRow dr = dt.NewRow();
                dr["shijian"] = dr1["shijian"];
                dr["shuju1"] = dr1["shuju"];
                dr["shuju2"] = ds1.Tables[0].Select("shijian='" + dr["shijian"].ToString() + "'")[0][1];
                dr["shuju3"] = ds2.Tables[0].Select("shijian='" + dr["shijian"].ToString() + "'")[0][1];
                dt.Rows.Add(dr);
            }

            this.dataGridView1.DataSource = dt;
        }
    }
}
