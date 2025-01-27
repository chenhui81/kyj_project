using kyj_project.Common;
using kyj_project.DAL;
using Microsoft.Win32;
using Modbus.Device;
using S7.Net;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kyj_client_srv
{
    public partial class Form1 : Form
    {
        public DataTable dt_base;
        public DataTable dt_mokuai;
        private int timer_flag = 0; //控制timer定时器，1启动0停止
        private System.Threading.Timer _timer;
        private Image[] StatusImgs; //指示灯状态
        public Form1()
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



            //获取站点模块列表
            this.get_mokuai();

            //获取地址参数对应字段table，uv_tx_mokuai_field相关数据
            this.get_mokuai_field();

            #endregion

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
                        ////每分钟定时,每个工作站间隔3秒
                        if (DateTime.Now.Second == 5 * (biz_cls.client_id - 1))
                        {
                            this.caiji_dingshi();
                        }

                        ////循环
                        if (DateTime.Now.Second == 30 + 5 * (biz_cls.client_id - 1))
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
                        if (DateTime.Now.Second % 10 == 3)
                        {
                            StringBuilder sb = new StringBuilder();
                            DateTime dt_server = Utility.ToDateTime(MySqlHelper.Get_sigle("select server_xintiao from base_xiangmu limit 1"));
                            if (dt_server <= DateTime.Now.AddSeconds(-30))
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
                    this.write_file_log("DoAsyncWork#" + ex.ToString());
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
                            string zhi_str = s7_cls.get_plc_value(_plc, dizhi);
                            decimal zhi = Utility.ToDecimal(zhi_str);
                            if (zhi_str.ToLower().Contains("e")) { zhi = Utility.ToDecimal(Utility.ToDouble(zhi_str)); }

                            sb.Append("insert into caiji_base values ('" + Guid.NewGuid().ToString() + "'," + field_id + "," + zhi + ",'" + shijian + "'," + shebei_id + ",'" + shebei_leixing_id + "','" + zhucanshu + "');");

                            if (value_down > 0)
                            {
                                if (zhi < value_down || zhi > value_up)
                                {
                                    //数值超限报警
                                    str += "参数值[" + zhi + "]";
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
                this.write_file_log(ex.ToString());
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
                            string zhi_str = mtcp_cls.get_mtcp_value(_mm, dizhi);
                            decimal zhi = Utility.ToDecimal(zhi_str);
                            if (zhi_str.ToLower().Contains("e")) { zhi = Utility.ToDecimal(Utility.ToDouble(zhi_str)); }

                            sb.Append("insert into caiji_base values ('" + Guid.NewGuid().ToString() + "'," + field_id + "," + zhi + ",'" + shijian + "'," + shebei_id + ",'" + shebei_leixing_id + "','" + zhucanshu + "');");
                            if (value_down > 0)
                            {
                                if (zhi < value_down || zhi > value_up)
                                {
                                    //数值超限报警
                                    str += "参数值[" + zhi + "]";
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
                this.write_file_log(ex.ToString());
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
                            string zhi_str = mrtu_cls.get_mrtu_value(_mm, dizhi);
                            decimal zhi = Utility.ToDecimal(zhi_str);
                            if (zhi_str.ToLower().Contains("e")) { zhi = Utility.ToDecimal(Utility.ToDouble(zhi_str)); }

                            sb.Append("insert into caiji_base values ('" + Guid.NewGuid().ToString() + "'," + field_id + "," + zhi + ",'" + shijian + "'," + shebei_id + ",'" + shebei_leixing_id + "','" + zhucanshu + "');");
                            if (value_down > 0)
                            {
                                if (zhi < value_down || zhi > value_up)
                                {
                                    //数值超限报警
                                    str += "参数值[" + zhi + "]";
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
                this.write_file_log(ex.ToString());
                return null;
            }
        }

        private void caiji_dingshi()
        {
            try
            {
                biz_cls.write_log("系统日志", "采集系统", "工作站：" + biz_cls.client_id.ToString() + "#站点：" + biz_cls.client_zhandian_ids + "#气电比数据——开始", "");

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
                    //在状态改变之前写日志记录
                    sb.Append(biz_cls.write_log_str("系统日志", "采集系统", "工作站：" + biz_cls.client_id.ToString() + "#站点：" + biz_cls.client_zhandian_ids + "#气电比数据——结束", ""));

                    sb.Append("insert into caiji_zt values(0,1,0,'" + shijian + "',''," + biz_cls.client_id + ",'" + biz_cls.client_zhandian_ids + "');");
                    MySqlHelper.ExecuteSql(sb.ToString());
                }



            }
            catch (Exception ex)
            {
                this.write_file_log(ex.ToString());
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
                            string zhi_str = s7_cls.get_plc_value(_plc, dizhi);
                            decimal zhi = Utility.ToDecimal(zhi_str);
                            if (zhi_str.ToLower().Contains("e")) { zhi = Utility.ToDecimal(Utility.ToDouble(zhi_str)); }

                            sb.Append("insert into caiji_base values ('" + Guid.NewGuid().ToString() + "'," + field_id + "," + zhi + ",'" + shijian + "'," + shebei_id + ",'" + shebei_leixing_id + "','" + zhucanshu + "');");
                            if (value_down > 0)
                            {
                                if (zhi < value_down || zhi > value_up)
                                {
                                    //数值超限报警
                                    str += "参数值[" + zhi + "]";
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
                this.write_file_log(ex.ToString());
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
                            string zhi_str = mtcp_cls.get_mtcp_value(_mm, dizhi);
                            decimal zhi = Utility.ToDecimal(zhi_str);
                            if (zhi_str.ToLower().Contains("e")) { zhi = Utility.ToDecimal(Utility.ToDouble(zhi_str)); }

                            sb.Append("insert into caiji_base values ('" + Guid.NewGuid().ToString() + "'," + field_id + "," + zhi + ",'" + shijian + "'," + shebei_id + ",'" + shebei_leixing_id + "','" + zhucanshu + "');");
                            if (value_down > 0)
                            {
                                if (zhi < value_down || zhi > value_up)
                                {
                                    //数值超限报警
                                    str += "参数值[" + zhi + "]";
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
                this.write_file_log(ex.ToString());
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
                            string zhi_str = mrtu_cls.get_mrtu_value(_mm, dizhi);
                            decimal zhi = Utility.ToDecimal(zhi_str);
                            if (zhi_str.ToLower().Contains("e")) { zhi = Utility.ToDecimal(Utility.ToDouble(zhi_str)); }

                            sb.Append("insert into caiji_base values ('" + Guid.NewGuid().ToString() + "'," + field_id + "," + zhi + ",'" + shijian + "'," + shebei_id + ",'" + shebei_leixing_id + "','" + zhucanshu + "');");
                            if (value_down > 0)
                            {
                                if (zhi < value_down || zhi > value_up)
                                {
                                    //数值超限报警
                                    str += "参数值[" + zhi + "]";
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
                this.write_file_log(ex.ToString());
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
                this.write_file_log(ex.ToString());
                return "";
            }
        }


        #endregion

        #region 通讯模块及相关参数变更的心跳检测

        public void tx_xintiao_jiance()
        {
            try
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
            catch (Exception ex)
            {
                this.write_file_log(ex.ToString());
            }
        }

        #endregion

        #region 判断是否是主机

        private bool if_zhuji()
        {
            try
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
            catch
            {
                return false;
            }
        }

        private void get_mac()
        {
            try
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
            catch (Exception ex)
            {
                this.write_file_log(ex.ToString());
            }
        }

        #endregion

        #region 通讯模块连接检测，把连不上的模块关闭启用

        private bool txmk_jiance()
        {
            try
            {
                this.get_mokuai();
                if (dt_mokuai == null) { return true; }

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

        private void get_mokuai()
        {
            try
            {
                //获取站点模块列表
                DataSet ds1 = new DataSet();
                if (biz_cls.client_mac != "" && biz_cls.client_txmk_ids != "")
                {
                    ds1 = MySqlHelper.Get_DataSet("select * from tx_mokuai where qiyong_flag=1 and txmk_id in (" + biz_cls.client_txmk_ids + ") order by txmk_id");
                    dt_mokuai = new DataTable();
                    dt_mokuai = ds1.Tables[0];
                }
                else
                {
                    dt_mokuai = null;
                }
            }
            catch (Exception ex)
            {
                this.write_file_log(ex.ToString());
            }

        }

        private void get_mokuai_field()
        {
            try
            {
                DataSet ds = new DataSet();
                ds = MySqlHelper.Get_DataSet("select field_id,shebei_id,txmk_id,zhandian_id,shebei_leixing_id,zhucanshu,fucanshu,dizhi,shebei_mingcheng,value_up,value_down from uv_tx_mokuai_field");
                dt_base = new DataTable();
                dt_base = ds.Tables[0];
            }
            catch (Exception ex)
            {
                this.write_file_log(ex.ToString());
            }
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

        #region 写本地日志

        private void write_file_log(string str)
        {
            // string filePath = @"C:\kyjsys_log.txt";
            string filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\kyjsys_log.txt";
            try
            {
                // 创建一个StreamWriter对象，它会自动创建文件（如果不存在）以及对应的路径
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine(DateTime.Now.ToString() + "\r\n" + str + "\r\n");

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"写入文件出现错误: {ex.Message}");
            }
        }

        #endregion

        #region 加入自启动

        private void auto_qidong()
        {
            try
            {
                //  string appPath = @"C:\Path\To\Your\Application.exe";
                string appPath = Assembly.GetExecutingAssembly().Location;
                string appName = "kyj_client_srv";
                // 打开所有用户的 Run 键
                RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                if (key == null)
                {
                    key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
                    key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                }
                // 设置自启动项
                key.SetValue(appName, appPath);

                key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                this.write_file_log("应用程序已设置为所有用户自启动。" + key.GetValue(appName));

                key.Close();
            }
            catch (Exception ex)
            {
                this.write_file_log($"设置自启动项时出错: {ex.Message}");
            }
        }

        #endregion

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {



                StatusImgs = new Image[] { kyj_client_srv.Properties.Resources._5, kyj_client_srv.Properties.Resources._6, kyj_client_srv.Properties.Resources._8, kyj_client_srv.Properties.Resources._9 };

                this.pb_flag.BackgroundImage = StatusImgs[0];
                this.label1.Text = "点击开启服务";

                this.Text = biz_cls.xiangmu_mingcheng;
                string lb_zhandian = "";
                if (biz_cls.client_zhandian_ids != "")
                {
                    string[] s = biz_cls.client_zhandian_ids.Split(',');
                    foreach (string s1 in s)
                    {
                        if (s1 != "")
                        {
                            lb_zhandian += Utility.ToObjectString(MySqlHelper.Get_sigle("select zhandian_mingcheng from base_zhandian where zhandian_id='" + s1 + "'")) + "，";
                        }
                    }
                }
                if (lb_zhandian == "")
                {
                    this.lb_zhandian.Text = "相关站点：无";
                }
                else
                {
                    this.lb_zhandian.Text = "相关站点：" + lb_zhandian;
                }

                //首次开启服务
                this.pb_flag_Click(null, null);

                //加入自启动
                this.auto_qidong();

                //显示运行时间
                this.lb_start.Text = "启动时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                // 最小化窗口
                this.WindowState = FormWindowState.Minimized;
                this.Hide();
                // 显示托盘图标
                notifyIcon1.Visible = true;
            }
            catch (Exception ex)
            {
                this.write_file_log(ex.ToString());
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                // 当窗口最小化时，隐藏窗口
                this.Hide();
                // 显示托盘图标
                notifyIcon1.Visible = true;
            }
        }


        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                // 当窗口是最小化状态时，重新显示窗口并还原窗口状态
                this.Show();
                this.WindowState = FormWindowState.Normal;
                notifyIcon1.Visible = false;
            }
        }

        private void pb_flag_Click(object sender, EventArgs e)
        {
            //通讯模块开启检测
            if (timer_flag == 1)
            {
                //通讯开启状态
                timer_flag = 0;
                this.pb_flag.BackgroundImage = kyj_client_srv.Properties.Resources._5;
                this.label1.Text = "点击开启服务";
                this.pb_flag.Tag = "停止通讯";
                this.lb_zt.Text = "通讯模块通讯已暂停...";
                this.write_file_log("服务已停止" + "#####################################\r\n");
            }
            else
            {
                if (this.txmk_jiance() == true)
                {
                    if (dt_mokuai != null && dt_mokuai.Rows.Count > 0)
                    {
                        timer_flag = 1;
                        this.pb_flag.BackgroundImage = kyj_client_srv.Properties.Resources._6;

                        this.label1.Text = "点击关闭服务";
                        this.pb_flag.Tag = "开启通讯";
                        this.lb_zt.Text = "通讯模块通讯已连接...";
                    }
                    else
                    {
                        this.lb_zt.Text = "没有启用的通讯模块需要连接！";
                    }
                }
                else
                {
                    this.lb_zt.Text = "通讯模块连接异常！";
                    this.write_file_log("通讯模块连接异常!");
                }

                this.write_file_log("服务已启动" + "#####################################\r\n");
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }
    }
}
