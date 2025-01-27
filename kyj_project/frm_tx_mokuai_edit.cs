using kyj_project.Common;
using kyj_project.DAL;
using kyj_project.Models;
using Modbus.Device;
using S7.Net;
using System;
using System.IO.Ports;
using System.Windows.Forms;

namespace kyj_project
{
    public partial class frm_tx_mokuai_edit : Form
    {
        public int txmk_id = 0;//站点ID
        private en_txmk en;
        ModbusMaster _mm;
        Plc _plc;

        public frm_tx_mokuai_edit()
        {
            InitializeComponent();
            this.Text = biz_cls.xiangmu_mingcheng;
        }

        private void load_data()
        {
            if (txmk_id == 0)
            {
                //新增状态
                this.lb_title.Text = "通讯模块新增";
                this.lb_zt.Text = "";
                en = new en_txmk();
            }
            else
            {
                //编辑状态
                this.lb_title.Text = "通讯模块编辑";
                en = new en_txmk();
                en = biz_cls.get_txmk_byid(txmk_id);
                this.txt_txmk_mingcheng.Text = en.txmk_mingcheng;
                this.txt_constr.Text = en.tx_xieyi_connstr;
                this.comb_xieyi.Text = en.tx_xieyi;
                if (en.qiyong_flag == 1)
                {
                    this.cb_qiyong.Checked = true;
                }
                else
                {
                    this.cb_qiyong.Checked = false;
                }
            }
        }

        private void frm_tx_mokuai_edit_Load(object sender, EventArgs e)
        {
            this.load_data();
        }

        private bool check()
        {
            if (this.txt_txmk_mingcheng.Text == "")
            {
                MessageBox.Show("通讯模块名称不能为空");
                this.txt_txmk_mingcheng.Focus();
                return false;
            }

            if (this.comb_xieyi.Text == "")
            {
                MessageBox.Show("通讯协议不能为空");
                this.comb_xieyi.Focus();
                return false;
            }

            if (this.txt_constr.Text == "")
            {
                MessageBox.Show("协议字符串不能为空");
                this.txt_constr.Focus();
                return false;
            }

            if (this.lb_zt.Text == "")
            {
                MessageBox.Show("请选择正确的通讯协议");
                this.comb_xieyi.Focus();
                return false;
            }

            #region 通讯协议校验

            switch (this.comb_xieyi.Text)
            {
                case "PLC S7":
                    string str = s7_cls.s7_check_constr(this.txt_constr.Text);
                    if (str != "")
                    {
                        MessageBox.Show("协议字符串格式错误");
                        this.txt_constr.Focus();
                        return false;
                    }
                    break;
                case "MODBUS TCP":
                    string[] s = this.txt_constr.Text.Split('|');
                    if (s.Length == 2)
                    {
                        string[] s1 = s[0].Split('.');
                        if (s1.Length != 4)
                        {
                            MessageBox.Show("协议字符串格式错误");
                            this.txt_constr.Focus();
                            return false;
                        }
                    }
                    else
                    {
                        MessageBox.Show("协议字符串格式错误");
                        this.txt_constr.Focus();
                        return false;
                    }
                    break;
                case "MODBUS RTU":
                    string[] s2 = this.txt_constr.Text.Split('|');
                    if (s2.Length != 5)
                    {
                        MessageBox.Show("协议字符串格式错误");
                        this.txt_constr.Focus();
                        return false;
                    }
                    break;
                default:
                    break;
            }

            #endregion



            return true;
        }

        private string add()
        {
            en.txmk_mingcheng = this.txt_txmk_mingcheng.Text;
            en.tx_xieyi = this.comb_xieyi.Text;
            en.tx_xieyi_connstr = this.txt_constr.Text;
            en.txmk_flag = 0;
            en.txshijian = "";
            if (this.cb_qiyong.Checked == true)
            {
                en.qiyong_flag = 1;
            }
            else
            {
                en.qiyong_flag = 0;
            }

            return biz_cls.txmk_add(en);
        }

        private string upd()
        {
            en.txmk_mingcheng = this.txt_txmk_mingcheng.Text;
            en.tx_xieyi = this.comb_xieyi.Text;
            en.tx_xieyi_connstr = this.txt_constr.Text;
            if (this.cb_qiyong.Checked == true)
            {
                en.qiyong_flag = 1;
            }
            else
            {
                en.qiyong_flag = 0;
            }

            return biz_cls.txmk_upd(en);
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.check() == false) { return; }
                string err_str = "";

                if (txmk_id == 0)
                {
                    err_str = this.add();
                }
                else
                {
                    err_str = this.upd();
                }

