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
            this.lblShowRange = new System.Windows.Forms.Label();
            this.btnSetAlarm = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnCom
            // 
            this.btnCom.Location = new System.Drawing.Point(12, 57);
            this.btnCom.Name = "btnCom";
            this.btnCom.Size = new System.Drawing.Size(174, 49);
            this.btnCom.TabIndex = 0;
            this.btnCom.Text = "Open Com";
            this.btnCom.UseVisualStyleBackColor = true;
            this.btnCom.Click += new System.EventHandler(this.btnOpenCom_Click);
            // 
            // cbxCom
            // 
            this.cbxCom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxCom.FormattingEnabled = true;
            this.cbxCom.Location = new System.Drawing.Point(12, 17);
            this.cbxCom.Name = "cbxCom";
            this.cbxCom.Size = new System.Drawing.Size(174, 23);
            this.cbxCom.TabIndex = 1;
            // 
            // lbMac
            // 
            this.lbMac.FormattingEnabled = true;
            this.lbMac.ItemHeight = 15;
            this.lbMac.Location = new System.Drawing.Point(12, 230);
            this.lbMac.Name = "lbMac";
            this.lbMac.Size = new System.Drawing.Size(174, 79);
            this.lbMac.TabIndex = 2;
            this.lbMac.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lbMac_MouseDoubleClick);
            // 
            // btnScan
            // 
            this.btnScan.Location = new System.Drawing.Point(12, 165);
            this.btnScan.Name = "btnScan";
            this.btnScan.Size = new System.Drawing.Size(174, 49);
            this.btnScan.TabIndex = 3;
            this.btnScan.Text = "Scan";
            this.btnScan.UseVisualStyleBackColor = true;
            this.btnScan.Click += new System.EventHandler(this.btnScan_Click);
            // 
            // zgcAxisX
            // 
            this.zgcAxisX.Location = new System.Drawing.Point(204, 17);
            this.zgcAxisX.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.zgcAxisX.Name = "zgcAxisX";
            this.zgcAxisX.ScrollGrace = 0D;
            this.zgcAxisX.ScrollMaxX = 0D;
            this.zgcAxisX.ScrollMaxY = 0D;
            this.zgcAxisX.ScrollMaxY2 = 0D;
            this.zgcAxisX.ScrollMinX = 0D;
            this.zgcAxisX.ScrollMinY = 0D;
            this.zgcAxisX.ScrollMinY2 = 0D;
            this.zgcAxisX.Size = new System.Drawing.Size(759, 258);
            this.zgcAxisX.TabIndex = 4;
            // 
            // zgcAxisY
            // 
            this.zgcAxisY.Location = new System.Drawing.Point(204, 281);
            this.zgcAxisY.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.zgcAxisY.Name = "zgcAxisY";
            this.zgcAxisY.ScrollGrace = 0D;
            this.zgcAxisY.ScrollMaxX = 0D;
            this.zgcAxisY.ScrollMaxY = 0D;
            this.zgcAxisY.ScrollMaxY2 = 0D;
            this.zgcAxisY.ScrollMinX = 0D;
            this.zgcAxisY.ScrollMinY = 0D;
            this.zgcAxisY.ScrollMinY2 = 0D;
            this.zgcAxisY.Size = new System.Drawing.Size(759, 258);
            this.zgcAxisY.TabIndex = 5;
            // 
            // zgcAxisZ
            // 
            this.zgcAxisZ.Location = new System.Drawing.Point(204, 545);
            this.zgcAxisZ.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.zgcAxisZ.Name = "zgcAxisZ";
            this.zgcAxisZ.ScrollGrace = 0D;
            this.zgcAxisZ.ScrollMaxX = 0D;
            this.zgcAxisZ.ScrollMaxY = 0D;
            this.zgcAxisZ.ScrollMaxY2 = 0D;
            this.zgcAxisZ.ScrollMinX = 0D;
            this.zgcAxisZ.ScrollMinY = 0D;
            this.zgcAxisZ.ScrollMinY2 = 0D;
            this.zgcAxisZ.Size = new System.Drawing.Size(759, 258);
            this.zgcAxisZ.TabIndex = 6;
            // 
            // zgcFftZ
            // 
            this.zgcFftZ.Location = new System.Drawing.Point(1008, 545);
            this.zgcFftZ.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.zgcFftZ.Name = "zgcFftZ";
            this.zgcFftZ.ScrollGrace = 0D;
            this.zgcFftZ.ScrollMaxX = 0D;
            this.zgcFftZ.ScrollMaxY = 0D;
            this.zgcFftZ.ScrollMaxY2 = 0D;
            this.zgcFftZ.ScrollMinX = 0D;
            this.zgcFftZ.ScrollMinY = 0D;
            this.zgcFftZ.ScrollMinY2 = 0D;
            this.zgcFftZ.Size = new System.Drawing.Size(513, 258);
            this.zgcFftZ.TabIndex = 9;
            // 
            // zgcFftY
            // 
            this.zgcFftY.Location = new System.Drawing.Point(1008, 281);
            this.zgcFftY.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.zgcFftY.Name = "zgcFftY";
            this.zgcFftY.ScrollGrace = 0D;
            this.zgcFftY.ScrollMaxX = 0D;
            this.zgcFftY.ScrollMaxY = 0D;
            this.zgcFftY.ScrollMaxY2 = 0D;
            this.zgcFftY.ScrollMinX = 0D;
            this.zgcFftY.ScrollMinY = 0D;
            this.zgcFftY.ScrollMinY2 = 0D;
            this.zgcFftY.Size = new System.Drawing.Size(513, 258);
            this.zgcFftY.TabIndex = 8;
            // 
            // zgcFftX
            // 
            this.zgcFftX.Location = new System.Drawing.Point(1008, 17);
            this.zgcFftX.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.zgcFftX.Name = "zgcFftX";
            this.zgcFftX.ScrollGrace = 0D;
            this.zgcFftX.ScrollMaxX = 0D;
            this.zgcFftX.ScrollMaxY = 0D;
            this.zgcFftX.ScrollMaxY2 = 0D;
            this.zgcFftX.ScrollMinX = 0D;
            this.zgcFftX.ScrollMinY = 0D;
            this.zgcFftX.ScrollMinY2 = 0D;
            this.zgcFftX.Size = new System.Drawing.Size(513, 258);
            this.zgcFftX.TabIndex = 7;
            // 
            // btnPause
            // 
            this.btnPause.Location = new System.Drawing.Point(12, 328);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(174, 49);
            this.btnPause.TabIndex = 10;
            this.btnPause.Text = "Pause";
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // btnLoadFile
            // 
            this.btnLoadFile.Location = new System.Drawing.Point(12, 644);
            this.btnLoadFile.Name = "btnLoadFile";
            this.btnLoadFile.Size = new System.Drawing.Size(174, 49);
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
            this.btnPlayPause.Location = new System.Drawing.Point(12, 699);
            this.btnPlayPause.Name = "btnPlayPause";
            this.btnPlayPause.Size = new System.Drawing.Size(174, 49);
            this.btnPlayPause.TabIndex = 15;
            this.btnPlayPause.Text = "Play";
            this.btnPlayPause.UseVisualStyleBackColor = true;
            this.btnPlayPause.Click += new System.EventHandler(this.btnPlayPause_Click);
            // 
            // btnSpeedController
            // 
            this.btnSpeedController.Location = new System.Drawing.Point(12, 754);
            this.btnSpeedController.Name = "btnSpeedController";
            this.btnSpeedController.Size = new System.Drawing.Size(174, 49);
            this.btnSpeedController.TabIndex = 18;
            this.btnSpeedController.Text = "1X";
            this.btnSpeedController.UseVisualStyleBackColor = true;
            this.btnSpeedController.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnSpeedController_MouseDown);
            // 
            // btnRecord
            // 
            this.btnRecord.Location = new System.Drawing.Point(12, 383);
            this.btnRecord.Name = "btnRecord";
            this.btnRecord.Size = new System.Drawing.Size(174, 49);
            this.btnRecord.TabIndex = 19;
            this.btnRecord.Text = "Record";
            this.btnRecord.UseVisualStyleBackColor = true;
            this.btnRecord.Click += new System.EventHandler(this.btnRecord_Click);
            // 
            // btnAddMark
            // 
            this.btnAddMark.Location = new System.Drawing.Point(12, 438);
            this.btnAddMark.Name = "btnAddMark";
            this.btnAddMark.Size = new System.Drawing.Size(174, 49);
            this.btnAddMark.TabIndex = 20;
            this.btnAddMark.Text = "Mark";
            this.btnAddMark.UseVisualStyleBackColor = true;
            this.btnAddMark.Click += new System.EventHandler(this.btnAddMark_Click);
            // 
            // cbxTimeScale
            // 
            this.cbxTimeScale.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxTimeScale.FormattingEnabled = true;
            this.cbxTimeScale.Location = new System.Drawing.Point(12, 525);
            this.cbxTimeScale.Name = "cbxTimeScale";
            this.cbxTimeScale.Size = new System.Drawing.Size(174, 23);
            this.cbxTimeScale.TabIndex = 23;
            // 
            // lblShowRange
            // 
            this.lblShowRange.AutoSize = true;
            this.lblShowRange.Location = new System.Drawing.Point(12, 507);
            this.lblShowRange.Name = "lblShowRange";
            this.lblShowRange.Size = new System.Drawing.Size(103, 15);
            this.lblShowRange.TabIndex = 24;
            this.lblShowRange.Text = "Time Range(sec)";
            // 
            // btnSetAlarm
            // 
            this.btnSetAlarm.Location = new System.Drawing.Point(12, 554);
            this.btnSetAlarm.Name = "btnSetAlarm";
            this.btnSetAlarm.Size = new System.Drawing.Size(174, 49);
            this.btnSetAlarm.TabIndex = 25;
            this.btnSetAlarm.Text = "Set Alarm";
            this.btnSetAlarm.UseVisualStyleBackColor = true;
            this.btnSetAlarm.Click += new System.EventHandler(this.btnSetAlarm_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1547, 837);
            this.Controls.Add(this.btnSetAlarm);
            this.Controls.Add(this.lblShowRange);
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
        private System.Windows.Forms.Label lblShowRange;
        private System.Windows.Forms.Button btnSetAlarm;
    }
}

