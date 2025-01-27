
namespace kyj_project
{
    partial class frm_shebei_edit
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frm_shebei_edit));
            this.label1 = new System.Windows.Forms.Label();
            this.txt_shebei_name = new System.Windows.Forms.TextBox();
            this.comb_zhandian = new System.Windows.Forms.ComboBox();
            this.cb_yali = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.comb_shebei_leixing = new System.Windows.Forms.ComboBox();
            this.txt_shebei_miaoshu = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txt_caijicanshu = new System.Windows.Forms.TextBox();
            this.lb_canshu = new System.Windows.Forms.Label();
            this.cb_liuliang = new System.Windows.Forms.CheckBox();
            this.cb_nenghao = new System.Windows.Forms.CheckBox();
            this.txt_yali_max = new System.Windows.Forms.TextBox();
            this.lb_yali_max = new System.Windows.Forms.Label();
            this.txt_yali_min = new System.Windows.Forms.TextBox();
            this.lb_yali_min = new System.Windows.Forms.Label();
            this.txt_fshebei_id = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btn_save = new System.Windows.Forms.Button();
            this.lb_beizhu = new System.Windows.Forms.Label();
            this.txt_paixu = new System.Windows.Forms.TextBox();
            this.lb_paixuhao = new System.Windows.Forms.Label();
            this.lb_title = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cb_qiyong = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(97, 122);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "设备名称";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // txt_shebei_name
            // 
            this.txt_shebei_name.Location = new System.Drawing.Point(165, 119);
            this.txt_shebei_name.Name = "txt_shebei_name";
            this.txt_shebei_name.Size = new System.Drawing.Size(153, 21);
            this.txt_shebei_name.TabIndex = 1;
            this.txt_shebei_name.TextChanged += new System.EventHandler(this.txt_shebei_name_TextChanged);
            // 
            // comb_zhandian
            // 
            this.comb_zhandian.FormattingEnabled = true;
            this.comb_zhandian.Location = new System.Drawing.Point(435, 81);
            this.comb_zhandian.Name = "comb_zhandian";
            this.comb_zhandian.Size = new System.Drawing.Size(145, 20);
            this.comb_zhandian.TabIndex = 2;
            // 
            // cb_yali
            // 
            this.cb_yali.AutoSize = true;
            this.cb_yali.Location = new System.Drawing.Point(645, 162);
            this.cb_yali.Name = "cb_yali";
            this.cb_yali.Size = new System.Drawing.Size(72, 16);
            this.cb_yali.TabIndex = 3;
            this.cb_yali.Text = "总管压力";
            this.cb_yali.UseVisualStyleBackColor = true;
            this.cb_yali.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(354, 84);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "空压机站点";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(97, 83);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "设备类型";
            // 
            // comb_shebei_leixing
            // 
            this.comb_shebei_leixing.FormattingEnabled = true;
            this.comb_shebei_leixing.Location = new System.Drawing.Point(165, 80);
            this.comb_shebei_leixing.Name = "comb_shebei_leixing";
            this.comb_shebei_leixing.Size = new System.Drawing.Size(153, 20);
            this.comb_shebei_leixing.TabIndex = 5;
            this.comb_shebei_leixing.SelectedIndexChanged += new System.EventHandler(this.comb_shebei_leixing_SelectedIndexChanged);
            // 
            // txt_shebei_miaoshu
            // 
            this.txt_shebei_miaoshu.Location = new System.Drawing.Point(165, 161);
            this.txt_shebei_miaoshu.Multiline = true;
            this.txt_shebei_miaoshu.Name = "txt_shebei_miaoshu";
            this.txt_shebei_miaoshu.Size = new System.Drawing.Size(415, 48);
            this.txt_shebei_miaoshu.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(97, 164);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 7;
            this.label4.Text = "设备描述";
            // 
            // txt_caijicanshu
            // 
            this.txt_caijicanshu.Location = new System.Drawing.Point(165, 225);
            this.txt_caijicanshu.Multiline = true;
            this.txt_caijicanshu.Name = "txt_caijicanshu";
            this.txt_caijicanshu.Size = new System.Drawing.Size(415, 82);
            this.txt_caijicanshu.TabIndex = 10;
            this.txt_caijicanshu.Visible = false;
            // 
            // lb_canshu
            // 
            this.lb_canshu.AutoSize = true;
            this.lb_canshu.Location = new System.Drawing.Point(73, 228);
            this.lb_canshu.Name = "lb_canshu";
            this.lb_canshu.Size = new System.Drawing.Size(77, 12);
            this.lb_canshu.TabIndex = 9;
            this.lb_canshu.Text = "实时采集参数";
            this.lb_canshu.Visible = false;
            // 
            // cb_liuliang
            // 
            this.cb_liuliang.AutoSize = true;
            this.cb_liuliang.Location = new System.Drawing.Point(645, 200);
            this.cb_liuliang.Name = "cb_liuliang";
            this.cb_liuliang.Size = new System.Drawing.Size(72, 16);
            this.cb_liuliang.TabIndex = 11;
            this.cb_liuliang.Text = "总管流量";
            this.cb_liuliang.UseVisualStyleBackColor = true;
            this.cb_liuliang.Visible = false;
            this.cb_liuliang.CheckedChanged += new System.EventHandler(this.cb_liuliang_CheckedChanged);
            // 
            // cb_nenghao
            // 
            this.cb_nenghao.AutoSize = true;
            this.cb_nenghao.Location = new System.Drawing.Point(645, 123);
            this.cb_nenghao.Name = "cb_nenghao";
            this.cb_nenghao.Size = new System.Drawing.Size(72, 16);
            this.cb_nenghao.TabIndex = 12;
            this.cb_nenghao.Text = "能耗计算";
            this.cb_nenghao.UseVisualStyleBackColor = true;
            this.cb_nenghao.Visible = false;
            // 
            // txt_yali_max
            // 
            this.txt_yali_max.Location = new System.Drawing.Point(435, 344);
            this.txt_yali_max.Name = "txt_yali_max";
            this.txt_yali_max.Size = new System.Drawing.Size(145, 21);
            this.txt_yali_max.TabIndex = 17;
            this.txt_yali_max.Visible = false;
            // 
            // lb_yali_max
            // 
            this.lb_yali_max.AutoSize = true;
            this.lb_yali_max.Location = new System.Drawing.Point(369, 347);
            this.lb_yali_max.Name = "lb_yali_max";
            this.lb_yali_max.Size = new System.Drawing.Size(53, 12);
            this.lb_yali_max.TabIndex = 16;
            this.lb_yali_max.Text = "最大压力";
            this.lb_yali_max.Visible = false;
            // 
            // txt_yali_min
            // 
            this.txt_yali_min.Location = new System.Drawing.Point(165, 344);
            this.txt_yali_min.Name = "txt_yali_min";
            this.txt_yali_min.Size = new System.Drawing.Size(153, 21);
            this.txt_yali_min.TabIndex = 15;
            this.txt_yali_min.Visible = false;
            // 
            // lb_yali_min
            // 
            this.lb_yali_min.AutoSize = true;
            this.lb_yali_min.Location = new System.Drawing.Point(97, 347);
            this.lb_yali_min.Name = "lb_yali_min";
            this.lb_yali_min.Size = new System.Drawing.Size(53, 12);
            this.lb_yali_min.TabIndex = 14;
            this.lb_yali_min.Text = "最小压力";
            this.lb_yali_min.Visible = false;
            // 
            // txt_fshebei_id
            // 
            this.txt_fshebei_id.Location = new System.Drawing.Point(435, 119);
            this.txt_fshebei_id.Name = "txt_fshebei_id";
            this.txt_fshebei_id.Size = new System.Drawing.Size(145, 21);
            this.txt_fshebei_id.TabIndex = 19;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(367, 122);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 12);
            this.label8.TabIndex = 18;
            this.label8.Text = "父设备ID";
            // 
            // btn_save
            // 
            this.btn_save.BackColor = System.Drawing.Color.Turquoise;
            this.btn_save.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_save.FlatAppearance.BorderSize = 0;
            this.btn_save.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_save.Font = new System.Drawing.Font("微软雅黑", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_save.ForeColor = System.Drawing.Color.Black;
            this.btn_save.Location = new System.Drawing.Point(435, 437);
            this.btn_save.Name = "btn_save";
            this.btn_save.Size = new System.Drawing.Size(145, 39);
            this.btn_save.TabIndex = 20;
            this.btn_save.Text = "保存";
            this.btn_save.UseVisualStyleBackColor = false;
            this.btn_save.Click += new System.EventHandler(this.btn_save_Click);
            // 
            // lb_beizhu
            // 
            this.lb_beizhu.AutoSize = true;
            this.lb_beizhu.Location = new System.Drawing.Point(163, 312);
            this.lb_beizhu.Name = "lb_beizhu";
            this.lb_beizhu.Size = new System.Drawing.Size(233, 12);
            this.lb_beizhu.TabIndex = 21;
            this.lb_beizhu.Text = "*样式：开关{#1}|压力{#2}Mpa|功率{#3}Kw";
            this.lb_beizhu.Visible = false;
            // 
            // txt_paixu
            // 
            this.txt_paixu.Location = new System.Drawing.Point(165, 387);
            this.txt_paixu.Name = "txt_paixu";
            this.txt_paixu.Size = new System.Drawing.Size(153, 21);
            this.txt_paixu.TabIndex = 23;
            this.txt_paixu.Visible = false;
            // 
            // lb_paixuhao
            // 
            this.lb_paixuhao.AutoSize = true;
            this.lb_paixuhao.Location = new System.Drawing.Point(49, 390);
            this.lb_paixuhao.Name = "lb_paixuhao";
            this.lb_paixuhao.Size = new System.Drawing.Size(101, 12);
            this.lb_paixuhao.TabIndex = 22;
            this.lb_paixuhao.Text = "螺杆机联动排序号";
            this.lb_paixuhao.Visible = false;
            // 
            // lb_title
            // 
            this.lb_title.AutoSize = true;
            this.lb_title.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lb_title.ForeColor = System.Drawing.Color.White;
            this.lb_title.Location = new System.Drawing.Point(13, 12);
            this.lb_title.Name = "lb_title";
            this.lb_title.Size = new System.Drawing.Size(96, 28);
            this.lb_title.TabIndex = 0;
            this.lb_title.Text = "设备编辑";
            this.lb_title.Click += new System.EventHandler(this.lb_lb_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Navy;
            this.panel1.Controls.Add(this.lb_title);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(744, 52);
            this.panel1.TabIndex = 24;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // cb_qiyong
            // 
            this.cb_qiyong.AutoSize = true;
            this.cb_qiyong.Location = new System.Drawing.Point(645, 83);
            this.cb_qiyong.Name = "cb_qiyong";
            this.cb_qiyong.Size = new System.Drawing.Size(72, 16);
            this.cb_qiyong.TabIndex = 25;
            this.cb_qiyong.Text = "启用设备";
            this.cb_qiyong.UseVisualStyleBackColor = true;
            // 
            // frm_shebei_edit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(744, 525);
            this.Controls.Add(this.cb_qiyong);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.cb_nenghao);
            this.Controls.Add(this.cb_liuliang);
            this.Controls.Add(this.cb_yali);
            this.Controls.Add(this.txt_paixu);
            this.Controls.Add(this.lb_paixuhao);
            this.Controls.Add(this.lb_beizhu);
            this.Controls.Add(this.btn_save);
            this.Controls.Add(this.txt_fshebei_id);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txt_yali_max);
            this.Controls.Add(this.lb_yali_max);
            this.Controls.Add(this.txt_yali_min);
            this.Controls.Add(this.lb_yali_min);
            this.Controls.Add(this.txt_caijicanshu);
            this.Controls.Add(this.lb_canshu);
            this.Controls.Add(this.txt_shebei_miaoshu);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comb_shebei_leixing);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comb_zhandian);
            this.Controls.Add(this.txt_shebei_name);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frm_shebei_edit";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.frm_shebei_edit_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_shebei_name;
        private System.Windows.Forms.ComboBox comb_zhandian;
        private System.Windows.Forms.CheckBox cb_yali;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comb_shebei_leixing;
        private System.Windows.Forms.TextBox txt_shebei_miaoshu;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txt_caijicanshu;
        private System.Windows.Forms.Label lb_canshu;
        private System.Windows.Forms.CheckBox cb_liuliang;
        private System.Windows.Forms.CheckBox cb_nenghao;
        private System.Windows.Forms.TextBox txt_yali_max;
        private System.Windows.Forms.Label lb_yali_max;
        private System.Windows.Forms.TextBox txt_yali_min;
        private System.Windows.Forms.Label lb_yali_min;
        private System.Windows.Forms.TextBox txt_fshebei_id;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btn_save;
        private System.Windows.Forms.Label lb_beizhu;
        private System.Windows.Forms.TextBox txt_paixu;
        private System.Windows.Forms.Label lb_paixuhao;
        private System.Windows.Forms.Label lb_title;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox cb_qiyong;
    }
}