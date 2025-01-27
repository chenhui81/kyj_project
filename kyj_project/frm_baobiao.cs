using kyj_project.Common;
using kyj_project.DAL;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace kyj_project
{
    public partial class frm_baobiao : Form
    {
        string id_sel = "";//选中的ID
        int load_over = 0;//初始，防止comb刷新
        public frm_baobiao()
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
            this.Load_data();
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
            this.Load_data();
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
            this.Load_data();
        }

        /// <summary>
        /// 尾页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sph_last_Click(object sender, EventArgs e)
        {
            this.page_index = this.page_count;
            this.Load_data();
        }

        private void comb_rowcount_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.load_over == 1)
            {
                biz_cls.page_rowcount = Utility.ToInt(this.comb_rowcount.Text);
                this.page_index = 1;
                this.Load_data();
            }
        }


        #endregion


        /// <summary>
        /// 格式化grid
        /// </summary>
        private void Format_grid()
        {
            this.dataGridView1.Columns.Clear();


            DataGridViewTextBoxColumn dc1c = new DataGridViewTextBoxColumn
            {
                Name = "id",
                DataPropertyName = "id",
                HeaderText = "编号",
                Width = 60
            };
            dc1c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dc1c.ReadOnly = true;
            dataGridView1.Columns.Add(dc1c);

            DataGridViewTextBoxColumn dc1ex = new DataGridViewTextBoxColumn
            {
                Name = "riqi",
                DataPropertyName = "riqi",
                HeaderText = "时间",
                Width = 90
            };
            dc1ex.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dc1ex.ReadOnly = true;
            dataGridView1.Columns.Add(dc1ex);

            if (this.comb_leixing.Text != "年" || this.comb_leixing.Text == "月")
            {
                DataGridViewTextBoxColumn dc1e = new DataGridViewTextBoxColumn
                {
                    Name = "riqi_se",
                    DataPropertyName = "riqi_se",
                    HeaderText = "日期范围",
                    Width = 180
                };
                dc1e.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dc1e.ReadOnly = true;
                dataGridView1.Columns.Add(dc1e);
            }

            DataGridViewTextBoxColumn dc1m1 = new DataGridViewTextBoxColumn
            {
                Name = "qiliang_s",
                DataPropertyName = "qiliang_s",
                HeaderText = "气量初值\r\nNm³",
                Width = 90
            };
            dc1m1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dc1m1.ReadOnly = true;
            dataGridView1.Columns.Add(dc1m1);

            DataGridViewTextBoxColumn dc1m = new DataGridViewTextBoxColumn
            {
                Name = "qiliang_e",
                DataPropertyName = "qiliang_e",
                HeaderText = "气量终值\r\nNm³",
                Width = 90
            };
            dc1m.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dc1m.ReadOnly = true;
            dataGridView1.Columns.Add(dc1m);

            DataGridViewTextBoxColumn dc1f = new DataGridViewTextBoxColumn
            {
                Name = "qiliang",
                DataPropertyName = "qiliang",
                HeaderText = "用气量\r\nNm³",
                Width = 90
            };
            dc1f.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dc1f.ReadOnly = true;
            dataGridView1.Columns.Add(dc1f);

            DataGridViewTextBoxColumn dc1ea = new DataGridViewTextBoxColumn
            {
                Name = "dianliang_s",
                DataPropertyName = "dianliang_s",
                HeaderText = "电量初值\r\nKwh",
                Width = 90
            };
            dc1ea.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dc1ea.ReadOnly = true;
            dataGridView1.Columns.Add(dc1ea);

            DataGridViewTextBoxColumn dc1ma = new DataGridViewTextBoxColumn
            {
                Name = "dianliang_e",
                DataPropertyName = "dianliang_e",
                HeaderText = "电量终值\r\nKwh",
                Width = 90
            };
            dc1ma.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dc1ma.ReadOnly = true;
            dataGridView1.Columns.Add(dc1ma);

            DataGridViewTextBoxColumn dc1fa = new DataGridViewTextBoxColumn
            {
                Name = "dianliang",
                DataPropertyName = "dianliang",
                HeaderText = "电量\r\nKwh",
                Width = 90
            };
            dc1fa.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dc1fa.ReadOnly = true;
            dataGridView1.Columns.Add(dc1fa);

            DataGridViewTextBoxColumn dc1yaa1 = new DataGridViewTextBoxColumn
            {
                Name = "qidianbi",
                DataPropertyName = "qidianbi_old",
                HeaderText = "原气电比\r\nKwh/Nm³",
                Width = 90
            };
            dc1yaa1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dc1yaa1.ReadOnly = true;
            dataGridView1.Columns.Add(dc1yaa1);

            DataGridViewTextBoxColumn dc1yaa = new DataGridViewTextBoxColumn
            {
                Name = "jienenglv",
                DataPropertyName = "jienenglv",
                HeaderText = "节能率\r\n%",
                Width = 90
            };
            dc1yaa.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dc1yaa.ReadOnly = true;
            dataGridView1.Columns.Add(dc1yaa);

            DataGridViewTextBoxColumn dc1yaa1a = new DataGridViewTextBoxColumn
            {
                Name = "jieyue_dianliang",
                DataPropertyName = "jieyue_dianliang",
                HeaderText = "节约电量\r\nKwh",
                Width = 90
            };
            dc1yaa1a.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dc1yaa1a.ReadOnly = true;
            dataGridView1.Columns.Add(dc1yaa1a);

            DataGridViewTextBoxColumn dc1yaa2 = new DataGridViewTextBoxColumn
            {
                Name = "jieyue_dianfei",
                DataPropertyName = "jieyue_dianfei",
                HeaderText = "节约电费\r\n元",
                Width = 90
            };
            dc1yaa2.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dc1yaa2.ReadOnly = true;
            dataGridView1.Columns.Add(dc1yaa2);

            //DataGridViewTextBoxColumn dc1yaa3 = new DataGridViewTextBoxColumn();
            //dc1yaa3.Name = "dianjia";
            //dc1yaa3.DataPropertyName = "dianjia";
            //dc1yaa3.HeaderText = "dianjia\r\n元";
            //dc1yaa3.Width = 100;
            //dc1yaa3.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //dc1yaa3.ReadOnly = true;
            //dataGridView1.Columns.Add(dc1yaa3);
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        public void Load_data()
        {
            try
            {
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

                switch (this.comb_leixing.Text)
                {
                    case "日":
                        sqlstr = "select * from rpt_ri  where riqi>='" + this.dtp_s.Value.ToString("yyyy-MM-dd") + "' and riqi<='" + this.dtp_e.Value.ToString("yyyy-MM-dd") + "'";
                        sqlstr1 = "select count(*) from rpt_ri   where riqi>='" + this.dtp_s.Value.ToString("yyyy-MM-dd") + "' and riqi<='" + this.dtp_e.Value.ToString("yyyy-MM-dd") + "'";
                        break;
                    case "月":
                        sqlstr = "select * from rpt_yue  where riqi>='" + this.dtp_s.Value.ToString("yyyy-MM") + "' and riqi<='" + this.dtp_e.Value.ToString("yyyy-MM") + "'";
                        sqlstr1 = "select count(*) from rpt_yue   where riqi>='" + this.dtp_s.Value.ToString("yyyy-MM") + "' and riqi<='" + this.dtp_e.Value.ToString("yyyy-MM") + "'";
                        break;
                    case "年":
                        sqlstr = "select * from rpt_nian  where riqi>='" + this.dtp_s.Value.ToString("yyyy") + "' and riqi<='" + this.dtp_e.Value.ToString("yyyy") + "'";
                        sqlstr1 = "select count(*) from rpt_nian   where riqi>='" + this.dtp_s.Value.ToString("yyyy") + "' and riqi<='" + this.dtp_e.Value.ToString("yyyy") + "'";
                        break;

                    default:
                        sqlstr = "select * from rpt_ri  where riqi>='" + this.dtp_s.Value.ToString("yyyy-MM-dd") + "' and riqi<='" + this.dtp_e.Value.ToString("yyyy-MM-dd") + "'";
                        sqlstr1 = "select count(*) from rpt_ri   where riqi>='" + this.dtp_s.Value.ToString("yyyy-MM-dd") + "' and riqi<='" + this.dtp_e.Value.ToString("yyyy-MM-dd") + "'";
                        break;

                }

                //string str_where = "";

                //sqlstr1 += str_where; //查询总记录数sql
                //sqlstr += str_where;



                this.row_total = Utility.ToInt(MySqlHelper.Get_sigle(sqlstr1));  //总记录数
                this.sph_current_page.Text = this.page_index.ToString() + " / " + this.page_count.ToString().ToString();
                this.sph_total.Text = this.row_total.ToString();

                sqlstr += " order by riqi desc";
                int start_index = biz_cls.page_rowcount * (page_index - 1);  //起始记录数
                sqlstr += " limit " + start_index + "," + biz_cls.page_rowcount; //sql
                ds = MySqlHelper.Get_DataSet(sqlstr);


                //绑定GRID
                this.dataGridView1.DataSource = ds.Tables[0];
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

                //定位到编辑项
                this.dataGridView1.ClearSelection();

                if (this.id_sel != "")
                {
                    foreach (DataGridViewRow dr in this.dataGridView1.Rows)
                    {
                        if (dr.Cells["id"].Value.ToString() == id_sel)
                        {
                            dr.Selected = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void frm_baobiao_Load(object sender, EventArgs e)
        {


            this.comb_leixing.Text = "日";
            this.dtp_s.Value = DateTime.Now;
            this.dtp_e.Value = DateTime.Now;

            this.Format_grid();
            this.Load_data();

            this.load_over = 1;
        }

        private void dataGridView1_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count >= 1)
            {
                id_sel = this.dataGridView1.SelectedRows[0].Cells["id"].Value.ToString();
            }
        }

        private void dataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            using (SolidBrush b = new SolidBrush(dataGridView1.RowHeadersDefaultCellStyle.ForeColor))
            {
                e.Graphics.DrawString((e.RowIndex + 1).ToString(System.Globalization.CultureInfo.CurrentCulture), dataGridView1.DefaultCellStyle.Font, b, e.RowBounds.Location.X + 12, e.RowBounds.Location.Y + 4);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Load_data();
        }

        private void pb_chaxun_Click(object sender, EventArgs e)
        {
            this.Format_grid();
            this.Load_data();
        }
    }
}
