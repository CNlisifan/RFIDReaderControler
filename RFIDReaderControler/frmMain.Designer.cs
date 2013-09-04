namespace RFIDReaderControler
{
    partial class frmMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.lblStatus = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.打开读写器SToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.关闭读写器CToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.退出XToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.系统SToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.读写器管理MToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.系统配置CToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.退出TToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.帮助AToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.关于AToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(3, 471);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(41, 12);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "label1";
            this.lblStatus.Visible = false;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(697, 487);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(94, 25);
            this.button3.TabIndex = 4;
            this.button3.Text = "上传标签测试";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Visible = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.系统SToolStripMenuItem,
            this.帮助AToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(645, 25);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.打开读写器SToolStripMenuItem,
            this.关闭读写器CToolStripMenuItem,
            this.退出XToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(60, 21);
            this.toolStripMenuItem1.Text = "操作(&C)";
            // 
            // 打开读写器SToolStripMenuItem
            // 
            this.打开读写器SToolStripMenuItem.Name = "打开读写器SToolStripMenuItem";
            this.打开读写器SToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.打开读写器SToolStripMenuItem.Text = "打开读写器(&S)";
            this.打开读写器SToolStripMenuItem.Click += new System.EventHandler(this.打开读写器SToolStripMenuItem_Click);
            // 
            // 关闭读写器CToolStripMenuItem
            // 
            this.关闭读写器CToolStripMenuItem.Name = "关闭读写器CToolStripMenuItem";
            this.关闭读写器CToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.关闭读写器CToolStripMenuItem.Text = "关闭读写器(&C)";
            this.关闭读写器CToolStripMenuItem.Visible = false;
            this.关闭读写器CToolStripMenuItem.Click += new System.EventHandler(this.关闭读写器CToolStripMenuItem_Click);
            // 
            // 退出XToolStripMenuItem
            // 
            this.退出XToolStripMenuItem.Name = "退出XToolStripMenuItem";
            this.退出XToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.退出XToolStripMenuItem.Text = "退出(&X)";
            this.退出XToolStripMenuItem.Click += new System.EventHandler(this.退出XToolStripMenuItem_Click);
            // 
            // 系统SToolStripMenuItem
            // 
            this.系统SToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.读写器管理MToolStripMenuItem,
            this.系统配置CToolStripMenuItem,
            this.退出TToolStripMenuItem});
            this.系统SToolStripMenuItem.Name = "系统SToolStripMenuItem";
            this.系统SToolStripMenuItem.Size = new System.Drawing.Size(83, 21);
            this.系统SToolStripMenuItem.Text = "系统设置(&S)";
            // 
            // 读写器管理MToolStripMenuItem
            // 
            this.读写器管理MToolStripMenuItem.Name = "读写器管理MToolStripMenuItem";
            this.读写器管理MToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.读写器管理MToolStripMenuItem.Text = "读写器管理(&M)";
            this.读写器管理MToolStripMenuItem.Click += new System.EventHandler(this.读写器管理MToolStripMenuItem_Click);
            // 
            // 系统配置CToolStripMenuItem
            // 
            this.系统配置CToolStripMenuItem.Name = "系统配置CToolStripMenuItem";
            this.系统配置CToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.系统配置CToolStripMenuItem.Text = "系统配置(&C)";
            this.系统配置CToolStripMenuItem.Click += new System.EventHandler(this.系统配置CToolStripMenuItem_Click);
            // 
            // 退出TToolStripMenuItem
            // 
            this.退出TToolStripMenuItem.Name = "退出TToolStripMenuItem";
            this.退出TToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.退出TToolStripMenuItem.Text = "退出(&T)";
            this.退出TToolStripMenuItem.Visible = false;
            this.退出TToolStripMenuItem.Click += new System.EventHandler(this.退出TToolStripMenuItem_Click);
            // 
            // 帮助AToolStripMenuItem
            // 
            this.帮助AToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.关于AToolStripMenuItem});
            this.帮助AToolStripMenuItem.Name = "帮助AToolStripMenuItem";
            this.帮助AToolStripMenuItem.Size = new System.Drawing.Size(61, 21);
            this.帮助AToolStripMenuItem.Text = "帮助(&H)";
            // 
            // 关于AToolStripMenuItem
            // 
            this.关于AToolStripMenuItem.Name = "关于AToolStripMenuItem";
            this.关于AToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.关于AToolStripMenuItem.Text = "关于(&A)";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(645, 490);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RFID读写器监控";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 系统SToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 系统配置CToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 退出TToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 帮助AToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 关于AToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 打开读写器SToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 关闭读写器CToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 读写器管理MToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 退出XToolStripMenuItem;
    }
}

