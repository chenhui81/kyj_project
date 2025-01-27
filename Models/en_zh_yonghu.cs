namespace kyj_project.Models
{
    public class en_zh_yonghu
    {
        public en_zh_yonghu()
        {

        }

        public string yonghu_id { get; set; }//用户ID
        public string yonghu_xingming { get; set; }//用户名
        public string yonghu_bumen { get; set; } //用户部门
        public string yonghu_dianhua { get; set; } //用户电话
        public string login_pwd { get; set; } //登陆密码
        public string juese_id { get; set; } //角色ID

    }
}