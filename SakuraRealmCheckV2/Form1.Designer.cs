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
            lbResults = new ListBox();
            label1 = new Label();
            btnGetFiles = new Button();
            progressBar1 = new ProgressBar();
            SuspendLayout();
            // 
            // lbResults
            // 
            lbResults.FormattingEnabled = true;
            lbResults.ItemHeight = 24;
            lbResults.Location = new Point(12, 12);
            lbResults.Name = "lbResults";
            lbResults.Size = new Size(770, 364);
            lbResults.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(13, 427);
            label1.Name = "label1";
            label1.Size = new Size(118, 24);
            label1.TabIndex = 1;
            label1.Text = "客户端版本：";
            // 
            // btnGetFiles
            // 
            btnGetFiles.Location = new Point(670, 427);
            btnGetFiles.Name = "btnGetFiles";
            btnGetFiles.Size = new Size(112, 34);
            btnGetFiles.TabIndex = 2;
            btnGetFiles.Text = "重新检测";
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
            // Form1
            // 
            AutoScaleDimensions = new SizeF(11F, 24F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(792, 484);
            Controls.Add(progressBar1);
            Controls.Add(btnGetFiles);
            Controls.Add(label1);
            Controls.Add(lbResults);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox lbResults;
        private Label label1;
        private Button btnGetFiles;
        private ProgressBar progressBar1;
    }
}