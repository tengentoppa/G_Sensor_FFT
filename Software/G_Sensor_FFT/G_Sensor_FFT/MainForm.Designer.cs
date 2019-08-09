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
            this.tbxStartPos = new System.Windows.Forms.TextBox();
            this.cbxSamples = new System.Windows.Forms.ComboBox();
            this.ofdInput = new System.Windows.Forms.OpenFileDialog();
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
            this.btnLoadFile.Location = new System.Drawing.Point(12, 488);
            this.btnLoadFile.Name = "btnLoadFile";
            this.btnLoadFile.Size = new System.Drawing.Size(174, 49);
            this.btnLoadFile.TabIndex = 11;
            this.btnLoadFile.Text = "Load File";
            this.btnLoadFile.UseVisualStyleBackColor = true;
            this.btnLoadFile.Visible = false;
            this.btnLoadFile.Click += new System.EventHandler(this.btnLoadFile_Click);
            // 
            // tbxStartPos
            // 
            this.tbxStartPos.Location = new System.Drawing.Point(12, 558);
            this.tbxStartPos.Name = "tbxStartPos";
            this.tbxStartPos.Size = new System.Drawing.Size(174, 25);
            this.tbxStartPos.TabIndex = 12;
            this.tbxStartPos.Visible = false;
            this.tbxStartPos.TextChanged += new System.EventHandler(this.tbxStartPos_TextChanged);
            // 
            // cbxSamples
            // 
            this.cbxSamples.FormattingEnabled = true;
            this.cbxSamples.Location = new System.Drawing.Point(12, 624);
            this.cbxSamples.Name = "cbxSamples";
            this.cbxSamples.Size = new System.Drawing.Size(174, 23);
            this.cbxSamples.TabIndex = 13;
            this.cbxSamples.Visible = false;
            this.cbxSamples.SelectedIndexChanged += new System.EventHandler(this.cbxSample_SelectedIndexChanged);
            // 
            // ofdInput
            // 
            this.ofdInput.DefaultExt = "csv";
            this.ofdInput.FileName = "log";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1547, 837);
            this.Controls.Add(this.cbxSamples);
            this.Controls.Add(this.tbxStartPos);
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
        private System.Windows.Forms.TextBox tbxStartPos;
        private System.Windows.Forms.ComboBox cbxSamples;
        private System.Windows.Forms.OpenFileDialog ofdInput;
    }
}

