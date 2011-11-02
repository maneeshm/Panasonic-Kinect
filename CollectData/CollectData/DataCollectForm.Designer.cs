namespace CollectData
{
    partial class DataCollectForm
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
            this.StartButton = new System.Windows.Forms.Button();
            this.preview = new System.Windows.Forms.PictureBox();
            this.previewCheckBox = new System.Windows.Forms.CheckBox();
            this.Time = new System.Windows.Forms.Label();
            this.timeTextBox = new System.Windows.Forms.TextBox();
            this.messageTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.gpsCom = new System.Windows.Forms.TextBox();
            this.headsetAudioCheckBox = new System.Windows.Forms.CheckBox();
            this.gpsCheckBox = new System.Windows.Forms.CheckBox();
            this.rawDepthCheckBox = new System.Windows.Forms.CheckBox();
            this.phidgetsCheckBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.preview)).BeginInit();
            this.SuspendLayout();
            // 
            // StartButton
            // 
            this.StartButton.Location = new System.Drawing.Point(472, 244);
            this.StartButton.Name = "StartButton";
            this.StartButton.Size = new System.Drawing.Size(109, 44);
            this.StartButton.TabIndex = 0;
            this.StartButton.Text = "Start";
            this.StartButton.UseVisualStyleBackColor = true;
            this.StartButton.Click += new System.EventHandler(this.StartButton_Click);
            // 
            // preview
            // 
            this.preview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.preview.Location = new System.Drawing.Point(23, 12);
            this.preview.Name = "preview";
            this.preview.Size = new System.Drawing.Size(320, 240);
            this.preview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.preview.TabIndex = 4;
            this.preview.TabStop = false;
            // 
            // previewCheckBox
            // 
            this.previewCheckBox.AutoSize = true;
            this.previewCheckBox.Location = new System.Drawing.Point(267, 271);
            this.previewCheckBox.Name = "previewCheckBox";
            this.previewCheckBox.Size = new System.Drawing.Size(76, 17);
            this.previewCheckBox.TabIndex = 5;
            this.previewCheckBox.Text = "PREVIEW";
            this.previewCheckBox.UseVisualStyleBackColor = true;
            // 
            // Time
            // 
            this.Time.AutoSize = true;
            this.Time.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Time.Location = new System.Drawing.Point(377, 106);
            this.Time.Name = "Time";
            this.Time.Size = new System.Drawing.Size(74, 31);
            this.Time.TabIndex = 6;
            this.Time.Text = "Time";
            // 
            // timeTextBox
            // 
            this.timeTextBox.BackColor = System.Drawing.Color.White;
            this.timeTextBox.Location = new System.Drawing.Point(472, 106);
            this.timeTextBox.Multiline = true;
            this.timeTextBox.Name = "timeTextBox";
            this.timeTextBox.Size = new System.Drawing.Size(109, 45);
            this.timeTextBox.TabIndex = 7;
            this.timeTextBox.Text = "30";
            // 
            // messageTextBox
            // 
            this.messageTextBox.Location = new System.Drawing.Point(370, 12);
            this.messageTextBox.Multiline = true;
            this.messageTextBox.Name = "messageTextBox";
            this.messageTextBox.ReadOnly = true;
            this.messageTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.messageTextBox.Size = new System.Drawing.Size(242, 71);
            this.messageTextBox.TabIndex = 8;
            this.messageTextBox.Text = "Message TextBox";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(380, 171);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "GPS COM";
            // 
            // gpsCom
            // 
            this.gpsCom.Location = new System.Drawing.Point(472, 171);
            this.gpsCom.Name = "gpsCom";
            this.gpsCom.Size = new System.Drawing.Size(109, 20);
            this.gpsCom.TabIndex = 10;
            this.gpsCom.Text = "3";
            // 
            // headsetAudioCheckBox
            // 
            this.headsetAudioCheckBox.AutoSize = true;
            this.headsetAudioCheckBox.Location = new System.Drawing.Point(370, 225);
            this.headsetAudioCheckBox.Name = "headsetAudioCheckBox";
            this.headsetAudioCheckBox.Size = new System.Drawing.Size(96, 17);
            this.headsetAudioCheckBox.TabIndex = 11;
            this.headsetAudioCheckBox.Text = "Headset Audio";
            this.headsetAudioCheckBox.UseVisualStyleBackColor = true;
            // 
            // gpsCheckBox
            // 
            this.gpsCheckBox.AutoSize = true;
            this.gpsCheckBox.Location = new System.Drawing.Point(370, 248);
            this.gpsCheckBox.Name = "gpsCheckBox";
            this.gpsCheckBox.Size = new System.Drawing.Size(48, 17);
            this.gpsCheckBox.TabIndex = 12;
            this.gpsCheckBox.Text = "GPS";
            this.gpsCheckBox.UseVisualStyleBackColor = true;
            // 
            // rawDepthCheckBox
            // 
            this.rawDepthCheckBox.AutoSize = true;
            this.rawDepthCheckBox.Checked = true;
            this.rawDepthCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.rawDepthCheckBox.Location = new System.Drawing.Point(370, 271);
            this.rawDepthCheckBox.Name = "rawDepthCheckBox";
            this.rawDepthCheckBox.Size = new System.Drawing.Size(80, 17);
            this.rawDepthCheckBox.TabIndex = 13;
            this.rawDepthCheckBox.Text = "Raw Depth";
            this.rawDepthCheckBox.UseVisualStyleBackColor = true;
            // 
            // phidgetsCheckBox
            // 
            this.phidgetsCheckBox.AutoSize = true;
            this.phidgetsCheckBox.Location = new System.Drawing.Point(370, 202);
            this.phidgetsCheckBox.Name = "phidgetsCheckBox";
            this.phidgetsCheckBox.Size = new System.Drawing.Size(67, 17);
            this.phidgetsCheckBox.TabIndex = 14;
            this.phidgetsCheckBox.Text = "Phidgets";
            this.phidgetsCheckBox.UseVisualStyleBackColor = true;
            // 
            // DataCollectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(637, 300);
            this.Controls.Add(this.phidgetsCheckBox);
            this.Controls.Add(this.rawDepthCheckBox);
            this.Controls.Add(this.gpsCheckBox);
            this.Controls.Add(this.headsetAudioCheckBox);
            this.Controls.Add(this.gpsCom);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.messageTextBox);
            this.Controls.Add(this.timeTextBox);
            this.Controls.Add(this.Time);
            this.Controls.Add(this.previewCheckBox);
            this.Controls.Add(this.preview);
            this.Controls.Add(this.StartButton);
            this.Name = "DataCollectForm";
            this.Text = "DataCollect";
            ((System.ComponentModel.ISupportInitialize)(this.preview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button StartButton;
        private System.Windows.Forms.PictureBox preview;
        private System.Windows.Forms.CheckBox previewCheckBox;
        private System.Windows.Forms.Label Time;
        private System.Windows.Forms.TextBox timeTextBox;
        private System.Windows.Forms.TextBox messageTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox gpsCom;
        private System.Windows.Forms.CheckBox headsetAudioCheckBox;
        private System.Windows.Forms.CheckBox gpsCheckBox;
        private System.Windows.Forms.CheckBox rawDepthCheckBox;
        private System.Windows.Forms.CheckBox phidgetsCheckBox;
    }
}

