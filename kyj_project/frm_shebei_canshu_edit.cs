using kyj_project.Common;
using kyj_project.DAL;
using kyj_project.Models;
using Modbus.Device;
using S7.Net;
using System;
using System.Data;
using System.IO.Ports;
using System.Windows.Forms;

namespace kyj_project
{
    public partial class frm_shebei_canshu_edit : Form
    {
        public int shebei_id = 0; //设备ID
        public int field_id = 0; //地址ID
        public string shebei_leixing_id = "";//设备类型ID
        public int txmk_id = 0;//通讯模块ID
        private en_tx_mokuai_field en;
        public string shebei_mingcheng = "";
        public string shebei_leixing = "";
        public string zhandian_mingcheng = "";

        public frm_shebei_canshu_edit()
        {
            InitializeComponent();
            this.Text = biz_cls.xiangmu_mingcheng;

        }

        #region 加载数据

        private void bind_txmk()
        {
            DataSet ds = new DataSet();
            ds = MySqlHelper.Get_DataSet("select txmk_id,txmk_mingcheng from tx_mokuai  order by txmk_id");
            DataRow dr = ds.Tables[0].NewRow();
            dr["txmk_id"] = "0";
            dr["txmk_mingcheng"] = "";
            ds.Tables[0].Rows.InsertAt(dr, 0);

            this.comb_txmk.DisplayMember = "txmk_mingcheng";
            this.comb_txmk.ValueMember = "txmk_id";
            this.comb_txmk.DataSource = ds.Tables[0];
        }

        private void bind_zhucanshu()
        {
            DataSet ds = new DataSet();
            ds = MySqlHelper.Get_DataSet("select canshu_field_name,CONCAT(canshu_mingcheng,' ',canshu_danwei) AS mingcheng from base_shebei_leixing_canshu where shebei_leixing_id='" + this.shebei_leixing_id + "' order by canshu_mingcheng");
            DataRow dr = ds.Tables[0].NewRow();
            dr["canshu_field_name"] = "";
            dr["mingcheng"] = "";
            ds.Tables[0].Rows.InsertAt(dr, 0);

            this.comb_zhucanshu.DisplayMember = "mingcheng";
            this.comb_zhucanshu.ValueMember = "canshu_field_name";
            this.comb_zhucanshu.DataSource = ds.Tables[0];
        }

        private void bind_fucanshu()
        {
            string fucanshu = Utility.ToObjectString(MySqlHelper.Get_sigle("select caiji_canshu_str from base_shebei where shebei_id=" + this.shebei_id));
            if (fucanshu != "" && fucanshu != "0")
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("id", typeof(string));
                dt.Columns.Add("name", typeof(string));
                string[] s = fucanshu.Split('|');
                if (s.Length > 0)
                {
                    foreach (string s1 in s)
                    {
                        DataRow dr = dt.NewRow();
                        dr["id"] = Utility.get_jiequ_str(s1, '{', '}');
                        dr["name"] = s1;
                        dt.Rows.Add(dr);
                    }
                }

                DataRow dr1 = dt.NewRow();
                dr1["id"] = "";
                dr1["name"] = "";
                dt.Rows.InsertAt(dr1, 0);

                this.comb_fucanshu.DisplayMember = "name";
                this.comb_fucanshu.ValueMember = "id";
                this.comb_fucanshu.DataSource = dt;
            }
            else
            {
                this.lb_fucanshu.Visible = false;
                this.comb_fucanshu.Visible = false;
            }
        }


        #endregion

        private void frm_shebei_canshu_edit_Load(object sender, EventArgs e)
        {
            this.shebei_leixing_id = Utility.ToObjectString(MySqlHelper.Get_sigle("select shebei_leixing_id from base_shebei where shebei_id=" + this.shebei_id));
            this.lb_title.Text = "设备；【" + shebei_mingcheng + "】  类型：【" + shebei_leixing + "】  站点：【" + zhandian_mingcheng + "】";

            this.bind_txmk();
            this.bind_zhucanshu();
            this.bind_fucanshu();

            this.load_data();
        }

