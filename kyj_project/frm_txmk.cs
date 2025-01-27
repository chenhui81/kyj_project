using kyj_project.Common;
using kyj_project.DAL;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace kyj_project
{
    public partial class frm_txmk : Form
    {
        private Image[] StatusImgs; //指示灯状态
        public frm_txmk()
        {
            InitializeComponent();
            this.Text = biz_cls.xiangmu_mingcheng;

            //初始化grid
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
        }

        private void frm_txmk_Load(object sender, EventArgs e)
        {
            StatusImgs = new Image[] { kyj_project.Properties.Resources.lianjie2, kyj_project.Properties.Resources.lianjie1 };

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




            DataGridViewTextBoxColumn dc1f = new DataGridViewTextBoxColumn
            {
                Name = "txmk_mingcheng",
                DataPropertyName = "txmk_mingcheng",
                HeaderText = "通讯模块名称",
                Width = 120
            };
            dc1f.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dc1f.ReadOnly = true;
            dataGridView1.Columns.Add(dc1f);

            DataGridViewTextBoxColumn dc1e = new DataGridViewTextBoxColumn
            {
                Name = "tx_xieyi",
                DataPropertyName = "tx_xieyi",
                HeaderText = "通讯协议",
                Width = 150
            };
            dc1e.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dc1e.ReadOnly = true;
            dataGridView1.Columns.Add(dc1e);

            DataGridViewTextBoxColumn dc1c = new DataGridViewTextBoxColumn
            {
                Name = "tx_xieyi_connstr",
                DataPropertyName = "tx_xieyi_connstr",
                HeaderText = "协议格式",
                Width = 300
            };
            dc1c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dc1c.ReadOnly = true;
            dataGridView1.Columns.Add(dc1c);




            // 添加图片列
            DataGridViewImageColumn imageColumn = new DataGridViewImageColumn
            {
                Name = "txmk_flag1",
                DataPropertyName = "txmk_flag",
                HeaderText = "通讯状态",
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

            DataGridViewTextBoxColumn dc1f1 = new DataGridViewTextBoxColumn
            {
                Name = "qiyong_flag",
                DataPropertyName = "qiyong_flag",
                HeaderText = "启用状态",
                Width = 100
            };
            dc1f1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dc1f1.ReadOnly = true;
            dataGridView1.Columns.Add(dc1f1);
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

                string sqlstr = "select * from tx_mokuai order by  txmk_id  ";
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

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            dataGridView1.ClearSelection();
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            string sname = this.dataGridView1.Columns[e.ColumnIndex].Name;


            if (sname == "txmk_flag1")
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

            if (sname == "qiyong_flag")
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
    }
}
