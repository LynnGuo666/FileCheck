namespace SakuraRealmCheckV2
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            lbResults = new ListBox();
            label1 = new Label();
            btnGetFiles = new Button();
            progressBar1 = new ProgressBar();
            checkBox1 = new CheckBox();
            checkBox2 = new CheckBox();
            linkLabel1 = new LinkLabel();
            linkLabel2 = new LinkLabel();
            linkLabel3 = new LinkLabel();
            checkBox3 = new CheckBox();
            SuspendLayout();
            // 
            // lbResults
            // 
            lbResults.FormattingEnabled = true;
            lbResults.HorizontalScrollbar = true;
            lbResults.ItemHeight = 24;
            lbResults.Location = new Point(12, 12);
            lbResults.Name = "lbResults";
            lbResults.Size = new Size(770, 364);
            lbResults.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(525, 467);
            label1.Name = "label1";
            label1.Size = new Size(255, 48);
            label1.TabIndex = 1;
            label1.Text = "Version 2.3 | By LynnGuo666\r\n\r\n";
            label1.Click += label1_Click;
            // 
            // btnGetFiles
            // 
            btnGetFiles.Location = new Point(670, 427);
            btnGetFiles.Name = "btnGetFiles";
            btnGetFiles.Size = new Size(112, 34);
            btnGetFiles.TabIndex = 2;
            btnGetFiles.Text = "检测文件";
            btnGetFiles.UseVisualStyleBackColor = true;
            btnGetFiles.Click += btnGetFiles_Click;
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(13, 382);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(769, 35);
            progressBar1.TabIndex = 3;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(556, 431);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(108, 28);
            checkBox1.TabIndex = 4;
            checkBox1.Text = "精确检测";
            checkBox1.UseVisualStyleBackColor = true;
            checkBox1.CheckedChanged += checkBox1_CheckedChanged_1;
            // 
            // checkBox2
            // 
            checkBox2.AutoSize = true;
            checkBox2.Checked = true;
            checkBox2.CheckState = CheckState.Checked;
            checkBox2.Location = new Point(433, 431);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(108, 28);
            checkBox2.TabIndex = 5;
            checkBox2.Text = "自动补全";
            checkBox2.UseVisualStyleBackColor = true;
            // 
            // linkLabel1
            // 
            linkLabel1.AutoSize = true;
            linkLabel1.Location = new Point(13, 432);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(82, 24);
            linkLabel1.TabIndex = 6;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "帮助中心";
            linkLabel1.LinkClicked += linkLabel1_LinkClicked;
            // 
            // linkLabel2
            // 
            linkLabel2.AutoSize = true;
            linkLabel2.Location = new Point(433, 467);
            linkLabel2.Name = "linkLabel2";
            linkLabel2.Size = new Size(82, 24);
            linkLabel2.TabIndex = 7;
            linkLabel2.TabStop = true;
            linkLabel2.Text = "赞助作者";
            linkLabel2.LinkClicked += linkLabel2_LinkClicked;
            // 
            // linkLabel3
            // 
            linkLabel3.AutoSize = true;
            linkLabel3.Location = new Point(13, 467);
            linkLabel3.Name = "linkLabel3";
            linkLabel3.Size = new Size(244, 24);
            linkLabel3.TabIndex = 8;
            linkLabel3.TabStop = true;
            linkLabel3.Text = "下载速度慢？尝试下载预载包";
            linkLabel3.LinkClicked += linkLabel3_LinkClicked;
            // 
            // checkBox3
            // 
            checkBox3.AutoSize = true;
            checkBox3.Enabled = false;
            checkBox3.Location = new Point(307, 431);
            checkBox3.Name = "checkBox3";
            checkBox3.Size = new Size(108, 28);
            checkBox3.TabIndex = 9;
            checkBox3.Text = "国内镜像";
            checkBox3.UseVisualStyleBackColor = true;
            checkBox3.CheckedChanged += checkBox3_CheckedChanged;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(11F, 24F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(792, 500);
            Controls.Add(checkBox3);
            Controls.Add(linkLabel3);
            Controls.Add(linkLabel2);
            Controls.Add(linkLabel1);
            Controls.Add(checkBox2);
            Controls.Add(checkBox1);
            Controls.Add(progressBar1);
            Controls.Add(btnGetFiles);
            Controls.Add(label1);
            Controls.Add(lbResults);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            Text = "文件缺失比对工具";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox lbResults;
        private Label label1;
        private Button btnGetFiles;
        private ProgressBar progressBar1;
        private CheckBox checkBox1;
        private CheckBox checkBox2;
        private LinkLabel linkLabel1;
        private LinkLabel linkLabel2;
        private LinkLabel linkLabel3;
        private CheckBox checkBox3;
    }
}