        private void load_data()
        {
            if (this.field_id <= 0)
            {
                en = new en_tx_mokuai_field();
            }
            else
            {
                en = new en_tx_mokuai_field();
                en = biz_cls.get_tx_mokuai_field_byid_en(field_id);

                this.comb_txmk.SelectedValue = en.txmk_id;
                this.comb_zhucanshu.SelectedValue = en.zhucanshu;
                this.comb_fucanshu.SelectedValue = en.fucanshu;
                this.txt_dizhi.Text = en.dizhi;
                if (en.if_xieru == 1) { this.cb_xieru.Checked = true; } else { this.cb_xieru.Checked = false; }
                if (en.value_up > 0) { this.txt_up.Text = en.value_up.ToString("G10"); }
                if (en.value_down > 0) { this.txt_down.Text = en.value_down.ToString("G10"); }

            }


        }

        private void comb_txmk_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Utility.ToObjectString(this.comb_txmk.SelectedValue) != "0")
            {
                this.lb_txmk.Text = Utility.ToObjectString(MySqlHelper.Get_sigle("select concat('协议：',tx_xieyi,'   ',tx_xieyi_connstr) from tx_mokuai where txmk_id='" + Utility.ToObjectString(this.comb_txmk.SelectedValue) + "'"));
                if (this.lb_txmk.Text.ToLower().Trim().Contains("s7"))
                {
                    this.lb_dizhi.Text = "非Bool类型：DB1.DB15|Real\r\nBool类型：1.15.2 | Bool\r\nWord/Int/DWord/DInt/Reel/Bool";
                }
                else if (this.lb_txmk.Text.ToLower().Trim().Contains("modbus"))
                {
                    this.lb_dizhi.Text = "地址参数 从站地址|寄存器|寄存器数量|数据类型（Int/DInt/Real/Bool）|高低位互换（0/1）|位数(-1代表没有)\r\n地址参数样式： 1 | 4100 | 1 | Int | 0 | -1";
                }
                else
                {
                    this.lb_dizhi.Text = "";
                }
            }
            else
            {
                this.lb_txmk.Text = "";
                this.lb_dizhi.Text = "";
            }
        }

        private string add()
        {
            try
            {

                en.txmk_id = Utility.ToInt(this.comb_txmk.SelectedValue);
                en.dizhi = this.txt_dizhi.Text;
                if (Utility.ToObjectString(this.comb_zhucanshu.SelectedValue) != "0")
                {
                    en.zhucanshu = Utility.ToObjectString(this.comb_zhucanshu.SelectedValue);
                    en.zhucanshu_miaoshu = Utility.ToObjectString(this.comb_zhucanshu.Text);
                }
                else
                {
                    en.zhucanshu = "";
                    en.zhucanshu_miaoshu = "";
                }
                if (Utility.ToObjectString(this.comb_fucanshu.SelectedValue) != "0")
                {
                    en.fucanshu = Utility.ToObjectString(this.comb_fucanshu.SelectedValue);
                    en.fucanshu_miaoshu = Utility.ToObjectString(this.comb_fucanshu.Text);
                }
                else
                {
                    en.fucanshu = "";
                    en.fucanshu_miaoshu = "";
                }
                en.shebei_id = this.shebei_id;
                if (this.cb_xieru.Checked) { en.if_xieru = 1; } else { en.if_xieru = 0; }

                if (Utility.ToDecimal(this.txt_up.Text) < 0) { en.value_up = 0; } else { en.value_up = Utility.ToDecimal(this.txt_up.Text); }
                if (Utility.ToDecimal(this.txt_down.Text) < 0) { en.value_down = 0; } else { en.value_down = Utility.ToDecimal(this.txt_down.Text); }

                return biz_cls.tx_mokuai_field_add(en);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string upd()
        {
            try
            {
                en.txmk_id = Utility.ToInt(this.comb_txmk.SelectedValue);
                en.dizhi = this.txt_dizhi.Text;
                if (Utility.ToObjectString(this.comb_zhucanshu.SelectedValue) != "0")
                {
                    en.zhucanshu = Utility.ToObjectString(this.comb_zhucanshu.SelectedValue);
                    en.zhucanshu_miaoshu = Utility.ToObjectString(this.comb_zhucanshu.Text);
                }
                else
                {
                    en.zhucanshu = "";
                    en.zhucanshu_miaoshu = "";
                }
                if (Utility.ToObjectString(this.comb_fucanshu.SelectedValue) != "0")
                {
                    en.fucanshu = Utility.ToObjectString(this.comb_fucanshu.SelectedValue);
                    en.fucanshu_miaoshu = Utility.ToObjectString(this.comb_fucanshu.Text);
                }
                else
                {
                    en.fucanshu = "";
                    en.fucanshu_miaoshu = "";
                }
                en.shebei_id = this.shebei_id;
                if (this.cb_xieru.Checked) { en.if_xieru = 1; } else { en.if_xieru = 0; }

                if (Utility.ToDecimal(this.txt_up.Text) < 0) { en.value_up = 0; } else { en.value_up = Utility.ToDecimal(this.txt_up.Text); }
                if (Utility.ToDecimal(this.txt_down.Text) < 0) { en.value_down = 0; } else { en.value_down = Utility.ToDecimal(this.txt_down.Text); }


                return biz_cls.tx_mokuai_field_upd(en);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void jisuan_canshu_num()
        {
            int zhucanshu_num = Utility.ToInt(MySqlHelper.Get_sigle("select count(*) from uv_tx_mokuai_field where shebei_id=" + this.shebei_id + " and zhucanshu<>''"));
            int fucanshu_num = Utility.ToInt(MySqlHelper.Get_sigle("select count(*) from uv_tx_mokuai_field where shebei_id=" + this.shebei_id + " and fucanshu<>''"));
            MySqlHelper.ExecuteSql("update base_shebei set zhucanshu_num=" + zhucanshu_num + ",fucanshu_num=" + fucanshu_num + " where shebei_id=" + this.shebei_id);
        }

        private bool check()
        {
            string zhucanshu = Utility.ToObjectString(this.comb_zhucanshu.SelectedValue);
            string fucanshu = Utility.ToObjectString(this.comb_fucanshu.SelectedValue);

            if (Utility.ToObjectString(this.comb_txmk.SelectedValue) == "0")
            {
                MessageBox.Show("请选择通讯模块");
                this.comb_txmk.Focus();
                return false;
            }

            if (this.txt_dizhi.Text == "")
            {
                MessageBox.Show("模块地址不能为空");
                this.txt_dizhi.Focus();
                return false;
            }

            #region 模块地址格式校验

            if (this.lb_txmk.Text.Trim().ToLower().Contains("modbus"))
            {
                string errstr = mtcp_cls.modbus_check_dizhi(this.txt_dizhi.Text);
                if (errstr != "")
                {
                    MessageBox.Show("模块地址格式不正确");
                    this.txt_dizhi.Focus();
                    return false;
                }
            }

            if (this.lb_txmk.Text.Trim().ToLower().Contains("s7"))
            {
                string errstr = s7_cls.s7_check_dizhi(this.txt_dizhi.Text);
                if (errstr != "")
                {
                    MessageBox.Show("模块地址格式不正确");
                    this.txt_dizhi.Focus();
                    return false;
                }
            }

            #endregion

            #region 模块地址重复校验

            if (field_id <= 0)
            {
                //新增状态
                if (Utility.ToInt(MySqlHelper.Get_sigle("select count(*) from tx_mokuai_field where txmk_id=" + Utility.ToInt(this.comb_txmk.SelectedValue) + " and dizhi='" + this.txt_dizhi.Text + "'")) > 0)
                {
                    MessageBox.Show("同一通讯模块下，地址重复");
                    this.txt_dizhi.Focus();
                    return false;
                }
            }
            else
            {
                //编辑状态
                if (Utility.ToInt(MySqlHelper.Get_sigle("select count(*) from tx_mokuai_field where txmk_id=" + Utility.ToInt(this.comb_txmk.SelectedValue) + " and dizhi='" + this.txt_dizhi.Text + "' and field_id<>" + this.field_id)) > 0)
                {
                    MessageBox.Show("同一通讯模块下，地址重复");
                    this.txt_dizhi.Focus();
                    return false;
                }
            }

            #endregion

            if (zhucanshu == "" && fucanshu == "")
            {
                MessageBox.Show("主参数和辅参数不能同时为空");
                this.comb_zhucanshu.Focus();
                return false;
            }

            #region 主参数和辅参数重复校验

            if (field_id <= 0)
            {
                //新增状态下判断主参数和辅参数是否重复

                if (zhucanshu != "")
                {
                    if (Utility.ToInt(MySqlHelper.Get_sigle("select count(*) from tx_mokuai_field where shebei_id=" + shebei_id + " and zhucanshu='" + zhucanshu + "'")) > 0)
                    {
                        MessageBox.Show("主参数已经被使用过");
                        this.comb_zhucanshu.Focus();
                        return false;
                    }
                }

                if (fucanshu != "")
                {
                    if (Utility.ToInt(MySqlHelper.Get_sigle("select count(*) from tx_mokuai_field where shebei_id=" + shebei_id + " and fucanshu='" + fucanshu + "'")) > 0)
                    {
                        MessageBox.Show("辅参数已被使用过");
                        this.comb_fucanshu.Focus();
                        return false;
                    }
                }
            }
            else
            {
                //修改状态下判断主参数和辅参数是否重复
                if (zhucanshu != "")
                {
                    if (Utility.ToInt(MySqlHelper.Get_sigle("select count(*) from tx_mokuai_field where field_id<>" + this.field_id + " and shebei_id=" + shebei_id + " and zhucanshu='" + zhucanshu + "'")) > 0)
                    {
                        MessageBox.Show("主参数已经被使用过");
                        this.comb_zhucanshu.Focus();
                        return false;
                    }
                }

                if (fucanshu != "")
                {
                    if (Utility.ToInt(MySqlHelper.Get_sigle("select count(*) from tx_mokuai_field where field_id<>" + this.field_id + " and shebei_id=" + shebei_id + " and fucanshu='" + fucanshu + "'")) > 0)
                    {
                        MessageBox.Show("辅参数已被使用过");
                        this.comb_fucanshu.Focus();
                        return false;
                    }
                }
            }

            #endregion

            if (this.txt_down.Text != "" || this.txt_up.Text != "")
            {
                if (Utility.IsFloat(this.txt_down.Text) == false)
                {
                    MessageBox.Show("应填入数字类型");
                    this.txt_down.Focus();
                    return false;
                }

                if (Utility.IsFloat(this.txt_up.Text) == false)
                {
                    MessageBox.Show("应填入数字类型");
                    this.txt_up.Focus();
                    return false;
                }

                if (Utility.ToDecimal(this.txt_down.Text) >= Utility.ToDecimal(this.txt_up.Text))
                {
                    MessageBox.Show("数值下限应小于上限值");
                    this.txt_down.Focus();
                    return false;
                }
            }

            return true;
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.check() == false) { return; }
                string err_str = "";

                if (field_id == 0)
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
                    MySqlHelper.ExecuteSql("update base_xiangmu set txmk_canshu_xintiao=1");

                    //计算参数数量
                    this.jisuan_canshu_num();

                    frm_shebei_canshu f = (frm_shebei_canshu)this.Owner;

                    f.Load_data();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btn_duqu_Click(object sender, EventArgs e)
        {
            if (Utility.ToObjectString(this.comb_txmk.SelectedValue) == "0")
            {
                MessageBox.Show("请选择通讯模块");
                this.comb_txmk.Focus();
                return;
            }

            if (this.txt_dizhi.Text == "")
            {
                MessageBox.Show("模块地址不能为空");
                this.txt_dizhi.Focus();
                return;
            }

            #region 模块地址格式校验

            if (this.lb_txmk.Text.Trim().ToLower().Contains("modbus"))
            {
                string errstr = mtcp_cls.modbus_check_dizhi(this.txt_dizhi.Text);
                if (errstr != "")
                {
                    MessageBox.Show("模块地址格式不正确");
                    this.txt_dizhi.Focus();
                    return;
                }
            }

            if (this.lb_txmk.Text.Trim().ToLower().Contains("s7"))
            {
                string errstr = s7_cls.s7_check_dizhi(this.txt_dizhi.Text);
                if (errstr != "")
                {
                    MessageBox.Show("模块地址格式不正确");
                    this.txt_dizhi.Focus();
                    return;
                }
            }

            #endregion

            DataSet ds = new DataSet();
            ds = MySqlHelper.Get_DataSet("select tx_xieyi,tx_xieyi_connstr from tx_mokuai where txmk_id=" + Utility.ToInt(this.comb_txmk.SelectedValue));
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                string tx_xieyi = Utility.ToObjectString(dr["tx_xieyi"]);
                string tx_xieyi_connstr = Utility.ToObjectString(dr["tx_xieyi_connstr"]);

                switch (tx_xieyi)
                {
                    case "PLC S7":
                        #region PLC S7数据读取测试
                        try
                        {
                            Plc _plc = s7_cls.get_plc(tx_xieyi_connstr);
                            if (_plc != null)
                            {
                                _plc.Open();

                                string ss = s7_cls.get_plc_value(_plc, this.txt_dizhi.Text);
                                MessageBox.Show("读取值：" + ss);

                                _plc.Close();
                            }
                            else
                            {
                                MessageBox.Show("PLC连接失败，请检查连接字符串");
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }

                        #endregion

                        break;
                    case "MODBUS TCP":
                        #region MODBUS TCP数据读取测试
                        try
                        {
                            ModbusMaster _mm = mtcp_cls.get_mtcp(tx_xieyi_connstr);

                            if (_mm != null)
                            {
                                //读取
                                string s1 = mtcp_cls.get_mtcp_value(_mm, this.txt_dizhi.Text);
                                MessageBox.Show("读取值：" + s1);

                                //断开
                                _mm.Dispose();
                            }
                            else
                            {
                                MessageBox.Show("MODBUS TCP连接失败，请检查连接字符串");
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }

                        #endregion

                        break;
                    case "MODBUS RTU":

                        #region MODBUS RTU数据读取测试
                        string[] s = tx_xieyi_connstr.Split('|');
                        string portName = s[0];  // 串口名称
                        int baudRate = int.Parse(s[1]);       // 波特率
                        int parity = int.Parse(s[2]);            // 校验位，0: 无校验，1: 偶校验，2: 奇校验
                        int dataBits = int.Parse(s[3]);          // 数据位
                        int stopBits = int.Parse(s[4]);          // 停止位

                        try
                        {
                            //1、打开串口连接
                            var serialPort = new SerialPort(portName, baudRate, (Parity)parity, dataBits, (StopBits)stopBits);
                            serialPort.Open();

                            // 2. 创建 Modbus RTU 主机对象
                            var modbusRtuMaster = ModbusSerialMaster.CreateRtu(serialPort);

                            if (modbusRtuMaster != null)
                            {
                                // 3. 读取从站设备的寄存器
                                string s3 = mrtu_cls.get_mrtu_value(modbusRtuMaster, this.txt_dizhi.Text);
                                MessageBox.Show("读取值：" + s3);

                                // 4. 关闭串口连接
                                serialPort.Close();
                            }
                            else
                            {
                                MessageBox.Show("MODBUS RTU连接失败，请检查连接字符串");
                            }


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
}
