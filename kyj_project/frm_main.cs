using kyj_project.Common;
using kyj_project.DAL;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace kyj_project
{
    public partial class frm_main : Form
    {
        public DataTable dt_base;
        public DataTable dt_mokuai;

        private Image[] StatusImgs; //指示灯状态


        public frm_main()
        {
            InitializeComponent();

            #region 系统初始化

            //获取项目信息
            DataSet dsx = new DataSet();
            dsx = MySqlHelper.Get_DataSet("select xiangmu_id,xiangmu_mingcheng,client_mac from base_xiangmu limit 1");
            if (dsx.Tables[0].Rows.Count > 0)
            {
                DataRow dr = dsx.Tables[0].Rows[0];
                biz_cls.xiangmu_id = Utility.ToObjectString(dr["xiangmu_id"]);
                biz_cls.xiangmu_mingcheng = Utility.ToObjectString(dr["xiangmu_mingcheng"]);
                //  biz_cls.client_mac = Utility.ToObjectString(dr["client_mac"]);
            }


            //加载工作站信息
            this.show_client();


            #endregion

            this.Text = biz_cls.xiangmu_mingcheng;

            //初始化grid
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;

        }








        private void frm_main_Load(object sender, EventArgs e)
        {

            StatusImgs = new Image[] { kyj_project.Properties.Resources._red, kyj_project.Properties.Resources._green, kyj_project.Properties.Resources._gray, kyj_project.Properties.Resources.lianjie1, kyj_project.Properties.Resources.lianjie2, kyj_project.Properties.Resources.computer1, kyj_project.Properties.Resources.computer2, kyj_project.Properties.Resources.server1, kyj_project.Properties.Resources.server2, kyj_project.Properties.Resources.alarm1, kyj_project.Properties.Resources.alarm2 };




            this.Format_grid();

            this.Load_data();

            //桌面定时监控
            this.jiankong();

            this.timer1.Start();


        }

        private void frm_main_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Environment.Exit(0);
            Application.Exit();
        }

        private void 通讯模块ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frm_txmk f = new frm_txmk();
            f.Show();
        }

        private void 站点设备ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frm_shebei_zt f = new frm_shebei_zt();
            f.Show();
        }

        private void 设备清单ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frm_shebei f = new frm_shebei();
            f.Show();
        }

        private void pLCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frm_plc_test f = new frm_plc_test();
            f.Show();
        }

        private void modebusTCPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frm_mbus_tcp_test f = new frm_mbus_tcp_test();
            f.Show();
        }

        private void mODBUSRTUToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frm_mbus_rtu_test f = new frm_mbus_rtu_test();
            f.Show();
        }

        private void 参数字段映射ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frm_zhandian f = new frm_zhandian();
            f.Show();
        }

        private void 模块管理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frm_tx_mokuai f = new frm_tx_mokuai();
            f.Show();
        }

        private void 获取MAC地址ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frm_mac f = new frm_mac();
            f.Show();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //桌面定时监控
            this.jiankong();

            this.Load_data();
        }

        /// <summary>
        /// 工作站、服务器状态和报警监控
        /// </summary>
        private void jiankong()
        {
            try
            {
                //获取客户端心跳
                DataSet dsc = new DataSet();
                dsc = MySqlHelper.Get_DataSet("select * from client");
                if (dsc.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in dsc.Tables[0].Rows)
                    {
                        DateTime client_time = Utility.ToDateTime(dr["xintiao"]);
                        string cid = Utility.ToObjectString(dr["id"]);
                        PictureBox pb = (PictureBox)this.panel2.Controls.Find("pic_client" + cid, false)[0];
                        if (client_time > DateTime.Now.AddSeconds(-10))
                        {

                            pb.BackgroundImage = StatusImgs[5];
                        }
                        else
                        {
                            pb.BackgroundImage = StatusImgs[6];
                        }
                    }
                }

                //获取服务端心跳
                DateTime dt_server = Utility.ToDateTime(MySqlHelper.Get_sigle("select server_xintiao from base_xiangmu limit 1"));
                if (dt_server > DateTime.Now.AddSeconds(-10))
                {
                    this.pic_server.BackgroundImage = StatusImgs[7];
                }
                else
                {
                    this.pic_server.BackgroundImage = StatusImgs[8];
                }

                //获取客户端报警
                int i_client = Utility.ToInt(MySqlHelper.Get_sigle("select count(*) from baojing where baojing_leixing_id=1 and shijian_e=''"));
                if (i_client > 0)
                {
                    this.pic1.BackgroundImage = StatusImgs[10];
                    this.lb1.Text = i_client.ToString();
                }
                else
                {
                    this.pic1.BackgroundImage = StatusImgs[9];
                    this.lb1.Text = "";
                }

                //获取服务端报警
                int i_server = Utility.ToInt(MySqlHelper.Get_sigle("select count(*) from baojing where baojing_leixing_id=2 and shijian_e=''"));
                if (i_server > 0)
                {
                    this.pic2.BackgroundImage = StatusImgs[10];
                    this.lb2.Text = i_server.ToString();
                }
                else
                {
                    this.pic2.BackgroundImage = StatusImgs[9];
                    this.lb2.Text = "";
                }

                //通讯模块异常报警
                int i_tx = Utility.ToInt(MySqlHelper.Get_sigle("select count(*) from baojing where baojing_leixing_id=3 and shijian_e=''"));
                if (i_tx > 0)
                {
                    this.pic3.BackgroundImage = StatusImgs[10];
                    this.lb3.Text = i_tx.ToString();
                }
                else
                {
                    this.pic3.BackgroundImage = StatusImgs[9];
                    this.lb3.Text = "";
                }

                //参数读取异常报警
                int i_tx4 = Utility.ToInt(MySqlHelper.Get_sigle("select count(*) from baojing where baojing_leixing_id=4 and shijian_e=''"));
                if (i_tx4 > 0)
                {
                    this.pic4.BackgroundImage = StatusImgs[10];
                    this.lb4.Text = i_tx4.ToString();
                }
                else
                {
                    this.pic4.BackgroundImage = StatusImgs[9];
                    this.lb4.Text = "";
                }

                //参数范围超限报警
                int i_tx45 = Utility.ToInt(MySqlHelper.Get_sigle("select count(*) from baojing where baojing_leixing_id=5 and shijian_e=''"));
                if (i_tx45 > 0)
                {
                    this.pic5.BackgroundImage = StatusImgs[10];
                    this.lb5.Text = i_tx45.ToString();
                }
                else
                {
                    this.pic5.BackgroundImage = StatusImgs[9];
                    this.lb5.Text = "";
                }
            }
            catch (Exception ex)
            {
                biz_cls.write_log("系统日志", "采集系统", "jiankong#" + ex.ToString(), "");
            }
        }

        private void 系统日志ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frm_rizhi f = new frm_rizhi();
            f.Show();
        }

        private void 报警记录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frm_baojing f = new frm_baojing();
            f.Show();
        }

        private void pic1_Click(object sender, EventArgs e)
        {
            frm_baojing f = new frm_baojing
            {
                baojing_leibie = "工作站断开"
            };
            f.Show();
        }


        private void pic2_Click(object sender, EventArgs e)
        {
            frm_baojing f = new frm_baojing
            {
                baojing_leibie = "服务器断开"
            };
            f.Show();
        }

        private void pic3_Click(object sender, EventArgs e)
        {
            frm_baojing f = new frm_baojing
            {
                baojing_leibie = "通讯模块异常"
            };
            f.Show();
        }

        private void pic4_Click(object sender, EventArgs e)
        {
            frm_baojing f = new frm_baojing
            {
                baojing_leibie = "参数读取异常"
            };
            f.Show();
        }

        private void pic5_Click(object sender, EventArgs e)
        {
            frm_baojing f = new frm_baojing
            {
                baojing_leibie = "参数范围超限"
            };
            f.Show();
        }


        /// <summary>
        /// 格式化grid
        /// </summary>
        private void Format_grid()
        {
            //this.dataGridView1.Columns.Clear();

            DataGridViewTextBoxColumn dc1f1 = new DataGridViewTextBoxColumn
            {
                Name = "txmk_id",
                DataPropertyName = "txmk_id",
                HeaderText = "编号",
                Width = 60
            };
            dc1f1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dc1f1.ReadOnly = true;
            dataGridView1.Columns.Add(dc1f1);


            DataGridViewTextBoxColumn dc1f = new DataGridViewTextBoxColumn
            {
                Name = "txmk_mingcheng",
                DataPropertyName = "txmk_mingcheng",
                HeaderText = "通讯模块名称",
                Width = 150
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

            DataGridViewTextBoxColumn dc1faa = new DataGridViewTextBoxColumn
            {
                Name = "qiyong_flag",
                DataPropertyName = "qiyong_flag",
                HeaderText = "启用状态",
                Width = 100
            };
            dc1faa.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dc1faa.ReadOnly = true;
            dataGridView1.Columns.Add(dc1faa);
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

            if (sname == "txmk_id")
            {
                if (this.txmk_in_client(Utility.ToInt(e.Value)) == true)
                {
                    this.dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Linen;
                }
            }

            if (sname == "txmk_flag1")
            {
                if (Utility.ToInt(e.Value) == 1)
                {
                    e.Value = StatusImgs[3];
                }
                else
                {
                    e.Value = StatusImgs[4];
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

        private void 气电比ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frm_qidianbi f = new frm_qidianbi();
            f.Show();
        }

        private void 统计报表ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frm_baobiao f = new frm_baobiao();
            f.Show();
        }

        private void 数据分析ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frm_fenxi f = new frm_fenxi();
            f.Show();
        }

        /// <summary>
        /// 判断通讯模块是否在此工作站中
        /// </summary>
        /// <param name="_txmk_id"></param>
        /// <returns></returns>
        private bool txmk_in_client(int _txmk_id)
        {
            if (biz_cls.client_txmk_ids != null && biz_cls.client_txmk_ids != "")
            {
                foreach (string s in biz_cls.client_txmk_ids.Split(','))
                {
                    if (_txmk_id == Utility.ToInt(s))
                    {
                        return true;
                    }
                }
            }

            return false;

        }


        private void show_client()
        {

            DataSet ds = new DataSet();
            ds = MySqlHelper.Get_DataSet("select * from client");
            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    string client_name = Utility.ToObjectString(dr["client_mingcheng"]);
                    int id = Utility.ToInt(dr["id"]);

                    PictureBox pb = new PictureBox
                    {
                        BackgroundImage = global::kyj_project.Properties.Resources.computer2,
                        BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom,
                        Location = new System.Drawing.Point(53 + 116 * (i + 1), 57),
                        Name = "pic_client" + id.ToString(),
                        Size = new System.Drawing.Size(65, 85),
                        TabStop = false
                    };

                    this.panel2.Controls.Add(pb);

                    Label lb = new Label
                    {
                        AutoSize = true,
                        Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 134),
                        Location = new System.Drawing.Point(60 + 116 * (i + 1), 146),
                        Name = "lb" + id.ToString(),
                        Size = new System.Drawing.Size(58, 22),
                        Text = client_name
                    };

                    this.panel2.Controls.Add(lb);
                }

            }
        }
    }
}
