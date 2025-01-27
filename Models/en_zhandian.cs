namespace kyj_project.Models
{
    public class en_zhandian
    {

        public en_zhandian()
        {

        }
        public string zhandian_id { get; set; }//站点ID
        public string xiangmu_id { get; set; }//项目ID
        public string zhandian_mingcheng { get; set; }//站点名称

        public int liandong_flag { get; set; }//是否螺杆机联动 1是0否

        public string zutaitu_url { get; set; }//组态图URL
        public decimal mubiaoyali { get; set; }//总管目标压力
    }
}
