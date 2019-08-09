namespace G_Sensor_FFT
{
    partial class MainForm
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && (components != null))
                {
                    components.Dispose();
                }
                base.Dispose(disposing);
            }
            catch { }
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btnCom = new System.Windows.Forms.Button();
            this.cbxCom = new System.Windows.Forms.ComboBox();
            this.lbMac = new System.Windows.Forms.ListBox();
            this.btnScan = new System.Windows.Forms.Button();
            this.zgcAxisX = new ZedGraph.ZedGraphControl();
            this.zgcAxisY = new ZedGraph.ZedGraphControl();
            this.zgcAxisZ = new ZedGraph.ZedGraphControl();
            this.zgcFftZ = new ZedGraph.ZedGraphControl();
            this.zgcFftY = new ZedGraph.ZedGraphControl();
            this.zgcFftX = new ZedGraph.ZedGraphControl();
            this.btnPause = new System.Windows.Forms.Button();
            this.btnLoadFile = new System.Windows.Forms.Button();
            this.ofdInput = new System.Windows.Forms.OpenFileDialog();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.btnPlayPause = new System.Windows.Forms.Button();
            this.tmrPaintLog = new System.Windows.Forms.Timer(this.components);
            this.btnSpeedController = new System.Windows.Forms.Button();
            this.btnRecord = new System.Windows.Forms.Button();
            this.btnAddMark = new System.Windows.Forms.Button();
            this.cbxTimeScale = new System.Windows.Forms.ComboBox();
            this.lblTimeScale = new System.Windows.Forms.Label();
            this.btnSetAlarm = new System.Windows.Forms.Button();
            this.lblWarnX = new G_Sensor_FFT.LabelWarn();
            this.lblWarnY = new G_Sensor_FFT.LabelWarn();
            this.lblWarnZ = new G_Sensor_FFT.LabelWarn();
            this.btnResetAlarm = new System.Windows.Forms.Button();
            this.lblCalMode = new System.Windows.Forms.Label();
            this.cbxCalMode = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // btnCom
            // 
            this.btnCom.Location = new System.Drawing.Point(9, 46);
            this.btnCom.Margin = new System.Windows.Forms.Padding(2);
            this.btnCom.Name = "btnCom";
            this.btnCom.Size = new System.Drawing.Size(130, 39);
            this.btnCom.TabIndex = 0;
            this.btnCom.Text = "Open Com";
            this.btnCom.UseVisualStyleBackColor = true;
            this.btnCom.Click += new System.EventHandler(this.btnOpenCom_Click);
            // 
            // cbxCom
            // 
            this.cbxCom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxCom.FormattingEnabled = true;
            this.cbxCom.Location = new System.Drawing.Point(9, 14);
            this.cbxCom.Margin = new System.Windows.Forms.Padding(2);
            this.cbxCom.Name = "cbxCom";
            this.cbxCom.Size = new System.Drawing.Size(132, 20);
            this.cbxCom.TabIndex = 1;
            // 
            // lbMac
            // 
            this.lbMac.FormattingEnabled = true;
            this.lbMac.ItemHeight = 12;
            this.lbMac.Location = new System.Drawing.Point(9, 152);
            this.lbMac.Margin = new System.Windows.Forms.Padding(2);
            this.lbMac.Name = "lbMac";
            this.lbMac.Size = new System.Drawing.Size(132, 64);
            this.lbMac.TabIndex = 2;
            this.lbMac.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lbMac_MouseDoubleClick);
            // 
            // btnScan
            // 
            this.btnScan.Location = new System.Drawing.Point(9, 100);
            this.btnScan.Margin = new System.Windows.Forms.Padding(2);
            this.btnScan.Name = "btnScan";
            this.btnScan.Size = new System.Drawing.Size(130, 39);
            this.btnScan.TabIndex = 3;
            this.btnScan.Text = "Scan";
            this.btnScan.UseVisualStyleBackColor = true;
            this.btnScan.Click += new System.EventHandler(this.btnScan_Click);
            // 
            // zgcAxisX
            // 
            this.zgcAxisX.Location = new System.Drawing.Point(153, 14);
            this.zgcAxisX.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.zgcAxisX.Name = "zgcAxisX";
            this.zgcAxisX.ScrollGrace = 0D;
            this.zgcAxisX.ScrollMaxX = 0D;
            this.zgcAxisX.ScrollMaxY = 0D;
            this.zgcAxisX.ScrollMaxY2 = 0D;
            this.zgcAxisX.ScrollMinX = 0D;
            this.zgcAxisX.ScrollMinY = 0D;
            this.zgcAxisX.ScrollMinY2 = 0D;
            this.zgcAxisX.Size = new System.Drawing.Size(569, 206);
            this.zgcAxisX.TabIndex = 4;
            // 
            // zgcAxisY
            // 
            this.zgcAxisY.Location = new System.Drawing.Point(153, 225);
            this.zgcAxisY.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.zgcAxisY.Name = "zgcAxisY";
            this.zgcAxisY.ScrollGrace = 0D;
            this.zgcAxisY.ScrollMaxX = 0D;
            this.zgcAxisY.ScrollMaxY = 0D;
            this.zgcAxisY.ScrollMaxY2 = 0D;
            this.zgcAxisY.ScrollMinX = 0D;
            this.zgcAxisY.ScrollMinY = 0D;
            this.zgcAxisY.ScrollMinY2 = 0D;
            this.zgcAxisY.Size = new System.Drawing.Size(569, 206);
            this.zgcAxisY.TabIndex = 5;
            // 
            // zgcAxisZ
            // 
            this.zgcAxisZ.Location = new System.Drawing.Point(153, 436);
            this.zgcAxisZ.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.zgcAxisZ.Name = "zgcAxisZ";
            this.zgcAxisZ.ScrollGrace = 0D;
            this.zgcAxisZ.ScrollMaxX = 0D;
            this.zgcAxisZ.ScrollMaxY = 0D;
            this.zgcAxisZ.ScrollMaxY2 = 0D;
            this.zgcAxisZ.ScrollMinX = 0D;
            this.zgcAxisZ.ScrollMinY = 0D;
            this.zgcAxisZ.ScrollMinY2 = 0D;
            this.zgcAxisZ.Size = new System.Drawing.Size(569, 206);
            this.zgcAxisZ.TabIndex = 6;
            // 
            // zgcFftZ
            // 
            this.zgcFftZ.Location = new System.Drawing.Point(756, 436);
            this.zgcFftZ.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.zgcFftZ.Name = "zgcFftZ";
            this.zgcFftZ.ScrollGrace = 0D;
            this.zgcFftZ.ScrollMaxX = 0D;
            this.zgcFftZ.ScrollMaxY = 0D;
            this.zgcFftZ.ScrollMaxY2 = 0D;
            this.zgcFftZ.ScrollMinX = 0D;
            this.zgcFftZ.ScrollMinY = 0D;
            this.zgcFftZ.ScrollMinY2 = 0D;
            this.zgcFftZ.Size = new System.Drawing.Size(385, 206);
            this.zgcFftZ.TabIndex = 9;
            // 
            // zgcFftY
            // 
            this.zgcFftY.Location = new System.Drawing.Point(756, 225);
            this.zgcFftY.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.zgcFftY.Name = "zgcFftY";
            this.zgcFftY.ScrollGrace = 0D;
            this.zgcFftY.ScrollMaxX = 0D;
            this.zgcFftY.ScrollMaxY = 0D;
            this.zgcFftY.ScrollMaxY2 = 0D;
            this.zgcFftY.ScrollMinX = 0D;
            this.zgcFftY.ScrollMinY = 0D;
            this.zgcFftY.ScrollMinY2 = 0D;
            this.zgcFftY.Size = new System.Drawing.Size(385, 206);
            this.zgcFftY.TabIndex = 8;
            // 
            // zgcFftX
            // 
            this.zgcFftX.Location = new System.Drawing.Point(756, 14);
            this.zgcFftX.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.zgcFftX.Name = "zgcFftX";
            this.zgcFftX.ScrollGrace = 0D;
            this.zgcFftX.ScrollMaxX = 0D;
            this.zgcFftX.ScrollMaxY = 0D;
            this.zgcFftX.ScrollMaxY2 = 0D;
            this.zgcFftX.ScrollMinX = 0D;
            this.zgcFftX.ScrollMinY = 0D;
            this.zgcFftX.ScrollMinY2 = 0D;
            this.zgcFftX.Size = new System.Drawing.Size(385, 206);
            this.zgcFftX.TabIndex = 7;
            // 
            // btnPause
            // 
            this.btnPause.Location = new System.Drawing.Point(9, 223);
            this.btnPause.Margin = new System.Windows.Forms.Padding(2);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(130, 39);
            this.btnPause.TabIndex = 10;
            this.btnPause.Text = "Pause";
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // btnLoadFile
            // 
            this.btnLoadFile.Location = new System.Drawing.Point(9, 515);
            this.btnLoadFile.Margin = new System.Windows.Forms.Padding(2);
            this.btnLoadFile.Name = "btnLoadFile";
            this.btnLoadFile.Size = new System.Drawing.Size(130, 39);
            this.btnLoadFile.TabIndex = 11;
            this.btnLoadFile.Text = "Load File";
            this.btnLoadFile.UseVisualStyleBackColor = true;
            this.btnLoadFile.Click += new System.EventHandler(this.btnLoadFile_Click);
            // 
            // ofdInput
            // 
            this.ofdInput.DefaultExt = "csv";
            this.ofdInput.FileName = "log";
            // 
            // btnPlayPause
            // 
            this.btnPlayPause.Location = new System.Drawing.Point(9, 559);
            this.btnPlayPause.Margin = new System.Windows.Forms.Padding(2);
            this.btnPlayPause.Name = "btnPlayPause";
            this.btnPlayPause.Size = new System.Drawing.Size(130, 39);
            this.btnPlayPause.TabIndex = 15;
            this.btnPlayPause.Text = "Play";
            this.btnPlayPause.UseVisualStyleBackColor = true;
            this.btnPlayPause.Click += new System.EventHandler(this.btnPlayPause_Click);
            // 
            // btnSpeedController
            // 
            this.btnSpeedController.Location = new System.Drawing.Point(9, 603);
            this.btnSpeedController.Margin = new System.Windows.Forms.Padding(2);
            this.btnSpeedController.Name = "btnSpeedController";
            this.btnSpeedController.Size = new System.Drawing.Size(130, 39);
            this.btnSpeedController.TabIndex = 18;
            this.btnSpeedController.Text = "1X";
            this.btnSpeedController.UseVisualStyleBackColor = true;
            this.btnSpeedController.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnSpeedController_MouseDown);
            // 
            // btnRecord
            // 
            this.btnRecord.Location = new System.Drawing.Point(9, 267);
            this.btnRecord.Margin = new System.Windows.Forms.Padding(2);
            this.btnRecord.Name = "btnRecord";
            this.btnRecord.Size = new System.Drawing.Size(130, 39);
            this.btnRecord.TabIndex = 19;
            this.btnRecord.Text = "Record";
            this.btnRecord.UseVisualStyleBackColor = true;
            this.btnRecord.Click += new System.EventHandler(this.btnRecord_Click);
            // 
            // btnAddMark
            // 
            this.btnAddMark.Location = new System.Drawing.Point(9, 311);
            this.btnAddMark.Margin = new System.Windows.Forms.Padding(2);
            this.btnAddMark.Name = "btnAddMark";
            this.btnAddMark.Size = new System.Drawing.Size(130, 39);
            this.btnAddMark.TabIndex = 20;
            this.btnAddMark.Text = "Mark";
            this.btnAddMark.UseVisualStyleBackColor = true;
            this.btnAddMark.Click += new System.EventHandler(this.btnAddMark_Click);
            // 
            // cbxTimeScale
            // 
            this.cbxTimeScale.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxTimeScale.FormattingEnabled = true;
            this.cbxTimeScale.Location = new System.Drawing.Point(9, 405);
            this.cbxTimeScale.Margin = new System.Windows.Forms.Padding(2);
            this.cbxTimeScale.Name = "cbxTimeScale";
            this.cbxTimeScale.Size = new System.Drawing.Size(130, 20);
            this.cbxTimeScale.TabIndex = 23;
            // 
            // lblTimeScale
            // 
            this.lblTimeScale.AutoSize = true;
            this.lblTimeScale.Location = new System.Drawing.Point(9, 390);
            this.lblTimeScale.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTimeScale.Name = "lblTimeScale";
            this.lblTimeScale.Size = new System.Drawing.Size(78, 12);
            this.lblTimeScale.TabIndex = 24;
            this.lblTimeScale.Text = "Time Scale(sec)";
            // 
            // btnSetAlarm
            // 
            this.btnSetAlarm.Location = new System.Drawing.Point(9, 428);
            this.btnSetAlarm.Margin = new System.Windows.Forms.Padding(2);
            this.btnSetAlarm.Name = "btnSetAlarm";
            this.btnSetAlarm.Size = new System.Drawing.Size(130, 39);
            this.btnSetAlarm.TabIndex = 25;
            this.btnSetAlarm.Text = "Set Alarm";
            this.btnSetAlarm.UseVisualStyleBackColor = true;
            this.btnSetAlarm.Click += new System.EventHandler(this.btnSetAlarm_Click);
            // 
            // lblWarnX
            // 
            this.lblWarnX.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.lblWarnX.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblWarnX.Location = new System.Drawing.Point(728, 16);
            this.lblWarnX.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblWarnX.Name = "lblWarnX";
            this.lblWarnX.Size = new System.Drawing.Size(24, 19);
            this.lblWarnX.TabIndex = 26;
            // 
            // lblWarnY
            // 
            this.lblWarnY.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.lblWarnY.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblWarnY.Location = new System.Drawing.Point(728, 225);
            this.lblWarnY.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblWarnY.Name = "lblWarnY";
            this.lblWarnY.Size = new System.Drawing.Size(24, 19);
            this.lblWarnY.TabIndex = 27;
            // 
            // lblWarnZ
            // 
            this.lblWarnZ.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.lblWarnZ.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblWarnZ.Location = new System.Drawing.Point(728, 436);
            this.lblWarnZ.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblWarnZ.Name = "lblWarnZ";
            this.lblWarnZ.Size = new System.Drawing.Size(24, 19);
            this.lblWarnZ.TabIndex = 28;
            // 
            // btnResetAlarm
            // 
            this.btnResetAlarm.Location = new System.Drawing.Point(9, 471);
            this.btnResetAlarm.Margin = new System.Windows.Forms.Padding(2);
            this.btnResetAlarm.Name = "btnResetAlarm";
            this.btnResetAlarm.Size = new System.Drawing.Size(130, 39);
            this.btnResetAlarm.TabIndex = 29;
            this.btnResetAlarm.Text = "Reset Alarm";
            this.btnResetAlarm.UseVisualStyleBackColor = true;
            this.btnResetAlarm.Click += new System.EventHandler(this.btnResetAlarm_Click);
            // 
            // lblCalMode
            // 
            this.lblCalMode.AutoSize = true;
            this.lblCalMode.Location = new System.Drawing.Point(9, 359);
            this.lblCalMode.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblCalMode.Name = "lblCalMode";
            this.lblCalMode.Size = new System.Drawing.Size(32, 12);
            this.lblCalMode.TabIndex = 31;
            this.lblCalMode.Text = "Mode";
            // 
            // cbxCalMode
            // 
            this.cbxCalMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxCalMode.FormattingEnabled = true;
            this.cbxCalMode.Items.AddRange(new object[] {
            "FFT",
            "Velocity",
            "Deplacement"});
            this.cbxCalMode.Location = new System.Drawing.Point(9, 374);
            this.cbxCalMode.Margin = new System.Windows.Forms.Padding(2);
            this.cbxCalMode.Name = "cbxCalMode";
            this.cbxCalMode.Size = new System.Drawing.Size(130, 20);
            this.cbxCalMode.TabIndex = 30;
            this.cbxCalMode.SelectedIndexChanged += new System.EventHandler(this.CbxCalMode_SelectedIndexChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1160, 670);
            this.Controls.Add(this.lblCalMode);
            this.Controls.Add(this.cbxCalMode);
            this.Controls.Add(this.btnResetAlarm);
            this.Controls.Add(this.lblWarnZ);
            this.Controls.Add(this.lblWarnY);
            this.Controls.Add(this.lblWarnX);
            this.Controls.Add(this.btnSetAlarm);
            this.Controls.Add(this.lblTimeScale);
            this.Controls.Add(this.cbxTimeScale);
            this.Controls.Add(this.btnAddMark);
            this.Controls.Add(this.btnRecord);
            this.Controls.Add(this.btnSpeedController);
            this.Controls.Add(this.btnPlayPause);
            this.Controls.Add(this.btnLoadFile);
            this.Controls.Add(this.btnPause);
            this.Controls.Add(this.zgcFftZ);
            this.Controls.Add(this.zgcFftY);
            this.Controls.Add(this.zgcFftX);
            this.Controls.Add(this.zgcAxisZ);
            this.Controls.Add(this.zgcAxisY);
            this.Controls.Add(this.zgcAxisX);
            this.Controls.Add(this.btnScan);
            this.Controls.Add(this.lbMac);
            this.Controls.Add(this.cbxCom);
            this.Controls.Add(this.btnCom);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCom;
        private System.Windows.Forms.ComboBox cbxCom;
        private System.Windows.Forms.ListBox lbMac;
        private System.Windows.Forms.Button btnScan;
        private ZedGraph.ZedGraphControl zgcAxisX;
        private ZedGraph.ZedGraphControl zgcAxisY;
        private ZedGraph.ZedGraphControl zgcAxisZ;
        private ZedGraph.ZedGraphControl zgcFftZ;
        private ZedGraph.ZedGraphControl zgcFftY;
        private ZedGraph.ZedGraphControl zgcFftX;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.Button btnLoadFile;
        private System.Windows.Forms.OpenFileDialog ofdInput;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Button btnPlayPause;
        private System.Windows.Forms.Timer tmrPaintLog;
        private System.Windows.Forms.Button btnSpeedController;
        private System.Windows.Forms.Button btnRecord;
        private System.Windows.Forms.Button btnAddMark;
        private System.Windows.Forms.ComboBox cbxTimeScale;
        private System.Windows.Forms.Label lblTimeScale;
        private System.Windows.Forms.Button btnSetAlarm;
        private LabelWarn lblWarnX;
        private LabelWarn lblWarnY;
        private LabelWarn lblWarnZ;
        private System.Windows.Forms.Button btnResetAlarm;
        private System.Windows.Forms.Label lblCalMode;
        private System.Windows.Forms.ComboBox cbxCalMode;
    }
}

