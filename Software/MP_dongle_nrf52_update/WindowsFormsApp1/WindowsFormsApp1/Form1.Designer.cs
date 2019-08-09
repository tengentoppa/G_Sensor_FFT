namespace WindowsFormsApp1
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
            this.components = new System.ComponentModel.Container();
            this.comboBox_serial = new System.Windows.Forms.ComboBox();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btn_serial = new System.Windows.Forms.Button();
            this.timer_check = new System.Windows.Forms.Timer(this.components);
            this.timer_send_data = new System.Windows.Forms.Timer(this.components);
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btn_connect = new System.Windows.Forms.Button();
            this.btn_scan = new System.Windows.Forms.Button();
            this.list_scan_device = new System.Windows.Forms.ListBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.btn_auto_connect = new System.Windows.Forms.Button();
            this.list_auto_connect = new System.Windows.Forms.ListBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btn_ConnectedDevice_refrash = new System.Windows.Forms.Button();
            this.list_connected_device = new System.Windows.Forms.ListBox();
            this.btn_disconnect = new System.Windows.Forms.Button();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.listView_SettingAutoList = new System.Windows.Forms.ListView();
            this.btn_SettingAutoList_add = new System.Windows.Forms.Button();
            this.txt_SettingAutoList_add = new System.Windows.Forms.TextBox();
            this.btn_SettingAutoList_scan = new System.Windows.Forms.Button();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.groupBox12 = new System.Windows.Forms.GroupBox();
            this.btn_reset = new System.Windows.Forms.Button();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.timer_get_list = new System.Windows.Forms.Timer(this.components);
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.tabPage7 = new System.Windows.Forms.TabPage();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.listView1 = new System.Windows.Forms.ListView();
            this.btn_remove = new System.Windows.Forms.Button();
            this.btn_send_data_all = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.btn_write_listview = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txt_CMD = new System.Windows.Forms.TextBox();
            this.txt_delay = new System.Windows.Forms.TextBox();
            this.txt_detail = new System.Windows.Forms.TextBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.txt_send_data = new System.Windows.Forms.TextBox();
            this.btn_send = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.txt_UUID = new System.Windows.Forms.TextBox();
            this.btn_UUID_get = new System.Windows.Forms.Button();
            this.btn_UUID_set = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txt_log = new System.Windows.Forms.TextBox();
            this.btn_bin = new System.Windows.Forms.Button();
            this.lab_bin = new System.Windows.Forms.Label();
            this.btn_update = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.groupBox12.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox11.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPage6.SuspendLayout();
            this.tabPage7.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBox_serial
            // 
            this.comboBox_serial.Font = new System.Drawing.Font("新細明體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.comboBox_serial.FormattingEnabled = true;
            this.comboBox_serial.Location = new System.Drawing.Point(9, 20);
            this.comboBox_serial.Name = "comboBox_serial";
            this.comboBox_serial.Size = new System.Drawing.Size(121, 27);
            this.comboBox_serial.TabIndex = 0;
            // 
            // serialPort1
            // 
            this.serialPort1.ReadBufferSize = 8196;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // btn_serial
            // 
            this.btn_serial.Location = new System.Drawing.Point(136, 20);
            this.btn_serial.Name = "btn_serial";
            this.btn_serial.Size = new System.Drawing.Size(94, 27);
            this.btn_serial.TabIndex = 1;
            this.btn_serial.Text = "Connect";
            this.btn_serial.UseVisualStyleBackColor = true;
            this.btn_serial.Click += new System.EventHandler(this.btn_serial_Click);
            // 
            // timer_check
            // 
            this.timer_check.Enabled = true;
            this.timer_check.Interval = 10;
            this.timer_check.Tick += new System.EventHandler(this.timer_check_Tick);
            // 
            // timer_send_data
            // 
            this.timer_send_data.Interval = 10;
            this.timer_send_data.Tick += new System.EventHandler(this.timer_send_data_Tick);
            // 
            // tabControl1
            // 
            this.tabControl1.Alignment = System.Windows.Forms.TabAlignment.Left;
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabControl1.ItemSize = new System.Drawing.Size(50, 100);
            this.tabControl1.Location = new System.Drawing.Point(6, 18);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(372, 511);
            this.tabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControl1.TabIndex = 29;
            this.tabControl1.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.tabControl1_DrawItem);
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.ForeColor = System.Drawing.SystemColors.WindowText;
            this.tabPage1.Location = new System.Drawing.Point(104, 4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(264, 503);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "手動連線";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btn_connect);
            this.groupBox2.Controls.Add(this.btn_scan);
            this.groupBox2.Controls.Add(this.list_scan_device);
            this.groupBox2.Location = new System.Drawing.Point(6, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(252, 491);
            this.groupBox2.TabIndex = 53;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Scan device";
            // 
            // btn_connect
            // 
            this.btn_connect.Enabled = false;
            this.btn_connect.Location = new System.Drawing.Point(6, 462);
            this.btn_connect.Name = "btn_connect";
            this.btn_connect.Size = new System.Drawing.Size(220, 23);
            this.btn_connect.TabIndex = 32;
            this.btn_connect.Text = "Connect Device";
            this.btn_connect.UseVisualStyleBackColor = true;
            this.btn_connect.Click += new System.EventHandler(this.btn_connect_Click);
            // 
            // btn_scan
            // 
            this.btn_scan.Font = new System.Drawing.Font("新細明體", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btn_scan.Location = new System.Drawing.Point(6, 21);
            this.btn_scan.Name = "btn_scan";
            this.btn_scan.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btn_scan.Size = new System.Drawing.Size(222, 116);
            this.btn_scan.TabIndex = 36;
            this.btn_scan.Text = "Scan Start";
            this.btn_scan.UseVisualStyleBackColor = true;
            this.btn_scan.Click += new System.EventHandler(this.btn_scan_Click);
            // 
            // list_scan_device
            // 
            this.list_scan_device.FormattingEnabled = true;
            this.list_scan_device.ItemHeight = 12;
            this.list_scan_device.Location = new System.Drawing.Point(6, 143);
            this.list_scan_device.Name = "list_scan_device";
            this.list_scan_device.ScrollAlwaysVisible = true;
            this.list_scan_device.Size = new System.Drawing.Size(220, 316);
            this.list_scan_device.TabIndex = 29;
            this.list_scan_device.SelectedIndexChanged += new System.EventHandler(this.list_scan_device_SelectedIndexChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox10);
            this.tabPage2.Location = new System.Drawing.Point(104, 4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(264, 503);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "自動連線";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.btn_auto_connect);
            this.groupBox10.Controls.Add(this.list_auto_connect);
            this.groupBox10.Location = new System.Drawing.Point(3, 3);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(251, 494);
            this.groupBox10.TabIndex = 0;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "Auto connect";
            // 
            // btn_auto_connect
            // 
            this.btn_auto_connect.Font = new System.Drawing.Font("新細明體", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btn_auto_connect.Location = new System.Drawing.Point(6, 20);
            this.btn_auto_connect.Name = "btn_auto_connect";
            this.btn_auto_connect.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btn_auto_connect.Size = new System.Drawing.Size(238, 116);
            this.btn_auto_connect.TabIndex = 60;
            this.btn_auto_connect.Text = "Auto Connect Start";
            this.btn_auto_connect.UseVisualStyleBackColor = true;
            this.btn_auto_connect.Click += new System.EventHandler(this.btn_auto_connect_Click);
            // 
            // list_auto_connect
            // 
            this.list_auto_connect.FormattingEnabled = true;
            this.list_auto_connect.ItemHeight = 12;
            this.list_auto_connect.Location = new System.Drawing.Point(6, 142);
            this.list_auto_connect.Name = "list_auto_connect";
            this.list_auto_connect.Size = new System.Drawing.Size(238, 340);
            this.list_auto_connect.TabIndex = 1;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.groupBox1);
            this.tabPage3.Controls.Add(this.btn_disconnect);
            this.tabPage3.Location = new System.Drawing.Point(104, 4);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(264, 503);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "連線狀態";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btn_ConnectedDevice_refrash);
            this.groupBox1.Controls.Add(this.list_connected_device);
            this.groupBox1.Location = new System.Drawing.Point(4, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(254, 464);
            this.groupBox1.TabIndex = 53;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Connected device";
            // 
            // btn_ConnectedDevice_refrash
            // 
            this.btn_ConnectedDevice_refrash.Location = new System.Drawing.Point(6, 21);
            this.btn_ConnectedDevice_refrash.Name = "btn_ConnectedDevice_refrash";
            this.btn_ConnectedDevice_refrash.Size = new System.Drawing.Size(240, 37);
            this.btn_ConnectedDevice_refrash.TabIndex = 35;
            this.btn_ConnectedDevice_refrash.Text = "重新讀取";
            this.btn_ConnectedDevice_refrash.UseVisualStyleBackColor = true;
            this.btn_ConnectedDevice_refrash.Click += new System.EventHandler(this.btn_ConnectedDevice_refrash_Click);
            // 
            // list_connected_device
            // 
            this.list_connected_device.FormattingEnabled = true;
            this.list_connected_device.ItemHeight = 12;
            this.list_connected_device.Location = new System.Drawing.Point(6, 64);
            this.list_connected_device.Name = "list_connected_device";
            this.list_connected_device.ScrollAlwaysVisible = true;
            this.list_connected_device.Size = new System.Drawing.Size(240, 388);
            this.list_connected_device.TabIndex = 34;
            this.list_connected_device.SelectedIndexChanged += new System.EventHandler(this.list_connected_device_SelectedIndexChanged);
            // 
            // btn_disconnect
            // 
            this.btn_disconnect.Enabled = false;
            this.btn_disconnect.Location = new System.Drawing.Point(8, 476);
            this.btn_disconnect.Name = "btn_disconnect";
            this.btn_disconnect.Size = new System.Drawing.Size(242, 23);
            this.btn_disconnect.TabIndex = 35;
            this.btn_disconnect.Text = "Disconnect Device";
            this.btn_disconnect.UseVisualStyleBackColor = true;
            this.btn_disconnect.Click += new System.EventHandler(this.btn_disconnect_Click);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.groupBox9);
            this.tabPage4.Location = new System.Drawing.Point(104, 4);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(264, 503);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "自動連線清單設定";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.button1);
            this.groupBox9.Controls.Add(this.listView_SettingAutoList);
            this.groupBox9.Controls.Add(this.btn_SettingAutoList_add);
            this.groupBox9.Controls.Add(this.txt_SettingAutoList_add);
            this.groupBox9.Controls.Add(this.btn_SettingAutoList_scan);
            this.groupBox9.Location = new System.Drawing.Point(3, 3);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(254, 497);
            this.groupBox9.TabIndex = 0;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "Auto Connect List Setting";
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("新細明體", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.button1.Location = new System.Drawing.Point(7, 433);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(241, 50);
            this.button1.TabIndex = 4;
            this.button1.Text = "Setting";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // listView_SettingAutoList
            // 
            this.listView_SettingAutoList.Location = new System.Drawing.Point(7, 107);
            this.listView_SettingAutoList.Name = "listView_SettingAutoList";
            this.listView_SettingAutoList.Size = new System.Drawing.Size(241, 320);
            this.listView_SettingAutoList.TabIndex = 3;
            this.listView_SettingAutoList.UseCompatibleStateImageBehavior = false;
            this.listView_SettingAutoList.View = System.Windows.Forms.View.Details;
            this.listView_SettingAutoList.SelectedIndexChanged += new System.EventHandler(this.listView_SettingAutoList_SelectedIndexChanged);
            // 
            // btn_SettingAutoList_add
            // 
            this.btn_SettingAutoList_add.Location = new System.Drawing.Point(192, 78);
            this.btn_SettingAutoList_add.Name = "btn_SettingAutoList_add";
            this.btn_SettingAutoList_add.Size = new System.Drawing.Size(56, 23);
            this.btn_SettingAutoList_add.TabIndex = 2;
            this.btn_SettingAutoList_add.Text = "Add";
            this.btn_SettingAutoList_add.UseVisualStyleBackColor = true;
            this.btn_SettingAutoList_add.Click += new System.EventHandler(this.btn_SettingAutoList_add_Click);
            // 
            // txt_SettingAutoList_add
            // 
            this.txt_SettingAutoList_add.Location = new System.Drawing.Point(7, 78);
            this.txt_SettingAutoList_add.Name = "txt_SettingAutoList_add";
            this.txt_SettingAutoList_add.Size = new System.Drawing.Size(178, 22);
            this.txt_SettingAutoList_add.TabIndex = 1;
            // 
            // btn_SettingAutoList_scan
            // 
            this.btn_SettingAutoList_scan.Font = new System.Drawing.Font("新細明體", 24.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btn_SettingAutoList_scan.Location = new System.Drawing.Point(7, 22);
            this.btn_SettingAutoList_scan.Name = "btn_SettingAutoList_scan";
            this.btn_SettingAutoList_scan.Size = new System.Drawing.Size(241, 49);
            this.btn_SettingAutoList_scan.TabIndex = 0;
            this.btn_SettingAutoList_scan.Text = "Scan device";
            this.btn_SettingAutoList_scan.UseVisualStyleBackColor = true;
            this.btn_SettingAutoList_scan.Click += new System.EventHandler(this.btn_SettingAutoList_scan_Click);
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.groupBox12);
            this.tabPage5.Location = new System.Drawing.Point(104, 4);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Size = new System.Drawing.Size(264, 503);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "Reset";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // groupBox12
            // 
            this.groupBox12.Controls.Add(this.btn_reset);
            this.groupBox12.Location = new System.Drawing.Point(3, 3);
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.Size = new System.Drawing.Size(258, 497);
            this.groupBox12.TabIndex = 0;
            this.groupBox12.TabStop = false;
            this.groupBox12.Text = "Reset";
            // 
            // btn_reset
            // 
            this.btn_reset.Font = new System.Drawing.Font("新細明體", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btn_reset.Location = new System.Drawing.Point(6, 21);
            this.btn_reset.Name = "btn_reset";
            this.btn_reset.Size = new System.Drawing.Size(246, 45);
            this.btn_reset.TabIndex = 0;
            this.btn_reset.Text = "Reset";
            this.btn_reset.UseVisualStyleBackColor = true;
            this.btn_reset.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.comboBox_serial);
            this.groupBox8.Controls.Add(this.btn_serial);
            this.groupBox8.Location = new System.Drawing.Point(12, 35);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(384, 61);
            this.groupBox8.TabIndex = 59;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "ComPort Setting";
            // 
            // groupBox11
            // 
            this.groupBox11.Controls.Add(this.tabControl1);
            this.groupBox11.Location = new System.Drawing.Point(12, 102);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.Size = new System.Drawing.Size(384, 535);
            this.groupBox11.TabIndex = 60;
            this.groupBox11.TabStop = false;
            this.groupBox11.Text = "Connect Device";
            // 
            // timer_get_list
            // 
            this.timer_get_list.Enabled = true;
            this.timer_get_list.Tick += new System.EventHandler(this.timer_get_list_Tick);
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabPage6);
            this.tabControl2.Controls.Add(this.tabPage7);
            this.tabControl2.Location = new System.Drawing.Point(402, 2);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(1040, 639);
            this.tabControl2.TabIndex = 61;
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.groupBox7);
            this.tabPage6.Controls.Add(this.groupBox6);
            this.tabPage6.Controls.Add(this.groupBox5);
            this.tabPage6.Controls.Add(this.groupBox4);
            this.tabPage6.Controls.Add(this.groupBox3);
            this.tabPage6.Location = new System.Drawing.Point(4, 22);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage6.Size = new System.Drawing.Size(1032, 613);
            this.tabPage6.TabIndex = 0;
            this.tabPage6.Text = "CMD";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // tabPage7
            // 
            this.tabPage7.Controls.Add(this.btn_update);
            this.tabPage7.Controls.Add(this.lab_bin);
            this.tabPage7.Controls.Add(this.btn_bin);
            this.tabPage7.Location = new System.Drawing.Point(4, 22);
            this.tabPage7.Name = "tabPage7";
            this.tabPage7.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage7.Size = new System.Drawing.Size(1032, 613);
            this.tabPage7.TabIndex = 1;
            this.tabPage7.Text = "UPDATE";
            this.tabPage7.UseVisualStyleBackColor = true;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.listView1);
            this.groupBox7.Controls.Add(this.btn_remove);
            this.groupBox7.Controls.Add(this.btn_send_data_all);
            this.groupBox7.Controls.Add(this.checkBox1);
            this.groupBox7.Location = new System.Drawing.Point(6, 361);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(562, 251);
            this.groupBox7.TabIndex = 63;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "CMD List";
            // 
            // listView1
            // 
            this.listView1.AllowDrop = true;
            this.listView1.Location = new System.Drawing.Point(6, 21);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(466, 203);
            this.listView1.TabIndex = 49;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // btn_remove
            // 
            this.btn_remove.Location = new System.Drawing.Point(478, 201);
            this.btn_remove.Name = "btn_remove";
            this.btn_remove.Size = new System.Drawing.Size(75, 23);
            this.btn_remove.TabIndex = 50;
            this.btn_remove.Text = "刪除";
            this.btn_remove.UseVisualStyleBackColor = true;
            // 
            // btn_send_data_all
            // 
            this.btn_send_data_all.Location = new System.Drawing.Point(481, 21);
            this.btn_send_data_all.Name = "btn_send_data_all";
            this.btn_send_data_all.Size = new System.Drawing.Size(75, 95);
            this.btn_send_data_all.TabIndex = 51;
            this.btn_send_data_all.Text = "送出";
            this.btn_send_data_all.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(481, 123);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(48, 16);
            this.checkBox1.TabIndex = 52;
            this.checkBox1.Text = "循環";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.btn_write_listview);
            this.groupBox6.Controls.Add(this.label4);
            this.groupBox6.Controls.Add(this.label5);
            this.groupBox6.Controls.Add(this.label6);
            this.groupBox6.Controls.Add(this.txt_CMD);
            this.groupBox6.Controls.Add(this.txt_delay);
            this.groupBox6.Controls.Add(this.txt_detail);
            this.groupBox6.Location = new System.Drawing.Point(6, 218);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(565, 124);
            this.groupBox6.TabIndex = 62;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Set CMD List";
            // 
            // btn_write_listview
            // 
            this.btn_write_listview.Location = new System.Drawing.Point(480, 21);
            this.btn_write_listview.Name = "btn_write_listview";
            this.btn_write_listview.Size = new System.Drawing.Size(75, 95);
            this.btn_write_listview.TabIndex = 48;
            this.btn_write_listview.Text = "Add";
            this.btn_write_listview.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(5, 33);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 12);
            this.label4.TabIndex = 42;
            this.label4.Text = "CMD：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(5, 66);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 12);
            this.label5.TabIndex = 43;
            this.label5.Text = "說明：";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(5, 97);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 12);
            this.label6.TabIndex = 44;
            this.label6.Text = "Delay(ms)：";
            // 
            // txt_CMD
            // 
            this.txt_CMD.Location = new System.Drawing.Point(76, 30);
            this.txt_CMD.Name = "txt_CMD";
            this.txt_CMD.Size = new System.Drawing.Size(397, 22);
            this.txt_CMD.TabIndex = 45;
            // 
            // txt_delay
            // 
            this.txt_delay.Location = new System.Drawing.Point(76, 94);
            this.txt_delay.Name = "txt_delay";
            this.txt_delay.Size = new System.Drawing.Size(397, 22);
            this.txt_delay.TabIndex = 46;
            // 
            // txt_detail
            // 
            this.txt_detail.Location = new System.Drawing.Point(76, 63);
            this.txt_detail.Name = "txt_detail";
            this.txt_detail.Size = new System.Drawing.Size(397, 22);
            this.txt_detail.TabIndex = 47;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.txt_send_data);
            this.groupBox5.Controls.Add(this.btn_send);
            this.groupBox5.Location = new System.Drawing.Point(6, 113);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(425, 82);
            this.groupBox5.TabIndex = 61;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "CMD";
            // 
            // txt_send_data
            // 
            this.txt_send_data.Location = new System.Drawing.Point(6, 21);
            this.txt_send_data.Name = "txt_send_data";
            this.txt_send_data.Size = new System.Drawing.Size(398, 22);
            this.txt_send_data.TabIndex = 37;
            // 
            // btn_send
            // 
            this.btn_send.Location = new System.Drawing.Point(6, 49);
            this.btn_send.Name = "btn_send";
            this.btn_send.Size = new System.Drawing.Size(75, 23);
            this.btn_send.TabIndex = 38;
            this.btn_send.Text = "Send";
            this.btn_send.UseVisualStyleBackColor = true;
            this.btn_send.Click += new System.EventHandler(this.btn_send_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.txt_UUID);
            this.groupBox4.Controls.Add(this.btn_UUID_get);
            this.groupBox4.Controls.Add(this.btn_UUID_set);
            this.groupBox4.Location = new System.Drawing.Point(6, 10);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(425, 82);
            this.groupBox4.TabIndex = 60;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "UUID";
            // 
            // txt_UUID
            // 
            this.txt_UUID.Location = new System.Drawing.Point(7, 21);
            this.txt_UUID.Name = "txt_UUID";
            this.txt_UUID.Size = new System.Drawing.Size(400, 22);
            this.txt_UUID.TabIndex = 39;
            // 
            // btn_UUID_get
            // 
            this.btn_UUID_get.Location = new System.Drawing.Point(88, 49);
            this.btn_UUID_get.Name = "btn_UUID_get";
            this.btn_UUID_get.Size = new System.Drawing.Size(75, 23);
            this.btn_UUID_get.TabIndex = 41;
            this.btn_UUID_get.Text = "Get";
            this.btn_UUID_get.UseVisualStyleBackColor = true;
            // 
            // btn_UUID_set
            // 
            this.btn_UUID_set.Location = new System.Drawing.Point(7, 49);
            this.btn_UUID_set.Name = "btn_UUID_set";
            this.btn_UUID_set.Size = new System.Drawing.Size(75, 23);
            this.btn_UUID_set.TabIndex = 40;
            this.btn_UUID_set.Text = "Set";
            this.btn_UUID_set.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txt_log);
            this.groupBox3.Location = new System.Drawing.Point(579, 10);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(448, 602);
            this.groupBox3.TabIndex = 59;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Log";
            // 
            // txt_log
            // 
            this.txt_log.Location = new System.Drawing.Point(6, 14);
            this.txt_log.Multiline = true;
            this.txt_log.Name = "txt_log";
            this.txt_log.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txt_log.Size = new System.Drawing.Size(436, 582);
            this.txt_log.TabIndex = 28;
            // 
            // btn_bin
            // 
            this.btn_bin.Location = new System.Drawing.Point(19, 30);
            this.btn_bin.Name = "btn_bin";
            this.btn_bin.Size = new System.Drawing.Size(75, 23);
            this.btn_bin.TabIndex = 0;
            this.btn_bin.Text = "選取bin檔";
            this.btn_bin.UseVisualStyleBackColor = true;
            this.btn_bin.Click += new System.EventHandler(this.btn_bin_Click);
            // 
            // lab_bin
            // 
            this.lab_bin.AutoSize = true;
            this.lab_bin.Location = new System.Drawing.Point(100, 35);
            this.lab_bin.Name = "lab_bin";
            this.lab_bin.Size = new System.Drawing.Size(0, 12);
            this.lab_bin.TabIndex = 1;
            // 
            // btn_update
            // 
            this.btn_update.Location = new System.Drawing.Point(19, 71);
            this.btn_update.Name = "btn_update";
            this.btn_update.Size = new System.Drawing.Size(75, 23);
            this.btn_update.TabIndex = 2;
            this.btn_update.Text = "開始更新";
            this.btn_update.UseVisualStyleBackColor = true;
            this.btn_update.Click += new System.EventHandler(this.btn_update_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1449, 649);
            this.Controls.Add(this.tabControl2);
            this.Controls.Add(this.groupBox11);
            this.Controls.Add(this.groupBox8);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Name = "Form1";
            this.Text = "BLE_gateway_auto";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.groupBox10.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.tabPage5.ResumeLayout(false);
            this.groupBox12.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
            this.groupBox11.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.tabPage6.ResumeLayout(false);
            this.tabPage7.ResumeLayout(false);
            this.tabPage7.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBox_serial;
        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button btn_serial;
        private System.Windows.Forms.Timer timer_check;
        private System.Windows.Forms.Timer timer_send_data;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button btn_scan;
        private System.Windows.Forms.Button btn_disconnect;
        private System.Windows.Forms.ListBox list_connected_device;
        private System.Windows.Forms.Button btn_connect;
        private System.Windows.Forms.ListBox list_scan_device;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.GroupBox groupBox10;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.Button btn_auto_connect;
        private System.Windows.Forms.ListBox list_auto_connect;
        private System.Windows.Forms.Button btn_SettingAutoList_scan;
        private System.Windows.Forms.GroupBox groupBox11;
        private System.Windows.Forms.ListView listView_SettingAutoList;
        private System.Windows.Forms.Button btn_SettingAutoList_add;
        private System.Windows.Forms.TextBox txt_SettingAutoList_add;
        private System.Windows.Forms.Timer timer_get_list;
        private System.Windows.Forms.Button btn_ConnectedDevice_refrash;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.GroupBox groupBox12;
        private System.Windows.Forms.Button btn_reset;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Button btn_remove;
        private System.Windows.Forms.Button btn_send_data_all;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Button btn_write_listview;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txt_CMD;
        private System.Windows.Forms.TextBox txt_delay;
        private System.Windows.Forms.TextBox txt_detail;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox txt_send_data;
        private System.Windows.Forms.Button btn_send;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox txt_UUID;
        private System.Windows.Forms.Button btn_UUID_get;
        private System.Windows.Forms.Button btn_UUID_set;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txt_log;
        private System.Windows.Forms.TabPage tabPage7;
        private System.Windows.Forms.Label lab_bin;
        private System.Windows.Forms.Button btn_bin;
        private System.Windows.Forms.Button btn_update;
    }
}

