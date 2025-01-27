using kyj_project.Common;
using kyj_project.DAL;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace kyj_project
{
    public partial class frm_shebei_zt : Form
    {
        private Image[] StatusImgs; //指示灯状态
        private string zhandian_id { get; set; }//用户ID
        public frm_shebei_zt()
        {
            InitializeComponent();
            this.Text = biz_cls.xiangmu_mingcheng;

            //初始化grid
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
        }

        private void frm_shebei_zt_Load(object sender, EventArgs e)
        {
            StatusImgs = new Image[] { kyj_project.Properties.Resources._gray, kyj_project.Properties.Resources._green };

            this.load_tv();

            this.Format_grid();

            this.Load_data();

            this.timer1.Start();
        }

        /// <summary>
        /// 格式化grid
        /// </summary>
        private void Format_grid()
        {
            //this.dataGridView1.Columns.Clear();


            //DataGridViewTextBoxColumn dc1c = new DataGridViewTextBoxColumn();
            //dc1c.Name = "shebei_id";
            //dc1c.DataPropertyName = "shebei_id";
            //dc1c.HeaderText = "设备编号";
            //dc1c.Width = 90;
            //dc1c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //dc1c.ReadOnly = true;
            //dataGridView1.Columns.Add(dc1c);

            DataGridViewTextBoxColumn dc1f = new DataGridViewTextBoxColumn
            {
                Name = "shebei_mingcheng",
                DataPropertyName = "shebei_mingcheng",
                HeaderText = "设备名称",
                Width = 120
            };
            dc1f.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dc1f.ReadOnly = true;
            dataGridView1.Columns.Add(dc1f);

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

                string sqlstr = "select * from uv_base_shebei where (shebei_leixing_id='lixinji' or shebei_leixing_id='luoganji' or shebei_leixing_id='lengqueji' or shebei_leixing_id='xiganji' or shebei_leixing_id='lengganji')";
                if (Utility.ToObjectString(this.zhandian_id) != "" && this.zhandian_id != "0")
                {
                    sqlstr += " and zhandian_id='" + zhandian_id + "'";
                }
                sqlstr += " order by  shebei_id ";
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

        private void load_tv()
        {
            TreeNode tn = new TreeNode
            {
                Text = "所有站点",
                Tag = 0
            };

            DataSet ds = new DataSet();
            ds = MySqlHelper.Get_DataSet("select zhandian_id,zhandian_mingcheng from base_zhandian order by zhandian_id");
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    TreeNode tn1 = new TreeNode
                    {
                        Tag = Utility.ToObjectString(dr["zhandian_id"]),
                        Text = Utility.ToObjectString(dr["zhandian_mingcheng"])
                    };
                    tn.Nodes.Add(tn1);
                }
            }

            this.treeView1.Nodes.Add(tn);
            this.treeView1.ItemHeight = 32;
            this.treeView1.ExpandAll();
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            dataGridView1.ClearSelection();
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            string sname = this.dataGridView1.Columns[e.ColumnIndex].Name;


            if (sname == "qiting_flag")
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
        }

        private void dataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            using (SolidBrush b = new SolidBrush(dataGridView1.RowHeadersDefaultCellStyle.ForeColor))
            {
                e.Graphics.DrawString((e.RowIndex + 1).ToString(System.Globalization.CultureInfo.CurrentCulture), dataGridView1.DefaultCellStyle.Font, b, e.RowBounds.Location.X + 12, e.RowBounds.Location.Y + 4);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Load_data();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            this.zhandian_id = Utility.ToObjectString(e.Node.Tag);
            this.Load_data();
        }
    }
}
