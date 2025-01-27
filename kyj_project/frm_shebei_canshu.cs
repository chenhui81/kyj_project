using kyj_project.Common;
using kyj_project.DAL;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace kyj_project
{
    public partial class frm_shebei_canshu : Form
    {

        public int shebei_id = 0;
        public string shebei_mingcheng = "";
        public string shebei_leixing = "";
        public string zhandian_mingcheng = "";

        string id_sel = "";//选中的ID

        public frm_shebei_canshu()
        {
            InitializeComponent();
            this.Text = biz_cls.xiangmu_mingcheng;

            //初始化grid
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
        }

        private void frm_shebei_canshu_Load(object sender, EventArgs e)
        {
            this.lb_title.Text = "设备；【" + shebei_mingcheng + "】  类型：【" + shebei_leixing + "】  站点：【" + zhandian_mingcheng + "】";

            this.Format_grid();
            this.Load_data();
        }

        /// <summary>
        /// 格式化grid
        /// </summary>
        private void Format_grid()
        {
            //this.dataGridView1.Columns.Clear();


            DataGridViewTextBoxColumn dc1c = new DataGridViewTextBoxColumn
            {
                Name = "field_id",
                DataPropertyName = "field_id",
                HeaderText = "编号",
                Width = 60
            };
            dc1c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dc1c.ReadOnly = true;
            dataGridView1.Columns.Add(dc1c);

            DataGridViewTextBoxColumn dc1e = new DataGridViewTextBoxColumn
            {
                Name = "txmk_mingcheng",
                DataPropertyName = "txmk_mingcheng",
                HeaderText = "通讯模块名称",
                Width = 130
            };
            dc1e.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dc1e.ReadOnly = true;
            dataGridView1.Columns.Add(dc1e);

            DataGridViewTextBoxColumn dc1m = new DataGridViewTextBoxColumn
            {
                Name = "tx_xieyi",
                DataPropertyName = "tx_xieyi",
                HeaderText = "通讯协议",
                Width = 90
            };
            dc1m.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dc1m.ReadOnly = true;
            dataGridView1.Columns.Add(dc1m);

            DataGridViewTextBoxColumn dc1f = new DataGridViewTextBoxColumn
            {
                Name = "tx_xieyi_connstr",
                DataPropertyName = "tx_xieyi_connstr",
                HeaderText = "协议字符串",
                Width = 250
            };
            dc1f.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dc1f.ReadOnly = true;
            dataGridView1.Columns.Add(dc1f);

            DataGridViewTextBoxColumn dc1y = new DataGridViewTextBoxColumn
            {
                Name = "dizhi",
                DataPropertyName = "dizhi",
                HeaderText = "模块地址",
                Width = 150
            };
            dc1y.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dc1y.ReadOnly = true;
            dataGridView1.Columns.Add(dc1y);



            DataGridViewTextBoxColumn dc1x = new DataGridViewTextBoxColumn
            {
                Name = "zhucanshu",
                DataPropertyName = "zhucanshu",
                HeaderText = "主参数",
                Width = 80
            };
            dc1x.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dc1x.ReadOnly = true;
            dataGridView1.Columns.Add(dc1x);

            DataGridViewTextBoxColumn dc1x1 = new DataGridViewTextBoxColumn
            {
                Name = "zhucanshu_miaoshu",
                DataPropertyName = "zhucanshu_miaoshu",
                HeaderText = "主参数描述",
                Width = 150
            };
            dc1x1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dc1x1.ReadOnly = true;
            dataGridView1.Columns.Add(dc1x1);

            DataGridViewTextBoxColumn dca3g = new DataGridViewTextBoxColumn
            {
                Name = "fucanshu",
                DataPropertyName = "fucanshu",
                HeaderText = "辅参数",
                Width = 80
            };
            dca3g.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dca3g.ReadOnly = true;
            dataGridView1.Columns.Add(dca3g);

            DataGridViewTextBoxColumn dca3 = new DataGridViewTextBoxColumn
            {
                Name = "fucanshu_miaoshu",
                DataPropertyName = "fucanshu_miaoshu",
                HeaderText = "辅参数描述",
                Width = 150
            };
            dca3.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dca3.ReadOnly = true;
            dataGridView1.Columns.Add(dca3);

            DataGridViewTextBoxColumn dc1x1a = new DataGridViewTextBoxColumn
            {
                Name = "down_up",
                DataPropertyName = "down_up",
                HeaderText = "数值范围",
                Width = 90
            };
            dc1x1a.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dc1x1a.ReadOnly = true;
            dataGridView1.Columns.Add(dc1x1a);

            DataGridViewTextBoxColumn dca31 = new DataGridViewTextBoxColumn
            {
                Name = "if_xieru",
                DataPropertyName = "if_xieru",
                HeaderText = "是否写入",
                Width = 82
            };
            dca31.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dca31.ReadOnly = true;
            dataGridView1.Columns.Add(dca31);


        }

        /// <summary>
        /// 加载数据
        /// </summary>
        public void Load_data()
        {
            try
            {
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

                string sqlstr = "select *,concat(value_down,'~',value_up) as down_up from uv_tx_mokuai_field  where shebei_id= " + this.shebei_id;

                sqlstr += " order by field_id ";
                ds = MySqlHelper.Get_DataSet(sqlstr);


                //绑定GRID
                this.dataGridView1.DataSource = ds.Tables[0];
                this.toolStripStatusLabel1.Text = "总计" + ds.Tables[0].Rows.Count.ToString() + "条记录";

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

                this.load_canshu_num();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void load_canshu_num()
        {
            string sqlstr = "select zhucanshu_num,zhucanshu_zong,fucanshu_num,fucanshu_zong   from uv_base_shebei  where shebei_id=" + shebei_id;
            DataSet ds = new DataSet();
            ds = MySqlHelper.Get_DataSet(sqlstr);
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                int zhucanshu_num = Utility.ToInt(dr["zhucanshu_num"]);
                int zhucanshu_zong = Utility.ToInt(dr["zhucanshu_zong"]);
                int fucanshu_num = Utility.ToInt(dr["fucanshu_num"]);
                int fucanshu_zong = Utility.ToInt(dr["fucanshu_zong"]);
                this.lb_zhucanshu.Text = "主参数：" + zhucanshu_num.ToString() + "/" + zhucanshu_zong.ToString();
                this.lb_fucanshu.Text = "辅参数：" + fucanshu_num.ToString() + "/" + fucanshu_zong.ToString();

                if (zhucanshu_num < zhucanshu_zong && zhucanshu_zong > 0)
                {
                    this.lb_zhucanshu.ForeColor = Color.Red;
                }
                else
                {
                    this.lb_zhucanshu.ForeColor = Color.White;
                }

                if (fucanshu_num < fucanshu_zong && fucanshu_zong > 0)
                {
                    this.lb_fucanshu.ForeColor = Color.Red;
                }
                else
                {
                    this.lb_fucanshu.ForeColor = Color.White;
                }
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


            int field_id = Utility.ToInt(this.dataGridView1.SelectedRows[0].Cells["field_id"].Value);

            frm_shebei_canshu_edit f = new frm_shebei_canshu_edit
            {
                Owner = this,
                shebei_id = shebei_id,
                field_id = field_id,
                shebei_mingcheng = shebei_mingcheng,
                shebei_leixing = shebei_leixing,
                zhandian_mingcheng = zhandian_mingcheng
            };
            f.ShowDialog();
        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count == 1)
            {
                int field_id = Utility.ToInt(this.dataGridView1.SelectedRows[0].Cells["field_id"].Value);

                frm_shebei_canshu_edit f = new frm_shebei_canshu_edit
                {
                    Owner = this,
                    shebei_id = shebei_id,
                    field_id = field_id,
                    shebei_mingcheng = shebei_mingcheng,
                    shebei_leixing = shebei_leixing,
                    zhandian_mingcheng = zhandian_mingcheng
                };
                f.ShowDialog();
            }
        }

        private void dataGridView1_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count >= 1)
            {
                id_sel = this.dataGridView1.SelectedRows[0].Cells["field_id"].Value.ToString();
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            frm_shebei_canshu_edit f = new frm_shebei_canshu_edit
            {
                Owner = this,
                shebei_id = shebei_id,
                shebei_mingcheng = shebei_mingcheng,
                shebei_leixing = shebei_leixing,
                zhandian_mingcheng = zhandian_mingcheng
            };
            f.ShowDialog();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count < 1)
            {
                MessageBox.Show("请选择需要删除的项！");
                return;
            }

            if (this.dataGridView1.SelectedRows.Count > 1)
            {
                MessageBox.Show("只能选择一项进行删除！");
                return;
            }

            try
            {

                int field_id = Utility.ToInt(this.dataGridView1.SelectedRows[0].Cells["field_id"].Value);

                biz_cls.tx_mokuai_field_del(field_id);

                this.Load_data();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            string sname = this.dataGridView1.Columns[e.ColumnIndex].Name;

            if (sname == "if_xieru")
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

            if (sname == "down_up")
            {
                string str = Utility.ToObjectString(e.Value);
                if (str != "")
                {
                    string[] s = str.Split('~');
                    if (s.Length == 2)
                    {
                        e.Value = Utility.ToDecimal(s[0]).ToString("G10") + "~" + Utility.ToDecimal(s[1]).ToString("G10");
                    }
                    else
                    {
                        e.Value = "";
                    }
                }
            }
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            dataGridView1.ClearSelection();
        }

        private void dataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            using (SolidBrush b = new SolidBrush(dataGridView1.RowHeadersDefaultCellStyle.ForeColor))
            {
                e.Graphics.DrawString((e.RowIndex + 1).ToString(System.Globalization.CultureInfo.CurrentCulture), dataGridView1.DefaultCellStyle.Font, b, e.RowBounds.Location.X + 12, e.RowBounds.Location.Y + 4);
            }
        }
    }
}
