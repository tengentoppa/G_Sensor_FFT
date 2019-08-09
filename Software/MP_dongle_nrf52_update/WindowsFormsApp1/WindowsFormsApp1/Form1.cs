using Ini;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public static string SERILA_CONNECT = "Connect";
        public static string SERILA_DISCONNECT = "Disconnect";
        public static string SCAN_START = "Scan Start";
        public static string SCAN_STOP = "Scan Stop";

        MP_dongle class_MP_dongle = new MP_dongle();
        string[] serial_COM_buffer;
        List<string> log_dongle = new List<string>();
        List<serial_data_type> list_serial_data = new List<serial_data_type>();
        List<serial_data_type> list_serial_data_error = new List<serial_data_type>();
        Thread thread_read_buffer;
        IniFile inifile;
        string path = "";
        bool flag_send_ble_data = false;
        bool flag_send_ble_data_auto = false;
        int index_send_ble_device = 0;
        int index_send_ble_list = 0;
        int delay_send_ble_data = 0;
        string str_send_ble_data_list = "";
        byte[] BinData;
        private void Form1_Load(object sender, EventArgs e)
        {
            btn_serial.Text = SERILA_CONNECT;
            btn_scan.Text = SCAN_START;
            thread_read_buffer = new Thread(Receive_serial_data);
            thread_read_buffer.IsBackground = true;
            thread_read_buffer.Start();
            int width = listView1.Width;
            listView1.Columns.Add("說明", width / 6, HorizontalAlignment.Center);
            listView1.Columns.Add("CMD", width / 3 * 2, HorizontalAlignment.Center);
            listView1.Columns.Add("Delay", width / 6, HorizontalAlignment.Center);
            listView1.FullRowSelect = true;

            width = listView_SettingAutoList.Width - 30;
            listView_SettingAutoList.Columns.Add("", 30, HorizontalAlignment.Center);
            listView_SettingAutoList.Columns.Add("MAC Address", width / 5 * 3, HorizontalAlignment.Center);
            listView_SettingAutoList.Columns.Add("Status", width / 5 * 2, HorizontalAlignment.Center);
            listView_SettingAutoList.CheckBoxes = true;
            listView_SettingAutoList.FullRowSelect = true;
            listView_SettingAutoList.Columns[0].DisplayIndex = listView_SettingAutoList.Columns.Count - 1;

            DirectoryInfo dir = new DirectoryInfo(System.Windows.Forms.Application.StartupPath);
            path = dir + "\\DATA.ini";
            inifile = new IniFile(path);
            try
            {
                string s = inifile.IniReadValue("Count", "Count");
                int size = Int32.Parse(s);
                for (int i = 0; i < size; i++)
                {
                    ListViewItem x = new ListViewItem(inifile.IniReadValue(i.ToString(), "Name"));
                    ListViewItem.ListViewSubItem sub_i1 = new ListViewItem.ListViewSubItem();
                    ListViewItem.ListViewSubItem sub_i2 = new ListViewItem.ListViewSubItem();
                    sub_i1.Text = inifile.IniReadValue(i.ToString(), "CMD");
                    sub_i2.Text = inifile.IniReadValue(i.ToString(), "Delay");
                    x.SubItems.Add(sub_i1);
                    x.SubItems.Add(sub_i2);
                    listView1.Items.Add(x);
                }
            }
            catch (Exception ex) { }
        }

        public bool serial_connect(string com_port, SerialPort r_serial)
        {
            if (com_port != "")
            {
                try
                {
                    r_serial.Close();
                    r_serial.PortName = com_port;
                    r_serial.BaudRate = 921600;
                    r_serial.Parity = Parity.None;
                    r_serial.StopBits = StopBits.One;
                    r_serial.DataBits = 8;
                    r_serial.Handshake = Handshake.None;
                    r_serial.Open();
                    return true;
                }
                catch (Exception ex) { }
            }
            return false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string[] temp = SerialPort.GetPortNames();
            bool ischange;
            bool islive_serial = false;
            try
            {
                ischange = false;
                int max = Math.Max(serial_COM_buffer.Length, temp.Length);
                for (int i = 0; i < max; i++)
                {
                    if (serial_COM_buffer[i] != temp[i])
                    {
                        ischange = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ischange = true;
            }
            if (ischange)
            {
                serial_COM_buffer = temp;
                comboBox_serial.Items.Clear();

                for (int i = 0; i < serial_COM_buffer.Length; i++)
                {
                    if (comboBox_serial.Text == serial_COM_buffer[i]) islive_serial = true;

                    comboBox_serial.Items.Add(serial_COM_buffer[i]);

                }
                if (islive_serial == false) comboBox_serial.Text = "";

                if (serial_COM_buffer.Length > 0 && comboBox_serial.Text == "") comboBox_serial.SelectedIndex = 0;

            }
            if (serialPort1.IsOpen == false && btn_serial.Text == SERILA_DISCONNECT)
            {
                serialPort1.Close();
                btn_serial.Text = SERILA_CONNECT;
            }
        }

        private void btn_serial_Click(object sender, EventArgs e)
        {
            if (btn_serial.Text == SERILA_CONNECT)
            {
                serial_connect(comboBox_serial.Text, serialPort1);
                btn_serial.Text = SERILA_DISCONNECT;
                serial_write_data(class_MP_dongle.version());
            }
            else
            {
                serialPort1.Close();
                btn_serial.Text = SERILA_CONNECT;
                this.Text = "BLE_gateway_auto";
            }
        }

        public void Receive_serial_data()
        {
            byte[] serial_buffer = null;
            List<byte> serial_error = new List<byte>();
            int status = (int)serial_read.status.STATUS_NULL;
            int payload_size = 0;
            int buffer_index = 0;
            bool flag_serial_error = false;
            bool flag_error_printf = false;
            DateTime dttest = DateTime.Now;
            List<byte> d = new List<byte>();
            while (true)
            {
                try
                {
                    if (serialPort1.IsOpen == true && serialPort1.BytesToRead > 0)
                    {
                        byte[] data = new byte[serialPort1.BytesToRead];
                        serialPort1.Read(data, 0, data.Length);
                        d.AddRange(data);
                        if ((DateTime.Now - dttest).TotalMilliseconds >= 1000)
                        {
                            Console.WriteLine(d.Count);
                            dttest = dttest.AddMilliseconds(1000);
                            d.Clear();
                        }

                        for (int i = 0; i < data.Length; i++)
                        {
                            switch (status)
                            {
                                case (int)serial_read.status.STATUS_NULL:
                                    if (data[i] == (byte)MP_dongle.CMD.RX_STX)
                                    {
                                        status = (int)serial_read.status.STATUS_SIZE;
                                        payload_size = 0;
                                        if (flag_serial_error) flag_error_printf = true;
                                    }
                                    else
                                    {
                                        flag_serial_error = true;
                                        serial_error.Add(data[i]);
                                    }
                                    break;

                                case (int)serial_read.status.STATUS_SIZE:
                                    payload_size = data[i] + 2;
                                    serial_buffer = new byte[data[i] + 4];
                                    serial_buffer[0] = (byte)MP_dongle.CMD.RX_STX;
                                    serial_buffer[1] = data[i];
                                    buffer_index = 2;
                                    status = (int)serial_read.status.STATUS_PAYLOAD;
                                    if (payload_size > 50)
                                        status = (int)serial_read.status.STATUS_NULL;
                                    break;

                                case (int)serial_read.status.STATUS_PAYLOAD:
                                    serial_buffer[buffer_index] = data[i];
                                    buffer_index++;
                                    payload_size--;
                                    if (payload_size == 0)
                                    {
                                        serial_data_type tmp = new serial_data_type();
                                        tmp.data = serial_buffer;
                                        tmp.time = DateTime.Now.ToString("yyy/MM/dd HH:mm:ss.fff");
                                        list_serial_data.Add(tmp);
                                        status = (int)serial_read.status.STATUS_NULL;
                                    }
                                    break;
                            }
                        }
                        if (flag_error_printf)
                        {
                            flag_error_printf = false;
                            flag_serial_error = false;
                            serial_data_type tmp = new serial_data_type();
                            tmp.data = new byte[serial_error.Count];
                            serial_error.CopyTo(tmp.data, 0);
                            tmp.time = DateTime.Now.ToString("yyy/MM/dd HH:mm:ss.fff");
                            list_serial_data_error.Add(tmp);
                        }
                    }
                }
                catch (Exception ex) { }
            }
        }

        public void select_connected_device(int index)
        {
            try
            {
                class_MP_dongle.select_connect_device = class_MP_dongle.connected_device_info[index].peer;
                btn_disconnect.Enabled = true;
            }
            catch (Exception ex)
            { }
        }

        public void conver_hex_dongle(byte[] rx, string date)
        {
            try
            {
                string str_log = "";
                string str_log_receive = class_MP_dongle.serial_Receive(rx, date);
                if (class_MP_dongle.Flag_is_change_device_show == false && class_MP_dongle.Flag_show_log == true/*&& class_MP_dongle.Flag_is_no_show_log == false*/)
                {
                    str_log = "[" + date + "] RX: ";
                    for (int i = 0; i < rx.Length; i++)
                        str_log += String.Format("{0:X2}", rx[i]) + " ";
                    str_log += Environment.NewLine;
                    log_dongle.Add(str_log);
                    serial_data_type tmp = new serial_data_type();
                    tmp.data = rx;
                    tmp.time = date;
                    list_serial_data_error.Add(tmp);
                }
                if (str_log_receive != "")
                    log_dongle.Add(str_log_receive);
                class_MP_dongle.Flag_is_no_show_log = false;
            }
            catch (Exception ex)
            { }
        }

        public void serial_write_data(byte[] data)
        {
            string str_log = "";
            if (serialPort1.IsOpen == false) return;
            str_log = "[" + DateTime.Now.ToString("yyy/MM/dd HH:mm:ss.fff") + "] TX: ";
            for (int i = 0; i < data.Length; i++)
                str_log += String.Format("{0:X2}", data[i]) + " ";
            str_log += Environment.NewLine;
            log_dongle.Add(str_log);
            serialPort1.Write(data, 0, data.Length);
        }

        public void send_ble_data(string str_data)
        {
            char[] char_send = str_data.ToCharArray();
            byte[] data_payload = new byte[char_send.Length / 2];
            if (serialPort1.IsOpen == false) return;
            if (char_send.Length % 2 != 0) return;
            for (int i = 0; i < (char_send.Length / 2); i++)
            {
                string str = char_send[2 * i].ToString() + char_send[2 * i + 1].ToString();
                data_payload[i] = Convert.ToByte(str, 16);
            }
            byte[] data = class_MP_dongle.ble_tx(data_payload);
            serial_write_data(data);
        }

        private void timer_check_Tick(object sender, EventArgs e)
        {
            while (list_serial_data.Count != 0)
            {
                try
                {
                    conver_hex_dongle(list_serial_data[0].data, list_serial_data[0].time);
                }
                catch (Exception ex)
                {

                }
                try
                {
                    list_serial_data.RemoveAt(0);
                }
                catch (Exception ex)
                { }
            }

            while (log_dongle.Count != 0)
            {
                txt_log.Text += log_dongle[0];
                log_dongle.RemoveAt(0);
            }

            if (class_MP_dongle.Flag_is_change_device_show)
            {
                if (tabControl1.SelectedIndex == 0)
                {
                    Point p = list_scan_device.AutoScrollOffset;
                    for (int i = 0; i < class_MP_dongle.ble_device.Count; i++)
                    {
                        if (list_scan_device.Items.Count > i)
                            list_scan_device.Items[i] = (class_MP_dongle.ble_device[i]);
                        else
                            list_scan_device.Items.Add(class_MP_dongle.ble_device[i]);
                    }
                    list_scan_device.AutoScrollOffset = p;
                }
                else if (tabControl1.SelectedIndex == 3)
                {
                    for (int i = listView_SettingAutoList.Items.Count; i < class_MP_dongle.Set_Auto_connect_list_info.Count; i++)
                    {
                        ListViewItem x = new ListViewItem("");
                        ListViewItem.ListViewSubItem sub_i1 = new ListViewItem.ListViewSubItem();
                        ListViewItem.ListViewSubItem sub_i2 = new ListViewItem.ListViewSubItem();
                        sub_i1.Text = class_MP_dongle.Set_Auto_connect_list_info[i].str_device_mac;
                        sub_i2.Text = class_MP_dongle.Set_Auto_connect_list_info[i].str_device_name;
                        x.SubItems.Add(sub_i1);
                        x.SubItems.Add(sub_i2);
                        listView_SettingAutoList.Items.Add(x);
                    }
                }
            }

            if (class_MP_dongle.Flag_is_change_device_connect_show)
            {
                class_MP_dongle.Flag_is_change_device_connect_show = false;
                list_connected_device.Items.Clear();
                for (int i = 0; i < class_MP_dongle.connected_device_info.Count; i++)
                {
                    if (list_connected_device.Items.Count > i)
                        list_connected_device.Items[i] = class_MP_dongle.connected_device_info[i].str_device_mac + "\t" + class_MP_dongle.connected_device_info[i].error_counter + "/" + class_MP_dongle.connected_device_info[i].catch_counter;
                    else
                        list_connected_device.Items.Add(class_MP_dongle.connected_device_info[i].str_device_mac + "\t" + class_MP_dongle.connected_device_info[i].error_counter + "/" + class_MP_dongle.connected_device_info[i].catch_counter);
                }
            }

            if (btn_serial.Text == SERILA_CONNECT && list_connected_device.Items.Count != 0)
            {
                class_MP_dongle.ble_device_connect.Clear();
                class_MP_dongle.Flag_is_change_device_connect_show = true;
                class_MP_dongle.connected_device_info.Clear();
            }

            if (class_MP_dongle.Flag_is_UUID_show)
            {
                class_MP_dongle.Flag_is_UUID_show = false;
                txt_UUID.Text = class_MP_dongle.str_UUID;
            }
            if (class_MP_dongle.Flag_is_connect_success)
            {
                class_MP_dongle.Flag_is_connect_success = false;
                if (btn_scan.Text != SCAN_START)
                {
                    byte[] data = class_MP_dongle.scan_stop();
                    serial_write_data(data);
                    btn_scan.Text = SCAN_START;
                }
            }
            if (class_MP_dongle.ble_rx_payload.Count != 0)
            {
                try
                {
                    FileStream fs = new FileStream(class_MP_dongle.ble_rx_payload[0].doc_name, FileMode.Append);
                    StreamWriter sw = new StreamWriter(fs, Encoding.Default);
                    sw.Write(class_MP_dongle.ble_rx_payload[0].content + Environment.NewLine);
                    sw.Flush();
                    sw.Close();
                    fs.Close();
                    class_MP_dongle.ble_rx_payload.RemoveAt(0);
                }
                catch (Exception ex)
                {
                    ex = ex;
                }
            }
            if (list_serial_data_error.Count != 0)
            {
                try
                {
                    FileStream fs = new FileStream("Error_data.txt", FileMode.Append);
                    StreamWriter sw = new StreamWriter(fs, Encoding.Default);
                    string str = "";
                    for (int i = 0; i < list_serial_data_error[0].data.Length; i++)
                        str += String.Format("{0:X2}", list_serial_data_error[0].data[i]);
                    sw.Write("[" + list_serial_data_error[0].time + "] " + str + Environment.NewLine);
                    sw.Flush();
                    sw.Close();
                    fs.Close();
                    list_serial_data_error.RemoveAt(0);
                }
                catch (Exception ex) { }
            }
            if (class_MP_dongle.Flag_is_auto_connect_list_show)
            {
                class_MP_dongle.Flag_is_auto_connect_list_show = false;
                if (tabControl1.SelectedIndex == 1)
                {
                    list_auto_connect.Items.Clear();
                    for (int i = 0; i < class_MP_dongle.Get_Auto_connect_list.Count; i++)
                    {
                        list_auto_connect.Items.Add(class_MP_dongle.Get_Auto_connect_list[i].str_device_mac);
                    }
                }
                else
                {
                    listView_SettingAutoList.Items.Clear();
                    class_MP_dongle.Set_Auto_connect_list_info.Clear();
                    for (int i = 0; i < class_MP_dongle.Get_Auto_connect_list.Count; i++)
                    {
                        ListViewItem x = new ListViewItem("");
                        ListViewItem.ListViewSubItem sub_i1 = new ListViewItem.ListViewSubItem();
                        ListViewItem.ListViewSubItem sub_i2 = new ListViewItem.ListViewSubItem();
                        sub_i1.Text = class_MP_dongle.Get_Auto_connect_list[i].str_device_mac;
                        sub_i2.Text = "Save";
                        x.SubItems.Add(sub_i1);
                        x.SubItems.Add(sub_i2);
                        listView_SettingAutoList.Items.Add(x);
                        class_MP_dongle.Get_Auto_connect_list[i].str_device_name = "Sava";
                        class_MP_dongle.Set_Auto_connect_list_info.Add(class_MP_dongle.Get_Auto_connect_list[i]);
                    }

                }
            }
            if (class_MP_dongle.Flag_get_version)
            {
                class_MP_dongle.Flag_get_version = false;
                this.Text = "BLE_gateway_auto" + " Version：" + class_MP_dongle.str_version;
            }
        }

        private void list_scan_device_SelectedIndexChanged(object sender, EventArgs e)
        {
            byte[] peer = new byte[6];
            try
            {
                string[] str = class_MP_dongle.ble_device_mac[list_scan_device.SelectedIndex].Split(':');
                for (int i = 0; i < str.Length; i++)
                {
                    peer[i] = (byte)Convert.ToInt32(str[i], 16);
                }
                class_MP_dongle.select_scan_device = peer;
                btn_connect.Enabled = true;
            }
            catch (Exception ex)
            { }
        }

        private void list_connected_device_SelectedIndexChanged(object sender, EventArgs e)
        {
            select_connected_device(list_connected_device.SelectedIndex);
        }

        private void btn_connect_Click(object sender, EventArgs e)
        {
            byte[] data;
            if (serialPort1.IsOpen)
            {
                data = class_MP_dongle.connect();
                serial_write_data(data);
            }
        }

        private void btn_disconnect_Click(object sender, EventArgs e)
        {
            byte[] data;
            if (serialPort1.IsOpen)
            {
                data = class_MP_dongle.disconnect();
                serial_write_data(data);
            }
        }

        private void btn_scan_Click(object sender, EventArgs e)
        {
            byte[] data;
            if (serialPort1.IsOpen == false) return;
            if (btn_scan.Text == SCAN_START)
            {
                class_MP_dongle.ble_device.Clear();
                class_MP_dongle.ble_device_mac.Clear();
                class_MP_dongle.Flag_is_change_device_show = true;
                class_MP_dongle.Flag_set_auto_list_scan = false;
                list_scan_device.Items.Clear();
                data = class_MP_dongle.scan_start((byte)MP_dongle.SCAN_TYPE.Scan_Mode);
                serial_write_data(data);
                btn_scan.Text = SCAN_STOP;
            }
            else
            {
                data = class_MP_dongle.scan_stop();
                serial_write_data(data);
                btn_scan.Text = SCAN_START;
            }
        }

        private void txt_log_TextChanged(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;
            try
            {
                txt.SelectionLength = txt.Text.Length;
                txt.ScrollToCaret();

                if (txt.Lines.Length > 400)
                    txt.Text = "";
            }
            catch (Exception ex)
            { }
        }

        private void btn_send_Click(object sender, EventArgs e)
        {
            send_ble_data(txt_send_data.Text);
        }

        private void btn_UUID_set_Click(object sender, EventArgs e)
        {
            char[] char_send = txt_UUID.Text.ToCharArray();
            byte[] data_payload = new byte[char_send.Length / 2];
            if (char_send.Length % 2 != 0 || char_send.Length == 0) return;
            for (int i = 0; i < (char_send.Length / 2); i++)
            {
                string str = char_send[2 * i].ToString() + char_send[2 * i + 1].ToString();
                data_payload[data_payload.Length - i - 1] = Convert.ToByte(str, 16);
            }
            byte[] data = class_MP_dongle.set_UUID(data_payload);
            serial_write_data(data);
        }

        private void btn_UUID_get_Click(object sender, EventArgs e)
        {
            byte[] data = class_MP_dongle.get_UUID();
            serial_write_data(data);
        }

        private void btn_write_listview_Click(object sender, EventArgs e)
        {
            ListViewItem i = new ListViewItem(txt_detail.Text);
            ListViewItem.ListViewSubItem sub_i1 = new ListViewItem.ListViewSubItem();
            ListViewItem.ListViewSubItem sub_i2 = new ListViewItem.ListViewSubItem();
            sub_i1.Text = txt_CMD.Text;
            sub_i2.Text = txt_delay.Text;
            i.SubItems.Add(sub_i1);
            i.SubItems.Add(sub_i2);
            listView1.Items.Add(i);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            int size = listView1.Items.Count;
            inifile.IniWriteValue("Count", "Count", size.ToString());
            for (int i = 0; i < size; i++)
            {
                inifile.IniWriteValue(i.ToString(), "Name", listView1.Items[i].SubItems[0].Text);
                inifile.IniWriteValue(i.ToString(), "CMD", listView1.Items[i].SubItems[1].Text);
                inifile.IniWriteValue(i.ToString(), "Delay", listView1.Items[i].SubItems[2].Text);
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            int a = 0;
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                if (listView1.Items[i].Selected == true)
                {
                    send_ble_data(listView1.Items[i].SubItems[1].Text);
                }
            }
        }

        private void btn_remove_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                if (listView1.Items[i].Selected == true)
                {
                    listView1.Items[i].Remove();
                }
            }
        }

        private void btn_send_data_all_Click(object sender, EventArgs e)
        {
            if (btn_send_data_all.Text == "送出")
            {
                index_send_ble_device = 0;
                if (checkBox1.Checked == true)
                {
                    flag_send_ble_data_auto = true;
                    index_send_ble_list = 0;
                    str_send_ble_data_list = listView1.Items[index_send_ble_list].SubItems[1].Text;
                    try
                    {
                        delay_send_ble_data = Int32.Parse(listView1.Items[index_send_ble_list].SubItems[2].Text);
                    }
                    catch (Exception ex)
                    {
                        delay_send_ble_data = 1000;
                    }
                    flag_send_ble_data = true;
                    timer_send_data.Interval = delay_send_ble_data;
                    timer_send_data.Enabled = true;
                    btn_send_data_all.Text = "暫停";
                }
                else
                {
                    flag_send_ble_data_auto = false;
                    flag_send_ble_data = true;
                    timer_send_data.Interval = delay_send_ble_data;
                    timer_send_data.Enabled = true;
                    btn_send_data_all.Text = "暫停";
                }
            }
            else
            {
                flag_send_ble_data = false;
                flag_send_ble_data_auto = false;
                timer_send_data.Enabled = false;
                btn_send_data_all.Text = "送出";
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                if (listView1.Items[i].Selected == true)
                {
                    str_send_ble_data_list = listView1.Items[i].SubItems[1].Text;
                    try
                    {
                        delay_send_ble_data = Int32.Parse(listView1.Items[i].SubItems[2].Text);
                    }
                    catch (Exception ex)
                    {
                        delay_send_ble_data = 100;
                    }
                }
            }
        }

        private void timer_send_data_Tick(object sender, EventArgs e)
        {
            if (flag_send_ble_data == false)
            {
                timer_send_data.Enabled = false;
                return;
            }
            if (index_send_ble_device < list_connected_device.Items.Count)
            {
                select_connected_device(index_send_ble_device);
                send_ble_data(str_send_ble_data_list);
                index_send_ble_device++;
            }
            else
            {
                if (flag_send_ble_data_auto == true)
                {
                    index_send_ble_device = 0;
                    index_send_ble_list++;
                    index_send_ble_list %= listView1.Items.Count;
                    str_send_ble_data_list = listView1.Items[index_send_ble_list].SubItems[1].Text;
                    try
                    {
                        delay_send_ble_data = Int32.Parse(listView1.Items[index_send_ble_list].SubItems[2].Text);
                    }
                    catch (Exception ex)
                    {
                        delay_send_ble_data = 1000;
                    }
                    timer_send_data.Interval = delay_send_ble_data;

                }
                else
                    timer_send_data.Enabled = false;

            }
        }

        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            Rectangle tabArea = tabControl1.GetTabRect(e.Index);//主要是做个转换来获得TAB项的RECTANGELF 
            RectangleF tabTextArea = (RectangleF)(tabControl1.GetTabRect(e.Index));
            Graphics g = e.Graphics;
            StringFormat sf = new StringFormat();//封装文本布局信息 
            //Brush bshBack;
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;
            Font font = this.tabControl1.Font;
            //bshBack = new SolidBrush(Color.White);
            SolidBrush brush = new SolidBrush(Color.Blue);//绘制边框的画笔 
            //g.FillRectangle(bshBack, ((TabControl)sender).GetTabRect(e.Index));
            g.DrawString(((TabControl)(sender)).TabPages[e.Index].Text, font, brush, tabTextArea, sf);
            if (tabControl1.SelectedIndex == e.Index)
            {
                //bshBack = new SolidBrush(Color.Purple);
                brush = new SolidBrush(Color.Red);//绘制边框的画笔 
                //g.FillRectangle(bshBack, ((TabControl)sender).GetTabRect(e.Index));
                g.DrawString(((TabControl)(sender)).TabPages[e.Index].Text, font, brush, tabTextArea, sf);
            }
            tabControl1.TabPages[e.Index].BackColor = Color.White;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (tabControl1.SelectedIndex == 1 || tabControl1.SelectedIndex == 3)
            {
                if (tabControl1.SelectedIndex == 3)
                {
                    serial_write_data(class_MP_dongle.scan_stop());
                    Thread.Sleep(100);
                }
                serial_write_data(class_MP_dongle.get_auto_connect_list((byte)MP_dongle.GET_LIST.First_Get));
            }
            if (tabControl1.SelectedIndex == 2)
            {
                listView_SettingAutoList.Items.Clear();
                class_MP_dongle.Set_Auto_connect_list_info.Clear();
                serial_write_data(class_MP_dongle.get_connected_list((byte)MP_dongle.GET_LIST.First_Get));
            }
        }

        private void btn_SettingAutoList_add_Click(object sender, EventArgs e)
        {
            string str_data = txt_SettingAutoList_add.Text;
            char[] char_send = str_data.ToCharArray();
            byte[] data_payload = new byte[char_send.Length / 2];

            if (char_send.Length % 2 != 0 || str_data.Length != 12)
            {
                txt_log.Text += "ERROR_ADD" + Environment.NewLine;
                return;
            }
            str_data = "";
            for (int i = 0; i < (char_send.Length / 2); i++)
            {
                string str = char_send[2 * i].ToString() + char_send[2 * i + 1].ToString();
                try
                {
                    data_payload[i] = Convert.ToByte(str, 16);
                }
                catch (Exception ex)
                {
                    txt_log.Text += "ERROR_ADD" + Environment.NewLine;
                    return;
                }
                str_data += String.Format("{0:X2}", data_payload[i]);
                if (i != 5) str_data += ":";
            }

            device_info info = new device_info();
            info.peer = data_payload;
            class_MP_dongle.Flag_is_change_device_show = true;
            class_MP_dongle.ble_device_mac.Add(str_data);
            info.str_device_name = "Add";
            info.str_device_mac = str_data;
            class_MP_dongle.Set_Auto_connect_list_info.Add(info);
        }

        private void timer_get_list_Tick(object sender, EventArgs e)
        {
            if (class_MP_dongle.Flag_get_auto_connect_list)
            {
                class_MP_dongle.Flag_get_auto_connect_list = false;
                serial_write_data(class_MP_dongle.get_auto_connect_list((byte)MP_dongle.GET_LIST.Continue));
            }
            if (class_MP_dongle.Flag_get_connected_list)
            {
                class_MP_dongle.Flag_get_connected_list = false;
                serial_write_data(class_MP_dongle.get_connected_list((byte)MP_dongle.GET_LIST.Continue));
            }
            if (class_MP_dongle.Flag_set_auto_list)
            {
                class_MP_dongle.Flag_set_auto_list = false;
                serial_write_data(class_MP_dongle.set_auto_connected_list((byte)MP_dongle.SET_LIST.Continue));
            }
        }

        private void btn_ConnectedDevice_refrash_Click(object sender, EventArgs e)
        {
            serial_write_data(class_MP_dongle.get_connected_list((byte)MP_dongle.GET_LIST.First_Get));
        }

        private void btn_auto_connect_Click(object sender, EventArgs e)
        {
            serial_write_data(class_MP_dongle.scan_start((byte)MP_dongle.SCAN_TYPE.Auto_Mode));
            btn_scan.Text = SCAN_START;
        }

        private void btn_SettingAutoList_scan_Click(object sender, EventArgs e)
        {
            //btn_scan.Text = SCAN_START;
            class_MP_dongle.ble_device.Clear();
            class_MP_dongle.ble_device_mac.Clear();
            class_MP_dongle.Flag_is_change_device_show = false;
            class_MP_dongle.Flag_set_auto_list_scan = true;
            list_scan_device.Items.Clear();
            serial_write_data(class_MP_dongle.scan_start((byte)MP_dongle.SCAN_TYPE.Scan_Mode));
        }

        private void listView_SettingAutoList_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < listView_SettingAutoList.Items.Count; i++)
            {
                if (listView_SettingAutoList.Items[i].Selected == true)
                {
                    listView_SettingAutoList.Items[i].Checked = !listView_SettingAutoList.Items[i].Checked;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            class_MP_dongle.Set_Auto_connect_list.Clear();
            for (int i = 0; i < listView_SettingAutoList.Items.Count; i++)
            {
                if (listView_SettingAutoList.Items[i].Checked)
                {
                    class_MP_dongle.Set_Auto_connect_list.Add(class_MP_dongle.Set_Auto_connect_list_info[i]);
                }
            }
            serial_write_data(class_MP_dongle.set_auto_connected_list((byte)MP_dongle.SET_LIST.First_Set));

        }

        private void button2_Click(object sender, EventArgs e)
        {
            serial_write_data(class_MP_dongle.reset());
        }

        private void btn_bin_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "BIN(*.BIN)|*.bin|" + "所有檔案|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                BinData = System.IO.File.ReadAllBytes(openFileDialog.FileName);
                lab_bin.Text = openFileDialog.FileName;
            }
        }

        private void btn_update_Click(object sender, EventArgs e)
        {
            serial_write_data(class_MP_dongle.scan_stop());
            Thread.Sleep(100);

            //class_MP_dongle.select_connect_device
        }
    }
}
