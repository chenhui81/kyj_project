
namespace kyj_project
{
    partial class frm_zhandian_edit
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frm_zhandian_edit));
            this.panel1 = new System.Windows.Forms.Panel();
            this.lb_title = new System.Windows.Forms.Label();
            this.cb_liandong = new System.Windows.Forms.CheckBox();
            this.btn_save = new System.Windows.Forms.Button();
            this.txt_zhandian_id = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_zhandian_mingcheng = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_mubiaoyali = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txt_url = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
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
            // cb_liandong
            // 
            this.cb_liandong.AutoSize = true;
            this.cb_liandong.Location = new System.Drawing.Point(227, 266);
            this.cb_liandong.Name = "cb_liandong";
            this.cb_liandong.Size = new System.Drawing.Size(108, 16);
            this.cb_liandong.TabIndex = 29;
            this.cb_liandong.Text = "螺杆机联动控制";
            this.cb_liandong.UseVisualStyleBackColor = true;
            // 
            // btn_save
            // 
            this.btn_save.BackColor = System.Drawing.Color.Turquoise;
            this.btn_save.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_save.FlatAppearance.BorderSize = 0;
            this.btn_save.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_save.Font = new System.Drawing.Font("微软雅黑", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_save.ForeColor = System.Drawing.Color.Black;
            this.btn_save.Location = new System.Drawing.Point(451, 318);
            this.btn_save.Name = "btn_save";
            this.btn_save.Size = new System.Drawing.Size(145, 39);
            this.btn_save.TabIndex = 30;
            this.btn_save.Text = "保存";
            this.btn_save.UseVisualStyleBackColor = false;
            this.btn_save.Click += new System.EventHandler(this.btn_save_Click);
            // 
            // txt_zhandian_id
            // 
            this.txt_zhandian_id.Location = new System.Drawing.Point(227, 90);
            this.txt_zhandian_id.Name = "txt_zhandian_id";
            this.txt_zhandian_id.Size = new System.Drawing.Size(153, 21);
            this.txt_zhandian_id.TabIndex = 28;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(159, 93);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 27;
            this.label1.Text = "站点编号";
            // 
            // txt_zhandian_mingcheng
            // 
            this.txt_zhandian_mingcheng.Location = new System.Drawing.Point(227, 134);
            this.txt_zhandian_mingcheng.Name = "txt_zhandian_mingcheng";
            this.txt_zhandian_mingcheng.Size = new System.Drawing.Size(153, 21);
            this.txt_zhandian_mingcheng.TabIndex = 32;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(159, 137);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 31;
            this.label2.Text = "站点名称";
            // 
            // txt_mubiaoyali
            // 
            this.txt_mubiaoyali.Location = new System.Drawing.Point(227, 175);
            this.txt_mubiaoyali.Name = "txt_mubiaoyali";
            this.txt_mubiaoyali.Size = new System.Drawing.Size(153, 21);
            this.txt_mubiaoyali.TabIndex = 34;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(159, 178);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 33;
            this.label3.Text = "目标压力";
            // 
            // txt_url
            // 
            this.txt_url.Location = new System.Drawing.Point(227, 218);
            this.txt_url.Name = "txt_url";
            this.txt_url.Size = new System.Drawing.Size(380, 21);
            this.txt_url.TabIndex = 36;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(159, 221);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 12);
            this.label4.TabIndex = 35;
            this.label4.Text = "组态图URL";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(386, 180);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(23, 12);
            this.label5.TabIndex = 37;
            this.label5.Text = "Mpa";
            // 
            // frm_zhandian_edit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(685, 416);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txt_url);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txt_mubiaoyali);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txt_zhandian_mingcheng);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cb_liandong);
            this.Controls.Add(this.btn_save);
            this.Controls.Add(this.txt_zhandian_id);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frm_zhandian_edit";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.frm_zhandian_edit_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lb_title;
        private System.Windows.Forms.CheckBox cb_liandong;
        private System.Windows.Forms.Button btn_save;
        private System.Windows.Forms.TextBox txt_zhandian_id;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_zhandian_mingcheng;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txt_mubiaoyali;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txt_url;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
    }
}