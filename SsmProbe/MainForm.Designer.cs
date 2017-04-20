namespace SsmProbe
{
    partial class MainForm
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
            this.payloadLabel = new System.Windows.Forms.Label();
            this.payload = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.commandByte = new System.Windows.Forms.ComboBox();
            this.testButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.copyButton = new System.Windows.Forms.Button();
            this.helpButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.serialPortList = new System.Windows.Forms.ComboBox();
            this.bitRates = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.device = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.extraByte = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.scanButton = new System.Windows.Forms.Button();
            this.summary = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // payloadLabel
            // 
            this.payloadLabel.AutoSize = true;
            this.payloadLabel.Location = new System.Drawing.Point(447, 80);
            this.payloadLabel.Name = "payloadLabel";
            this.payloadLabel.Size = new System.Drawing.Size(77, 13);
            this.payloadLabel.TabIndex = 0;
            this.payloadLabel.Text = "&Payload Bytes:";
            // 
            // payload
            // 
            this.payload.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.payload.FormattingEnabled = true;
            this.payload.Location = new System.Drawing.Point(450, 100);
            this.payload.Name = "payload";
            this.payload.Size = new System.Drawing.Size(322, 21);
            this.payload.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(155, 80);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Command &Byte";
            // 
            // commandByte
            // 
            this.commandByte.FormattingEnabled = true;
            this.commandByte.Location = new System.Drawing.Point(158, 100);
            this.commandByte.Name = "commandByte";
            this.commandByte.Size = new System.Drawing.Size(140, 21);
            this.commandByte.TabIndex = 4;
            // 
            // testButton
            // 
            this.testButton.Location = new System.Drawing.Point(12, 127);
            this.testButton.Name = "testButton";
            this.testButton.Size = new System.Drawing.Size(139, 23);
            this.testButton.TabIndex = 6;
            this.testButton.Text = "&Test";
            this.testButton.UseVisualStyleBackColor = true;
            this.testButton.Click += new System.EventHandler(this.testButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(450, 127);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(140, 23);
            this.saveButton.TabIndex = 7;
            this.saveButton.Text = "&Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Visible = false;
            // 
            // copyButton
            // 
            this.copyButton.Location = new System.Drawing.Point(158, 127);
            this.copyButton.Name = "copyButton";
            this.copyButton.Size = new System.Drawing.Size(140, 23);
            this.copyButton.TabIndex = 8;
            this.copyButton.Text = "&Copy";
            this.copyButton.UseVisualStyleBackColor = true;
            this.copyButton.Click += new System.EventHandler(this.copyButton_Click);
            // 
            // helpButton
            // 
            this.helpButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.helpButton.Location = new System.Drawing.Point(632, 127);
            this.helpButton.Name = "helpButton";
            this.helpButton.Size = new System.Drawing.Size(140, 23);
            this.helpButton.TabIndex = 10;
            this.helpButton.Text = "&Help";
            this.helpButton.UseVisualStyleBackColor = true;
            this.helpButton.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Port";
            // 
            // serialPortList
            // 
            this.serialPortList.FormattingEnabled = true;
            this.serialPortList.Location = new System.Drawing.Point(11, 25);
            this.serialPortList.Name = "serialPortList";
            this.serialPortList.Size = new System.Drawing.Size(140, 21);
            this.serialPortList.TabIndex = 12;
            this.serialPortList.SelectedIndexChanged += new System.EventHandler(this.serialPortList_SelectedIndexChanged);
            // 
            // bitRates
            // 
            this.bitRates.FormattingEnabled = true;
            this.bitRates.Location = new System.Drawing.Point(158, 25);
            this.bitRates.Name = "bitRates";
            this.bitRates.Size = new System.Drawing.Size(140, 21);
            this.bitRates.TabIndex = 14;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(155, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Baud Rate";
            // 
            // device
            // 
            this.device.FormattingEnabled = true;
            this.device.Location = new System.Drawing.Point(12, 100);
            this.device.Name = "device";
            this.device.Size = new System.Drawing.Size(140, 21);
            this.device.TabIndex = 16;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 80);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "&Target Device";
            // 
            // extraByte
            // 
            this.extraByte.FormattingEnabled = true;
            this.extraByte.Location = new System.Drawing.Point(304, 100);
            this.extraByte.Name = "extraByte";
            this.extraByte.Size = new System.Drawing.Size(140, 21);
            this.extraByte.TabIndex = 18;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(301, 80);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(83, 13);
            this.label5.TabIndex = 17;
            this.label5.Text = "Add &Extra Byte?";
            // 
            // scanButton
            // 
            this.scanButton.Location = new System.Drawing.Point(304, 127);
            this.scanButton.Name = "scanButton";
            this.scanButton.Size = new System.Drawing.Size(140, 23);
            this.scanButton.TabIndex = 19;
            this.scanButton.Text = "Scan &All Devices";
            this.scanButton.UseVisualStyleBackColor = true;
            this.scanButton.Click += new System.EventHandler(this.scanButton_Click);
            // 
            // summary
            // 
            this.summary.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.summary.Location = new System.Drawing.Point(11, 171);
            this.summary.Multiline = true;
            this.summary.Name = "summary";
            this.summary.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.summary.Size = new System.Drawing.Size(761, 379);
            this.summary.TabIndex = 20;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.summary);
            this.Controls.Add(this.scanButton);
            this.Controls.Add(this.extraByte);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.device);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.bitRates);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.serialPortList);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.helpButton);
            this.Controls.Add(this.copyButton);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.testButton);
            this.Controls.Add(this.commandByte);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.payload);
            this.Controls.Add(this.payloadLabel);
            this.Name = "MainForm";
            this.Text = "SSM Probe";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label payloadLabel;
        private System.Windows.Forms.ComboBox payload;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox commandByte;
        private System.Windows.Forms.Button testButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button copyButton;
        private System.Windows.Forms.Button helpButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox serialPortList;
        private System.Windows.Forms.ComboBox bitRates;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox device;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox extraByte;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button scanButton;
        private System.Windows.Forms.TextBox summary;
    }
}

