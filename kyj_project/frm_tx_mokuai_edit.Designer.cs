
namespace kyj_project
{
    partial class frm_tx_mokuai_edit
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frm_tx_mokuai_edit));
            this.panel1 = new System.Windows.Forms.Panel();
            this.lb_title = new System.Windows.Forms.Label();
            this.btn_save = new System.Windows.Forms.Button();
            this.txt_txmk_mingcheng = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_constr = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.lb_zt = new System.Windows.Forms.Label();
            this.comb_xieyi = new System.Windows.Forms.ComboBox();
            this.btn_con = new System.Windows.Forms.Button();
            this.cb_qiyong = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Navy;
            this.panel1.Controls.Add(this.lb_title);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(685, 64);
            this.panel1.TabIndex = 26;
            // 
            // lb_title
            // 
            this.lb_title.AutoSize = true;
            this.lb_title.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lb_title.ForeColor = System.Drawing.Color.White;
            this.lb_title.Location = new System.Drawing.Point(17, 17);
            this.lb_title.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lb_title.Name = "lb_title";
            this.lb_title.Size = new System.Drawing.Size(96, 28);
            this.lb_title.TabIndex = 0;
            this.lb_title.Text = "站点管理";
            // 
            // btn_save
            // 
            this.btn_save.BackColor = System.Drawing.Color.Turquoise;
            this.btn_save.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_save.FlatAppearance.BorderSize = 0;
            this.btn_save.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_save.Font = new System.Drawing.Font("微软雅黑", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_save.ForeColor = System.Drawing.Color.Black;
            this.btn_save.Location = new System.Drawing.Point(367, 308);
            this.btn_save.Name = "btn_save";
            this.btn_save.Size = new System.Drawing.Size(145, 39);
            this.btn_save.TabIndex = 30;
            this.btn_save.Text = "保存";
            this.btn_save.UseVisualStyleBackColor = false;
            this.btn_save.Click += new System.EventHandler(this.btn_save_Click);
            // 
            // txt_txmk_mingcheng
            // 
            this.txt_txmk_mingcheng.Location = new System.Drawing.Point(203, 109);
            this.txt_txmk_mingcheng.Name = "txt_txmk_mingcheng";
            this.txt_txmk_mingcheng.Size = new System.Drawing.Size(309, 21);
            this.txt_txmk_mingcheng.TabIndex = 28;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(111, 112);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 27;
            this.label1.Text = "通讯模块名称";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(135, 156);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 31;
            this.label2.Text = "通讯协议";
            // 
            // txt_constr
            // 
            this.txt_constr.Location = new System.Drawing.Point(203, 198);
            this.txt_constr.Name = "txt_constr";
            this.txt_constr.Size = new System.Drawing.Size(309, 21);
            this.txt_constr.TabIndex = 34;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(123, 202);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 33;
            this.label3.Text = "协议字符串";
            // 
            // lb_zt
            // 
            this.lb_zt.AutoSize = true;
            this.lb_zt.Location = new System.Drawing.Point(201, 226);
            this.lb_zt.Name = "lb_zt";
            this.lb_zt.Size = new System.Drawing.Size(35, 12);
            this.lb_zt.TabIndex = 35;
            this.lb_zt.Text = "*格式";
            // 
            // comb_xieyi
            // 
            this.comb_xieyi.FormattingEnabled = true;
            this.comb_xieyi.Items.AddRange(new object[] {
            "PLC S7",
            "MODBUS TCP",
            "MODBUS RTU"});
            this.comb_xieyi.Location = new System.Drawing.Point(203, 153);
            this.comb_xieyi.Name = "comb_xieyi";
            this.comb_xieyi.Size = new System.Drawing.Size(309, 20);
            this.comb_xieyi.TabIndex = 38;
            this.comb_xieyi.SelectedIndexChanged += new System.EventHandler(this.comb_xieyi_SelectedIndexChanged);
            this.comb_xieyi.TextChanged += new System.EventHandler(this.comb_xieyi_TextChanged);
            // 
            // btn_con
            // 
            this.btn_con.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btn_con.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_con.FlatAppearance.BorderSize = 0;
            this.btn_con.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btn_con.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_con.ForeColor = System.Drawing.Color.Black;
            this.btn_con.Location = new System.Drawing.Point(546, 194);
            this.btn_con.Name = "btn_con";
            this.btn_con.Size = new System.Drawing.Size(95, 30);
            this.btn_con.TabIndex = 39;
            this.btn_con.Text = "连接测试";
            this.btn_con.UseVisualStyleBackColor = false;
            this.btn_con.Click += new System.EventHandler(this.btn_con_Click);
            // 
            // cb_qiyong
            // 
            this.cb_qiyong.AutoSize = true;
            this.cb_qiyong.Location = new System.Drawing.Point(203, 260);
            this.cb_qiyong.Name = "cb_qiyong";
            this.cb_qiyong.Size = new System.Drawing.Size(48, 16);
            this.cb_qiyong.TabIndex = 40;
            this.cb_qiyong.Text = "启用";
            this.cb_qiyong.UseVisualStyleBackColor = true;
            // 
            // frm_tx_mokuai_edit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(685, 416);
            this.Controls.Add(this.cb_qiyong);
            this.Controls.Add(this.btn_con);
            this.Controls.Add(this.comb_xieyi);
            this.Controls.Add(this.lb_zt);
            this.Controls.Add(this.txt_constr);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btn_save);
            this.Controls.Add(this.txt_txmk_mingcheng);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frm_tx_mokuai_edit";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.frm_tx_mokuai_edit_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lb_title;
        private System.Windows.Forms.Button btn_save;
        private System.Windows.Forms.TextBox txt_txmk_mingcheng;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txt_constr;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lb_zt;
        private System.Windows.Forms.ComboBox comb_xieyi;
        private System.Windows.Forms.Button btn_con;
        private System.Windows.Forms.CheckBox cb_qiyong;
    }
}