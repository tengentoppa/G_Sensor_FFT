namespace Performance_Test
{
    partial class Form1
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
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.zgcTest1 = new ZedGraph.ZedGraphControl();
            this.zgcTest2 = new ZedGraph.ZedGraphControl();
            this.zgcTest3 = new ZedGraph.ZedGraphControl();
            this.tmrPlay = new System.Windows.Forms.Timer();
            this.btnStart = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // zgcTest1
            // 
            this.zgcTest1.Location = new System.Drawing.Point(13, 12);
            this.zgcTest1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.zgcTest1.Name = "zgcTest1";
            this.zgcTest1.ScrollGrace = 0D;
            this.zgcTest1.ScrollMaxX = 0D;
            this.zgcTest1.ScrollMaxY = 0D;
            this.zgcTest1.ScrollMaxY2 = 0D;
            this.zgcTest1.ScrollMinX = 0D;
            this.zgcTest1.ScrollMinY = 0D;
            this.zgcTest1.ScrollMinY2 = 0D;
            this.zgcTest1.Size = new System.Drawing.Size(861, 254);
            this.zgcTest1.TabIndex = 0;
            // 
            // zgcTest2
            // 
            this.zgcTest2.Location = new System.Drawing.Point(13, 272);
            this.zgcTest2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.zgcTest2.Name = "zgcTest2";
            this.zgcTest2.ScrollGrace = 0D;
            this.zgcTest2.ScrollMaxX = 0D;
            this.zgcTest2.ScrollMaxY = 0D;
            this.zgcTest2.ScrollMaxY2 = 0D;
            this.zgcTest2.ScrollMinX = 0D;
            this.zgcTest2.ScrollMinY = 0D;
            this.zgcTest2.ScrollMinY2 = 0D;
            this.zgcTest2.Size = new System.Drawing.Size(861, 254);
            this.zgcTest2.TabIndex = 1;
            // 
            // zgcTest3
            // 
            this.zgcTest3.Location = new System.Drawing.Point(13, 532);
            this.zgcTest3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.zgcTest3.Name = "zgcTest3";
            this.zgcTest3.ScrollGrace = 0D;
            this.zgcTest3.ScrollMaxX = 0D;
            this.zgcTest3.ScrollMaxY = 0D;
            this.zgcTest3.ScrollMaxY2 = 0D;
            this.zgcTest3.ScrollMinX = 0D;
            this.zgcTest3.ScrollMinY = 0D;
            this.zgcTest3.ScrollMinY2 = 0D;
            this.zgcTest3.Size = new System.Drawing.Size(861, 254);
            this.zgcTest3.TabIndex = 2;
            // 
            // tmrPlay
            // 
            this.tmrPlay.Tick += new System.EventHandler(this.tmrPlay_Tick);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(98, 810);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 3;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(553, 810);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1010, 915);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.zgcTest3);
            this.Controls.Add(this.zgcTest2);
            this.Controls.Add(this.zgcTest1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private ZedGraph.ZedGraphControl zgcTest1;
        private ZedGraph.ZedGraphControl zgcTest2;
        private ZedGraph.ZedGraphControl zgcTest3;
        private System.Windows.Forms.Timer tmrPlay;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button button1;
    }
}

