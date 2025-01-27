using kyj_project.Common;
using kyj_project.DAL;
using Modbus.Device;
using S7.Net;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kyj_project
{
    public partial class frm_main_new : Form
    {
        public DataTable dt_base;
        public DataTable dt_mokuai;
        private int timer_flag = 0; //控制timer定时器，1启动0停止
        private System.Threading.Timer _timer;
        private Image[] StatusImgs; //指示灯状态


        public frm_main_new()
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

            //获取主机信息
            this.get_mac();

            //加载工作站信息
            this.show_client();

            //获取站点模块列表
            this.get_mokuai();

            //获取地址参数对应字段table，uv_tx_mokuai_field相关数据
            this.get_mokuai_field();

            #endregion

            this.Text = biz_cls.xiangmu_mingcheng;

            //初始化grid
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;


            // 创建一个间隔为1秒的Timer
            _timer = new System.Threading.Timer(
                async _ => await DoAsyncWork(),
                null,
                0,
                1000);

        }


        private async System.Threading.Tasks.Task DoAsyncWork()
        {

            // 异步工作，不会阻塞UI线程
            await Task.Run(() =>
            {
                try
                {
                    #region 长时间运行的任务

                    if (timer_flag == 1)
                    {
                        ////每分钟定时
                        if (DateTime.Now.Second == 0)
                        {
                            this.caiji_dingshi();
                        }

                        ////循环
                        if (DateTime.Now.Second == 30)
                        {
                            string s1 = this.caiji_xunhuan();
                            MySqlHelper.ExecuteSql("insert into caiji_zt values(0,2,0,'" + s1 + "',''," + biz_cls.client_id + ",'" + biz_cls.client_zhandian_ids + "');");
                        }

                        //if (DateTime.Now.Second == 40)
                        //{
                        //    string s2 = this.caiji_xunhuan();
                        //    MySqlHelper.ExecuteSql("insert into caiji_zt values(0,3,0,'" + s2 + "','',"+ biz_cls.client_id +");");
                        //}



                        // 工作站客户端心跳、服务端心跳检测
                        if (DateTime.Now.Second % 5 == 4)
                        {
                            StringBuilder sb = new StringBuilder();
                            DateTime dt_server = Utility.ToDateTime(MySqlHelper.Get_sigle("select server_xintiao from base_xiangmu limit 1"));
                            if (dt_server <= DateTime.Now.AddSeconds(-10))
                            {
                                //服务端断线
                                sb.Append("insert into baojing_detail values('" + Guid.NewGuid().ToString() + "',2,'服务器断开','','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "');");
                            }
                            // sb.Append("update base_xiangmu set client_xintiao='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' ");
                            sb.Append("update client set xintiao='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where id=" + biz_cls.client_id + ";");
                            MySqlHelper.ExecuteSql(sb.ToString());
                        }

                    }

                    #endregion

                }
                catch (Exception ex)
                {
                    biz_cls.write_log("系统日志", "采集系统", "DoAsyncWork#" + ex.ToString(), "");
                }



            });




        }

        #region 每分钟定时采集1次

        private StringBuilder caiji_dingshi_s7(string shijian, int txmk_id, string txmk_mingcheng, string tx_xieyi_connstr)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                Plc _plc = s7_cls.get_plc(tx_xieyi_connstr);
                if (_plc != null)
                {

                    #region 打开PLC
                    try
                    {
                        _plc.Open();
                        //更新模块联通状态
                        sb.Append("update tx_mokuai set txmk_flag=1,txshijian='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where txmk_id=" + txmk_id + "; ");
                    }
                    catch
                    {
                        //通讯模块连接异常报警
                        sb.Append("insert into baojing_detail values('" + Guid.NewGuid().ToString() + "',3,'" + txmk_mingcheng + "','','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "');");
                        //连接失败，停用通讯模块，将通讯模块连接状态改为0
                        sb.Append("update tx_mokuai set txmk_flag=0,txshijian='',qiyong_flag=0 where txmk_id=" + txmk_id + ";");
                        return sb;
                    }
                    #endregion

                    #region 读取模块地址


                    //读取除了开关量之外的主参数
                    DataRow[] drs1 = dt_base.Select(" txmk_id=" + txmk_id + " and zhucanshu<>'' and zhucanshu<>'kaiguan' and dizhi<>''");
                    foreach (DataRow dr1 in drs1)
                    {
                        int field_id = Utility.ToInt(dr1["field_id"]);
                        string dizhi = Utility.ToObjectString(dr1["dizhi"]);
                        decimal value_down = Utility.ToDecimal(dr1["value_down"]);
                        decimal value_up = Utility.ToDecimal(dr1["value_up"]);
                        string shebei_mingcheng = Utility.ToObjectString(dr1["shebei_mingcheng"]);
                        string zhucanshu = Utility.ToObjectString(dr1["zhucanshu"]);
                        int shebei_id = Utility.ToInt(dr1["shebei_id"]);
                        string shebei_leixing_id = Utility.ToObjectString(dr1["shebei_leixing_id"]);

                        string str = "设备名称[" + shebei_mingcheng + "]主参数[" + zhucanshu + "]";

                        try
                        {
                            //读取相应地址数据
                            decimal zhi = Utility.ToDecimal(s7_cls.get_plc_value(_plc, dizhi));
                            sb.Append("insert into caiji_base values ('" + Guid.NewGuid().ToString() + "'," + field_id + "," + zhi + ",'" + shijian + "'," + shebei_id + ",'" + shebei_leixing_id + "','" + zhucanshu + "');");

                            if (value_down > 0)
                            {
                                if (zhi < value_down || zhi > value_up)
                                {
                                    //数值超限报警
                                    sb.Append("insert into baojing_detail values('" + Guid.NewGuid().ToString() + "',5,'" + str + "','','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "');");
                                }
                            }
                        }
                        catch
                        {
                            //地址读取错误报警
                            sb.Append("insert into baojing_detail values('" + Guid.NewGuid().ToString() + "',4,'" + str + "','','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "');");
                        }
                    }


                    #endregion

                    //关闭
                    _plc.Close();


                }
                else
                {
                    //通讯模块连接异常报警
                    sb.Append("insert into baojing_detail values('" + Guid.NewGuid().ToString() + "',3,'" + txmk_mingcheng + "','','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "');");
                    //连接失败，停用通讯模块，将通讯模块连接状态改为0
                    sb.Append("update tx_mokuai set txmk_flag=0,txshijian='',qiyong_flag=0 where txmk_id=" + txmk_id + ";");
                    return sb;
                }

                return sb;
            }
            catch (Exception ex)
            {
                biz_cls.write_log("系统日志", "采集系统", ex.ToString(), "");
                return null;
            }
        }

        private StringBuilder caiji_dingshi_modbus_tcp(string shijian, int txmk_id, string txmk_mingcheng, string tx_xieyi_connstr)
        {
            try
            {
                StringBuilder sb = new StringBuilder();


                #region 打开MODBUS TCP
                try
                {
                    ModbusMaster _mm = mtcp_cls.get_mtcp(tx_xieyi_connstr);
                    if (_mm == null)
                    {
                        //通讯模块连接异常报警
                        sb.Append("insert into baojing_detail values('" + Guid.NewGuid().ToString() + "',3,'" + txmk_mingcheng + "','','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "');");
                        //连接失败，停用通讯模块，将通讯模块连接状态改为0
                        sb.Append("update tx_mokuai set txmk_flag=0,txshijian='',qiyong_flag=0 where txmk_id=" + txmk_id + ";");
                        return sb;
                    }

                    //更新模块联通状态
                    sb.Append("update tx_mokuai set txmk_flag=1,txshijian='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where txmk_id=" + txmk_id + "; ");

                    #region 读取模块地址

                    //读取除了开关量之外的主参数
                    DataRow[] drs1 = dt_base.Select(" txmk_id=" + txmk_id + " and zhucanshu<>'' and zhucanshu<>'kaiguan' and dizhi<>''");
                    foreach (DataRow dr1 in drs1)
                    {
                        int field_id = Utility.ToInt(dr1["field_id"]);
                        string dizhi = Utility.ToObjectString(dr1["dizhi"]);
                        decimal value_down = Utility.ToDecimal(dr1["value_down"]);
                        decimal value_up = Utility.ToDecimal(dr1["value_up"]);
                        string shebei_mingcheng = Utility.ToObjectString(dr1["shebei_mingcheng"]);
                        string zhucanshu = Utility.ToObjectString(dr1["zhucanshu"]);
                        int shebei_id = Utility.ToInt(dr1["shebei_id"]);
                        string shebei_leixing_id = Utility.ToObjectString(dr1["shebei_leixing_id"]);
                        string str = "设备名称[" + shebei_mingcheng + "]主参数[" + zhucanshu + "]";

                        try
                        {
                            //读取相应地址数据
                            decimal zhi = Utility.ToDecimal(mtcp_cls.get_mtcp_value(_mm, dizhi));
                            sb.Append("insert into caiji_base values ('" + Guid.NewGuid().ToString() + "'," + field_id + "," + zhi + ",'" + shijian + "'," + shebei_id + ",'" + shebei_leixing_id + "','" + zhucanshu + "');");
                            if (value_down > 0)
                            {
                                if (zhi < value_down || zhi > value_up)
                                {
                                    //数值超限报警
                                    sb.Append("insert into baojing_detail values('" + Guid.NewGuid().ToString() + "',5,'" + str + "','','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "');");
                                }
                            }
                        }
                        catch
                        {
                            //地址读取错误报警
                            sb.Append("insert into baojing_detail values('" + Guid.NewGuid().ToString() + "',4,'" + str + "','','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "');");
                        }
                    }


                    #endregion

                    //关闭
                    _mm.Dispose();

                }
                catch
                {
                    //通讯模块连接异常报警
                    sb.Append("insert into baojing_detail values('" + Guid.NewGuid().ToString() + "',3,'" + txmk_mingcheng + "','','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "');");
                    //连接失败，停用通讯模块，将通讯模块连接状态改为0
                    sb.Append("update tx_mokuai set txmk_flag=0,txshijian='',qiyong_flag=0 where txmk_id=" + txmk_id + ";");
                    return sb;
                }
                #endregion

                return sb;

            }
            catch (Exception ex)
            {
                biz_cls.write_log("系统日志", "采集系统", ex.ToString(), "");
                return null;
            }
        }

        private StringBuilder caiji_dingshi_modbus_rtu(string shijian, int txmk_id, string txmk_mingcheng, string tx_xieyi_connstr)
        {
            try
            {
                StringBuilder sb = new StringBuilder();


                #region 打开MODBUS RTU
                try
                {
                    string[] s = tx_xieyi_connstr.Split('|');
                    string portName = s[0];  // 串口名称
                    int baudRate = int.Parse(s[1]);       // 波特率
                    int parity = int.Parse(s[2]);            // 校验位，0: 无校验，1: 偶校验，2: 奇校验
                    int dataBits = int.Parse(s[3]);          // 数据位
                    int stopBits = int.Parse(s[4]);          // 停止位

                    //1、打开串口连接
                    var serialPort = new SerialPort(portName, baudRate, (Parity)parity, dataBits, (StopBits)stopBits);
                    serialPort.Open();

                    // 2. 创建 Modbus RTU 主机对象
                    var _mm = ModbusSerialMaster.CreateRtu(serialPort);

                    if (_mm == null)
                    {
                        //通讯模块连接异常报警
                        sb.Append("insert into baojing_detail values('" + Guid.NewGuid().ToString() + "',3,'" + txmk_mingcheng + "','','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "');");
                        //连接失败，停用通讯模块，将通讯模块连接状态改为0
                        sb.Append("update tx_mokuai set txmk_flag=0,txshijian='',qiyong_flag=0 where txmk_id=" + txmk_id + ";");
                        return sb;
                    }

                    //更新模块联通状态
                    sb.Append("update tx_mokuai set txmk_flag=1,txshijian='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where txmk_id=" + txmk_id + "; ");

                    #region 读取模块地址

                    //读取除了开关量之外的主参数
                    DataRow[] drs1 = dt_base.Select(" txmk_id=" + txmk_id + " and zhucanshu<>'' and zhucanshu<>'kaiguan' and dizhi<>''");
                    foreach (DataRow dr1 in drs1)
                    {
                        int field_id = Utility.ToInt(dr1["field_id"]);
                        string dizhi = Utility.ToObjectString(dr1["dizhi"]);
                        decimal value_down = Utility.ToDecimal(dr1["value_down"]);
                        decimal value_up = Utility.ToDecimal(dr1["value_up"]);
                        string shebei_mingcheng = Utility.ToObjectString(dr1["shebei_mingcheng"]);
                        string zhucanshu = Utility.ToObjectString(dr1["zhucanshu"]);
                        int shebei_id = Utility.ToInt(dr1["shebei_id"]);
                        string shebei_leixing_id = Utility.ToObjectString(dr1["shebei_leixing_id"]);
                        string str = "设备名称[" + shebei_mingcheng + "]主参数[" + zhucanshu + "]";

                        try
                        {
                            //读取相应地址数据
                            decimal zhi = Utility.ToDecimal(mrtu_cls.get_mrtu_value(_mm, dizhi));
                            sb.Append("insert into caiji_base values ('" + Guid.NewGuid().ToString() + "'," + field_id + "," + zhi + ",'" + shijian + "'," + shebei_id + ",'" + shebei_leixing_id + "','" + zhucanshu + "');");
                            if (value_down > 0)
                            {
                                if (zhi < value_down || zhi > value_up)
                                {
                                    //数值超限报警
                                    sb.Append("insert into baojing_detail values('" + Guid.NewGuid().ToString() + "',5,'" + str + "','','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "');");
                                }
                            }
                        }
                        catch
                        {
                            //地址读取错误报警
                            sb.Append("insert into baojing_detail values('" + Guid.NewGuid().ToString() + "',4,'" + str + "','','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "');");
                        }
                    }


                    #endregion

                    //关闭
                    serialPort.Close();

                }
                catch
                {
                    //通讯模块连接异常报警
                    sb.Append("insert into baojing_detail values('" + Guid.NewGuid().ToString() + "',3,'" + txmk_mingcheng + "','','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "');");
                    //连接失败，停用通讯模块，将通讯模块连接状态改为0
                    sb.Append("update tx_mokuai set txmk_flag=0,txshijian='',qiyong_flag=0 where txmk_id=" + txmk_id + ";");
                    return sb;
                }
                #endregion

                return sb;

            }
            catch (Exception ex)
            {
                biz_cls.write_log("系统日志", "采集系统", ex.ToString(), "");
                return null;
            }
        }

        private void caiji_dingshi()
        {
            try
            {

                //通讯模块及参数是否变更的心跳检测
                this.tx_xintiao_jiance();


                string shijian = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                StringBuilder sb = new StringBuilder();

                foreach (DataRow dr in dt_mokuai.Rows)
                {
                    int txmk_id = Utility.ToInt(dr["txmk_id"]);
                    string txmk_mingcheng = Utility.ToObjectString(dr["txmk_mingcheng"]);
                    string tx_xieyi = Utility.ToObjectString(dr["tx_xieyi"]);
                    string tx_xieyi_connstr = Utility.ToObjectString(dr["tx_xieyi_connstr"]);

                    if (this.txmk_in_client(txmk_id) == true)
                    {
                        switch (tx_xieyi)
                        {
                            case "PLC S7":
                                sb.Append(this.caiji_dingshi_s7(shijian, txmk_id, txmk_mingcheng, tx_xieyi_connstr));
                                break;
                            case "MODBUS TCP":
                                sb.Append(this.caiji_dingshi_modbus_tcp(shijian, txmk_id, txmk_mingcheng, tx_xieyi_connstr));
                                break;
                            case "MODBUS RTU":
                                sb.Append(this.caiji_dingshi_modbus_rtu(shijian, txmk_id, txmk_mingcheng, tx_xieyi_connstr));
                                break;
                            default:
                                break;
                        }
                    }
                }

                if (sb.ToString().Length > 10)
                {
                    sb.Append("insert into caiji_zt values(0,1,0,'" + shijian + "',''," + biz_cls.client_id + ",'" + biz_cls.client_zhandian_ids + "');");
                    MySqlHelper.ExecuteSql(sb.ToString());
                }


                biz_cls.write_log("系统日志", "采集系统", "工作站：" + biz_cls.client_id.ToString() + "#站点：" + biz_cls.client_zhandian_ids + "#气电比数据，每分钟定时采集1次", "");
            }
            catch (Exception ex)
            {
                biz_cls.write_log("系统日志", "采集系统", ex.ToString(), "");
            }
        }

        #endregion

        #region 每分钟循环采集多次

        private StringBuilder caiji_xunhuan_s7(string shijian, int txmk_id, string txmk_mingcheng, string tx_xieyi_connstr)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                Plc _plc = s7_cls.get_plc(tx_xieyi_connstr);
                if (_plc != null)
                {

                    #region 打开PLC
                    try
                    {
                        _plc.Open();
                        //更新模块联通状态
                        sb.Append("update tx_mokuai set txmk_flag=1,txshijian='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where txmk_id=" + txmk_id + "; ");
                    }
                    catch
                    {
                        //通讯模块连接异常报警
                        sb.Append("insert into baojing_detail values('" + Guid.NewGuid().ToString() + "',3,'" + txmk_mingcheng + "','','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "');");
                        //连接失败，停用通讯模块，将通讯模块连接状态改为0
                        sb.Append("update tx_mokuai set txmk_flag=0,txshijian='',qiyong_flag=0 where txmk_id=" + txmk_id + ";");
                        return sb;
                    }
                    #endregion

                    #region 读取模块地址

                    //所有开关量 及辅参数
                    DataRow[] drs1 = dt_base.Select(" txmk_id=" + txmk_id + " and (zhucanshu='kaiguan' or fucanshu<>'')");

                    foreach (DataRow dr1 in drs1)
                    {
                        int field_id = Utility.ToInt(dr1["field_id"]);
                        string dizhi = Utility.ToObjectString(dr1["dizhi"]);
                        decimal value_down = Utility.ToDecimal(dr1["value_down"]);
                        decimal value_up = Utility.ToDecimal(dr1["value_up"]);
                        string shebei_mingcheng = Utility.ToObjectString(dr1["shebei_mingcheng"]);
                        string zhucanshu = Utility.ToObjectString(dr1["zhucanshu"]);
                        int shebei_id = Utility.ToInt(dr1["shebei_id"]);
                        string shebei_leixing_id = Utility.ToObjectString(dr1["shebei_leixing_id"]);
                        string str = "设备名称[" + shebei_mingcheng + "]主参数[" + zhucanshu + "]";

                        try
                        {
                            //读取相应地址数据
                            decimal zhi = Utility.ToDecimal(s7_cls.get_plc_value(_plc, dizhi));
                            sb.Append("insert into caiji_base values ('" + Guid.NewGuid().ToString() + "'," + field_id + "," + zhi + ",'" + shijian + "'," + shebei_id + ",'" + shebei_leixing_id + "','" + zhucanshu + "');");
                            if (value_down > 0)
                            {
                                if (zhi < value_down || zhi > value_up)
                                {
                                    //数值超限报警
                                    sb.Append("insert into baojing_detail values('" + Guid.NewGuid().ToString() + "',5,'" + str + "','','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "');");
                                }
                            }
                        }
                        catch
                        {
                            //地址读取错误报警
                            sb.Append("insert into baojing_detail values('" + Guid.NewGuid().ToString() + "',4,'" + str + "','','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "');");
                        }
                    }


                    #endregion

                    //关闭
                    _plc.Close();

                }
                else
                {
                    //通讯模块连接异常报警
                    sb.Append("insert into baojing_detail values('" + Guid.NewGuid().ToString() + "',3,'" + txmk_mingcheng + "','','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "');");
                    //连接失败，停用通讯模块，将通讯模块连接状态改为0
                    sb.Append("update tx_mokuai set txmk_flag=0,txshijian='',qiyong_flag=0 where txmk_id=" + txmk_id + ";");
                    return sb;
                }

                return sb;
            }
            catch (Exception ex)
            {
                biz_cls.write_log("系统日志", "采集系统", ex.ToString(), "");
                return null;
            }
        }

        private StringBuilder caiji_xunhuan_modbus_tcp(string shijian, int txmk_id, string txmk_mingcheng, string tx_xieyi_connstr)
        {
            try
            {
                StringBuilder sb = new StringBuilder();


                #region 打开MODBUS TCP
                try
                {
                    ModbusMaster _mm = mtcp_cls.get_mtcp(tx_xieyi_connstr);

                    if (_mm == null)
                    {
                        //通讯模块连接异常报警
                        sb.Append("insert into baojing_detail values('" + Guid.NewGuid().ToString() + "',3,'" + txmk_mingcheng + "','','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "');");
                        //连接失败，停用通讯模块，将通讯模块连接状态改为0
                        sb.Append("update tx_mokuai set txmk_flag=0,txshijian='',qiyong_flag=0 where txmk_id=" + txmk_id + ";");
                        return sb;
                    }

                    //更新模块联通状态
                    sb.Append("update tx_mokuai set txmk_flag=1,txshijian='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where txmk_id=" + txmk_id + "; ");

                    #region 读取模块地址

                    //读取除了开关量之外的主参数
                    DataRow[] drs1 = dt_base.Select(" txmk_id=" + txmk_id + " and zhucanshu<>'' and zhucanshu<>'kaiguan' and dizhi<>''");
                    foreach (DataRow dr1 in drs1)
                    {
                        int field_id = Utility.ToInt(dr1["field_id"]);
                        string dizhi = Utility.ToObjectString(dr1["dizhi"]);
                        decimal value_down = Utility.ToDecimal(dr1["value_down"]);
                        decimal value_up = Utility.ToDecimal(dr1["value_up"]);
                        string shebei_mingcheng = Utility.ToObjectString(dr1["shebei_mingcheng"]);
                        string zhucanshu = Utility.ToObjectString(dr1["zhucanshu"]);
                        int shebei_id = Utility.ToInt(dr1["shebei_id"]);
                        string shebei_leixing_id = Utility.ToObjectString(dr1["shebei_leixing_id"]);
                        string str = "设备名称[" + shebei_mingcheng + "]主参数[" + zhucanshu + "]";

                        try
                        {
                            //读取相应地址数据
                            decimal zhi = Utility.ToDecimal(mtcp_cls.get_mtcp_value(_mm, dizhi));
                            sb.Append("insert into caiji_base values ('" + Guid.NewGuid().ToString() + "'," + field_id + "," + zhi + ",'" + shijian + "'," + shebei_id + ",'" + shebei_leixing_id + "','" + zhucanshu + "');");
                            if (value_down > 0)
                            {
                                if (zhi < value_down || zhi > value_up)
                                {
                                    //数值超限报警
                                    sb.Append("insert into baojing_detail values('" + Guid.NewGuid().ToString() + "',5,'" + str + "','','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "');");
                                }
                            }
                        }
                        catch
                        {
                            //地址读取错误报警
                            sb.Append("insert into baojing_detail values('" + Guid.NewGuid().ToString() + "',4,'" + str + "','','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "');");
                        }
                    }


                    #endregion

                    //关闭
                    _mm.Dispose();
                }
                catch
                {
                    //通讯模块连接异常报警
                    sb.Append("insert into baojing_detail values('" + Guid.NewGuid().ToString() + "',3,'" + txmk_mingcheng + "','','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "');");
                    //连接失败，停用通讯模块，将通讯模块连接状态改为0
                    sb.Append("update tx_mokuai set txmk_flag=0,txshijian='',qiyong_flag=0 where txmk_id=" + txmk_id + ";");
                    return sb;
                }
                #endregion

                return sb;

            }
            catch (Exception ex)
            {
                biz_cls.write_log("系统日志", "采集系统", ex.ToString(), "");
                return null;
            }
        }

        private StringBuilder caiji_xunhuan_modbus_rtu(string shijian, int txmk_id, string txmk_mingcheng, string tx_xieyi_connstr)
        {
            try
            {
                StringBuilder sb = new StringBuilder();


                #region 打开MODBUS RTU
                try
                {
                    string[] s = tx_xieyi_connstr.Split('|');
                    string portName = s[0];  // 串口名称
                    int baudRate = int.Parse(s[1]);       // 波特率
                    int parity = int.Parse(s[2]);            // 校验位，0: 无校验，1: 偶校验，2: 奇校验
                    int dataBits = int.Parse(s[3]);          // 数据位
                    int stopBits = int.Parse(s[4]);          // 停止位

                    //1、打开串口连接
                    var serialPort = new SerialPort(portName, baudRate, (Parity)parity, dataBits, (StopBits)stopBits);
                    serialPort.Open();

                    // 2. 创建 Modbus RTU 主机对象
                    var _mm = ModbusSerialMaster.CreateRtu(serialPort);

                    if (_mm == null)
                    {
                        //通讯模块连接异常报警
                        sb.Append("insert into baojing_detail values('" + Guid.NewGuid().ToString() + "',3,'" + txmk_mingcheng + "','','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "');");
                        //连接失败，停用通讯模块，将通讯模块连接状态改为0
                        sb.Append("update tx_mokuai set txmk_flag=0,txshijian='',qiyong_flag=0 where txmk_id=" + txmk_id + ";");
                        return sb;
                    }

                    //更新模块联通状态
                    sb.Append("update tx_mokuai set txmk_flag=1,txshijian='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where txmk_id=" + txmk_id + "; ");

                    #region 读取模块地址

                    //读取除了开关量之外的主参数
                    DataRow[] drs1 = dt_base.Select(" txmk_id=" + txmk_id + " and zhucanshu<>'' and zhucanshu<>'kaiguan' and dizhi<>''");
                    foreach (DataRow dr1 in drs1)
                    {
                        int field_id = Utility.ToInt(dr1["field_id"]);
                        string dizhi = Utility.ToObjectString(dr1["dizhi"]);
                        decimal value_down = Utility.ToDecimal(dr1["value_down"]);
                        decimal value_up = Utility.ToDecimal(dr1["value_up"]);
                        string shebei_mingcheng = Utility.ToObjectString(dr1["shebei_mingcheng"]);
                        string zhucanshu = Utility.ToObjectString(dr1["zhucanshu"]);
                        int shebei_id = Utility.ToInt(dr1["shebei_id"]);
                        string shebei_leixing_id = Utility.ToObjectString(dr1["shebei_leixing_id"]);
                        string str = "设备名称[" + shebei_mingcheng + "]主参数[" + zhucanshu + "]";

                        try
                        {
                            //读取相应地址数据
                            decimal zhi = Utility.ToDecimal(mrtu_cls.get_mrtu_value(_mm, dizhi));
                            sb.Append("insert into caiji_base values ('" + Guid.NewGuid().ToString() + "'," + field_id + "," + zhi + ",'" + shijian + "'," + shebei_id + ",'" + shebei_leixing_id + "','" + zhucanshu + "');");
                            if (value_down > 0)
                            {
                                if (zhi < value_down || zhi > value_up)
                                {
                                    //数值超限报警
                                    sb.Append("insert into baojing_detail values('" + Guid.NewGuid().ToString() + "',5,'" + str + "','','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "');");
                                }
                            }
                        }
                        catch
                        {
                            //地址读取错误报警
                            sb.Append("insert into baojing_detail values('" + Guid.NewGuid().ToString() + "',4,'" + str + "','','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "');");
                        }
                    }


                    #endregion

                    //关闭
                    serialPort.Close();

                }
                catch
                {
                    //通讯模块连接异常报警
                    sb.Append("insert into baojing_detail values('" + Guid.NewGuid().ToString() + "',3,'" + txmk_mingcheng + "','','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "');");
                    //连接失败，停用通讯模块，将通讯模块连接状态改为0
                    sb.Append("update tx_mokuai set txmk_flag=0,txshijian='',qiyong_flag=0 where txmk_id=" + txmk_id + ";");
                    return sb;
                }
                #endregion

                return sb;

            }
            catch (Exception ex)
            {
                biz_cls.write_log("系统日志", "采集系统", ex.ToString(), "");
                return null;
            }
        }
        private string caiji_xunhuan()
        {
            try
            {
                //通讯模块及参数是否变更的心跳检测
                this.tx_xintiao_jiance();

                string shijian = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                StringBuilder sb = new StringBuilder();

                foreach (DataRow dr in dt_mokuai.Rows)
                {
                    int txmk_id = Utility.ToInt(dr["txmk_id"]);
                    string txmk_mingcheng = Utility.ToObjectString(dr["txmk_mingcheng"]);
                    string tx_xieyi = Utility.ToObjectString(dr["tx_xieyi"]);
                    string tx_xieyi_connstr = Utility.ToObjectString(dr["tx_xieyi_connstr"]);

                    if (this.txmk_in_client(txmk_id) == true)
                    {
                        switch (tx_xieyi)
                        {
                            case "PLC S7":
                                sb.Append(this.caiji_xunhuan_s7(shijian, txmk_id, txmk_mingcheng, tx_xieyi_connstr));
                                break;
                            case "MODBUS TCP":
                                sb.Append(this.caiji_xunhuan_modbus_tcp(shijian, txmk_id, txmk_mingcheng, tx_xieyi_connstr));
                                break;
                            case "MODBUS RTU":
                                sb.Append(this.caiji_xunhuan_modbus_rtu(shijian, txmk_id, txmk_mingcheng, tx_xieyi_connstr));
                                break;
                            default:
                                break;
                        }
                    }
                }

                if (sb.ToString().Length > 10)
                {
                    MySqlHelper.ExecuteSql(sb.ToString());
                }

                biz_cls.write_log("系统日志", "采集系统", "工作站：" + biz_cls.client_id.ToString() + "#站点：" + biz_cls.client_zhandian_ids + "#开关量、辅参数，每分钟循环采集多次", "");

                return shijian;
            }
            catch (Exception ex)
            {
                biz_cls.write_log("系统日志", "采集系统", ex.ToString(), "");
                return "";
            }
        }


        #endregion

        #region 通讯模块及相关参数变更的心跳检测

        public void tx_xintiao_jiance()
        {
            #region 通讯模块是否变更的心跳检测
            int txmk_xintiao = Utility.ToInt(MySqlHelper.Get_sigle("select txmk_xintiao from base_xiangmu limit 1"));
            if (txmk_xintiao > 0)
            {
                //获取站点模块列表
                this.get_mokuai();

                //恢复通讯模块心跳为0
                MySqlHelper.ExecuteSql("update base_xiangmu set txmk_xintiao=0");
            }
            #endregion

            #region 通讯模块参数是否变更的心跳检测
            int txmk_canshu_xintiao = Utility.ToInt(MySqlHelper.Get_sigle("select txmk_canshu_xintiao from base_xiangmu limit 1"));
            if (txmk_canshu_xintiao > 0)
            {
                //获取地址参数对应字段table，uv_tx_mokuai_field相关数据
                this.get_mokuai_field();

                //恢复通讯模块心跳为0
                MySqlHelper.ExecuteSql("update base_xiangmu set txmk_canshu_xintiao=0");
            }
            #endregion
        }

        #endregion

        #region 判断是否是主机

        private bool if_zhuji()
        {
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up)
                {
                    string macAddress = nic.GetPhysicalAddress().ToString();
                    if (macAddress != "" && biz_cls.client_mac != "")
                    {
                        foreach (string s in biz_cls.client_mac.Split('|'))
                        {
                            if (s == macAddress)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        private void get_mac()
        {
            DataSet ds = new DataSet();
            ds = MySqlHelper.Get_DataSet("select * from client");

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up)
                {
                    string macAddress = nic.GetPhysicalAddress().ToString();
                    if (macAddress != "")
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            if (macAddress == Utility.ToObjectString(dr["client_mac"]))
                            {
                                biz_cls.client_mac = macAddress;
                                biz_cls.client_name = Utility.ToObjectString(dr["client_mingcheng"]);
                                biz_cls.client_txmk_ids = Utility.ToObjectString(dr["txmk_ids"]);
                                biz_cls.client_id = Utility.ToInt(dr["id"]);
                                biz_cls.client_zhandian_ids = Utility.ToObjectString(dr["zhandian_ids"]);
                                return;
                            }
                        }

                    }
                }
            }
        }

        #endregion

        #region 通讯模块连接检测，把连不上的模块关闭启用

        private bool txmk_jiance()
        {
            try
            {
                this.get_mokuai();

                StringBuilder sb = new StringBuilder();
                foreach (DataRow dr in dt_mokuai.Rows)
                {
                    int txmk_id = Utility.ToInt(dr["txmk_id"]);
                    string txmk_mingcheng = Utility.ToObjectString(dr["txmk_mingcheng"]);
                    string tx_xieyi = Utility.ToObjectString(dr["tx_xieyi"]);
                    string tx_xieyi_connstr = Utility.ToObjectString(dr["tx_xieyi_connstr"]);
                    if (this.txmk_in_client(txmk_id) == true)
                    {
                        switch (tx_xieyi)
                        {
                            case "PLC S7":
                                try
                                {


                                    Plc _plc = s7_cls.get_plc(tx_xieyi_connstr);
                                    if (_plc != null)
                                    {
                                        _plc.Open();
                                        _plc.Close();
                                    }
                                    else
                                    {
                                        //连接失败，停用通讯模块，将通讯模块连接状态改为0
                                        sb.Append("update tx_mokuai set txmk_flag=0,txshijian='',qiyong_flag=0 where txmk_id=" + txmk_id + ";");
                                    }

                                }
                                catch
                                {
                                    //连接失败，停用通讯模块，将通讯模块连接状态改为0
                                    sb.Append("update tx_mokuai set txmk_flag=0,txshijian='',qiyong_flag=0 where txmk_id=" + txmk_id + ";");
                                }
                                break;
                            case "MODBUS TCP":
                                try
                                {
                                    ModbusMaster _mm = mtcp_cls.get_mtcp(tx_xieyi_connstr);
                                    if (_mm == null)
                                    {
                                        //连接失败，停用通讯模块，将通讯模块连接状态改为0
                                        sb.Append("update tx_mokuai set txmk_flag=0,txshijian='',qiyong_flag=0 where txmk_id=" + txmk_id + ";");
                                    }
                                }
                                catch
                                {
                                    //连接失败，停用通讯模块，将通讯模块连接状态改为0
                                    sb.Append("update tx_mokuai set txmk_flag=0,txshijian='',qiyong_flag=0 where txmk_id=" + txmk_id + ";");
                                }
                                break;
                            case "MODBUS RTU":
                                try
                                {
                                    string[] s3 = tx_xieyi_connstr.Split('|');
                                    string portName = s3[0];  // 串口名称
                                    int baudRate = int.Parse(s3[1]);       // 波特率
                                    int parity = int.Parse(s3[2]);            // 校验位，0: 无校验，1: 偶校验，2: 奇校验
                                    int dataBits = int.Parse(s3[3]);          // 数据位
                                    int stopBits = int.Parse(s3[4]);          // 停止位

                                    //1、打开串口连接
                                    var serialPort = new SerialPort(portName, baudRate, (Parity)parity, dataBits, (StopBits)stopBits);
                                    serialPort.Open();

                                    // 2. 创建 Modbus RTU 主机对象
                                    var modbusRtuMaster = ModbusSerialMaster.CreateRtu(serialPort);
                                    if (modbusRtuMaster == null)
                                    {
                                        //连接失败，停用通讯模块，将通讯模块连接状态改为0
                                        sb.Append("update tx_mokuai set txmk_flag=0,txshijian='',qiyong_flag=0 where txmk_id=" + txmk_id + ";");
                                    }
                                    serialPort.Close();
                                }
                                catch
                                {
                                    //连接失败，停用通讯模块，将通讯模块连接状态改为0
                                    sb.Append("update tx_mokuai set txmk_flag=0,txshijian='',qiyong_flag=0 where txmk_id=" + txmk_id + ";");
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
                if (sb.ToString().Length > 10)
                {
                    MySqlHelper.ExecuteSql(sb.ToString());


                    //更新本地最新站点模块列表
                    this.get_mokuai();

                    return false;
                }


                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        private void frm_main_new_Load(object sender, EventArgs e)
        {

            StatusImgs = new Image[] { kyj_project.Properties.Resources._red, kyj_project.Properties.Resources._green, kyj_project.Properties.Resources._gray, kyj_project.Properties.Resources.lianjie1, kyj_project.Properties.Resources.lianjie2, kyj_project.Properties.Resources.computer1, kyj_project.Properties.Resources.computer2, kyj_project.Properties.Resources.server1, kyj_project.Properties.Resources.server2, kyj_project.Properties.Resources.alarm1, kyj_project.Properties.Resources.alarm2 };


            if (biz_cls.client_mac != "" && biz_cls.client_txmk_ids != "")
            {
                this.pb_lianjie.Visible = true;
            }
            else
            {
                this.pb_lianjie.Visible = false;
            }


            this.Format_grid();

            this.Load_data();

            //桌面定时监控
            this.jiankong();

            this.timer1.Start();


        }

        private void btn_lianjie_Click(object sender, EventArgs e)
        {

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

        private void pb_lianjie_Click(object sender, EventArgs e)
        {
            //通讯模块开启检测
            if (timer_flag == 1)
            {
                //通讯开启状态
                timer_flag = 0;
                this.pb_lianjie.BackgroundImage = kyj_project.Properties.Resources._8;
                this.pb_lianjie.Tag = "停止通讯";
                this.lb_zt.Text = "通讯模块通讯已暂停...";
            }
            else
            {
                if (this.txmk_jiance() == true)
                {
                    if (dt_mokuai.Rows.Count > 0)
                    {
                        timer_flag = 1;
                        this.pb_lianjie.BackgroundImage = kyj_project.Properties.Resources._9;
                        this.pb_lianjie.Tag = "开启通讯";
                        this.lb_zt.Text = "通讯模块通讯已连接...";
                    }
                    else
                    {
                        MessageBox.Show("没有启用的通讯模块需要连接！");
                    }
                }
                else
                {
                    MessageBox.Show("通讯模块连接异常！");
                }
            }
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

        private void get_mokuai()
        {
            //获取站点模块列表
            DataSet ds1 = new DataSet();
            if (biz_cls.client_mac != null && biz_cls.client_mac != "" && biz_cls.client_txmk_ids != null && biz_cls.client_txmk_ids != "")
            {
                ds1 = MySqlHelper.Get_DataSet("select * from tx_mokuai where qiyong_flag=1 and txmk_id in (" + biz_cls.client_txmk_ids + ") order by txmk_id");
            }
            else
            {
                ds1 = MySqlHelper.Get_DataSet("select * from tx_mokuai where qiyong_flag=1  order by txmk_id");
            }
            dt_mokuai = new DataTable();
            dt_mokuai = ds1.Tables[0];
        }

        private void get_mokuai_field()
        {
            DataSet ds = new DataSet();
            ds = MySqlHelper.Get_DataSet("select field_id,shebei_id,txmk_id,zhandian_id,shebei_leixing_id,zhucanshu,fucanshu,dizhi,shebei_mingcheng,value_up,value_down from uv_tx_mokuai_field");
            dt_base = new DataTable();
            dt_base = ds.Tables[0];
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
