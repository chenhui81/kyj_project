using kyj_project.Common;
using kyj_project.DAL;
using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace kyj_server
{
    public partial class Form1 : Form
    {
        int int_flag = 0;//初始化是否完成 1完成0未完成
        private System.Threading.Timer _timer;
        int qidianbi_chuli_flag = 0; //气电比处理开关
        int kaiguan_fucanshu_chuli_flag = 0; //开关量辅参数处理开关
        DateTime shijian_start;

        public Form1()
        {
            InitializeComponent();

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
                // 长时间运行的任务

                #region 长时间运行的任务

                if (int_flag == 1)
                {
                    //气电比、主参数统计处理，每分钟一次，处理完更新状态
                    if (qidianbi_chuli_flag == 0 && DateTime.Now.Second < 30)
                    {
                        this.qidianbi_chuli();
                    }

                    //处理开关量、辅助参数统计数据
                    if (kaiguan_fucanshu_chuli_flag == 0 && DateTime.Now.Second >= 30)
                    {
                        this.kaiguan_fucanshu_chuli();
                    }

                    //心跳检测
                    if (DateTime.Now.Second % 10 == 3)
                    {
                        StringBuilder sb = new StringBuilder();

                        DataSet ds = new DataSet();
                        ds = MySqlHelper.Get_DataSet("select * from client");
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                DateTime dt_client = Utility.ToDateTime(dr["xintiao"]);
                                string client_mingcheng = Utility.ToObjectString(dr["client_mingcheng"]) + "断开";
                                if (dt_client <= DateTime.Now.AddSeconds(-30))
                                {
                                    //客户端断线
                                    sb.Append("insert into baojing_detail values('" + Guid.NewGuid().ToString() + "',1,'" + client_mingcheng + "','','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "');");
                                }
                            }
                        }

                        sb.Append("update base_xiangmu set server_xintiao='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' ");
                        MySqlHelper.ExecuteSql(sb.ToString());
                    }

                    //报警处理
                    if (DateTime.Now.Second == 56)
                    {
                        this.baojing_chuli();
                    }

                    //采集数据的清理
                    if (DateTime.Now.Second == 58)
                    {
                        this.data_10m_clear();
                    }

                    //凌晨清理数据
                    if (DateTime.Now.ToString("HH:mm:ss") == "00:00:52")
                    {
                        this.data_day_clear();
                    }

                    //凌晨结算上一天节能报告
                    if (DateTime.Now.ToString("HH:mm:ss") == "00:00:54")
                    {
                        try
                        {
                            biz_cls.rpt_ri_g(DateTime.Now.AddDays(-1));
                            biz_cls.write_log("系统日志", "后台服务", "结算上一天节能报告", "");
                        }
                        catch (Exception ex)
                        {
                            biz_cls.write_log("系统日志", "后台服务", ex.Message, "");
                        }
                    }


                }

                #endregion

            });
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.kaiguan_int(); //设备开关量初始化

            this.Text = "空压机节能系统服务端";
            this.lb_title.Text = biz_cls.xiangmu_mingcheng;
            this.notifyIcon1.Text = biz_cls.xiangmu_mingcheng;
            shijian_start = DateTime.Now;
            this.lb_zt.Text = "服务启动时间：" + shijian_start.ToString("yyyy-MM-dd HH:mm:ss");

            int_flag = 1;

            this.timer1.Start();
        }


        #region  设备开关量初始化

        private void kaiguan_int()
        {
            DataSet ds = new DataSet();
            ds = MySqlHelper.Get_DataSet("select shebei_id,qiting_flag from base_shebei where (shebei_leixing_id='lixinji' or shebei_leixing_id='luoganji' or shebei_leixing_id='lengqueji'or shebei_leixing_id='lengganji' or shebei_leixing_id='xiganji')");
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        int qiting_flag = Utility.ToInt(dr["qiting_flag"]);
                        int shebei_id = Utility.ToInt(dr["shebei_id"]);

                        if (qiting_flag == 1)
                        {
                            //判断caiji_shebei_qiting 有没有设备开机记录
                            int i = Utility.ToInt(MySqlHelper.Get_sigle("select count(*) from caiji_shebei_qiting where shebei_id=" + shebei_id + " and shijian_e=''"));
                            if (i <= 0)
                            {
                                MySqlHelper.ExecuteSql("insert into caiji_shebei_qiting values(0," + shebei_id + ",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','')");
                            }
                        }
                    }
                }
            }
        }

        #endregion


        #region 处理气电比统计数据

        private void qidianbi_chuli()
        {
            try
            {


                qidianbi_chuli_flag = 1;
                string shijian = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                string shijian_last = DateTime.Now.AddMinutes(-1).ToString("yyyy-MM-dd HH:mm");
                StringBuilder sb = new StringBuilder();

                DataSet ds_zt = new DataSet();
                ds_zt = MySqlHelper.Get_DataSet("select id,zhandian_ids,client_id from caiji_zt where shijian = '" + shijian + "' and caiji_flag = 1 and chuli_flag = 0 order by shijian limit 1");
                if (ds_zt.Tables[0].Rows.Count > 0)
                {
                    DataRow dr_zt = ds_zt.Tables[0].Rows[0];
                    int id_zt = Utility.ToInt(dr_zt["id"]);
                    int client_id = Utility.ToInt(dr_zt["client_id"]);
                    string zhandian_ids = Utility.ToObjectString(dr_zt["zhandian_ids"]);
                    string[] ls_zhandian = zhandian_ids.Split(',');

                    biz_cls.write_log("系统日志", "后台服务", "工作站：" + client_id.ToString() + "#处理气电比统计数据——开始", "");

                    #region  循环每个站点，分别统计1m气电比

                    foreach (string zhandian_id in ls_zhandian)
                    {
                        int dianliang_e = Convert.ToInt32(MySqlHelper.Get_sigle("select sum(shuju) from uv_caiji_base where shijian='" + shijian + "' and zhandian_id='" + zhandian_id + "' and zhucanshu='dianliang' and if_nenghaojisuan=1 and qiyong_flag=1"));
                        int qiliang_e = Convert.ToInt32(MySqlHelper.Get_sigle("select sum(shuju) from uv_caiji_base where shijian='" + shijian + "' and zhandian_id='" + zhandian_id + "' and zhucanshu='qiliang' and if_nenghaojisuan=1 and qiyong_flag=1"));

                        int dianliang_s = Convert.ToInt32(MySqlHelper.Get_sigle("select sum(shuju) from uv_caiji_base where shijian='" + shijian_last + "' and zhandian_id='" + zhandian_id + "' and zhucanshu='dianliang' and if_nenghaojisuan=1 and qiyong_flag=1"));
                        if (dianliang_s <= 0)
                        {
                            //如果前1分钟没有初始电量，则以tongji_m_qidianbi中最新电量为准
                            dianliang_s = Convert.ToInt32(MySqlHelper.Get_sigle("select dianliang_e from tongji_m_qidianbi where zhandian_id='" + zhandian_id + "'  order by shijian desc limit 1"));
                        }

                        int qiliang_s = Convert.ToInt32(MySqlHelper.Get_sigle("select sum(shuju) from uv_caiji_base where shijian='" + shijian_last + "' and zhandian_id='" + zhandian_id + "' and zhucanshu='qiliang' and if_nenghaojisuan=1 and qiyong_flag=1"));
                        if (qiliang_s <= 0)
                        {
                            //如果前1分钟没有初始气量，则以tongji_m_qidianbi中最新气量为准
                            qiliang_s = Convert.ToInt32(MySqlHelper.Get_sigle("select qiliang_e from tongji_m_qidianbi where zhandian_id='" + zhandian_id + "' order by shijian desc limit 1"));
                        }

                        decimal yali = Utility.ToDecimal(MySqlHelper.Get_sigle("select shuju from uv_caiji_base where shijian='" + shijian + "' and zhandian_id='" + zhandian_id + "' and zhucanshu='yali' and if_zongguanyali=1 and qiyong_flag=1"));


                        sb.Append(biz_cls.caiji_m(zhandian_id, shijian, dianliang_s, dianliang_e, qiliang_s, qiliang_e, yali));

                    }

                    #endregion

                    #region 循环每条主参数数据，分别统计1m值

                    foreach (string zhandian_id in ls_zhandian)
                    {
                        DataSet ds = new DataSet();
                        ds = MySqlHelper.Get_DataSet("select * from uv_caiji_base where shijian='" + shijian + "' and zhandian_id ='" + zhandian_id + "'");
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                int shebei_id = Utility.ToInt(dr["shebei_id"]);
                                string zhucanshu = Utility.ToObjectString(dr["zhucanshu"]);
                                decimal shuju = Utility.ToDecimal(dr["shuju"]);

                                if (zhucanshu == "dianliang" || zhucanshu == "qiliang")
                                {
                                    //电量、气量，需要计算终值与初值之差
                                    decimal shuju_s = Utility.ToDecimal(MySqlHelper.Get_sigle("select shuju from uv_caiji_base where  shebei_id=" + shebei_id + " and zhucanshu='" + zhucanshu + "' and shijian<='" + shijian_last + "'  order by shijian desc limit 1"));
                                    //if (shuju_s < 0) 
                                    //{ 
                                    //    shuju_s = Utility.ToDecimal(MySqlHelper.Get_sigle("select shuju from tongji_m_value where shebei_id=" + shebei_id + " and zhucanshu='" + zhucanshu + "'  order by shijian desc limit 1"));
                                    //}
                                    decimal sj = shuju - shuju_s;
                                    sb.Append("insert into tongji_m_value values(0,'" + shijian + "'," + shebei_id + ",'" + zhucanshu + "'," + sj + ",'" + zhandian_id + "');");
                                }
                                else
                                {
                                    sb.Append("insert into tongji_m_value values(0,'" + shijian + "'," + shebei_id + ",'" + zhucanshu + "'," + shuju + ",'" + zhandian_id + "');");
                                }
                            }
                        }
                    }



                    #endregion

                    sb.Append("update caiji_zt set chuli_flag=1,shijian_chuli='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where id=" + id_zt + ";");
                    //执行1m气电比统计数据插入
                    MySqlHelper.ExecuteSql(sb.ToString());

                    //  biz_cls.write_log("系统日志", "后台服务", "工作站：" + client_id.ToString() + "#处理气电比统计数据——1m级数据统计完成", "");

                    //更新采集状态记录

                    #region 生成5m、h、day、month气电比统计数据

                    StringBuilder sb1 = new StringBuilder();
                    foreach (string zhandian_id in ls_zhandian)
                    {
                        sb1.Append(biz_cls.caiji_qidianbi_auto(zhandian_id, Utility.ToDateTime(shijian)));
                    }
                    if (sb1.ToString().Length > 10)
                    {
                        MySqlHelper.ExecuteSql(sb1.ToString());
                    }

                    #endregion

                    //   biz_cls.write_log("系统日志", "后台服务", "工作站：" + client_id.ToString() + "#处理气电比统计数据——气电比数据统计完成", "");

                    #region 生成5m、h设备主参数统计表

                    foreach (string zhandian_id in ls_zhandian)
                    {
                        biz_cls.tongji_zhucanshu(zhandian_id, Utility.ToDateTime(shijian));
                    }


                    #endregion


                    biz_cls.write_log("系统日志", "后台服务", "工作站：" + client_id.ToString() + "#处理气电比统计数据——结束", "");

                }


                qidianbi_chuli_flag = 0;
            }
            catch (Exception ex)
            {
                qidianbi_chuli_flag = 0;
                biz_cls.write_log("系统日志", "后台服务", ex.ToString(), "");
            }
        }

        #endregion

        #region 处理开关量、辅助参数统计数据

        private void kaiguan_fucanshu_chuli()
        {
            try
            {
                kaiguan_fucanshu_chuli_flag = 1;

                DataSet ds_zt = new DataSet();
                ds_zt = MySqlHelper.Get_DataSet("select id,zhandian_ids,client_id,shijian from caiji_zt where shijian>='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:00") + "' and caiji_flag>=2 and chuli_flag=0 order by shijian limit 1");
                if (ds_zt.Tables[0].Rows.Count > 0)
                {
                    DataRow dr_zt = ds_zt.Tables[0].Rows[0];
                    int id_zt = Utility.ToInt(dr_zt["id"]);
                    int client_id = Utility.ToInt(dr_zt["client_id"]);
                    string zhandian_ids = Utility.ToObjectString(dr_zt["zhandian_ids"]);
                    string shijian = Utility.ToObjectString(dr_zt["shijian"]);
                    string[] ls_zhandian = zhandian_ids.Split(',');

                    StringBuilder sb = new StringBuilder();
                    biz_cls.write_log("系统日志", "后台服务", "工作站：" + client_id.ToString() + "#处理开关量、辅助参数统计数据——开始", "");

                    #region 开关量处理

                    foreach (string zhandian_id in ls_zhandian)
                    {
                        DataSet ds = new DataSet();
                        ds = MySqlHelper.Get_DataSet("select shuju,shebei_id from uv_caiji_base where zhandian_id='" + zhandian_id + "' and shijian>='" + shijian + "' and zhucanshu='kaiguan'");
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                int shebei_id = Utility.ToInt(dr["shebei_id"]);
                                int kaiguan_new = Convert.ToInt32(dr["shuju"]);
                                sb.Append("insert into tongji_m_value values(0,'" + Utility.ToDateTime(shijian).ToString("yyyy-MM-dd HH:mm") + "'," + shebei_id + ",'kaiguan'," + kaiguan_new + ",'" + zhandian_id + "');");

                                int kaiguan_old = Utility.ToInt(MySqlHelper.Get_sigle("select qiting_flag from base_shebei where shebei_id=" + shebei_id));

                                if (kaiguan_old == 0)
                                {
                                    //原先关闭
                                    if (kaiguan_new == 1)
                                    {
                                        sb.Append("update base_shebei set qiting_flag=1,txshijian='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where shebei_id=" + shebei_id + ";");
                                        sb.Append("insert into caiji_shebei_qiting values(0," + shebei_id + ",'" + shijian + "','');");
                                    }
                                    else
                                    {
                                        sb.Append("update base_shebei set txshijian='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where shebei_id=" + shebei_id + ";");
                                    }
                                }
                                else
                                {
                                    if (kaiguan_new == 0)
                                    {
                                        sb.Append("update base_shebei set qiting_flag=0 ,txshijian='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'  where shebei_id=" + shebei_id + ";");
                                        sb.Append("update caiji_shebei_qiting set shijian_e='" + shijian + "' where shebei_id=" + shebei_id + " and shijian_e='';");
                                    }
                                    else
                                    {
                                        sb.Append("update base_shebei set txshijian='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where shebei_id=" + shebei_id + ";");
                                    }
                                }
                            }


                        }
                    }



                    #endregion


                    #region 辅助参数处理
                    foreach (string zhandian_id in ls_zhandian)
                    {
                        DataSet ds1 = new DataSet();
                        ds1 = MySqlHelper.Get_DataSet("select shebei_id,caiji_canshu_str from base_shebei where zhandian_id='" + zhandian_id + "' and (shebei_leixing_id='lixinji' or shebei_leixing_id='luoganji' or shebei_leixing_id='dianbiao')");
                        if (ds1.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow dr1 in ds1.Tables[0].Rows)
                            {
                                int shebei_id = Utility.ToInt(dr1["shebei_id"]);
                                string str = Utility.ToObjectString(dr1["caiji_canshu_str"]);

                                if (str != "")
                                {
                                    DataSet ds2 = new DataSet();
                                    ds2 = MySqlHelper.Get_DataSet("select shuju,fucanshu from uv_caiji_base where shijian>='" + shijian + "' and shebei_id=" + shebei_id);
                                    if (ds2.Tables[0].Rows.Count > 0)
                                    {
                                        foreach (DataRow dr2 in ds2.Tables[0].Rows)
                                        {
                                            string shuju = Utility.ToObjectString(dr2["shuju"]);
                                            string fucanshu = "{" + Utility.ToObjectString(dr2["fucanshu"]) + "}";
                                            str = str.Replace(fucanshu, shuju);
                                        }
                                    }
                                    sb.Append("update base_shebei set caiji_canshu_value='" + str + "' where shebei_id=" + shebei_id + ";");
                                }
                            }
                        }
                    }


                    #endregion

                    if (sb.ToString().Length > 10)
                    {
                        sb.Append("update caiji_zt set chuli_flag=1,shijian_chuli='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'where id=" + id_zt + ";");
                        MySqlHelper.ExecuteSql(sb.ToString());
                    }

                    biz_cls.write_log("系统日志", "后台服务", "工作站：" + client_id.ToString() + "#处理开关量、辅助参数统计数据——结束", "");

                }


                kaiguan_fucanshu_chuli_flag = 0;
            }
            catch (Exception ex)
            {
                kaiguan_fucanshu_chuli_flag = 0;
                biz_cls.write_log("系统日志", "后台服务", ex.Message, "");
            }
        }

        #endregion

        #region 删除10分钟前的采集数据 caiji_base

        private void data_10m_clear()
        {
            try
            {
                MySqlHelper.ExecuteSql("delete from caiji_base where shijian<='" + DateTime.Now.AddMinutes(-10).ToString("yyyy-MM-dd HH:mm:ss") + "'");

                biz_cls.write_log("系统日志", "后台服务", "删除5分钟前的数据", "");
            }
            catch (Exception ex)
            {
                biz_cls.write_log("系统日志", "后台服务", ex.Message, "");
            }
        }

        #endregion

        #region 每天清理统计数据 tongji_m_value tongji_5m_value 、tongji_m_qidianbi、tongji_5m_qidianbi

        private void data_day_clear()
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("delete from tongji_m_value where shijian<='" + DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd HH:mm") + "';");
                sb.Append("delete from tongji_5m_value where shijian<='" + DateTime.Now.AddMonths(-3).ToString("yyyy-MM-dd HH:mm") + "';");
                sb.Append("delete from tongji_m_qidianbi where shijian<='" + DateTime.Now.AddMonths(-2).ToString("yyyy-MM-dd HH:mm") + "';");
                sb.Append("delete from tongji_5m_qidianbi where shijian<='" + DateTime.Now.AddMonths(-6).ToString("yyyy-MM-dd HH:mm") + "';");
                sb.Append("delete from log_data where shijian<='" + DateTime.Now.AddDays(-10).ToString("yyyy-MM-dd HH:mm:ss") + "'");
                MySqlHelper.ExecuteSql(sb.ToString());

                biz_cls.write_log("系统日志", "后台服务", "每天数据清理", "");
            }
            catch (Exception ex)
            {
                biz_cls.write_log("系统日志", "后台服务", ex.Message, "");
            }
        }

        #endregion

        #region 报警明细处理

        private void baojing_chuli()
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                DataSet ds = new DataSet();
                ds = MySqlHelper.Get_DataSet("select title,baojing_leixing_id,min(shijian) as shijian_s from baojing_detail group by baojing_leixing_id,title");
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        int baojing_leixing_id = Utility.ToInt(dr["baojing_leixing_id"]);
                        string title = Utility.ToObjectString(dr["title"]);
                        string shijian_s = Utility.ToObjectString(dr["shijian_s"]);
                        if (Utility.ToInt(MySqlHelper.Get_sigle("select count(*) from baojing where title='" + title + "' and  shijian_e=''")) <= 0)
                        {
                            //新增报警记录
                            sb.Append("insert into baojing values('" + Guid.NewGuid().ToString() + "'," + baojing_leixing_id + ",'" + title + "','','" + shijian_s + "','','');");
                        }
                        //删除报警明细
                        sb.Append("delete from baojing_detail where title='" + title + "';");
                    }
                }
                if (sb.ToString().Length > 10)
                {
                    MySqlHelper.ExecuteSql(sb.ToString());
                }

                biz_cls.write_log("系统日志", "后台服务", "报警处理", "");
            }
            catch (Exception ex)
            {
                biz_cls.write_log("系统日志", "后台服务", ex.Message, "");
            }
        }

        #endregion

        private string TimeDiff(DateTime previousTime)
        {
            DateTime currentTime = DateTime.Now;
            TimeSpan difference = currentTime - previousTime;
            int days = difference.Days;
            int hours = difference.Hours;
            int minutes = difference.Minutes;
            int seconds = difference.Seconds;
            return $"{days} 天 {hours} 小时 {minutes} 分钟 {seconds} 秒";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.lb_timediff.Text = this.TimeDiff(shijian_start);
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
    }
}

