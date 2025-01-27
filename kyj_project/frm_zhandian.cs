using kyj_project.Common;
using kyj_project.DAL;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace kyj_project
{
    public partial class frm_zhandian : Form
    {
        string id_sel = "";//选中的ID
        public frm_zhandian()
        {
            InitializeComponent();
            this.Text = biz_cls.xiangmu_mingcheng;

            //初始化grid
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
        }

        private void frm_zhandian_Load(object sender, EventArgs e)
        {
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
                Name = "zhandian_id",
                DataPropertyName = "zhandian_id",
                HeaderText = "编号",
                Width = 60
            };
            dc1c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dc1c.ReadOnly = true;
            dataGridView1.Columns.Add(dc1c);

            DataGridViewTextBoxColumn dc1e = new DataGridViewTextBoxColumn
            {
                Name = "zhandian_mingcheng",
                DataPropertyName = "zhandian_mingcheng",
                HeaderText = "站点名称",
                Width = 150
            };
            dc1e.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dc1e.ReadOnly = true;
            dataGridView1.Columns.Add(dc1e);

            DataGridViewTextBoxColumn dc1m = new DataGridViewTextBoxColumn
            {
                Name = "mubiaoyali",
                DataPropertyName = "mubiaoyali",
                HeaderText = "目标压力Mpa",
                Width = 120
            };
            dc1m.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dc1m.ReadOnly = true;
            dataGridView1.Columns.Add(dc1m);

            DataGridViewTextBoxColumn dc1f = new DataGridViewTextBoxColumn
            {
                Name = "liandong_flag",
                DataPropertyName = "liandong_flag",
                HeaderText = "联动控制",
                Width = 90
            };
            dc1f.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dc1f.ReadOnly = true;
            dataGridView1.Columns.Add(dc1f);

            DataGridViewTextBoxColumn dc1y = new DataGridViewTextBoxColumn
            {
                Name = "zutaitu_url",
                DataPropertyName = "zutaitu_url",
                HeaderText = "组态图URL",
                Width = 500
            };
            dc1y.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dc1y.ReadOnly = true;
            dataGridView1.Columns.Add(dc1y);




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

                string sqlstr = "select * from base_zhandian order by zhandian_id ";

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


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            frm_zhandian_edit f = new frm_zhandian_edit
            {
                Owner = this
            };
            f.Show();
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


            string zid = Utility.ToObjectString(this.dataGridView1.SelectedRows[0].Cells["zhandian_id"].Value);

            frm_zhandian_edit f = new frm_zhandian_edit
            {
                Owner = this,
                zhandian_id = zid
            };
            f.ShowDialog();
        }

        private void dataGridView1_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count >= 1)
            {
                id_sel = this.dataGridView1.SelectedRows[0].Cells["zhandian_id"].Value.ToString();
            }
        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
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


            string zid = Utility.ToObjectString(this.dataGridView1.SelectedRows[0].Cells["zhandian_id"].Value);

            frm_zhandian_edit f = new frm_zhandian_edit
            {
                Owner = this,
                zhandian_id = zid
            };
            f.ShowDialog();
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


        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            string sname = this.dataGridView1.Columns[e.ColumnIndex].Name;

            if (sname == "liandong_flag")
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


            string zid = Utility.ToObjectString(this.dataGridView1.SelectedRows[0].Cells["zhandian_id"].Value);
            string err_str = biz_cls.zhandian_del(zid);
            if (err_str != "")
            {
                MessageBox.Show(err_str);
            }
            else
            {
                this.Load_data();
            }
        }
    }
}
