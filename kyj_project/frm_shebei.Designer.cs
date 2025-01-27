
namespace kyj_project
{
    partial class frm_shebei
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frm_shebei));
            this.panel1 = new System.Windows.Forms.Panel();
            this.pb_chaxun = new System.Windows.Forms.PictureBox();
            this.cb_nenghao = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.lb_lb = new System.Windows.Forms.Label();
            this.comb_shebei_leixing = new System.Windows.Forms.ComboBox();
            this.comb_zhandian = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton5 = new System.Windows.Forms.ToolStripButton();
            this.panel3 = new System.Windows.Forms.Panel();
            this.comb_rowcount = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.sph_total = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.sph_current_page = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.sph_last = new System.Windows.Forms.Button();
            this.sph_first = new System.Windows.Forms.Button();
            this.sph_next = new System.Windows.Forms.Button();
            this.sph_up = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_chaxun)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Navy;
            this.panel1.Controls.Add(this.pb_chaxun);
            this.panel1.Controls.Add(this.cb_nenghao);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.lb_lb);
            this.panel1.Controls.Add(this.comb_shebei_leixing);
            this.panel1.Controls.Add(this.comb_zhandian);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(965, 60);
            this.panel1.TabIndex = 20;
            // 
            // pb_chaxun
            // 
            this.pb_chaxun.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pb_chaxun.BackgroundImage = global::kyj_project.Properties.Resources._11;
            this.pb_chaxun.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pb_chaxun.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pb_chaxun.Location = new System.Drawing.Point(901, 12);
            this.pb_chaxun.Name = "pb_chaxun";
            this.pb_chaxun.Size = new System.Drawing.Size(36, 36);
            this.pb_chaxun.TabIndex = 48;
            this.pb_chaxun.TabStop = false;
            this.pb_chaxun.Tag = "查询";
            this.pb_chaxun.Click += new System.EventHandler(this.pb_chaxun_Click);
            // 
            // cb_nenghao
            // 
            this.cb_nenghao.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cb_nenghao.AutoSize = true;
            this.cb_nenghao.ForeColor = System.Drawing.Color.White;
            this.cb_nenghao.Location = new System.Drawing.Point(802, 23);
            this.cb_nenghao.Name = "cb_nenghao";
            this.cb_nenghao.Size = new System.Drawing.Size(72, 16);
            this.cb_nenghao.TabIndex = 13;
            this.cb_nenghao.Text = "能耗计算";
            this.cb_nenghao.UseVisualStyleBackColor = true;
            this.cb_nenghao.CheckedChanged += new System.EventHandler(this.cb_nenghao_CheckedChanged);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(610, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 10;
            this.label3.Text = "设备类型";
            // 
            // lb_lb
            // 
            this.lb_lb.AutoSize = true;
            this.lb_lb.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lb_lb.ForeColor = System.Drawing.Color.White;
            this.lb_lb.Location = new System.Drawing.Point(13, 12);
            this.lb_lb.Name = "lb_lb";
            this.lb_lb.Size = new System.Drawing.Size(96, 28);
            this.lb_lb.TabIndex = 0;
            this.lb_lb.Text = "设备管理";
            // 
            // comb_shebei_leixing
            // 
            this.comb_shebei_leixing.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comb_shebei_leixing.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.comb_shebei_leixing.FormattingEnabled = true;
            this.comb_shebei_leixing.IntegralHeight = false;
            this.comb_shebei_leixing.ItemHeight = 12;
            this.comb_shebei_leixing.Location = new System.Drawing.Point(666, 21);
            this.comb_shebei_leixing.Name = "comb_shebei_leixing";
            this.comb_shebei_leixing.Size = new System.Drawing.Size(98, 20);
            this.comb_shebei_leixing.TabIndex = 9;
            this.comb_shebei_leixing.SelectedIndexChanged += new System.EventHandler(this.comb_shebei_leixing_SelectedIndexChanged);
            // 
            // comb_zhandian
            // 
            this.comb_zhandian.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comb_zhandian.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.comb_zhandian.FormattingEnabled = true;
            this.comb_zhandian.IntegralHeight = false;
            this.comb_zhandian.ItemHeight = 12;
            this.comb_zhandian.Location = new System.Drawing.Point(433, 21);
            this.comb_zhandian.Name = "comb_zhandian";
            this.comb_zhandian.Size = new System.Drawing.Size(145, 20);
            this.comb_zhandian.TabIndex = 7;
            this.comb_zhandian.SelectedIndexChanged += new System.EventHandler(this.comb_zhandian_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(365, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 8;
            this.label2.Text = "空压机站点";
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
            this.dataGridView1.Location = new System.Drawing.Point(0, 85);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(965, 421);
            this.dataGridView1.TabIndex = 21;
            this.dataGridView1.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGridView1_CellFormatting);
            this.dataGridView1.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dataGridView1_DataBindingComplete);
            this.dataGridView1.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.dataGridView1_RowPostPaint);
            this.dataGridView1.Click += new System.EventHandler(this.dataGridView1_Click);
            this.dataGridView1.DoubleClick += new System.EventHandler(this.dataGridView1_DoubleClick);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripSeparator1,
            this.toolStripButton2,
            this.toolStripSeparator2,
            this.toolStripButton3,
            this.toolStripSeparator3,
            this.toolStripButton4,
            this.toolStripButton5});
            this.toolStrip1.Location = new System.Drawing.Point(0, 60);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(965, 25);
            this.toolStrip1.TabIndex = 24;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(60, 22);
            this.toolStripButton1.Text = "设备新增";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(60, 22);
            this.toolStripButton2.Text = "设备编辑";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton3.Image")));
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(60, 22);
            this.toolStripButton3.Text = "设备删除";
            this.toolStripButton3.Click += new System.EventHandler(this.toolStripButton3_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButton4
            // 
            this.toolStripButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton4.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton4.Image")));
            this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton4.Name = "toolStripButton4";
            this.toolStripButton4.Size = new System.Drawing.Size(84, 22);
            this.toolStripButton4.Text = "设备参数设置";
            this.toolStripButton4.Click += new System.EventHandler(this.toolStripButton4_Click);
            // 
            // toolStripButton5
            // 
            this.toolStripButton5.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton5.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton5.Image")));
            this.toolStripButton5.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton5.Name = "toolStripButton5";
            this.toolStripButton5.Size = new System.Drawing.Size(84, 22);
            this.toolStripButton5.Text = "设备实时数据";
            this.toolStripButton5.Click += new System.EventHandler(this.toolStripButton5_Click);
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
            this.panel3.Controls.Add(this.label1);
            this.panel3.Controls.Add(this.sph_last);
            this.panel3.Controls.Add(this.sph_first);
            this.panel3.Controls.Add(this.sph_next);
            this.panel3.Controls.Add(this.sph_up);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 464);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(965, 42);
            this.panel3.TabIndex = 25;
            // 
            // comb_rowcount
            // 
            this.comb_rowcount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
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
            this.sph_total.Location = new System.Drawing.Point(848, 9);
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
            this.label5.Location = new System.Drawing.Point(777, 17);
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
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(310, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "当前页：";
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
            // frm_shebei
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(965, 506);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frm_shebei";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frm_shebei_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_chaxun)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lb_lb;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comb_shebei_leixing;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comb_zhandian;
        private System.Windows.Forms.CheckBox cb_nenghao;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton toolStripButton4;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.ComboBox comb_rowcount;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label sph_total;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label sph_current_page;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button sph_last;
        private System.Windows.Forms.Button sph_first;
        private System.Windows.Forms.Button sph_next;
        private System.Windows.Forms.Button sph_up;
        private System.Windows.Forms.PictureBox pb_chaxun;
        private System.Windows.Forms.ToolStripButton toolStripButton5;
    }
}