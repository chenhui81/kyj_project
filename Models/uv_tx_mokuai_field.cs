namespace kyj_project.Models
{
    public class uv_tx_mokuai_field
    {

        public uv_tx_mokuai_field()
        {

        }
        public int field_id { get; set; }//ID
        public int txmk_id { get; set; }//通讯模块ID
        public int shebei_id { get; set; }//设备ID

        public string dizhi { get; set; }//模块地址
        public string zhucanshu { get; set; }//主参数
        public string fucanshu { get; set; }//辅参数

        public int if_xieru { get; set; }//是否写入

        public string zhucanshu_miaoshu { get; set; }//主参数描述
        public string fucanshu_miaoshu { get; set; }//辅参数描述

        public string shebei_mingcheng { get; set; }//设备名称
        public string txmk_mingcheng { get; set; }//通讯模块名称
        public string zhandian_mingcheng { get; set; }//站点名称
        public string shebei_leixing_mingcheng { get; set; }//设备类型名称
        public string fucanshu_peizhi_field { get; set; }//辅参数配置字段

        public decimal value_up { get; set; }//数值上限
        public decimal value_down { get; set; }//数值下限




    }
}
