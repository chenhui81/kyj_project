using kyj_project.Common;
using kyj_project.DAL;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace kyj_project
{
    public partial class frm_tx_mokuai : Form
    {
        string id_sel = "";//选中的ID
        public frm_tx_mokuai()
        {
            InitializeComponent();
            this.Text = biz_cls.xiangmu_mingcheng;

            //初始化grid
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
        }

        private void frm_tx_mokuai_Load(object sender, EventArgs e)
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
                Name = "txmk_id",
                DataPropertyName = "txmk_id",
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
                Width = 150
            };
            dc1e.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dc1e.ReadOnly = true;
            dataGridView1.Columns.Add(dc1e);

            DataGridViewTextBoxColumn dc1m = new DataGridViewTextBoxColumn
            {
                Name = "tx_xieyi",
                DataPropertyName = "tx_xieyi",
                HeaderText = "通讯协议",
                Width = 120
            };
            dc1m.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dc1m.ReadOnly = true;
            dataGridView1.Columns.Add(dc1m);

            DataGridViewTextBoxColumn dc1f = new DataGridViewTextBoxColumn
            {
                Name = "tx_xieyi_connstr",
                DataPropertyName = "tx_xieyi_connstr",
                HeaderText = "协议字符串",
                Width = 200
            };
            dc1f.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dc1f.ReadOnly = true;
            dataGridView1.Columns.Add(dc1f);

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

                string sqlstr = "select * from tx_mokuai order by txmk_id ";

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
            frm_tx_mokuai_edit f = new frm_tx_mokuai_edit
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


            int txmk_id = Utility.ToInt(this.dataGridView1.SelectedRows[0].Cells["txmk_id"].Value);

            frm_tx_mokuai_edit f = new frm_tx_mokuai_edit
            {
                Owner = this,
                txmk_id = txmk_id
            };
            f.ShowDialog();
        }

        private void dataGridView1_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count >= 1)
            {
                id_sel = this.dataGridView1.SelectedRows[0].Cells["txmk_id"].Value.ToString();
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


            int txmk_id = Utility.ToInt(this.dataGridView1.SelectedRows[0].Cells["txmk_id"].Value);

            frm_tx_mokuai_edit f = new frm_tx_mokuai_edit
            {
                Owner = this,
                txmk_id = txmk_id
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


            int txmk_id = Utility.ToInt(this.dataGridView1.SelectedRows[0].Cells["txmk_id"].Value);
            string err_str = biz_cls.txmk_del(txmk_id);
            if (err_str != "")
            {
                MessageBox.Show(err_str);
            }
            else
            {
                this.Load_data();
            }
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count < 1)
            {
                MessageBox.Show("请选择需要启用的通讯模块！");
                return;
            }

            if (this.dataGridView1.SelectedRows.Count > 1)
            {
                MessageBox.Show("只能选择一个通讯模块进行启用！");
                return;
            }


            int txmk_id = Utility.ToInt(this.dataGridView1.SelectedRows[0].Cells["txmk_id"].Value);
            string tx_xieyi = Utility.ToObjectString(this.dataGridView1.SelectedRows[0].Cells["tx_xieyi"].Value);
            string tx_xieyi_connstr = Utility.ToObjectString(this.dataGridView1.SelectedRows[0].Cells["tx_xieyi_connstr"].Value);

            string err_str = "";
            //try
            //{
            //    switch (tx_xieyi)
            //    {
            //        case "PLC S7":
            //            #region  PLC S7连接测试

            //            try
            //            {
            //                if (s7_cls.s7_check_constr(tx_xieyi_connstr) != "")
            //                {
            //                    err_str= "PLC连接字符串格式不正确";
            //                }

            //                Plc  _plc = s7_cls.get_plc(tx_xieyi_connstr);
            //                if (_plc != null)
            //                {
            //                    _plc.Open();
            //                  //  MessageBox.Show("PLC已连接");
            //                    _plc.Close();
            //                }
            //                else
            //                {
            //                    err_str= "PLC连接失败，请检查连接字符串";
            //                }

            //            }
            //            catch (Exception ex)
            //            {
            //                err_str=ex.Message;
            //            }

            //            #endregion

            //            break;
            //        case "MODBUS TCP":

            //            #region  MODBUS TCP连接测试
            //            try
            //            {
            //                string[] s = tx_xieyi_connstr.Split('|');
            //                if (s.Length == 2)
            //                {
            //                    string[] s1 = s[0].Split('.');
            //                    if (s1.Length != 4)
            //                    {
            //                        err_str = "协议字符串格式错误";
            //                    }
            //                }
            //                else
            //                {
            //                    err_str="协议字符串格式错误";

            //                }
            //                ModbusMaster _mm = mtcp_cls.get_mtcp(tx_xieyi_connstr);
            //                if (_mm == null)
            //                {
            //                    err_str="MODBUS TCP连接失败，请检查连接字符串";
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                MessageBox.Show(ex.Message);
            //            }
            //            #endregion

            //            break;
            //        case "MODBUS RTU":

            //            #region MODBUS RTU 连接测试

            //            string[] s3 = tx_xieyi_connstr.Split('|');
            //            if (s3.Length != 5)
            //            {
            //                err_str = "协议字符串格式错误";
            //            }

            //            string portName = s3[0];  // 串口名称
            //            int baudRate = int.Parse(s3[1]);       // 波特率
            //            int parity = int.Parse(s3[2]);            // 校验位，0: 无校验，1: 偶校验，2: 奇校验
            //            int dataBits = int.Parse(s3[3]);          // 数据位
            //            int stopBits = int.Parse(s3[4]);          // 停止位



            //            try
            //            {
            //                //1、打开串口连接
            //                var serialPort = new SerialPort(portName, baudRate, (Parity)parity, dataBits, (StopBits)stopBits);
            //                serialPort.Open();

            //                // 2. 创建 Modbus RTU 主机对象
            //                var modbusRtuMaster = ModbusSerialMaster.CreateRtu(serialPort);
            //                if (modbusRtuMaster == null)
            //                {
            //                    err_str = "MODBUS RTU连接失败，请检查协议字符串";
            //                }
            //                serialPort.Close();
            //            }
            //            catch (Exception ex)
            //            {
            //                err_str = ex.Message;
            //            }

            //            #endregion

            //            break;
            //        default:
            //            break;
            //    }
            //}
            //catch
            //{ 

            //}


            if (err_str != "")
            {
                MessageBox.Show(err_str);
            }
            else
            {
                //将通讯模块设置为启用状态，工作站会自动开始该模块下的采集服务
                MySqlHelper.ExecuteSql("update tx_mokuai set qiyong_flag=1 where txmk_id=" + txmk_id);
                //通讯模块变更后，触发心跳
                MySqlHelper.ExecuteSql("update base_xiangmu set txmk_xintiao=1");

                this.Load_data();
            }



        }
    }
}
