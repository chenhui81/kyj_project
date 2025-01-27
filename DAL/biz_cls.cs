using kyj_project.Common;
using kyj_project.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace kyj_project.DAL
{
    public class biz_cls
    {
        public static string xiangmu_id = ""; //项目ID
        public static string xiangmu_mingcheng = ""; //项目名称
        public static string client_mac = ""; //工作站MAC地址
        public static string client_name = ""; //工作站名称
        public static string client_txmk_ids = ""; //工作站通讯模块地址集
        public static string client_zhandian_ids = "";//工作站中的多个站点ID，形如1d,1g
        public static int client_num = 1;//工作站数量
        public static int client_id = 0;//当前工作站ID

        public static int page_rowcount = 30; //分页数量

        public static string yonghu_name = "";//用户姓名
        public static string juese_id = "";//角色ID
        public static string juese_name = "";//角色名称



        #region  写日志
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="leibie">系统日志or人工操作</param>
        /// <param name="leibie_sys">web/采集系统/后台服务</param>
        /// <param name="content">日志内容</param>
        /// <param name="username">操作用户名</param>
        public static void write_log(string leibie, string leibie_sys, string content, string username)
        {
            MySqlHelper.ExecuteSql("insert into log_data values(0,'" + leibie + "','" + leibie_sys + "','" + content.Replace("'", "''") + "','" + username + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')");
        }

        public static string write_log_str(string leibie, string leibie_sys, string content, string username)
        {
            return "insert into log_data values(0,'" + leibie + "','" + leibie_sys + "','" + content.Replace("'", "''") + "','" + username + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "');";
        }

        #endregion




        #region 获取用户列表

        public static List<en_zh_yonghu> get_yonghu_list()
        {
            List<en_zh_yonghu> ls = new List<en_zh_yonghu>();
            DataSet ds = new DataSet();
            ds = MySqlHelper.Get_DataSet("select * from zh_yonghu");
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    en_zh_yonghu en = new en_zh_yonghu
                    {
                        yonghu_id = Utility.ToObjectString(dr["yonghu_id"]),
                        yonghu_xingming = Utility.ToObjectString(dr["yonghu_xingming"]),
                        yonghu_bumen = Utility.ToObjectString(dr["yonghu_bumen"]),
                        yonghu_dianhua = Utility.ToObjectString(dr["yonghu_dianhua"]),
                        login_pwd = Utility.ToObjectString(dr["login_pwd"]),
                        juese_id = Utility.ToObjectString(dr["juese_id"])
                    };
                    ls.Add(en);
                }
            }
            return ls;
        }

        #endregion


        #region 分钟级电量/流量/气电比数据采集

        /// <summary>
        /// 生成分钟级电量/流量/气电比数据采集
        /// </summary>
        /// <param name="zhandian_id">站点ID</param>
        /// <param name="dt_shijian">统计时间 格式为yyyy-MM-dd HH:mm</param>
        /// <param name="dianliang_s">电量初值</param>
        /// <param name="dianliang_e">电量终值</param>
        /// <param name="qiliang_s">气量初值</param>
        /// <param name="qiliang_e">气量终值</param>
        /// <param name="yali">总管压力</param>
        /// <returns></returns>
        public static string caiji_m(string zhandian_id, string dt_shijian, int dianliang_s, int dianliang_e, int qiliang_s, int qiliang_e, decimal yali)
        {
            try
            {

                int dianliang = dianliang_e - dianliang_s;  //用电量 KWh
                int qiliang = qiliang_e - qiliang_s; //用气量 m³
                int gonglv = 0; //功率 KW
                int qiliang_pingjun = 0; //每分钟用气量 m³/min
                decimal qidianbi = 0;

                if (dianliang <= 0)
                {
                    dianliang = 0;
                    gonglv = 0;
                }
                else
                {
                    gonglv = dianliang * 60; //功率 KW
                }

                if (qiliang <= 0)
                {
                    qiliang = 0;
                    qiliang_pingjun = 0;
                    qidianbi = 0;
                }
                else
                {
                    qiliang_pingjun = qiliang;
                    qidianbi = Utility.ToDecimal(dianliang / qiliang);
                }

                return "insert into tongji_m_qidianbi values(0,'" + zhandian_id + "','" + dt_shijian + "'," + dianliang_s + "," + dianliang_e + "," + dianliang + "," + gonglv + "," + qiliang_s + "," + qiliang_e + "," + qiliang + "," + qiliang_pingjun + "," + yali + "," + qidianbi + ");";
            }
            catch
            {
                return "";
            }
        }

        #endregion

        #region 循环每分钟采集数据，生成5m/小时/天/月的气电比统计表，按站点

        public static string caiji_qidianbi_auto(string zhandian_id, DateTime dt_m)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                DataSet ds = new DataSet();
                ds = MySqlHelper.Get_DataSet("select * from tongji_m_qidianbi where zhandian_id='" + zhandian_id + "' and shijian='" + dt_m.ToString("yyyy-MM-dd HH:mm") + "'");
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    int dianliang_s = Utility.ToInt(dr["dianliang_s"]);
                    int dianliang_e = Utility.ToInt(dr["dianliang_e"]);
                    int qiliang_s = Utility.ToInt(dr["qiliang_s"]);
                    int qiliang_e = Utility.ToInt(dr["qiliang_e"]);
                    decimal yali = Utility.ToDecimal(dr["yali"]);

                    //更新气电比5分钟、小时、日、月级采集数据
                    sb.Append(biz_cls.caiji_qidianbi_5m_new(zhandian_id, dt_m, dianliang_s, dianliang_e, qiliang_s, qiliang_e, yali));
                    sb.Append(biz_cls.caiji_qidianbi_h_new(zhandian_id, dt_m, dianliang_s, dianliang_e, qiliang_s, qiliang_e, yali));
                    sb.Append(biz_cls.caiji_qidianbi_d_new(zhandian_id, dt_m, dianliang_s, dianliang_e, qiliang_s, qiliang_e, yali));
                    sb.Append(biz_cls.caiji_qidianbi_yue_new(zhandian_id, dt_m, dianliang_s, dianliang_e, qiliang_s, qiliang_e, yali));
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                biz_cls.write_log("系统日志", "后台服务", "caiji_qidianbi_auto#" + ex.ToString(), "");
                return "";
            }
        }

        //更新气电比5m级采集数据
        public static string caiji_qidianbi_5m_new(string zhandian_id, DateTime dt_m, int dianliang_s, int dianliang_e, int qiliang_s, int qiliang_e, decimal yali)
        {
            try
            {
                string sqlstr = "";
                string shijian_m = dt_m.ToString("yyyy-MM-dd HH:mm");
                int i = 5 * (dt_m.Minute / 5);
                string shijian_5m = dt_m.ToString("yyyy-MM-dd HH") + ":" + i.ToString("00");

                int dianliang = dianliang_e - dianliang_s;  //用电量 KWh
                int qiliang = qiliang_e - qiliang_s; //用气量 m³
                int gonglv = dianliang * 60; //功率 KW
                int qiliang_pingjun = qiliang; //每分钟用气量 m³/min
                decimal qidianbi = 0;
                if (qiliang > 0)
                {
                    qidianbi = Utility.ToDecimal(dianliang / qiliang);
                }

                DataSet ds = new DataSet();
                ds = MySqlHelper.Get_DataSet("select * from tongji_5m_qidianbi where zhandian_id='" + zhandian_id + "' and shijian='" + shijian_5m + "'");
                if (ds.Tables[0].Rows.Count <= 0)
                {
                    //没有记录

                    sqlstr = "insert into tongji_5m_qidianbi values(0,'" + zhandian_id + "','" + shijian_5m + "'," + dianliang_s + "," + dianliang_e + "," + dianliang + "," + gonglv + "," + qiliang_s + "," + qiliang_e + "," + qiliang + "," + qiliang_pingjun + "," + yali + "," + qidianbi + ");";
                }
                else
                {
                    //有记录
                    int i_minute = dt_m.Minute;

                    DataRow dr = ds.Tables[0].Rows[0];
                    dianliang_s = Utility.ToInt(dr["dianliang_s"]);
                    qiliang_s = Utility.ToInt(dr["qiliang_s"]);

                    //用电量 KWh 首尾相减与合计，取最大值
                    dianliang = dianliang_e - dianliang_s;
                    //int dianliang_totle = Utility.ToInt(MySqlHelper.Get_sigle("select sum(dianliang) from tongji_m_qidianbi where zhandian_id='" + zhandian_id + "' and shijian>='" + shijian_5m + "' and shijian<='" + shijian_m + "'"));
                    //if (dianliang < dianliang_totle) { dianliang = dianliang_totle; }

                    //用气量 m³ 首尾相减与合计，取最大值
                    qiliang = qiliang_e - qiliang_s;
                    //int qiliang_totle = Utility.ToInt(MySqlHelper.Get_sigle("select sum(qiliang) from tongji_m_qidianbi where zhandian_id='" + zhandian_id + "' and shijian>='" + shijian_5m + "' and shijian<='" + shijian_m + "'"));
                    //if (qiliang < qiliang_totle) { qiliang = qiliang_totle; }

                    gonglv = dianliang * 60 / (dt_m.Minute % 5 + 1);

                    qiliang_pingjun = qiliang / (dt_m.Minute % 5 + 1);

                    //气电比
                    if (dianliang == 0 || qiliang == 0)
                    {
                        qidianbi = 0;
                    }
                    else
                    {
                        qidianbi = Utility.ToDecimal(dianliang / qiliang);
                    }

                    decimal yali_avg = Utility.ToDecimal(MySqlHelper.Get_sigle("select avg(yali) from tongji_m_qidianbi where zhandian_id='" + zhandian_id + "' and  shijian>='" + shijian_5m + "' and shijian<'" + shijian_m + "';"));

                    sqlstr = "update tongji_5m_qidianbi set dianliang_s=" + dianliang_s + ",dianliang_e=" + dianliang_e + ",dianliang=" + dianliang + ",gonglv=" + gonglv + ",qiliang_s=" + qiliang_s + ",qiliang_e=" + qiliang_e + ",qiliang=" + qiliang + ",qiliang_pingjun=" + qiliang_pingjun + ",yali=" + yali_avg + ",qidianbi=" + qidianbi + " where zhandian_id='" + zhandian_id + "' and shijian='" + shijian_5m + "';";
                }

                return sqlstr;

            }
            catch (Exception ex)
            {
                biz_cls.write_log("系统日志", "后台服务", "caiji_qidianbi_5m_new#" + ex.ToString(), "");
                return "";
            }
        }

        //更新气电比小时级采集数据
        public static string caiji_qidianbi_h_new(string zhandian_id, DateTime dt_m, int dianliang_s, int dianliang_e, int qiliang_s, int qiliang_e, decimal yali)
        {
            try
            {
                string sqlstr = "";
                string shijian = dt_m.ToString("yyyy-MM-dd HH");
                string shijian_m = dt_m.ToString("yyyy-MM-dd HH:mm");
                string shijian_h = dt_m.ToString("yyyy-MM-dd HH:00");
                int dianliang = dianliang_e - dianliang_s;  //用电量 KWh
                int qiliang = qiliang_e - qiliang_s; //用气量 m³
                int gonglv = dianliang * 60; //功率 KW
                int qiliang_pingjun = qiliang; //每分钟用气量 m³/min
                decimal qidianbi = 0;
                if (qiliang > 0)
                {
                    qidianbi = Utility.ToDecimal(dianliang / qiliang);
                }

                DataSet ds = new DataSet();
                ds = MySqlHelper.Get_DataSet("select * from tongji_h_qidianbi where zhandian_id='" + zhandian_id + "' and shijian='" + shijian + "'");
                if (ds.Tables[0].Rows.Count <= 0)
                {
                    //没有记录

                    sqlstr = "insert into tongji_h_qidianbi values(0,'" + zhandian_id + "','" + shijian + "'," + dianliang_s + "," + dianliang_e + "," + dianliang + "," + gonglv + "," + qiliang_s + "," + qiliang_e + "," + qiliang + "," + qiliang_pingjun + "," + yali + "," + qidianbi + ");";
                }
                else
                {
                    //有记录
                    int i_minute = dt_m.Minute;

                    DataRow dr = ds.Tables[0].Rows[0];
                    dianliang_s = Utility.ToInt(dr["dianliang_s"]);
                    qiliang_s = Utility.ToInt(dr["qiliang_s"]);

                    //用电量 KWh 首尾相减与合计，取最大值
                    dianliang = dianliang_e - dianliang_s;
                    //int dianliang_totle = Utility.ToInt(MySqlHelper.Get_sigle("select sum(dianliang) from tongji_m_qidianbi where zhandian_id='" + zhandian_id + "' and shijian>='" + shijian_h + "' and shijian<='" + shijian_m + "'"));
                    //if (dianliang < dianliang_totle) { dianliang = dianliang_totle; }

                    //用气量 m³ 首尾相减与合计，取最大值
                    qiliang = qiliang_e - qiliang_s;
                    //int qiliang_totle = Utility.ToInt(MySqlHelper.Get_sigle("select sum(qiliang) from tongji_m_qidianbi where zhandian_id='" + zhandian_id + "' and shijian>='" + shijian_h + "' and shijian<='" + shijian_m + "'"));
                    //if (qiliang < qiliang_totle) { qiliang = qiliang_totle; }

                    gonglv = dianliang * 60 / (i_minute + 1);

                    qiliang_pingjun = qiliang / (i_minute + 1);

                    //气电比
                    if (dianliang == 0 || qiliang == 0)
                    {
                        qidianbi = 0;
                    }
                    else
                    {
                        qidianbi = Utility.ToDecimal(dianliang / qiliang);
                    }

                    decimal yali_avg = Utility.ToDecimal(MySqlHelper.Get_sigle("select avg(yali) from tongji_m_qidianbi where zhandian_id='" + zhandian_id + "' and  shijian>='" + shijian_h + "' and shijian<'" + shijian_m + "'"));

                    sqlstr = "update tongji_h_qidianbi set dianliang_s=" + dianliang_s + ",dianliang_e=" + dianliang_e + ",dianliang=" + dianliang + ",gonglv=" + gonglv + ",qiliang_s=" + qiliang_s + ",qiliang_e=" + qiliang_e + ",qiliang=" + qiliang + ",qiliang_pingjun=" + qiliang_pingjun + ",yali=" + yali_avg + ",qidianbi=" + qidianbi + " where zhandian_id='" + zhandian_id + "' and shijian='" + shijian + "';";
                }

                return sqlstr;

            }
            catch (Exception ex)
            {
                biz_cls.write_log("系统日志", "后台服务", "caiji_qidianbi_h_new#" + ex.ToString(), "");
                return "";
            }
        }


        //更新气电比天级采集数据
        public static string caiji_qidianbi_d_new(string zhandian_id, DateTime dt_m, int dianliang_s, int dianliang_e, int qiliang_s, int qiliang_e, decimal yali)
        {
            try
            {
                string sqlstr = "";
                string shijian = dt_m.ToString("yyyy-MM-dd");
                string shijian_m = dt_m.ToString("yyyy-MM-dd HH:mm");
                string shijian_d = dt_m.ToString("yyyy-MM-dd 00:00");
                int dianliang = dianliang_e - dianliang_s;  //用电量 KWh
                int qiliang = qiliang_e - qiliang_s; //用气量 m³
                int gonglv = dianliang * 60; //功率 KW
                int qiliang_pingjun = qiliang; //每分钟用气量 m³/min
                decimal qidianbi = 0;
                if (qiliang > 0)
                {
                    qidianbi = Utility.ToDecimal(dianliang / qiliang);
                }

                DataSet ds = new DataSet();
                ds = MySqlHelper.Get_DataSet("select * from tongji_d_qidianbi where zhandian_id='" + zhandian_id + "' and shijian='" + shijian + "';");
                if (ds.Tables[0].Rows.Count <= 0)
                {
                    //没有记录

                    sqlstr = "insert into tongji_d_qidianbi values(0,'" + zhandian_id + "','" + shijian + "'," + dianliang_s + "," + dianliang_e + "," + dianliang + "," + gonglv + "," + qiliang_s + "," + qiliang_e + "," + qiliang + "," + qiliang_pingjun + "," + yali + "," + qidianbi + ");";
                }
                else
                {
                    //有记录
                    int i_hour = dt_m.Hour;
                    int i_minute = 60 * i_hour + dt_m.Minute;

                    DataRow dr = ds.Tables[0].Rows[0];
                    dianliang_s = Utility.ToInt(dr["dianliang_s"]);
                    qiliang_s = Utility.ToInt(dr["qiliang_s"]);

                    //用电量 KWh 首尾相减与合计，取最大值
                    dianliang = dianliang_e - dianliang_s;
                    //int dianliang_totle = Utility.ToInt(MySqlHelper.Get_sigle("select sum(dianliang) from tongji_m_qidianbi where zhandian_id='" + zhandian_id + "' and shijian>='" + shijian_d + "' and shijian<='" + shijian_m + "'"));
                    //if (dianliang < dianliang_totle) { dianliang = dianliang_totle; }

                    //用气量 m³ 首尾相减与合计，取最大值
                    qiliang = qiliang_e - qiliang_s;
                    //int qiliang_totle = Utility.ToInt(MySqlHelper.Get_sigle("select sum(qiliang) from tongji_m_qidianbi where zhandian_id='" + zhandian_id + "' and shijian>='" + shijian_d + "' and shijian<='" + shijian_m + "'"));
                    //if (qiliang < qiliang_totle) { qiliang = qiliang_totle; }

                    gonglv = dianliang * 60 / (i_minute + 1);

                    qiliang_pingjun = qiliang / (i_minute + 1);

                    //气电比
                    if (dianliang == 0 || qiliang == 0)
                    {
                        qidianbi = 0;
                    }
                    else
                    {
                        qidianbi = Utility.ToDecimal(dianliang / qiliang);
                    }

                    decimal yali_avg = Utility.ToDecimal(MySqlHelper.Get_sigle("select avg(yali) from tongji_m_qidianbi where zhandian_id='" + zhandian_id + "' and  shijian>='" + shijian_d + "' and shijian<'" + shijian_m + "'"));

                    sqlstr = "update tongji_d_qidianbi set dianliang_s=" + dianliang_s + ",dianliang_e=" + dianliang_e + ",dianliang=" + dianliang + ",gonglv=" + gonglv + ",qiliang_s=" + qiliang_s + ",qiliang_e=" + qiliang_e + ",qiliang=" + qiliang + ",qiliang_pingjun=" + qiliang_pingjun + ",yali=" + yali_avg + ",qidianbi=" + qidianbi + " where zhandian_id='" + zhandian_id + "' and shijian='" + shijian + "';";
                }

                return sqlstr;

            }
            catch (Exception ex)
            {
                biz_cls.write_log("系统日志", "后台服务", "caiji_qidianbi_d_new#" + ex.ToString(), "");
                return "";
            }
        }


        //更新气电比月级采集数据
        public static string caiji_qidianbi_yue_new(string zhandian_id, DateTime dt_m, int dianliang_s, int dianliang_e, int qiliang_s, int qiliang_e, decimal yali)
        {
            try
            {
                string sqlstr = "";
                string shijian = dt_m.ToString("yyyy-MM");
                string shijian_m = dt_m.ToString("yyyy-MM-dd HH:mm");
                string shijian_yue = dt_m.ToString("yyyy-MM-00 00:00");
                int dianliang = dianliang_e - dianliang_s;  //用电量 KWh
                int qiliang = qiliang_e - qiliang_s; //用气量 m³
                int gonglv = dianliang * 60; //功率 KW
                int qiliang_pingjun = qiliang; //每分钟用气量 m³/min
                decimal qidianbi = 0;
                if (qiliang > 0)
                {
                    qidianbi = Utility.ToDecimal(dianliang / qiliang);
                }

                DataSet ds = new DataSet();
                ds = MySqlHelper.Get_DataSet("select * from tongji_yue_qidianbi where zhandian_id='" + zhandian_id + "' and shijian='" + shijian + "'");
                if (ds.Tables[0].Rows.Count <= 0)
                {
                    //没有记录

                    sqlstr = "insert into tongji_yue_qidianbi values(0,'" + zhandian_id + "','" + shijian + "'," + dianliang_s + "," + dianliang_e + "," + dianliang + "," + gonglv + "," + qiliang_s + "," + qiliang_e + "," + qiliang + "," + qiliang_pingjun + "," + yali + "," + qidianbi + ");";
                }
                else
                {
                    //有记录
                    int i_day = dt_m.Day;
                    int i_minute = (i_day - 1) * 24 * 60 + dt_m.Hour * 60 + dt_m.Minute;

                    DataRow dr = ds.Tables[0].Rows[0];
                    dianliang_s = Utility.ToInt(dr["dianliang_s"]);
                    qiliang_s = Utility.ToInt(dr["qiliang_s"]);

                    //用电量 KWh 首尾相减与合计，取最大值
                    dianliang = dianliang_e - dianliang_s;
                    //int dianliang_totle = Utility.ToInt(MySqlHelper.Get_sigle("select sum(dianliang) from tongji_m_qidianbi where zhandian_id='" + zhandian_id + "' and shijian>='" + shijian_yue + "' and shijian<='" + shijian_m + "'"));
                    //if (dianliang < dianliang_totle) { dianliang = dianliang_totle; }

                    //用气量 m³ 首尾相减与合计，取最大值
                    qiliang = qiliang_e - qiliang_s;
                    //int qiliang_totle = Utility.ToInt(MySqlHelper.Get_sigle("select sum(qiliang) from tongji_m_qidianbi where zhandian_id='" + zhandian_id + "' and shijian>='" + shijian_yue + "' and shijian<='" + shijian_m + "'"));
                    //if (qiliang < qiliang_totle) { qiliang = qiliang_totle; }

                    gonglv = dianliang * 60 / (i_minute + 1);

                    qiliang_pingjun = qiliang / (i_minute + 1);

                    //气电比
                    if (dianliang == 0 || qiliang == 0)
                    {
                        qidianbi = 0;
                    }
                    else
                    {
                        qidianbi = Utility.ToDecimal(dianliang / qiliang);
                    }

                    decimal yali_avg = Utility.ToDecimal(MySqlHelper.Get_sigle("select avg(yali) from tongji_m_qidianbi where zhandian_id='" + zhandian_id + "' and  shijian>='" + shijian_yue + "' and shijian<'" + shijian_m + "'"));

                    sqlstr = "update tongji_yue_qidianbi set dianliang_s=" + dianliang_s + ",dianliang_e=" + dianliang_e + ",dianliang=" + dianliang + ",gonglv=" + gonglv + ",qiliang_s=" + qiliang_s + ",qiliang_e=" + qiliang_e + ",qiliang=" + qiliang + ",qiliang_pingjun=" + qiliang_pingjun + ",yali=" + yali_avg + ",qidianbi=" + qidianbi + " where zhandian_id='" + zhandian_id + "' and shijian='" + shijian + "';";
                }

                return sqlstr;

            }
            catch (Exception ex)
            {
                biz_cls.write_log("系统日志", "后台服务", "caiji_qidianbi_yue_new#" + ex.ToString(), "");
                return "";
            }
        }


        #region 气电比统计 弃用

        ////更新气电比5分钟级采集数据
        //public static string caiji_qidianbi_5m(string zhandian_id, DateTime dt_m, decimal dianliang_s, decimal dianliang_e, decimal qiliang_s, decimal qiliang_e, decimal yali)
        //{
        //    try
        //    {
        //        string sqlstr = "";
        //        string shijian = dt_m.ToString("yyyy-MM-dd HH:mm");
        //        decimal dianliang = dianliang_e - dianliang_s;  //用电量 KWh
        //        decimal qiliang = qiliang_e - qiliang_s; //用气量 m³
        //        decimal gonglv = dianliang * 60; //功率 KW
        //        decimal qiliang_pingjun = qiliang; //每分钟用气量 m³/min
        //        decimal qidianbi = Utility.ToDecimal(dianliang / qiliang);

        //        int i = 5 * (dt_m.Minute / 5);
        //        string shijian1 = dt_m.ToString("yyyy-MM-dd HH") + ":" + i.ToString("00");

        //        DataSet ds = new DataSet();
        //        ds = MySqlHelper.Get_DataSet("select * from tongji_5m_qidianbi where zhandian_id='" + zhandian_id + "' and shijian='" + shijian1 + "'");
        //        if (ds.Tables[0].Rows.Count <= 0)
        //        {
        //            //没有记录

        //            sqlstr = "insert into tongji_5m_qidianbi values(0,'" + zhandian_id + "','" + shijian1 + "'," + dianliang_s + "," + dianliang_e + "," + dianliang + "," + gonglv + "," + qiliang_s + "," + qiliang_e + "," + qiliang + "," + qiliang_pingjun + "," + yali + "," + qidianbi + ");";
        //        }
        //        else
        //        {
        //            //有记录
        //            int i_minute = dt_m.Minute;

        //            DataRow dr = ds.Tables[0].Rows[0];
        //            dianliang_s = Utility.ToDecimal(dr["dianliang_s"]);
        //            dianliang = dianliang_e - dianliang_s; //用电量 KWh
        //            qiliang_s = Utility.ToDecimal(dr["qiliang_s"]);
        //            qiliang = qiliang_e - qiliang_s; //用气量 m³
        //            gonglv = dianliang * 60 / (dt_m.Minute % 5 + 1);
        //            qiliang_pingjun = qiliang / (dt_m.Minute % 5 + 1);
        //            qidianbi = Utility.ToDecimal(dianliang / qiliang);
        //            decimal yali_avg1 = Utility.ToDecimal(MySqlHelper.Get_sigle("select sum(yali) from tongji_m_qidianbi where shijian>='" + shijian1 + "' and shijian<'" + dt_m.ToString("yyyy-MM-dd HH:mm") + "'"));
        //            decimal yali_avg = (yali_avg1 + yali) / (dt_m.Minute % 5 + 1);

        //            // sqlstr = "insert into tongji_h_qidianbi values(0,'" + zhandian_id + "','" + shijian + "'," + dianliang_s + "," + dianliang_e + "," + dianliang + "," + gonglv + "," + qiliang_s + "," + qiliang_e + "," + qiliang + "," + qiliang_pingjun + "," + yali + "," + qidianbi + ");";
        //            sqlstr = "update tongji_5m_qidianbi set dianliang_s=" + dianliang_s + ",dianliang_e=" + dianliang_e + ",dianliang=" + dianliang + ",gonglv=" + gonglv + ",qiliang_s=" + qiliang_s + ",qiliang_e=" + qiliang_e + ",qiliang=" + qiliang + ",qiliang_pingjun=" + qiliang_pingjun + ",yali=" + yali_avg + ",qidianbi=" + qidianbi + " where zhandian_id='" + zhandian_id + "' and shijian='" + shijian1 + "'";
        //        }

        //        MySqlHelper.ExecuteSql(sqlstr);



        //        return "";
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.Message;
        //    }
        //}


        ////更新气电比小时级采集数据
        //public static string caiji_qidianbi_h(string zhandian_id, DateTime dt_m, decimal dianliang_s, decimal dianliang_e, decimal qiliang_s, decimal qiliang_e, decimal yali)
        //{
        //    try
        //    {
        //        string sqlstr = "";
        //        string shijian = dt_m.ToString("yyyy-MM-dd HH");
        //        decimal dianliang = dianliang_e - dianliang_s;  //用电量 KWh
        //        decimal qiliang = qiliang_e - qiliang_s; //用气量 m³
        //        decimal gonglv = dianliang * 60; //功率 KW
        //        decimal qiliang_pingjun = qiliang; //每分钟用气量 m³/min
        //        decimal qidianbi = Utility.ToDecimal(dianliang / qiliang);

        //        DataSet ds = new DataSet();
        //        ds = MySqlHelper.Get_DataSet("select * from tongji_h_qidianbi where zhandian_id='" + zhandian_id + "' and shijian='" + shijian + "'");
        //        if (ds.Tables[0].Rows.Count <= 0)
        //        {
        //            //没有记录

        //            sqlstr = "insert into tongji_h_qidianbi values(0,'" + zhandian_id + "','" + shijian + "'," + dianliang_s + "," + dianliang_e + "," + dianliang + "," + gonglv + "," + qiliang_s + "," + qiliang_e + "," + qiliang + "," + qiliang_pingjun + "," + yali + "," + qidianbi + ");";
        //        }
        //        else
        //        {
        //            //有记录
        //            int i_minute = dt_m.Minute;

        //            DataRow dr = ds.Tables[0].Rows[0];
        //            dianliang_s = Utility.ToDecimal(dr["dianliang_s"]);
        //            dianliang = dianliang_e - dianliang_s; //用电量 KWh
        //            qiliang_s = Utility.ToDecimal(dr["qiliang_s"]);
        //            qiliang = qiliang_e - qiliang_s; //用气量 m³
        //            gonglv = dianliang * 60 / (i_minute + 1);
        //            qiliang_pingjun = qiliang / (i_minute + 1);
        //            qidianbi = Utility.ToDecimal(dianliang / qiliang);
        //            decimal yali_avg1 = Utility.ToDecimal(MySqlHelper.Get_sigle("select sum(yali) from tongji_m_qidianbi where shijian>='" + dt_m.ToString("yyyy-MM-dd HH:00") + "' and shijian<'" + dt_m.ToString("yyyy-MM-dd HH:mm") + "'"));
        //            decimal yali_avg = (yali_avg1 + yali) / (i_minute + 1);

        //            // sqlstr = "insert into tongji_h_qidianbi values(0,'" + zhandian_id + "','" + shijian + "'," + dianliang_s + "," + dianliang_e + "," + dianliang + "," + gonglv + "," + qiliang_s + "," + qiliang_e + "," + qiliang + "," + qiliang_pingjun + "," + yali + "," + qidianbi + ");";
        //            sqlstr = "update tongji_h_qidianbi set dianliang_s=" + dianliang_s + ",dianliang_e=" + dianliang_e + ",dianliang=" + dianliang + ",gonglv=" + gonglv + ",qiliang_s=" + qiliang_s + ",qiliang_e=" + qiliang_e + ",qiliang=" + qiliang + ",qiliang_pingjun=" + qiliang_pingjun + ",yali=" + yali_avg + ",qidianbi=" + qidianbi + " where zhandian_id='" + zhandian_id + "' and shijian='" + shijian + "'";
        //        }

        //        MySqlHelper.ExecuteSql(sqlstr);

        //        //更新气电比日级采集数据
        //        biz_cls.caiji_qidianbi_d(zhandian_id, dt_m, dianliang_s, dianliang_e, qiliang_s, qiliang_e, yali);

        //        return "";
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.Message;
        //    }
        //}

        ////更新气电比日级采集数据
        //public static string caiji_qidianbi_d(string zhandian_id, DateTime dt_m, decimal dianliang_s, decimal dianliang_e, decimal qiliang_s, decimal qiliang_e, decimal yali)
        //{
        //    try
        //    {
        //        string sqlstr = "";
        //        string shijian = dt_m.ToString("yyyy-MM-dd");
        //        decimal dianliang = dianliang_e - dianliang_s;  //用电量 KWh
        //        decimal qiliang = qiliang_e - qiliang_s; //用气量 m³
        //        decimal gonglv = dianliang * 60; //功率 KW
        //        decimal qiliang_pingjun = qiliang; //每分钟用气量 m³/min
        //        decimal qidianbi = Utility.ToDecimal(dianliang / qiliang);

        //        DataSet ds = new DataSet();
        //        ds = MySqlHelper.Get_DataSet("select * from tongji_d_qidianbi where zhandian_id='" + zhandian_id + "' and shijian='" + shijian + "'");
        //        if (ds.Tables[0].Rows.Count <= 0)
        //        {
        //            //没有记录

        //            sqlstr = "insert into tongji_d_qidianbi values(0,'" + zhandian_id + "','" + shijian + "'," + dianliang_s + "," + dianliang_e + "," + dianliang + "," + gonglv + "," + qiliang_s + "," + qiliang_e + "," + qiliang + "," + qiliang_pingjun + "," + yali + "," + qidianbi + ");";
        //        }
        //        else
        //        {
        //            //有记录
        //            int i_hour = dt_m.Hour;
        //            int i_minute = 60 * i_hour + dt_m.Minute + 1;

        //            DataRow dr = ds.Tables[0].Rows[0];
        //            dianliang_s = Utility.ToDecimal(dr["dianliang_s"]);
        //            qiliang_s = Utility.ToDecimal(dr["qiliang_s"]);

        //            dianliang = dianliang_e - dianliang_s; //用电量 KWh

        //            qiliang = qiliang_e - qiliang_s; //用气量 m³

        //            gonglv = dianliang * 60 / i_minute;
        //            qiliang_pingjun = qiliang / i_minute;
        //            qidianbi = Utility.ToDecimal(dianliang / qiliang);
        //            decimal yali_avg1 = Utility.ToDecimal(MySqlHelper.Get_sigle("select sum(yali) from tongji_m_qidianbi where shijian>='" + dt_m.ToString("yyyy-MM-dd 00:00") + "' and shijian<'" + dt_m.ToString("yyyy-MM-dd HH:mm") + "'"));
        //            decimal yali_avg = (yali_avg1 + yali) / i_minute;

        //            // sqlstr = "insert into tongji_d_qidianbi values(0,'" + zhandian_id + "','" + shijian + "'," + dianliang_s + "," + dianliang_e + "," + dianliang + "," + gonglv + "," + qiliang_s + "," + qiliang_e + "," + qiliang + "," + qiliang_pingjun + "," + yali + "," + qidianbi + ");";
        //            sqlstr = "update tongji_d_qidianbi set dianliang_s=" + dianliang_s + ",dianliang_e=" + dianliang_e + ",dianliang=" + dianliang + ",gonglv=" + gonglv + ",qiliang_s=" + qiliang_s + ",qiliang_e=" + qiliang_e + ",qiliang=" + qiliang + ",qiliang_pingjun=" + qiliang_pingjun + ",yali=" + yali_avg + ",qidianbi=" + qidianbi + " where zhandian_id='" + zhandian_id + "' and shijian='" + shijian + "'";
        //        }

        //        MySqlHelper.ExecuteSql(sqlstr);

        //        //更新气电比月级采集数据
        //        biz_cls.caiji_qidianbi_yue(zhandian_id, dt_m, dianliang_s, dianliang_e, qiliang_s, qiliang_e, yali);

        //        return "";
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.Message;
        //    }
        //}

        ////更新气电比月级采集数据
        //public static string caiji_qidianbi_yue(string zhandian_id, DateTime dt_m, decimal dianliang_s, decimal dianliang_e, decimal qiliang_s, decimal qiliang_e, decimal yali)
        //{
        //    try
        //    {
        //        string sqlstr = "";
        //        string shijian = dt_m.ToString("yyyy-MM");
        //        decimal dianliang = dianliang_e - dianliang_s;  //用电量 KWh
        //        decimal qiliang = qiliang_e - qiliang_s; //用气量 m³
        //        decimal gonglv = dianliang * 60; //功率 KW
        //        decimal qiliang_pingjun = qiliang; //每分钟用气量 m³/min
        //        decimal qidianbi = Utility.ToDecimal(dianliang / qiliang);

        //        DataSet ds = new DataSet();
        //        ds = MySqlHelper.Get_DataSet("select * from tongji_yue_qidianbi where zhandian_id='" + zhandian_id + "' and shijian='" + shijian + "'");
        //        if (ds.Tables[0].Rows.Count <= 0)
        //        {
        //            //没有记录

        //            sqlstr = "insert into tongji_yue_qidianbi values(0,'" + zhandian_id + "','" + shijian + "'," + dianliang_s + "," + dianliang_e + "," + dianliang + "," + gonglv + "," + qiliang_s + "," + qiliang_e + "," + qiliang + "," + qiliang_pingjun + "," + yali + "," + qidianbi + ");";
        //        }
        //        else
        //        {
        //            //有记录
        //            int i_day = dt_m.Day;
        //            int i_minute = (i_day - 1) * 24 * 60 + dt_m.Hour * 60 + dt_m.Minute + 1;

        //            DataRow dr = ds.Tables[0].Rows[0];
        //            dianliang_s = Utility.ToDecimal(dr["dianliang_s"]);
        //            dianliang = dianliang_e - dianliang_s; //用电量 KWh
        //            qiliang_s = Utility.ToDecimal(dr["qiliang_s"]);
        //            qiliang = qiliang_e - qiliang_s; //用气量 m³
        //            gonglv = dianliang * 60 / i_minute;
        //            qiliang_pingjun = Utility.ToDecimal(dr["qiliang"]) / i_minute;
        //            qidianbi = Utility.ToDecimal(dianliang / qiliang);
        //            decimal yali_avg1 = Utility.ToDecimal(MySqlHelper.Get_sigle("select sum(yali) from tongji_m_qidianbi where shijian>='" + dt_m.ToString("yyyy-MM-dd 00:00") + "' and shijian<'" + dt_m.ToString("yyyy-MM-dd HH:mm") + "'"));
        //            decimal yali_avg = (yali_avg1 + yali) / i_minute;

        //            // sqlstr = "insert into tongji_yue_qidianbi values(0,'" + zhandian_id + "','" + shijian + "'," + dianliang_s + "," + dianliang_e + "," + dianliang + "," + gonglv + "," + qiliang_s + "," + qiliang_e + "," + qiliang + "," + qiliang_pingjun + "," + yali + "," + qidianbi + ");";
        //            sqlstr = "update tongji_yue_qidianbi set dianliang_s=" + dianliang_s + ",dianliang_e=" + dianliang_e + ",dianliang=" + dianliang + ",gonglv=" + gonglv + ",qiliang_s=" + qiliang_s + ",qiliang_e=" + qiliang_e + ",qiliang=" + qiliang + ",qiliang_pingjun=" + qiliang_pingjun + ",yali=" + yali_avg + ",qidianbi=" + qidianbi + " where zhandian_id='" + zhandian_id + "' and shijian='" + shijian + "'";
        //        }

        //        MySqlHelper.ExecuteSql(sqlstr);
        //        return "";
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.Message;
        //    }
        //}


        #endregion

        #endregion


        #region 统计5m.h主参数数据

        public static void tongji_zhucanshu(string zhandian_id, DateTime dt_m)
        {
            try
            {
                string shijian = dt_m.ToString("yyyy-MM-dd HH:mm");
                int i = 5 * (dt_m.Minute / 5);
                string shijian1 = dt_m.ToString("yyyy-MM-dd HH") + ":" + i.ToString("00");
                string shijian2 = dt_m.ToString("yyyy-MM-dd HH:00");
                string shijian3 = dt_m.ToString("yyyy-MM-dd HH");
                StringBuilder sb = new StringBuilder();

                DataSet ds = new DataSet();
                ds = MySqlHelper.Get_DataSet("select * from tongji_m_value where shijian='" + shijian + "' and zhandian_id='" + zhandian_id + "'");
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        int shebei_id = Utility.ToInt(dr["shebei_id"]);
                        string zhucanshu = Utility.ToObjectString(dr["zhucanshu"]);
                        decimal shujv_5m_avg = Utility.ToDecimal(MySqlHelper.Get_sigle("select avg(shuju) from tongji_m_value where shebei_id=" + shebei_id + " and zhucanshu='" + zhucanshu + "' and shijian>='" + shijian1 + "' and shijian<='" + shijian + "'"));
                        decimal shujv_h_avg = Utility.ToDecimal(MySqlHelper.Get_sigle("select avg(shuju) from tongji_m_value where shebei_id=" + shebei_id + " and zhucanshu='" + zhucanshu + "' and  shijian>='" + shijian2 + "' and shijian<='" + shijian + "'"));

                        int id_5m = Utility.ToInt(MySqlHelper.Get_sigle("select id from tongji_5m_value where shijian='" + shijian1 + "' and shebei_id=" + shebei_id + " and zhucanshu='" + zhucanshu + "'"));
                        if (id_5m <= 0)
                        {
                            sb.Append("insert into tongji_5m_value values(0,'" + shijian1 + "'," + shebei_id + ",'" + zhucanshu + "'," + shujv_5m_avg + ",'" + zhandian_id + "');");
                        }
                        else
                        {
                            sb.Append("update tongji_5m_value set shuju=" + shujv_5m_avg + " where id=" + id_5m + ";");
                        }

                        int id_h = Utility.ToInt(MySqlHelper.Get_sigle("select id from tongji_h_value where shijian='" + shijian3 + "' and shebei_id=" + shebei_id + " and zhucanshu='" + zhucanshu + "'"));
                        if (id_h <= 0)
                        {
                            sb.Append("insert into tongji_h_value values(0,'" + shijian3 + "'," + shebei_id + ",'" + zhucanshu + "'," + shujv_h_avg + ",'" + zhandian_id + "');");
                        }
                        else
                        {
                            sb.Append("update tongji_h_value set shuju=" + shujv_h_avg + " where id=" + id_h + ";");
                        }
                    }

                    if (sb.ToString().Length > 10)
                    {
                        MySqlHelper.ExecuteSql(sb.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region  统计前一日/月/年项目的整体气电比报表

        #region  统计前一日项目气电比报表

        public static void rpt_ri_g(DateTime dt)
        {
            try
            {

                int i = Utility.ToInt(MySqlHelper.Get_sigle("select count(*) from rpt_ri where riqi='" + dt.ToString("yyyy-MM-dd") + "'"));
                if (i <= 0)
                {
                    #region  如果没有当日报表记录，则插入

                    DataSet ds = new DataSet();
                    ds = MySqlHelper.Get_DataSet("select sum(dianliang_s) as dianliang_s,sum(dianliang_e) as dianliang_e,sum(dianliang) as dianliang,sum(qiliang_s) as qiliang_s , sum(qiliang_e ) as qiliang_e,sum(qiliang) as qiliang from tongji_d_qidianbi where shijian='" + dt.ToString("yyyy-MM-dd") + "'");
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataRow dr = ds.Tables[0].Rows[0];
                        decimal dianliang_s = Utility.ToDecimal(dr["dianliang_s"]);
                        decimal dianliang_e = Utility.ToDecimal(dr["dianliang_e"]);
                        decimal dianliang = Utility.ToDecimal(dr["dianliang"]);
                        decimal qiliang_s = Utility.ToDecimal(dr["qiliang_s"]);
                        decimal qiliang_e = Utility.ToDecimal(dr["qiliang_e"]);
                        decimal qiliang = Utility.ToDecimal(dr["qiliang"]);

                        string xiangmu_id = "";
                        decimal hetong_dianjia = 0;
                        decimal hetong_qidianbi = 0;
                        DataSet ds1 = new DataSet();
                        ds1 = MySqlHelper.Get_DataSet("select xiangmu_id,hetong_dianjia,hetong_qidianbi from base_xiangmu ");
                        if (ds1.Tables[0].Rows.Count > 0)
                        {
                            DataRow dr1 = ds1.Tables[0].Rows[0];
                            xiangmu_id = Utility.ToObjectString(dr1["xiangmu_id"]);
                            hetong_dianjia = Utility.ToDecimal(dr1["hetong_dianjia"]);
                            hetong_qidianbi = Utility.ToDecimal(dr1["hetong_qidianbi"]);
                        }
                        if (hetong_dianjia <= 0 || hetong_qidianbi <= 0)
                        {
                            throw new Exception("项目的合同电价和合同气电比未设置，不能统计");
                        }

                        decimal qidianbi = 0;
                        if (qiliang <= 0) { qidianbi = 0; } else { qidianbi = dianliang / qiliang; }
                        decimal jienenglv = (hetong_qidianbi - qidianbi) / hetong_qidianbi;
                        decimal jieyue_dianliang = qiliang * (hetong_qidianbi - qidianbi);
                        decimal jieyue_dianjia = jieyue_dianliang * hetong_dianjia;

                        string riqi = dt.ToString("yyyy-MM-dd");

                        int shebei_num = Utility.ToInt(MySqlHelper.Get_sigle("select count(*) from base_shebei where (shebei_leixing_id='lixinji' or shebei_leixing_id='luoganji')"));


                        string sqlstr = "insert into rpt_ri values(0,'" + xiangmu_id + "','" + riqi + "'," + shebei_num + "," + hetong_dianjia + "," + hetong_qidianbi + "," + dianliang_s + "," + dianliang_e + "," + dianliang + "," + qiliang_s + "," + qiliang_e + "," + qiliang + "," + qidianbi + "," + jienenglv + "," + jieyue_dianliang + "," + jieyue_dianjia + ")";
                        MySqlHelper.ExecuteSql(sqlstr);

                        biz_cls.rpt_yue_g(dt, xiangmu_id, hetong_dianjia, hetong_qidianbi, shebei_num); //生成月报表
                        biz_cls.rpt_nian_g(dt, xiangmu_id, hetong_dianjia, hetong_qidianbi, shebei_num);//生成年报表
                    }

                    #endregion


                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region  统计前一日,月度项目气电比报表

        public static void rpt_yue_g(DateTime dt, string xiangmu_id, decimal hetong_dianjia, decimal hetong_qidianbi, int shebei_num)
        {
            try
            {
                string riqi = dt.ToString("yyyy-MM");
                string riqi_se = dt.ToString("yyyy年MM月01日") + "-" + dt.ToString("yyyy年MM月dd日");

                int i = Utility.ToInt(MySqlHelper.Get_sigle("select id from rpt_yue where riqi='" + riqi + "'"));
                if (i <= 0)
                {

                    decimal dianliang_s = 0;
                    decimal dianliang_e = 0;
                    decimal dianliang = 0;
                    decimal qiliang_s = 0;
                    decimal qiliang_e = 0;
                    decimal qiliang = 0;

                    decimal qidianbi = 0;
                    decimal jienenglv = 0;
                    decimal jieyue_dianliang = 0;
                    decimal jieyue_dianfei = 0;


                    string first_day = Utility.ToObjectString(MySqlHelper.Get_sigle("select riqi from rpt_ri where riqi>='" + dt.ToString("yyyy-MM-01") + "' and riqi<='" + dt.ToString("yyyy-MM-dd") + "' order by riqi limit 1"));
                    DataSet ds2 = MySqlHelper.Get_DataSet("select sum(dianliang_s) as dianliang_s,sum(qiliang_s) as qiliang_s from rpt_ri where riqi='" + first_day + "'");
                    if (ds2.Tables[0].Rows.Count > 0)
                    {
                        DataRow dr2 = ds2.Tables[0].Rows[0];
                        dianliang_s = Utility.ToDecimal(dr2["dianliang_s"]);
                        qiliang_s = Utility.ToDecimal(dr2["qiliang_s"]);
                    }

                    DataSet ds3 = MySqlHelper.Get_DataSet("select sum(dianliang_e) as dianliang_e,sum(qiliang_e) as qiliang_e from rpt_ri where riqi='" + dt.ToString("yyyy-MM-dd") + "'");
                    if (ds3.Tables[0].Rows.Count > 0)
                    {
                        DataRow dr3 = ds3.Tables[0].Rows[0];
                        dianliang_e = Utility.ToDecimal(dr3["dianliang_e"]);
                        qiliang_e = Utility.ToDecimal(dr3["qiliang_e"]);
                    }
                    else
                    {
                        return;
                    }

                    DataSet ds4 = MySqlHelper.Get_DataSet("select sum(dianliang) as dianliang,sum(qiliang) as qiliang from rpt_ri where riqi>='" + dt.ToString("yyyy-MM-01") + "' and riqi<='" + dt.ToString("yyyy-MM-dd") + "'");
                    if (ds4.Tables[0].Rows.Count > 0)
                    {
                        DataRow dr4 = ds4.Tables[0].Rows[0];
                        dianliang = Utility.ToDecimal(dr4["dianliang"]);
                        qiliang = Utility.ToDecimal(dr4["qiliang"]);
                    }
                    else
                    {
                        return;
                    }

                    if (qiliang <= 0) { throw new Exception("气电比数据统计错误"); }
                    qidianbi = dianliang / qiliang;


                    jienenglv = (hetong_qidianbi - qidianbi) / hetong_qidianbi;
                    jieyue_dianliang = qiliang * (hetong_qidianbi - qidianbi);
                    jieyue_dianfei = jieyue_dianliang * hetong_dianjia;

                    //  int shebei_num = Utility.ToInt(MySqlHelper.Get_sigle("select count(*) from base_shebei where (shebei_leixing_id='lixinji' or shebei_leixing_id='luoganji')"));


                    string sqlstr = "insert into rpt_yue values(0,'" + xiangmu_id + "','" + riqi + "','" + riqi_se + "'," + shebei_num + "," + hetong_dianjia + "," + hetong_qidianbi + "," + dianliang_s + "," + dianliang_e + "," + dianliang + "," + qiliang_s + "," + qiliang_e + "," + qiliang + "," + qidianbi + "," + jienenglv + "," + jieyue_dianliang + "," + jieyue_dianfei + ")";
                    MySqlHelper.ExecuteSql(sqlstr);

                }
                else
                {
                    int j = Utility.ToInt(MySqlHelper.Get_sigle("select id from rpt_yue where riqi='" + riqi + "' and riqi_se='" + riqi_se + "'"));
                    if (j <= 0)
                    {
                        //如果没有当天的统计
                        decimal dianliang_s = 0;
                        decimal dianliang_e = 0;
                        decimal dianliang = 0;
                        decimal qiliang_s = 0;
                        decimal qiliang_e = 0;
                        decimal qiliang = 0;

                        decimal qidianbi = 0;
                        decimal jienenglv = 0;
                        decimal jieyue_dianliang = 0;
                        decimal jieyue_dianfei = 0;


                        string first_day = Utility.ToObjectString(MySqlHelper.Get_sigle("select riqi from rpt_ri where riqi>='" + dt.ToString("yyyy-MM-01") + "' and riqi<='" + dt.ToString("yyyy-MM-dd") + "' order by riqi limit 1"));
                        DataSet ds2 = MySqlHelper.Get_DataSet("select sum(dianliang_s) as dianliang_s,sum(qiliang_s) as qiliang_s from rpt_ri where riqi='" + first_day + "'");
                        if (ds2.Tables[0].Rows.Count > 0)
                        {
                            DataRow dr2 = ds2.Tables[0].Rows[0];
                            dianliang_s = Utility.ToDecimal(dr2["dianliang_s"]);
                            qiliang_s = Utility.ToDecimal(dr2["qiliang_s"]);
                        }

                        DataSet ds3 = MySqlHelper.Get_DataSet("select sum(dianliang_e) as dianliang_e,sum(qiliang_e) as qiliang_e from rpt_ri where riqi='" + dt.ToString("yyyy-MM-dd") + "'");
                        if (ds3.Tables[0].Rows.Count > 0)
                        {
                            DataRow dr3 = ds3.Tables[0].Rows[0];
                            dianliang_e = Utility.ToDecimal(dr3["dianliang_e"]);
                            qiliang_e = Utility.ToDecimal(dr3["qiliang_e"]);
                        }
                        else
                        {
                            return;
                        }

                        DataSet ds4 = MySqlHelper.Get_DataSet("select sum(dianliang) as dianliang,sum(qiliang) as qiliang from rpt_ri where riqi>='" + dt.ToString("yyyy-MM-01") + "' and riqi<='" + dt.ToString("yyyy-MM-dd") + "'");
                        if (ds4.Tables[0].Rows.Count > 0)
                        {
                            DataRow dr4 = ds4.Tables[0].Rows[0];
                            dianliang = Utility.ToDecimal(dr4["dianliang"]);
                            qiliang = Utility.ToDecimal(dr4["qiliang"]);
                        }
                        else
                        {
                            return;
                        }

                        if (qiliang <= 0) { throw new Exception("气电比数据统计错误"); }
                        qidianbi = dianliang / qiliang;


                        jienenglv = (hetong_qidianbi - qidianbi) / hetong_qidianbi;
                        jieyue_dianliang = qiliang * (hetong_qidianbi - qidianbi);
                        jieyue_dianfei = jieyue_dianliang * hetong_dianjia;

                        // int shebei_num = Utility.ToInt(MySqlHelper.Get_sigle("select count(*) from base_shebei where (shebei_leixing_id='lixinji' or shebei_leixing_id='luoganji')"));


                        string sqlstr = "update rpt_yue set xiangmu_id='" + xiangmu_id + "',riqi='" + riqi + "',riqi_se='" + riqi_se + "',shebei_num=" + shebei_num + ",dianjia=" + hetong_dianjia + ",qidianbi_old=" + hetong_qidianbi + ",dianliang_s=" + dianliang_s + ",dianliang_e=" + dianliang_e + ",dianliang=" + dianliang + ",qiliang_s=" + qiliang + ",qiliang_e=" + qiliang_e + ",qiliang=" + qiliang + ",qidianbi=" + qidianbi + ",jienenglv=" + jienenglv + ",jieyue_dianliang=" + jieyue_dianliang + ",jieyue_dianfei=" + jieyue_dianfei + " where id=" + i;
                        MySqlHelper.ExecuteSql(sqlstr);

                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region  统计前一日,年度项目气电比报表

        public static void rpt_nian_g(DateTime dt, string xiangmu_id, decimal hetong_dianjia, decimal hetong_qidianbi, int shebei_num)
        {
            try
            {
                string riqi = dt.ToString("yyyy");
                string riqi_se = dt.ToString("yyyy年01月01日") + "-" + dt.ToString("yyyy年MM月dd日");

                int i = Utility.ToInt(MySqlHelper.Get_sigle("select id from rpt_nian where riqi='" + riqi + "'"));
                if (i <= 0)
                {
                    decimal dianliang_s = 0;
                    decimal dianliang_e = 0;
                    decimal dianliang = 0;
                    decimal qiliang_s = 0;
                    decimal qiliang_e = 0;
                    decimal qiliang = 0;

                    decimal qidianbi = 0;
                    decimal jienenglv = 0;
                    decimal jieyue_dianliang = 0;
                    decimal jieyue_dianfei = 0;


                    string first_day = Utility.ToObjectString(MySqlHelper.Get_sigle("select riqi from rpt_ri where riqi>='" + dt.ToString("yyyy-01-01") + "' and riqi<='" + dt.ToString("yyyy-MM-dd") + "' order by riqi limit 1"));
                    DataSet ds2 = MySqlHelper.Get_DataSet("select sum(dianliang_s) as dianliang_s,sum(qiliang_s) as qiliang_s from rpt_ri where riqi='" + first_day + "'");
                    if (ds2.Tables[0].Rows.Count > 0)
                    {
                        DataRow dr2 = ds2.Tables[0].Rows[0];
                        dianliang_s = Utility.ToDecimal(dr2["dianliang_s"]);
                        qiliang_s = Utility.ToDecimal(dr2["qiliang_s"]);
                    }

                    DataSet ds3 = MySqlHelper.Get_DataSet("select sum(dianliang_e) as dianliang_e,sum(qiliang_e) as qiliang_e from tongji_d_qidianbi where shijian='" + dt.ToString("yyyy-MM-dd") + "'");
                    if (ds3.Tables[0].Rows.Count > 0)
                    {
                        DataRow dr3 = ds3.Tables[0].Rows[0];
                        dianliang_e = Utility.ToDecimal(dr3["dianliang_e"]);
                        qiliang_e = Utility.ToDecimal(dr3["qiliang_e"]);
                    }
                    else
                    {
                        return;
                    }

                    DataSet ds4 = MySqlHelper.Get_DataSet("select sum(dianliang) as dianliang,sum(qiliang) as qiliang from rpt_ri where riqi>='" + dt.ToString("yyyy-01-01") + "' and riqi<='" + dt.ToString("yyyy-MM-dd") + "'");
                    if (ds4.Tables[0].Rows.Count > 0)
                    {
                        DataRow dr4 = ds4.Tables[0].Rows[0];
                        dianliang = Utility.ToDecimal(dr4["dianliang"]);
                        qiliang = Utility.ToDecimal(dr4["qiliang"]);
                    }
                    else
                    {
                        return;
                    }
                    if (qiliang <= 0) { throw new Exception("气电比数据统计错误"); }
                    qidianbi = dianliang / qiliang;


                    jienenglv = (hetong_qidianbi - qidianbi) / hetong_qidianbi;
                    jieyue_dianliang = qiliang * (hetong_qidianbi - qidianbi);
                    jieyue_dianfei = jieyue_dianliang * hetong_dianjia;

                    //   int shebei_num = Utility.ToInt(MySqlHelper.Get_sigle("select count(*) from base_shebei where (shebei_leixing_id='lixinji' or shebei_leixing_id='luoganji')"));


                    string sqlstr = "insert into rpt_nian values(0,'" + xiangmu_id + "','" + riqi + "','" + riqi_se + "'," + shebei_num + "," + hetong_dianjia + "," + hetong_qidianbi + "," + dianliang_s + "," + dianliang_e + "," + dianliang + "," + qiliang_s + "," + qiliang_e + "," + qiliang + "," + qidianbi + "," + jienenglv + "," + jieyue_dianliang + "," + jieyue_dianfei + ")";
                    MySqlHelper.ExecuteSql(sqlstr);

                }
                else
                {
                    int j = Utility.ToInt(MySqlHelper.Get_sigle("select count(*) from rpt_nian where riqi='" + riqi + "' and riqi_se='" + riqi_se + "'"));
                    if (j <= 0)
                    {
                        //如果没有当天的统计
                        decimal dianliang_s = 0;
                        decimal dianliang_e = 0;
                        decimal dianliang = 0;
                        decimal qiliang_s = 0;
                        decimal qiliang_e = 0;
                        decimal qiliang = 0;

                        decimal qidianbi = 0;
                        decimal jienenglv = 0;
                        decimal jieyue_dianliang = 0;
                        decimal jieyue_dianfei = 0;


                        string first_day = Utility.ToObjectString(MySqlHelper.Get_sigle("select riqi from rpt_ri where riqi>='" + dt.ToString("yyyy-01-01") + "' and riqi<='" + dt.ToString("yyyy-MM-dd") + "' order by riqi limit 1"));
                        DataSet ds2 = MySqlHelper.Get_DataSet("select sum(dianliang_s) as dianliang_s,sum(qiliang_s) as qiliang_s from tongji_d_qidianbi where shijian='" + first_day + "'");
                        if (ds2.Tables[0].Rows.Count > 0)
                        {
                            DataRow dr2 = ds2.Tables[0].Rows[0];
                            dianliang_s = Utility.ToDecimal(dr2["dianliang_s"]);
                            qiliang_s = Utility.ToDecimal(dr2["qiliang_s"]);
                        }

                        DataSet ds3 = MySqlHelper.Get_DataSet("select sum(dianliang_e) as dianliang_e,sum(qiliang_e) as qiliang_e from tongji_d_qidianbi where shijian='" + dt.ToString("yyyy-MM-dd") + "'");
                        if (ds3.Tables[0].Rows.Count > 0)
                        {
                            DataRow dr3 = ds3.Tables[0].Rows[0];
                            dianliang_e = Utility.ToDecimal(dr3["dianliang_e"]);
                            qiliang_e = Utility.ToDecimal(dr3["qiliang_e"]);
                        }
                        else
                        {
                            return;
                        }

                        DataSet ds4 = MySqlHelper.Get_DataSet("select sum(dianliang) as dianliang,sum(qiliang) as qiliang from rpt_ri where riqi>='" + dt.ToString("yyyy-01-01") + "' and riqi<='" + dt.ToString("yyyy-MM-dd") + "'");
                        if (ds4.Tables[0].Rows.Count > 0)
                        {
                            DataRow dr4 = ds4.Tables[0].Rows[0];
                            dianliang = Utility.ToDecimal(dr4["dianliang"]);
                            qiliang = Utility.ToDecimal(dr4["qiliang"]);
                        }
                        else
                        {
                            return;
                        }
                        if (qiliang <= 0) { throw new Exception("气电比数据统计错误"); }
                        qidianbi = dianliang / qiliang;


                        jienenglv = (hetong_qidianbi - qidianbi) / hetong_qidianbi;
                        jieyue_dianliang = qiliang * (hetong_qidianbi - qidianbi);
                        jieyue_dianfei = jieyue_dianliang * hetong_dianjia;

                        //  int shebei_num = Utility.ToInt(MySqlHelper.Get_sigle("select count(*) from base_shebei where (shebei_leixing_id='lixinji' or shebei_leixing_id='luoganji')"));


                        string sqlstr = "update rpt_nian set xiangmu_id='" + xiangmu_id + "',riqi='" + riqi + "',riqi_se='" + riqi_se + "',shebei_num=" + shebei_num + ",dianjia=" + hetong_dianjia + ",qidianbi_old=" + hetong_qidianbi + ",dianliang_s=" + dianliang_s + ",dianliang_e=" + dianliang_e + ",dianliang=" + dianliang + ",qiliang_s=" + qiliang + ",qiliang_e=" + qiliang_e + ",qiliang=" + qiliang + ",qidianbi=" + qidianbi + ",jienenglv=" + jienenglv + ",jieyue_dianliang=" + jieyue_dianliang + ",jieyue_dianfei=" + jieyue_dianfei + " where id=" + i;
                        MySqlHelper.ExecuteSql(sqlstr);

                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #endregion

        #region 站点管理

        #region  获取站点列表

        public static List<en_zhandian> get_zhandian_list()
        {
            List<en_zhandian> ls = new List<en_zhandian>();
            DataSet ds = new DataSet();
            ds = MySqlHelper.Get_DataSet("select * from base_zhandian order by zhandian_id ");
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    en_zhandian en = new en_zhandian
                    {
                        zhandian_id = Utility.ToObjectString(dr["zhandian_id"]),
                        xiangmu_id = Utility.ToObjectString(dr["xiangmu_id"]),
                        zhandian_mingcheng = Utility.ToObjectString(dr["zhandian_mingcheng"]),
                        liandong_flag = Utility.ToInt(dr["liandong_flag"]),
                        zutaitu_url = Utility.ToObjectString(dr["zutaitu_url"]),
                        mubiaoyali = Utility.ToDecimal(dr["mubiaoyali"])
                    };
                    ls.Add(en);
                }
            }
            return ls;
        }

        #endregion

        #region  根据站点ID获取站点信息

        public static en_zhandian get_zhandian_byid(string zhandian_id)
        {
            try
            {
                en_zhandian en = new en_zhandian();
                DataSet ds = new DataSet();
                ds = MySqlHelper.Get_DataSet("select * from base_zhandian where zhandian_id='" + zhandian_id + "'");
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    en.zhandian_id = Utility.ToObjectString(dr["zhandian_id"]);
                    en.xiangmu_id = Utility.ToObjectString(dr["xiangmu_id"]);
                    en.zhandian_mingcheng = Utility.ToObjectString(dr["zhandian_mingcheng"]);
                    en.liandong_flag = Utility.ToInt(dr["liandong_flag"]);
                    en.zutaitu_url = Utility.ToObjectString(dr["zutaitu_url"]);
                    en.mubiaoyali = Utility.ToDecimal(dr["mubiaoyali"]);
                    return en;
                }
                else
                {
                    return null;
                }

            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region  站点新增

        public static string zhandian_add(en_zhandian en)
        {
            try
            {
                //判断站点ID是否存在
                int i = Utility.ToInt(MySqlHelper.Get_sigle("select count(*) from base_zhandian where zhandian_id='" + en.zhandian_id + "'"));
                if (i > 0)
                {
                    return "站点编号已存在";
                }
                else
                {
                    string sqlstr = "insert into base_zhandian values('" + en.zhandian_id + "','" + en.xiangmu_id + "','" + en.zhandian_mingcheng + "'," + en.liandong_flag + ",'" + en.zutaitu_url + "'," + en.mubiaoyali + ")";
                    MySqlHelper.ExecuteSql(sqlstr);
                    return "";
                }

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion

        #region  站点信息更新

        public static string zhandian_upd(en_zhandian en)
        {
            try
            {
                string sqlstr = "update base_zhandian set xiangmu_id='" + en.xiangmu_id + "',liandong_flag=" + en.liandong_flag + ",zutaitu_url='" + en.zutaitu_url + "',mubiaoyali=" + en.mubiaoyali + " where zhandian_id='" + en.zhandian_id + "'";
                MySqlHelper.ExecuteSql(sqlstr);
                return "";

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion

        #region  站点删除

        public static string zhandian_del(string zhandian_id)
        {
            try
            {
                //站点下有设备，则不能删除
                int i = Utility.ToInt(MySqlHelper.Get_sigle("select count(*) from base_shebei where zhandian_id='" + zhandian_id + "'"));
                if (i > 0)
                {
                    return "此站点下有设备，不能删除";
                }

                string sqlstr = "delete from base_zhandian  where zhandian_id='" + zhandian_id + "'";
                MySqlHelper.ExecuteSql(sqlstr);
                return "";

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion

        #endregion

        #region 设备管理

        #region  获取设备列表

        public static List<en_shebei> get_shebei_list(string zhandian_id, string shebei_leixing_id)
        {
            List<en_shebei> ls = new List<en_shebei>();
            DataSet ds = new DataSet();
            string sqlstr = "select * from uv_shebei_leixing where 1=1 ";
            if (zhandian_id != "")
            {
                sqlstr += " and zhandian_id='" + zhandian_id + "'";
            }
            if (shebei_leixing_id != "")
            {
                sqlstr += " and shebei_leixing_id='" + shebei_leixing_id + "'";
            }
            sqlstr += " order by paixuhao,paixu_num,shebei_mingcheng ";

            ds = MySqlHelper.Get_DataSet(sqlstr);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    en_shebei en = new en_shebei
                    {
                        shebei_id = Utility.ToInt(dr["shebei_id"]),
                        shebei_mingcheng = Utility.ToObjectString(dr["shebei_mingcheng"]),
                        f_shebei_id = Utility.ToInt(dr["f_shebei_id"]),
                        shebei_leixing_id = Utility.ToObjectString(dr["shebei_leixing_id"]),
                        zhandian_id = Utility.ToObjectString(dr["zhandian_id"]),
                        xiangmu_id = Utility.ToObjectString(dr["xiangmu_id"]),
                        shebei_pinpai = Utility.ToObjectString(dr["shebei_pinpai"]),
                        shebei_xianghao = Utility.ToObjectString(dr["shebei_xianghao"]),
                        shebei_miaoshu = Utility.ToObjectString(dr["shebei_miaoshu"]),
                        shebei_gongyingshang = Utility.ToObjectString(dr["shebei_gongyingshang"]),
                        if_zongguanyali = Utility.ToInt(dr["if_zongguanyali"]),
                        if_zongguanliuliang = Utility.ToInt(dr["if_zongguanliuliang"]),
                        if_nenghaojisuan = Utility.ToInt(dr["if_nenghaojisuan"]),
                        qiting_flag = Utility.ToInt(dr["qiting_flag"]),
                        lianjie_flag = Utility.ToInt(dr["lianjie_flag"]),
                        diubao_flag = Utility.ToInt(dr["diubao_flag"]),
                        qiyong_flag = Utility.ToInt(dr["qiyong_flag"]),
                        yali_min = Utility.ToDecimal(dr["yali_min"]),
                        yali_max = Utility.ToDecimal(dr["yali_max"]),
                        paixu_num = Utility.ToInt(dr["paixu_num"]),
                        if_caiji = Utility.ToInt(dr["if_caiji"]),
                        caiji_canshu_str = Utility.ToObjectString(dr["caiji_canshu_str"])
                    };
                    ls.Add(en);
                }
            }
            return ls;
        }

        #endregion

        #region  根据设备ID获取设备信息

        public static en_shebei get_shebei_byid(int shebei_id)
        {
            try
            {
                en_shebei en = new en_shebei();
                DataSet ds = new DataSet();
                ds = MySqlHelper.Get_DataSet("select * from base_shebei where shebei_id=" + shebei_id);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    en.shebei_id = Utility.ToInt(dr["shebei_id"]);
                    en.shebei_mingcheng = Utility.ToObjectString(dr["shebei_mingcheng"]);
                    en.f_shebei_id = Utility.ToInt(dr["f_shebei_id"]);
                    en.shebei_leixing_id = Utility.ToObjectString(dr["shebei_leixing_id"]);
                    en.zhandian_id = Utility.ToObjectString(dr["zhandian_id"]);
                    en.xiangmu_id = Utility.ToObjectString(dr["xiangmu_id"]);
                    en.shebei_pinpai = Utility.ToObjectString(dr["shebei_pinpai"]);
                    en.shebei_xianghao = Utility.ToObjectString(dr["shebei_xianghao"]);
                    en.shebei_miaoshu = Utility.ToObjectString(dr["shebei_miaoshu"]);
                    en.shebei_gongyingshang = Utility.ToObjectString(dr["shebei_gongyingshang"]);
                    en.if_zongguanyali = Utility.ToInt(dr["if_zongguanyali"]);
                    en.if_zongguanliuliang = Utility.ToInt(dr["if_zongguanliuliang"]);
                    en.if_nenghaojisuan = Utility.ToInt(dr["if_nenghaojisuan"]);
                    en.qiting_flag = Utility.ToInt(dr["qiting_flag"]);
                    en.lianjie_flag = Utility.ToInt(dr["lianjie_flag"]);
                    en.diubao_flag = Utility.ToInt(dr["diubao_flag"]);
                    en.qiyong_flag = Utility.ToInt(dr["qiyong_flag"]);
                    en.yali_min = Utility.ToDecimal(dr["yali_min"]);
                    en.yali_max = Utility.ToDecimal(dr["yali_max"]);
                    en.paixu_num = Utility.ToInt(dr["paixu_num"]);
                    en.if_caiji = Utility.ToInt(dr["if_caiji"]);
                    en.caiji_canshu_str = Utility.ToObjectString(dr["caiji_canshu_str"]);
                    return en;
                }
                else
                {
                    return null;
                }

            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region  设备新增

        public static string shebei_add(en_shebei en)
        {
            try
            {
                // int shebei_id = Utility.ToInt(MySqlHelper.Get_sigle("select max(shebei_id) from base_shebei ")) + 1;
                string str = "insert into base_shebei values(";
                str += "0,";
                str += "'" + en.shebei_mingcheng + "',";
                str += "'" + en.f_shebei_id + "',";
                str += "'" + en.shebei_leixing_id + "',";
                str += "'" + en.zhandian_id + "',";
                str += "'" + en.xiangmu_id + "',";
                str += "'" + en.shebei_pinpai + "',";
                str += "'" + en.shebei_xianghao + "',";
                str += "'" + en.shebei_miaoshu + "',";
                str += "'" + en.shebei_gongyingshang + "',";
                str += en.if_zongguanyali + ",";
                str += en.if_zongguanliuliang + ",";
                str += en.if_nenghaojisuan + ",";
                str += "0,0,0,";
                str += en.qiyong_flag + ",";
                str += en.yali_min + ",";
                str += en.yali_max + ",";
                str += en.paixu_num + ",";
                str += en.if_caiji + ",";
                str += "'" + en.caiji_canshu_str + "','',''," + en.zhucanshu_num + "," + en.zhucanshu_zong + "," + en.fucanshu_num + "," + en.fucanshu_zong + ")";

                MySqlHelper.ExecuteSql(str);
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion

        #region  设备更新

        public static string shebei_upd(en_shebei en)
        {
            try
            {
                string sqlstr = "update base_shebei set ";
                sqlstr += "shebei_mingcheng='" + en.shebei_mingcheng + "',";
                sqlstr += "f_shebei_id=" + en.f_shebei_id + ",";
                sqlstr += "shebei_leixing_id='" + en.shebei_leixing_id + "',";
                sqlstr += "zhandian_id='" + en.zhandian_id + "',";
                sqlstr += "xiangmu_id='" + en.xiangmu_id + "',";
                sqlstr += "shebei_pinpai='" + en.shebei_pinpai + "',";
                sqlstr += "shebei_xianghao='" + en.shebei_xianghao + "',";
                sqlstr += "shebei_miaoshu='" + en.shebei_miaoshu + "',";
                sqlstr += "shebei_gongyingshang='" + en.shebei_gongyingshang + "',";
                sqlstr += "if_zongguanyali=" + en.if_zongguanyali + ",";
                sqlstr += "if_zongguanliuliang=" + en.if_zongguanliuliang + ",";
                sqlstr += "if_nenghaojisuan=" + en.if_nenghaojisuan + ",";
                sqlstr += "qiyong_flag=" + en.qiyong_flag + ",";
                sqlstr += "yali_min=" + en.yali_min + ",";
                sqlstr += "yali_max=" + en.yali_max + ",";
                sqlstr += "paixu_num=" + en.paixu_num + ",";
                sqlstr += "if_caiji=" + en.if_caiji + ",";
                sqlstr += "caiji_canshu_str='" + en.caiji_canshu_str + "',";
                sqlstr += "zhucanshu_num=" + en.zhucanshu_num + ",";
                sqlstr += "zhucanshu_zong=" + en.zhucanshu_zong + ",";
                sqlstr += "fucanshu_num=" + en.fucanshu_num + ",";
                sqlstr += "fucanshu_zong=" + en.fucanshu_zong + "";
                sqlstr += " where shebei_id=" + en.shebei_id;

                MySqlHelper.ExecuteSql(sqlstr);
                return "";

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion

        #region  设备删除

        public static string shebei_del(int shebei_id)
        {
            try
            {
                //判断设备是否已经对应模块
                int i = Utility.ToInt("select count(*) from tx_mokuai_shebei where shebei_id=" + shebei_id);
                if (i > 0)
                {
                    return "设备已配置好通讯模块，不能删除";
                }
                else
                {
                    string sqlstr = "delete from base_shebei  where shebei_id=" + shebei_id;
                    MySqlHelper.ExecuteSql(sqlstr);
                    return "";
                }

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion

        #endregion

        #region 通讯模块管理

        #region  获取通讯模块列表

        public static List<en_txmk> get_txmk_list()
        {
            List<en_txmk> ls = new List<en_txmk>();
            DataSet ds = new DataSet();
            ds = MySqlHelper.Get_DataSet("select * from tx_mokuai order by txmk_id ");
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    en_txmk en = new en_txmk
                    {
                        txmk_id = Utility.ToInt(dr["txmk_id"]),
                        txmk_mingcheng = Utility.ToObjectString(dr["txmk_mingcheng"]),
                        tx_xieyi = Utility.ToObjectString(dr["tx_xieyi"]),
                        tx_xieyi_connstr = Utility.ToObjectString(dr["tx_xieyi_connstr"]),
                        txmk_flag = Utility.ToInt(dr["txmk_flag"]),
                        qiyong_flag = Utility.ToInt(dr["qiyong_flag"])
                    };
                    ls.Add(en);
                }
            }
            return ls;
        }

        #endregion

        #region  根据通讯模块ID获取通讯模块信息

        public static en_txmk get_txmk_byid(int txmk_id)
        {
            try
            {
                en_txmk en = new en_txmk();
                DataSet ds = new DataSet();
                ds = MySqlHelper.Get_DataSet("select * from tx_mokuai where txmk_id=" + txmk_id);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    en.txmk_id = Utility.ToInt(dr["txmk_id"]);
                    en.txmk_mingcheng = Utility.ToObjectString(dr["txmk_mingcheng"]);
                    en.tx_xieyi = Utility.ToObjectString(dr["tx_xieyi"]);
                    en.tx_xieyi_connstr = Utility.ToObjectString(dr["tx_xieyi_connstr"]);
                    en.txmk_flag = Utility.ToInt(dr["txmk_flag"]);
                    en.qiyong_flag = Utility.ToInt(dr["qiyong_flag"]);
                    return en;
                }
                else
                {
                    return null;
                }

            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region  通讯模块新增

        public static string txmk_add(en_txmk en)
        {
            try
            {
                int txmk_id = Utility.ToInt(MySqlHelper.Get_sigle("select max(txmk_id) from tx_mokuai ")) + 1;
                string sqlstr = "insert into tx_mokuai values('" + txmk_id + "','" + en.txmk_mingcheng + "','" + en.tx_xieyi + "','" + en.tx_xieyi_connstr + "',0,''," + en.qiyong_flag + ")";
                MySqlHelper.ExecuteSql(sqlstr);
                return "";

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion

        #region  通讯模块更新

        public static string txmk_upd(en_txmk en)
        {
            try
            {
                string sqlstr = "update tx_mokuai set txmk_mingcheng='" + en.txmk_mingcheng + "',tx_xieyi='" + en.tx_xieyi + "',tx_xieyi_connstr='" + en.tx_xieyi_connstr + "',qiyong_flag=" + en.qiyong_flag + " where txmk_id=" + en.txmk_id;
                MySqlHelper.ExecuteSql(sqlstr);
                return "";

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion

        #region  通讯模块删除

        public static string txmk_del(int txmk_id)
        {
            try
            {
                //通讯模块下已设置地址对应参数，不能删除
                int j = Utility.ToInt(MySqlHelper.Get_sigle("select count(*) from tx_mokuai_field where txmk_id=" + txmk_id));
                if (j > 0)
                {
                    return "此通讯模块下已设置地址对应参数，不能删除";
                }

                string sqlstr = "delete from tx_mokuai  where txmk_id=" + txmk_id;
                MySqlHelper.ExecuteSql(sqlstr);
                return "";

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion

        #endregion

        #region 通讯模块对应字段相关


        #region  获取模块对应字段列表

        public static List<uv_tx_mokuai_field> get_tx_mokuai_field_list(int txmk_id, int shebei_id)
        {
            List<uv_tx_mokuai_field> ls = new List<uv_tx_mokuai_field>();
            DataSet ds = new DataSet();
            string sqlstr = "select * from uv_tx_mokuai_field where 1=1 ";
            if (txmk_id > 0)
            {
                sqlstr += " and txmk_id=" + txmk_id + "";
            }
            if (shebei_id > 0)
            {
                sqlstr += " and shebei_id=" + shebei_id + "";
            }
            sqlstr += " order by txmk_id,shebei_id ";

            ds = MySqlHelper.Get_DataSet(sqlstr);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    uv_tx_mokuai_field en = new uv_tx_mokuai_field
                    {
                        field_id = Utility.ToInt(dr["field_id"]),
                        txmk_id = Utility.ToInt(dr["txmk_id"]),
                        shebei_id = Utility.ToInt(dr["shebei_id"]),
                        dizhi = Utility.ToObjectString(dr["dizhi"]),
                        zhucanshu = Utility.ToObjectString(dr["zhucanshu"]),
                        fucanshu = Utility.ToObjectString(dr["fucanshu"]),
                        if_xieru = Utility.ToInt(dr["if_xieru"]),
                        shebei_mingcheng = Utility.ToObjectString(dr["shebei_mingcheng"]),
                        txmk_mingcheng = Utility.ToObjectString(dr["txmk_mingcheng"]),
                        zhandian_mingcheng = Utility.ToObjectString(dr["zhandian_mingcheng"]),
                        shebei_leixing_mingcheng = Utility.ToObjectString(dr["shebei_leixing_mingcheng"]),
                        fucanshu_peizhi_field = Utility.ToObjectString(dr["caiji_canshu_str"]),
                        zhucanshu_miaoshu = Utility.ToObjectString(dr["zhucanshu_miaoshu"]),
                        fucanshu_miaoshu = Utility.ToObjectString(dr["fucanshu_miaoshu"]),
                        value_up = Utility.ToDecimal(dr["value_up"]),
                        value_down = Utility.ToDecimal(dr["value_down"])
                    };
                    ls.Add(en);
                }
            }
            return ls;
        }

        #endregion

        #region  根据对应字段ID获取模块对应字段信息

        public static uv_tx_mokuai_field get_tx_mokuai_field_byid(int field_id)
        {
            try
            {
                uv_tx_mokuai_field en = new uv_tx_mokuai_field();
                DataSet ds = new DataSet();
                ds = MySqlHelper.Get_DataSet("select * from uv_tx_mokuai_field where field_id=" + field_id);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    en.field_id = Utility.ToInt(dr["field_id"]);
                    en.txmk_id = Utility.ToInt(dr["txmk_id"]);
                    en.shebei_id = Utility.ToInt(dr["shebei_id"]);
                    en.dizhi = Utility.ToObjectString(dr["dizhi"]);
                    en.zhucanshu = Utility.ToObjectString(dr["zhucanshu"]);
                    en.fucanshu = Utility.ToObjectString(dr["fucanshu"]);
                    en.if_xieru = Utility.ToInt(dr["if_xieru"]);
                    en.shebei_mingcheng = Utility.ToObjectString(dr["shebei_mingcheng"]);
                    en.txmk_mingcheng = Utility.ToObjectString(dr["txmk_mingcheng"]);
                    en.zhandian_mingcheng = Utility.ToObjectString(dr["zhandian_mingcheng"]);
                    en.shebei_leixing_mingcheng = Utility.ToObjectString(dr["shebei_leixing_mingcheng"]);
                    en.fucanshu_peizhi_field = Utility.ToObjectString(dr["caiji_canshu_str"]);
                    en.zhucanshu_miaoshu = Utility.ToObjectString(dr["zhucanshu_miaoshu"]);
                    en.fucanshu_miaoshu = Utility.ToObjectString(dr["fucanshu_miaoshu"]);
                    en.value_up = Utility.ToDecimal(dr["value_up"]);
                    en.value_down = Utility.ToDecimal(dr["value_down"]);
                    return en;
                }
                else
                {
                    return null;
                }

            }
            catch
            {
                return null;
            }
        }

        public static en_tx_mokuai_field get_tx_mokuai_field_byid_en(int field_id)
        {
            try
            {
                en_tx_mokuai_field en = new en_tx_mokuai_field();
                DataSet ds = new DataSet();
                ds = MySqlHelper.Get_DataSet("select * from tx_mokuai_field where field_id=" + field_id);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    en.field_id = Utility.ToInt(dr["field_id"]);
                    en.txmk_id = Utility.ToInt(dr["txmk_id"]);
                    en.shebei_id = Utility.ToInt(dr["shebei_id"]);
                    en.dizhi = Utility.ToObjectString(dr["dizhi"]);
                    en.zhucanshu = Utility.ToObjectString(dr["zhucanshu"]);
                    en.fucanshu = Utility.ToObjectString(dr["fucanshu"]);
                    en.if_xieru = Utility.ToInt(dr["if_xieru"]);
                    en.zhucanshu_miaoshu = Utility.ToObjectString(dr["zhucanshu_miaoshu"]);
                    en.fucanshu_miaoshu = Utility.ToObjectString(dr["fucanshu_miaoshu"]);
                    en.value_up = Utility.ToDecimal(dr["value_up"]);
                    en.value_down = Utility.ToDecimal(dr["value_down"]);
                    return en;
                }
                else
                {
                    return null;
                }

            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region  模块对应字段新增

        public static string tx_mokuai_field_add(en_tx_mokuai_field en)
        {
            try
            {
                string str = "insert into tx_mokuai_field values(0,";
                str += "" + en.txmk_id + ",";
                str += "" + en.shebei_id + ",";
                str += "'" + en.dizhi + "',";
                str += "'" + en.zhucanshu + "',";
                str += "'" + en.fucanshu + "',";
                str += "" + en.if_xieru + ",'" + en.zhucanshu_miaoshu + "','" + en.fucanshu_miaoshu + "'," + en.value_up + "," + en.value_down + ")";

                MySqlHelper.ExecuteSql(str);
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion

        #region  模块对应字段更新

        public static string tx_mokuai_field_upd(en_tx_mokuai_field en)
        {
            try
            {
                string sqlstr = "update tx_mokuai_field set ";
                sqlstr += "txmk_id=" + en.txmk_id + ",";
                sqlstr += "shebei_id=" + en.shebei_id + ",";
                sqlstr += "dizhi='" + en.dizhi + "',";
                sqlstr += "zhucanshu='" + en.zhucanshu + "',";
                sqlstr += "fucanshu='" + en.fucanshu + "',";
                sqlstr += "if_xieru=" + en.if_xieru + ",";
                sqlstr += "zhucanshu_miaoshu='" + en.zhucanshu_miaoshu + "',";
                sqlstr += "fucanshu_miaoshu='" + en.fucanshu_miaoshu + "',";
                sqlstr += "value_up=" + en.value_up + ",";
                sqlstr += "value_down=" + en.value_down + "";
                sqlstr += " where field_id=" + en.field_id;

                MySqlHelper.ExecuteSql(sqlstr);
                return "";

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion

        #region  通讯模块对应字段删除

        public static string tx_mokuai_field_del(int field_id)
        {
            try
            {
                string sqlstr = "delete from tx_mokuai_field  where field_id=" + field_id;
                MySqlHelper.ExecuteSql(sqlstr);
                return "";

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion

        #endregion

        #region 获取设备主参数数量

        public static int get_shebei_zhucanshu_zong(string shebei_leixing_id)
        {
            // string shebei_leixing_id = Utility.ToObjectString(MySqlHelper.Get_sigle("select shebei_leixing_id from base_shebei where shebei_id="+shebei_id));
            return Utility.ToInt(MySqlHelper.Get_sigle("select count(*) from base_shebei_leixing_canshu where shebei_leixing_id='" + shebei_leixing_id + "'"));
        }

        #endregion


    }
}
