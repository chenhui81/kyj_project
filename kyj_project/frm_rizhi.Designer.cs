
namespace kyj_project
{
    partial class frm_rizhi
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frm_rizhi));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.panel3 = new System.Windows.Forms.Panel();
            this.comb_rowcount = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.sph_total = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.sph_current_page = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.sph_last = new System.Windows.Forms.Button();
            this.sph_first = new System.Windows.Forms.Button();
            this.sph_next = new System.Windows.Forms.Button();
            this.sph_up = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pb_chaxun = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.lb_title = new System.Windows.Forms.Label();
            this.comb_xitong = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comb_leixing = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel3.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_chaxun)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.dataGridView1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.GridColor = System.Drawing.SystemColors.ButtonFace;
            this.dataGridView1.Location = new System.Drawing.Point(0, 60);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(1064, 482);
            this.dataGridView1.TabIndex = 20;
            this.dataGridView1.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.dataGridView1_RowPostPaint);
            this.dataGridView1.Click += new System.EventHandler(this.dataGridView1_Click);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.LightGray;
            this.panel3.Controls.Add(this.comb_rowcount);
            this.panel3.Controls.Add(this.label13);
            this.panel3.Controls.Add(this.label12);
            this.panel3.Controls.Add(this.sph_total);
            this.panel3.Controls.Add(this.label5);
            this.panel3.Controls.Add(this.sph_current_page);
            this.panel3.Controls.Add(this.label2);
            this.panel3.Controls.Add(this.sph_last);
            this.panel3.Controls.Add(this.sph_first);
            this.panel3.Controls.Add(this.sph_next);
            this.panel3.Controls.Add(this.sph_up);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 542);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1064, 42);
            this.panel3.TabIndex = 21;
            // 
            // comb_rowcount
            // 
            this.comb_rowcount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comb_rowcount.ForeColor = System.Drawing.Color.Black;
            this.comb_rowcount.FormattingEnabled = true;
            this.comb_rowcount.Items.AddRange(new object[] {
            "20",
            "30",
            "40",
            "50",
            "100"});
            this.comb_rowcount.Location = new System.Drawing.Point(570, 13);
            this.comb_rowcount.Name = "comb_rowcount";
            this.comb_rowcount.Size = new System.Drawing.Size(51, 20);
            this.comb_rowcount.TabIndex = 10;
            this.comb_rowcount.SelectedIndexChanged += new System.EventHandler(this.comb_rowcount_SelectedIndexChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.ForeColor = System.Drawing.Color.Black;
            this.label13.Location = new System.Drawing.Point(628, 17);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(17, 12);
            this.label13.TabIndex = 9;
            this.label13.Text = "条";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.ForeColor = System.Drawing.Color.Black;
            this.label12.Location = new System.Drawing.Point(502, 16);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(65, 12);
            this.label12.TabIndex = 8;
            this.label12.Text = "每页显示：";
            // 
            // sph_total
            // 
            this.sph_total.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.sph_total.AutoSize = true;
            this.sph_total.Font = new System.Drawing.Font("微软雅黑", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.sph_total.ForeColor = System.Drawing.Color.Black;
            this.sph_total.Location = new System.Drawing.Point(947, 9);
            this.sph_total.Name = "sph_total";
            this.sph_total.Size = new System.Drawing.Size(24, 26);
            this.sph_total.TabIndex = 7;
            this.sph_total.Text = "0";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(876, 17);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 6;
            this.label5.Text = "总计数量：";
            // 
            // sph_current_page
            // 
            this.sph_current_page.AutoSize = true;
            this.sph_current_page.Font = new System.Drawing.Font("微软雅黑", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.sph_current_page.ForeColor = System.Drawing.Color.Black;
            this.sph_current_page.Location = new System.Drawing.Point(369, 8);
            this.sph_current_page.Name = "sph_current_page";
            this.sph_current_page.Size = new System.Drawing.Size(45, 26);
            this.sph_current_page.TabIndex = 5;
            this.sph_current_page.Text = "0/0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(310, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "当前页：";
            // 
            // sph_last
            // 
            this.sph_last.FlatAppearance.BorderSize = 0;
            this.sph_last.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.sph_last.ForeColor = System.Drawing.Color.Black;
            this.sph_last.Location = new System.Drawing.Point(227, 11);
            this.sph_last.Name = "sph_last";
            this.sph_last.Size = new System.Drawing.Size(57, 23);
            this.sph_last.TabIndex = 3;
            this.sph_last.Text = "尾页";
            this.sph_last.UseVisualStyleBackColor = true;
            this.sph_last.Click += new System.EventHandler(this.sph_last_Click);
            // 
            // sph_first
            // 
            this.sph_first.FlatAppearance.BorderSize = 0;
            this.sph_first.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.sph_first.ForeColor = System.Drawing.Color.Black;
            this.sph_first.Location = new System.Drawing.Point(28, 11);
            this.sph_first.Name = "sph_first";
            this.sph_first.Size = new System.Drawing.Size(57, 23);
            this.sph_first.TabIndex = 2;
            this.sph_first.Text = "首页";
            this.sph_first.UseVisualStyleBackColor = true;
            this.sph_first.Click += new System.EventHandler(this.sph_first_Click);
            // 
            // sph_next
            // 
            this.sph_next.FlatAppearance.BorderSize = 0;
            this.sph_next.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.sph_next.ForeColor = System.Drawing.Color.Black;
            this.sph_next.Location = new System.Drawing.Point(156, 11);
            this.sph_next.Name = "sph_next";
            this.sph_next.Size = new System.Drawing.Size(57, 23);
            this.sph_next.TabIndex = 1;
            this.sph_next.Text = "下一页";
            this.sph_next.UseVisualStyleBackColor = true;
            this.sph_next.Click += new System.EventHandler(this.sph_next_Click);
            // 
            // sph_up
            // 
            this.sph_up.FlatAppearance.BorderSize = 0;
            this.sph_up.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.sph_up.ForeColor = System.Drawing.Color.Black;
            this.sph_up.Location = new System.Drawing.Point(91, 11);
            this.sph_up.Name = "sph_up";
            this.sph_up.Size = new System.Drawing.Size(57, 23);
            this.sph_up.TabIndex = 0;
            this.sph_up.Text = "上一页";
            this.sph_up.UseVisualStyleBackColor = true;
            this.sph_up.Click += new System.EventHandler(this.sph_up_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Navy;
            this.panel1.Controls.Add(this.pb_chaxun);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.lb_title);
            this.panel1.Controls.Add(this.comb_xitong);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.comb_leixing);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1064, 60);
            this.panel1.TabIndex = 25;
            // 
            // pb_chaxun
            // 
            this.pb_chaxun.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pb_chaxun.BackgroundImage = global::kyj_project.Properties.Resources._11;
            this.pb_chaxun.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pb_chaxun.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pb_chaxun.Location = new System.Drawing.Point(997, 12);
            this.pb_chaxun.Name = "pb_chaxun";
            this.pb_chaxun.Size = new System.Drawing.Size(36, 36);
            this.pb_chaxun.TabIndex = 47;
            this.pb_chaxun.TabStop = false;
            this.pb_chaxun.Tag = "查询";
            this.pb_chaxun.Click += new System.EventHandler(this.pb_chaxun_Click);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(839, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 14;
            this.label3.Text = "系统";
            // 
            // lb_title
            // 
            this.lb_title.AutoSize = true;
            this.lb_title.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lb_title.ForeColor = System.Drawing.Color.White;
            this.lb_title.Location = new System.Drawing.Point(23, 18);
            this.lb_title.Name = "lb_title";
            this.lb_title.Size = new System.Drawing.Size(96, 28);
            this.lb_title.TabIndex = 0;
            this.lb_title.Text = "日志记录";
            // 
            // comb_xitong
            // 
            this.comb_xitong.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comb_xitong.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.comb_xitong.FormattingEnabled = true;
            this.comb_xitong.IntegralHeight = false;
            this.comb_xitong.ItemHeight = 12;
            this.comb_xitong.Items.AddRange(new object[] {
            "",
            "采集系统",
            "后台服务",
            "web系统"});
            this.comb_xitong.Location = new System.Drawing.Point(874, 19);
            this.comb_xitong.Name = "comb_xitong";
            this.comb_xitong.Size = new System.Drawing.Size(98, 20);
            this.comb_xitong.TabIndex = 13;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(677, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 12;
            this.label1.Text = "类型";
            // 
            // comb_leixing
            // 
            this.comb_leixing.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comb_leixing.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.comb_leixing.FormattingEnabled = true;
            this.comb_leixing.IntegralHeight = false;
            this.comb_leixing.ItemHeight = 12;
            this.comb_leixing.Items.AddRange(new object[] {
            "",
            "系统日志",
            "操作日志"});
            this.comb_leixing.Location = new System.Drawing.Point(716, 18);
            this.comb_leixing.Name = "comb_leixing";
            this.comb_leixing.Size = new System.Drawing.Size(101, 20);
            this.comb_leixing.TabIndex = 11;
            // 
            // frm_rizhi
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1064, 584);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel3);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frm_rizhi";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.frm_rizhi_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_chaxun)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.ComboBox comb_rowcount;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label sph_total;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label sph_current_page;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button sph_last;
        private System.Windows.Forms.Button sph_first;
        private System.Windows.Forms.Button sph_next;
        private System.Windows.Forms.Button sph_up;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lb_title;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comb_xitong;
        private System.Windows.Forms.ComboBox comb_leixing;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pb_chaxun;
    }
}