namespace Server
{
    partial class ServerForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.listBox_log = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label_online = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // listBox_log
            // 
            this.listBox_log.FormattingEnabled = true;
            this.listBox_log.Location = new System.Drawing.Point(12, 51);
            this.listBox_log.Name = "listBox_log";
            this.listBox_log.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.listBox_log.Size = new System.Drawing.Size(497, 173);
            this.listBox_log.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft PhagsPa", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(8, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 19);
            this.label1.TabIndex = 1;
            this.label1.Text = "ONLINE:";
            // 
            // label_online
            // 
            this.label_online.AutoSize = true;
            this.label_online.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label_online.Location = new System.Drawing.Point(90, 17);
            this.label_online.Name = "label_online";
            this.label_online.Size = new System.Drawing.Size(18, 20);
            this.label_online.TabIndex = 2;
            this.label_online.Text = "0";
            // 
            // ServerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(522, 286);
            this.Controls.Add(this.label_online);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listBox_log);
            this.Name = "ServerForm";
            this.Text = "Chat server";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBox_log;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label_online;
    }
}

