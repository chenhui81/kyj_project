using kyj_project.Common;
using kyj_project.DAL;
using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kyj_project
{
    public partial class Form1 : Form
    {
        private DataTable dt_base;
        private DataTable dt_mokuai;
        private int timer_flag = 0;

        private System.Threading.Timer _timer;

        public Form1()
        {
            InitializeComponent();

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

                #endregion

                if (timer_flag == 1)
                {
                    //每分钟定时
                    if (DateTime.Now.Second == 0)
                    {
                        this.caiji_dingshi();
                    }

                    //循环
                    if (DateTime.Now.Second == 10)
                    {
                        string s1 = this.caiji_xunhuan();
                        MySqlHelper.ExecuteSql("insert into caiji_zt values(0,2,0,'" + s1 + "','');");
                    }

                    if (DateTime.Now.Second == 20)
                    {
                        string s2 = this.caiji_xunhuan();
                        MySqlHelper.ExecuteSql("insert into caiji_zt values(0,3,0,'" + s2 + "','');");
                    }


                    if (DateTime.Now.Second == 30)
                    {
                        string s3 = this.caiji_xunhuan();
                        MySqlHelper.ExecuteSql("insert into caiji_zt values(0,4,0,'" + s3 + "','');");
                    }


                    if (DateTime.Now.Second == 40)
                    {
                        string s4 = this.caiji_xunhuan();
                        MySqlHelper.ExecuteSql("insert into caiji_zt values(0,5,0,'" + s4 + "','');");
                    }
                }
            });
        }



        /// <summary>
        /// 插入分钟级气电比测试数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            string zhandian_id = "1d";
            DateTime dt_s = Utility.ToDateTime("2024-11-01 00:00:00");
            int dianliang_s = 1000;
            int qiliang_s = 1000;

            StringBuilder sb = new StringBuilder();

            for (int j = 0; j < 100; j++)
            {
                //循环天
                DateTime dt_day = dt_s.AddDays(j);
                if (dt_day > DateTime.Today) { break; }

                for (int i = 0; i < 1500; i++)
                {
                    //循环分钟
                    DateTime dt = dt_day.AddMinutes(i);
                    if (dt >= dt_day.AddDays(1)) { continue; }

                    Random random = new Random(Random_cls.GetRandomSeed());
                    int dianliang_rd = random.Next(50, 200);
                    int qiliang_rd = random.Next(100, 500);
                    decimal yali = random.Next(3, 10);

                    int dianliang_e = dianliang_s + dianliang_rd;
                    int qiliang_e = qiliang_s + qiliang_rd;

                    string sqlstr = biz_cls.caiji_m(zhandian_id, dt.ToString("yyyy-MM-dd HH:mm"), dianliang_s, dianliang_e, qiliang_s, qiliang_e, yali);
                    sb.Append(sqlstr);

                    dianliang_s = dianliang_s + dianliang_rd;
                    qiliang_s = qiliang_s + qiliang_rd;
                }

                MySqlHelper.ExecuteSql(sb.ToString());
                sb.Clear();
            }



            MessageBox.Show("完成！");

        }

        private void button2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 1000000; i++)
            {
                DateTime dt = Utility.ToDateTime("2024-11-01 00:00").AddMinutes(i);
                if (dt >= DateTime.Today.AddDays(1)) { break; }
                biz_cls.caiji_qidianbi_auto("1d", dt);
            }
            MessageBox.Show("完成！");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //获取站点模块列表
            DataSet ds1 = new DataSet();
            ds1 = MySqlHelper.Get_DataSet("select * from tx_mokuai order by txmk_id");
            dt_mokuai = new DataTable();
            dt_mokuai = ds1.Tables[0];

            //获取地址参数对应字段table，uv_tx_mokuai_field相关数据
            DataSet ds = new DataSet();
            ds = MySqlHelper.Get_DataSet("select field_id,shebei_id,txmk_id,zhandian_id,shebei_leixing_id,zhucanshu,fucanshu,dizhi,shebei_mingcheng,value_up,value_down from uv_tx_mokuai_field");
            dt_base = new DataTable();
            dt_base = ds.Tables[0];

            MessageBox.Show("完成");

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (timer_flag == 0)
            {
                timer_flag = 1;
                this.label1.Text = "模拟采集_开始状态";
            }
            else
            {
                timer_flag = 0;
                this.label1.Text = "模拟采集_停止状态";
            }



        }





        #region 每分钟定时采集1次

        private void caiji_dingshi()
        {
            try
            {
                string shijian = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                StringBuilder sb = new StringBuilder();

                foreach (DataRow dr in dt_mokuai.Rows)
                {
                    #region 1、通讯模块连接

                    #endregion


                    #region //2、每分钟开始采集一次，用于计算每分钟气电比数据

                    //电表
                    DataRow[] drs1 = dt_base.Select(" txmk_id=" + Utility.ToInt(dr["txmk_id"]) + " and shebei_leixing_id='dianbiao' and zhucanshu='dianliang'");
                    foreach (DataRow dr1 in drs1)
                    {
                        int field_id = Utility.ToInt(dr1["field_id"]);
                        int shebei_id = Utility.ToInt(dr1["shebei_id"]);
                        string shebei_leixing_id = Utility.ToObjectString(dr1["shebei_leixing_id"]);
                        string zhucanshu = Utility.ToObjectString(dr1["zhucanshu"]);

                        Random random = new Random(Random_cls.GetRandomSeed());
                        decimal zhi_old = Utility.ToDecimal(MySqlHelper.Get_sigle("select shuju from uv_caiji_base where field_id=" + field_id + " and LENGTH(shijian)=16 order by shijian desc limit 1"));
                        decimal zhi = zhi_old + Utility.ToDecimal(random.Next(200, 500));
                        sb.Append("insert into caiji_base values ('" + Guid.NewGuid().ToString() + "'," + field_id + "," + zhi + ",'" + shijian + "'," + shebei_id + ",'" + shebei_leixing_id + "','" + zhucanshu + "');");
                    }

                    //流量计
                    DataRow[] drs2 = dt_base.Select(" txmk_id = " + Utility.ToInt(dr["txmk_id"]) + " and  shebei_leixing_id ='liuliangji' and zhucanshu='qiliang'");
                    foreach (DataRow dr2 in drs2)
                    {
                        int field_id = Utility.ToInt(dr2["field_id"]);
                        int shebei_id = Utility.ToInt(dr2["shebei_id"]);
                        string shebei_leixing_id = Utility.ToObjectString(dr2["shebei_leixing_id"]);
                        string zhucanshu = Utility.ToObjectString(dr2["zhucanshu"]);
                        Random random = new Random(Random_cls.GetRandomSeed());
                        decimal zhi_old = Utility.ToDecimal(MySqlHelper.Get_sigle("select shuju from uv_caiji_base where field_id=" + field_id + " and LENGTH(shijian)=16 order by shijian desc limit 1"));
                        decimal zhi = zhi_old + Utility.ToDecimal(random.Next(500, 1000));
                        sb.Append("insert into caiji_base values ('" + Guid.NewGuid().ToString() + "'," + field_id + "," + zhi + ",'" + shijian + "'," + shebei_id + ",'" + shebei_leixing_id + "','" + zhucanshu + "');");
                    }

                    //压力传感器
                    DataRow[] drs3 = dt_base.Select(" txmk_id=" + Utility.ToInt(dr["txmk_id"]) + " and  shebei_leixing_id='yalichuanganqi' and zhucanshu='yali'");
                    foreach (DataRow dr3 in drs3)
                    {
                        int field_id = Utility.ToInt(dr3["field_id"]);
                        Random random = new Random(Random_cls.GetRandomSeed());
                        decimal zhi = Utility.ToDecimal(random.Next(3, 8));
                        int shebei_id = Utility.ToInt(dr3["shebei_id"]);
                        string shebei_leixing_id = Utility.ToObjectString(dr3["shebei_leixing_id"]);
                        string zhucanshu = Utility.ToObjectString(dr3["zhucanshu"]);
                        sb.Append("insert into caiji_base values ('" + Guid.NewGuid().ToString() + "'," + field_id + "," + zhi + ",'" + shijian + "'," + shebei_id + ",'" + shebei_leixing_id + "','" + zhucanshu + "');");
                    }

                    //露点仪
                    DataRow[] drs4 = dt_base.Select(" txmk_id=" + Utility.ToInt(dr["txmk_id"]) + " and  shebei_leixing_id='ludianyi' and zhucanshu='ludian'");
                    foreach (DataRow dr4 in drs4)
                    {
                        int field_id = Utility.ToInt(dr4["field_id"]);
                        Random random = new Random(Random_cls.GetRandomSeed());
                        decimal zhi = Utility.ToDecimal(random.Next(20, 50));
                        int shebei_id = Utility.ToInt(dr4["shebei_id"]);
                        string shebei_leixing_id = Utility.ToObjectString(dr4["shebei_leixing_id"]);
                        string zhucanshu = Utility.ToObjectString(dr4["zhucanshu"]);
                        sb.Append("insert into caiji_base values ('" + Guid.NewGuid().ToString() + "'," + field_id + "," + zhi + ",'" + shijian + "'," + shebei_id + ",'" + shebei_leixing_id + "','" + zhucanshu + "');");
                    }

                    //螺杆机压力
                    DataRow[] drs5 = dt_base.Select(" txmk_id=" + Utility.ToInt(dr["txmk_id"]) + " and  shebei_leixing_id='luoganji' and zhucanshu='yali'");
                    foreach (DataRow dr5 in drs5)
                    {
                        int field_id = Utility.ToInt(dr5["field_id"]);
                        int shebei_id = Utility.ToInt(dr5["shebei_id"]);
                        string shebei_leixing_id = Utility.ToObjectString(dr5["shebei_leixing_id"]);
                        string zhucanshu = Utility.ToObjectString(dr5["zhucanshu"]);
                        Random random = new Random(Random_cls.GetRandomSeed());
                        decimal zhi = Utility.ToDecimal(random.Next(3, 6));
                        sb.Append("insert into caiji_base values ('" + Guid.NewGuid().ToString() + "'," + field_id + "," + zhi + ",'" + shijian + "'," + shebei_id + ",'" + shebei_leixing_id + "','" + zhucanshu + "');");
                    }

                    //离心机压力
                    DataRow[] drs6 = dt_base.Select(" txmk_id=" + Utility.ToInt(dr["txmk_id"]) + " and  shebei_leixing_id='lixinji' and zhucanshu='yali'");
                    foreach (DataRow dr6 in drs6)
                    {
                        int field_id = Utility.ToInt(dr6["field_id"]);
                        int shebei_id = Utility.ToInt(dr6["shebei_id"]);
                        string shebei_leixing_id = Utility.ToObjectString(dr6["shebei_leixing_id"]);
                        string zhucanshu = Utility.ToObjectString(dr6["zhucanshu"]);
                        Random random = new Random(Random_cls.GetRandomSeed());
                        decimal zhi = Utility.ToDecimal(random.Next(4, 10));
                        sb.Append("insert into caiji_base values ('" + Guid.NewGuid().ToString() + "'," + field_id + "," + zhi + ",'" + shijian + "'," + shebei_id + ",'" + shebei_leixing_id + "','" + zhucanshu + "');");
                    }

                    //离心机进气、排气比例
                    DataRow[] drs7 = dt_base.Select(" txmk_id=" + Utility.ToInt(dr["txmk_id"]) + " and  shebei_leixing_id='lixinji' and (zhucanshu='jinqi' or zhucanshu='paiqi')");
                    foreach (DataRow dr7 in drs7)
                    {
                        int field_id = Utility.ToInt(dr7["field_id"]);
                        int shebei_id = Utility.ToInt(dr7["shebei_id"]);
                        string shebei_leixing_id = Utility.ToObjectString(dr7["shebei_leixing_id"]);
                        string zhucanshu = Utility.ToObjectString(dr7["zhucanshu"]);
                        Random random = new Random(Random_cls.GetRandomSeed());
                        decimal zhi = Utility.ToDecimal(random.Next(0, 100));
                        sb.Append("insert into caiji_base values ('" + Guid.NewGuid().ToString() + "'," + field_id + "," + zhi + ",'" + shijian + "'," + shebei_id + ",'" + shebei_leixing_id + "','" + zhucanshu + "');");
                    }

                    //冷却机频率
                    DataRow[] drs8 = dt_base.Select(" txmk_id=" + Utility.ToInt(dr["txmk_id"]) + " and  shebei_leixing_id='lengqueji' and zhucanshu='pinlv'");
                    foreach (DataRow dr8 in drs8)
                    {
                        int field_id = Utility.ToInt(dr8["field_id"]);
                        int shebei_id = Utility.ToInt(dr8["shebei_id"]);
                        string shebei_leixing_id = Utility.ToObjectString(dr8["shebei_leixing_id"]);
                        string zhucanshu = Utility.ToObjectString(dr8["zhucanshu"]);
                        Random random = new Random(Random_cls.GetRandomSeed());
                        decimal zhi = Utility.ToDecimal(random.Next(50, 100));
                        sb.Append("insert into caiji_base values ('" + Guid.NewGuid().ToString() + "'," + field_id + "," + zhi + ",'" + shijian + "'," + shebei_id + ",'" + shebei_leixing_id + "','" + zhucanshu + "');");
                    }

                    //冷却机进水温度、出水温度
                    DataRow[] drs9 = dt_base.Select(" txmk_id=" + Utility.ToInt(dr["txmk_id"]) + " and  shebei_leixing_id='lengqueji' and (zhucanshu='jinshuiwendu' or zhucanshu='chushuiwendu')");
                    foreach (DataRow dr9 in drs9)
                    {
                        int field_id = Utility.ToInt(dr9["field_id"]);
                        int shebei_id = Utility.ToInt(dr9["shebei_id"]);
                        string shebei_leixing_id = Utility.ToObjectString(dr9["shebei_leixing_id"]);
                        string zhucanshu = Utility.ToObjectString(dr9["zhucanshu"]);
                        Random random = new Random(Random_cls.GetRandomSeed());
                        decimal zhi = Utility.ToDecimal(random.Next(20, 60));
                        sb.Append("insert into caiji_base values ('" + Guid.NewGuid().ToString() + "'," + field_id + "," + zhi + ",'" + shijian + "'," + shebei_id + ",'" + shebei_leixing_id + "','" + zhucanshu + "');");
                    }

                    #endregion

                    //更新模块联通状态
                    sb.Append("update tx_mokuai set txmk_flag=1,txshijian='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where txmk_id=" + Utility.ToInt(dr["txmk_id"]) + "; ");
                }

                if (sb.ToString().Length > 10)
                {
                    sb.Append("insert into caiji_zt values(0,1,0,'" + shijian + "','');");
                    MySqlHelper.ExecuteSql(sb.ToString());
                }


                biz_cls.write_log("系统日志", "采集系统", "气电比数据，每分钟定时采集1次", "");
            }
            catch (Exception ex)
            {
                biz_cls.write_log("系统日志", "采集系统", ex.ToString(), "");
            }
        }

        #endregion

        #region 每分钟循环采集多次

        private string caiji_xunhuan()
        {
            try
            {
                string shijian = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                StringBuilder sb = new StringBuilder();

                foreach (DataRow dr in dt_mokuai.Rows)
                {
                    #region 1、通讯模块连接

                    #endregion

                    //所有离心机、螺杆机、冷干机、吸干机、冷却机开关量
                    DataRow[] drs1 = dt_base.Select(" txmk_id=" + Utility.ToInt(dr["txmk_id"]) + " and zhucanshu='kaiguan'");
                    foreach (DataRow dr1 in drs1)
                    {
                        int field_id = Utility.ToInt(dr1["field_id"]);
                        int shebei_id = Utility.ToInt(dr1["shebei_id"]);
                        string shebei_leixing_id = Utility.ToObjectString(dr1["shebei_leixing_id"]);
                        string zhucanshu = Utility.ToObjectString(dr1["zhucanshu"]);
                        Random random = new Random(Random_cls.GetRandomSeed());
                        int i = random.Next(1, 1000);
                        decimal zhi = 1;
                        if (i <= 10)
                        {
                            zhi = 0;
                        }

                        sb.Append("insert into caiji_base values ('" + Guid.NewGuid().ToString() + "'," + field_id + "," + zhi + ",'" + shijian + "'," + shebei_id + ",'" + shebei_leixing_id + "','" + zhucanshu + "');");
                    }

                    //所有辅参数
                    DataRow[] drs2 = dt_base.Select(" txmk_id=" + Utility.ToInt(dr["txmk_id"]) + " and zhucanshu<>'kaiguan' and fucanshu<>''");
                    foreach (DataRow dr2 in drs2)
                    {
                        int field_id = Utility.ToInt(dr2["field_id"]);
                        int shebei_id = Utility.ToInt(dr2["shebei_id"]);
                        string shebei_leixing_id = Utility.ToObjectString(dr2["shebei_leixing_id"]);
                        string zhucanshu = Utility.ToObjectString(dr2["zhucanshu"]);
                        Random random = new Random(Random_cls.GetRandomSeed());
                        decimal zhi = Utility.ToDecimal(random.Next(200, 500));
                        sb.Append("insert into caiji_base values ('" + Guid.NewGuid().ToString() + "'," + field_id + "," + zhi + ",'" + shijian + "'," + shebei_id + ",'" + shebei_leixing_id + "','" + zhucanshu + "');");
                    }

                    //更新模块联通状态
                    sb.Append("update tx_mokuai set txmk_flag=1,txshijian='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where txmk_id=" + Utility.ToInt(dr["txmk_id"]) + "; ");
                }

                if (sb.ToString().Length > 10)
                {
                    MySqlHelper.ExecuteSql(sb.ToString());
                }

                biz_cls.write_log("系统日志", "采集系统", "开关量、辅参数，每分钟循环采集多次", "");

                return shijian;
            }
            catch (Exception ex)
            {
                biz_cls.write_log("系统日志", "采集系统", ex.ToString(), "");
                return "";
            }
        }


        #endregion

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                biz_cls.rpt_ri_g(Utility.ToDateTime("2024-11-19"));
                MessageBox.Show("完成");
            }
            catch (Exception ex)
            {
                biz_cls.write_log("系统日志", "采集系统", ex.Message, "");
            }
        }
    }
}
