namespace NateW.Ssm.ApplicationLogic
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
            this.tabs = new System.Windows.Forms.TabControl();
            this.controlTab = new System.Windows.Forms.TabPage();
            this.loggingModeGroupBox = new System.Windows.Forms.GroupBox();
            this.logAlways = new System.Windows.Forms.RadioButton();
            this.logOff = new System.Windows.Forms.RadioButton();
            this.logFullThrottle = new System.Windows.Forms.RadioButton();
            this.logClosedLoop = new System.Windows.Forms.RadioButton();
            this.logOpenLoop = new System.Windows.Forms.RadioButton();
            this.logDefogger = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.folderButton = new System.Windows.Forms.Button();
            this.folderLabel = new System.Windows.Forms.Label();
            this.openLogFolderButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.plxSerialPorts = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ssmSerialPorts = new System.Windows.Forms.ListBox();
            this.ecuIdentifierLabel = new System.Windows.Forms.Label();
            this.connectButton = new System.Windows.Forms.Button();
            this.profileTab = new System.Windows.Forms.TabPage();
            this.removeButton = new System.Windows.Forms.Button();
            this.newButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.profiles = new System.Windows.Forms.ListBox();
            this.saveAsButton = new System.Windows.Forms.Button();
            this.openButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.parametersTab = new System.Windows.Forms.TabPage();
            this.parameterGrid = new System.Windows.Forms.DataGridView();
            this.ParamEnabled = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ParamName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ParamUnits = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.dashboardTab = new System.Windows.Forms.TabPage();
            this.canvas = new System.Windows.Forms.Label();
            this.statusTab = new System.Windows.Forms.TabPage();
            this.statusText = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tabs.SuspendLayout();
            this.controlTab.SuspendLayout();
            this.loggingModeGroupBox.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.profileTab.SuspendLayout();
            this.parametersTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.parameterGrid)).BeginInit();
            this.dashboardTab.SuspendLayout();
            this.statusTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabs
            // 
            this.tabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabs.Controls.Add(this.controlTab);
            this.tabs.Controls.Add(this.profileTab);
            this.tabs.Controls.Add(this.parametersTab);
            this.tabs.Controls.Add(this.dashboardTab);
            this.tabs.Controls.Add(this.statusTab);
            this.tabs.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabs.Location = new System.Drawing.Point(2, 2);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(684, 434);
            this.tabs.TabIndex = 0;
            // 
            // controlTab
            // 
            this.controlTab.Controls.Add(this.loggingModeGroupBox);
            this.controlTab.Controls.Add(this.groupBox2);
            this.controlTab.Controls.Add(this.groupBox1);
            this.controlTab.Location = new System.Drawing.Point(4, 29);
            this.controlTab.Name = "controlTab";
            this.controlTab.Padding = new System.Windows.Forms.Padding(3);
            this.controlTab.Size = new System.Drawing.Size(676, 401);
            this.controlTab.TabIndex = 0;
            this.controlTab.Text = "Settings";
            this.controlTab.UseVisualStyleBackColor = true;
            // 
            // loggingModeGroupBox
            // 
            this.loggingModeGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.loggingModeGroupBox.Controls.Add(this.logAlways);
            this.loggingModeGroupBox.Controls.Add(this.logOff);
            this.loggingModeGroupBox.Controls.Add(this.logFullThrottle);
            this.loggingModeGroupBox.Controls.Add(this.logClosedLoop);
            this.loggingModeGroupBox.Controls.Add(this.logOpenLoop);
            this.loggingModeGroupBox.Controls.Add(this.logDefogger);
            this.loggingModeGroupBox.Location = new System.Drawing.Point(384, 6);
            this.loggingModeGroupBox.Name = "loggingModeGroupBox";
            this.loggingModeGroupBox.Size = new System.Drawing.Size(284, 274);
            this.loggingModeGroupBox.TabIndex = 15;
            this.loggingModeGroupBox.TabStop = false;
            this.loggingModeGroupBox.Text = "Logging Control";
            // 
            // logAlways
            // 
            this.logAlways.AutoSize = true;
            this.logAlways.Location = new System.Drawing.Point(17, 88);
            this.logAlways.Name = "logAlways";
            this.logAlways.Size = new System.Drawing.Size(137, 24);
            this.logAlways.TabIndex = 31;
            this.logAlways.Text = "&Always Logging";
            this.logAlways.UseVisualStyleBackColor = true;
            this.logAlways.CheckedChanged += new System.EventHandler(this.logAlways_CheckedChanged);
            // 
            // logOff
            // 
            this.logOff.AutoSize = true;
            this.logOff.Location = new System.Drawing.Point(17, 58);
            this.logOff.Name = "logOff";
            this.logOff.Size = new System.Drawing.Size(96, 24);
            this.logOff.TabIndex = 30;
            this.logOff.Text = "&View Only";
            this.logOff.UseVisualStyleBackColor = true;
            this.logOff.CheckedChanged += new System.EventHandler(this.logOff_CheckedChanged);
            // 
            // logFullThrottle
            // 
            this.logFullThrottle.AutoSize = true;
            this.logFullThrottle.Enabled = false;
            this.logFullThrottle.Location = new System.Drawing.Point(17, 148);
            this.logFullThrottle.Name = "logFullThrottle";
            this.logFullThrottle.Size = new System.Drawing.Size(110, 24);
            this.logFullThrottle.TabIndex = 35;
            this.logFullThrottle.Text = "Full &Throttle";
            this.logFullThrottle.UseVisualStyleBackColor = true;
            this.logFullThrottle.CheckedChanged += new System.EventHandler(this.logFullThrottle_CheckedChanged);
            // 
            // logClosedLoop
            // 
            this.logClosedLoop.AutoSize = true;
            this.logClosedLoop.Enabled = false;
            this.logClosedLoop.Location = new System.Drawing.Point(17, 215);
            this.logClosedLoop.Name = "logClosedLoop";
            this.logClosedLoop.Size = new System.Drawing.Size(116, 24);
            this.logClosedLoop.TabIndex = 34;
            this.logClosedLoop.Text = "Closed &Loop";
            this.logClosedLoop.UseVisualStyleBackColor = true;
            this.logClosedLoop.Visible = false;
            this.logClosedLoop.CheckedChanged += new System.EventHandler(this.logClosedLoop_CheckedChanged);
            // 
            // logOpenLoop
            // 
            this.logOpenLoop.AutoSize = true;
            this.logOpenLoop.Enabled = false;
            this.logOpenLoop.Location = new System.Drawing.Point(17, 185);
            this.logOpenLoop.Name = "logOpenLoop";
            this.logOpenLoop.Size = new System.Drawing.Size(106, 24);
            this.logOpenLoop.TabIndex = 33;
            this.logOpenLoop.Text = "Open &Loop";
            this.logOpenLoop.UseVisualStyleBackColor = true;
            this.logOpenLoop.Visible = false;
            this.logOpenLoop.CheckedChanged += new System.EventHandler(this.logOpenLoop_CheckedChanged);
            // 
            // logDefogger
            // 
            this.logDefogger.AutoSize = true;
            this.logDefogger.Checked = true;
            this.logDefogger.Enabled = false;
            this.logDefogger.Location = new System.Drawing.Point(17, 118);
            this.logDefogger.Name = "logDefogger";
            this.logDefogger.Size = new System.Drawing.Size(94, 24);
            this.logDefogger.TabIndex = 32;
            this.logDefogger.TabStop = true;
            this.logDefogger.Text = "&Defogger";
            this.logDefogger.UseVisualStyleBackColor = true;
            this.logDefogger.CheckedChanged += new System.EventHandler(this.logDefogger_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.folderButton);
            this.groupBox2.Controls.Add(this.folderLabel);
            this.groupBox2.Controls.Add(this.openLogFolderButton);
            this.groupBox2.Location = new System.Drawing.Point(7, 286);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(661, 107);
            this.groupBox2.TabIndex = 14;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Log Directory";
            // 
            // folderButton
            // 
            this.folderButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.folderButton.Location = new System.Drawing.Point(6, 53);
            this.folderButton.Name = "folderButton";
            this.folderButton.Size = new System.Drawing.Size(168, 38);
            this.folderButton.TabIndex = 11;
            this.folderButton.Text = "Set &Folder";
            this.folderButton.UseVisualStyleBackColor = true;
            this.folderButton.Click += new System.EventHandler(this.folderButton_Click);
            // 
            // folderLabel
            // 
            this.folderLabel.AutoSize = true;
            this.folderLabel.Location = new System.Drawing.Point(6, 22);
            this.folderLabel.Name = "folderLabel";
            this.folderLabel.Size = new System.Drawing.Size(156, 20);
            this.folderLabel.TabIndex = 10;
            this.folderLabel.Text = "Logs will be saved in:";
            // 
            // openLogFolderButton
            // 
            this.openLogFolderButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.openLogFolderButton.Location = new System.Drawing.Point(185, 53);
            this.openLogFolderButton.Name = "openLogFolderButton";
            this.openLogFolderButton.Size = new System.Drawing.Size(180, 38);
            this.openLogFolderButton.TabIndex = 12;
            this.openLogFolderButton.Text = "&Open Log Folder";
            this.openLogFolderButton.UseVisualStyleBackColor = true;
            this.openLogFolderButton.Click += new System.EventHandler(this.openLogFolderButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.plxSerialPorts);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.ssmSerialPorts);
            this.groupBox1.Controls.Add(this.ecuIdentifierLabel);
            this.groupBox1.Controls.Add(this.connectButton);
            this.groupBox1.Location = new System.Drawing.Point(7, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(371, 274);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Connection";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(181, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 20);
            this.label4.TabIndex = 9;
            this.label4.Text = "&PLX Port";
            // 
            // plxSerialPorts
            // 
            this.plxSerialPorts.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.plxSerialPorts.FormattingEnabled = true;
            this.plxSerialPorts.ItemHeight = 20;
            this.plxSerialPorts.Location = new System.Drawing.Point(185, 48);
            this.plxSerialPorts.Name = "plxSerialPorts";
            this.plxSerialPorts.Size = new System.Drawing.Size(180, 104);
            this.plxSerialPorts.TabIndex = 8;
            this.plxSerialPorts.SelectedIndexChanged += new System.EventHandler(this.plxSerialPorts_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "&SSM Port";
            // 
            // ssmSerialPorts
            // 
            this.ssmSerialPorts.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.ssmSerialPorts.FormattingEnabled = true;
            this.ssmSerialPorts.ItemHeight = 20;
            this.ssmSerialPorts.Location = new System.Drawing.Point(6, 48);
            this.ssmSerialPorts.Name = "ssmSerialPorts";
            this.ssmSerialPorts.Size = new System.Drawing.Size(168, 104);
            this.ssmSerialPorts.TabIndex = 3;
            this.ssmSerialPorts.SelectedIndexChanged += new System.EventHandler(this.ssmSerialPorts_SelectedIndexChanged);
            // 
            // ecuIdentifierLabel
            // 
            this.ecuIdentifierLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ecuIdentifierLabel.AutoSize = true;
            this.ecuIdentifierLabel.Location = new System.Drawing.Point(2, 207);
            this.ecuIdentifierLabel.Name = "ecuIdentifierLabel";
            this.ecuIdentifierLabel.Size = new System.Drawing.Size(117, 20);
            this.ecuIdentifierLabel.TabIndex = 6;
            this.ecuIdentifierLabel.Text = "Not connected.";
            // 
            // connectButton
            // 
            this.connectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.connectButton.Location = new System.Drawing.Point(6, 230);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(168, 38);
            this.connectButton.TabIndex = 7;
            this.connectButton.Text = "&Reconnect";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // profileTab
            // 
            this.profileTab.Controls.Add(this.removeButton);
            this.profileTab.Controls.Add(this.newButton);
            this.profileTab.Controls.Add(this.saveButton);
            this.profileTab.Controls.Add(this.profiles);
            this.profileTab.Controls.Add(this.saveAsButton);
            this.profileTab.Controls.Add(this.openButton);
            this.profileTab.Controls.Add(this.label2);
            this.profileTab.Location = new System.Drawing.Point(4, 29);
            this.profileTab.Name = "profileTab";
            this.profileTab.Padding = new System.Windows.Forms.Padding(3);
            this.profileTab.Size = new System.Drawing.Size(676, 401);
            this.profileTab.TabIndex = 3;
            this.profileTab.Text = "Profile";
            this.profileTab.UseVisualStyleBackColor = true;
            // 
            // removeButton
            // 
            this.removeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.removeButton.Location = new System.Drawing.Point(192, 350);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(180, 38);
            this.removeButton.TabIndex = 16;
            this.removeButton.Text = "&Remove From List";
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // newButton
            // 
            this.newButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.newButton.Location = new System.Drawing.Point(6, 299);
            this.newButton.Name = "newButton";
            this.newButton.Size = new System.Drawing.Size(180, 38);
            this.newButton.TabIndex = 12;
            this.newButton.Text = "&New Profile";
            this.newButton.UseVisualStyleBackColor = true;
            this.newButton.Click += new System.EventHandler(this.newButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.saveButton.Enabled = false;
            this.saveButton.Location = new System.Drawing.Point(378, 350);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(180, 38);
            this.saveButton.TabIndex = 13;
            this.saveButton.Text = "&Save Profile";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // profiles
            // 
            this.profiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.profiles.FormattingEnabled = true;
            this.profiles.ItemHeight = 20;
            this.profiles.Location = new System.Drawing.Point(6, 46);
            this.profiles.Name = "profiles";
            this.profiles.Size = new System.Drawing.Size(662, 224);
            this.profiles.TabIndex = 11;
            this.profiles.SelectedIndexChanged += new System.EventHandler(this.profiles_SelectedIndexChanged);
            // 
            // saveAsButton
            // 
            this.saveAsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.saveAsButton.Enabled = false;
            this.saveAsButton.Location = new System.Drawing.Point(378, 306);
            this.saveAsButton.Name = "saveAsButton";
            this.saveAsButton.Size = new System.Drawing.Size(180, 38);
            this.saveAsButton.TabIndex = 15;
            this.saveAsButton.Text = "Save Profile &As...";
            this.saveAsButton.UseVisualStyleBackColor = true;
            this.saveAsButton.Click += new System.EventHandler(this.saveAsButton_Click);
            // 
            // openButton
            // 
            this.openButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.openButton.Location = new System.Drawing.Point(6, 350);
            this.openButton.Name = "openButton";
            this.openButton.Size = new System.Drawing.Size(180, 38);
            this.openButton.TabIndex = 14;
            this.openButton.Text = "&Open Profile...";
            this.openButton.UseVisualStyleBackColor = true;
            this.openButton.Click += new System.EventHandler(this.openButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(120, 20);
            this.label2.TabIndex = 10;
            this.label2.Text = "Selected &Profile";
            // 
            // parametersTab
            // 
            this.parametersTab.Controls.Add(this.parameterGrid);
            this.parametersTab.Location = new System.Drawing.Point(4, 29);
            this.parametersTab.Name = "parametersTab";
            this.parametersTab.Padding = new System.Windows.Forms.Padding(3);
            this.parametersTab.Size = new System.Drawing.Size(676, 401);
            this.parametersTab.TabIndex = 2;
            this.parametersTab.Text = "Parameters";
            this.parametersTab.UseVisualStyleBackColor = true;
            // 
            // parameterGrid
            // 
            this.parameterGrid.AllowUserToAddRows = false;
            this.parameterGrid.AllowUserToDeleteRows = false;
            this.parameterGrid.AllowUserToResizeColumns = false;
            this.parameterGrid.AllowUserToResizeRows = false;
            this.parameterGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.parameterGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.parameterGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ParamEnabled,
            this.ParamName,
            this.ParamUnits});
            this.parameterGrid.Location = new System.Drawing.Point(9, 6);
            this.parameterGrid.Name = "parameterGrid";
            this.parameterGrid.RowHeadersVisible = false;
            this.parameterGrid.RowTemplate.Height = 30;
            this.parameterGrid.ShowEditingIcon = false;
            this.parameterGrid.Size = new System.Drawing.Size(659, 387);
            this.parameterGrid.TabIndex = 16;
            this.parameterGrid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.parameterGrid_CellValueChanged);
            // 
            // ParamEnabled
            // 
            this.ParamEnabled.FillWeight = 1F;
            this.ParamEnabled.Frozen = true;
            this.ParamEnabled.HeaderText = "Enabled";
            this.ParamEnabled.MinimumWidth = 20;
            this.ParamEnabled.Name = "ParamEnabled";
            this.ParamEnabled.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.ParamEnabled.Width = 75;
            // 
            // ParamName
            // 
            this.ParamName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ParamName.HeaderText = "Name";
            this.ParamName.Name = "ParamName";
            this.ParamName.ReadOnly = true;
            // 
            // ParamUnits
            // 
            this.ParamUnits.FillWeight = 1F;
            this.ParamUnits.HeaderText = "Units";
            this.ParamUnits.MinimumWidth = 50;
            this.ParamUnits.Name = "ParamUnits";
            this.ParamUnits.Width = 150;
            // 
            // dashboardTab
            // 
            this.dashboardTab.Controls.Add(this.canvas);
            this.dashboardTab.Location = new System.Drawing.Point(4, 29);
            this.dashboardTab.Name = "dashboardTab";
            this.dashboardTab.Padding = new System.Windows.Forms.Padding(3);
            this.dashboardTab.Size = new System.Drawing.Size(676, 401);
            this.dashboardTab.TabIndex = 1;
            this.dashboardTab.Text = "Display";
            this.dashboardTab.UseVisualStyleBackColor = true;
            // 
            // canvas
            // 
            this.canvas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.canvas.Location = new System.Drawing.Point(3, 3);
            this.canvas.Name = "canvas";
            this.canvas.Size = new System.Drawing.Size(670, 395);
            this.canvas.TabIndex = 0;
            // 
            // statusTab
            // 
            this.statusTab.Controls.Add(this.statusText);
            this.statusTab.Controls.Add(this.label3);
            this.statusTab.Location = new System.Drawing.Point(4, 29);
            this.statusTab.Name = "statusTab";
            this.statusTab.Size = new System.Drawing.Size(676, 401);
            this.statusTab.TabIndex = 4;
            this.statusTab.Text = "Debug";
            this.statusTab.UseVisualStyleBackColor = true;
            // 
            // statusText
            // 
            this.statusText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.statusText.Location = new System.Drawing.Point(6, 34);
            this.statusText.Multiline = true;
            this.statusText.Name = "statusText";
            this.statusText.ReadOnly = true;
            this.statusText.Size = new System.Drawing.Size(662, 359);
            this.statusText.TabIndex = 16;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 11);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(138, 20);
            this.label3.TabIndex = 15;
            this.label3.Text = "Debug messages:";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(686, 436);
            this.Controls.Add(this.tabs);
            this.Name = "MainForm";
            this.Text = "Lumberjack";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.tabs.ResumeLayout(false);
            this.controlTab.ResumeLayout(false);
            this.loggingModeGroupBox.ResumeLayout(false);
            this.loggingModeGroupBox.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.profileTab.ResumeLayout(false);
            this.profileTab.PerformLayout();
            this.parametersTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.parameterGrid)).EndInit();
            this.dashboardTab.ResumeLayout(false);
            this.statusTab.ResumeLayout(false);
            this.statusTab.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.TabPage controlTab;
        private System.Windows.Forms.TabPage dashboardTab;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label ecuIdentifierLabel;
        private System.Windows.Forms.TabPage parametersTab;
        private System.Windows.Forms.DataGridView parameterGrid;
        private System.Windows.Forms.Label canvas;
        private System.Windows.Forms.ListBox ssmSerialPorts;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.Button folderButton;
        private System.Windows.Forms.Label folderLabel;
        private System.Windows.Forms.Button openLogFolderButton;
        private System.Windows.Forms.TabPage profileTab;
        private System.Windows.Forms.Button newButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.ListBox profiles;
        private System.Windows.Forms.Button saveAsButton;
        private System.Windows.Forms.Button openButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TabPage statusTab;
        private System.Windows.Forms.TextBox statusText;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ParamEnabled;
        private System.Windows.Forms.DataGridViewTextBoxColumn ParamName;
        private System.Windows.Forms.DataGridViewComboBoxColumn ParamUnits;
        private System.Windows.Forms.GroupBox loggingModeGroupBox;
        private System.Windows.Forms.RadioButton logAlways;
        private System.Windows.Forms.RadioButton logOff;
        private System.Windows.Forms.RadioButton logFullThrottle;
        private System.Windows.Forms.RadioButton logClosedLoop;
        private System.Windows.Forms.RadioButton logOpenLoop;
        private System.Windows.Forms.RadioButton logDefogger;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListBox plxSerialPorts;
        private System.Windows.Forms.Button removeButton;
    }
}

