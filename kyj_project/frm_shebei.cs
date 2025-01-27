using kyj_project.Common;
using kyj_project.DAL;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace kyj_project
{
    public partial class frm_shebei : Form
    {
        private Image[] StatusImgs; //指示灯状态
        string id_sel = "";//选中的ID
        int load_over = 0;//初始，防止comb刷新
        public frm_shebei()
        {
            InitializeComponent();

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

        private void frm_shebei_Load(object sender, EventArgs e)
        {
            StatusImgs = new Image[] { kyj_project.Properties.Resources._gray, kyj_project.Properties.Resources._green, kyj_project.Properties.Resources._1pix };

            this.bind_zhandian();
            this.bind_shebei_leixing();

            this.Format_grid();

            this.Load_data();

            this.load_over = 1;
        }

        /// <summary>
        /// 格式化grid
        /// </summary>
        private void Format_grid()
        {
            //this.dataGridView1.Columns.Clear();


            DataGridViewTextBoxColumn dc1c = new DataGridViewTextBoxColumn
            {
                Name = "shebei_id",
                DataPropertyName = "shebei_id",
                HeaderText = "设备编号",
                Width = 90
            };
            dc1c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dc1c.ReadOnly = true;
            dataGridView1.Columns.Add(dc1c);

            DataGridViewTextBoxColumn dc1e = new DataGridViewTextBoxColumn
            {
                Name = "zhandian_mingcheng",
                DataPropertyName = "zhandian_mingcheng",
                HeaderText = "站点名称",
                Width = 100
            };
            dc1e.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dc1e.ReadOnly = true;
            dataGridView1.Columns.Add(dc1e);

            DataGridViewTextBoxColumn dc1m = new DataGridViewTextBoxColumn
            {
                Name = "shebei_leixing_mingcheng",
                DataPropertyName = "shebei_leixing_mingcheng",
                HeaderText = "类型",
                Width = 60
            };
            dc1m.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dc1m.ReadOnly = true;
            dataGridView1.Columns.Add(dc1m);

            DataGridViewTextBoxColumn dc1f = new DataGridViewTextBoxColumn
            {
                Name = "shebei_mingcheng",
                DataPropertyName = "shebei_mingcheng",
                HeaderText = "设备名称",
                Width = 100
            };
            dc1f.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dc1f.ReadOnly = true;
            dataGridView1.Columns.Add(dc1f);

            DataGridViewTextBoxColumn dc1y = new DataGridViewTextBoxColumn
            {
                Name = "f_shebeimingcheng",
                DataPropertyName = "f_shebeimingcheng",
                HeaderText = "父设备名称",
                Width = 100
            };
            dc1y.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dc1y.ReadOnly = true;
            dataGridView1.Columns.Add(dc1y);

            DataGridViewTextBoxColumn dc1x = new DataGridViewTextBoxColumn
            {
                Name = "shebei_miaoshu",
                DataPropertyName = "shebei_miaoshu",
                HeaderText = "设备描述",
                Width = 200
            };
            dc1x.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dc1x.ReadOnly = true;
            dataGridView1.Columns.Add(dc1x);

            DataGridViewTextBoxColumn dca3 = new DataGridViewTextBoxColumn
            {
                Name = "if_nenghaojisuan",
                DataPropertyName = "if_nenghaojisuan",
                HeaderText = "能耗计算",
                Width = 82
            };
            dca3.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dca3.ReadOnly = true;
            dataGridView1.Columns.Add(dca3);

            DataGridViewTextBoxColumn dca1 = new DataGridViewTextBoxColumn
            {
                Name = "if_zongguanyali",
                DataPropertyName = "if_zongguanyali",
                HeaderText = "总管压力",
                Width = 82
            };
            dca1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dca1.ReadOnly = true;
            dataGridView1.Columns.Add(dca1);

            DataGridViewTextBoxColumn dca2 = new DataGridViewTextBoxColumn
            {
                Name = "if_zongguanliuliang",
                DataPropertyName = "if_zongguanliuliang",
                HeaderText = "总管流量",
                Width = 82
            };
            dca2.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dca2.ReadOnly = true;
            dataGridView1.Columns.Add(dca2);

            DataGridViewTextBoxColumn dca22 = new DataGridViewTextBoxColumn
            {
                Name = "qiyong_flag",
                DataPropertyName = "qiyong_flag",
                HeaderText = "是否启用",
                Width = 82
            };
            dca22.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dca22.ReadOnly = true;
            dataGridView1.Columns.Add(dca22);


            DataGridViewTextBoxColumn dca4 = new DataGridViewTextBoxColumn
            {
                Name = "zhucanshu",
                DataPropertyName = "zhucanshu",
                HeaderText = "主参数配置",
                Width = 100
            };
            dca4.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dca4.ReadOnly = true;
            dataGridView1.Columns.Add(dca4);

            DataGridViewTextBoxColumn dca41 = new DataGridViewTextBoxColumn
            {
                Name = "fucanshu",
                DataPropertyName = "fucanshu",
                HeaderText = "辅参数配置",
                Width = 100
            };
            dca41.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dca41.ReadOnly = true;
            dataGridView1.Columns.Add(dca41);


            // 添加图片列
            DataGridViewImageColumn imageColumn = new DataGridViewImageColumn
            {
                Name = "qiting_flag",
                DataPropertyName = "qiting_flag",
                HeaderText = "设备状态",
                ImageLayout = DataGridViewImageCellLayout.Zoom,
                Width = 80
            };
            imageColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            imageColumn.ReadOnly = true;
            dataGridView1.Columns.Add(imageColumn);

            DataGridViewTextBoxColumn dc1d = new DataGridViewTextBoxColumn
            {
                Name = "txshijian",
                DataPropertyName = "txshijian",
                HeaderText = "最新通讯时间",
                Width = 150
            };
            dc1d.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dc1d.ReadOnly = true;
            dataGridView1.Columns.Add(dc1d);


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

                string sqlstr = "select *,CONCAT(zhucanshu_num,'/',zhucanshu_zong) as zhucanshu, CONCAT(fucanshu_num,'/',fucanshu_zong) as fucanshu  from uv_base_shebei  where 1=1 ";
                string sqlstr1 = "select count(*) from uv_base_shebei where 1=1 ";

                string str_where = "";

                if (Utility.ToObjectString(this.comb_zhandian.SelectedValue) != "0")
                {
                    str_where += " and zhandian_id='" + Utility.ToObjectString(this.comb_zhandian.SelectedValue) + "'";
                }
                if (Utility.ToObjectString(this.comb_shebei_leixing.SelectedValue) != "0")
                {
                    str_where += " and shebei_leixing_id='" + Utility.ToObjectString(this.comb_shebei_leixing.SelectedValue) + "'";
                }
                if (this.cb_nenghao.Checked)
                {
                    str_where += " and if_nenghaojisuan=1 ";
                }


                sqlstr1 += str_where; //查询总记录数sql
                sqlstr += str_where;


                this.row_total = Utility.ToInt(MySqlHelper.Get_sigle(sqlstr1));  //总记录数
                this.sph_current_page.Text = this.page_index.ToString() + " / " + this.page_count.ToString().ToString();
                this.sph_total.Text = this.row_total.ToString();

                sqlstr += " order by shebei_id desc";
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
                        if (dr.Cells["shebei_id"].Value.ToString() == id_sel)
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


        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            dataGridView1.ClearSelection();
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            string sname = this.dataGridView1.Columns[e.ColumnIndex].Name;

            if (sname == "if_zongguanyali" || sname == "if_zongguanliuliang" || sname == "if_nenghaojisuan" || sname == "qiyong_flag")
            {
                if (Utility.ToInt(e.Value) == 1)
                {
                    e.Value = "✔";
                }
                else
                {
                    e.Value = "";
                }
            }


            if (sname == "qiting_flag")
            {
                string str = this.dataGridView1.Rows[e.RowIndex].Cells["shebei_leixing_mingcheng"].Value.ToString();
                if (str == "离心机" || str == "螺杆机" || str == "吸干机" || str == "冷干机" || str == "冷却机")
                {
                    if (Utility.ToInt(e.Value) == 1)
                    {
                        e.Value = StatusImgs[1];
                    }
                    else
                    {
                        e.Value = StatusImgs[0];
                    }
                }
                else
                {
                    e.Value = StatusImgs[2];
                }

            }

            if (sname == "zhucanshu" || sname == "fucanshu")
            {
                string[] s = Utility.ToObjectString(e.Value).Split('/');
                if (s.Length == 2)
                {


                    if (Utility.ToInt(s[1]) == 0)
                    {
                        e.Value = "";
                    }
                    else
                    {
                        if (Utility.ToInt(s[0]) < Utility.ToInt(s[1]))
                        {
                            e.CellStyle.ForeColor = Color.Red;
                        }
                        else
                        {
                            e.CellStyle.ForeColor = Color.Black;
                        }
                    }
                }
            }
        }

        private void dataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            using (SolidBrush b = new SolidBrush(dataGridView1.RowHeadersDefaultCellStyle.ForeColor))
            {
                e.Graphics.DrawString((e.RowIndex + 1).ToString(System.Globalization.CultureInfo.CurrentCulture), dataGridView1.DefaultCellStyle.Font, b, e.RowBounds.Location.X + 12, e.RowBounds.Location.Y + 4);
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            frm_shebei_edit f = new frm_shebei_edit
            {
                Owner = this,
                shebei_id = 0
            };
            f.ShowDialog();
        }

        private void dataGridView1_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count >= 1)
            {
                id_sel = this.dataGridView1.SelectedRows[0].Cells["shebei_id"].Value.ToString();
            }
        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count == 1)
            {
                int did = Utility.ToInt(this.dataGridView1.SelectedRows[0].Cells["shebei_id"].Value);

                frm_shebei_edit f = new frm_shebei_edit
                {
                    Owner = this,
                    shebei_id = did
                };
                f.ShowDialog();
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count < 1)
            {
                MessageBox.Show("请选择需要编辑的项！");
                return;
            }

            if (this.dataGridView1.SelectedRows.Count > 1)
            {
                MessageBox.Show("只能选择一项进行编辑！");
                return;
            }


            int did = Utility.ToInt(this.dataGridView1.SelectedRows[0].Cells["shebei_id"].Value);


            frm_shebei_edit f = new frm_shebei_edit
            {
                Owner = this,
                shebei_id = did
            };

            f.ShowDialog();
        }

        private void comb_zhandian_SelectedIndexChanged(object sender, EventArgs e)
        {
            // this.Load_data();
        }

        private void comb_shebei_leixing_SelectedIndexChanged(object sender, EventArgs e)
        {
            // this.Load_data();
        }

        private void cb_nenghao_CheckedChanged(object sender, EventArgs e)
        {
            //  this.Load_data();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count < 1)
            {
                MessageBox.Show("请选择需要编辑的项！");
                return;
            }

            if (this.dataGridView1.SelectedRows.Count > 1)
            {
                MessageBox.Show("只能选择一项进行编辑！");
                return;
            }


            int did = Utility.ToInt(this.dataGridView1.SelectedRows[0].Cells["shebei_id"].Value);
            string shebei_mingcheng = Utility.ToObjectString(this.dataGridView1.SelectedRows[0].Cells["shebei_mingcheng"].Value);
            string zhandian_mingcheng = Utility.ToObjectString(this.dataGridView1.SelectedRows[0].Cells["zhandian_mingcheng"].Value);
            string shebei_leixing_mingcheng = Utility.ToObjectString(this.dataGridView1.SelectedRows[0].Cells["shebei_leixing_mingcheng"].Value);

            frm_shebei_canshu f = new frm_shebei_canshu
            {
                Owner = this,
                shebei_id = did,
                shebei_mingcheng = shebei_mingcheng,
                shebei_leixing = shebei_leixing_mingcheng,
                zhandian_mingcheng = zhandian_mingcheng
            };
            f.Show();
        }

        private void pb_chaxun_Click(object sender, EventArgs e)
        {
            this.Load_data();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count < 1)
            {
                MessageBox.Show("请选择需要查看的设备！");
                return;
            }

            if (this.dataGridView1.SelectedRows.Count > 1)
            {
                MessageBox.Show("只能选择一个设备查看！");
                return;
            }


            int did = Utility.ToInt(this.dataGridView1.SelectedRows[0].Cells["shebei_id"].Value);
            string shebei_mingcheng = Utility.ToObjectString(this.dataGridView1.SelectedRows[0].Cells["shebei_mingcheng"].Value);
            string zhandian_mingcheng = Utility.ToObjectString(this.dataGridView1.SelectedRows[0].Cells["zhandian_mingcheng"].Value);
            string shebei_leixing_mingcheng = Utility.ToObjectString(this.dataGridView1.SelectedRows[0].Cells["shebei_leixing_mingcheng"].Value);

            frm_shebei_shishi f = new frm_shebei_shishi
            {
                Owner = this,
                shebei_id = did,
                shebei_mingcheng = shebei_mingcheng,
                shebei_leixing = shebei_leixing_mingcheng,
                zhandian_mingcheng = zhandian_mingcheng
            };
            f.Show();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count < 1)
            {
                MessageBox.Show("请选择需要删除的设备！");
                return;
            }

            if (this.dataGridView1.SelectedRows.Count > 1)
            {
                MessageBox.Show("只能选择一个设备删除！");
                return;
            }


            int did = Utility.ToInt(this.dataGridView1.SelectedRows[0].Cells["shebei_id"].Value);

            if (Utility.ToInt(MySqlHelper.Get_sigle("select count(*) from tx_mokuai_field where shebei_id=" + did)) > 0)
            {
                MessageBox.Show("此设备下已经配置通讯模块参数，不能删除！");
                return;
            }

            MySqlHelper.ExecuteSql("delete from base_shebei where shebei_id=" + did);

            this.Load_data();
        }
    }
}
