using kyj_project.Common;
using kyj_project.DAL;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace kyj_project
{
    public partial class frm_baojing : Form
    {
        string id_sel = "";//选中的ID
        int load_over = 0;//初始，防止comb刷新
        public string baojing_leibie = "";//报警类别
        public frm_baojing()
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
            //this.dataGridView1.Columns.Clear();


            DataGridViewTextBoxColumn dc1c = new DataGridViewTextBoxColumn
            {
                Name = "id",
                DataPropertyName = "id",
                HeaderText = "",
                Width = 0
            };
            dc1c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dc1c.ReadOnly = true;
            dataGridView1.Columns.Add(dc1c);

            DataGridViewTextBoxColumn dc1e = new DataGridViewTextBoxColumn
            {
                Name = "baojing_leixing_mingcheng",
                DataPropertyName = "baojing_leixing_mingcheng",
                HeaderText = "类别",
                Width = 150
            };
            dc1e.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dc1e.ReadOnly = true;
            dataGridView1.Columns.Add(dc1e);

            DataGridViewTextBoxColumn dc1m = new DataGridViewTextBoxColumn
            {
                Name = "title",
                DataPropertyName = "title",
                HeaderText = "内容",
                Width = 400
            };
            dc1m.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dc1m.ReadOnly = true;
            dataGridView1.Columns.Add(dc1m);

            DataGridViewTextBoxColumn dc1f = new DataGridViewTextBoxColumn
            {
                Name = "shijian_s",
                DataPropertyName = "shijian_s",
                HeaderText = "报警时间",
                Width = 150
            };
            dc1f.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dc1f.ReadOnly = true;
            dataGridView1.Columns.Add(dc1f);

            DataGridViewTextBoxColumn dc1y = new DataGridViewTextBoxColumn
            {
                Name = "shijian_e",
                DataPropertyName = "shijian_e",
                HeaderText = "处理时间",
                Width = 150
            };
            dc1y.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dc1y.ReadOnly = true;
            dataGridView1.Columns.Add(dc1y);



            DataGridViewTextBoxColumn dc1x = new DataGridViewTextBoxColumn
            {
                Name = "yonghu_name",
                DataPropertyName = "yonghu_name",
                HeaderText = "确认用户",
                Width = 100
            };
            dc1x.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dc1x.ReadOnly = true;
            dataGridView1.Columns.Add(dc1x);


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

                string sqlstr = "select * from uv_baojing where 1=1 ";
                string sqlstr1 = "select count(*) from uv_baojing where 1=1 ";

                string str_where = "";

                if (this.comb_leixing.Text != "")
                {
                    str_where += " and baojing_leixing_mingcheng='" + this.comb_leixing.Text + "'";
                }




                sqlstr1 += str_where; //查询总记录数sql
                sqlstr += str_where;



                this.row_total = Utility.ToInt(MySqlHelper.Get_sigle(sqlstr1));  //总记录数
                this.sph_current_page.Text = this.page_index.ToString() + " / " + this.page_count.ToString().ToString();
                this.sph_total.Text = this.row_total.ToString();

                sqlstr += " order by shijian_s desc";
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

        private void frm_baojing_Load(object sender, EventArgs e)
        {

            if (this.baojing_leibie != "")
            {
                this.comb_leixing.Text = this.baojing_leibie;
            }

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

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (this.id_sel != "")
            {
                MySqlHelper.ExecuteSql("update baojing set shijian_e='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',yonghu_name='" + biz_cls.yonghu_name + "' where id='" + this.id_sel + "'");
                this.Load_data();
            }
        }

        private void pb_chaxun_Click(object sender, EventArgs e)
        {
            this.Load_data();
        }
    }
}