                if (err_str != "")
                {
                    MessageBox.Show(err_str);
                }
                else
                {
                    //通讯模块变更后，触发心跳
                    MySqlHelper.ExecuteSql("update base_xiangmu set txmk_xintiao=1");

                    frm_tx_mokuai f = (frm_tx_mokuai)this.Owner;
                    f.Load_data();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void comb_xieyi_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comb_xieyi_TextChanged(object sender, EventArgs e)
        {
            switch (this.comb_xieyi.Text)
            {
                case "PLC S7":
                    this.lb_zt.Text = "格式：规格型号|IP|机架号|插槽号\r\n例如：S7 - 1200 | 192.168.0.1 | 0 | 1";
                    break;
                case "MODBUS TCP":
                    this.lb_zt.Text = "格式：IP|端口号\r\n例如：192.168.0.1|502";
                    break;
                case "MODBUS RTU":
                    this.lb_zt.Text = "格式：串口号|波特率|校验位|数据位|停止位\r\n例如：COM1|9600|0|8|1";
                    break;
                default:
                    this.lb_zt.Text = "";
                    break;
            }
        }

        private void btn_con_Click(object sender, EventArgs e)
        {
            if (this.comb_xieyi.Text == "")
            {
                MessageBox.Show("通讯协议不能为空");
                this.comb_xieyi.Focus();
                return;
            }

            if (this.txt_constr.Text == "")
            {
                MessageBox.Show("协议字符串不能为空");
                this.txt_constr.Focus();
                return;
            }

            if (this.lb_zt.Text == "")
            {
                MessageBox.Show("请选择正确的通讯协议");
                this.comb_xieyi.Focus();
                return;
            }

            switch (this.comb_xieyi.Text)
            {
                case "PLC S7":
                    #region  PLC S7连接测试

                    try
                    {
                        if (s7_cls.s7_check_constr(this.txt_constr.Text) != "")
                        {
                            MessageBox.Show("PLC连接字符串格式不正确");
                            this.txt_constr.Focus();
                            return;
                        }

                        _plc = s7_cls.get_plc(this.txt_constr.Text);
                        if (_plc != null)
                        {
                            _plc.Open();
                            MessageBox.Show("PLC已连接");
                            _plc.Close();
                        }
                        else
                        {
                            MessageBox.Show("PLC连接失败，请检查连接字符串");
                            this.txt_constr.Focus();
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        this.txt_constr.Focus();
                    }

                    #endregion

                    break;
                case "MODBUS TCP":

                    #region  MODBUS TCP连接测试
                    try
                    {
                        string[] s = this.txt_constr.Text.Split('|');
                        if (s.Length == 2)
                        {
                            string[] s1 = s[0].Split('.');
                            if (s1.Length != 4)
                            {
                                MessageBox.Show("协议字符串格式错误");
                                this.txt_constr.Focus();
                                return;
                            }
                        }
                        else
                        {
                            MessageBox.Show("协议字符串格式错误");
                            this.txt_constr.Focus();
                            return;

                        }
                        _mm = mtcp_cls.get_mtcp(this.txt_constr.Text);
                        if (_mm != null)
                        {
                            MessageBox.Show("MODBUS TCP已连接");
                            _mm.Dispose();
                        }
                        else
                        {
                            MessageBox.Show("MODBUS TCP连接失败，请检查连接字符串");
                            this.txt_constr.Focus();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    #endregion

                    break;
                case "MODBUS RTU":

                    #region MODBUS RTU 连接测试

                    string[] s3 = this.txt_constr.Text.Split('|');
                    if (s3.Length != 5)
                    {
                        MessageBox.Show("协议字符串格式错误");
                        this.txt_constr.Focus();
                        return;
                    }

                    string portName = s3[0];  // 串口名称
                    int baudRate = int.Parse(s3[1]);       // 波特率
                    int parity = int.Parse(s3[2]);            // 校验位，0: 无校验，1: 偶校验，2: 奇校验
                    int dataBits = int.Parse(s3[3]);          // 数据位
                    int stopBits = int.Parse(s3[4]);          // 停止位



                    try
                    {
                        //1、打开串口连接
                        var serialPort = new SerialPort(portName, baudRate, (Parity)parity, dataBits, (StopBits)stopBits);
                        serialPort.Open();

                        // 2. 创建 Modbus RTU 主机对象
                        var modbusRtuMaster = ModbusSerialMaster.CreateRtu(serialPort);
                        if (modbusRtuMaster != null)
                        {
                            MessageBox.Show("MODBUS RTU已连接");
                        }
                        else
                        {
                            MessageBox.Show("MODBUS RTU连接失败,请检查协议字符串格式");
                            this.txt_constr.Focus();
                        }
                        serialPort.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                    #endregion

                    break;
                default:
                    break;
            }
        }
    }
}
