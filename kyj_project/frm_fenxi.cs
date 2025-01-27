using kyj_project.Common;
using kyj_project.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace kyj_project
{
    public partial class frm_fenxi : Form
    {

        List<string> ls_sel = new List<string>();
        int load_over = 0;//初始，防止comb刷新
        public frm_fenxi()
        {
            InitializeComponent();
            this.Text = biz_cls.xiangmu_mingcheng;

            //初始化grid
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
        }


        #region 分页属性

        //private int _page_rowcount = 50;
        ///// <summary>
        ///// 每页记录数
        ///// </summary>
        //public int page_rowcount
        //{
        //    get { return _page_rowcount; }
        //    set { _page_rowcount = value; }
        //}

        private int _page_index = 1;
        /// <summary>
        /// 当前页码
        /// </summary>
        public int page_index
        {
            get { return _page_index; }
            set { _page_index = value; }
        }


        private int _row_total = 0;
        /// <summary>
        /// 总记录数
        /// </summary>
        public int row_total
        {
            get { return _row_total; }
            set { _row_total = value; }
        }

        /// <summary>
        /// 总计页数
        /// </summary>
        public int page_count
        {
            get { return this.get_totalpage(row_total, biz_cls.page_rowcount); }
        }

        /// <summary>
        /// 获取分页数量
        /// </summary>
        /// <param name="totalRecord">总记录数</param>
        /// <param name="maxResult">每页数量</param>
        /// <returns></returns>
        private int get_totalpage(int totalRecord, int maxResult)
        {
            return totalRecord % maxResult == 0 ? totalRecord / maxResult : totalRecord / maxResult + 1;
        }

        /// <summary>
        /// 首页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sph_first_Click(object sender, EventArgs e)
        {
            this.page_index = 1;
            this.Format_grid(ls_sel);
            this.Load_data(ls_sel);
        }

        /// <summary>
        /// 上一页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sph_up_Click(object sender, EventArgs e)
        {
            this.page_index = this.page_index - 1;
            if (this.page_index < 1) { this.page_index = 1; }
            this.Format_grid(ls_sel);
            this.Load_data(ls_sel);
        }

        /// <summary>
        /// 下一页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sph_next_Click(object sender, EventArgs e)
        {
            this.page_index = this.page_index + 1;
            if (this.page_index > this.page_count) { this.page_index = this.page_count; }
            this.Format_grid(ls_sel);
            this.Load_data(ls_sel);
        }

        /// <summary>
        /// 尾页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sph_last_Click(object sender, EventArgs e)
        {
            this.page_index = this.page_count;
            this.Format_grid(ls_sel);
            this.Load_data(ls_sel);
        }

        private void comb_rowcount_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.load_over == 1)
            {
                biz_cls.page_rowcount = Utility.ToInt(this.comb_rowcount.Text);
                this.page_index = 1;
                this.Format_grid(ls_sel);
                this.Load_data(ls_sel);
            }
        }


        #endregion


        /// <summary>
        /// 格式化grid
        /// </summary>
        private void Format_grid(List<string> ls)
        {

            if (ls.Count <= 0) { return; }
            this.dataGridView1.Columns.Clear();

            DataGridViewTextBoxColumn dc1ex = new DataGridViewTextBoxColumn
            {
                Name = "shijian",
                DataPropertyName = "shijian",
                HeaderText = "时间",
                Width = 120
            };
            dc1ex.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dc1ex.ReadOnly = true;
            dataGridView1.Columns.Add(dc1ex);

            //3 | jinqi | 3#离心机|进气比例|%
            for (int i = 0; i < ls.Count; i++)
            {
                string[] s = ls[i].Split('|');
                if (s.Length == 5)
                {
                    DataGridViewTextBoxColumn dc = new DataGridViewTextBoxColumn
                    {
                        Name = "shuju" + i.ToString(),
                        DataPropertyName = "shuju" + i.ToString(),
                        HeaderText = s[2] + "\r\n" + s[3] + "\r\n" + s[4],
                        Width = 120
                    };
                    dc.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dc.ReadOnly = true;
                    dataGridView1.Columns.Add(dc);
                }
            }


        }

        /// <summary>
        /// 加载数据
        /// </summary>
        public void Load_data(List<string> ls)
        {
            try
            {
                if (ls.Count <= 0) { return; }
                //3 | jinqi | 3#离心机|进气比例|%
                #region 创建dt

                DataTable dt = new DataTable();
                // 创建shijian列
                DataColumn dc_shijian = new DataColumn("shijian", typeof(string));
                dt.Columns.Add(dc_shijian);

                for (int i = 0; i < ls.Count; i++)
                {
                    // 创建shuju列
                    DataColumn dc = new DataColumn("shuju" + i.ToString(), typeof(decimal));
                    dt.Columns.Add(dc);
                }

                #endregion

                this.comb_rowcount.Text = biz_cls.page_rowcount.ToString();

                string s1 = "";
                string s2 = "";
                bool bsort = false;

                if (this.dataGridView1.SortedColumn != null)
                {
                    s1 = this.dataGridView1.SortedColumn.Name;
                    s2 = this.dataGridView1.SortedColumn.HeaderCell.SortGlyphDirection.ToString();
                    bsort = true;
                }

                DataSet ds = new DataSet();
                string sqlstr = "";
                string sqlstr1 = "";

                int shebei_id_0 = Utility.ToInt(ls[0].Split('|')[0]);
                string zhucanshu_0 = ls[0].Split('|')[1];


                switch (this.comb_leixing.Text)
                {
                    case "分钟":
                        sqlstr = "select shijian,shuju from tongji_m_value  where shebei_id=" + shebei_id_0 + " and zhucanshu='" + zhucanshu_0 + "' and shijian>='" + this.dtp_s.Value.ToString("yyyy-MM-dd 00:00") + "' and shijian<'" + this.dtp_e.Value.AddDays(1).ToString("yyyy-MM-dd 00:00") + "'";
                        sqlstr1 = "select count(*) from tongji_m_value   where shebei_id=" + shebei_id_0 + " and zhucanshu='" + zhucanshu_0 + "' and shijian>='" + this.dtp_s.Value.ToString("yyyy-MM-dd 00:00") + "' and shijian<'" + this.dtp_e.Value.AddDays(1).ToString("yyyy-MM-dd 00:00") + "'";
                        break;
                    case "5分钟":
                        sqlstr = "select shijian,shuju from tongji_5m_value  where shebei_id=" + shebei_id_0 + " and zhucanshu='" + zhucanshu_0 + "' and shijian>='" + this.dtp_s.Value.ToString("yyyy-MM-dd 00:00") + "' and shijian<'" + this.dtp_e.Value.AddDays(1).ToString("yyyy-MM-dd 00:00") + "'";
                        sqlstr1 = "select count(*) from tongji_5m_value   where shebei_id=" + shebei_id_0 + " and zhucanshu='" + zhucanshu_0 + "' and shijian>='" + this.dtp_s.Value.ToString("yyyy-MM-dd 00:00") + "' and shijian<'" + this.dtp_e.Value.AddDays(1).ToString("yyyy-MM-dd 00:00") + "'";
                        break;
                    case "小时":
                        sqlstr = "select shijian,shuju from tongji_h_value   where shebei_id=" + shebei_id_0 + " and zhucanshu='" + zhucanshu_0 + "' and shijian>='" + this.dtp_s.Value.ToString("yyyy-MM-dd 00") + "' and shijian<'" + this.dtp_e.Value.AddDays(1).ToString("yyyy-MM-dd 00") + "'";
                        sqlstr1 = "select count(*) from tongji_h_value     where shebei_id=" + shebei_id_0 + " and zhucanshu='" + zhucanshu_0 + "' and shijian>='" + this.dtp_s.Value.ToString("yyyy-MM-dd 00") + "' and shijian<'" + this.dtp_e.Value.AddDays(1).ToString("yyyy-MM-dd 00") + "'";
                        break;

                    default:
                        sqlstr = "select shijian,shuju from tongji_m_value  where shebei_id=" + ls[0].Split('|')[0] + " and zhucanshu='" + zhucanshu_0 + "' and shijian>='" + this.dtp_s.Value.ToString("yyyy-MM-dd 00:00") + "' and shijian<'" + this.dtp_e.Value.AddDays(1).ToString("yyyy-MM-dd 00:00") + "'";
                        sqlstr1 = "select count(*) from tongji_m_value   where shebei_id=" + ls[0].Split('|')[0] + " and zhucanshu='" + zhucanshu_0 + "' and shijian>='" + this.dtp_s.Value.ToString("yyyy-MM-dd 00:00") + "' and shijian<'" + this.dtp_e.Value.AddDays(1).ToString("yyyy-MM-dd 00:00") + "'";
                        break;

                }

                //string str_where = "";

                //sqlstr1 += str_where; //查询总记录数sql
                //sqlstr += str_where;



                this.row_total = Utility.ToInt(MySqlHelper.Get_sigle(sqlstr1));  //总记录数
                if (row_total <= 0)
                {
                    MessageBox.Show("首列没有记录！");
                    return;
                }

                if (this.page_index <= 0) { this.page_index = 1; }

                this.sph_current_page.Text = this.page_index.ToString() + " / " + this.page_count.ToString().ToString();
                this.sph_total.Text = this.row_total.ToString();

                sqlstr += " order by shijian desc";
                int start_index = biz_cls.page_rowcount * (page_index - 1);  //起始记录数
                sqlstr += " limit " + start_index + "," + biz_cls.page_rowcount; //sql
                ds = MySqlHelper.Get_DataSet(sqlstr);
                DataTable dt0 = ds.Tables[0];
                string max_time = Utility.ToObjectString(dt0.Rows[0]["shijian"]);
                string min_time = Utility.ToObjectString(dt0.Rows[dt0.Rows.Count - 1]["shijian"]);

                //获取除了第一项之外其他的DATATABLE
                List<DataTable> ls_dt = new List<DataTable>();
                for (int i = 1; i < ls.Count; i++)
                {

                    int shebei_id_i = Utility.ToInt(ls[i].Split('|')[0]);
                    string zhucanshu_i = ls[i].Split('|')[1];
                    string sql_str2 = "";
                    switch (this.comb_leixing.Text)
                    {
                        case "分钟":
                            sql_str2 = "select shijian,shuju from tongji_m_value  where shebei_id=" + shebei_id_i + " and zhucanshu='" + zhucanshu_i + "' and shijian>='" + min_time + "' and shijian<='" + max_time + "'";
                            break;
                        case "5分钟":
                            sql_str2 = "select shijian,shuju from tongji_5m_value  where shebei_id=" + shebei_id_i + " and zhucanshu='" + zhucanshu_i + "' and shijian>='" + min_time + "' and shijian<='" + max_time + "'";
                            break;
                        case "小时":
                            sql_str2 = "select shijian,shuju from tongji_h_value  where shebei_id=" + shebei_id_i + " and zhucanshu='" + zhucanshu_i + "' and shijian>='" + min_time + "' and shijian<='" + max_time + "'";
                            break;

                        default:
                            sql_str2 = "select shijian,shuju from tongji_m_value  where shebei_id=" + shebei_id_i + " and zhucanshu='" + zhucanshu_i + "' and shijian>='" + min_time + "' and shijian<='" + max_time + "'";
                            break;

                    }
                    if (sql_str2 != "")
                    {
                        ls_dt.Add(MySqlHelper.Get_DataSet(sql_str2).Tables[0]);
                    }
                }

                foreach (DataRow dr0 in dt0.Rows)
                {
                    DataRow dr1 = dt.NewRow();
                    dr1["shijian"] = dr0["shijian"];
                    dr1["shuju0"] = dr0["shuju"];
                    for (int i = 1; i < ls.Count; i++)
                    {
                        DataRow[] dr_linshi = ls_dt[i - 1].Select("shijian='" + dr0["shijian"].ToString() + "'");
                        if (dr_linshi.Length > 0)
                        {
                            dr1["shuju" + i] = dr_linshi[0][1];
                        }
                        else
                        {
                            dr1["shuju" + i] = -1;
                        }

                    }
                    dt.Rows.Add(dr1);
                }


                //绑定GRID
                this.dataGridView1.DataSource = dt;
                //   this.toolStripStatusLabel1.Text = "总计" + ds.Tables[0].Rows.Count.ToString() + "条记录";

                if (bsort == true)
                {
                    DataGridViewColumn dvc = this.dataGridView1.Columns[s1];
                    if (s2 == "Ascending")
                    {
                        this.dataGridView1.Sort(dvc, ListSortDirection.Ascending);
                    }
                    else
                    {
                        this.dataGridView1.Sort(dvc, ListSortDirection.Descending);
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void load_tv()
        {
            DataSet ds = new DataSet();
            ds = MySqlHelper.Get_DataSet("select zhandian_id, zhandian_mingcheng from base_zhandian order by zhandian_id");
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    TreeNode tn = new TreeNode
                    {
                        Text = Utility.ToObjectString(dr["zhandian_mingcheng"]),
                        Tag = "zd_" + Utility.ToObjectString(dr["zhandian_id"])
                    };

                    this.tv.Nodes.Add(tn);

                    //DataSet ds1 = new DataSet();
                    //ds1 = MySqlHelper.Get_DataSet("select shebei_id,shebei_mingcheng,shebei_leixing_id from base_shebei where zhandian_id='" + Utility.ToObjectString(dr["zhandian_id"]) + "'");
                    //if (ds1.Tables[0].Rows.Count > 0)
                    //{
                    //    foreach (DataRow dr1 in ds1.Tables[0].Rows)
                    //    {
                    //        TreeNode tn1 = new TreeNode();
                    //        tn1.Text = Utility.ToObjectString(dr1["shebei_mingcheng"]);
                    //        tn1.Tag = Utility.ToObjectString(dr1["shebei_id"]);
                    //        tn.Nodes.Add(tn1);

                    //        DataSet ds2 = new DataSet();
                    //        ds2 = MySqlHelper.Get_DataSet("select canshu_field_name,canshu_mingcheng,canshu_danwei from base_shebei_leixing_canshu where shebei_leixing_id='" + Utility.ToObjectString(dr1["shebei_leixing_id"])+"'");
                    //        if (ds2.Tables[0].Rows.Count > 0)
                    //        {
                    //            foreach (DataRow dr2 in ds2.Tables[0].Rows)
                    //            {
                    //                TreeNode tn2 = new TreeNode();
                    //                tn2.Text = Utility.ToObjectString(dr2["canshu_mingcheng"])+"#"+Utility.ToObjectString(dr2["canshu_danwei"]);
                    //                tn2.Tag = Utility.ToObjectString(dr2["canshu_field_name"]);

                    //                tn1.Nodes.Add(tn2);
                    //            }
                    //        }

                    //    }
                    //} 
                }
            }
        }

        private void frm_fenxi_Load(object sender, EventArgs e)
        {


            this.comb_leixing.Text = "分钟";
            this.dtp_s.Value = DateTime.Now;
            this.dtp_e.Value = DateTime.Now;
            this.comb_rowcount.SelectedIndex = 1;

            this.load_tv();

            this.load_over = 1;

            //this.Format_grid();
            //this.Load_data();
        }

        private void dataGridView1_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            using (SolidBrush b = new SolidBrush(dataGridView1.RowHeadersDefaultCellStyle.ForeColor))
            {
                e.Graphics.DrawString((e.RowIndex + 1).ToString(System.Globalization.CultureInfo.CurrentCulture), dataGridView1.DefaultCellStyle.Font, b, e.RowBounds.Location.X + 12, e.RowBounds.Location.Y + 4);
            }
        }


        private void pb_chaxun_Click(object sender, EventArgs e)
        {

            List<string> ls = new List<string>();
            foreach (TreeNode tn in this.tv.Nodes)
            {
                if (tn.Nodes.Count > 0)
                {
                    foreach (TreeNode tn1 in tn.Nodes)
                    {
                        if (tn1.Nodes.Count > 0)
                        {
                            foreach (TreeNode tn2 in tn1.Nodes)
                            {
                                if (tn2.Checked == true)
                                {
                                    ls.Add(tn2.Tag.ToString() + "|" + tn2.Text);
                                }
                            }
                        }
                    }
                }
            }

            //int i = ls.Count;
            this.ls_sel = ls;
            this.Format_grid(ls);
            this.Load_data(ls);
        }

        private void tv_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag.ToString().Length > 3 && e.Node.Tag.ToString().Substring(0, 3) == "zd_")
            {
                if (e.Node.Nodes.Count > 0) { return; }
                DataSet ds1 = new DataSet();
                ds1 = MySqlHelper.Get_DataSet("select shebei_id,shebei_mingcheng,shebei_leixing_id from base_shebei where zhandian_id='" + Utility.ToObjectString(e.Node.Tag).Substring(3) + "'");
                if (ds1.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr1 in ds1.Tables[0].Rows)
                    {
                        TreeNode tn1 = new TreeNode
                        {
                            Text = Utility.ToObjectString(dr1["shebei_mingcheng"]),
                            Tag = "sb_" + Utility.ToObjectString(dr1["shebei_leixing_id"]) + "_" + Utility.ToObjectString(dr1["shebei_id"])
                        };
                        e.Node.Nodes.Add(tn1);

                    }
                }
            }

            if (e.Node.Tag.ToString().Length > 3 && e.Node.Tag.ToString().Substring(0, 3) == "sb_")
            {
                if (e.Node.Nodes.Count > 0) { return; }
                string[] s = e.Node.Tag.ToString().Split('_');
                string shebei_leixing_id = s[1];
                string shebei_id = s[2];

                DataSet ds2 = new DataSet();
                ds2 = MySqlHelper.Get_DataSet("select canshu_field_name,canshu_mingcheng,canshu_danwei from base_shebei_leixing_canshu where shebei_leixing_id='" + shebei_leixing_id + "'");
                if (ds2.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr2 in ds2.Tables[0].Rows)
                    {
                        TreeNode tn2 = new TreeNode
                        {
                            Text = Utility.ToObjectString(dr2["canshu_mingcheng"]) + "|" + Utility.ToObjectString(dr2["canshu_danwei"]),
                            Tag = shebei_id + "|" + Utility.ToObjectString(dr2["canshu_field_name"]) + "|" + e.Node.Text
                        };
                        Font anotherFont = new Font("微软雅黑", 12, FontStyle.Bold);
                        tn2.NodeFont = anotherFont;
                        e.Node.Nodes.Add(tn2);

                    }
                }
            }
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            string sname = this.dataGridView1.Columns[e.ColumnIndex].HeaderText;

            if (sname.ToLower().Contains("电量") || sname.ToLower().Contains("气量") || sname.ToLower().Contains("开关"))
            {
                e.CellStyle.Format = "N0";
            }
        }
    }
}
