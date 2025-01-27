namespace kyj_project.Models
{
    public class en_shebei
    {

        public en_shebei()
        {

        }
        public int shebei_id { get; set; }//设备ID
        public string shebei_mingcheng { get; set; }//设备名称
        public int f_shebei_id { get; set; }//父级设备ID
        public string shebei_leixing_id { get; set; }//设备类型ID
        public string zhandian_id { get; set; }//站点ID
        public string xiangmu_id { get; set; }//项目ID

        public string shebei_pinpai { get; set; }//设备品牌
        public string shebei_xianghao { get; set; }//设备型号
        public string shebei_miaoshu { get; set; }//设备描述
        public string shebei_gongyingshang { get; set; }//设备供应商
        public int if_zongguanyali { get; set; }//是否总管压力
        public int if_zongguanliuliang { get; set; }//是否总管流量

        public int if_nenghaojisuan { get; set; }//是否参与能耗计算
        public int qiting_flag { get; set; }//起停状态
        public int lianjie_flag { get; set; }//连接状态
        public int diubao_flag { get; set; }//丢包状态
        public int qiyong_flag { get; set; }//启用状态
        public decimal yali_min { get; set; }//空压机最小压力
        public decimal yali_max { get; set; }//空压机最大压力
        public int paixu_num { get; set; }//螺杆机联动排序号

        public int if_caiji { get; set; }//是否是采集设备 1是0否

        public string caiji_canshu_str { get; set; }//采集参数设置

        public int zhucanshu_num { get; set; }//已配置主参数数量
        public int zhucanshu_zong { get; set; }//主参数总数量

        public int fucanshu_num { get; set; }//已配置辅参数数量
        public int fucanshu_zong { get; set; }//辅参数总数量
    }
}
