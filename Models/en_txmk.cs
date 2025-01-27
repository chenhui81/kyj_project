namespace kyj_project.Models
{
    public class en_txmk
    {

        public en_txmk()
        {

        }
        public int txmk_id { get; set; }//通讯模块ID
        public string txmk_mingcheng { get; set; }//通讯模块名称
        public string tx_xieyi { get; set; }//通讯协议
        public string tx_xieyi_connstr { get; set; }//通讯连接字符串
        public int txmk_flag { get; set; }//通讯模块是否联通（1是0否）
        public string txshijian { get; set; }//上次通讯时间

        public int qiyong_flag { get; set; }//是否启用 1是0否
    }
}
